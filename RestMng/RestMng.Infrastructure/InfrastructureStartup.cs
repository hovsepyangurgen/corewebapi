using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using AutoMapper;
using System.Reflection;

namespace RestMng.Infrastructure
{
    public static class InfrastructureStartup
    {
        public static IServiceCollection AddDatabase(this IServiceCollection services, string connectionString)
        {
            var optionsBuilder = new DbContextOptionsBuilder<SqlContext>().UseSqlServer(connectionString, m => m.MigrationsAssembly("RestMng.API"));
            services.AddSingleton(optionsBuilder.Options);
            services.AddEntityFrameworkSqlServer().AddDbContext<SqlContext>();

            using (var context = new SqlContext(optionsBuilder.Options))
            {
                context.Database.EnsureCreated();
            }
            return services;
        }

        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<ClientsRepository>();
            services.AddScoped<CustomersRepository>();
            services.AddScoped<MenuItemsRepository>();
            services.AddScoped<OrdersRepository>();
            services.AddScoped<OrderItemsRepository>();
            services.AddScoped<InventoryRepository>();
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            return services;
        }
    }
}
