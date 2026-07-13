namespace LaundryMVC.Models
{
    public class Order
    {

        public int Id { get; set; }

        public int UserId { get; set; }

        public string ItemName { get; set; }

        public int Quantity { get; set; }

        public decimal TotalPrice { get; set; }

        public string PaymentMethod { get; set; }
        public string Email { get; set; }
        // UI calculation only
        public decimal Price { get; set; }
        public string Status { get; set; }
    }
    }

