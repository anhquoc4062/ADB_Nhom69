using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Models
{
    public class Category
    {
        [BsonId]
        public ObjectId category_id { get; set; }
        [BsonElement]
        public string category_name { get; set; }

    }

}
