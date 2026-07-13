using LaundryMVC.Models;

namespace LaundryMVC.Repository.IRepo
{
    public interface ILauIRepo
    {
   
        Task<List<Item>> GetAllItems();
        Task<Item> GetItemById(int id);
        Task<bool> AddItem(Item item);
        Task UpdateItem(Item item);
        Task DeleteItem(int id);


        Task<bool> SaveOrder(Order order);
        Task DeleteOrder(int id);
        Task<List<Order>> GetAllOrders();
        Task UpdateOrder(Order order);
        Task<List<Order>> GetOrdersByUserId(int userId);

       
        Task<List<Register>> GetAllCustomers();
    }
}

