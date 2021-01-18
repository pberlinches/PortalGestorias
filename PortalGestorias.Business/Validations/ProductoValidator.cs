using PortalGestorias.Domain.Entities;
using PortalGestorias.Infrastructure.Data;
using System.Linq;

namespace PortalGestorias.Business.Validations
{
   public class ProductoValidator : EntityValidatorBase<Producto>
    {
        public ProductoValidator(CrmDbContext context) : base(context)
        {
        }

        public override EntityState Validate(Producto entity)
        {
            var entityState = base.Validate(entity);

            if (string.IsNullOrEmpty(entity.NumeroSerie))
            {
                entityState.ValidationErrors.Add(
               "NumeroSerie",
               "El Nº de Serie no puede estar vacío");
            }


            if (string.IsNullOrEmpty(entity.CodigoBarras))
            {
                entityState.ValidationErrors.Add(
               "CodigoBarras",
               "El Código de Barras no puede estar vacío");
            }

            if (Db.Productos.Any(d => d.Activo == true && d.Id != entity.Id && d.CodigoBarras.ToUpper() == entity.CodigoBarras.ToUpper()))
            {
                entityState.ValidationErrors.Add(
                   "CodigoBarras",
                   "Ya existe un Producto con el Código de Barras indicado.");
            }

            if (Db.Productos.Any(d => d.Activo == true && d.Id != entity.Id && d.NumeroSerie.ToUpper() == entity.NumeroSerie.ToUpper()))
            {
                entityState.ValidationErrors.Add(
                   "NumeroSerie",
                   "Ya existe un Producto con el Nº de Serie indicado.");
            }

            if (entity.Modelo == null)
            {
                entityState.ValidationErrors.Add(
                  "Modelo",
                  "El modelo no puede estar vacío");
            }

            if (entity.Marca == null)
            {
                entityState.ValidationErrors.Add(
                  "Marca",
                  "La marca no puede estar vacía");
            }

            if (entity.Ubicacion == null)
            {
                entityState.ValidationErrors.Add(
                  "Ubicacion",
                  "La Ubicación no puede estar vacía");
            }

            if (entity.Almacen == null)
            {
                entityState.ValidationErrors.Add(
                  "Almacen",
                  "El Almacén no puede estar vacío");
            }

            if (entity.Departamento == null)
            {
                entityState.ValidationErrors.Add(
                  "Departamento",
                  "El Departamento no puede estar vacío");
            }

            if (entity.Estado == null)
            {
                entityState.ValidationErrors.Add(
                  "Estado",
                  "El Estado no puede estar vacío");
            }


            if (string.IsNullOrEmpty(entity.NumeroPedido))
            {
                entityState.ValidationErrors.Add(
                  "NumeroPedido",
                  "El Numero de Pedido no puede estar vacío");
            }

            entityState.Valid = entityState.ValidationErrors.Count == 0;
            return entityState;
        }
    }
}
