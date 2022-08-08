using DepthMarketTest.Models;
using DepthMarketTest.Repository;

namespace DepthMarketTest.Services
{
    public class DepthMarketService : IDepthMarketService
    {
        private readonly IAskMarketRepository _askMarketRepository;
        private readonly IOrdersRepository _ordersRepository;
        private readonly IBidMarketRepository _bidMarketRepository;
        public DepthMarketService(
            IAskMarketRepository askMarketRepository,
            IOrdersRepository ordersRepository,
            IBidMarketRepository bidMarketRepository
            )
        {
            _askMarketRepository = askMarketRepository;
            _ordersRepository = ordersRepository;
            _bidMarketRepository = bidMarketRepository;
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

                        var marketModel = new MarketModel()
                        {
                            ProductId = order.ProductId,
                            OrderId = order.Id,
                            Volume = order.Volume,
                            Price = order.Price,
                            OnlyFullExecution = order.OnlyFullExecution,
                            SubmissionTime = order.SubmissionTime
                        };
                        await _askMarketRepository.CreateNewAsync(marketModel);
                    }
                    else
                    {
                        var matchedOrder = await _ordersRepository.GetByIdAsync(matchedMarketModel.Id);
                        matchedOrder.Status = OrderStatus.Executing;
                        order.Status = OrderStatus.Executing;
                        // send in candidates

                        await _ordersRepository.UpdateAsync(matchedOrder);
                        await _ordersRepository.UpdateAsync(order);
                        if (matchedMarketModel.OnlyFullExecution)
                        { 
                            await _bidMarketRepository.DeleteAsync(matchedOrder.Id);
                        }
                        else
                        {
                            var newVolume = matchedOrder.Volume - order.Volume;
                            if(newVolume != 0)
                            {
                                var newPartialBidOrder = new OrderModel()
                                {
                                    OrderType = matchedOrder.OrderType,
                                    ProductId = matchedOrder.ProductId,
                                    Volume = newVolume,
                                    Price = matchedOrder.Price,
                                    InvestorId = matchedOrder.InvestorId,
                                    OnlyFullExecution = matchedOrder.OnlyFullExecution,
                                    LimitTime = matchedOrder.LimitTime,
                                    SubmissionTime = matchedOrder.SubmissionTime,
                                    Status = OrderStatus.Validated
                                };
                                var newOrderId = await _ordersRepository.CreateNewAsync(newPartialBidOrder);
                                await _bidMarketRepository.DeleteAsync(matchedOrder.Id);
                                await ProcessOrderAsync(newOrderId);
                            }
                        }
                    }
                }
                else
                {
                    var matchedMarketModel = await BidPartialExecSearchAsync(order, relevantBids);
                    if(matchedMarketModel == null)
                    {
                        //in separet method put logic of change status and add to market repo
                        order.Status = OrderStatus.Active; 
                        await _ordersRepository.UpdateAsync(order);

                        var marketModel = new MarketModel()
                        {
                            ProductId = order.ProductId,
                            OrderId = order.Id,
                            Volume = order.Volume,
                            Price = order.Price,
                            OnlyFullExecution = order.OnlyFullExecution,
                            SubmissionTime = order.SubmissionTime
                        };
                        await _askMarketRepository.CreateNewAsync(marketModel);
                    }
                    else
                    {
                        var matchedOrder = await _ordersRepository.GetByIdAsync(matchedMarketModel.Id);
                        matchedOrder.Status = OrderStatus.Executing;
                        order.Status = OrderStatus.Executing;
                        // send in candidates kafka

                        await _ordersRepository.UpdateAsync(matchedOrder);
                        await _ordersRepository.UpdateAsync(order);
                        if (matchedMarketModel.OnlyFullExecution)
                        {
                            var newVolume = order.Volume - matchedOrder.Volume;
                            if(newVolume != 0) 
                            {
                                var newPartialAskOrder = new OrderModel()
                                {
                                    OrderType = order.OrderType,
                                    ProductId = order.ProductId,
                                    Volume = newVolume,
                                    Price = order.Price,
                                    InvestorId = order.InvestorId,
                                    OnlyFullExecution = order.OnlyFullExecution,
                                    LimitTime = order.LimitTime,
                                    SubmissionTime = order.SubmissionTime,
                                    Status = OrderStatus.Validated
                                };
                                var newOrderId = await _ordersRepository.CreateNewAsync(newPartialAskOrder);
                                await ProcessOrderAsync(newOrderId);
                            }
                            await _bidMarketRepository.DeleteAsync(matchedOrder.Id);
                        }
                        else
                        {
                            var newVolume = matchedOrder.Volume - order.Volume;
                            if (newVolume != 0)
                            {
                                var newPartialBidOrder = new OrderModel()
                                {
                                    OrderType = matchedOrder.OrderType,
                                    ProductId = matchedOrder.ProductId,
                                    Volume = newVolume,
                                    Price = matchedOrder.Price,
                                    InvestorId = matchedOrder.InvestorId,
                                    OnlyFullExecution = matchedOrder.OnlyFullExecution,
                                    LimitTime = matchedOrder.LimitTime,
                                    SubmissionTime = matchedOrder.SubmissionTime,
                                    Status = OrderStatus.Validated
                                };
                                var newOrderId = await _ordersRepository.CreateNewAsync(newPartialBidOrder);
                                await _bidMarketRepository.DeleteAsync(matchedOrder.Id);
                                await ProcessOrderAsync(newOrderId);
                            }
                        }
                    }


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
        private async Task<MarketModel?> BidFullExecSearchAsync(OrderModel model, List<MarketModel> relevantMarketList)
        {
            foreach (var listItem in relevantMarketList)
            {
                if (listItem.OnlyFullExecution)
                {
                    if(model.Volume == listItem.Volume)
                    {
                        return listItem;
                    }
                    continue;
                }
                if(listItem.Volume >= model.Volume)
                    return listItem;
            }
            return null;
        }
        private async Task<MarketModel?> BidPartialExecSearchAsync(OrderModel model, List<MarketModel> relevantMarketList)
        {
            foreach (var listItem in relevantMarketList)
            {
                if (listItem.OnlyFullExecution)
                {
                    if(model.Volume >= listItem.Volume)
                    {
                        return listItem;
                    }
                    continue;
                }
                return listItem;
            }
            return null;
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
