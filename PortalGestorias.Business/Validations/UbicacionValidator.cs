using PortalGestorias.Domain.Entities;
using PortalGestorias.Infrastructure.Data;
using System.Linq;

namespace PortalGestorias.Business.Validations
{
   public class UbicacionValidator : EntityValidatorBase<Ubicacion>
    {
        public UbicacionValidator(CrmDbContext context) : base(context)
        {
        }

        public override EntityState Validate(Ubicacion entity)
        {
            var entityState = base.Validate(entity);

            if (Db.Ubicaciones.Any(d => d.Activo == true && d.Id != entity.Id && d.Nombre.ToUpper() == entity.Nombre.ToUpper()))
            {
                entityState.ValidationErrors.Add(
                   "Ubicacion.Nombre",
                   "Ya existe una Ubicación con el nombre indicado.");
            }


            entityState.Valid = entityState.ValidationErrors.Count == 0;
            return entityState;
        }
    }
}
