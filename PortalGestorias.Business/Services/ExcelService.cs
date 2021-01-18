using PortalGestorias.Infrastructure.Data;
using System;
using System.Linq;
using PortalGestorias.Domain.Entities;
using System.Linq.Expressions;
using System.Data;
using System.Reflection;
using OfficeOpenXml;
using System.IO;
using System.Collections.Generic;
using Serilog;
using OfficeOpenXml.Style;
using OfficeOpenXml.Table;
using System.Text;
using System.Security.Cryptography;
using System.Configuration;
using System.Drawing;

namespace PortalGestorias.Business.Services
{
    public class ExcelService : IService
    {
        protected CrmDbContext Db;

        public ExcelService(CrmDbContext contexto = null)
        {
            Db = contexto ?? new CrmDbContext();
            Db.Configuration.AutoDetectChangesEnabled = false;
        }

        public bool ImportExcel<E>(Expression<Func<E, bool>> predicate = null)
            where E : BusinessEntity
        {
            var dbList = Db.Set<E>().AsQueryable();

            if (predicate != null)
            {
                dbList = dbList.Where(predicate);
            }
            dbList = dbList.Where(e => e.Activo == true);

            return true;
        }

        public byte[] ExportToExcel<T>(List<T> dbList) where T : BusinessEntity
        {
            try
            {
                MemoryStream ms = new MemoryStream();

                using (ExcelPackage excel = new ExcelPackage(ms))
                {
                    var ws = excel.Workbook.Worksheets.Add("Datos");

                    ws.Cells[1, 1].LoadFromCollectionFiltered(dbList);

                    int colNumber = 1;


                    PropertyInfo[] propertyInfos = typeof(T)
                            .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                            .Where(p => !Attribute.IsDefined(p, typeof(EpplusIgnore)))
                            .ToArray();

                    foreach (PropertyInfo propertyInfo in propertyInfos)
                    {

                        if (propertyInfo.PropertyType == typeof(DateTime) || propertyInfo.Name.Equals("FechaBaja") || propertyInfo.Name.Equals("FechaEntrega"))
                        {
                            ws.Column(colNumber).Style.Numberformat.Format = "dd/MM/yyyy";
                        }
                        colNumber++;
                    }

                    if (dbList.Count() == 0)
                    {
                        ws.Cells[2, 1].Value = String.Empty;
                    }
                    ws.Cells[ws.Dimension.Address].AutoFitColumns();

                    excel.SaveAs(ms);
                    excel.Dispose();

                    byte[] dt = ms.ToArray();

                    return dt;
                }
            }

            catch (Exception)
            {
                return null;
            }
        }

        public byte[] ExportToExcelStock<T>(List<Producto> dbList) where T : BusinessEntity
        {
            try
            {
                MemoryStream ms = new MemoryStream();

                using (ExcelPackage excel = new ExcelPackage(ms))
                {
                    var ws = excel.Workbook.Worksheets.Add("Datos");

                    ws.Cells[1, 1].LoadFromCollectionFiltered(dbList);

                    int colNumber = 1;
                   


                    PropertyInfo[] propertyInfos = typeof(T)
                            .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                            .Where(p => !Attribute.IsDefined(p, typeof(EpplusIgnore)))
                            .ToArray();

                    foreach (PropertyInfo propertyInfo in propertyInfos)
                    {

                        if (propertyInfo.PropertyType == typeof(DateTime) || propertyInfo.Name.Equals("FechaBaja") || propertyInfo.Name.Equals("FechaEntrega"))
                        {
                            ws.Column(colNumber).Style.Numberformat.Format = "MM/dd/yyyy";
                        }
                        colNumber++;
                    }

                    if (dbList.Count() == 0)
                    {
                        ws.Cells[2, 1].Value = String.Empty;
                    }
                    ws.Cells[ws.Dimension.Address].AutoFitColumns();

                    ws.InsertRow(ws.Dimension.End.Row + 2, 1);
                    int row = ws.Dimension.End.Row + 2;
                    decimal  totalStock = dbList.Sum(m => m.Modelo.Importe);
                    ws.Cells[row, 2].Style.Font.Size = 13;
                    ws.Cells[row, 2].Style.Font.Name = "Calibri";
                    ws.Cells[row, 2].Style.Font.Bold = true;
                    ws.Cells[row, 2].Style.Font.Color.SetColor(Color.DarkGreen);
                    ws.Cells[row, 2].Value = "TOTAL STOCK: " + totalStock.ToString() + "€";

                    excel.SaveAs(ms);
                    excel.Dispose();

                    byte[] dt = ms.ToArray();

                    return dt;
                }
            }

            catch (Exception)
            {
                return null;
            }
        }
        public bool ExportAssert(List<Producto> dbList)
        {
            string template = "HP Asset Manager.xlsx";
            FileInfo templateFile = new FileInfo(ConfigurationManager.AppSettings["TemplatePath"] + template);
            ExcelPackage pack = new ExcelPackage(templateFile, true);

            ExcelWorksheet hoja;

            hoja = pack.Workbook.Worksheets[1];
            int fila = 14;
            int columna = 1;

            foreach (Producto producto  in dbList)
            {
                columna = 1;
                hoja.Cells[fila, columna++].Value = producto.Departamento?.Codigo;
                hoja.Cells[fila, columna++].Value = producto.Departamento?.StockRoomString;
                hoja.Cells[fila, columna++].Value = producto.CodigoBarras;
                hoja.Cells[fila, columna++].Value = producto.NumeroSerie;
                hoja.Cells[fila, columna++].Value = producto.Marca;
                hoja.Cells[fila, columna++].Value = producto.Modelo;
                hoja.Cells[fila, columna++].Value = producto.Modelo?.Barcode;
                hoja.Cells[fila, columna++].Value = producto.Modelo?.TipoModelo;
                fila++;
                columna = 1;

            }
            //if (pack != null)
            {
                string name = null;
                FileInfo file = null;

                name = DateTime.Now.ToString("yyyyMMdd.hhmmss") + "." + "EntradaProductos" + ".xlsx";

                file = new FileInfo(ConfigurationManager.AppSettings["OutputPath"] + name);

                pack.SaveAs(file);
                pack.Dispose();
            }
            return true;
        }
    }

    public static class ExcelService_Extensions
    {
        public static string ByteArrayToString(byte[] arrInput)
        {
            int i;
            StringBuilder sOutput = new StringBuilder(arrInput.Length);
            for (i = 0; i < arrInput.Length - 1; i++)
            {
                sOutput.Append(arrInput[i].ToString("X2"));
            }
            return sOutput.ToString();
        }

        public static ExcelRangeBase LoadFromCollectionFiltered<T>(this ExcelRangeBase @this, IEnumerable<T> collection) where T : BusinessEntity
        {
            MemberInfo[] membersToInclude = typeof(T)
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(p => !Attribute.IsDefined(p, typeof(EpplusIgnore)))
                .ToArray();

            return @this.LoadFromCollection<T>(collection, true,
               TableStyles.Light20,
                BindingFlags.Instance | BindingFlags.Public,
                membersToInclude);
        }

    }
}
