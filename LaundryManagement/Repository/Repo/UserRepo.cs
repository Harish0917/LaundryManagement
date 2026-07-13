using LaundryManagement.Model;
using LaundryManagement.Model.data;
using LaundryManagement.Repository.IRepo;
using Microsoft.EntityFrameworkCore;

namespace LaundryManagement.Repository.Repo
{
    public class UserRepo:IUserIRepo
    {
        private readonly LaundryDb _cto;
        public UserRepo(LaundryDb cto)
        {
            _cto = cto;
        }
        //  LOGIN METHOD
        public async Task<Userla> login(string Email, string password)
        {
            return await _cto.Users.FirstOrDefaultAsync(x =>x.Email == Email &&x.Password == password);
        }

        //  GET ALL USERS
        public async Task<IEnumerable<Userla>> Getalluser()
        {
            return await _cto.Users.ToListAsync();
        }

        //  GET USER BY ID
        public async Task<Userla> GetbyuserId(string Id)
        {
            return await _cto.Users.FindAsync(Id);
        }

        //  ADD USER
        public async Task adduser(Userla user)
        {
            await _cto.Users.AddAsync(user);
            await _cto.SaveChangesAsync();
        }
        // ADMIN LOGIN
        public async Task<Userla?> AdminLogin(string Email, string Password)
        {
            return await _cto.Users.FirstOrDefaultAsync(x =>x.Email == Email && x.Password == Password && x.role == "Admin");
        }

        // CUSTOMER LOGIN
        public async Task<Userla?> CustomerLogin(string Email, string Password)
        {
            return await _cto.Users.FirstOrDefaultAsync(x =>x.Email == Email && x.Password == Password && x.role == "Customer");
        }
    }
}

