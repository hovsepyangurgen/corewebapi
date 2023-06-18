using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace ProManAPI
{
    public partial class ProductConfig : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> entity)
        {
            entity.HasKey(e => e.Id);

            entity.HasIndex(e => e.Name)
                  .IsUnique();
            entity.Property(e => e.Name)
                  .HasMaxLength(50)
                  .IsRequired();
            
            entity.Property(e => e.Description)
                  .HasMaxLength(250);

            entity.Property(e => e.DateCreated).HasColumnType("datetime");

            OnConfigurePartial(entity);
        }

        partial void OnConfigurePartial(EntityTypeBuilder<Product> entity);
    }
}
