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
    public class ProductosController : BusinessController<Producto, BusquedaProducto>
    {
        protected string action;
        protected List<EntradaProducto> altaEntity;
        protected List<SalidaProducto> salidaEntity;
        protected MailService mailservice;
        private static CrmDbContext  contextDefault;

        public ProductosController(CrmDbContext dbContext, IEntityValidator<Producto> validator, MailService mail)
             : base(dbContext, validator)
        {
            this.validator = validator;
            searchEntity = new EntitySearchBase<BusquedaProducto>(dbContext);
            altaEntity = new List<EntradaProducto>();
            salidaEntity = new List<SalidaProducto>();
            contextDefault = new CrmDbContext();
            mailservice = mail;
        }

        [ResourceAuthorize(DataOperation.Read, Controllers.Productos)]
        public override ActionResult Index(int? page) => base.Index(page);

        [ResourceAuthorize(DataOperation.Read, Controllers.Productos)]
        public override ActionResult Details(int? id, bool? listadoModelos, bool? listadoProductos) => base.Details(id, listadoModelos, listadoProductos);

        [HttpGet, ActionName("Create")]
        [ResourceAuthorize(DataOperation.Create, Controllers.Productos)]
        public ActionResult Create(int? idAlmacen, int? idTipoModelo, int? idStockRoom, int? idMarca, int? idModelo, int? idUbicacion, int? Cantidad, string NumeroPedido)
        {
            if (TempData["ViewData"] != null)
            {
                ViewData = (ViewDataDictionary)TempData["ViewData"];
            }

            var productos = TempData["productos"];
            var producto = TempData["producto"];

            if (productos != null)
            {
                altaEntity = (List<EntradaProducto>)productos;
                var model = (Producto)TempData["producto"];
                model.Cantidad = altaEntity.Count();
                FillCombos(model);

                var modelo = new Producto();
                modelo = model;
                return View("Alta", modelo);
            }

            Producto defaultEntity = new Producto();

            if (idAlmacen != null)
                defaultEntity.Almacen = Db.Almacenes.Find(idAlmacen);

            if (idStockRoom != null)
                defaultEntity.Departamento = Db.StockRooms.Find(idStockRoom);

            if (idMarca != null)
                defaultEntity.Marca = Db.Marcas.Find(idMarca);

            if (defaultEntity != null)
                defaultEntity.Modelo = Db.Modelos.Find(idModelo);

            if (idUbicacion != null)
                defaultEntity.Ubicacion = Db.Ubicaciones.Find(idUbicacion);

            if (idModelo != null)
                defaultEntity.Modelo = Db.Modelos.Find(idModelo);

            if (idTipoModelo != null)
                defaultEntity.defaultTipoModelo = Db.TiposModelos.Find(idTipoModelo).Nombre;

            defaultEntity.Cantidad = Cantidad ?? 0;


            FillCombos(defaultEntity);
            ViewBag.Titulo = "Alta";
            return View("Alta", defaultEntity);
        }

        [HttpPost, ActionName("Create")]
        [ValidateAntiForgeryToken]
        [ResourceAuthorize(DataOperation.Create, Controllers.Productos)]
        public ActionResult Create(int? idAlmacen, int? idTipoModelo, int? idStockRoom, int? idMarca, int? idModelo, int? idUbicacion, [Bind(Include = "Id,NumeroPedido,Cantidad")]Producto model)
        {
            if (idAlmacen != null)
                model.Almacen = Db.Almacenes.Find(idAlmacen);

            if (idStockRoom != null)
                model.Departamento = Db.StockRooms.Find(idStockRoom);

            if (idMarca != null)
                model.Marca = Db.Marcas.Find(idMarca);

            if (idModelo != null)
                model.Modelo = Db.Modelos.Find(idModelo);

            if (idUbicacion != null)
                model.Ubicacion = Db.Ubicaciones.Find(idUbicacion);

            model.Estado = Db.Estados.Where(m => m.Nombre.Contains("En Stock")).FirstOrDefault();

            model.UsuarioAlta = Db.Empleados.Find(UserId);
            model.FechaEntrada = DateTime.Now;
            return base.Create(model);
        }

        [HttpPost, ActionName("Alta")]
        [ValidateAntiForgeryToken]
        [ResourceAuthorize(DataOperation.Create, Controllers.Productos)]
        public ActionResult Alta(int? idAlmacen, int? idTipoModelo, int? idStockRoom, int? idMarca, int? idModelo, int? idUbicacion, [Bind(Include = "Id,NumeroPedido,Cantidad")]Producto model)
        {
            if (idAlmacen != null)
                model.Almacen = Db.Almacenes.Find(idAlmacen);

            if (idStockRoom != null)
                model.Departamento = Db.StockRooms.Find(idStockRoom);

            if (idMarca != null)
                model.Marca = Db.Marcas.Find(idMarca);

            if (idModelo != null)
            {
                model.Modelo = Db.Modelos.Find(idModelo);
            }

            if (idUbicacion != null)
                model.Ubicacion = Db.Ubicaciones.Find(idUbicacion);

            model.Estado = Db.Estados.Where(m => m.Nombre.Contains("En Stock")).FirstOrDefault();

            model.UsuarioAlta = Db.Empleados.Find(UserId);
            model.FechaEntrada = DateTime.Now;

            for (int i = 0; i < model.Cantidad; i++)
            {
                var entrada = new EntradaProducto();
                entrada.Almacen = model.Almacen;
                entrada.Marca = model.Marca;
                entrada.Modelo = model.Modelo;
                entrada.TipoModelo = entrada.Modelo.TipoModelo;
                entrada.StockRoom = model.Departamento;
                entrada.Ubicacion = model.Ubicacion;
                entrada.NumeroPedido = model.NumeroPedido;
                altaEntity.Add(entrada);
            }

            FillCombos(model);

            ViewBag.Almacenes = Db.Almacenes.AsQueryable().Select(almacen => new SelectListItem
            {
                Value = almacen.Id.ToString(),
                Text = almacen.Nombre,
                Selected = false,
            });
            ViewBag.Titulo = "Alta";

            return View("Alta", model);
        }
        
        [HttpGet, ActionName("Edit")]
        [ResourceAuthorize(DataOperation.Update, Controllers.Productos)]
        public ActionResult Edit(int? id, int? idMarca)
        {
            ModelState.Clear();
            var entidad = Db.Productos.Find(id);

            if (entidad == null)
            {
                return HttpNotFound();
            }

            if (idMarca != null)

                entidad.Marca = Db.Marcas.Find(idMarca);

           FillCombosEdit(entidad);

            ViewBag.Titulo = "Editar " + entidad?.DefaultValue;

            return View("Edit", entidad);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ResourceAuthorize(DataOperation.Update, Controllers.Productos)]
        public ActionResult Edit(int? idAlmacen, int? idTipoModelo, int? idStockRoom, int? idMarca, int? idModelo, int? idUbicacion, int? idEstado, int? idUsuarioAlta, int? idUsuarioEntrega, int? idUsuarioBaja, [Bind(Include = "Id,NumeroPedido,Cantidad, CodigoBarras, NumeroSerie, Destinatario, FechaEntrada, FechaEntrega, FechaBaja")]Producto model)
        {
            if (idAlmacen != null)
                model.Almacen = Db.Almacenes.Find(idAlmacen);

            if (idStockRoom != null)
                model.Departamento = Db.StockRooms.Find(idStockRoom);

            if (idMarca != null)
                model.Marca = Db.Marcas.Find(idMarca);

            if (idModelo != null)
                model.Modelo = Db.Modelos.Find(idModelo);

            if (idUbicacion != null)
                model.Ubicacion = Db.Ubicaciones.Find(idUbicacion);

            if (idEstado != null)
                model.Estado = Db.Estados.Find(idEstado);

            if (idUsuarioAlta != null)
                model.UsuarioAlta = Db.Empleados.Find(idUsuarioAlta);

            if (idUsuarioEntrega != null)
                model.UsuarioEntrega = Db.Empleados.Find(idUsuarioEntrega);

            if (idUsuarioBaja != null)
                model.UsuarioBaja = Db.Empleados.Find(idUsuarioBaja);

            return base.Edit(model);
        }

        [ResourceAuthorize(DataOperation.Update, Controllers.Productos)]
        public ActionResult DeleteAlta(int? id)
        {
            var productos = TempData["productos"];
            var producto = TempData["producto"];

            if (productos != null)
            {
                salidaEntity = (List<SalidaProducto>)productos;
                var model = (BusquedaSalidaProducto)TempData["producto"];
                FillCombos();
                return View("Salida", model);
            }

            BusquedaSalidaProducto defaultEntity = new BusquedaSalidaProducto();
            defaultEntity.CheckCodigoBarras = true;

            FillCombos();
            ViewBag.Titulo = "Salida";
            return View("Salida", defaultEntity);

        }

        [ResourceAuthorize(DataOperation.Delete, Controllers.Productos)]
        public override ActionResult Delete(int? id) => base.Delete(id);

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [ResourceAuthorize(DataOperation.Delete, Controllers.Productos)]
        public override ActionResult DeleteConfirmed(int id) => base.DeleteConfirmed(id);

        [ResourceAuthorize(DataOperation.Read, Controllers.Productos)]
        public ActionResult Search(int? idMarca, int? idTipoModelo, int? idStockRoom, int? idModelo, int? idAlmacen, int? idEstado, BusquedaProducto model, string submitButton, int? page)
        {
            var skip = (page - 1) * base.RowsPerPage ?? 0;

            var list = Db.Productos.AsQueryable().Where(a => a.Activo == true);
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
                list = list.Where(e => e.Modelo.TipoModelo.Id == idTipoModelo);
            }

            if (idStockRoom.HasValue)
            {
                list = list.Where(e => e.Departamento.Id == idStockRoom);
            }

            if (idModelo.HasValue)
            {
                list = list.Where(e => e.Modelo.Id == idModelo);
            }

            if (idAlmacen.HasValue)
            {
                list = list.Where(e => e.Almacen.Id == idAlmacen);
            }

            if (idEstado.HasValue)
            {
                list = list.Where(e => e.Estado.Id == idEstado);
            }

            if (!string.IsNullOrWhiteSpace(model.NumeroPedido))
            {
                list = list.Where(e => e.NumeroPedido.Contains(model.NumeroPedido));
            }

            if (!string.IsNullOrWhiteSpace(model.CodigoBarras))
            {
                list = list.Where(e => e.CodigoBarras.Contains(model.CodigoBarras));
            }

            if (!string.IsNullOrWhiteSpace(model.NumeroSerie))
            {
                list = list.Where(e => e.NumeroSerie.Contains(model.NumeroSerie));
            }

            DateTime? FechaEntradaDesde = null;
            if (Request.Params["FechaEntradaDesde"] != "") { FechaEntradaDesde = Convert.ToDateTime(Request.Params["FechaEntradaDesde"]); }
            DateTime? FechaEntradaHasta = null;
            if (Request.Params["FechaEntradaHasta"] != "") { FechaEntradaHasta = Convert.ToDateTime(Request.Params["FechaEntradaHasta"]); }
            if (FechaEntradaDesde != null && FechaEntradaHasta != null)
            {
                list = list.Where(e => e.FechaEntrada >= FechaEntradaDesde && e.FechaEntrada <= FechaEntradaHasta);
            }
            else
            {
                if (FechaEntradaDesde != null)
                {
                    list = list.Where(e => e.FechaEntrada >= FechaEntradaDesde);
                }
                if (FechaEntradaHasta != null)
                {
                    list = list.Where(e => e.FechaEntrada <= FechaEntradaHasta);
                }
            }

            list = list.OrderByDescending(e => e.Id);

            ViewBag.SearchCount = list.Count();
            ViewBag.SearchEntity = model;

            var producto = list.Select(m => m.Id).ToList();
            ViewBag.listProductos = string.Join("|", producto);

            FillCombos();

            if (submitButton == "Buscar")
                return View("Index", list.Skip(skip).Take(base.RowsPerPage).ToList());

            ExcelService excelService = new ExcelService();
            byte[] dt = null;

            if (submitButton == "Generar Informe")
                 dt = excelService.ExportToExcel<Producto>(list.ToList());
            else
                dt = excelService.ExportToExcelStock<Producto>(list.ToList());

            string fileName = String.Empty;

            fileName += "Productos_" + DateTime.Now.ToString("yyyyMMdd") + ".xlsx";
            Response.AddHeader("content-disposition", "attachment; filename=" + fileName);
            Response.ContentType = "application/vnd.ms-excel";
            Response.BinaryWrite(dt);
            Response.End();
            return null;


        }

        [ResourceAuthorize(DataOperation.Read, Controllers.Productos)]
        public ActionResult SearchBaja(int? idMarca, int? idTipoModelo, int? idStockRoom, int? idModelo, int? idAlmacen, int? idUbicacion, string Confirmar, BusquedaProducto model, int? page)
        {
            var skip = (page - 1) * base.RowsPerPage ?? 0;

            var list = Db.Productos.AsQueryable().Where(a => a.Activo == true && (a.Estado.Nombre.Contains("En Stock") || a.Estado.Nombre.Contains("Pending AFD")));
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
                list = list.Where(e => e.Modelo.TipoModelo.Id == idTipoModelo);
            }

            if (idUbicacion.HasValue)
            {
                list = list.Where(e => e.Ubicacion.Id == idUbicacion);
            }

            if (idAlmacen.HasValue)
            {
                list = list.Where(e => e.Almacen.Id == idAlmacen);
            }


            if (idStockRoom.HasValue)
            {
                list = list.Where(e => e.Departamento.Id == idStockRoom);
            }

            if (idModelo.HasValue)
            {
                list = list.Where(e => e.Modelo.Id == idModelo);
            }

            if (!string.IsNullOrWhiteSpace(model.CodigoBarras))
            {
                list = list.Where(e => e.CodigoBarras.Contains(model.CodigoBarras));
            }

            if (!string.IsNullOrWhiteSpace(model.NumeroPedido))
            {
                list = list.Where(e => e.NumeroPedido.Contains(model.NumeroPedido));
            }

            if (!string.IsNullOrWhiteSpace(model.NumeroSerie))
            {
                list = list.Where(e => e.NumeroSerie.Contains(model.NumeroSerie));
            }


            list = list.OrderByDescending(e => e.Id);

            if (Confirmar != null)
            {
                if (model.selectedArticulos != null)
                {
                    List<string> articulos = model.selectedArticulos.Split('|').ToList();

                    var lista = Db.Productos.AsQueryable().Where(a => a.Activo == true && (a.Estado.Nombre.Contains("En Stock") || a.Estado.Nombre.Contains("Pending AFD")));
                    lista = lista.Where(m => articulos.Contains(m.Id.ToString()));

                    foreach (var producto in lista.ToList())
                    {
                        producto.Estado = Db.Estados.Where(m => m.Nombre.Equals("Inventory")).FirstOrDefault();
                        producto.UsuarioBaja = Db.Empleados.Find(UserId);
                        producto.FechaBaja = DateTime.Now;
                    }

                    Db.SaveChanges();
                    return RedirectToAction("Index");
                }

            }


            ViewBag.SearchCount = list.Count();
            ViewBag.SearchEntity = model;

            FillCombos();
            return View("Baja", list.ToList());
        }

        [HttpPost, ActionName("ConfirmarAlta")]
        [ValidateAntiForgeryToken]
        [ResourceAuthorize(DataOperation.Read, Controllers.Productos)]
        public ActionResult ConfirmarAlta(List<EntradaProducto> productos)
        {
            ModelState.Clear();
            if (productos != null && productos.Count > 0)
            {
                int indice = 0;
                bool error = false;
                string valor = productos[0].Almacen.Nombre;
                var almacen = Db.Almacenes.Where(m => m.Nombre == valor && m.Activo == true).First();
                valor = productos[0].TipoModelo.Nombre;
                var tipo = Db.TiposModelos.Where(m => m.Nombre == valor && m.Activo == true).First();
                valor = productos[0].Marca.Nombre;
                var marca = Db.Marcas.Where(m => m.Nombre == valor && m.Activo == true).First();
                valor = productos[0].Modelo.Nombre;
                var modelo = Db.Modelos.Where(m => m.Nombre == valor && m.Activo == true).First();
                valor = productos[0].StockRoom.Codigo;
                var sotckroom = Db.StockRooms.Where(m => m.Codigo == valor && m.Activo == true).First();
                valor = productos[0].Ubicacion.Nombre;
                var ubicacion = Db.Ubicaciones.Where(m => m.Nombre == valor && m.Activo == true).First();
                var productosAux = productos.ToList();
                foreach (var nuevoProducto in productos)
                {
                    Producto producto = new Producto();
                    producto.Almacen = almacen;
                    producto.Marca = marca;
                    producto.Modelo = modelo;
                    producto.Departamento = sotckroom;
                    producto.Ubicacion = ubicacion;
                    producto.FechaEntrada = DateTime.Now;
                    producto.CodigoBarras = nuevoProducto.CodigoBarras;
                    producto.NumeroSerie = nuevoProducto.NumeroSerie;
                    producto.UsuarioAlta = Db.Empleados.Find(UserId);
                        
                    if (producto.UsuarioAlta == null)
                        producto.UsuarioAlta =  Db.Empleados.Find(1);
                    producto.Estado = Db.Estados.Where(m => m.Nombre.Contains("En Stock")).FirstOrDefault();
                    producto.NumeroPedido = nuevoProducto.NumeroPedido;


                    var navResult = NavigationPropertiesValidation(producto);
                    if (navResult != null) return navResult;

                    var result = validator.Validate(producto);
                    if (result.Valid)
                    {
                        Db.Productos.Add(producto);
                        Db.SaveChanges();
                        productosAux.Remove(nuevoProducto);
                    }

                    else
                    {
                        ViewBag.Titulo = "Alta";
                        result.ValidationErrors.ToList().ForEach(v => ViewBag.MessageError += v.Value + "<br/>");
                        //result.ValidationErrors.ToList().ForEach(v => ModelState.Remove("productos[" + indice + "]." + v.Key));
                        result.ValidationErrors.ToList().ForEach(v => ModelState.AddModelError("productos[" + indice + "]." + v.Key, v.Value));
                        error = true;
                        TempData["producto"] = producto;
                        indice++;
                    }
                }

                if (error)
                {
                    productos = productosAux;
                    TempData["ViewData"] = ViewData;
                    TempData["productos"] = productos;
                    TempData["error"] = ViewBag.MessageError;

                    return RedirectToAction("Create");
                }
            }

            FillCombos();
            ExportExcel(productos);
            return RedirectToAction("Index" );
        }

        [ResourceAuthorize(DataOperation.Update, Controllers.Productos)]
        public ActionResult Salida(int? page)
        {
            var productos = TempData["productos"];
            var producto = TempData["producto"];
            var error = TempData["error"];
            ViewBag.Titulo = "Salida";

            if (error != null)
                ViewBag.MessageError = error;

            if (productos != null)
            {
                salidaEntity = (List<SalidaProducto>)productos;
                var model = (BusquedaSalidaProducto)TempData["producto"];
                FillCombos();
                return View("Salida", model);
            }

            BusquedaSalidaProducto defaultEntity = new BusquedaSalidaProducto();
            defaultEntity.CheckCodigoBarras = true;

            FillCombos();
            return View("Salida", defaultEntity);
        }

        [ResourceAuthorize(DataOperation.Update, Controllers.Productos)]
        public ActionResult Devolucion(int? page)
        {
            var productos = TempData["productos"];
            var producto = TempData["producto"];
            var error = TempData["error"];
            var estado = TempData["estado"];
            ViewBag.Titulo = "Devolución";

            if (error != null)
                ViewBag.MessageError = error;

            if (productos != null)
            {
                salidaEntity = (List<SalidaProducto>)productos;
                var model = (BusquedaSalidaProducto)TempData["producto"];
                PutEntityInViewBag<Estado>(model.EstadoDevolucion.GetId(), true, "EstadoDevolucion", predicate: m => m.Nombre.Contains("En Stock") || m.Nombre.Contains("Pending AFD"));
                FillCombos();
                return View("Devolucion", model);
            }

            BusquedaSalidaProducto defaultEntity = new BusquedaSalidaProducto();
            defaultEntity.CheckCodigoBarras = true;

            FillCombos();
            PutEntityInViewBag<Estado>(null, true, "EstadoDevolucion", predicate: m => m.Nombre.Contains("En Stock") || m.Nombre.Contains("Pending AFD"));


            return View("Devolucion", defaultEntity);
        }

        [HttpPost, ActionName("Salida")]
        [ValidateAntiForgeryToken]
        [ResourceAuthorize(DataOperation.Create, Controllers.Productos)]
        public ActionResult Salida(int? idEstadoDevolucion, string indice, List<SalidaProducto> productos, string codigoBarras, string numeroSerie, string destinatario, string Confirmar)
        {
            if (string.IsNullOrEmpty(codigoBarras) && string.IsNullOrEmpty(numeroSerie) && productos == null)
            {
                ViewBag.MessageError = "Por favor, introduzca algún producto";
                productos = new List<SalidaProducto>();
            }

            else
            {

                if (Confirmar == "Confirmar Salida")
                    return ConfirmarSalida(productos);

                if (Confirmar == "Confirmar Entrada")
                    return ConfirmarDevolucion(idEstadoDevolucion, productos);

                if (indice != null && productos != null)
                {
                    productos.RemoveAt(int.Parse(indice));
                }

                if (productos == null)
                {
                    productos = new List<SalidaProducto>();
                }

                if (!string.IsNullOrEmpty(codigoBarras))
                {
                    if (productos.Any(m => m.CodigoBarras == codigoBarras))
                        ViewBag.MessageError = "Ya se ha introducido un producto con el Código de Barras " + codigoBarras;
                    else
                    {
                        Producto producto;
                        if (idEstadoDevolucion != null)
                        {
                            producto = Db.Productos.Where(m => m.CodigoBarras == codigoBarras && m.Estado.Nombre.Contains("En Uso")).FirstOrDefault();
                            if (producto == null)
                                ViewBag.MessageError = "No hay ningún producto En Uso con el Código de Barras " + codigoBarras;
                        }
                        else
                        {
                            producto = Db.Productos.Where(m => m.CodigoBarras == codigoBarras && m.Estado.Nombre.Contains("En Stock")).FirstOrDefault();
                            if (producto == null)
                                ViewBag.MessageError = "No hay ningún producto En Stock con el Código de Barras " + codigoBarras;
                        }

                        if (producto != null)
                        {
                            var salida = new SalidaProducto();
                            salida.Almacen = producto.Almacen;
                            salida.Marca = producto.Marca;
                            salida.Modelo = producto.Modelo;
                            salida.StockRoom = producto.Departamento;
                            salida.Ubicacion = producto.Ubicacion;
                            salida.NumeroPedido = producto.NumeroPedido;
                            salida.CodigoBarras = producto.CodigoBarras;
                            salida.NumeroSerie = producto.NumeroSerie;
                            salida.IdProducto = producto.Id;
                            salida.Destinatario = destinatario;
                            productos.Add(salida);

                        }
                    }
                }

                if (!string.IsNullOrEmpty(numeroSerie) && !productos.Any(m => m.NumeroSerie == numeroSerie))
                {
                    Producto producto;

                    if (idEstadoDevolucion != null)
                    {
                        producto = Db.Productos.Where(m => m.NumeroSerie == numeroSerie && m.Estado.Nombre.Contains("En Uso")).FirstOrDefault();
                        if (producto == null)
                            ViewBag.MessageError = "No hay ningún producto En Uso con el Nº de Serie " + numeroSerie;
                    }
                    else
                    {
                        producto = Db.Productos.Where(m => m.NumeroSerie == numeroSerie && m.Estado.Nombre.Contains("En Stock")).FirstOrDefault();
                        if (producto == null)
                            ViewBag.MessageError = "No hay ningún producto En Stock con el Nº de Serie " + numeroSerie;
                    }

                    if (producto != null)
                    {
                        var salida = new SalidaProducto();
                        salida.Almacen = producto.Almacen;
                        salida.Marca = producto.Marca;
                        salida.Modelo = producto.Modelo;
                        salida.StockRoom = producto.Departamento;
                        salida.Ubicacion = producto.Ubicacion;
                        salida.NumeroPedido = producto.NumeroPedido;
                        salida.CodigoBarras = producto.CodigoBarras;
                        salida.NumeroSerie = producto.NumeroSerie;
                        salida.IdProducto = producto.Id;
                        salida.Destinatario = destinatario;
                        productos.Add(salida);

                    }
                }
            }

            ViewBag.Titulo = "Salida";
            ViewBag.SearchEntity = new BusquedaSalidaProducto();
            ViewBag.SearchEntity.Destinatario = destinatario;

            if (idEstadoDevolucion != null)
                ViewBag.SearchEntity.EstadoDevolucion = Db.Estados.Find(idEstadoDevolucion);

            if (!string.IsNullOrEmpty(numeroSerie))
                ViewBag.SearchEntity.CheckNumeroSerie = true;
            else
                ViewBag.SearchEntity.CheckCodigoBarras = true;

            FillCombos();

            salidaEntity = productos;

            ViewBag.TotalRowCount = salidaEntity.Count();
            ViewBag.SearchCount = salidaEntity.Count();

            TempData["productos"] = productos;
            TempData["producto"] = ViewBag.SearchEntity;
            TempData["error"] = ViewBag.MessageError;
            return RedirectToAction("Salida");
        }

        [HttpPost, ActionName("Devolucion")]
        [ValidateAntiForgeryToken]
        [ResourceAuthorize(DataOperation.Create, Controllers.Productos)]
        public ActionResult Devolucion(int? idEstadoDevolucion, string indice, List<SalidaProducto> productos, string codigoBarras, string numeroSerie, string destinatario, string Confirmar)
        {
    
            if (string.IsNullOrEmpty(codigoBarras) && string.IsNullOrEmpty(numeroSerie) && productos == null)
            {
                ViewBag.MessageError = "Por favor, introduzca algún producto";
                productos = new List<SalidaProducto>();
            }

            else
            {

                //if (Confirmar == "Confirmar Salida")
                //    return ConfirmarSalida(productos);

                if (Confirmar == "Confirmar")
                {
                    if (idEstadoDevolucion != null)
                        return ConfirmarDevolucion(idEstadoDevolucion, productos);
                    else
                        ViewBag.MessageError = "Por favor, introduzca un Estado Final ";
                } 

                if (indice != null && productos != null)
                {
                    productos.RemoveAt(int.Parse(indice));
                }

                if (productos == null)
                {
                    productos = new List<SalidaProducto>();
                }

                if (!string.IsNullOrEmpty(codigoBarras))
                {
                    if (productos.Any(m => m.CodigoBarras == codigoBarras))
                        ViewBag.MessageError = "Ya se ha introducido un producto con el Código de Barras " + codigoBarras;
                    else
                    {
                        Producto producto;

                        producto = Db.Productos.Where(m => m.CodigoBarras == codigoBarras && m.Estado.Nombre.Contains("En Uso")).FirstOrDefault();
                        if (producto == null)
                            ViewBag.MessageError = "No hay ningún producto En Uso con el Código de Barras " + codigoBarras;

                        if (producto != null)
                        {
                            var salida = new SalidaProducto();
                            salida.Almacen = producto.Almacen;
                            salida.Marca = producto.Marca;
                            salida.Modelo = producto.Modelo;
                            salida.StockRoom = producto.Departamento;
                            salida.Ubicacion = producto.Ubicacion;
                            salida.NumeroPedido = producto.NumeroPedido;
                            salida.CodigoBarras = producto.CodigoBarras;
                            salida.NumeroSerie = producto.NumeroSerie;
                            salida.IdProducto = producto.Id;
                            salida.Destinatario = producto.Destinatario;
                            productos.Add(salida);

                        }
                    }
                }

                if (!string.IsNullOrEmpty(numeroSerie))
                {
                    if (productos.Any(m => m.NumeroSerie == numeroSerie))
                        ViewBag.MessageError = "Ya se ha introducido un producto con el Nº de Serie  " + numeroSerie;
                    else
                    {
                        Producto producto;

                        producto = Db.Productos.Where(m => m.NumeroSerie == numeroSerie && m.Estado.Nombre.Contains("En Uso")).FirstOrDefault();
                        if (producto == null)
                            ViewBag.MessageError = "No hay ningún producto En Uso con el Nº de Serie " + numeroSerie;

                        if (producto != null)
                        {
                            var salida = new SalidaProducto();
                            salida.Almacen = producto.Almacen;
                            salida.Marca = producto.Marca;
                            salida.Modelo = producto.Modelo;
                            salida.StockRoom = producto.Departamento;
                            salida.Ubicacion = producto.Ubicacion;
                            salida.NumeroPedido = producto.NumeroPedido;
                            salida.CodigoBarras = producto.CodigoBarras;
                            salida.NumeroSerie = producto.NumeroSerie;
                            salida.IdProducto = producto.Id;
                            salida.Destinatario = destinatario;
                            productos.Add(salida);
                        }
                    }
                }
            }

            ViewBag.Titulo = "Entrada";
            ViewBag.SearchEntity = new BusquedaSalidaProducto();
            ViewBag.SearchEntity.Destinatario = destinatario;

            if (idEstadoDevolucion != null)
                ViewBag.SearchEntity.EstadoDevolucion = Db.Estados.Find(idEstadoDevolucion);

            if (!string.IsNullOrEmpty(numeroSerie))
                ViewBag.SearchEntity.CheckNumeroSerie = true;
            else
                ViewBag.SearchEntity.CheckCodigoBarras = true;

            FillCombos();

            salidaEntity = productos;

            ViewBag.TotalRowCount = salidaEntity.Count();
            ViewBag.SearchCount = salidaEntity.Count();

            TempData["productos"] = productos;
            TempData["producto"] = ViewBag.SearchEntity;
            TempData["error"] = ViewBag.MessageError;
            //TempData["estado"] = ViewBag.EstadoId;
            return RedirectToAction("Devolucion");
        }

        [HttpPost]
        [ResourceAuthorize(DataOperation.Update, Controllers.Productos)]
        public ActionResult ConfirmarSalida(List<SalidaProducto> productos)
        {
            if (productos != null && productos.Count > 0)
            {
                foreach (var salidaProducto in productos)
                {
                    Producto producto = Db.Productos.Find(salidaProducto.IdProducto);
                    if (producto != null)
                    {
                        producto.Estado = Db.Estados.Where(m => m.Nombre.Contains("En Uso")).FirstOrDefault();
                        producto.UsuarioEntrega = Db.Empleados.Find(UserId);
                        producto.FechaEntrega = DateTime.Now;
                        producto.Destinatario = salidaProducto.Destinatario;
                    }
                }
            }

            Db.SaveChanges();
            FillCombos();
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ResourceAuthorize(DataOperation.Update, Controllers.Productos)]
        public ActionResult ConfirmarDevolucion(int? estado, List<SalidaProducto> productos)
        {
            if (productos != null && productos.Count > 0)
            {
                foreach (var devolucionProducto in productos)
                {
                    Producto producto = Db.Productos.Find(devolucionProducto.IdProducto);
                    if (producto != null)
                    {
                        producto.Estado = Db.Estados.Find(estado);
                        producto.UsuarioEntrega = Db.Empleados.Find(UserId);
                        producto.FechaEntrega = DateTime.Now;
                    }
                }
            }

            Db.SaveChanges();
            FillCombos();
            return RedirectToAction("Index");
        }

        [ResourceAuthorize(DataOperation.Update, Controllers.Productos)]
        public ActionResult Baja()
        {
            var entidades = new List<Producto>();

            ViewBag.TotalRowCount = Db.Productos.AsQueryable().Where(a => a.Activo == true && (a.Estado.Nombre.Contains("En Stock") || a.Estado.Nombre.Contains("Pending AFD"))).Count();

            ViewBag.SearchCount = entidades.Count();

            if (this.searchEntity != null)
                ViewBag.SearchEntity = BuildSearchEntity();
            else
                ViewBag.SearchEntity = BuildDefaultSearchEntity();


            ViewBag.ListaExcel = entidades;

            FillViewBag();

            return View("Baja", entidades.ToList());
        }

        protected override void FillCombos(Producto entidad = null)
        {
            base.FillCombos(entidad);
            if (entidad != null)
            {
                if (entidad.Almacen != null)
                {
                    PutEntityInViewBag<Almacen>(entidad.Almacen.GetId(), true);
                }
                else
                {
                    PutEntityInViewBag<Almacen>(null, true);
                }

                if (entidad.StockRoom != null)
                {
                    PutEntityInViewBag<StockRoom>(entidad.Departamento.GetId(), true);
                }
                else
                {
                    PutEntityInViewBag<StockRoom>(null, true);
                }

                if (entidad.Marca != null)
                {
              
                    PutEntityInViewBag<Marca>(entidad.Marca.GetId(), true);

                    if (entidad.Modelo == null && entidad.defaultTipoModelo == null)
                    {
                        PutEntityInViewBag<Modelo>(null, true, predicate: m => m.Marca.Id == entidad.Marca.Id);
                        PutEntityInViewBag<TipoModelo>(null, true);
                    }
                    else
                    {
                        if (entidad.Modelo != null && entidad.Modelo.TipoModelo != null)
                        {

                            PutEntityInViewBag<TipoModelo>(entidad.Modelo.TipoModelo.GetId(), true, predicate: m => m.Id == entidad.Modelo.TipoModelo.Id);
                            PutEntityInViewBag<Modelo>(entidad.Modelo.GetId(), true, predicate: m => m.Marca.Id == entidad.Marca.Id && m.TipoModelo.Id == entidad.Modelo.TipoModelo.Id);
                         
                         }
                        else
                        {
                            if (entidad.Modelo != null)
                            {
                                PutEntityInViewBag<Modelo>(entidad.Modelo.GetId(), true);
                                PutEntityInViewBag<TipoModelo>(null, true, predicate: m => m.Id == entidad.Modelo.TipoModelo.Id);

                            }
                            else
                            {
                                int idTipoModelo = Db.TiposModelos.Where(m => m.Nombre == entidad.defaultTipoModelo).FirstOrDefault().Id;
                                PutEntityInViewBag<Modelo>(null, true, predicate: m => m.Marca.Id == entidad.Marca.Id && m.TipoModelo.Id == idTipoModelo);
                                PutEntityInViewBag<TipoModelo>(idTipoModelo, true);
                            }
                        }
                    }
                }
                else
                {
                    if (entidad.Modelo != null && entidad.Modelo.TipoModelo != null)
                    {
                        PutEntityInViewBag<TipoModelo>(entidad.Modelo.TipoModelo.GetId(), true, predicate: m => m.Id == entidad.Modelo.TipoModelo.Id);
                        PutEntityInViewBag<Modelo>(entidad.Modelo.TipoModelo.GetId(), true);
                    }
                    else
                    {
                        if (entidad.Modelo != null)
                        {
                            PutEntityInViewBag<Modelo>(entidad.Modelo.GetId(), true);
                            PutEntityInViewBag<TipoModelo>(null, true, predicate: m => m.Id == entidad.Modelo.TipoModelo.Id);

                        }
                        else
                        {
                            PutEntityInViewBag<Modelo>(null, true);
                            PutEntityInViewBag<TipoModelo>(null, true);
                        }
                    }

                    PutEntityInViewBag<Marca>(null, true);
                }

                if (entidad.Ubicacion != null)
                {
                    PutEntityInViewBag<Ubicacion>(entidad.Ubicacion.GetId(), true);
                }
                else
                {
                    PutEntityInViewBag<Ubicacion>(null, true);
                }

                if (entidad.Estado != null)
                {
                    PutEntityInViewBag<Estado>(entidad.Estado.GetId(), true);
                }
                else
                {
                    PutEntityInViewBag<Estado>(null, true);
                }

                if (entidad.UsuarioAlta != null)
                {
                    PutEntityInViewBag<Empleado>(entidad.UsuarioAlta.GetId(), true, "UsuarioAlta");
                }
                else
                {
                    PutEntityInViewBag<Empleado>(null, true, "UsuarioAlta");
                }

                if (entidad.UsuarioEntrega != null)
                {
                    PutEntityInViewBag<Empleado>(entidad.UsuarioEntrega.GetId(), true, "UsuarioEntrega");
                }
                else
                {
                    PutEntityInViewBag<Empleado>(null, true, "UsuarioEntrega");
                }

                if (entidad.UsuarioBaja != null)
                {
                    PutEntityInViewBag<Empleado>(entidad.UsuarioBaja.GetId(), true, "UsuarioBaja");
                }
                else
                {
                    PutEntityInViewBag<Empleado>(null, true, "UsuarioBaja");
                }
            }

            else
            {
                PutEntityInViewBag<Almacen>(null, true);
                PutEntityInViewBag<TipoModelo>(null, true);
                PutEntityInViewBag<StockRoom>(null, true);
                PutEntityInViewBag<Marca>(null, true);
                PutEntityInViewBag<Modelo>(null, true);
                PutEntityInViewBag<Ubicacion>(null, true);
                PutEntityInViewBag<Estado>(null, true);
                PutEntityInViewBag<Empleado>(null, true, "UsuarioAlta");
                PutEntityInViewBag<Empleado>(null, true, "UsuarioEntrega");
                PutEntityInViewBag<Empleado>(null, true, "UsuarioBaja");
            }

            ViewBag.AltaProducto = altaEntity;
            ViewBag.AltaTotalRowCount = altaEntity.Count();
            ViewBag.AltaRowsPerPage = 100;
            ViewBag.SalidaProducto = salidaEntity;
            ViewBag.SalidaTotalRowCount = salidaEntity.Count();
            ViewBag.SalidaRowsPerPage = 100;
        }

        protected void FillCombosEdit(Producto entidad = null)
        {
            base.FillCombos(entidad);
            if (entidad != null)
            {
                if (entidad.Almacen != null)
                {
                    PutEntityInViewBag<Almacen>(entidad.Almacen.GetId(), true);
                }
                else
                {
                    PutEntityInViewBag<Almacen>(null, true);
                }

                if (entidad.StockRoom != null)
                {
                    PutEntityInViewBag<StockRoom>(entidad.Departamento.GetId(), true);
                }
                else
                {
                    PutEntityInViewBag<StockRoom>(null, true);
                }

                if (entidad.Marca != null)
                {
                    PutEntityInViewBag<Modelo>(entidad.Modelo.GetId(), true, predicate: m => m.Marca.Id == entidad.Marca.Id);
                    PutEntityInViewBag<Marca>(entidad.Marca.GetId(), true);
                    
                }
                else
                {
                    PutEntityInViewBag<Marca>(null, true);
                    PutEntityInViewBag<Modelo>(null, true);
                }

                PutEntityInViewBag<TipoModelo>(null, true);

                if (entidad.Ubicacion != null)
                {
                    PutEntityInViewBag<Ubicacion>(entidad.Ubicacion.GetId(), true);
                }
                else
                {
                    PutEntityInViewBag<Ubicacion>(null, true);
                }

                if (entidad.Estado != null)
                {
                    PutEntityInViewBag<Estado>(entidad.Estado.GetId(), true);
                }
                else
                {
                    PutEntityInViewBag<Estado>(null, true);
                }

                if (entidad.UsuarioAlta != null)
                {
                    PutEntityInViewBag<Empleado>(entidad.UsuarioAlta.GetId(), true, "UsuarioAlta");
                }
                else
                {
                    PutEntityInViewBag<Empleado>(null, true, "UsuarioAlta");
                }

                if (entidad.UsuarioEntrega != null)
                {
                    PutEntityInViewBag<Empleado>(entidad.UsuarioEntrega.GetId(), true, "UsuarioEntrega");
                }
                else
                {
                    PutEntityInViewBag<Empleado>(null, true, "UsuarioEntrega");
                }

                if (entidad.UsuarioBaja != null)
                {
                    PutEntityInViewBag<Empleado>(entidad.UsuarioBaja.GetId(), true, "UsuarioBaja");
                }
                else
                {
                    PutEntityInViewBag<Empleado>(null, true, "UsuarioBaja");
                }
            }

            else
            {
                PutEntityInViewBag<Almacen>(null, true);
                PutEntityInViewBag<TipoModelo>(null, true);
                PutEntityInViewBag<StockRoom>(null, true);
                PutEntityInViewBag<Marca>(null, true);
                PutEntityInViewBag<Modelo>(null, true);
                PutEntityInViewBag<Ubicacion>(null, true);
                PutEntityInViewBag<Estado>(null, true);
                PutEntityInViewBag<Empleado>(null, true, "UsuarioAlta");
                PutEntityInViewBag<Empleado>(null, true, "UsuarioEntrega");
                PutEntityInViewBag<Empleado>(null, true, "UsuarioBaja");
            }

            ViewBag.AltaProducto = altaEntity;
            ViewBag.AltaTotalRowCount = altaEntity.Count();
            ViewBag.AltaRowsPerPage = 100;
            ViewBag.SalidaProducto = salidaEntity;
            ViewBag.SalidaTotalRowCount = salidaEntity.Count();
            ViewBag.SalidaRowsPerPage = 100;
        }

        [ResourceAuthorize(DataOperation.Read, Controllers.Productos)]
        public ActionResult ExportExcelConsulta(string listProductos, string submitButton)
        {
            string[] ids = listProductos.Split('|');
            var listIds = ids.ToList().Select(int.Parse).ToList();
            var list = Db.Productos.Where(m => listIds.Contains(m.Id)).ToList();

            ExcelService excelService = new ExcelService();
            byte[] dt = null;

            if (submitButton == "Generar Informe")
                dt = excelService.ExportToExcel<Producto>(list.ToList());
            else
                dt = excelService.ExportToExcelStock<Producto>(list.ToList());
           
            string fileName = String.Empty;

            fileName += "Productos_" + DateTime.Now.ToString("yyyyMMdd") + ".xlsx";
            Response.AddHeader("content-disposition", "attachment; filename=" + fileName);
            Response.ContentType = "application/vnd.ms-excel";
            Response.BinaryWrite(dt);
            Response.End();
            return null;
        }

        [ResourceAuthorize(DataOperation.Read, Controllers.Productos)]
        public ActionResult  ExportExcel(List<EntradaProducto> productos)
        {

            try
            {
                ExcelService excelService = new ExcelService();
                FileInfo templateFile = new FileInfo(Server.MapPath("~") + ConfigurationManager.AppSettings["CarpetaTemplates"] + "\\HP_Asset_Manager.xlsx");
                ExcelPackage pack = new ExcelPackage(templateFile, true);
                int columna = 1;
                int fila = 14;

                ExcelWorksheet hoja;

                //aqui se lee el archivo
                hoja = pack.Workbook.Worksheets[1];

                foreach (var producto in productos)
                {
                    columna = 1;

                    hoja.Cells[fila, columna++].Value = "/SPAIN/MADRID/GETAFE/" + producto.StockRoom.Codigo + "/SISTEMAS/ALMACEN/";
                    hoja.Cells[fila, columna++].Value = producto.StockRoom?.Codigo;
                    hoja.Cells[fila, columna++].Value = producto.StockRoom?.StockRoomString;
                    hoja.Cells[fila, columna++].Value = producto.CodigoBarras;
                    hoja.Cells[fila, columna++].Value = producto.NumeroSerie;
                    hoja.Cells[fila, columna++].Value = producto.Marca;
                    hoja.Cells[fila, columna++].Value = producto.Modelo;
                    string code = Db.Modelos.Where(m => m.Nombre.Equals(producto.Modelo.DefaultValue)).FirstOrDefault().Barcode;
                    hoja.Cells[fila, columna++].Value = code;
                    hoja.Cells[fila, columna++].Value = producto.TipoModelo;
                    hoja.Cells[fila, columna++].Value = "INVENTORY";
                    hoja.Cells[fila, columna++].Value = "In Stock";
                    hoja.Cells[fila, columna++].Value = "";
                    hoja.Cells[fila, columna++].Value = "";
                    hoja.Cells[fila, columna++].Value = DateTime.Parse(DateTime.Now.ToShortDateString()).ToString("dd-MM-yyyy");
                    hoja.Column(columna).Style.Numberformat.Format = "mm-dd-yyyy";
                    hoja.Cells[fila, columna++].Value = DateTime.Parse(DateTime.Now.ToShortDateString()).ToString("dd-MM-yyyy");
                    hoja.Column(columna).Style.Numberformat.Format = "mm-dd-yyyy";
                    hoja.Cells[fila, columna++].Value = Db.Empleados.Find(UserId).Login;
                    hoja.Cells[fila, columna++].Value = "";
                    hoja.Cells[fila, columna++].Value = "";
                    hoja.Cells[fila, columna++].Value = "";
                    hoja.Cells[fila, columna++].Value = producto.NumeroPedido;
                    columna = 1;
                    fila++;
                }

                if (pack != null)
                {
                    string name = null;
                    FileInfo file = null;

                    name = Server.MapPath("~") + ConfigurationManager.AppSettings["CarpetaAdjuntos"] + "\\" + DateTime.Now.ToString("yyyyMMdd.hhmmss") + "." + "HP_Asset_Manager" + ".xlsx";

                    file = new FileInfo(name);

                    pack.SaveAs(file);
                    pack.Dispose();

                    mailservice.MailAltaProductos(name);
                }
            }

            catch (Exception e) {
                string error = e.ToString();
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ResourceAuthorize(DataOperation.Read, Controllers.Productos)]
        public ActionResult ImportExcel(HttpPostedFileBase productos, Producto model)
        {
            ModelState.Clear();

            if (productos != null && productos.ContentLength > 0 && (Path.GetExtension(productos.FileName) == ".xlsx" || Path.GetExtension(productos.FileName) == ".xls"))
            {
                int update = 0;
                int fail = 0;
                int count = 2;
                string lines = String.Empty;
                string barcode = String.Empty;

                try
                {
                    using (var excel = new ExcelPackage(productos.InputStream))
                    {
                        var wsFile = excel.Workbook.Worksheets.First();

                        var entities = wsFile.Extract<Producto>()

                           .WithProperty(p => p.CodigoBarras, "A")
                           .WithProperty(p => p.NumeroSerie, "B")
                           .WithProperty(p => p.Departamento, "C", getStockRoom)
                           .WithProperty(p => p.Almacen, "E", getAlmacen)
                           .WithProperty(p => p.Ubicacion, "F", getUbicacion)
                           .WithProperty(p => p.NumeroPedido, "G")
                           .WithProperty(p => p.FechaEntrada, "H", getFecha)
                            .WithProperty(p => p.defaultTipoModelo, "I")
                           .WithProperty(p => p.Estado, "J", getEstado)
                           .WithProperty(p => p.defaultMarca, "K")
                            .WithProperty(p => p.defaultModelo, "L")
                            .WithProperty(p => p.defaultBarcode, "M")
        
                            .GetData(2, wsFile.Dimension.Rows).ToList();

                        foreach (var entity in entities)
                        {
                            try
                            {
                                if (entity.CodigoBarras != null && entity.CodigoBarras != "CódigodeBarras")
                                {
                                    var exists = contextDefault.Productos.Where(m => m.CodigoBarras.ToUpper() == entity.CodigoBarras.ToUpper() || m.NumeroSerie.ToUpper() == entity.NumeroSerie.ToUpper()).FirstOrDefault();
                                    if (exists == null)
                                    {
                                        var marca = contextDefault.Marcas.Where(m => m.Nombre.ToUpper().Equals(entity.defaultMarca.ToUpper())).FirstOrDefault();

                                        if (marca == null)
                                        {
                                            marca = new Marca();
                                            marca.Nombre = entity.defaultMarca;

                                            if (marca.Nombre != null)
                                            {
                                                contextDefault.Marcas.Add(marca);
                                                contextDefault.SaveChanges();
                                            }

                                            else
                                            {
                                                fail++;
                                                lines += "FILA:" + count.ToString() + " *** ERROR: ";
                                                lines += "MARCA VACIA";
                                                lines += Environment.NewLine;
                                            }
                                        }

                                        entity.Marca = marca;

                                        var modelo = contextDefault.Modelos.Where(m => m.Nombre.ToUpper().Equals(entity.defaultModelo.ToString().ToUpper()) && m.Marca.Nombre.ToUpper().Equals(entity.defaultMarca.ToUpper())).FirstOrDefault();

                                        if (modelo == null)
                                        {
                                            modelo = new Modelo();
                                            modelo.Nombre = entity.defaultModelo;
                                            modelo.Marca = entity.Marca;
                                            modelo.TipoModelo = contextDefault.TiposModelos.Where(m => m.Nombre.ToUpper() == entity.defaultTipoModelo.ToUpper()).FirstOrDefault();
                                            modelo.Barcode = entity.defaultBarcode;

                                            if (modelo.TipoModelo != null && modelo.Nombre != null)
                                            {
                                                contextDefault.Modelos.Add(modelo);
                                                contextDefault.SaveChanges();
                                                entity.Modelo = modelo;
                                            }

                                            else
                                            {
                                                fail++;
                                                lines += "FILA:" + count.ToString() + " *** ERROR: ";
                                                lines += "NOMBRE MODELO O TIPOMODELO VACIO";
                                                lines += Environment.NewLine;
                                            }
                                        }

                                        else
                                        {
                                            entity.Modelo = modelo;
                                            
                                        }

                                        entity.UsuarioAlta = contextDefault.Empleados.Find(UserId);

                                        if (entity.UsuarioAlta == null)
                                            entity.UsuarioAlta = contextDefault.Empleados.Find(1);



                                        var navResult = NavigationPropertiesValidation(entity);
                                        if (navResult != null) return navResult;

                                        var result = validator.Validate(entity);
                                        if (result.Valid)
                                        {
                                            contextDefault.Productos.Add(entity);
                                            contextDefault.SaveChanges();
                                            update++;
                                        }
                                        else
                                        {
                                            fail++;
                                            lines += "FILA:" + count.ToString() + " *** ERROR: ";

                                            foreach(var resulterror in result.ValidationErrors)
                                            {
                                                lines += resulterror.Value + " - ";
                                            }

                                            lines += Environment.NewLine;
                                        }
                                    }

                                    else
                                    {
                                        fail++;
                                        lines += "FILA:" + count.ToString() + " *** ERROR: ";
                                        lines += "YA EXISTE UN PRODUCTO CON ESE CODIGO DE BARRAS O NUMERO DE SERIE";
                                        lines += Environment.NewLine;
                                    }
                                }
                                {
                                    count++;
                                }
                            }

                            catch (Exception e)
                            {
                                lines += count.ToString() + ",";
                                count++;
                                fail++;
                            }
                        }

                        ViewBag.Message = String.Format("Registros Leidos: {0}", update + fail);
                        ViewBag.RegistrosActualizados = String.Format("Registros Actualizados: {0}", update);
                        ViewBag.RegistrosFallidos = String.Format("Registros Fallidos: {0}", fail );

                        var fileName = "ErrorAltaProductos_" + DateTime.Now.Ticks.ToString() + ".txt";

                        string fileErrorName = Server.MapPath("~") + ConfigurationManager.AppSettings["CarpetaLogs"] + "\\" +  fileName;

                        string cast = lines.Replace("no puede estar vacío", "NO EXISTE");
                        cast = cast.Replace("no puede estar vacía", "NO EXISTE").ToUpper();
                        System.IO.File.WriteAllText(fileErrorName, cast);
                        ViewBag.Path = ConfigurationManager.AppSettings["UrlLogs"] + fileName;
                    }
                }

                catch (Exception e)
                {
                    ViewBag.Message = "Formato de fichero no válido";
                }
            }

            else
            {
                ViewBag.Message = "Debe seleccionar un fichero válido";
            }
            base.Index(null);
            ModelState.Clear();
            return View("Alta", model);
        }

        Func<object, StockRoom> getStockRoom = objStr =>
        {
            if (objStr == null)
                return null;

            var stockRoom = contextDefault.StockRooms.Where(m => m.Codigo.ToUpper().Equals(objStr.ToString().ToUpper())).FirstOrDefault();
            return stockRoom;
        };

        Func<object, Almacen> getAlmacen = objStr =>
        {
            if (objStr == null)
                return null;

            var almacen = contextDefault.Almacenes.Where(m => m.Nombre.ToUpper().Equals(objStr.ToString().ToUpper())).FirstOrDefault();
            return almacen;
        };

        Func<object, DateTime> getFecha = objStr =>
        {
            DateTime fechaAlta = DateTime.Now;

            if (objStr == null)
                return fechaAlta;

            var success = DateTime.TryParse(objStr.ToString(), out fechaAlta);

            if (success && fechaAlta >= DateTime.Parse("01-01-1900"))
                return fechaAlta;

            return DateTime.Now;
        
        };

        Func<object,   Ubicacion> getUbicacion = objStr =>
        {
            if (objStr == null)
            {
                return null;
            }

            var ubicacion = contextDefault.Ubicaciones.Where(m => m.Nombre.ToUpper().Equals(objStr.ToString().ToUpper())).FirstOrDefault();
            return ubicacion;
        };

        Func<object, Estado> getEstado = objStr =>
        {
            if (objStr == null)
                return null;

            var estado = contextDefault.Estados.Where(m => m.Nombre.ToUpper().Equals(objStr.ToString().ToUpper())).FirstOrDefault();
            return estado;
        };
    }    
}

   
          

