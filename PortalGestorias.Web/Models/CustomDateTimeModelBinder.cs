using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web.Mvc;
using PortalGestorias.Domain.Entities;
using PortalGestorias.Infrastructure.Data;
using PortalGestorias.Web.Controllers;

namespace PortalGestorias.Web.Models
{
    public class CustomDateTimeModelBinder : IModelBinder
    {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var displayFormat = bindingContext.ModelMetadata.DisplayFormatString ?? "dd/MM/yyyy";
            var value = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);

            if (!string.IsNullOrEmpty(displayFormat) && value != null)
            {
                DateTime date;
                displayFormat = displayFormat.Replace
                ("{0:", string.Empty).Replace("}", string.Empty);
                if (DateTime.TryParseExact(value.AttemptedValue, displayFormat,
                CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
                {
                    return date;
                }
                else
                {
                    bindingContext.ModelState.AddModelError(
                        bindingContext.ModelName,
                        string.Format("{0} is an invalid date format", value.AttemptedValue)
                    );
                }
            }

            return DateTime.MinValue;
        }
    }
}