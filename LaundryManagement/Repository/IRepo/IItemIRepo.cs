using LaundryManagement.Model;

namespace LaundryManagement.Repository.IRepo
{
    public interface IItemIRepo
    {
        Task<IEnumerable<Itemla>> GetAllItems();

        Task<Itemla> GetItemById(int id);

        Task AddItem(Itemla item);

        Task UpdateItem(Itemla item);

        Task DeleteItem(int id);
    }
}
