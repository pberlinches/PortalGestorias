using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EPPlus.DataExtractor;
using PortalGestorias.Business.Services;
using PortalGestorias.Business.Validations;
using PortalGestorias.Domain.Entities;
using PortalGestorias.Domain.Models;
using PortalGestorias.Infrastructure.Data;
using PortalGestorias.Web.Security;
using OfficeOpenXml;
using Thinktecture.IdentityModel.Mvc;


namespace PortalGestorias.Web.Controllers
{
    public class ModelosController : BusinessController<Modelo, BusquedaModelo>
    {
        protected string action;


        public ModelosController(CrmDbContext dbContext, IEntityValidator<Modelo> validator)
             : base(dbContext, validator)
        {
            this.validator = validator;
            this.searchEntity = new EntitySearchBase<BusquedaModelo>(dbContext);
        }

        [ResourceAuthorize(DataOperation.Read, Controllers.Modelos)]
        public override ActionResult Index(int? page) => base.Index(page);


        [ResourceAuthorize(DataOperation.Read, Controllers.Modelos)]
        public override ActionResult Details(int? id, bool? listadoModelos, bool? listadoProductos) => base.Details(id, listadoModelos, listadoProductos);

        [HttpGet, ActionName("Create")]
        [ResourceAuthorize(DataOperation.Create, Controllers.Modelos)]
        public  ActionResult Create(int? idAlmacen, int? idTipo, int? idStockRoom, int? idMarca, int? idUbicacion, bool? redirectToProductos)
        {
            Modelo entity = BuildDefaultEntity();

            if (idMarca != null)
            {
                entity.Marca = Db.Marcas.Find(idMarca);
            }

            if (idTipo != null)
            {
                entity.TipoModelo = Db.TiposModelos.Find(idTipo);
            }

            ViewBag.idAlmacen = idAlmacen;
            ViewBag.idTipo = idTipo;
            ViewBag.idUbicacion = idUbicacion;
            ViewBag.idStockRoom = idStockRoom;
            ViewBag.redirectToProductos  = redirectToProductos;

            FillCombos(entity);
            ViewBag.Titulo = "Alta";
            return View("Edit", entity);
        }

        [HttpPost, ActionName("Create")]
        [ValidateAntiForgeryToken]
        [ResourceAuthorize(DataOperation.Create, Controllers.Modelos)]
        public ActionResult Create(int? idMarca, int? idTipoModelo, int? idAlmacen, int? idStockRoom, int? idUbicacion,  bool? redirectToProductos, [Bind(Include = "Id,Nombre,Barcode,Importe")]Modelo model)
        {
            if (idMarca != null)
                model.Marca = Db.Marcas.Find(idMarca);

            if (idTipoModelo != null)
                model.TipoModelo = Db.TiposModelos.Find(idTipoModelo);

            if (ModelState.IsValid)
            {
                var navResult = NavigationPropertiesValidation(model);

                if (navResult != null) return navResult;

                var result = validator.Validate(model);
                if (result.Valid)
                {
                    Db.Modelos.Add(model);
                    Db.SaveChanges();

                    if (redirectToProductos != null)
                        return RedirectToAction("Create", "Productos", new { idAlmacen = idAlmacen , idMarca = idMarca, idTipoModelo = idTipoModelo, idStockRoom = idStockRoom, idUbicacion = idUbicacion, idModelo = model.Id});
                    else
                        return RedirectToAction("Index", RouteValues);
                }
                result.ValidationErrors.ToList().ForEach(v => ModelState.AddModelError(v.Key, v.Value));
            }

            FillCombos(model);
            ViewBag.Titulo = "Alta";
            return View("Edit", model);
        }

        [HttpGet, ActionName("Edit")]
        [ResourceAuthorize(DataOperation.Update, Controllers.Modelos)]
        public override ActionResult Edit(int? id) => base.Edit(id);

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ResourceAuthorize(DataOperation.Update, Controllers.Modelos)]
        public ActionResult Edit(int? idMarca, int? idTipoModelo, [Bind(Include = "Id,Nombre,Barcode,Importe")]Modelo model)
        {
            if (idMarca != null)
                model.Marca = Db.Marcas.Find(idMarca);

            if (idTipoModelo != null)
                model.TipoModelo = Db.TiposModelos.Find(idTipoModelo);

            return base.Edit(model);
        }


