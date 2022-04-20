namespace IdentityApp.Data
{
    public interface IUnitOfWork
    {
        IProductRepository Products { get; }
        IOrderRepository Orders { get; }
        Task CompleteAsync();
    }
}
