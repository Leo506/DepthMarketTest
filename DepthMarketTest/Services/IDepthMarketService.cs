namespace DepthMarketTest.Services
{
    public interface IDepthMarketService
    {
         Task ProcessOrderAsync(string orderId);
    }
}
