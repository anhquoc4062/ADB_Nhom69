using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using WebApplication1.Models;
using MongoDB.Driver.Linq;
using System.Web;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Net.Http.Headers;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace WebApplication1.Controllers
{
    public class ProductController : Controller
    {
        private IMongoDatabase mongoDB;
        public IMongoDatabase getDatabase()
        {
            var mongoClient = new MongoClient("mongodb://localhost:27017");
            return mongoClient.GetDatabase("CoffeeShopDB");
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult List(string product_id_string)
        {
            List<Product> nullList = new List<Product>();
            Product nullProduct = new Product();
            nullList.Add(nullProduct);
            ViewBag.EditData = nullList;
            ViewBag.NullObjectID = new ObjectId("000000000000000000000000");
            
            mongoDB = getDatabase();
            if (product_id_string == null)
            {
                ViewBag.Title = "Thêm";
                ViewBag.Action = "Add";
            }
            else
            {
                ViewBag.Title = "Sửa";
                ViewBag.Action = "Update";
                ObjectId key_id = new ObjectId(product_id_string);
                ViewBag.EditData= from product in mongoDB.GetCollection<Product>("Product").AsQueryable()
                              where product.product_id == key_id
                              select product;
            }
     
            ViewBag.Category = from e in mongoDB.GetCollection<Category>("Category").AsQueryable()
                               select e;

            ViewBag.ProductWithCategory = (from product in mongoDB.GetCollection<Product>("Product").AsQueryable()
                            join cate in mongoDB.GetCollection<Category>("Category").AsQueryable() on product.category_id equals cate.category_id
                            into joiningCategory
                            select new ProductWithCategory
                            {
                                product_name = product.product_name,
                                product_id = product.product_id,
                                product_date = product.product_id.CreationTime,
                                product_img = product.product_img,
                                product_info = product.product_info,
                                product_price = product.product_price,
                                category = joiningCategory
                            }).ToList();
  
            return View();
        }

        [HttpPost]
        public IActionResult Add(Product product, IFormFile image, string category_id_string, string mota)
        {
            if(image != null)
            {
                string path_to_image = "wwwroot/images/product/" + image.FileName;
                using (var stream = new FileStream(path_to_image, FileMode.Create))
                {
                    image.CopyTo(stream);
                }
                product.product_img = image.FileName;
            }
            else
            {
                product.product_img = "";
            }
            product.category_id = new ObjectId(category_id_string);
            product.product_info = mota;
            mongoDB = getDatabase();
            mongoDB.GetCollection<Product>("Product")
                .InsertOne(product);
            return RedirectToAction("List");
        }

        [HttpGet]
        public IActionResult Delete(string product_id_string)
        {
            ObjectId key_id = new ObjectId(product_id_string);
            mongoDB = getDatabase();
            mongoDB.GetCollection<Product>("Product")
                .DeleteOne<Product>(self => self.product_id == key_id);
            return RedirectToAction("List");
        }

        [HttpPost]
        public IActionResult Update(Product product, string mota, string product_id_string,string category_id_string, IFormFile image, string old_image)
        {
            mongoDB = getDatabase();
            if (image == null)
            {
                product.product_img = old_image;
            }
            else
            {
                string path_to_image = "wwwroot/images/product/" + image.FileName;
                using (var stream = new FileStream(path_to_image, FileMode.Create))
                {
                    image.CopyTo(stream);
                }
                product.product_img = image.FileName;
            }
            product.product_id = new ObjectId(product_id_string);
            product.category_id = new ObjectId(category_id_string);
            product.product_info = mota;
            var where = Builders<Product>.Filter.Eq("product_id", product.product_id);
            var update = Builders<Product>.Update
                .Set("product_name", product.product_name)
                .Set("product_price", product.product_price)
                .Set("product_img", product.product_img)
                .Set("product_info", product.product_info)
                .Set("category_id", product.category_id);

            mongoDB.GetCollection<Product>("Product").UpdateOne(where, update);
            return RedirectToAction("List");
        }
        public IActionResult Search(string key)
        {
            mongoDB = getDatabase();
            List<ProductWithCategory> list_data = (from product in mongoDB.GetCollection<Product>("Product").AsQueryable()
                                       join cate in mongoDB.GetCollection<Category>("Category").AsQueryable() on product.category_id equals cate.category_id
                                       into joiningCategory
                                       where product.product_name.Contains(key)
                                       select new ProductWithCategory
                                       {
                                           product_name = product.product_name,
                                           product_id = product.product_id,
                                           product_date = product.product_id.CreationTime,
                                           product_img = product.product_img,
                                           product_info = product.product_info,
                                           product_price = product.product_price,
                                           category = joiningCategory
                                       }).ToList();
            //list_data.Add("Increment" => )
            return Json(list_data);
        }
    }
}