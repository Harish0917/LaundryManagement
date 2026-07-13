using LaundryMVC.Models;
using LaundryMVC.Repository.IRepo;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace LaundryMVC.Controllers
{
    public class AdminController : Controller
    {
        private readonly ILauIRepo _repo;

        public AdminController(ILauIRepo repo)
        {
            _repo = repo;
        }
        private bool IsAdmin()
        {
            return HttpContext.Session.GetString("Role") == "Admin";
        }

        private bool IsCustomer()
        {
            return HttpContext.Session.GetString("Role") == "Customer";
        }

        //  CHECK LOGIN
        private bool IsAdminLoggedIn()
        {
            return HttpContext.Session.GetString("JWToken") != null;
        }

        // DASHBOARD
        public async Task<IActionResult> Dashboard()
        {
            if (!IsAdminLoggedIn())
                return RedirectToAction("AdminLogin", "Auth");

            var orders = await _repo.GetAllOrders();   
            if (orders == null)
                orders = new List<Order>();     
            ViewBag.TotalOrders = orders.Count();
            ViewBag.TotalCustomers = orders.Any()
            ? orders.Select(x => x.UserId).Distinct().Count() : 0;
           ViewBag.Pending = orders.Count(x => x.PaymentMethod == "Pending");
            ViewBag.Completed = orders.Count(x => x.PaymentMethod != "Pending");
            return View(orders);
        }

        
        public async Task<IActionResult> Orders()
        {
          
            if (!IsAdmin())
            {
                return RedirectToAction("AdminLogin", "Auth");
            }         
            var orders = await _repo.GetAllOrders();       
            if (orders == null)
            {
                orders = new List<Order>();
            }
            return View(orders);
        }
        public async Task<IActionResult> Edit(int id)
        {
            if (!IsAdmin())
                return RedirectToAction("AdminLogin", "Auth");

            var orders = await _repo.GetAllOrders();
            var order = orders.FirstOrDefault(x => x.Id == id);
            if (order == null)
                return NotFound();
            return View(order);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Order order)
        {
            if (!IsAdmin())
                return RedirectToAction("AdminLogin", "Auth");

            //  Get item price
            var items = await _repo.GetAllItems();
            var selectedItem = items.FirstOrDefault(x => x.ItemName == order.ItemName);
           if (selectedItem != null)
            {
                //  Recalculate Total
                order.TotalPrice =
                    selectedItem.Price * order.Quantity;
            }

            await _repo.UpdateOrder(order);
            return RedirectToAction("Orders");
        }
        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int Id,string Status)
        {
            if (!IsAdmin())
                return RedirectToAction("AdminLogin", "Auth");

            var orders =await _repo.GetAllOrders();

            var order =orders.FirstOrDefault(x => x.Id == Id);

            if (order == null)
                return NotFound();

            order.Status = Status;
            await _repo.UpdateOrder(order);
            return RedirectToAction("Orders");
        }
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            if (!IsAdmin())
                return RedirectToAction("AdminLogin", "Auth");

            await _repo.DeleteOrder(id);

            return RedirectToAction("Orders");
        }

      

        public async Task<IActionResult> DeleteItem(int id)
        {
            if (!IsAdmin())
                return RedirectToAction("AdminLogin", "Auth");

            await _repo.DeleteItem(id);

            return RedirectToAction("Items");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("AdminLogin", "Auth");
        }
        public async Task<IActionResult> Customers()
        {
            if (HttpContext.Session.GetString("JWToken") == null)
                return RedirectToAction("AdminLogin", "Auth");

            var users =
                await _repo.GetAllCustomers();

            // ✅ SHOW ONLY CUSTOMERS

            var customers =
                users
                .Where(x => x.role == "Customer")
                .ToList();

            return View(customers);
        }
        public async Task<IActionResult> Admins()
        {
            if (HttpContext.Session.GetString("JWToken") == null)
                return RedirectToAction("AdminLogin", "Auth");

            var users =
                await _repo.GetAllCustomers();

            // ✅ SHOW ONLY ADMINS

            var admins =
                users
                .Where(x => x.role == "Admin")
                .ToList();

            return View(admins);
        }


        public async Task<IActionResult> EditItem(int id)
        {
            if (!IsAdmin())
                return RedirectToAction("AdminLogin", "Auth");

            var item = await _repo.GetItemById(id);
            return View(item);
        }

     
        [HttpPost]
        public async Task<IActionResult> EditItem(Item item)
        {
            if (!IsAdmin())
                return RedirectToAction("AdminLogin", "Auth");

            await _repo.UpdateItem(item);

            return RedirectToAction("Items");
        }
        public IActionResult AddItem()
        {
            if (!IsAdmin())
                return RedirectToAction("AdminLogin", "Auth");

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddItem(Item item)
        {
            if (!IsAdmin())
                return RedirectToAction("AdminLogin", "Auth");

            await _repo.AddItem(item);

            return RedirectToAction("Items");
        }

        public async Task<IActionResult> Items()
        {
            if (!IsAdmin())
                return RedirectToAction("AdminLogin", "Auth");

            var items = await _repo.GetAllItems();

            if (items == null)
                items = new List<Item>();

            return View(items);
        }

    }
}
