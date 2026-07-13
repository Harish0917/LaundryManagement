using LaundryManagement.DTOs;
using LaundryManagement.Model;
using LaundryManagement.Repository.IRepo;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LaundryManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Ordercontroller : ControllerBase
    {
        public readonly IorderIRepo _ord;
        public Ordercontroller(IorderIRepo ord)
        {
            _ord=ord;
        }

        [HttpGet]
        public async Task<IActionResult> GetOrders()
        {
            var data =await _ord.GetAllOrders();
            return Ok(data);
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderById(int id)
        {
            var order =await _ord.GetById(id);
            if (order == null)
            return NotFound();
            return Ok(order);
        }
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetOrdersByUserId(int userId)
        {
            var orders =await _ord.GetAllOrders();

            var userOrders =orders.Where(o => o.UserId == userId).ToList();
            if (userOrders == null || !userOrders.Any())
            return NotFound();
            return Ok(userOrders);
        }


        [HttpPost]
        public async Task<IActionResult> AddOrder(OrderDTO dto)
        {
            var order = new orderla
            {
                UserId = dto.UserId,
                ItemName = dto.ItemName,
                Quantity = dto.Quantity,
                TotalPrice = dto.TotalPrice,
                PaymentMethod = dto.PaymentMethod,
                Status = "Pending"
            };

            await _ord.AddOrder(order);
            return Ok("Order Added");
        }


        [HttpPut("UpdateStatus/{id}")]
        public async Task<IActionResult> UpdateStatus(int id,[FromBody] string status)
        {
            var order =await _ord.GetById(id);
            if (order == null)
                return NotFound();           
            order.Status = status;
            await _ord.UpdateOrder(order);
            return Ok("Status Updated");
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrder(int id,OrderDTO dto)
        {
            if (id != dto.Id)
            return BadRequest();

            var order =await _ord.GetById(id);

            if (order == null)
            return NotFound();            
            order.ItemName = dto.ItemName;
            order.Quantity = dto.Quantity;
            order.TotalPrice = dto.TotalPrice;
            order.PaymentMethod = dto.PaymentMethod;
            order.Status = dto.Status;
            await _ord.UpdateOrder(order);
            return Ok("Updated Successfully");
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            await _ord.DeleteOrder(id);
            return Ok("Deleted");
        }
    }
}
