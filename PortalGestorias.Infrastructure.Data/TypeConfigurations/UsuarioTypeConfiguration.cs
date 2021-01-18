using System.Data.Entity.ModelConfiguration;
using PortalGestorias.Domain.Entities;


namespace PortalGestorias.Infrastructure.Data.TypeConfigurations
{
    class UsuarioTypeConfiguration : EntityTypeConfiguration<Usuario>
    {

        public UsuarioTypeConfiguration()
        {
            ToTable("Usuario");

            HasKey(e => e.Id);

        }

    }
}
