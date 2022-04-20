using IdentityApp.Models;
using Microsoft.EntityFrameworkCore;

namespace IdentityApp.Data
{
    public class ProductRepository : GenericRepository<Product>, IProductRepository
    {
        public ProductRepository(ApplicationDbContext context, 
            ILogger logger) : base(context, logger) { }

        public override async Task<IEnumerable<Product>> GetAll()
        {
            try
            {
                return await dbSet.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Repo} All function error", typeof(ProductRepository));
                return new List<Product>();
            }
        }

        public override async Task<bool> Update(Product entity)
        {
            try
            {
                var existingProduct = await dbSet.Where(x => x.ProductId == entity.ProductId)
                                                    .FirstOrDefaultAsync();

                if (existingProduct == null)
                    return await Add(entity);

                existingProduct.Name = entity.Name;
                existingProduct.Description = entity.Description;
                existingProduct.Price = entity.Price;
                existingProduct.ImageUrl = entity.ImageUrl;


                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Repo} Update function error", typeof(ProductRepository));
                return false;
            }
        }

        public override async Task<bool> Delete(int id)
        {
            try
            {
                var exist = await dbSet.Where(x => x.ProductId == id)
                                        .FirstOrDefaultAsync();

                if (exist == null) return false;

                dbSet.Remove(exist);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Repo} Delete function error", typeof(ProductRepository));
                return false;
            }
        }
    }

}
