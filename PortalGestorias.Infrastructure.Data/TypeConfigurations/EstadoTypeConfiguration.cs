using System.Data.Entity.ModelConfiguration;
using PortalGestorias.Domain.Entities;

namespace PortalGestorias.Infrastructure.Data.TypeConfigurations
{
    public class EstadoTypeConfiguration : EntityTypeConfiguration<Estado>
    {
        public EstadoTypeConfiguration()
        {
            ToTable("Estados");

            HasKey(c => c.Id);
        }
    }
}


