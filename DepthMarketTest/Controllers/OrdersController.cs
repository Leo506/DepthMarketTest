using DepthMarketTest.Models;
using DepthMarketTest.Repository;
using DepthMarketTest.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace DepthMarketTest.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrdersRepository _ordersRepository;
        private readonly IAskMarketRepository _askMarketRepository;
        private readonly IBidMarketRepository _bidMarketRepository;
        public OrdersController(
            IOrdersRepository ordersRepository,
            IAskMarketRepository askMarketRepository,
            IBidMarketRepository bidMarketRepository
            )
        {
            _ordersRepository = ordersRepository;
            _askMarketRepository = askMarketRepository;
            _bidMarketRepository = bidMarketRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var people = await _ordersRepository.GetAllAsync();
            if (people == null)
            {
                return NotFound();
            }

            return Ok(people);
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
            // ���� ���� ����� ��� ������������
            var orderModel = new OrderModel()
            {
                OrderType = model.OrderType,
                ProductId = model.ProductId,
                Volume = model.Volume,
                Price = model.Price,
                InvestorId = model.InvestorId,
                LimitTime = model.LimitTime,
                SubmissionTime = model.SubmittionTime,
                Status = model.Status,
                OnlyFullExecution = model.OnlyFullExecution
            };
            var orderId = await _ordersRepository.CreateNewAsync(orderModel);
            return CreatedAtAction(nameof(Get), new { id = orderId }, orderModel);
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
                LimitTime = model.LimitTime,
                SubmissionTime = model.SubmittionTime,
                Status = model.Status,
                OnlyFullExecution = model.OnlyFullExecution
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