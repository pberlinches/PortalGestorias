using System.Data.Entity.ModelConfiguration;
using PortalGestorias.Domain.Entities;

namespace PortalGestorias.Infrastructure.Data.TypeConfigurations
{
   public class BatchTareaTypeConfiguration : EntityTypeConfiguration<BatchTarea>
    {
        public BatchTareaTypeConfiguration()
        {
            ToTable("BatchTareas");

            HasKey(d => d.Id);
            HasRequired(d => d.TipoTarea).WithMany().Map(fk => fk.MapKey("IdTipoTarea"));
        }
    }
}



