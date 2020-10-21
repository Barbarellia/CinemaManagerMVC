using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using CinemaManager.Data;
using System;
using System.Linq;
using CinemaManager.Controllers;

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

                    context.SaveChanges();

                    context.Shows.AddRange(
                        new Show
                        {
                            ShowDate = DateTime.Parse("1989-2-12"),
                            Genre = "Romantic Comedy",
                            Price = 7.99M,
                            Film = context.Films.FirstOrDefault(q => q.Title == "Rio Bravo")
                        },

                        new Show
                        {
                            ShowDate = DateTime.Parse("1984-3-13"),
                            Genre = "Comedy",
                            Price = 8.99M,
                            Film = context.Films.FirstOrDefault(q => q.Title == "Pulp Fiction")
                        },

                        new Show
                        {
                            ShowDate = DateTime.Parse("1986-2-23"),
                            Genre = "Comedy",
                            Price = 9.99M,
                            Film = context.Films.FirstOrDefault(q => q.Title == "Ghostbusters")
                        }
                    );

                    context.SaveChanges();
                }
            }
        }
    }
}