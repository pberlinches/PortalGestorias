using PortalGestorias.Domain.Entities;
using PortalGestorias.Infrastructure.Data;

namespace PortalGestorias.Business.Validations
{
    public abstract class EntityValidatorBase<T> : IEntityValidator<T>
        where T : TrackeableEntity, new()
    {

        protected CrmDbContext Db;

        public EntityValidatorBase(CrmDbContext context)
        {
            Db = context;
        }

        public virtual EntityState Validate(T entity)
        {
            var result = new EntityState { Valid = true };

            if (!entity.Activo)
            {
                result.Valid = false;
                result.ValidationErrors.Add(ErrorConstants.EntityNotActiveKey, ErrorConstants.EntityNotActive);
            }

            return result;
        }

        public virtual EntityState DeleteValidate(T entity)
        {
            var result = new EntityState { Valid = true };

            return result;
        }
    }
}