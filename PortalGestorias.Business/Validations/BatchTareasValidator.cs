using PortalGestorias.Domain.Entities;
using PortalGestorias.Infrastructure.Data;
using System.Linq;

namespace PortalGestorias.Business.Validations
{
   public class BatchTareasValidator : EntityValidatorBase<BatchTarea>
    {
        public BatchTareasValidator(CrmDbContext context) : base(context)
        {
        }

        public override EntityState Validate(BatchTarea entity)
        {
            var entityState = base.Validate(entity);

            if (Db.BatchTareas.Any(c => c.TipoTarea == null && c.Id != entity.Id))
            {
                entityState.ValidationErrors.Add(
                    "TipoTarea",
                    "La tarea debe tener asiganada un tipo de tarea");
            }

            entityState.Valid = entityState.ValidationErrors.Count == 0;
            return entityState;
        }
    }
}
