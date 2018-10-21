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
        public IActionResult List()
        {
            mongoDB = getDatabase();
            ViewBag.Product = mongoDB.GetCollection<Product>("Product").Find(FilterDefinition<Product>.Empty).ToList();
            ViewBag.Category = from e in mongoDB.GetCollection<Category>("Category").AsQueryable()
                               select e;

            ViewBag.Test = (from product in mongoDB.GetCollection<Product>("Product").AsQueryable()
                        join cate in mongoDB.GetCollection<Category>("Category").AsQueryable() on product.category_id equals cate.category_id
                        into joiningCategory
                        select new ProductWithCategory
                        {
                            product_name = product.product_name,
                            product_id = product.product_id,
                            product_date = product.product_date,
                            product_img = product.product_img,
                            product_info = product.product_info,
                            product_price = product.product_price,
                            category = joiningCategory
                        }).ToList();
            //ViewBag.Test = query.ToList();
            return View();
        }

        [HttpPost]
        public ActionResult Add(Product product, IFormFile file)
        {
            string path_to_image = "wwwroot/images/product/"+file.FileName;
            using (var stream = new FileStream(path_to_image, FileMode.Create))
            {
                file.CopyTo(stream);
            }
            //ViewBag.category = product.category_id_string;
            product.category_id = new ObjectId(product.category_id_string);
            product.product_img = file.FileName;
            //ViewBag.category = product.category_id;
            mongoDB = getDatabase();
            //Insert to Product collection
            mongoDB.GetCollection<Product>("Product").InsertOne(product);
            return View();

            //return RedirectToAction("List");

        }
    }
}