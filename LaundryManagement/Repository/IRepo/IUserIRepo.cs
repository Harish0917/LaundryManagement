using LaundryManagement.Model;

namespace LaundryManagement.Repository.IRepo
{
    public interface IUserIRepo
    {
        Task<Userla>login(string Email,string password);

        Task<IEnumerable<Userla>> Getalluser();

        Task<Userla>GetbyuserId(string Id);

        Task adduser(Userla user);

        Task<Userla?> AdminLogin(string Email, string Password);

        Task<Userla?> CustomerLogin(string Email, string Password);
        
    }
}
