using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CinemaManager.Models
{
    public class Film
    {
        public int Id { get; set; }
        [StringLength(60, MinimumLength = 2)]
        [Required]
        public string Title { get; set; }
        public List<Show> Shows { get; set; }
    }
}
