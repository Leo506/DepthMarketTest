using DepthMarketTest.Base;
using DepthMarketTest.Models;
using DepthMarketTest.Repository;
using DepthMarketTest.ViewModels;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace DepthMarketTest.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrdersRepository _ordersRepository;
        public OrdersController(IOrdersRepository ordersRepository)
        {
            _ordersRepository = ordersRepository;
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            var people = await _ordersRepository.GetByIdAsync(id);
            if (people == null)
            {
                return NotFound();
            }

            return Ok(people);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody]OrderViewModel model)
        {
            // лень было мапер тут прикручивать
            var orderModel = new OrderModel()
            {
                OrderType = model.OrderType,
                ProductId = model.ProductId,
                Volume = model.Volume,
                Price = model.Price,
                InvestorId = model.InvestorId,
                Deadline = new BsonDateTime(DateTime.Now),
                Status = model.Status
            };
            await _ordersRepository.CreateNewAsync(orderModel);
            return CreatedAtAction(nameof(Get), new { id = orderModel.Id }, orderModel);
        }

        [HttpPut]
        public async Task<IActionResult> Put(OrderViewModel model)
        {
            var order = await _ordersRepository.GetByIdAsync(model.Id);
            if (order == null)
            {
                return NotFound();
            }
            var orderModel = new OrderModel()
            {
                OrderType = model.OrderType,
                ProductId = model.ProductId,
                Volume = model.Volume,
                Price = model.Price,
                InvestorId = model.InvestorId,
                OnlyFullExecution = model.OnlyFullExecution,
                Deadline = model.Deadline,
                Status = model.Status
            };
            await _ordersRepository.UpdateAsync(orderModel);
            return NoContent();
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(string id)
        {
            var people = await _ordersRepository.GetByIdAsync(id);
            if (people == null)
            {
                return NotFound();
            }

            await _ordersRepository.DeleteAsync(id);
            return NoContent();
        }
    }
}