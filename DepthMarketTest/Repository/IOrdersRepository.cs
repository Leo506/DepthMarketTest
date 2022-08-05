using DepthMarketTest.Models;

namespace DepthMarketTest.Repository
{
    public interface IOrdersRepository
    {
        Task<List<OrderModel>> GetAllAsync();
        Task UpdateAsync(OrderModel orderToUpdate);
        Task<OrderModel> GetByIdAsync(string id);
        Task<string> CreateNewAsync(OrderModel newOrder);
        Task DeleteAsync(string id);
    }
}
