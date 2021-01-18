using System;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using PortalGestorias.Business.Validations;
using PortalGestorias.Domain.Entities;
using PortalGestorias.Infrastructure.Data;
using PortalGestorias.Web.Security;
using Thinktecture.IdentityModel.Mvc;

namespace PortalGestorias.Web.Controllers
{
    public class DatosMaestrosController : DataControllerBase<DatoMaestro>
    {
        private new  readonly IEntityValidator<DatoMaestro> validator;

        public DatosMaestrosController(CrmDbContext dbContext, IEntityValidator<DatoMaestro> validator)
            : base(dbContext)
        {
            this.validator = validator;
        }

        // GET: DatosMaestros
        public ActionResult Index(int? pageDato, int? pageGrupo)
        {
            SetGruposInViewbag(pageGrupo);


            var skip = (pageDato - 1) * RowsPerPage ?? 0;
            ViewBag.TotalRowCount = Db.DatosMaestros.Count();
            var datosMaestros = Db.DatosMaestros.OrderBy(a => a.Grupo.Nombre).ThenBy(a => a.Value).Skip(skip).Take(RowsPerPage);
            return View(datosMaestros.ToList());
        }

        public ActionResult Search(int? id, int? pageDato, int? pageGrupo)
        {
            SetGruposInViewbag(pageGrupo);

            var datosMaestros = Db.DatosMaestros.AsQueryable();
            if (id != null)
            {
                datosMaestros = datosMaestros.Where(dm => dm.IdGrupo == id.Value);
            }

            var skip = (pageDato - 1) * RowsPerPage ?? 0;
            ViewBag.TotalRowCount = datosMaestros.Count();
            datosMaestros = datosMaestros.OrderBy(a => a.Grupo.Nombre).ThenBy(a => a.Value).Skip(skip).Take(RowsPerPage);
            return View("Index", datosMaestros.ToList());
        }

        [ResourceAuthorize(DataOperation.Create, Controllers.Maestros)]
        public ActionResult CreateGrupo()
        {
            return View("EditGrupo", new GrupoDatosMaestros());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ResourceAuthorize(DataOperation.Create, Controllers.Maestros)]
        public ActionResult CreateGrupo([Bind(Include = "Nombre,Metadatos")] GrupoDatosMaestros grupoDatosMaestros)
        {
            if (ModelState.IsValid)
            {
                Db.GruposDatosMaestros.Add(grupoDatosMaestros);
                Db.SaveChanges();
                return RedirectToAction("Search", new { id = grupoDatosMaestros.Id });
            }
            return View("EditGrupo", grupoDatosMaestros);
        }

        [ResourceAuthorize(DataOperation.Read, Controllers.Maestros)]
        public ActionResult DetailsGrupo(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var grupoDatosMaestros = Db.GruposDatosMaestros.Find(id);
            if (grupoDatosMaestros == null)
            {
                return HttpNotFound();
            }
            ViewBag.ViewOnly = true;
            return View("EditGrupo", grupoDatosMaestros);
        }

        [ResourceAuthorize(DataOperation.Update, Controllers.Maestros)]
        public ActionResult EditGrupo(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var grupoDatosMaestros = Db.GruposDatosMaestros.Find(id);
            if (grupoDatosMaestros == null)
            {
                return HttpNotFound();
            }
            return View(grupoDatosMaestros);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ResourceAuthorize(DataOperation.Update, Controllers.Maestros)]
        public ActionResult EditGrupo([Bind(Include = "Id,Nombre,Metadatos")] GrupoDatosMaestros grupoDatosMaestros)
        {
            if (ModelState.IsValid)
            {
                var grupoDatoMaestro = Db.GruposDatosMaestros.ApplyValues(grupoDatosMaestros, grupoDatosMaestros.Id);
                if (grupoDatoMaestro == null)
                {
                    return HttpNotFound();
                }
                Db.SaveChanges();
                return RedirectToAction("Search", new { id = grupoDatosMaestros.Id });
            }
            return View(grupoDatosMaestros);
        }

        public ActionResult DeleteGrupo(int? idGrupo)
        {
            throw new NotImplementedException();
        }

