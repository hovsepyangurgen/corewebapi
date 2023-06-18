using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RestMng.Domain;

namespace RestMng.Infrastructure
{
    public partial class OrdersConfig : IEntityTypeConfiguration<Orders>
    {
        public void Configure(EntityTypeBuilder<Orders> entity)
        {

            entity.HasKey(e => e.OrderID);

            entity.HasMany(e => e.OrderItems)
                  .WithOne(e => e.Orders)
                  .IsRequired();

            entity.HasOne(e=>e.Clients)
                .WithMany(e => e.Orders)
                .HasForeignKey(e=>e.ClientID)
                .IsRequired();

            entity.HasOne(e => e.Customers)
                .WithMany(e => e.Orders)
                .HasForeignKey(e => e.CustomerID)
                .IsRequired();

            entity.Property(e => e.Created).HasColumnType("datetime");
            entity.Property(e => e.Updated).HasColumnType("datetime");

            OnConfigurePartial(entity);
        }

        partial void OnConfigurePartial(EntityTypeBuilder<Orders> entity);
    }
}
