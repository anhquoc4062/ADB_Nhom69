using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Models
{
    public class Product
    {
        [BsonId]
        [DisplayName("Mã sản phẩm")]
        public ObjectId product_id { get; set; }
        [BsonElement]
        [DisplayName("Tên sản phẩm")]
        public string product_name { get; set; }
        [BsonElement]
        [DisplayName("Đơn giá")]
        public int product_price { get; set; }
        [BsonElement]
        [DisplayName("Mã loại")]
        public ObjectId category_id { get; set; }
        [BsonElement]
        [DisplayName("Hình ảnh")]
        public string product_img { get; set; }
        [BsonElement]
        [DisplayName("Mô tả")]
        public string product_info { get; set; }
        [BsonElement]
        [DisplayName("Ngày tạo")]
        public string product_date { get; set; }


    }
}
