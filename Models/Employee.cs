using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Models
{
    public class Employee
    {
        [BsonId]
        public ObjectId emp_id { get; set; }
        [BsonElement]
        public string emp_name { get; set; }
        [BsonElement]
        public string emp_avatar { get; set; }
        [BsonElement]
        public string emp_email { get; set; }
        [BsonElement]
        public string emp_tel { get; set; }
        [BsonElement]
        public string emp_address { get; set; }
        [BsonElement]
        public int emp_sex { get; set; }
        [BsonElement]
        public string emp_position { get; set; }
    }
}
