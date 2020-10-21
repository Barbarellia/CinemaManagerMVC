using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;

namespace CinemaManager.Models
{
    public class FilmTitleViewModel
    {
        public Film Film { get; set; }
        public SelectList Titles { get; set; }
        public Show Show { get; set; }
        public string FilmTitle { get; set; }
        //public string SearchString { get; set; }
    }
}
