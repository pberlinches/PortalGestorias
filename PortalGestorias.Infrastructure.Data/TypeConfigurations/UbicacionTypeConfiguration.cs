using System.Data.Entity.ModelConfiguration;
using PortalGestorias.Domain.Entities;

namespace PortalGestorias.Infrastructure.Data.TypeConfigurations
{
    public class UbicacionTypeConfiguration : EntityTypeConfiguration<Ubicacion>
    {
        public UbicacionTypeConfiguration()
        {
            ToTable("Ubicaciones");

            HasKey(c => c.Id);
        }
    }
}


