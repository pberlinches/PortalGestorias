using PortalGestorias.Domain.Entities;
using PortalGestorias.Infrastructure.Data;
using System.Linq;

namespace PortalGestorias.Business.Validations
{
    public class UsuarioValidator : EntityValidatorBase<Usuario>
    {
        public UsuarioValidator (CrmDbContext context) : base(context)
        {
        }

        public override EntityState Validate(Usuario entity)
        {
            var entityState = base.Validate(entity);

            entityState.Valid = entityState.ValidationErrors.Count == 0;
            return entityState;
        }
    }
}
