using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class DemoController : Controller
    {
        private IMongoDatabase mongoDatabase;
        public IMongoDatabase GetMongoDatabase()
        {
            var mongoClient = new MongoClient("mongodb://localhost:27017");
            return mongoClient.GetDatabase("CoffeeShopDB");
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Show()
        {
            mongoDatabase = GetMongoDatabase();
            var result = mongoDatabase.GetCollection<Category>("Category").Find(FilterDefinition<Category>.Empty).ToList();
            return View(result);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Customers customer)
        {
            mongoDatabase = GetMongoDatabase();
            mongoDatabase.GetCollection<Customers>("Customers").InsertOne(customer);
            return RedirectToAction("Show");
        }
        [HttpGet]
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            //get database connection
            mongoDatabase = GetMongoDatabase();
            Customers customer = mongoDatabase.GetCollection<Customers>("Customers").Find<Customers>(key => key.customer_id == id).FirstOrDefault();
            if(customer == null)
            {
                return NotFound();
            }
            return View(customer);
        }
        [HttpPost]
        public IActionResult Delete(Customers customers)
        {
            mongoDatabase = GetMongoDatabase();
            mongoDatabase.GetCollection<Customers>("Customers").DeleteOne<Customers>(key => key.customer_id == customers.customer_id);
            return RedirectToAction("Show");
        }
        [HttpGet]
        public IActionResult Edit(int id)
        {
            mongoDatabase = GetMongoDatabase();
            Customers customer = mongoDatabase.GetCollection<Customers>("Customers").Find<Customers>(key => key.customer_id == id).FirstOrDefault();
            return View(customer);
        }
        [HttpPost]
        public IActionResult Edit(Customers customer)
        {
            mongoDatabase = GetMongoDatabase();
            //xây dựng điều kiện update
            var condition = Builders<Customers>.Filter.Eq("customer_id", customer.customer_id);
            //xây dựng câu lệnh update
            var update = Builders<Customers>.Update.Set("customer_name", customer.customer_name);
            update = update.Set("customer_address", customer.customer_address);
            mongoDatabase.GetCollection<Customers>("Customers").UpdateOne(condition, update);
            return RedirectToAction("Show");
        }
    }
}