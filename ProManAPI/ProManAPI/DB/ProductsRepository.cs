using Microsoft.EntityFrameworkCore;

namespace ProManAPI
{
    public class ProductsRepository : IDisposable
    {
        protected readonly ProductDBContext _context;
        private static readonly Dictionary<int, SemaphoreSlim> _prodLocks = new Dictionary<int, SemaphoreSlim>();
        private static readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1);
        public ProductsRepository(ProductDBContext sqlContext)
        {
            _context = sqlContext;
        }

        public async Task<IEnumerable<Product>> GetAll()
        {
            return await _context.Products.ToListAsync();
        }

        public async Task<Product?> Get(int id)
        {
            return await _context.Products.FindAsync(id);
        }
        public async Task<Product?> Add(Product product)
        {
            await _semaphore.WaitAsync();
            try
            {
                var existingProd = await _context.Products.FirstOrDefaultAsync(p => p.Name == product.Name);
                if (existingProd != null)
                {
                    throw new Exception("A product with the same name already exists.");
                }
                product.DateCreated = DateTime.Now;
                _context.Products.Add(product);
                await _context.SaveChangesAsync();
            }
            finally
            {
                _semaphore.Release();
            }
            return product;
        }
        public async Task<Product?> Update(int id, Product updateProduct)
        {
            Product? product = null;
            SemaphoreSlim prodLock;
            lock (_prodLocks)
            {
                if (_prodLocks.ContainsKey(id))
                {
                    prodLock = _prodLocks[id];
                }
                else
                {
                    prodLock = new SemaphoreSlim(1);
                    _prodLocks[id] = prodLock;
                }
            }
            try
            {
                await prodLock.WaitAsync();
                using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    product = await _context.Set<Product>().FindAsync(id);
                    if (product != null)
                    {
                        product.Name = updateProduct.Name;
                        product.Available += updateProduct.Available;
                        product.Description = updateProduct.Description;
                        product.Price = updateProduct.Price;

                        _context.Entry(product).State = EntityState.Modified;
                        await _context.SaveChangesAsync();

                        await transaction.CommitAsync();
                    }
                }
                catch { 
                    await transaction.RollbackAsync();
                }
            }
            finally
            {
                prodLock.Release();
            }

            return product;
        }

        public async Task Delete(int id)
        {
            await _semaphore.WaitAsync();
            try
            {
                var product = await _context.Products.FindAsync(id);
                if (product == null)
                {
                    throw new Exception("A product not found.");
                }
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }
            finally
            {
                _semaphore.Release();
            }
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
}
