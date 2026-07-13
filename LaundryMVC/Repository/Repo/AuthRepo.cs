using LaundryMVC.Models;
using LaundryMVC.Repository.IRepo;

namespace LaundryMVC.Repository.Repo
{
    public class AuthRepo : IAuthIRepo
    {
        private readonly HttpClient _http;

        private string customerLoginUrl ="https://localhost:7087/api/User/CustomerLogin";
        private string adminLoginUrl ="https://localhost:7087/api/User/AdminLogin";
        private string customerRegisterUrl ="https://localhost:7087/api/User";

        public AuthRepo(HttpClient http)
        {
            _http = http;
        }

      
        public async Task<bool> CustomerRegister(Register reg)
        {
            var response =await _http.PostAsJsonAsync(customerRegisterUrl,reg);
            Console.WriteLine("Status Code: " +response.StatusCode);
            var msg =await response.Content.ReadAsStringAsync();
            Console.WriteLine("Response: " +msg);
            return response.IsSuccessStatusCode;
        }
        public async Task<string>CustomerLogin(Login login)
        {
            var response =await _http.PostAsJsonAsync(customerLoginUrl,login);

            if (!response.IsSuccessStatusCode)
                return null;

            var result =await response.Content.ReadFromJsonAsync<TokenResponse>();

            return result.Token;
        }
        public async Task<string> AdminLogin(Login login)
        {
            var data = new
            {
                email = login.Email,
                password = login.Password
            };

            var response = await _http.PostAsJsonAsync(adminLoginUrl, data);

            if (!response.IsSuccessStatusCode)
                return null;

            var raw = await response.Content.ReadAsStringAsync();

            var json = System.Text.Json.JsonDocument.Parse(raw);
            var token = json.RootElement.GetProperty("token").GetString();

            return token;
        }

        public async Task<bool> AdminRegister(Register reg)
        {
            reg.role = "Admin";

            var response =await _http.PostAsJsonAsync("https://localhost:7087/api/User",reg);

            Console.WriteLine("Admin Register Status: " + response.StatusCode);

            var msg = await response.Content.ReadAsStringAsync();
            Console.WriteLine("Response: " + msg);

            return response.IsSuccessStatusCode;
        }

        public class TokenResponse
        {
            public string Token { get; set; }
            public string Role { get; set; }
        }

    }
}

