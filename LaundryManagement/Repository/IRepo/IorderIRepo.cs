using LaundryManagement.Model;

namespace LaundryManagement.Repository.IRepo
{
    public interface IorderIRepo
    {
        // ✅ GET ALL
        Task<List<orderla>> GetAllOrders();

        // ✅ GET BY ID
        Task<orderla> GetById(int id);

        // ✅ ADD
        Task AddOrder(orderla order);

        // ✅ UPDATE
        Task UpdateOrder(orderla order);

        // ✅ DELETE
        Task DeleteOrder(int id);
        // ✅ ADD THIS
        Task<List<orderla>> GetOrdersByUserId(int userId);
    }
}
