namespace DAL.Models
{
    public class ForrestSurvey : Survey
    {
        public required string ForestType {  get; set; }
        public required bool isMeasurementMap {  get; set; }
        public ICollection<CheckListItem> CheckListItems { get; set; } = new List<CheckListItem>();
    };

   
}
