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
        [BsonIgnoreIfNull]
        public bool? OnlyFullExecution { get; set; } = null;

        [BsonElement("deadline")]
        [BsonIgnoreIfNull]
        public BsonDateTime? Deadline { get; set; }


        [BsonElement("status")]
        public OrderStatus Status { get; set; }

    }
}
