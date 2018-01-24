using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SmartClient.Model;

namespace SmartClient.Web.Controllers
{
    public class ProductController : ApiController
    {
    
        private IEnumerable<Product> QueryAllProducts()
        {
            Product[] products = new Product[]
               {
                    new Product { Id = 1, Name = "Tomato Soup", Category = "Groceries", Price = 1 },
                    new Product { Id = 2, Name = "Yo-yo", Category = "Toys", Price = 3.75M },
                    new Product { Id = 3, Name = "Hammer", Category = "Hardware", Price = 16.99M },
                    new Product { Id = 4, Name = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff"), Category = "Hardware", Price = 1006.99M }
               };

            return products;

        }

        [HttpGet]
        public IEnumerable<Product> GetAllProducts()
        {
            return this.QueryAllProducts();
        }
        [HttpGet]
        public Product GetProductById(int? id)
        {
            if (null == id)
            {
                return null;
            }

            var product = this.QueryAllProducts().FirstOrDefault((p) => p.Id == id);
            if (product == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
            return product;
        }

        [HttpGet]
        public IEnumerable<Product> GetProductsByCategory(string category)
        {
            return this.QueryAllProducts().Where(p => string.Equals(p.Category, category,
                    StringComparison.OrdinalIgnoreCase));
        }

    }
}
