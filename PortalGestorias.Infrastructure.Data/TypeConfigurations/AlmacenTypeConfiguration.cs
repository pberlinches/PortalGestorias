using System.Data.Entity.ModelConfiguration;
using PortalGestorias.Domain.Entities;

namespace PortalGestorias.Infrastructure.Data.TypeConfigurations
{
    public class AlmacenTypeConfiguration : EntityTypeConfiguration<Almacen>
    {
        public AlmacenTypeConfiguration()
        {
            ToTable("Almacenes");

            HasKey(c => c.Id);
        }
    }
}


