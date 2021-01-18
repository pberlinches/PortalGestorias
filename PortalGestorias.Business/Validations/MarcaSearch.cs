using PortalGestorias.Domain.Entities;
using PortalGestorias.Infrastructure.Data;
using PortalGestorias.Domain.Models;

namespace PortalGestorias.Business.Validations
{
    public class MarcaSearch : EntitySearchBase<BusquedaMarca>
    {
        public MarcaSearch(CrmDbContext context) : base(context)
        {
        }
    }
}