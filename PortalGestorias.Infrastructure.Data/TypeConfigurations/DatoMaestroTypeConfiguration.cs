using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using PortalGestorias.Domain.Entities;

namespace PortalGestorias.Infrastructure.Data.TypeConfigurations
{
    public class DatoMaestrotypeConfiguration : EntityTypeConfiguration<DatoMaestro>
    {
        public DatoMaestrotypeConfiguration()
        {
            ToTable("ParamDatos");

            HasKey(d => new { d.IdGrupo, d.IdCode });// param datos tiene 2 primary key

            Property(d => d.IdGrupo).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
            Property(d => d.IdCode).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
        }
    }
}