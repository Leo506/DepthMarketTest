using DepthMarketTest.Models;

namespace DepthMarketTest.Repository
{
    public interface IBidMarketRepository
    {
        Task<List<MarketModel>> GetRelevantBidsAsync(OrderModel model);
        Task<List<MarketModel>> GetAllAsync();
        Task UpdateAsync(MarketModel marketBidToUpdate);
        Task<MarketModel> GetByIdAsync(string id);
        Task<string> CreateNewAsync(MarketModel newMarketBid);
        Task DeleteAsync(string id);
    }
}
