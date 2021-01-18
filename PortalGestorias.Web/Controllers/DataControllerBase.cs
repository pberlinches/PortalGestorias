using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Web.Mvc;
using PortalGestorias.Domain.Entities;
using PortalGestorias.Infrastructure.Data;
using PortalGestorias.Business.Validations;
using EntityState = PortalGestorias.Business.Validations.EntityState;
using System.Security.Claims;

namespace PortalGestorias.Web.Controllers
{
    public abstract class DataControllerBase<T> : CrmControllerBase<T>    where T: TrackeableEntity,  new()
    {
        protected IEntityValidator<T> validator;

        protected int UserId
        {
            get
            {
                return Convert.ToInt32(((ClaimsPrincipal)User).FindFirst("id").Value);
            }
        }

        protected DataControllerBase(CrmDbContext dbContext)
            : base(dbContext)
        {
        }

        protected void SetNotValidState(EntityState result)
        {
            result.ValidationErrors.ToList().ForEach(v => ModelState.AddModelError(v.Key, v.Value));
            ViewBag.ViewOnly = false;
        }

        protected T PreloadVirtualProps(T entity)
        {
            if (entity == null)
            {
                throw new NullReferenceException("Entity not found in DB");
            }
            var properties = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(p => !(p.PropertyType.IsInterface && p.PropertyType.IsGenericType &&
                              p.PropertyType.GetGenericTypeDefinition() == typeof(ICollection<>))
                              && (p.CanRead ? p.GetMethod : p.SetMethod).IsVirtual).ToList();
            properties.ForEach(p =>
            {
                p.GetValue(entity);
            });

            return entity;
        }


        protected ActionResult BadRequest()
        {
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }

        protected virtual void FillCombos(T entidad = null)
        {
        }
    }
}