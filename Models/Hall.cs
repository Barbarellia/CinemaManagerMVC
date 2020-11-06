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
        [Display(Name = "Number of rows")]
        public int Rows { get; set; }
        [Display(Name = "Number of seats in a row")]
        public int Columns { get; set; }
    }
}
