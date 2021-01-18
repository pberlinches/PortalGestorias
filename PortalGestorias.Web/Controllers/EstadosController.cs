using System.Linq;
using System.Web.Mvc;
using PortalGestorias.Business.Validations;
using PortalGestorias.Domain.Entities;
using PortalGestorias.Domain.Models;
using PortalGestorias.Infrastructure.Data;
using PortalGestorias.Web.Security;
using Thinktecture.IdentityModel.Mvc;


namespace PortalGestorias.Web.Controllers
{
    public class EstadosController : BusinessController<Estado, BusquedaEstado>
    {
        protected string action;


        public EstadosController(CrmDbContext dbContext, IEntityValidator<Estado> validator)
             : base(dbContext, validator)
        {
            this.validator = validator;
            this.searchEntity = new EntitySearchBase<BusquedaEstado>(dbContext);
        }

        [ResourceAuthorize(DataOperation.Read, Controllers.Estados)]
        public override ActionResult Index(int? page) => base.Index(page);

        [ResourceAuthorize(DataOperation.Read, Controllers.Estados)]
        public override ActionResult Details(int? id, bool? listadoModelos, bool? listadoProductos) => base.Details(id, listadoModelos, listadoProductos);

        [ResourceAuthorize(DataOperation.Create, Controllers.Estados)]
        public override ActionResult Create() => base.Create();

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ResourceAuthorize(DataOperation.Create, Controllers.Estados)]
        public override ActionResult Create([Bind(Include = "Id,Nombre")]Estado model) => base.Create(model);

        [ResourceAuthorize(DataOperation.Update, Controllers.Estados)]
        public override ActionResult Edit(int? id) => base.Edit(id);

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ResourceAuthorize(DataOperation.Update, Controllers.Estados)]
        public override ActionResult Edit([Bind(Include = "Id, Nombre")] Estado model) => base.Edit(model);

        [ResourceAuthorize(DataOperation.Delete, Controllers.Estados)]
        public override ActionResult Delete(int? id) 
        {
            var entity = Db.Estados.Find(id);

            var existe = Db.Productos.Any(m => m.Activo == true && m.Estado.Id == id);

            if (existe)
            {
                ViewBag.Message = "No se puede eliminar el estado " + entity.Nombre + ". Hay productos con este Estado asignado.";
                base.Index(null);
                return View("Index");
            }

            return base.Delete(id);

        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [ResourceAuthorize(DataOperation.Delete, Controllers.Estados)]
        public override ActionResult DeleteConfirmed(int id) => base.DeleteConfirmed(id);

        [ResourceAuthorize(DataOperation.Read, Controllers.Estados)]
        public  ActionResult Search(BusquedaEstado model, int? page)
        {
            var skip = (page - 1) * base.RowsPerPage ?? 0;

            var list = Db.Estados.AsQueryable().Where(a => a.Activo == true);
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
                var entidad = Db.Set<Estado>().Find(list.FirstOrDefault().Id);

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