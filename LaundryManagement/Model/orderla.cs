namespace LaundryManagement.Model
{
    public class orderla
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public string ItemName { get; set; }

        public int Quantity { get; set; }

        public decimal TotalPrice { get; set; }

        public string PaymentMethod { get; set; }

        public string Status { get; set; }
    }
}
