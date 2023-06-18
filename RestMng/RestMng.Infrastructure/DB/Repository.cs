using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using RestMng.Core;
using RestMng.Domain;
using System.ComponentModel;
using System.Linq.Expressions;

namespace RestMng.Infrastructure
{
    public abstract class Repository<TEntity, TContext> : IRepository<TEntity>
          where TEntity : class, IEntity
          where TContext : SqlContext
    {

        protected readonly SqlContext _context;
        public Repository(SqlContext SqlContext)
        {
            _context = SqlContext;
        }
        public async Task<TEntity> Add(TEntity entity)
        {
            _context.Set<TEntity>().Add(entity); 
            await _context.SaveChangesAsync();
            return entity;
        }
        public async Task<TEntity> Delete(int id)
        {
            var entity = await _context.Set<TEntity>().FindAsync(id);
            if (entity == null)
            {
                return entity;
            }

            _context.Set<TEntity>().Remove(entity);
            await _context.SaveChangesAsync();

            return entity;
        }
        public async Task<TEntity> Get(int id)
        {
            return await _context.Set<TEntity>().FindAsync(id);
        }
        public async Task<List<TEntity>> GetAll()
        {
            return await _context.Set<TEntity>().ToListAsync();
        }
        public async Task<TEntity> Update(TEntity entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return entity;
        }
        public async Task<TEntity> Set(TEntity entity)
        {

            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return entity;
        }
        public async Task<TEntity> Set(TEntity entity, EntityState state)
        {

            _context.Entry(entity).State = state;
            await _context.SaveChangesAsync();

            return entity;
        }

        public async Task<TEntity> GetByKey(Expression<Func<TEntity, bool>> predicate)
        {
            return await _context.Set<TEntity>().FirstOrDefaultAsync(predicate);
        }
        public async Task<List<TEntity>> Get(Expression<Func<TEntity, bool>> predicate)
        {
            return await _context.Set<TEntity>().Where(predicate).ToListAsync();
        }
        public async Task AddRange(IEnumerable<TEntity> items)
        {
            _context.Set<TEntity>().AddRange(items);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAll()
        {
            var items = await GetAll();
            _context.Set<TEntity>().RemoveRange(items);
            _context.SaveChanges();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _context.Dispose();
            }
        }
    }

    public class ClientsRepository : Repository<Clients, SqlContext>
    {
        public ClientsRepository(SqlContext sqlContext) : base(sqlContext)
        {

        }
    }
    public class CustomersRepository : Repository<Customers, SqlContext>
    {
        public CustomersRepository(SqlContext sqlContext) : base(sqlContext)
        {

        }
    }
    public class MenuItemsRepository : Repository<MenuItems, SqlContext>
    {
        public MenuItemsRepository(SqlContext sqlContext) : base(sqlContext)
        {

        }
    }
    public class InventoryRepository : Repository<Inventory, SqlContext>
    {
        public InventoryRepository(SqlContext sqlContext) : base(sqlContext)
        {

        }
        public new async Task<Inventory> Add(Inventory inventory)
        {
            using (var trans = _context.Database.BeginTransaction())
            {
                try
                {
                    _context.Set<Inventory>().Add(inventory);
                    _ = await _context.SaveChangesAsync();


                    await trans.CommitAsync();
                }
                catch
                {
                    trans.Rollback();
                    throw;
                }
            }
            return inventory;
        }
    }
    public class OrdersRepository : Repository<Orders, SqlContext>
    {
        private static SemaphoreSlim _lock = new SemaphoreSlim(1);

        public OrdersRepository(SqlContext sqlContext) : base(sqlContext) { }
        public new async Task<Orders> Add(Orders order)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var client = await _context.Set<Clients>().FindAsync(order.ClientID);
                var customer = await _context.Set<Customers>().FindAsync(order.CustomerID);
                if (client == null || customer == null)
                {
                    throw new Exception("Customer/ Client not exists.");
                }
                if (client != null && client.Role == ClientType.manager)
                {
                    throw new Exception("The manager can not add orders.");
                }

                order.Status = OrderStatus.InProgress;
                foreach (var orderItem in order.OrderItems)
                {
                    var menuItem = await _context.Set<MenuItems>().FindAsync(orderItem.ItemID);
                    if (menuItem == null) throw new Exception("Not existent menu item");
                    orderItem.Subtotal = orderItem.Quantity * menuItem.Price;
                    order.TotalAmount += orderItem.Subtotal;
                }
                await base.Add(order);


                // Check quantity in Inventory
                try
                {
                    await _lock.WaitAsync();
                    foreach (var orderItem in order.OrderItems)
                    {
                        Inventory? invitem = await _context.Set<Inventory>().FindAsync(orderItem.ItemID);
                        if (invitem != null)
                        {
                            if (invitem.Quantity >= orderItem.Quantity)
                            {
                                invitem.Quantity -= orderItem.Quantity;
                                _context.Entry(invitem).State = EntityState.Modified;
                                await _context.SaveChangesAsync();
                            }
                            else
                            {
                                throw new Exception("Not enough quantity");
                            }
                        }
                        else
                        {
                            throw new Exception("Not existent item");
                        }
                    }
                }
                finally
                {
                    _lock.Release();
                }


                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
            return order;
        }
    }
    public class OrderItemsRepository : Repository<OrderItems, SqlContext>
    {
        private static SemaphoreSlim _lock = new SemaphoreSlim(1);
        public OrderItemsRepository(SqlContext sqlContext) : base(sqlContext) { }

        public async Task<ICollection<OrderItems>> Set(List<OrderItems> orderItems)
        {
            List<OrderItems> resultOrdItems = new List<OrderItems>();
            try
            {
                await _lock.WaitAsync();
                foreach (var orderItem in orderItems)
                {
                    Inventory? invitem = await _context.Set<Inventory>().FindAsync(orderItem.ItemID);
                    if (invitem != null)
                    {
                        var order = await _context.Set<Orders>().Include(e => e.OrderItems).FirstOrDefaultAsync(e => e.OrderID == orderItem.OrderID);
                        if (order == null) throw new Exception("Not existing order");
                        var dbOrdItem = order.OrderItems.Where(oi => oi.OrderItemID == orderItem.OrderItemID).FirstOrDefault();
                        if (dbOrdItem != null)
                        {
                            if (dbOrdItem.Quantity > orderItem.Quantity)
                            {
                                // Difference add to inventory
                                invitem.Quantity += (dbOrdItem.Quantity - orderItem.Quantity);
                            }
                            else
                            {
                                if (invitem.Quantity >= orderItem.Quantity - dbOrdItem.Quantity)
                                {
                                    invitem.Quantity -= (orderItem.Quantity - dbOrdItem.Quantity);
                                }
                                else
                                {
                                    throw new Exception("Not enough quantity");
                                }
                            }
                            dbOrdItem.Quantity = orderItem.Quantity;
                            _context.Entry(dbOrdItem).State = EntityState.Modified;
                            await _context.SaveChangesAsync();
                            resultOrdItems.Add(dbOrdItem);
                        }
                        else
                        {
                            if (orderItem.Quantity > invitem.Quantity)
                            {
                                throw new Exception("Not enough quantity");
                            }
                            invitem.Quantity -= orderItem.Quantity;
                            // Create new order item
                            _context.Set<OrderItems>().Add(orderItem);
                            await _context.SaveChangesAsync();
                            resultOrdItems.Add(orderItem);
                        }
                        _context.Entry(invitem).State = EntityState.Modified;
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        throw new Exception("Not existent item");
                    }
                }
            }
            finally
            {
                _lock.Release();
            }
            return resultOrdItems;
        }
    }
}
