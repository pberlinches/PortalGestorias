using System.Data.Entity.ModelConfiguration;
using PortalGestorias.Domain.Entities;

namespace PortalGestorias.Infrastructure.Data.TypeConfigurations
{
    public class ProductoTypeConfiguration : EntityTypeConfiguration<Producto>
    {
        public ProductoTypeConfiguration()
        {
            ToTable("Productos");

            HasKey(c => c.Id);
            HasRequired(c => c.Marca).WithMany().Map(fk => fk.MapKey("IdMarca"));
            HasRequired(c => c.Ubicacion).WithMany().Map(fk => fk.MapKey("IdUbicacion"));
            HasRequired(c => c.Modelo).WithMany().Map(fk => fk.MapKey("IdModelo"));
            HasRequired(c => c.Almacen).WithMany().Map(fk => fk.MapKey("IdAlmacen"));
            HasRequired(c => c.Departamento).WithMany().Map(fk => fk.MapKey("IdStockRoom"));
            HasRequired(c => c.Estado).WithMany().Map(fk => fk.MapKey("IdEstado"));
            HasRequired(c => c.UsuarioAlta).WithMany().Map(fk => fk.MapKey("IdUsuarioAlta"));
            HasOptional(c => c.UsuarioEntrega).WithMany().Map(fk => fk.MapKey("IdUsuarioEntrega"));
            HasOptional(c => c.UsuarioBaja).WithMany().Map(fk => fk.MapKey("IdUsuarioBaja"));

            Ignore(c => c.Cantidad);
            Ignore(c => c.Valor);
            Ignore(c => c.TipoModelo);
            Ignore(c => c.StockRoom);
            Ignore(c => c.defaultBarcode);
            Ignore(c => c.defaultTipoModelo);
            Ignore(c => c.defaultModelo);
            Ignore(c => c.defaultMarca);
            Ignore(c => c.Seleccionado);

        }
    }
}


