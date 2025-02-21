namespace EcoAppApi.DTOs
{
    public class SearchOrderDto
    {
        public int Id { get; set; }
        public string UserEmail { get; set; } = string.Empty;

        public string UserName { get; set; } = "UnKnown";

        public string ServiceType { get; set; } = "Unspecified";

        public string City { get; set; } = "Unknown";

        public string StatusTypeString { get; set; } = "Panding";

        public string ConsultancyTypeString { get; set; } = "General";
    }
}
