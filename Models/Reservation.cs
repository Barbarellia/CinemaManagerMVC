using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CinemaManager.Models
{
    public class Reservation
    {
        public int Id { get; set; }
        public Show Show { get; set; }
        public int? SeatRow { get; set; }
        public int? SeatColumn { get; set; }

        [Display(Name = "Reservation Date")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy HH:mm}", ApplyFormatInEditMode = false)]
        public DateTime ReservationDate { get; set; }

    }
}
