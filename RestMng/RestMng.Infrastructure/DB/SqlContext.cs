using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using RestMng.Core;
using RestMng.Domain;
using System.ComponentModel;
using System.Reflection.Emit;

namespace RestMng.Infrastructure
{
    public class SqlContext : DbContext
    {
        public SqlContext(DbContextOptions<SqlContext> options) : base(options)
        { }
        private readonly object _locker = new object();

        public DbSet<Clients> Clients { get; set; }
        public DbSet<Customers> Customers { get; set; }
        public DbSet<OrderItems> OrderItems { get; set; }
        public DbSet<Orders> Orders { get; set; }
        public DbSet<MenuItems> MenuItems { get; set; }
        public DbSet<Inventory> Inventory { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new ClientsConfig());
            builder.ApplyConfiguration(new CustomersConfig());
            builder.ApplyConfiguration(new MenuItemsConfig());
            builder.ApplyConfiguration(new InventoryConfig());
            builder.ApplyConfiguration(new OrdersConfig());
            builder.ApplyConfiguration(new OrderItemsConfig());

            base.OnModelCreating(builder);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var entries = ChangeTracker
            .Entries()
            .Where(e => e.Entity is IEntity && (
            e.State == EntityState.Added
            || e.State == EntityState.Modified));

            foreach (var entityEntry in entries)
            {
                ((IEntity)entityEntry.Entity).Updated = DateTime.Now;
                if (entityEntry.State == EntityState.Added)
                {
                    ((IEntity)entityEntry.Entity).Created = DateTime.Now;
                }
            }
            lock (_locker)
            {
                return base.SaveChangesAsync(cancellationToken);
            }
        }
    }
    public static class SqlContextFactory
    {
        public static SqlContext Create(string connectionString)
        {
            var optionsBuilder = new DbContextOptionsBuilder<SqlContext>();
            optionsBuilder.UseSqlServer(connectionString);

            using var context = new SqlContext(optionsBuilder.Options);
            context.Database.Migrate();

            return context;
        }
    }
}
