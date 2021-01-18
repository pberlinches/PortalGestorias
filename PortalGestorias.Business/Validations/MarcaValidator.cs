using PortalGestorias.Domain.Entities;
using PortalGestorias.Infrastructure.Data;
using System.Linq;

namespace PortalGestorias.Business.Validations
{
   public class MarcaValidator : EntityValidatorBase<Marca>
    {
        public MarcaValidator(CrmDbContext context) : base(context)
        {
        }

        public override EntityState Validate(Marca entity)
        {
            var entityState = base.Validate(entity);

            if (Db.Marcas.Any(d => d.Activo == true && d.Id != entity.Id && d.Nombre.ToUpper() == entity.Nombre.ToUpper()))
            {
                entityState.ValidationErrors.Add(
                   "Nombre",
                   "Ya existe una Marca con el nombre indicado.");
            }


            entityState.Valid = entityState.ValidationErrors.Count == 0;
            return entityState;
        }
    }
}
