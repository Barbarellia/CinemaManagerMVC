using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CinemaManager.Data;
using CinemaManager.Models;
using Microsoft.VisualStudio.Web.CodeGeneration.Contracts.Messaging;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using System.Threading;
using Microsoft.AspNetCore.Authorization;

namespace CinemaManager.Controllers
{
    public class ShowsController : Controller
    {
        private readonly CinemaManagerContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly TimeSpan expTime = new TimeSpan(0, 0, 10);

        public ShowsController(CinemaManagerContext context, 
            UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Shows
        public async Task<IActionResult> Index()
        {
            return View(await _context.Shows
                .Include(q => q.Film)
                .Include(p=>p.Hall)
                .ToListAsync());
        }

        // GET: Shows/Details/5
        [Authorize]
        public async Task<IActionResult> Details(int? id, int? row, int? column, int? confirmId)
        {
            if(confirmId != null)
            {
                await ConfirmReservation(confirmId);
                return RedirectToAction(nameof(Index));
            }

            #region ExtractConfirmed

            var allShowReservations = await _context.Reservations
                .Include(x => x.Show)
                .Where(x => x.Show.Id == id)
                .ToListAsync();

            var notConfirmed = allShowReservations.Where(x => !x.IsConfirmed).ToList();
            var confirmed = allShowReservations.Where(x => x.IsConfirmed).ToList();

            var expired = notConfirmed.Where(q => DateTime.Now - q.ClickDate > expTime).ToList();

            if (expired.Any())
            {
                foreach (var r in expired)
                {
                    _context.Reservations.Remove(r);
                }
                await _context.SaveChangesAsync();
            }

            //notConfirmed = notConfirmed - expired
            notConfirmed = _context.Reservations
                .Include(x => x.Show)
                .Where(x => x.Show.Id == id && !x.IsConfirmed)
                .ToList();

            TempData["notConfirmed"] = notConfirmed;
            TempData["confirmed"] = confirmed;
            TempData["reservationId"] = 0;
            TempData["isClicked"] = false;

            #endregion

            if (id == null)
            {
                return NotFound();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var showToUpdate = await _context.Shows
                .Include(q=>q.Film)
                .Include(p => p.Hall)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (showToUpdate == null)
            {
                return NotFound();
            }

            if (row != null && column != null)
            {
                var resOnSeat = await _context.Reservations.FirstOrDefaultAsync(x => x.IsConfirmed == false && x.Show.Id == id && x.SeatRow == row && x.SeatColumn == column);

                if (resOnSeat != null)
                {
                    ModelState.AddModelError(string.Empty, "This seat is temporary booked. Book another seat or check its availability later.");
                }

                else
                {
                    var prevRes = await _context.Reservations.FirstOrDefaultAsync(x => x.UserId == userId && x.Show.Id == id && x.IsConfirmed == false);

                    if (prevRes != null)
                    {
                        if (await TryUpdateModelAsync<Reservation>(
                            prevRes,
                            "",
                            s => s.SeatRow, s => s.SeatColumn))
                        {
                            prevRes.SeatRow = row;
                            prevRes.SeatColumn = column;
                            prevRes.ClickDate = DateTime.Now;
                        }
                    }
                    else
                    {
                        Reservation res = new Reservation
                        {
                            Show = showToUpdate,
                            SeatRow = row,
                            SeatColumn = column,
                            UserId = userId,
                            ClickDate = DateTime.Now,
                            IsConfirmed = false
                        };

                        _context.Reservations.Add(res);
                    }
                    await _context.SaveChangesAsync();

                    TempData["row"] = row;
                    TempData["column"] = column;

                    var selected = _context.Reservations.FirstOrDefault
                    (
                        x => x.IsConfirmed == false &&
                        x.UserId == userId &&
                        x.SeatRow == row &&
                        x.SeatColumn == column &&
                        x.Show.Id == id
                    );
                    if (selected != null)
                    {
                        TempData["reservationId"] = selected.Id;
                    }

                    ModelState.AddModelError(string.Empty, "Confirm your reservation, otherwise it will be removed in:");
                    TempData["isClicked"] = true;
                }
                
            }
            return View(showToUpdate);
        }

        public async Task<IActionResult> ConfirmReservation(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reservation = await _context.Reservations.FirstOrDefaultAsync(x => x.Id == id);

            reservation.ConfirmationDate = DateTime.Now;
            reservation.IsConfirmed = true;

            _context.Update(reservation);

            await _context.SaveChangesAsync();
            return null;
        }

        // GET: Shows/Create
        public IActionResult Create()
        {
            return View();
        }

    
        // POST: Shows/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Genre,Price,ShowDate")] Show show, string filmTitle, int hallNr)
        {
            if (show == null)
            {
                return NotFound();
            }

            var film = _context.Films.FirstOrDefault(x => x.Title == filmTitle);
            var hall = _context.Halls.FirstOrDefault(q => q.Nr == hallNr);

            if(film == null)
            {
                //jesli nie ma takiego filmu, to zaznacz okienko na czerwono
                ModelState.AddModelError(string.Empty, "Film o podanym tytule nie istnieje w bazie");
            }
            else
            {
                show.Film = film;
            }

            if (hall == null)
            {
                ModelState.AddModelError(string.Empty, "Sala o podanym numerze nie istnieje w bazie");
                //return View();
            }
            else
            {
                show.Hall = hall;
            }


            if (ModelState.IsValid)
            {               
                _context.Add(show);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(show);
        }

        // GET: Shows/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var show = await _context.Shows.FindAsync(id);
            if (show == null)
            {
                return NotFound();
            }
            return View(show);
        }

        // POST: Shows/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Genre,Price,ShowDate")] Show show)
        {
            if (id != show.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(show);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ShowExists(show.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(show);
        }

        // GET: Shows/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var show = await _context.Shows
                .FirstOrDefaultAsync(m => m.Id == id);
            if (show == null)
            {
                return NotFound();
            }

            return View(show);
        }

        // POST: Shows/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var show = await _context.Shows.FindAsync(id);
            _context.Shows.Remove(show);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ShowExists(int id)
        {
            return _context.Shows.Any(e => e.Id == id);
        }
    }
}