        [ResourceAuthorize(DataOperation.Delete, Controllers.Modelos)]
        public override ActionResult Delete(int? id)
        {
            var entity = Db.Modelos.Find(id);

            var existe = Db.Productos.Any(m => m.Activo == true && m.Modelo.Id == id);

            if (existe)
            {
                ViewBag.Message = "No se puede eliminar el modelo " + entity.Nombre + ". Hay productos con este Modelo asignado.";
                base.Index(null);
                return View("Index");
            }

            return base.Delete(id);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [ResourceAuthorize(DataOperation.Delete, Controllers.Modelos)]
        public override ActionResult DeleteConfirmed(int id) => base.DeleteConfirmed(id);

        [ResourceAuthorize(DataOperation.Read, Controllers.Modelos)]
        public ActionResult Search(int? idMarca, int? idTipoModelo, BusquedaModelo model, int? page)
        {
            var skip = (page - 1) * base.RowsPerPage ?? 0;

            var list = Db.Modelos.AsQueryable().Where(a => a.Activo == true);
            ViewBag.TotalRowCount = list.Count();

            if (list == null)
            {
                return HttpNotFound();
            }

            if (idMarca.HasValue)
            {
                list = list.Where(e => e.Marca.Id == idMarca);
            }

            if (idTipoModelo.HasValue)
            {
                list = list.Where(e => e.TipoModelo.Id == idTipoModelo);
            }

            if (!string.IsNullOrWhiteSpace(model.Nombre))
            {
                list = list.Where(e => e.Nombre.Contains(model.Nombre));
            }

            if (model.ImporteDesde != null)
            {
                list = list.Where(e => e.Importe >= model.ImporteDesde);
            }

            if (model.ImporteHasta != null)
            {
                list = list.Where(e => e.Importe <=  model.ImporteHasta);
            }

            list = list.OrderBy(e => e.Nombre);

            ViewBag.SearchCount = list.Count();
            ViewBag.SearchEntity = model;

            FillCombos();
            return View("Index", list.Skip(skip).Take(base.RowsPerPage).ToList());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ResourceAuthorize(DataOperation.Read, Controllers.Modelos)]
        public ActionResult ImportExcel(HttpPostedFileBase fileImport)
        {

            try
            {
                if (fileImport != null && fileImport.ContentLength > 0 && (Path.GetExtension(fileImport.FileName) == ".xlsx" || Path.GetExtension(fileImport.FileName) == ".xls"))
                {
                    int update = 0;
                    int fail = 0;
                    int count = 2;
                    string lines = String.Empty;

                    using (var excel = new ExcelPackage(fileImport.InputStream))
                    {
                        var wsFile = excel.Workbook.Worksheets.First();

                        var entities = wsFile.Extract<Modelo>()

                           .WithProperty(p => p.Nombre, "A")
                           .WithProperty(p => p.Marca, "B", getMarca)
                           .WithProperty(p => p.TipoModelo, "C", getTipo)
                           .WithProperty(p => p.Importe, "D")
                           .WithProperty(p => p.Barcode, "E").
                             GetData(2, wsFile.Dimension.Rows).ToList();

                        foreach (var entity in entities)
                        {
                            if (entity.Nombre != null)
                            {
                                if (entity.Marca != null)
                                {
                                    var exists = Db.Modelos.Where(m => m.Nombre.ToUpper().Equals(entity.Nombre.ToUpper()) && m.Activo == true).FirstOrDefault();
                                    if (exists == null)
                                    {
                                        var result = validator.Validate(entity);
                                        if (result.Valid)
                                        {
                                            var modelo = new Modelo();
                                            modelo.Marca = Db.Marcas.Find(entity.Marca.Id);
                                            modelo.TipoModelo = Db.TiposModelos.Find(entity.TipoModelo.Id);
                                            modelo.Importe = entity.Importe;
                                            modelo.Nombre = entity.Nombre;
                                            modelo.Barcode = entity.Barcode;
                                            Db.Modelos.Add(modelo);
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
                                        lines += "FILA:" + count.ToString() + " *** ERROR: YA EXISTE EL MODELO";
                                        lines += Environment.NewLine;
                                    }
                                }
                                else
                                {
                                    fail++;
                                    lines += "FILA:" + count.ToString() + " *** ERROR: NO EXISTE LA MARCA";
                                    lines += Environment.NewLine;
                                }
                            }
                            count++;
                        }

                        ViewBag.Message = String.Format("Registros Leidos: {0}", update + fail);
                        ViewBag.RegistrosActualizados = String.Format("Registros Actualizados: {0}", update);
                        ViewBag.RegistrosFallidos = String.Format("Registros Fallidos: {0}", fail);

                        var fileName = "ErrorAltaModelos_" + DateTime.Now.Ticks.ToString() + ".txt";

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
            }

            catch (Exception e)
            {
               
            }

            base.Index(null);
            return View("Index");
        }


        Func<object, Marca> getMarca= objStr =>
        {
            if (objStr == null)
                return null;

            var Marca = new CrmDbContext().Marcas.Where(m => m.Nombre.Equals(objStr.ToString())).FirstOrDefault();
            return Marca;

        };

        Func<object, TipoModelo> getTipo= objStr =>
        {
            if (objStr == null)
                return null;

            var Tipo = new CrmDbContext().TiposModelos.Where(m => m.Nombre.Equals(objStr.ToString())).FirstOrDefault();
            return Tipo;

        };

        [ResourceAuthorize(DataOperation.Read, Controllers.Modelos)]
        public void ExportExcel()
        {
            ExcelService excelService = new ExcelService();
            List<Modelo> dbList = Db.Modelos.AsQueryable().Where(m => m.Activo == true).ToList();
            byte[] dt = excelService.ExportToExcel<Modelo>(dbList);

            string fileName = String.Empty;

            fileName += "Modelos_" + DateTime.Now.ToString("yyyyMMdd") + ".xlsx";
            Response.AddHeader("content-disposition", "attachment; filename=" + fileName);
            Response.ContentType = "application/vnd.ms-excel";
            Response.BinaryWrite(dt);
            Response.End();
        }

protected override void FillCombos(Modelo entidad = null)
        {
            base.FillCombos(entidad);
            if (entidad != null)
            {
                if (entidad.Marca != null)
                {
                    PutEntityInViewBag<Marca>(entidad.Marca.GetId(), true);
                }
                else
                {
                    PutEntityInViewBag<Marca>(null, true);
                }

                if (entidad.TipoModelo != null)
                {
                    PutEntityInViewBag<TipoModelo>(entidad.TipoModelo.GetId(), true);
                }
                else
                {
                    PutEntityInViewBag<TipoModelo>(null, true);
                }
            }

            else
            {
                PutEntityInViewBag<Marca>(null, true);
                PutEntityInViewBag<TipoModelo>(null, true);
            }
        }
    }
}