using System.Data.Entity.ModelConfiguration;
using PortalGestorias.Domain.Entities;

namespace PortalGestorias.Infrastructure.Data.TypeConfigurations
{
    public class TipoModeloTypeConfiguration : EntityTypeConfiguration<TipoModelo>
    {
        public TipoModeloTypeConfiguration()
        {
            ToTable("TiposModelos");

            HasKey(c => c.Id);
        }
    }
}


