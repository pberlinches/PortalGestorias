using System.Data.Entity.ModelConfiguration;
using PortalGestorias.Domain.Entities;

namespace PortalGestorias.Infrastructure.Data.TypeConfigurations
{
    public class StockRoomTypeConfiguration : EntityTypeConfiguration<StockRoom>
    {
        public StockRoomTypeConfiguration()
        {
            ToTable("StockRooms");

            HasKey(c => c.Id);

            Ignore(m => m.StockRoomString);

        }
    }
}


