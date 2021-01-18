using PortalGestorias.Domain.Entities;
using PortalGestorias.Infrastructure.Data;
using PortalGestorias.Domain.Models;
using System.Linq;

namespace PortalGestorias.Business.Validations
{
   public class EstadoValidator : EntityValidatorBase<Estado>
    {
        public EstadoValidator(CrmDbContext context) : base(context)
        {
        }

        public override EntityState Validate(Estado entity)
        {
            var entityState = base.Validate(entity);

            if (Db.Estados.Any(d => d.Activo == true && d.Id != entity.Id && d.Nombre.ToUpper() == entity.Nombre.ToUpper()))
            {
                entityState.ValidationErrors.Add(
                   "Nombre",
                   "Ya existe un Estado con el nombre indicado.");
            }

            entityState.Valid = entityState.ValidationErrors.Count == 0;
            return entityState;
        }
    }
}
