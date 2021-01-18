using PortalGestorias.Domain.Entities;

namespace PortalGestorias.Business.Validations
{
    public interface IEntityValidator<in T>
        where T : TrackeableEntity, new()
    {
        EntityState Validate(T entity);

        EntityState DeleteValidate(T entity);
    }
}