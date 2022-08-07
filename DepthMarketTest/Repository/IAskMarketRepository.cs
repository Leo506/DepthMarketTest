using DepthMarketTest.Models;

namespace DepthMarketTest.Repository
{
    public interface IAskMarketRepository
    {
        Task<List<MarketModel>> GetRelevantBidsAsync(OrderModel model);
        Task<List<MarketModel>> GetAllAsync();
        Task UpdateAsync(MarketModel orderToUpdate);
        Task<MarketModel> GetByIdAsync(string id);
        Task<string> CreateNewAsync(MarketModel newOrder);
        Task DeleteAsync(string id);
    }
}
