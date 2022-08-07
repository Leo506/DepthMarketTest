using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DepthMarketTest.Models
{
    public class MarketModel
    {
        [BsonId]
        [BsonIgnoreIfNull]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("product_id")]
        public string ProductId { get; set; } = null!;

        [BsonElement("order_id")]
        public string OrderId { get; set; } = null!;

        [BsonElement("volume")]
        public double Volume { get; set; }

        [BsonElement("price")]
        public double Price { get; set; }

        [BsonElement("only_full_execution")]
        public bool OnlyFullExecution { get; set; }

        [BsonElement("submittion_time")]
        public DateTime SubmissionTime { get; set; }
    }
}
