namespace DAL.Models
{
    public class Survey : Service
    {
        public required string Location {  get; set; }
        public required decimal AreaSize {  get; set; }
        public required string Survaypurpose { get; set; }
        public override string GetServiceDetails()
        {
            return $"{base.GetServiceDetails()}, Area Size: {AreaSize} square kilometers, Survaypurpose: {Survaypurpose} " ;
        }

    }
}
