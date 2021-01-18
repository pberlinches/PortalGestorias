using PortalGestorias.Domain.Entities;
using PortalGestorias.Infrastructure.Data;
using System.Linq;

namespace PortalGestorias.Business.Validations
{
   public class StockRoomValidator : EntityValidatorBase<StockRoom>
    {
        public StockRoomValidator(CrmDbContext context) : base(context)
        {
        }

        public override EntityState Validate(StockRoom entity)
        {
            var entityState = base.Validate(entity);


            if (Db.StockRooms.Any(d => d.Activo == true && d.Id != entity.Id && d.Codigo.ToUpper() == entity.Codigo.ToUpper()))
            {
                entityState.ValidationErrors.Add(
                   "Codigo",
                   "Ya existe un StockRoom con el código indicado.");
            }

            entityState.Valid = entityState.ValidationErrors.Count == 0;
            return entityState;
        }
    }
}
