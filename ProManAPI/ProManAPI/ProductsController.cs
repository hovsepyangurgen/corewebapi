using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace ProManAPI
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {

        private readonly ProductsRepository _prodRepo;
        private readonly IMemoryCache _cache;
        public ProductsController(ProductsRepository productsRepository, IMemoryCache memoryCache)
        {
            _prodRepo = productsRepository;
            _cache = memoryCache;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            if (!_cache.TryGetValue("Products", out IEnumerable<Product> products))
            {
                products = await RetrieveProductsFromDatabase();
                SetProductsCache(products);
            }

            return Ok(products?.Select(p => new { Id = p.Id, Name = p.Name, Available = p.Available, Price = p.Price }));
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            var prod = await _prodRepo.Get(id);
            if (prod == null)
            {
                return NotFound();
            }
            return prod;
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<Product?>> AddProduct([FromBody]Product product)
        {
            try
            {
                var createdProd = await _prodRepo.Add(product);
                AddProductCache(createdProd);
                return createdProd;
            }
            catch (Exception ex)
            {
                return Conflict(ex.Message);
            }
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<ActionResult<Product?>> UpdateProduct(int id,[FromBody] Product updateProduct)
        {
            var product = await _prodRepo.Update(id, updateProduct);
            if (product == null)
            {
                return NotFound();
            }
            UpdateProductCache(product);
            return product;
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteProduct(int id)
        {
            try
            {
                await _prodRepo.Delete(id);
                DeleteProductCache(id);
            }
            catch (Exception ex)
            {
                return Conflict(ex.Message);
            }
            return NoContent();
        }


        #region Memory Cache
        private async Task<IEnumerable<Product>> RetrieveProductsFromDatabase()
        {
            return await _prodRepo.GetAll();
        }
        private void SetProductsCache(IEnumerable<Product> products)
        {
            var cacheOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10),
                SlidingExpiration = TimeSpan.FromMinutes(2)
            };

            _cache.Set("Products", products, cacheOptions);
        }
        private void UpdateProductCache(Product product)
        {
            if (_cache.TryGetValue("Products", out IEnumerable<Product> products))
            {
                var updatedProducts = new List<Product>(products);
                int index = updatedProducts.FindIndex(p => p.Id == product.Id);
                if (index != -1)
                {
                    updatedProducts[index] = product;
                    SetProductsCache(updatedProducts);
                }
            }
        }
        private void AddProductCache(Product product)
        {
            if (_cache.TryGetValue("Products", out IEnumerable<Product> products))
            {
                var updatedProducts = new List<Product>(products);
                updatedProducts.Add(product);
                SetProductsCache(updatedProducts);
            }
        }
        private void DeleteProductCache(int id)
        {
            if (_cache.TryGetValue("Products", out IEnumerable<Product> products))
            {
                var updatedProducts = new List<Product>(products);
                int index = updatedProducts.FindIndex(p => p.Id == id);
                if (index > -1)
                {
                    updatedProducts.RemoveAt(id);
                    SetProductsCache(updatedProducts);
                }
            }
        }
        #endregion
    }
}
