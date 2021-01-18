
using PortalGestorias.Domain.Entities;

namespace PortalGestorias.Business.Validations
{
    public interface IEntitySearch<in S>
        where S : TrackeableEntity, new()
    {
        
    }
}
