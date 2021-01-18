using System.Data.Entity.ModelConfiguration;
using PortalGestorias.Domain.Entities;

namespace PortalGestorias.Infrastructure.Data.TypeConfigurations
{
    public class CorreoPlantillasTypeConfiguration : EntityTypeConfiguration<CorreoPlantillas>
    {
        public CorreoPlantillasTypeConfiguration()
        {
            ToTable("CorreoPlantillas");

            HasKey(c => c.Id);
        }
    }
}


