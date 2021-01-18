using System;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EPPlus.DataExtractor;
using PortalGestorias.Business.Validations;
using PortalGestorias.Domain.Entities;
using PortalGestorias.Domain.Models;
using PortalGestorias.Business.Services;
using PortalGestorias.Infrastructure.Data;
using PortalGestorias.Web.Security;
using OfficeOpenXml;
using Thinktecture.IdentityModel.Mvc;
using System.Collections.Generic;
using System.Configuration;

namespace PortalGestorias.Web.Controllers
{
    public class MarcasController : BusinessController<Marca, BusquedaMarca>
    {
        protected string action;

        public MarcasController(CrmDbContext dbContext, IEntityValidator<Marca> validator)
            : base(dbContext, validator)
        {
            this.validator = validator;
            this.searchEntity = new EntitySearchBase<BusquedaMarca>(dbContext);

        }

        [ResourceAuthorize(DataOperation.Read, Controllers.Marcas)]
        public override ActionResult Index(int? page) => base.Index(page);

        [ResourceAuthorize(DataOperation.Read, Controllers.Marcas)]
        public override ActionResult Details(int? id, bool? listadoModelos, bool? listadoProductos) => base.Details(id, listadoModelos, listadoProductos);

        [ResourceAuthorize(DataOperation.Create, Controllers.Marcas)]
        public override ActionResult Create() => base.Create();

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ResourceAuthorize(DataOperation.Create, Controllers.Marcas)]
        public override ActionResult Create([Bind(Include = "Id,Nombre")]Marca model) => base.Create(model);

        [ResourceAuthorize(DataOperation.Update, Controllers.Marcas)]
        public override ActionResult Edit(int? id) => base.Edit(id);

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ResourceAuthorize(DataOperation.Update, Controllers.Marcas)]
        public override ActionResult Edit([Bind(Include = "Id,Nombre")]Marca model) => base.Edit(model);

        [ResourceAuthorize(DataOperation.Delete, Controllers.Marcas)]
        public override ActionResult Delete(int? id)
        {
            var entity = Db.Marcas.Find(id);

            var existe = Db.Productos.Any(m => m.Activo == true && m.Marca.Id == id);

            if (existe)
            {
                ViewBag.Message = "No se puede eliminar la marca " + entity.Nombre + ". Hay productos con esta Marca asignada.";
                base.Index(null);
                return View("Index");
            }

            existe = Db.Modelos.Any(m => m.Activo == true && m.Marca.Id == id);

            if (existe)
            {
                ViewBag.Message = "No se puede eliminar la marca " + entity.Nombre + ". Hay modelos con esta Marca asignada.";
                base.Index(null);
                return View("Index");
            }

            return base.Delete(id);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [ResourceAuthorize(DataOperation.Delete, Controllers.Marcas)]
        public override ActionResult DeleteConfirmed(int id) => base.DeleteConfirmed(id);
    
        [ResourceAuthorize(DataOperation.Read, Controllers.Marcas)]
        public ActionResult Search(BusquedaMarca model, int? page)
        {
            var skip = (page - 1) * base.RowsPerPage ?? 0;

            var list = Db.Marcas.AsQueryable().Where(a => a.Activo == true);
            ViewBag.TotalRowCount = list.Count();

            if (list == null)
            {
                return HttpNotFound();
            }

            if (!string.IsNullOrWhiteSpace(model.Nombre))
            {
                list = list.Where(e => e.Nombre.Contains(model.Nombre));
            }

            list = list.OrderBy(e => e.Nombre);


            ViewBag.SearchCount = list.Count();
            ViewBag.SearchEntity = model;


            if (list.Count() == 1)
            {
                var entidad = Db.Set<Marca>().Find(list.FirstOrDefault().Id);

                if (entidad == null)
                {
                    return HttpNotFound();
                }

                ViewBag.ViewOnly = true;
                FillCombos(entidad);
                return RedirectToAction("Details", new { id = entidad.Id });
            }

            FillCombos();
            return View("Index", list.Skip(skip).Take(base.RowsPerPage).ToList());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ResourceAuthorize(DataOperation.Read, Controllers.Marcas)]
        public ActionResult ImportExcel(HttpPostedFileBase fileImport) 
        {
           
            if (fileImport != null && fileImport.ContentLength > 0 && (Path.GetExtension(fileImport.FileName) == ".xlsx" || Path.GetExtension(fileImport.FileName) == ".xls"))
            {
                int update = 0;
                int fail = 0;
                int count = 2;
                string lines = String.Empty;

                using (var excel = new ExcelPackage(fileImport.InputStream))
                {
                    var wsFile= excel.Workbook.Worksheets.First();

                    var marcas = wsFile.Extract<Marca>()

                       .WithProperty(p => p.Nombre, "A").
                         GetData(2, wsFile.Dimension.Rows).ToList();

                    foreach (var marca in marcas)
                    {
                        if (marca.Nombre != null)
                        {
                            var exists = Db.Marcas.Where(m => m.Nombre.ToUpper().Equals(marca.Nombre.ToUpper()) && m.Activo == true).FirstOrDefault();
                            if (exists == null)
                            {
                                var result = validator.Validate(marca);
                                if (result.Valid)
                                {
                                    Db.Marcas.Add(marca);
                                    Db.SaveChanges();
                                    update++;
                                }
                                else
                                {
                                    lines += "FILA:" + count.ToString() + " *** ERROR: ";

                                    foreach (var resulterror in result.ValidationErrors)
                                    {
                                        lines += resulterror.Value + " ";
                                    }
                                    lines += Environment.NewLine;
                                    fail++;
                                }
                            }

                            else
                            {
                                fail++;
                                lines += "FILA:" + count.ToString() + " *** ERROR: YA EXISTE LA MARCA";
                                lines += Environment.NewLine;
                            }
                        }

                        count++;
                    }
                    
                    ViewBag.Message = String.Format("Registros Leidos: {0}", update + fail);
                    ViewBag.RegistrosActualizados = String.Format("Registros Actualizados: {0}", update);
                    ViewBag.RegistrosFallidos = String.Format("Registros Fallidos: {0}", fail);

                    var fileName = "ErrorAltaMarcas_" + DateTime.Now.Ticks.ToString() + ".txt";

                    string fileErrorName = Server.MapPath("~") + ConfigurationManager.AppSettings["CarpetaLogs"] + "\\" + fileName;
                    string cast = lines.Replace("no puede estar vacío", "NO EXISTE").ToUpper();
                    System.IO.File.WriteAllText(fileErrorName, cast);
                    ViewBag.Path = ConfigurationManager.AppSettings["UrlLogs"] + fileName;
                }
            }

            else
            {
                ViewBag.Message = "Debe seleccionar un fichero válido";
            }

            base.Index(null);
            return View("Index");
        }


        [ResourceAuthorize(DataOperation.Read, Controllers.Marcas)]
        public void ExportExcel()
        {
            ExcelService excelService = new ExcelService(); 
            List<Marca> dbList  = Db.Marcas.AsQueryable().Where(m => m.Activo == true).ToList();
            byte[] dt = excelService.ExportToExcel<Marca>(dbList);

            string fileName = String.Empty;

            fileName += "Marcas_" + DateTime.Now.ToString("yyyyMMdd") + ".xlsx";
            Response.AddHeader("content-disposition", "attachment; filename=" + fileName);
            Response.ContentType = "application/vnd.ms-excel";
            Response.BinaryWrite(dt);
            Response.End();
        }
    }
}