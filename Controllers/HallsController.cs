using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CinemaManager.Data;
using CinemaManager.Models;
using Microsoft.AspNetCore.Authorization;

namespace CinemaManager.Controllers
{
    [Authorize(Roles = "Employee")]
    public class HallsController : Controller
    {
        private readonly CinemaManagerContext _context;

        public HallsController(CinemaManagerContext context)
        {
            _context = context;
        }

        // GET: Halls
        public async Task<IActionResult> Index()
        {
            return View(await _context.Halls.ToListAsync());
        }

        // GET: Halls/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hall = await _context.Halls
                .FirstOrDefaultAsync(m => m.Id == id);
            if (hall == null)
            {
                return NotFound();
            }

            return View(hall);
        }

        // GET: Halls/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Halls/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nr,Rows,Columns")] Hall hall)
        {
            if (ModelState.IsValid)
            {
                _context.Add(hall);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(hall);
        }

        // GET: Halls/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hall = await _context.Halls.FindAsync(id);
            if (hall == null)
            {
                return NotFound();
            }
            return View(hall);
        }

        // POST: Halls/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nr,Rows,Columns")] Hall hall)
        {
            if (id != hall.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(hall);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HallExists(hall.Id))
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
            return View(hall);
        }

        // GET: Halls/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hall = await _context.Halls
                .FirstOrDefaultAsync(m => m.Id == id);
            if (hall == null)
            {
                return NotFound();
            }

            return View(hall);
        }

        // POST: Halls/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var hall = await _context.Halls.FindAsync(id);
            _context.Halls.Remove(hall);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool HallExists(int id)
        {
            return _context.Halls.Any(e => e.Id == id);
        }
    }
}
