using PortalGestorias.Domain.Entities;
using PortalGestorias.Infrastructure.Data;
using PortalGestorias.Domain.Models;

namespace PortalGestorias.Business.Validations
{
    public class EstadoSearch : EntitySearchBase<BusquedaEstado>
    {
        public EstadoSearch(CrmDbContext context) : base(context)
        {
        }
    }
}
