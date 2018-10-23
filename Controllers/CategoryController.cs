using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class CategoryController : Controller
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
        //[HttpPost]
        public IActionResult Create(string category_name)
        {
            Category newCate = new Category();
            newCate.category_name = category_name;
            mongoDB = getDatabase();
            mongoDB.GetCollection<Category>("Category").InsertOne(newCate);
            
            ViewBag.test = newCate;
            //return View();
            return RedirectToAction("List", "Product", new { area = "" });
        }
        public IActionResult Delete(string category_id)
        {
            mongoDB = getDatabase();
            ObjectId key_id = new ObjectId(category_id); 
            mongoDB.GetCollection<Category>("Category").DeleteOne(self => self.category_id == key_id);
            return RedirectToAction("List", "Product", new { area = "" });
        }

    }
}