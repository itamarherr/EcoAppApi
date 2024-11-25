﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Models
{
    public class Service
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public bool Editing { get; set; }


        [Column(TypeName = "Money")]
        public decimal Price { get; set; }
        public string? Description { get; set; } // General descripti

        //public string ImageUrl { get; set; }



        //Likes/Rating (when we add users/identity)





        //Navigation props:

        public int CategoryId { get; set; }

        public Category Category { get; set; }

        public virtual string GetServiceDetails()
        {
            return $"Service: {Name}, Price: {Price:C}";
        }

    }
}
