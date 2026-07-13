using LaundryManagement.DTOs;
using LaundryManagement.Model;
using LaundryManagement.Repository.IRepo;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;

namespace LaundryManagement.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class Usercontroller : ControllerBase
    {
        private readonly IUserIRepo _cl;
        private readonly IConfiguration _conf;

        public Usercontroller(
            IUserIRepo cl,
            IConfiguration conf)
        {
            _cl = cl;
            _conf = conf;
        }

        
        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var users =await _cl.Getalluser();

            var result = users.Select(x => new UserDTO
            {
                Name = x.Name,
                Email = x.Email,
                Password = x.Password,
                role = x.role,
                Address = x.Address,
                MobileNo = x.MobileNo
            });

            return Ok(result);
        }

    

        [HttpGet("{id}")]
        public async Task<IActionResult>GetUserById(string id)
        {
            var user =await _cl.GetbyuserId(id);
            if (user == null)
                return NotFound();
            var dto = new UserDTO
            {
                Name = user.Name,
                Email = user.Email,
                Password = user.Password,
                role = user.role
            };

            return Ok(dto);
        }

      

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult>AddUser(UserDTO dto)
        {
            var user = new Userla
            {
                Id = Guid.NewGuid().ToString(),
                Name = dto.Name,
                Email = dto.Email,
                Password = dto.Password,
                role = dto.role,
                Address = dto.Address,
                MobileNo = dto.MobileNo
            };
            await _cl.adduser(user);
            return Ok(new
            {
                Message = "User Added Successfully",
                UserId = user.Id
            });
        }

       

        [AllowAnonymous]
        [HttpPost("AdminLogin")]
        public async Task<IActionResult>AdminLogin(LoginDTO dto)
        {
            var users =await _cl.Getalluser();

            var admin =users.FirstOrDefault(x =>x.Email.Trim().ToLower()== dto.Email.Trim().ToLower()&&x.Password.Trim()== dto.Password.Trim()&&x.role.Trim().ToLower()== "admin");

            if (admin == null)
                return Unauthorized("Invalid Admin Login");
            var token =GenerateToken(admin.Id,admin.Email,admin.role);
            return Ok(new
            {
                Token = token,
                Role = admin.role,
                UserId = admin.Id   
            });
        }

       

        [AllowAnonymous]
        [HttpPost("CustomerLogin")]
        public async Task<IActionResult>CustomerLogin(LoginDTO dto)
        {
            var users =await _cl.Getalluser();

            var customer =users.FirstOrDefault(x =>

                    x.Email.Trim().ToLower()
                    == dto.Email.Trim().ToLower()

                    &&

                    x.Password.Trim()
                    == dto.Password.Trim()

                    &&

                    x.role.Trim().ToLower()
                    == "customer");

            if (customer == null)
                return Unauthorized("Invalid Customer Login");

            var token =GenerateToken(customer.Id,customer.Email,customer.role);
            return Ok(new
            {
                Token = token,
                Role = customer.role,
                UserId = customer.Id   
            });
        }

       

        private string GenerateToken(string userId,string email,string role)
        {
            var jwtSettings =_conf.GetSection("Jwt");

            var key =new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]));

            var creds =new SigningCredentials(key,SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier,userId),

                new Claim(ClaimTypes.Email,email),

                new Claim(ClaimTypes.Role,role)
            };

            var token =new JwtSecurityToken(issuer:jwtSettings["Issuer"],

                    audience:jwtSettings["Audience"],
                    claims: claims,
                    expires:DateTime.Now.AddMinutes(Convert.ToDouble(jwtSettings["DurationInMinutes"])),
                    signingCredentials: creds);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}