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
        public ObjectId product_id;
        [BsonElement]
        [DisplayName("Tên sản phẩm")]
        public string product_name;
        [BsonElement]
        [DisplayName("Đơn giá")]
        public int product_price;
        [BsonElement]
        [DisplayName("Mã loại")]
        public ObjectId category_id;
        [BsonElement]
        [DisplayName("Hình ảnh")]
        public string product_img;
        [BsonElement]
        [DisplayName("Mô tả")]
        public string product_info;
        [BsonElement]
        [DisplayName("Ngày tạo")]
        public string product_date;

    }
}
