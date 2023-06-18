using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RestMng.Domain;

namespace RestMng.Infrastructure
{
    public partial class InventoryConfig : IEntityTypeConfiguration<Inventory>
    {
        public void Configure(EntityTypeBuilder<Inventory> entity)
        {
            entity.HasKey(e => e.ItemID);

            entity.HasOne(e => e.MenuItems)
                  .WithOne(e => e.Inventory)
                  .HasForeignKey<Inventory>(e => e.ItemID)
                  .IsRequired();

            entity.Property(e => e.Created).HasColumnType("datetime");
            entity.Property(e => e.Updated).HasColumnType("datetime");

            OnConfigurePartial(entity);
        }

        partial void OnConfigurePartial(EntityTypeBuilder<Inventory> entity);
    }
}
