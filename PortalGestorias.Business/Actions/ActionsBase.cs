using PortalGestorias.Domain.Entities;
using PortalGestorias.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortalGestorias.Business.Actions
{
    public class ActionsBase<T> : IEntityActions<T> where T:BusinessEntity
    {
        protected CrmDbContext Db;

        public ActionsBase(CrmDbContext context)
        {
            Db = context ?? new CrmDbContext();
        }

    }
}
