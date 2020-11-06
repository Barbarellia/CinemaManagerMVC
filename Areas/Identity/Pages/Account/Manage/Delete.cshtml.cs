using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CinemaManager.Data;
using CinemaManager.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CinemaManager.Areas.Identity.Pages.Account.Manage
{
    public class DeleteModel : PageModel
    {
        private readonly CinemaManagerContext _context;

        public DeleteModel(CinemaManagerContext context)
        {
            _context = context;
        }
        public Reservation Reservation { get; set; }

        // GET: Films/Delete/5
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Reservation = await _context.Reservations
                .Include(q=>q.Show)
                .ThenInclude(q=>q.Film)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (Reservation == null)
            {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Reservation = await _context.Reservations
                .Include(q => q.Show)
                .ThenInclude(q => q.Film)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (Reservation != null)
            {
                _context.Reservations.Remove(Reservation);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
