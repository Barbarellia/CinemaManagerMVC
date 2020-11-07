using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CinemaManager.Data;
using CinemaManager.Helpers;
using CinemaManager.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace CinemaManager.Controllers
{
    [Authorize(Roles = "Employee")]
    public class UsersController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly CinemaManagerContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly int CipherKey;

        public UsersController(
            UserManager<IdentityUser> userManager, 
            CinemaManagerContext context, 
            RoleManager<IdentityRole> roleManager, 
            IConfiguration configuration)
        {
            _userManager = userManager;
            _context = context;
            _roleManager = roleManager;
            _configuration = configuration;
            CipherKey = _configuration.GetValue<int>("CipherKey");
        }

        public async Task<IActionResult> Index()
        {
            var allUsers = _userManager.Users.ToList();
            var allUsersVM = new List<IdentityUserViewModel>();

            foreach (var user in allUsers)
            {
                var roles = await _userManager.GetRolesAsync(user);
                var role = roles.FirstOrDefault();
                IdentityUserViewModel identityUserVM = new IdentityUserViewModel
                {
                    Id = user.Id,
                    UserName = Ceasar.Decipher(user.UserName, CipherKey),
                    Role = role
                };
                allUsersVM.Add(identityUserVM);
            }
            return View(allUsersVM);

        }

        // GET: Shows/Edit/5
        public IActionResult Edit(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return NotFound();
            }
                      
            return View();
        }

        // POST: Shows/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,UserName,Role")] IdentityUserViewModel userVM)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return NotFound();
            }

            var newRole = userVM.Role;

            var role = await _roleManager.FindByNameAsync(newRole);

            if (role != null)
            {
                var user = await _userManager.FindByIdAsync(id);
                // wez role usera tego powyzej
                var prevRoleList = await _userManager.GetRolesAsync(user);
                string prevRole = prevRoleList.FirstOrDefault();
                // usun z tego usera stara role
                await _userManager.RemoveFromRoleAsync(user, prevRole);
                // mozwile ze tu bedzie trzeba savechanges ale jak nie to gituwa 
                // przypisz nowa role do usera
                await _userManager.AddToRolesAsync(user, new List<string> { role.Name });
                //update i elo
                //_context.Update(user);
                await _context.SaveChangesAsync();
            }

            //if (await _userManager.IsInRoleAsync(user, "Customer"))
            //{
            //    await _userManager.RemoveFromRoleAsync(user, "Customer");

            //    //await _context.saveChangesAsync()

            //    await _userManager.AddToRoleAsync(user, newRole);
            //}

            //_context.Update(user);
            //await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
            
            //return View(user);
        }

    }
}