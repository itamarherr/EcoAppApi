namespace DAL.Models
{
    public class Consultancy : Service
    {
        public required string ConsultancyExperise {  get; set; }
        public required decimal Duration {  get; set; }
        public override string GetServiceDetails()
        {
            return $"{base.GetServiceDetails()} Consultancy Experise: {ConsultancyExperise} , Duration: {Duration} Houres";
        }
    }
}
