using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using Castle.Core.Internal;
using PortalGestorias.Domain;
using PortalGestorias.Infrastructure.Data;
using PortalGestorias.Web.Models;
using PortalGestorias.Domain.Entities;
using System.Linq.Expressions;

namespace PortalGestorias.Web.Controllers
{
    public abstract class CrmControllerBase<T> : Controller
    {
        protected CrmDbContext Db;
        protected string EntityName;

        protected int RowsPerPage
        {
            get { return 20; } 
        }

        protected CrmControllerBase(CrmDbContext dbContext)
        {
            EntityName = typeof(T).HasAttribute<ClassNameAttribute>() ? typeof(T).GetAttribute<ClassNameAttribute>().Name : typeof(T).Name;
            Db = dbContext;
            Breadcrumb = new Breadcrumb();
            Breadcrumb.Items.Enqueue(new BreadcrumbItem { Text = $"{EntityName}", Action = "Index" });
            ViewBag.Breadcrumb = Breadcrumb;
            ViewBag.RowsPerPage = RowsPerPage;
        }

        public Breadcrumb Breadcrumb { get; set; }

        protected override IAsyncResult BeginExecute(RequestContext requestContext, AsyncCallback callback, object state)
        {
            if (requestContext.HttpContext.Request.HttpMethod.Equals("GET", StringComparison.InvariantCultureIgnoreCase))
            {
                var actionName = requestContext.RouteData.Values["action"].ToString();
                var breadcrumbText = GetBreadcrumbText(actionName);
                AddBreadcrumb(breadcrumbText, string.Empty);
            }

            return base.BeginExecute(requestContext, callback, state);
        }

        private string GetBreadcrumbText(string actionName)
        {
            actionName = actionName.ToLowerInvariant();
            var breadcrumbText = string.Empty;
            switch (actionName)
            {
                case "details":
                    breadcrumbText = "Consulta";
                    break;
                case "create":
                    breadcrumbText = $"Nuev{(EntityName.Equals("actividad", StringComparison.InvariantCultureIgnoreCase) ? "a" : "o")}";
                    break;
                case "edit":
                    breadcrumbText = "Editar";
                    break;
                case "delete":
                    breadcrumbText = "Dar de Baja";
                    break;
                case "index":
                case "search":
                    breadcrumbText = "B�squeda";
                    break;
            }
            return breadcrumbText;
        }

        private void AddBreadcrumb(string text, string action)
        {
            Breadcrumb.Items.Enqueue(new BreadcrumbItem { Text = text, Action = action });
        }

    
        protected void PutEntityInViewBag<E>(int? selectedEntity, bool addGenericAll = false, string key = null, Expression<Func<E, bool>> predicate = null) 
            where E : BusinessEntity
        {
            var childEntityName = typeof(E).HasAttribute<ClassNameAttribute>() ? typeof(E).GetAttribute<ClassNameAttribute>().Name : typeof(E).Name;

            var dbList = Db.Set<E>().AsQueryable().Where(m => m.Activo == true);
            
            if (predicate != null)
            {
                dbList = dbList.Where(predicate);
            }

            dbList = dbList.Where(e => e.Activo == true);
          
            var list = dbList.ToList().Select(i => new { Key = (int?)i.Id, Value = i.DefaultValue }).OrderBy(e=>e.Value).ToList();

            if (addGenericAll)
            {
                var genericValue = string.Empty;
                genericValue = "Seleccione";
                list.Insert(0, new { Key = (int?)null, Value = genericValue });
            }

            ViewData.Add(key ?? childEntityName, new SelectList(list, "Key", "Value", selectedEntity));
        }

        protected void PutEntityInViewBagWithoutActivo<E>(int? selectedEntity, bool addGenericAll = false, string key = null, Expression<Func<E, bool>> predicate = null)
            where E : BusinessEntity
        {
            var childEntityName = typeof(E).HasAttribute<ClassNameAttribute>() ? typeof(E).GetAttribute<ClassNameAttribute>().Name : typeof(E).Name;

            var dbList = Db.Set<E>().AsQueryable();

            if (predicate != null)
            {
                dbList = dbList.Where(predicate);
            }

            var list = dbList.ToList().Select(i => new { Key = (int?)i.Id, Value = i.DefaultValue }).OrderBy(e => e.Value).ToList();

            if (addGenericAll)
            {
                var genericValue = string.Empty;
                genericValue = "Seleccione";
                list.Insert(0, new { Key = (int?)null, Value = genericValue });
            }

            ViewData.Add(key ?? childEntityName, new SelectList(list, "Key", "Value", selectedEntity));
        }

       
        protected void PutEntityInViewBagOrdered<E>(int? selectedEntity, bool addGenericAll = false, string key = null, Expression<Func<E, bool>> predicate = null, Expression<Func<E, int>> order = null)
           where E : BusinessEntity
        {
            var childEntityName = typeof(E).HasAttribute<ClassNameAttribute>() ? typeof(E).GetAttribute<ClassNameAttribute>().Name : typeof(E).Name;

            var dbList = Db.Set<E>().AsQueryable();
            
            if (predicate != null)
            {
                dbList = dbList.Where(predicate);
            }

            if (order != null)
            {
                dbList = dbList.OrderBy(order);
            }

            dbList = dbList.Where(e => e.Activo == true);

            var list = dbList.ToList().Select(i => new { Key = (int?)i.Id, Value = i.DefaultValue }).ToList();

            if (addGenericAll)
            {
                var genericValue = string.Empty;
                genericValue = "Seleccione";
                list.Insert(0, new { Key = (int?)null, Value = genericValue});
            }

            ViewData.Add(key ?? childEntityName, new SelectList(list, "Key", "Value", selectedEntity));
        }
    }
}