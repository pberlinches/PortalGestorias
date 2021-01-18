using System;
using System.Linq;
using System.Web.Mvc;
using PortalGestorias.Domain.Entities;
using PortalGestorias.Domain.Models;
using PortalGestorias.Infrastructure.Data;
using PortalGestorias.Business.Validations;
using System.Security.Claims;
using OfficeOpenXml;
using System.Collections.Generic;
using System.Reflection;
using OfficeOpenXml.Table;
using System.IO;
using System.Configuration;

namespace PortalGestorias.Web.Controllers
{
    public abstract class BusinessController<T, S> : DataControllerBase<T> where T : BusinessEntity, new() where S : BusinessEntity, new()
    {

        protected new IEntityValidator<T> validator;
        protected EntitySearchBase<S> searchEntity;


        protected virtual object RouteValues
        {
            get
            {
                return null;
            }
        }

        protected new int UserId
        {
            get
            {
                return Convert.ToInt32(((ClaimsPrincipal)User).FindFirst("id").Value);
            }
        }

        protected BusinessController(CrmDbContext dbContext, IEntityValidator<T> validator)
            : base(dbContext)
        {

            this.validator = validator;

        }

        public virtual ActionResult Index(int? page)
        {
            var skip = (page - 1) * RowsPerPage ?? 0;
            var entidades = DefaultSearch();

            ViewBag.TotalRowCount = entidades.Count();
            ViewBag.SearchCount = entidades.Count();

            if (this.searchEntity != null)
                ViewBag.SearchEntity = BuildSearchEntity();
            else
                ViewBag.SearchEntity = BuildDefaultSearchEntity();


            ViewBag.ListaExcel = entidades;

            ViewBag.listProductos = String.Empty;

            var entities = entidades.Select(m => m.Id).ToList();

            ViewBag.listProductos = string.Join("|", entities);

            FillViewBag();

            return View(entidades.Skip(skip).Take(RowsPerPage).ToList());
        }

        public virtual ActionResult SearchEntity(T entidad, int? page)
        {
            var skip = (page - 1) * RowsPerPage ?? 0;
            var lista = SearchEntities(entidad);

            if (lista == null) return BadRequest();

            if (entidad.Activo)
            {
                lista = lista.Where(e => e.Activo == true);
            }

            ViewBag.TotalRowCount = lista.Count();
            ViewBag.SearchEntity = entidad;

            lista = OrderEntities(lista);
            ViewBag.ListaExcel = lista;

            return View("Index", lista.Skip(skip).Take(RowsPerPage).ToList());
        }

        public virtual ActionResult Details(int? id, bool? listadoModelos, bool? listadoProductos)
        {
            if (id == null)
            {
                return BadRequest();
            }

            var entidad = Db.Set<T>().Find(id);
            if (entidad == null)
            {
                return HttpNotFound();
            }

            ViewBag.ViewOnly = true;
            ViewBag.Titulo = entidad?.DefaultValue;

            FillViewBag(entidad);

            ViewBag.ListadoModelos = listadoModelos;
            ViewBag.ListadoProductos = listadoProductos;

            return View("Edit", entidad);
        }

        public virtual ActionResult Create()
        {
            T defaultEntity = BuildDefaultEntity();
            FillViewBag(defaultEntity);
            ViewBag.Titulo = "Alta";
            return View("Edit", defaultEntity);
        }

        public virtual ActionResult Create(T model)
        {
            if (ModelState.IsValid)
            {

                var navResult = NavigationPropertiesValidation(model);

                if (navResult != null) return navResult;

                var result = validator.Validate(model);
                if (result.Valid)
                {
                    Db.Set<T>().Add(model);
                    Db.SaveChanges();
                    return RedirectToAction("Index", RouteValues);
                }
                result.ValidationErrors.ToList().ForEach(v => ModelState.AddModelError(v.Key, v.Value));
            }

            FillCombos(model);
            ViewBag.Titulo = "Alta";
            return View("Edit", model);
        }

        public virtual ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }

            var entidad = Db.Set<T>().Find(id);
            if (entidad == null)
            {
                return HttpNotFound();
            }

            var result = validator.Validate(entidad);
            if (!result.Valid)
            {
                SetNotValidState(result);
            }

            FillViewBag(entidad);
            ViewBag.Titulo = "Editar " + entidad?.DefaultValue;

            return View("Edit", entidad);
        }

        public virtual ActionResult Edit(T model)
        {
            if (ModelState.IsValid)
            {
                var navResult = NavigationPropertiesValidation(model);
                if (navResult != null) return navResult;

                var dbEntity = Db.Set<T>().ApplyValues(model, model.Id);
                if (dbEntity == null)
                {
                    return HttpNotFound();
                }

                var result = validator.Validate(dbEntity);
                if (!result.Valid)
                {
                    SetNotValidState(result);
                    FillViewBag(model);
                    ViewBag.Titulo = "Editar " + dbEntity?.DefaultValue;
                    return View(dbEntity);
                }

                Db.SaveChanges();
                return RedirectToAction("Index", RouteValues);
            }

            FillViewBag(model);
            ViewBag.Titulo = "Editar " + model?.DefaultValue;
            return View(model);
        }

        public virtual ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }

            var entity = Db.Set<T>().Find(id);
            if (entity == null)
            {
                return HttpNotFound();
            }

            var result = validator.DeleteValidate(entity);
            if (!result.Valid)
            {
                SetNotValidState(result);
            }

            FillViewBag(entity);

            return View(entity);
        }

        public virtual ActionResult DeleteConfirmed(int id)
        {
            var entity = Db.Set<T>().Find(id);
            if (entity != null)
            {
                PreloadVirtualProps(entity);
                entity.Activo = false;
                Db.SaveChanges();
            }
            return RedirectToAction("Index", RouteValues);
        }

        public virtual ActionResult Activate(int id)
        {
            var entity = Db.Set<T>().Find(id);
            if (entity != null)
            {
                PreloadVirtualProps(entity);
                entity.Activo = true;
                Db.SaveChanges();
            }
            return RedirectToAction("Index", RouteValues);
        }

        protected virtual T BuildDefaultEntity()
        {
            return new T();
        }

        protected virtual T BuildDefaultSearchEntity()
        {
            return new T();
        }

        protected virtual S BuildSearchEntity()
        {
            return new S();
        }

        protected virtual ActionResult NavigationPropertiesValidation(T entity)
        {
            return null;
        }

        protected virtual void FillViewBag(T entidad = null)
        {
            ViewBag.IdEntidad = entidad?.Id;
            FillCombos(entidad);
        }

        protected virtual IQueryable<T> DefaultSearch(IQueryable<T> lista = null)
        {
            var entidades = lista ?? Db.Set<T>().AsQueryable();

            entidades = entidades.Where(s => s.Activo);

            return OrderEntities(entidades);
        }

        protected virtual IQueryable<T> SearchEntities(T entidad)
        {
            return Db.Set<T>().AsQueryable();
        }

        protected virtual IQueryable<T> OrderEntities(IQueryable<T> lista)
        {
            return lista.OrderBy(c => c.Id);
        }

       
    }
}