        // GET: DatosMaestros/Details/5
        [ResourceAuthorize(DataOperation.Read, Controllers.Maestros)]
        public ActionResult Details(int? idCode, int? idGrupo)
        {
            if (idCode == null || idGrupo==null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var datoMaestro = Db.DatosMaestros.Find(idGrupo, idCode);
            if (datoMaestro == null)
            {
                return HttpNotFound();
            }
            ViewBag.IdGrupo = new SelectList(Db.GruposDatosMaestros.Where(g => g.Activo).OrderBy(g => g.Nombre), "Id", "Nombre");
            ViewBag.ViewOnly = true;
            return View("Edit", datoMaestro);
        }

        // GET: DatosMaestros/Create
        [ResourceAuthorize(DataOperation.Create, Controllers.Maestros)]
        public ActionResult Create()
        {
            ViewBag.IdGrupo = new SelectList(Db.GruposDatosMaestros.Where(g => g.Activo).OrderBy(g => g.Nombre), "Id", "Nombre");
            return View("Edit", new DatoMaestro());
        }

        // POST: DatosMaestros/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ResourceAuthorize(DataOperation.Create, Controllers.Maestros)]
        public ActionResult Create([Bind(Include = "IdCode,IdGrupo,Value,Metadata")] DatoMaestro datoMaestro)
        {
            if (ModelState.IsValid)
            {
                datoMaestro.Grupo = Db.GruposDatosMaestros.Find(datoMaestro.IdGrupo);
                Db.DatosMaestros.Add(datoMaestro);
                Db.SaveChanges(discardDatosMaestros: false);
                return RedirectToAction("Index");
            }

            ViewBag.IdGrupo = new SelectList(Db.GruposDatosMaestros.Where(g => g.Activo).OrderBy(g => g.Nombre), "Id", "Nombre", datoMaestro.IdGrupo);
            return View("Edit", datoMaestro);
        }

        // GET: DatosMaestros/Edit/5
        [ResourceAuthorize(DataOperation.Update, Controllers.Maestros)]
        public ActionResult Edit(int? idCode, int? idGrupo)
        {
            if (idCode == null || idGrupo == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var datoMaestro = Db.DatosMaestros.Find(idGrupo, idCode);
            if (datoMaestro == null)
            {
                return HttpNotFound();
            }
            var result = validator.Validate(datoMaestro);
            if (!result.Valid)
            {
                SetNotValidState(result);
            }

            ViewBag.IdGrupo = new SelectList(Db.GruposDatosMaestros.OrderBy(g => g.Nombre), "Id", "Nombre", datoMaestro.IdGrupo);
            return View(datoMaestro);
        }

        // POST: DatosMaestros/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ResourceAuthorize(DataOperation.Update, Controllers.Maestros)]
        public ActionResult Edit([Bind(Include = "IdCode,IdGrupo,Value,Metadata")] DatoMaestro datoMaestro)
        {
            if (ModelState.IsValid)
            {
                datoMaestro.Grupo = Db.GruposDatosMaestros.Find(datoMaestro.IdGrupo);
                var datoMaestroDb = Db.DatosMaestros.ApplyValues(datoMaestro, datoMaestro.IdGrupo, datoMaestro.IdCode);
                if (datoMaestroDb == null)
                {
                    return HttpNotFound();
                }
                var result = validator.Validate(datoMaestroDb);
                if (!result.Valid)
                {
                    SetNotValidState(result);
                    return View(datoMaestro);
                }

                Db.SaveChanges(discardDatosMaestros: false);
                return RedirectToAction("Search", new { id = datoMaestroDb.IdGrupo });
            }
            ViewBag.IdGrupo = new SelectList(Db.GruposDatosMaestros, "Id", "Nombre", datoMaestro.IdGrupo);
            return View(datoMaestro);
        }

        private ActionResult Delete(int? id, int? idInversor, int? idContacto, int? idCriterio)
        {
            throw new NotImplementedException();
        }

        [ResourceAuthorize(DataOperation.Delete, Controllers.Maestros)]
        public ActionResult DeleteDatoMaestro(int? idGrupo, int? idCode)
        {
            if (idCode == null || idGrupo == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var entity = Db.DatosMaestros.Find(idGrupo, idCode);
            if (entity == null)
            {
                return HttpNotFound();
            }

            return View(entity);
        }

        private ActionResult DeleteConfirmed(int id, int? idInversor, int? idContacto, int? idCriterio)
        {
            throw new NotImplementedException();
        }

        [HttpPost, ActionName("DeleteDatoMaestro")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteDatoMaestroConfirmed(int idGrupo, int idCode)
        {
            var entity = Db.DatosMaestros.Find(idGrupo, idCode);
            if (entity != null)
            {
                PreloadVirtualProps(entity);
                entity.Activo = false;
                Db.SaveChanges(false);
            }
            return RedirectToAction("Search", new { id = idGrupo });
        }

        private  ActionResult Activate(int id, int? idInversor, int? idContacto, int? idCriterio)
        {
            throw new NotImplementedException();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ResourceAuthorize(DataOperation.Delete, Controllers.Maestros)]
        public ActionResult ActivateDatoMaestro(int idGrupo, int idCode)
        {
            var entity = Db.DatosMaestros.Find(idGrupo, idCode);
            if (entity != null)
            {
                PreloadVirtualProps(entity);
                entity.Activo = true;
                Db.SaveChanges(false);
            }
            return RedirectToAction("Search", new { id = idGrupo });
        }


        private void SetGruposInViewbag(int? pageGrupo)
        {
            var grupos = Db.GruposDatosMaestros.AsQueryable();
            var skip = (pageGrupo - 1) * RowsPerPage / 2 ?? 0;
            ViewBag.Grupos = grupos.OrderBy(g => g.Nombre).Skip(skip).Take(RowsPerPage / 2).ToList();
            ViewBag.TotalRowsGrupos = Db.GruposDatosMaestros.Count();
            ViewBag.CurrentPageGrupo = pageGrupo;
        }
    }
}
