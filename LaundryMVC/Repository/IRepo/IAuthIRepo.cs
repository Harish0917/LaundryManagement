using LaundryMVC.Models;

namespace LaundryMVC.Repository.IRepo
{
    public interface IAuthIRepo
    {
        Task<string> CustomerLogin(Login login);

        Task<string> AdminLogin(Login login);

        Task<bool> CustomerRegister(Register reg);
        Task<bool> AdminRegister(Register reg);
    }
}