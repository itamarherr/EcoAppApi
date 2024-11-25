namespace DAL.Models
{
    public class CheckListItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsSelected { get; set; }
        public int SurveyId { get; set; }
        public Survey Survey { get; set; }

    }
}