using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DepthMarketTest.Models
{
    public class OrderModel
    {
        [BsonId]
        [BsonIgnoreIfNull]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("type")]
        public OrderTypes OrderType { get; set; }

        [BsonElement("product_id")] 
        public string ProductId { get; set; } = null!;

        [BsonElement("volume")] 
        public double Volume { get; set; }

        [BsonElement("price")]
        public double Price { get; set; }

        [BsonElement("investor_id")] 
        public string InvestorId { get; set; } = null!;

        [BsonElement("only_full_execution")]
        public bool OnlyFullExecution { get; set; } 

        [BsonElement("limit_time")]
        
        public DateTime LimitTime { get; set; }

        [BsonElement("submittion_time")]
        public DateTime SubmissionTime { get; set; }


        [BsonElement("status")]
        public OrderStatus Status { get; set; }

    }
}
