using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Models
{
    public static class Test
    {
        private static IMongoDatabase mongoDB;
        public static IMongoDatabase getDatabase()
        {
            var mongoClient = new MongoClient("mongodb://localhost:27017");
            return mongoClient.GetDatabase("CoffeeShopDB");
        }
        public static List<Category> getCategoryById(this ObjectId _id)
        {
            mongoDB = getDatabase();
            return mongoDB.GetCollection<Category>("Category").Find(x => x.category_id == _id).ToList();
        }
    }
}
