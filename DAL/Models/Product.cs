using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Models;

public class Product
{
    [Key]
    public int Id { get; set; } = 1;
    [Required]
    public string Name { get; set; } = "Oak Consultancy";
    public string? Description { get; set; }

    [Column (TypeName = "Money")]
    public decimal Price { get; set; }


    public bool Editing { get; set; }


    //Navigation props:

    public ICollection<Order> Orders { get; set; } = new List<Order> ();





}
