using PortalGestorias.Domain.Entities;
using PortalGestorias.Infrastructure.Data;

namespace PortalGestorias.Business.Validations
{
    public class EntitySearchBase<S> : IEntitySearch<S>
        where S : BusinessEntity, new()
    {

        protected CrmDbContext Db;

        public EntitySearchBase(CrmDbContext context)
        {
            Db = context;
        }
    }
}