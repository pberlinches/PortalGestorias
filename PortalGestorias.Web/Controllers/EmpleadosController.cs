using System;
using PortalGestorias.Business.Validations;
using PortalGestorias.Domain.Entities;
using PortalGestorias.Infrastructure.Data;
using PortalGestorias.Web.Security;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Thinktecture.IdentityModel.Mvc;
using PortalGestorias.Business.Services;
using System.Configuration;
using System.Diagnostics;
using PortalGestorias.Domain.Models;

namespace PortalGestorias.Web.Controllers
{
    public class EmpleadosController : DataControllerBase<Empleado>
    {
        protected iCalServices iCalServices;
        protected string action;


        public EmpleadosController(CrmDbContext dbContext, IEntityValidator<Empleado> validator, iCalServices iCal)
             : base(dbContext)
        {
            this.validator = validator;
            iCalServices = iCal;
        }

        [ResourceAuthorize(DataOperation.Read, Controllers.Empleados)]
        public ActionResult Index(int? page, int? idCliente, bool? viewAll)
        {
            var skip = (page - 1) * RowsPerPage ?? 0;
            var entidades = Db.Empleados.AsQueryable();

            ViewBag.TotalRowCount = entidades.Count();
            ViewBag.SearchCount = entidades.Count();
            FillCombos();

            ViewBag.listUsuarios = entidades.ToList();

            if (viewAll != null && entidades.Count() > 0)
            {
                ViewBag.RowsPerPage = entidades.Count();
                return View(entidades.ToList());
            }

            return View("Index", entidades.OrderBy(a => a.Id).OrderBy(m => m.Login).Skip(skip).Take(RowsPerPage).ToList());
        }

        [ResourceAuthorize(DataOperation.Read, Controllers.Empleados)]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var empleado = Db.Empleados.Find(id);

            if (empleado == null)
            {
                return HttpNotFound();
            }

          
            ViewBag.idEmpleado = empleado.Id;
            ViewBag.Detalle = true;
            ViewBag.ViewOnly = true;
            FillCombos(empleado);
            return View("Edit", empleado);
        }

        [ResourceAuthorize(DataOperation.Update, Controllers.Empleados)]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var empleado = Db.Empleados.Find(id);
            if (empleado == null)
            {
                return HttpNotFound();
            }

