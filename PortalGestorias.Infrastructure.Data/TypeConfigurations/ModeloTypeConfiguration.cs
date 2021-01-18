using System.Data.Entity.ModelConfiguration;
using PortalGestorias.Domain.Entities;

namespace PortalGestorias.Infrastructure.Data.TypeConfigurations
{
    public class ModeloTypeConfiguration : EntityTypeConfiguration<Modelo>
    {
        public ModeloTypeConfiguration()
        {
            ToTable("Modelos");

            HasKey(c => c.Id);
            HasRequired(c => c.Marca).WithMany().Map(fk => fk.MapKey("IdMarca"));
            HasRequired(c => c.TipoModelo).WithMany().Map(fk => fk.MapKey("IdTipoModelo"));
        }
    }
}


