using PortalGestorias.Domain.Entities;
using PortalGestorias.Infrastructure.Data;
using System.Linq;

namespace PortalGestorias.Business.Validations
{
   public class AlmacenValidator : EntityValidatorBase<Almacen>
    {
        public AlmacenValidator(CrmDbContext context) : base(context)
        {
        }

        public override EntityState Validate(Almacen entity)
        {
            var entityState = base.Validate(entity);

            if (Db.Almacenes.Any(d => d.Activo == true && d.Id != entity.Id && d.Nombre.ToUpper() == entity.Nombre.ToUpper()))
            {
                entityState.ValidationErrors.Add(
                   "Nombre",
                   "Ya existe un Almacén con el nombre indicado.");
            }

            entityState.Valid = entityState.ValidationErrors.Count == 0;
            return entityState;
        }
    }
}
