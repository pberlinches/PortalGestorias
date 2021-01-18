using System.Data.Entity.ModelConfiguration;
using PortalGestorias.Domain.Entities;

namespace PortalGestorias.Infrastructure.Data.TypeConfigurations
{
    public class MarcaTypeConfiguration : EntityTypeConfiguration<Marca>
    {
        public MarcaTypeConfiguration()
        {
            ToTable("Marcas");

            HasKey(c => c.Id);
        }
    }
}


