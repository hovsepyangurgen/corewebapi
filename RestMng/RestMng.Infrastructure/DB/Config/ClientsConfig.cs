using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using RestMng.Domain;

namespace RestMng.Infrastructure
{
    public partial class ClientsConfig : IEntityTypeConfiguration<Clients>
    {
        public void Configure(EntityTypeBuilder<Clients> entity)
        {
            entity.HasKey(e => e.ClientID);
            entity.Property(e => e.Name).HasMaxLength(50);

            //entity.HasMany(e => e.Orders)
            //    .WithOne(e => e.Clients);

            entity.Property(e => e.Created).HasColumnType("datetime");
            entity.Property(e => e.Updated).HasColumnType("datetime");

            OnConfigurePartial(entity);
        }

        partial void OnConfigurePartial(EntityTypeBuilder<Clients> entity);
    }
}
