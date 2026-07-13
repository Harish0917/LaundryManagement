using LaundryMVC.Models;
using LaundryMVC.Repository.IRepo;
using System.Net.Http.Headers;

namespace LaundryMVC.Repository.Repo
{
    public class LauRepo : ILauIRepo
    {
        private readonly HttpClient _http;
        private readonly IHttpContextAccessor _context;

        string itemUrl ="https://localhost:7087/api/Item";

        string orderUrl ="https://localhost:7087/api/Order";

        string userUrl ="https://localhost:7087/api/User";

        public LauRepo(HttpClient http,IHttpContextAccessor context)
        {
            _http = http;
            _context = context;
        }

        
        public async Task<List<Item>> GetAllItems()
        {
            var response =await _http.GetFromJsonAsync<List<Item>>(itemUrl);
            return response ?? new List<Item>();
        }

        // GET ITEM BY ID
        public async Task<Item> GetItemById(int id)
        {
            return await _http.GetFromJsonAsync<Item>($"{itemUrl}/{id}");
        }

        // ADD ITEM
        public async Task<bool> AddItem(Item item)
        {
            var token =_context.HttpContext.Session.GetString("JWToken");

            _http.DefaultRequestHeaders.Authorization =new AuthenticationHeaderValue("Bearer",token);

            var response =await _http.PostAsJsonAsync(itemUrl,item);

            return response.IsSuccessStatusCode;
        }

     
        public async Task UpdateItem(Item item)
        {
            var token =_context.HttpContext.Session.GetString("JWToken");

            _http.DefaultRequestHeaders.Authorization =new AuthenticationHeaderValue("Bearer",token);

            await _http.PutAsJsonAsync($"{itemUrl}/{item.Id}",item);
        }

        // DELETE ITEM
        public async Task DeleteItem(int id)
        {
            var token =_context.HttpContext.Session.GetString("JWToken");

            _http.DefaultRequestHeaders.Authorization =new AuthenticationHeaderValue("Bearer",token);

            await _http.DeleteAsync($"{itemUrl}/{id}");
        }

        
        public async Task<bool> SaveOrder(Order order)
        {
            var token =_context.HttpContext.Session.GetString("JWToken");

            if (string.IsNullOrEmpty(token))
                return false;

            _http.DefaultRequestHeaders.Authorization =new AuthenticationHeaderValue("Bearer",token);

            var orderDto = new
            {
                UserId = order.UserId,
                ItemName = order.ItemName,
                Quantity = order.Quantity,
                TotalPrice = order.TotalPrice,        
                PaymentMethod = order.PaymentMethod,
                Status = "Pending"
            };

            var response =await _http.PostAsJsonAsync(orderUrl, orderDto);
            return response.IsSuccessStatusCode;
        }

        // DELETE ORDER
        public async Task DeleteOrder(int id)
        {
            var token =_context.HttpContext.Session.GetString("JWToken");

            _http.DefaultRequestHeaders.Authorization =new AuthenticationHeaderValue("Bearer", token);

            await _http.DeleteAsync($"{orderUrl}/{id}");
        }

        // GET ALL ORDERS
        public async Task<List<Order>> GetAllOrders()
        {
            var token =_context.HttpContext.Session.GetString("JWToken");

            _http.DefaultRequestHeaders.Authorization =new AuthenticationHeaderValue("Bearer",token);

            var response =await _http.GetFromJsonAsync<List<Order>>(orderUrl);

            return response ?? new List<Order>();
        }

        // UPDATE ORDER
        public async Task UpdateOrder(Order order)
        {
            var token =_context.HttpContext.Session.GetString("JWToken");

            _http.DefaultRequestHeaders.Authorization =new AuthenticationHeaderValue("Bearer",token);

            await _http.PutAsJsonAsync($"{orderUrl}/{order.Id}",order);
        }

       
        public async Task<List<Order>> GetOrdersByUserId(int userId)
        {
           var token =_context.HttpContext.Session.GetString("JWToken");
            _http.DefaultRequestHeaders.Authorization =new AuthenticationHeaderValue("Bearer",token);
            var response =await _http.GetFromJsonAsync<List<Order>>($"{orderUrl}/user/{userId}");
            return response ?? new List<Order>();
        }


        public async Task<List<Register>> GetAllCustomers()
        {
            var token =_context.HttpContext.Session.GetString("JWToken");
            _http.DefaultRequestHeaders.Authorization =new AuthenticationHeaderValue("Bearer",token);
            var response =await _http.GetFromJsonAsync<List<Register>>(userUrl);
            return response ?? new List<Register>();
        }
    }
}