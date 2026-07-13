using iText.Kernel.Counter.Context;
using LaundryManagement.Model;
using LaundryManagement.Model.data;
using LaundryManagement.Repository.IRepo;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LaundryManagement.Repository.Repo
{
    public class OrderRepo : IorderIRepo
    {
        private readonly LaundryDb _ct;

        public OrderRepo(LaundryDb ct)
        {
            _ct = ct;
        }

        //  GET ALL ORDERS
        public async Task<List<orderla>> GetAllOrders()
        {
            return await _ct.Orders.ToListAsync();
        }

        //  GET ORDER BY ID
        public async Task<orderla> GetById(int id)
        {
            return await _ct.Orders.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<List<orderla>> GetOrdersByUserId(int userId)
        {
            return await _ct.Orders.Where(o => o.UserId == userId).ToListAsync();
        }

        //  ADD ORDER
        public async Task AddOrder(orderla order)
        {
            await _ct.Orders.AddAsync(order);
            await _ct.SaveChangesAsync();
        }

        //  UPDATE ORDER
        public async Task UpdateOrder(orderla order)
        {
            _ct.Orders.Update(order);
            await _ct.SaveChangesAsync();
        }

        //  DELETE ORDER
        public async Task DeleteOrder(int id)
        {
            var data =await _ct.Orders.FindAsync(id);
            if (data != null)
            {
                _ct.Orders.Remove(data);
                await _ct.SaveChangesAsync();
            }
        }
    }
}