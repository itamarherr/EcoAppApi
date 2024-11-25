namespace DAL.Models
{
    public class Category

    {

        public int Id { get; set; }

        public string Name { get; set; }



        //Navigation Props:

        public ICollection<Service> Services { get; set; }

    }
}