            FillCombos(empleado);
            ViewBag.idEmpleado = id;
            ViewBag.ViewOnly = false;
            return View("Edit", empleado);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ResourceAuthorize(DataOperation.Update, Controllers.Empleados)]
        public ActionResult Edit([Bind(Include = "Id,Login,Password,Administrador, Modificacion, Consulta, Nombre, Apellidos, Email")] Empleado model)
        {
            try
            {
                var passwordChanged = false;

                var dbEntity = Db.Empleados.Find(model.Id);

                if (dbEntity == null)
                {
                    return HttpNotFound();
                }

                dbEntity.Login = model.Login;

                if (model.Password != dbEntity.Password)
                {
                    passwordChanged = true;
                }

                dbEntity.Password = model.Password;
                dbEntity.Administrador = model.Administrador;
                dbEntity.Modificacion = model.Modificacion;
                dbEntity.Consulta = model.Consulta;
                dbEntity.Nombre = model.Nombre;
                dbEntity.Apellidos = model.Apellidos;

                Db.SaveChanges();

                string userRoles = "";
                if (dbEntity.Administrador) { userRoles += "GestorEmpleado"; }
                if (dbEntity.Modificacion) { if (userRoles == "") { userRoles += "GestorModificacion"; } else { userRoles += ",GestorModificacion"; } }
                if (dbEntity.Consulta) { if (userRoles == "") { userRoles += "GestorConsulta"; } else { userRoles += ",GestorConsulta"; } }
                userRoles = "\"" + userRoles + "\"";

                Process p = new Process();
                string paramsLlamadaEjecutableECAuthCli = "delete-add-claims " +
                    " -u " + dbEntity.Login +
                    " -d \"GestorEmpleado,GestorModificacion,GestorConsulta\" " +
                    " -a " + userRoles;

                if (passwordChanged) paramsLlamadaEjecutableECAuthCli += " -p " + model.Password;

                try
                {
                    p.StartInfo.FileName = ConfigurationManager.AppSettings["RutaEjecutableECAuthCLI"].ToString();
                    p.StartInfo.Arguments = paramsLlamadaEjecutableECAuthCli;
                    bool resultado = p.Start();

                }
                catch (Exception)
                {
                   
                }

                return RedirectToAction("Index");

            }
            catch (Exception)
            {
            
            }
            return View(model);
        }

        [ResourceAuthorize(DataOperation.Create, Controllers.Empleados)]
        public ActionResult Create()
        {
            FillCombos();
            ViewBag.ViewOnly = false;
            var entidad = new Empleado();
            
            return View("Edit", entidad);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ResourceAuthorize(DataOperation.Create, Controllers.Empleados)]
        public ActionResult Create([Bind(Include = "Id,Login, Password, Nombre,Apellidos,Email, Administrador, Modificacion, Consulta")] Empleado modelo)
        {
            if (ModelState.IsValid)
            {
                var result = validator.Validate(modelo);
                if (result.Valid)
                {
                    Db.Empleados.Add(modelo);
                    Db.SaveChanges();

                    Process p = new Process();

                    string userRoles = "Usuario";
                    if (modelo.Administrador) { userRoles += ",GestorEmpleado"; }
                    if (modelo.Modificacion) { userRoles += ",GestorModificacion"; }
                    if (modelo.Consulta) { userRoles += ",GestorConsulta"; }
                    userRoles = "\"" + userRoles + "\"";


                    string paramsLlamadaEjecutableECAuthCli = "add-account " +
                        " -u " + modelo.Login + " -e " + modelo.Email + " -p " + modelo.Password +
                        " -r " + userRoles +
                        " -c \"id;" + modelo.Id.ToString() + ",family_name;" + modelo.Login + ",given_name;" + modelo.NombreCompleto.ToUpper() + ",name;" + modelo.NombreCompleto.ToUpper() + "\"";

                    try
                    {
                        p.StartInfo.FileName = ConfigurationManager.AppSettings["RutaEjecutableECAuthCLI"].ToString();
                        p.StartInfo.Arguments = paramsLlamadaEjecutableECAuthCli;
                        bool resultado = p.Start();
                    }

                    catch (Exception)
                    {
                       
                    }


                    return RedirectToAction("Index");
                }
                result.ValidationErrors.ToList().ForEach(v => ModelState.AddModelError(v.Key, v.Value));
            }

            FillCombos();
            return View("Edit", modelo);
        }

        [ResourceAuthorize(DataOperation.Delete, Controllers.Empleados)]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var entity = Db.Empleados.Find(id);
            if (entity == null)
            {
                return HttpNotFound();
            }
            return View(entity);
        }

        // METODO DE ACTIVACION DEL BORRADO
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ResourceAuthorize(DataOperation.Delete, Controllers.Empleados)]
        public virtual ActionResult Activate(int id)
        {
            var entity = Db.Empleados.Find(id);
            if (entity != null)
            {
                PreloadVirtualProps(entity);
                entity.Activo = true;
                Db.SaveChanges();

                Process p = new Process();

                string userRoles = "Usuario";
                if (entity.Administrador) { userRoles += ",GestorEmpleado"; }
                if (entity.Modificacion) { userRoles += ",GestorModificacion"; }
                if (entity.Consulta) { userRoles += ",GestorConsulta"; }
                userRoles = "\"" + userRoles + "\"";


                string paramsLlamadaEjecutableECAuthCli = "add-account " +
                    " -u " + entity.Login + " -e " + entity.Email + " -p " + entity.Password +
                    " -r " + userRoles +
                    " -c \"id;" + entity.Id.ToString() + ",family_name;" + entity.Login + ",given_name;" + entity.NombreCompleto.ToUpper() + ",name;" + entity.NombreCompleto.ToUpper() + "\"";

                try
                {
                    p.StartInfo.FileName = ConfigurationManager.AppSettings["RutaEjecutableECAuthCLI"].ToString();
                    p.StartInfo.Arguments = paramsLlamadaEjecutableECAuthCli;
                    bool resultado = p.Start();
                }

                catch (Exception)
                {
                    throw;
                }
            }
            return RedirectToAction("Index");
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [ResourceAuthorize(DataOperation.Delete, Controllers.Empleados)]
        public ActionResult DeleteConfirmed(int id)
        {
            var entity = Db.Empleados.Find(id);
            if (entity != null)
            {
                PreloadVirtualProps(entity);
                entity.Activo = false;
                Db.SaveChanges();

                Process p = new Process();
                string paramsLlamadaEjecutableECAuthCli = "delete-claims " +
                    " -u " + entity.Login;

                try
                {
                    p.StartInfo.FileName = ConfigurationManager.AppSettings["RutaEjecutableECAuthCLI"].ToString();
                    p.StartInfo.Arguments = paramsLlamadaEjecutableECAuthCli;
                    bool resultado = p.Start();

                }
                catch (Exception)
                {
                    throw;
                }

                p = new Process();

                paramsLlamadaEjecutableECAuthCli = "delete-account " +
                " -w True" + 
                " -u " + entity.Email;

                try
                {
                    p.StartInfo.FileName = ConfigurationManager.AppSettings["RutaEjecutableECAuthCLI"].ToString();
                    p.StartInfo.Arguments = paramsLlamadaEjecutableECAuthCli;
                    bool resultado = p.Start();
                }

                catch (Exception)
                {
                    throw;
                }
            }
            return RedirectToAction("Index");
        }

        protected override void FillCombos(Empleado  entidad = null)
        {
            base.FillCombos(entidad);

            if (entidad != null)
            {
                PutEntityInViewBag<Empleado>(entidad.GetId(), true, "BuscarEmpleado", m=> m.Activo == true);
            }
          else
            {
                PutEntityInViewBag<Empleado>(null, true, "BuscarEmpleado", m => m.Activo == true);
            }
        }

        [ResourceAuthorize(DataOperation.Read, Controllers.Empleados)]
        public ActionResult Search(int? idEmpleado, BusquedaEmpleado model, int? page)
        {
            var skip = (page - 1) * base.RowsPerPage ?? 0;

            var list = Db.Empleados.AsQueryable();
            ViewBag.TotalRowCount = list.Count();

            if (list == null)
            {
                return HttpNotFound();
            }

            if (idEmpleado.HasValue)
            {
                list = list.Where(e => e.Id == idEmpleado);
            }

            if (!string.IsNullOrWhiteSpace(model.Login))
            {
                list = list.Where(e => e.Login.Contains(model.Login));
            }

            if (model.Activo)
            {
                list = list.Where(e => e.Activo == true);
            }
            else
            {
                list = list.Where(e => e.Activo == false);
            }

            if (model.Administrador)
            {
                list = list.Where(e => e.Administrador == true);
            }

            if (model.Modificacion)
            {
                list = list.Where(e => e.Modificacion == true);
            }

            if (model.Consulta)
            {
                list = list.Where(e => e.Consulta == true);
            }
         
            list = list.OrderBy(e => e.Nombre);

            ViewBag.SearchCount = list.Count();
            ViewBag.SearchEntity = model;


            if (list.Count() == 1)
            {
                var entidad = Db.Set<Empleado>().Find(list.FirstOrDefault().Id);

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
    }
}