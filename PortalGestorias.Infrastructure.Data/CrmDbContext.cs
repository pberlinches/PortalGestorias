using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using PortalGestorias.Domain.Entities;
using PortalGestorias.Infrastructure.Data.TypeConfigurations;

namespace PortalGestorias.Infrastructure.Data
{
    public class CrmDbContext : DbContext
    {
        private readonly IIdentity identity;

        public CrmDbContext()
            : this(null)
        {
        }

        public CrmDbContext(IIdentity identity)
            : this(identity, "CrmConnection")// AQUI LE DECIMOS LA CADENA DE CONEXION
        {
        }

        public CrmDbContext(IIdentity identity, string connectionString)
            : base(connectionString)
        {
            this.identity = identity ?? WindowsIdentity.GetCurrent();
        }
        // CADA ENTIDAD QUE SE CREA HAY QUE METER AQUI UNA LINEA  QUE NOS UTILICE EL DATASET. 
        //ABAJO EN EL MODELBUILDER TAMBIEN PARA CONFIGURAR CLASES DE DOMINIO

        public DbSet<GrupoDatosMaestros> GruposDatosMaestros { get; set; }

        public DbSet<DatoMaestro> DatosMaestros { get; set; }

        public virtual DbSet<Almacen>Almacenes{ get; set; }

        public virtual DbSet<BatchTarea> BatchTareas { get; set; }

        public virtual DbSet<BatchTipoTarea> BatchTipoTareas { get; set; }

        public virtual DbSet<CorreoPlantillas> CorreoPlantillas { get; set; }

        public virtual DbSet<Empleado> Empleados { get; set; }

        public virtual DbSet<Estado> Estados { get; set; }

        public virtual DbSet<Marca> Marcas { get; set; }

        public virtual DbSet<Modelo> Modelos { get; set; }

        public virtual DbSet<Producto> Productos { get; set; }

        public virtual DbSet<StockRoom> StockRooms { get; set; }

        public virtual DbSet<TipoModelo> TiposModelos { get; set; }

        public virtual DbSet<Ubicacion> Ubicaciones { get; set; }

        public int SaveChanges(bool discardDatosMaestros = true)
        {
            var currentModifyTime = DateTime.Now;
            ChangeTracker.Entries().Where(e => e.State != EntityState.Unchanged && e.Entity is TrackeableEntity).ToList().ForEach(
                e =>
                {
                    ((TrackeableEntity)e.Entity).FechaMod = currentModifyTime;
                    ((TrackeableEntity)e.Entity).UsuMod = identity.Name;
                });
            if (discardDatosMaestros && !ChangeTracker.Entries().All(e => e.Entity is DatoMaestro))
            {
                ChangeTracker.Entries<DatoMaestro>().ToList().ForEach(dm => dm.State = EntityState.Unchanged);
            }
            return SaveChanges();
        }

        private new int SaveChanges()
        {
            return base.SaveChanges();
        }

        public int SaveChangesBase()
        {
            int numero = 0;

            try
            {
                numero = base.SaveChanges();
            }
            catch (Exception)
            {
                throw;
            }

            return numero;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Configurations.Add(new AlmacenTypeConfiguration());
            modelBuilder.Configurations.Add(new BatchTareaTypeConfiguration());
            modelBuilder.Configurations.Add(new BatchTipoTareaTypeConfiguration());
            modelBuilder.Configurations.Add(new CorreoPlantillasTypeConfiguration());
            modelBuilder.Configurations.Add(new DatoMaestrotypeConfiguration());
            modelBuilder.Configurations.Add(new EmpleadoTypeConfiguration());
            modelBuilder.Configurations.Add(new EstadoTypeConfiguration());
            modelBuilder.Configurations.Add(new GrupoDatosMaestrosTypeConfiguration());
            modelBuilder.Configurations.Add(new MarcaTypeConfiguration());
            modelBuilder.Configurations.Add(new ModeloTypeConfiguration());
            modelBuilder.Configurations.Add(new ProductoTypeConfiguration());
            modelBuilder.Configurations.Add(new StockRoomTypeConfiguration());
            modelBuilder.Configurations.Add(new TipoModeloTypeConfiguration());
            modelBuilder.Configurations.Add(new UbicacionTypeConfiguration());
        }
    }

    public static class DbContextExtensions
    {
        public static T ApplyValues<T>(this DbSet<T> set, T entity, params object[] key) where T : class
        {
            var dbEntity = set.Find(key);
            if (dbEntity == null)
            {
                return null;
            }

            var properties = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(p => !(p.PropertyType.IsInterface && p.PropertyType.IsGenericType &&
                              p.PropertyType.GetGenericTypeDefinition() == typeof(ICollection<>))
                             && p.CanWrite).ToList();
            properties.ForEach(p =>
            {
                p.GetValue(dbEntity);
                p.SetValue(dbEntity, p.GetValue(entity));
            });

            return dbEntity;
        }
    }
}