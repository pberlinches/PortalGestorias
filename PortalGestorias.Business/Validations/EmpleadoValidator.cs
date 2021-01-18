using PortalGestorias.Domain.Entities;
using PortalGestorias.Infrastructure.Data;
using System.Linq;

namespace PortalGestorias.Business.Validations
{
   public class EmpleadoValidator : EntityValidatorBase<Empleado>
    {
        public EmpleadoValidator(CrmDbContext context) : base(context)
        {
        }

        public override EntityState Validate(Empleado entity)
        {
            var entityState = base.Validate(entity);

            if (Db.Empleados.Any(d => d.Activo == true && d.Id != entity.Id && d.Login.ToUpper() == entity.Login.ToUpper()))
            {
                entityState.ValidationErrors.Add(
                   "Login",
                   "Ya existe un Usuario con el login indicado.");
            }

            if (Db.Empleados.Any(d => d.Activo == true && d.Id != entity.Id && d.Email.ToUpper() == entity.Email.ToUpper()))
            {
                entityState.ValidationErrors.Add(
                   "Email",
                   "Ya existe un Usuario con el email indicado.");
            }


            entityState.Valid = entityState.ValidationErrors.Count == 0;
            return entityState;
        }
    }
}
