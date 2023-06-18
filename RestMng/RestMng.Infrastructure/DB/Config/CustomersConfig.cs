using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RestMng.Domain;

namespace RestMng.Infrastructure
{
    public partial class CustomersConfig : IEntityTypeConfiguration<Customers>
    {
        public void Configure(EntityTypeBuilder<Customers> entity)
        {
            entity.HasKey(e => e.CustomerID);

            //entity.HasMany(e => e.Orders)
            //    .WithOne(e => e.Customers);

            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.ContactInfo).HasMaxLength(100);

            entity.Property(e => e.Created).HasColumnType("datetime");
            entity.Property(e => e.Updated).HasColumnType("datetime");

            OnConfigurePartial(entity);
        }

        partial void OnConfigurePartial(EntityTypeBuilder<Customers> entity);
    }
}
