using System.Data.Entity.ModelConfiguration;
using PortalGestorias.Domain.Entities;

namespace PortalGestorias.Infrastructure.Data.TypeConfigurations
{
    class BatchTipoTareaTypeConfiguration : EntityTypeConfiguration<BatchTipoTarea>
    {
        public BatchTipoTareaTypeConfiguration()
        {
            ToTable("BatchTipoTareas");

            HasKey(d => d.Id);
        }
    }
}
