using System;
using System.Web.Mvc;
using PortalGestorias.Domain.Entities;
using PortalGestorias.Infrastructure.Data;
using PortalGestorias.Web.Controllers;

namespace PortalGestorias.Web.Models
{
    public class DatoMaestroModelBinder : IModelBinder
    {
        private readonly Func<CrmDbContext> dbFactory;

        public DatoMaestroModelBinder(Func<CrmDbContext> factory)
        {
            dbFactory = factory;
        }

        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var dbContext = dbFactory.Invoke();
            if (controllerContext.Controller is DatosMaestrosController)
            {
                var bindedModel = ModelBinders.Binders.DefaultBinder.BindModel(controllerContext, bindingContext);
                return bindedModel;
            }
            var valueResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            if (valueResult != null && valueResult.AttemptedValue.Contains("|"))
            {
                var values = valueResult.AttemptedValue.Split('|');
                var datoMaestro = values.Length == 2
                    ? dbContext.DatosMaestros.Find(int.Parse(values[0]), int.Parse(values[1])) ?? new DatoMaestro {IdGrupo = int.Parse(values[0]), IdCode = int.Parse(values[1])}
                    : null;
                return datoMaestro;
            }
            return null;
        }
    }
}