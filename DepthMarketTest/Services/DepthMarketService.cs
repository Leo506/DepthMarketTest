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
                    var matchedMarketModel = BidFullExecSearch(order, ref relevantBids);
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
                        if (matchedMarketModel.OnlyFullExecution)
                        {
                            matchedOrder.Status = OrderStatus.Executing;
                            order.Status = OrderStatus.Executing;
                            // send in candidates also send a volume just like an idea

                            await _ordersRepository.UpdateAsync(matchedOrder);
                            await _ordersRepository.UpdateAsync(order);
                            await _bidMarketRepository.DeleteAsync(matchedOrder.Id);
                        }
                        else
                        {
                            var newVolume = matchedOrder.Volume - order.Volume;
                            if (newVolume != 0)
                            {
                                matchedOrder.Volume = newVolume;
                                await _ordersRepository.UpdateAsync(matchedOrder);
                                matchedMarketModel.Volume = newVolume;
                                await _bidMarketRepository.UpdateAsync(matchedMarketModel);

                                order.Status = OrderStatus.Executing;
                                await _ordersRepository.UpdateAsync(order);
                                // send in candidates
                            }
                            else
                            {
                                matchedOrder.Status = OrderStatus.Executing;
                                order.Status = OrderStatus.Executing;
                                // send in candidates

                                await _ordersRepository.UpdateAsync(matchedOrder);
                                await _ordersRepository.UpdateAsync(order);
                                await _bidMarketRepository.DeleteAsync(matchedOrder.Id);
                            }
                        }
                    }
                }
                else
                {
                    var matchedMarketModels = BidPartialExecSearch(order, ref relevantBids);
                    if(!matchedMarketModels.Any())
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
                    } // correct
                    else
                    {
                        foreach(var matchedMarketModel in matchedMarketModels)
                        {
                            if (!matchedMarketModel.OnlyFullExecution &&
                                matchedMarketModel.Volume > order.Volume)
                            {
                                order.Status = OrderStatus.Executing;
                                await _ordersRepository.UpdateAsync(order);

                                var matchedOrder = await _ordersRepository.GetByIdAsync(matchedMarketModel.OrderId);
                                matchedOrder.Volume -= order.Volume;
                                await _ordersRepository.UpdateAsync(matchedOrder);
                                matchedMarketModel.Volume = matchedOrder.Volume;
                                await _bidMarketRepository.UpdateAsync(matchedMarketModel);
                            }
                            else
                            {
                                var matchedOrder = await _ordersRepository.GetByIdAsync(matchedMarketModel.Id);

                                matchedOrder.Status = OrderStatus.Executing;
                                await _ordersRepository.UpdateAsync(matchedOrder);

                                await _bidMarketRepository.DeleteAsync(matchedMarketModel.Id);
                                // send in candidates
                            }
                            
                        }
                        if(order.Volume == 0)
                        {
                            order.Status = OrderStatus.Executing;
                            await _ordersRepository.UpdateAsync(order);
                        }
                        else
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
        private MarketModel BidFullExecSearch(OrderModel model, ref List<MarketModel> relevantMarketList)
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
        private List<MarketModel> BidPartialExecSearch(OrderModel model, ref List<MarketModel> relevantMarketList)
        {
            var existingVolume = model.Volume;
            var candidatesList = new List<MarketModel>();
            foreach (var listItem in relevantMarketList)
            {
                if (listItem.OnlyFullExecution)
                {
                    if (existingVolume >= listItem.Volume)
                    {
                        existingVolume -= listItem.Volume;
                        candidatesList.Add(listItem);
                    }
                }
                else
                {
                    if(existingVolume >= listItem.Volume)
                    {
                        existingVolume -= listItem.Volume;
                        candidatesList.Add(listItem);
                    }
                    else
                    {
                        candidatesList.Add(listItem);
                        break;
                    }
                }
            }
            return candidatesList;
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
