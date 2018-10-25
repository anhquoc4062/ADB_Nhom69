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
     
        public IActionResult Test()
        {
            mongoDB = getDatabase();
            ViewBag.Test = mongoDB.GetCollection<Product>("Product").Find(FilterDefinition<Product>.Empty).ToList();
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
     
            ViewBag.Category = from cate in mongoDB.GetCollection<Category>("Category").AsQueryable()
                               select cate;

            ViewBag.ProductWithCategory = (from product in mongoDB.GetCollection<Product>("Product").AsQueryable()
                           join cate in mongoDB.GetCollection<Category>("Category").AsQueryable() on product.category_id equals cate.category_id
                           into joiningCategory
                           select new ProductWithCategory
                            {
                                product_name = product.product_name,
                                product_id = product.product_id,
                               //product_date = product.product_id.CreationTime,
                                product_img = product.product_img,
                                product_info = product.product_info,
                                product_price = product.product_price,
                                category = joiningCategory
                            }).ToList();
            ViewBag.Product = from product in mongoDB.GetCollection<Product>("Product").AsQueryable()
                              select product;


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
        [HttpPost]
        public IActionResult Search(string key)
        {
            mongoDB = getDatabase();
            List<ProductWithCategory> list_data = new List<ProductWithCategory>();
            if(key == "#%$null$%#") {
                list_data = (from product in mongoDB.GetCollection<Product>("Product").AsQueryable()
                             join cate in mongoDB.GetCollection<Category>("Category").AsQueryable() on product.category_id equals cate.category_id
                             into joiningCategory
                             select new ProductWithCategory
                             {
                                 product_name = product.product_name,
                                 product_id = product.product_id,
                                 product_date = "",
                                 product_img = product.product_img,
                                 product_info = product.product_info,
                                 product_price = product.product_price,
                                 category = joiningCategory
                             }).ToList();

            }
            else
            {
                key = key.ToLower();
                list_data = (from product in mongoDB.GetCollection<Product>("Product").AsQueryable()
                            join cate in mongoDB.GetCollection<Category>("Category").AsQueryable() on product.category_id equals cate.category_id
                            into joiningCategory
                            where product.product_name.ToLower().Contains(key)
                            select new ProductWithCategory
                            {
                                product_name = product.product_name,
                                product_id = product.product_id,
                                product_date = "",
                                product_img = product.product_img,
                                product_info = product.product_info,
                                product_price = product.product_price,
                                category = joiningCategory
                            }).ToList();
            }
            string htmlString = "";
            int count = 0;
            foreach (var item in list_data)
            {
                htmlString += "<tr data-id=" + item.product_id.Increment + ">";
                htmlString += "<td>" + item.product_id.Increment + "</td>";
                htmlString += "<td>" + item.product_name + "</td>";
                htmlString += "<td>" + string.Format("{0:N0}", item.product_price) + " đ</td>";
                htmlString += "<td>" + item.category.First().category_name + "</td>";
                htmlString += "<td>" + item.product_id.CreationTime + "</td></tr>";
                htmlString += "<tr data-id='" + item.product_id.Increment + "' style='display:none'>";
                htmlString += "<td colspan='5' style='font-size: 15px'><div class='detail'><div class='container'><div class='row'>";
                htmlString += "<div class='col-lg-2'><img width='150' height='150' src=/images/product/" + item.product_img + " /></div>";
                htmlString += "<div class='col-lg-4'><div class='row form-group detail-row'><div class='col col-md-4'>";
                htmlString += "<label class=' form-control-label'>Mã sản phẩm:</label></div><div class='col-12 col-md-8'>";
                htmlString += "<p class='form-control-static'>" + item.product_id.Increment + "</p></div></div>";
                htmlString += "<div class='row form-group detail-row'><div class='col col-md-4'>";
                htmlString += "<label class='form-control-label'>Tên sản phẩm:</label></div>";
                htmlString += "<div class='col-12 col-md-8'><p class='form-control-static'>" + item.product_name + "</p></div></div>";
                htmlString += "<div class='row form-group detail-row'><div class='col col-md-4'>";
                htmlString += "<label class='form-control-label'>Đơn giá:</label></div><div class='col-12 col-md-8'>";
                htmlString += "<p class='form-control-static'>" + string.Format("{0:N0}", item.product_price) + " đ</p></div></div>";
                htmlString += "<div class='row form-group detail-row'><div class='col col-md-4'>";
                htmlString += "<label class='form-control-label'>Loại sản phẩm:</label></div><div class='col-12 col-md-8'>";
                htmlString += "<p class='form-control-static'>" + item.category.First().category_name + "</p></div></div></div>";
                htmlString += "<div class='col-lg-5 ml-auto'><div class='row form-group detail-row'><div class='col col-md-4'>";
                htmlString += "<label class='form-control-label'>Mô tả:</label></div></div><div class='form-control-static'>";
                htmlString += "<p class='form-control-static'>" + item.product_info + "</p></div></div>";
                htmlString += "</div><div class='row'><div class='manage-button'>";
                htmlString += "<button type='submit' class='btn btn-success btn-sm' onclick=window.location.href='/Product/List?product_id_string="+ item.product_id + "'>";
                htmlString += "<i class='fa fa-edit'></i> Cập nhật</button><button type='submit' class='btn btn-danger btn-sm' onclick=window.location.href='/Product/Delete?product_id_string=" + item.product_id + "'>";
                htmlString += "<i class='fa fa-remove'></i> Xóa</button></div></div></div></div></td></tr>";
                count++;
            }
            if (count == 0)
            {
                htmlString = "<tr><td colspan=5 style=text-align:center; font-size:30px>Không tìm thấy sản phẩm</td></tr>";
            }
            //htmlString = key;
            return Json(htmlString);
        }
    }
}