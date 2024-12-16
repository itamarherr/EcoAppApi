namespace EcoAppApi.DTOs
{
    public class UpdateOrderDto
    {
        public string Status { get; set; }
        public decimal? TotalPrice { get; set; }
        public string? AdditionalNotes { get; set; }
        public DateTime? DateForConsultancy { get; set; }
    }
}
