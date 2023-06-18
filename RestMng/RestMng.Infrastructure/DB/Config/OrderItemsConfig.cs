using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RestMng.Domain;

namespace RestMng.Infrastructure
{
    public partial class OrderItemsConfig : IEntityTypeConfiguration<OrderItems>
    {
        public void Configure(EntityTypeBuilder<OrderItems> entity)
        {
            entity.HasKey(e => e.OrderItemID);

            entity.HasOne(e => e.Orders)
                    .WithMany(e => e.OrderItems)
                    .HasForeignKey(e => e.OrderID)
                    .IsRequired();

            entity.Property(e => e.Created).HasColumnType("datetime");
            entity.Property(e => e.Updated).HasColumnType("datetime");

            OnConfigurePartial(entity);
        }

        partial void OnConfigurePartial(EntityTypeBuilder<OrderItems> entity);
    }
}
