using PortalGestorias.Domain.Entities;
using PortalGestorias.Infrastructure.Data;
using System.Linq;

namespace PortalGestorias.Business.Validations
{
   public class ModeloValidator : EntityValidatorBase<Modelo>
    {
        public ModeloValidator(CrmDbContext context) : base(context)
        {
        }

        public override EntityState Validate(Modelo entity)
        {
            var entityState = base.Validate(entity);

            if (Db.Modelos.Any(d => d.Activo == true && d.Id != entity.Id && d.Nombre.ToUpper() == entity.Nombre.ToUpper()))
            {
                entityState.ValidationErrors.Add(
                   "Nombre",
                   "Ya existe un Modelo con el nombre indicado.");
            }

            if (entity.TipoModelo == null)
            {
                entityState.ValidationErrors.Add(
                  "TipoModelo",
                  "El TipoModelo no puede estar vacío");
            }

           


            entityState.Valid = entityState.ValidationErrors.Count == 0;

            return entityState;
        }
    }
}
