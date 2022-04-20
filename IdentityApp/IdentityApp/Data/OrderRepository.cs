using IdentityApp.Models;
using Microsoft.EntityFrameworkCore;

namespace IdentityApp.Data
{
    public class OrderRepository : GenericRepository<Order>, IOrderRepository
    {
        public OrderRepository(ApplicationDbContext context,
            ILogger logger) : base(context, logger) { }


        public override async Task<Order> GetById(int id)
        {
            return await dbSet.Include(o=>o.Lines).ThenInclude(l=>l.Product).FirstAsync(o=>o.OrderID == id);
        }


        public override async Task<IEnumerable<Order>> GetAll()
        {
            try
            {
                return await dbSet.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Repo} All function error", typeof(OrderRepository));
                return new List<Order>();
            }
        }

        public override async Task<bool> Update(Order entity)
        {
            try
            {
                var existingOrder = await dbSet.Where(x => x.OrderID == entity.OrderID)
                                                    .FirstOrDefaultAsync();

                if (existingOrder == null)
                    return await Add(entity);

                existingOrder.UserName = entity.UserName;
                existingOrder.OrderDate = entity.OrderDate;
                existingOrder.OrderStatus = entity.OrderStatus;
                existingOrder.TotalCost = entity.TotalCost;
                existingOrder.Lines = entity.Lines;



                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Repo} Update function error", typeof(OrderRepository));
                return false;
            }
        }

        public override async Task<bool> Delete(int id)
        {
            try
            {
                var exist = await dbSet.Where(x => x.OrderID == id)
                                        .FirstOrDefaultAsync();

                if (exist == null) return false;

                dbSet.Remove(exist);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Repo} Delete function error", typeof(OrderRepository));
                return false;
            }
        }
    }

}
