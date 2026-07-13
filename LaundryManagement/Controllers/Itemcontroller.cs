using LaundryManagement.DTOs;
using LaundryManagement.Model;
using LaundryManagement.Repository.IRepo;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LaundryManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Itemcontroller : ControllerBase
    {
        private readonly IItemIRepo _item;
        public Itemcontroller(IItemIRepo item)
        {
            _item = item;
        }
        //  GET ALL ITEMS
        [HttpGet]
        public async Task<IActionResult> GetAllItems()
        {
            var items = await _item.GetAllItems();

            var result = items.Select(x => new ItemDTO
            {
                Id = x.Id,
                ItemName = x.Itemname,
                Price = x.price
            });

            return Ok(result);
        }

        // GET BY ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetItemById(int id)
        {
            var item = await _item.GetItemById(id);

            if (item == null)
                return NotFound();

            return Ok(new ItemDTO
            {
                Id = item.Id,
                ItemName = item.Itemname,
                Price = item.price
            });
        }

        // ADD
        [HttpPost]
        public async Task<IActionResult> AddItem(ItemDTO dto)
        {
            var item = new Itemla
            {
                Itemname = dto.ItemName,
                price = dto.Price
            };

            await _item.AddItem(item);

            return Ok("Item Added Successfully");
        }

        // UPDATE
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateItem(int id, ItemDTO dto)
        {
            var item = await _item.GetItemById(id);

            if (item == null)
                return NotFound();

            item.Itemname = dto.ItemName;
            item.price = dto.Price;

            await _item.UpdateItem(item);

            return Ok("Item Updated Successfully");
        }

        // DELETE
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteItem(int id)
        {
            await _item.DeleteItem(id);

            return Ok("Item Deleted Successfully");
        }
    }
}
