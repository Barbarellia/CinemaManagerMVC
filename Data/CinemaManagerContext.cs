using System;
using CinemaManager.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CinemaManager.Data
{
    public class CinemaManagerContext : IdentityDbContext
    {
        public CinemaManagerContext(DbContextOptions<CinemaManagerContext> options)
            : base(options)
        {
        }

        public DbSet<Show> Show { get; set; }
    }
}
