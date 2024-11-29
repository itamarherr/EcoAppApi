namespace DAL.Models
{
    public class Order
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public AppUser User { get; set; }
        public int ServiceId { get; set; }
        public Service Service { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.Now;
        public string Status { get; set; } = "Pending";
        public decimal TotalPrice { get; set; }
        public string? additionalNotes { get; set; }
        public DateTime LastUpdate { get; set; } = DateTime.Now;
    }
}
