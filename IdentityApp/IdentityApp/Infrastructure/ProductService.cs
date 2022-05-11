using IdentityApp.Models;
using Newtonsoft.Json;
using System.Net;

namespace IdentityApp.Infrastructure
{
    public class ProductService
    {
        HttpClient client = new HttpClient();
        string uri;
        string tokenUri;
        class LoginResponse
        {
            public string access_token;
            public string username;
        }

        public ProductService(IConfiguration conf)
        {
            uri =  conf.GetSection("WebApiUri").Value + $"api/products";
            tokenUri = conf.GetSection("WebApiUri").Value + $"token?username=admin@gmail.com&password=12345";
        }

        public async Task<string> GetToken()
        {

            var resp = await client.PostAsync(tokenUri, null); ;
            var result = resp.Content.ReadAsStringAsync().Result;
            var login = JsonConvert.DeserializeObject<LoginResponse>(result);
            return login.access_token;
        }

        public void AddToken(string token)
        {
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
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
