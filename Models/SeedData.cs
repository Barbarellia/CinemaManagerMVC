using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using CinemaManager.Data;
using System;
using System.Linq;
using CinemaManager.Controllers;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace CinemaManager.Models
{
    public static class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new CinemaManagerContext(
                serviceProvider.GetRequiredService<
                    DbContextOptions<CinemaManagerContext>>()))
            {
                if (!context.Films.Any())
                {
                    context.Films.AddRange(
                        new Film
                        {
                            Title = "Pulp Fiction"
                        },

                        new Film
                        {
                            Title = "Rio Bravo"
                        },

                        new Film
                        {
                            Title = "Ghostbusters"
                        }
                    );

                    context.Halls.AddRange(
                        new Hall
                        {
                            Nr=1,
                            Rows=3,
                            Columns=3
                        },
                        new Hall
                        {
                            Nr=2,
                            Rows=5,
                            Columns=5
                        }
                    );

                    context.SaveChanges();

                    context.Shows.AddRange(
                        new Show
                        {
                            ShowDate = DateTime.Parse("1989-2-12"),
                            Genre = "Romantic Comedy",
                            Price = 7.99M,
                            Film = context.Films.FirstOrDefault(q => q.Title == "Rio Bravo"),
                            Hall = context.Halls.FirstOrDefault(h=> h.Nr == 1)
                        },

                        new Show
                        {
                            ShowDate = DateTime.Parse("1984-3-13"),
                            Genre = "Comedy",
                            Price = 8.99M,
                            Film = context.Films.FirstOrDefault(q => q.Title == "Pulp Fiction"),
                            Hall = context.Halls.FirstOrDefault(h => h.Nr == 1)
                        },

                        new Show
                        {
                            ShowDate = DateTime.Parse("1986-2-23"),
                            Genre = "Comedy",
                            Price = 9.99M,
                            Film = context.Films.FirstOrDefault(q => q.Title == "Ghostbusters"),
                            Hall = context.Halls.FirstOrDefault(h => h.Nr == 1)
                        }
                    );

                    context.SaveChanges();
                }
            }
        }
    }
}