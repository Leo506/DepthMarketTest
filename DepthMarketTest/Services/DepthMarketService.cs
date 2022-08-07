using DepthMarketTest.Models;
using DepthMarketTest.Repository;
using System;

namespace DepthMarketTest.Services
{
    public class DepthMarketService : IDepthMarketService
    {
        private readonly IAskMarketRepository _askMarketRepository;
        private readonly IOrdersRepository _ordersRepository;
        private readonly IBidMarketRepository _bidMarketRepository;
        public DepthMarketService(
            IAskMarketRepository askMarketRepository,
            IOrdersRepository ordersRepository
            )
        {
            _askMarketRepository = askMarketRepository;
            _ordersRepository = ordersRepository;
        }
        public async Task ProcessOrderAsync(string orderId)
        {
            var order = await _ordersRepository.GetByIdAsync(orderId);
            
            if (order.OrderType is OrderTypes.Ask)
            {
                var relevantBids = await _bidMarketRepository.GetRelevantBidsAsync(order);
                if (order.OnlyFullExecution)
                {
                    var matchedMarketModel = await BidFullExecSearchAsync(order, relevantBids);
                    if(matchedMarketModel == null)
                    {
                        order.Status = OrderStatus.Active;
                        await _ordersRepository.UpdateAsync(order);
                    }
                    else
                    {
                        if (matchedMarketModel.OnlyFullExecution)
                        {
                            if(matchedMarketModel.Volume == order.Volume)
                            {
                                var matchedOrder =await _ordersRepository.GetByIdAsync(matchedMarketModel.Id);

                                matchedOrder.Status = OrderStatus.Executing;
                                order.Status = OrderStatus.Executing;

                                await _bidMarketRepository.DeleteAsync(matchedOrder.Id);
                            }
                        }
                    }
                }
                else
                {

                }
            }
            else
            {
                if (order.OnlyFullExecution)
                {

                }
                else
                {

                }
            }
        }
        private async Task<MarketModel> BidFullExecSearchAsync(OrderModel model, List<MarketModel> relevantMarketList)
        {
            throw new NotImplementedException();
        }
        private async Task<MarketModel> BidPartialExecSearchAsync()
        {
            throw new NotImplementedException();
        }
        private async Task<MarketModel> AskFullExecSearchAsync()
        {
            throw new NotImplementedException();
        }
        private async Task<MarketModel> AskPartialExecSearchAsync()
        {
            throw new NotImplementedException();
        }

    }
}
