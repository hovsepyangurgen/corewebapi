using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RestMng.Domain;

namespace RestMng.Infrastructure
{
    public partial class MenuItemsConfig : IEntityTypeConfiguration<MenuItems>
    {
        public void Configure(EntityTypeBuilder<MenuItems> entity)
        {
            entity.HasKey(e => e.ItemID);

            //entity.HasOne(e => e.Inventory)
            //    .WithOne(e => e.MenuItems);

            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.Description).HasMaxLength(250);

            entity.Property(e => e.Created).HasColumnType("datetime");
            entity.Property(e => e.Updated).HasColumnType("datetime");

            OnConfigurePartial(entity);
        }

        partial void OnConfigurePartial(EntityTypeBuilder<MenuItems> entity);
    }
}
