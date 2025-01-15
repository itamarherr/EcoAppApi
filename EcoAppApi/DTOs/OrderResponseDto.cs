namespace EcoAppApi.DTOs
{
    public class OrderResponseDto
    {
        public bool IsPrivateArea { get; set; }
        public string IsPrivateAreaString
        {
            get => IsPrivateArea ? "Yes" : "No";
        }
    }
}
