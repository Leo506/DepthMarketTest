using DepthMarketTest.Models;
using MongoDB.Bson;

namespace DepthMarketTest.ViewModels
{
    public class OrderViewModel
    {
        public string Id { get; set; }
        public OrderTypes OrderType { get; set; }

        public string ProductId { get; set; }

        public double Volume { get; set; }

        public double Price { get; set; }

        public string InvestorId { get; set; }

        public bool OnlyFullExecution { get; set; }

        public DateTime LimitTime { get; set; }
        public DateTime SubmittionTime { get; set; }
        public OrderStatus Status { get; set; }
    }
}
