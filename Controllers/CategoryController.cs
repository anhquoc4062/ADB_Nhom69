using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using WebApplication1.Models;
using System.Linq;

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
            mongoDB = getDatabase();
            ViewBag.test = mongoDB.GetCollection<Category>("Category").Find(FilterDefinition<Category>.Empty).ToList();
            return View();
        }
        [HttpPost]
        public IActionResult Create(Category category)
        {
            mongoDB = getDatabase();
            mongoDB.GetCollection<Category>("Category").InsertOne(category);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Delete(string category_id_string)
        {
            mongoDB = getDatabase();
            ObjectId key_id = new ObjectId(category_id_string);
            mongoDB.GetCollection<Category>("Category").DeleteOne(key => key.category_id == key_id);
            return RedirectToAction("Index");
        }
    }
}