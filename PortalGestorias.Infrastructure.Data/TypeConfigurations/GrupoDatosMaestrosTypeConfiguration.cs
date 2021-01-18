using System.Data.Entity.ModelConfiguration;
using PortalGestorias.Domain.Entities;

namespace PortalGestorias.Infrastructure.Data.TypeConfigurations
{
    public class GrupoDatosMaestrosTypeConfiguration : EntityTypeConfiguration<GrupoDatosMaestros>
    {
        public GrupoDatosMaestrosTypeConfiguration()
        {
            ToTable("ParamGrupos");

            HasKey(g => g.Id);
            HasMany(g => g.Datos) .WithRequired(d => d.Grupo).HasForeignKey(d => d.IdGrupo);
        }
    }
}