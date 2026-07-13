using LaundryManagement.Model;
using LaundryManagement.Model.data;
using LaundryManagement.Repository.IRepo;
using Microsoft.EntityFrameworkCore;

namespace LaundryManagement.Repository.Repo
{
    public class ItemRepo : IItemIRepo
    {
        private readonly LaundryDb _ctox;
        public ItemRepo( LaundryDb ctox)
        {
            _ctox=ctox;
        }
        // GET ALL
        public async Task<IEnumerable<Itemla>> GetAllItems()
        {
            return await _ctox.LaundryItem.ToListAsync();
        }

        // GET BY ID
        public async Task<Itemla> GetItemById(int id)
        {
            return await _ctox.LaundryItem.FindAsync(id);
        }

        // ADD
        public async Task AddItem(Itemla item)
        {
            await _ctox.LaundryItem.AddAsync(item);
            await _ctox.SaveChangesAsync();
        }

        // UPDATE
        public async Task UpdateItem(Itemla item)
        {
            _ctox.LaundryItem.Update(item);
            await _ctox.SaveChangesAsync();
        }

        // DELETE
        public async Task DeleteItem(int id)
        {
            var data = await _ctox.LaundryItem.FindAsync(id);

            if (data != null)
            {
                _ctox.LaundryItem.Remove(data);
                await _ctox.SaveChangesAsync();
            }
        }
    }
}
