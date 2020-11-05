using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CinemaManager.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CinemaManager.Controllers
{
    public class UsersController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly CinemaManagerContext _context;

        public UsersController(UserManager<IdentityUser> userManager, CinemaManagerContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public IActionResult Index()
        {
            var allUsers = _userManager.Users.ToList();
            return View(allUsers);

        }

        // GET: Users/Create
        public IActionResult Create()
        {
            return View();
        }

        //POST: Users/Create/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,UserName")] IdentityUser user)
        {
            if (user == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                //await _userManager.AddToRoleAsync(user, role);
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(user);
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
        public async Task<IActionResult> Edit(int id, [Bind("Id,UserName")] IdentityUser user)
        {           
            if (ModelState.IsValid)
            {
                _context.Update(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

    }
}