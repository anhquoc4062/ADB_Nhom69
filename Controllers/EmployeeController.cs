using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using WebApplication1.Models;
using X.PagedList;

namespace WebApplication1.Controllers
{
    public class EmployeeController : Controller
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

        public IActionResult List(int? page)
        {
            mongoDB = getDatabase();
            var allEmp = from employee in mongoDB.GetCollection<Employee>("Employee").AsQueryable()
                         select employee;
            var pageNumber = page ?? 1;
            var onePageEmp = allEmp.ToPagedList(pageNumber, 5);
            ViewBag.Employee = onePageEmp;

            return View();
        }
        [HttpPost]
        public IActionResult Add(string emp_name, string emp_email, string emp_tel, string emp_address, int emp_sex, string emp_position, IFormFile image)
        {
            Employee newEmp = new Employee();
            if (image != null)
            {
                string path_to_image = "wwwroot/images/employee/" + image.FileName;
                using (var stream = new FileStream(path_to_image, FileMode.Create))
                {
                    image.CopyTo(stream);
                }
                newEmp.emp_avatar = image.FileName;
            }
            else
            {
                newEmp.emp_avatar = "";
            }
            newEmp.emp_name = emp_name;
            newEmp.emp_email = emp_email;
            newEmp.emp_tel = emp_tel;
            newEmp.emp_address = emp_address;
            newEmp.emp_sex = emp_sex;
            newEmp.emp_position = emp_position;
            mongoDB = getDatabase();
            mongoDB.GetCollection<Employee>("Employee").InsertOne(newEmp);
            return RedirectToAction("List");
        }
        [HttpPost]
        public IActionResult Update(string emp_id, string emp_name, string emp_email, string emp_tel, string emp_address, int emp_sex, string emp_position, IFormFile image, string old_image)
        {
            string emp_avatar = "";
            if (image != null)
            {
                string path_to_image = "wwwroot/images/employee/" + image.FileName;
                using (var stream = new FileStream(path_to_image, FileMode.Create))
                {
                    image.CopyTo(stream);
                }
                emp_avatar = image.FileName;
            }
            else
            {
                emp_avatar = old_image;
            }
            ObjectId key_id = new ObjectId(emp_id);
            var where = Builders<Employee>.Filter.Eq("emp_id", key_id);
            var update = Builders<Employee>.Update
                .Set("emp_name", emp_name)
                .Set("emp_email", emp_email)
                .Set("emp_tel", emp_tel)
                .Set("emp_address", emp_address)
                .Set("emp_sex", emp_sex)
                .Set("emp_position", emp_position)
                .Set("emp_avatar", emp_avatar);
            mongoDB = getDatabase();
            mongoDB.GetCollection<Employee>("Employee").UpdateOne(where, update);

            return RedirectToAction("List");
        }
        [HttpGet]
        public IActionResult Delete(string emp_id)
        {
            ObjectId key_id = new ObjectId(emp_id);
            mongoDB = getDatabase();
            mongoDB.GetCollection<Employee>("Employee").DeleteOne(self => self.emp_id == key_id);
            return RedirectToAction("List");
        }
        [HttpPost]
        public IActionResult Search(int? page, string key_word)
        {
            if(key_word == null)
            {
                return RedirectToAction("List");
            }
            else
            {
                mongoDB = getDatabase();
                key_word = key_word.ToLower();
                var allEmp = from employee in mongoDB.GetCollection<Employee>("Employee").AsQueryable()
                             where employee.emp_name.ToLower().Contains(key_word)
                             select employee;
                var pageNumber = page ?? 1;
                var onePageEmp = allEmp.ToPagedList(pageNumber, 5);
                ViewBag.Employee = onePageEmp;
                ViewBag.Count = allEmp.Count();
                return View("List");
            }
            
        }
    }
}