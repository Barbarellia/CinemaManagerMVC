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

namespace CinemaManager.Controllers
{
    public class ShowsController : Controller
    {
        private readonly CinemaManagerContext _context;

        public ShowsController(CinemaManagerContext context)
        {
            _context = context;
        }

        // GET: Shows
        public async Task<IActionResult> Index()
        {
            return View(await _context.Shows
                .Include(q => q.Film)
                .ToListAsync());
        }

        // GET: Shows/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var show = await _context.Shows
                .Include(q=>q.Film)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (show == null)
            {
                return NotFound();
            }

            return View(show);
        }

        //public SelectList FilmsSL { get; set; }

        //public void PopulateFilmsDropDownList(CinemaManagerContext _context,
        //    object selectedFilm = null)
        //{
        //    var filmsQuery = from d in _context.Films
        //                     orderby d.Title // Sort by name.
        //                     select d;

        //    FilmsSL = new SelectList(filmsQuery.AsNoTracking(),
        //                "Id", "Title", selectedFilm);
        //}

        // GET: Shows/Create
        public async Task<IActionResult> Create()
        {
            IQueryable<string> titleQuery = from f in _context.Films
                                            orderby f.Title
                                            select f.Title;
            //PopulateFilmsDropDownList(_context);
            //List<Film> allFilms = await _context.Films.ToListAsync();
            
            var filmTitleVM = new FilmTitleViewModel
            {
                //Film = await _context.Films.ToListAsync(),
                Titles = new SelectList(await titleQuery.Distinct().ToListAsync())
            };
            return View(filmTitleVM);
        }

    
        // POST: Shows/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Genre,Price,ShowDate")] Show show, string filmTitle)
        {
            var film = _context.Films.FirstOrDefault(x => x.Title == filmTitle);

            if(film==null)
            {
                //jesli nie ma takiego filmu, to zaznacz okienko na czerwono
                return RedirectToAction(nameof(Create));
            }

            if (film != null)
            {
                show.Film = film;
            }

            if (show == null)
            {
                return NotFound();
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
