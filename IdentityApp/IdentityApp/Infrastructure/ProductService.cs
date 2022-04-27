using IdentityApp.Models;
using Newtonsoft.Json;
using System.Net;

namespace IdentityApp.Infrastructure
{
    public class ProductService
    {
        HttpClient client = new HttpClient();
        string uri; 

        public ProductService(IConfiguration conf)
        {
            uri =  conf.GetSection("WebApiUri").Value + $"/products";
        }

        public async Task<IEnumerable<Product>> GetProducts()
        {
            var resp = await client.GetAsync(uri);
            var result = resp.Content.ReadAsStringAsync().Result;
            var prods = JsonConvert.DeserializeObject<List<Product>>(result);
            return prods;
        }

        public async Task<Product> GetProductByID(int id)
        {
            var resp = await client.GetAsync(uri + $"/{id}");
            var result = resp.Content.ReadAsStringAsync().Result;
            var product = JsonConvert.DeserializeObject<Product>(result);
            return product;
        }
        public async Task<HttpStatusCode> AddProduct(Product product)
        {
            var content = new StringContent(JsonConvert.SerializeObject(product),
                System.Text.Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PostAsync(
                uri, content);
            return response.StatusCode;
        }

        public async Task<HttpStatusCode> EditProduct(int id, Product product)
        {
            var content = new StringContent(JsonConvert.SerializeObject(product),
                System.Text.Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PutAsync(
                uri + $"/{id}", content);
            return response.StatusCode;
        }

        public async Task<HttpStatusCode> DeleteProduct(int id)
        {
            HttpResponseMessage response = await client.DeleteAsync(
                uri + $"/{id}");
            return response.StatusCode;
        }



    }
}
