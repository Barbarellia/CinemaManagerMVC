using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CinemaManager.Models
{
    public class Hall
    {
        public List<Show> Shows { get; set; }
        public int Id { get; set; }
        [Required]
        [Display(Name = "Hall number")]
        public int Nr { get; set; }
        [Required]
        [Display(Name = "Number of seats")]
        public int Rows { get; set; }
        public int Columns { get; set; }
    }
}
