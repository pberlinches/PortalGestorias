using System.Data.Entity.ModelConfiguration;
using PortalGestorias.Domain.Entities;

namespace PortalGestorias.Infrastructure.Data.TypeConfigurations
{
    public class EmpleadoTypeConfiguration : EntityTypeConfiguration<Empleado>
    {
        public EmpleadoTypeConfiguration()
        {
            ToTable("Empleados");

            HasKey(c => c.Id);
        }
    }
}


