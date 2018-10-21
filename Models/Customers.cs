using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Models
{
    public class Customers
    {
        [BsonId]
        [DisplayName("Mã Khách Hàng")]
        public ObjectId Id { get; set; }
        [BsonElement]
        public int customer_id { get; set; }
        [BsonElement]
        public string customer_name { get; set; }
        [BsonElement]
        public string customer_address { get; set; }

    }
}
