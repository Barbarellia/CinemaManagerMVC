using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using CinemaManager.Helpers;
using Microsoft.AspNetCore.Authentication;

namespace CinemaManager.Areas.Identity.Pages.Account.Manage
{
    public partial class EmailModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly int CipherKey;

        public EmailModel(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            CipherKey = _configuration.GetValue<int>("CipherKey");
        }

        public string Username { get; set; }

        public string Email { get; set; }

        public bool IsEmailConfirmed { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "New email")]
            public string NewEmail { get; set; }
        }

        private async Task LoadAsync(IdentityUser user)
        {
            var email = await _userManager.GetEmailAsync(user);
            Email = Ceasar.Decipher(email,CipherKey);

            Input = new InputModel
            {
                NewEmail = Ceasar.Decipher(email, CipherKey)
            };

            IsEmailConfirmed = await _userManager.IsEmailConfirmedAsync(user);
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task ChgangeEmailAndLogout(IdentityUser user, string code)
        {
            await _userManager.ChangeEmailAsync(user, Ceasar.Encipher(Input.NewEmail, CipherKey), code);
            await _userManager.SetUserNameAsync(user, Ceasar.Encipher(Input.NewEmail, CipherKey));
            await _signInManager.SignOutAsync();         
        }

        public async Task<IActionResult> OnPostChangeEmailAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            var email = await _userManager.GetEmailAsync(user);
            if (Input.NewEmail != Ceasar.Decipher(email, CipherKey))
            {
                var code = await _userManager.GenerateChangeEmailTokenAsync(user, Ceasar.Encipher(Input.NewEmail, CipherKey));

                await ChgangeEmailAndLogout(user, code);
                return RedirectToAction("Index", "Home");
            }

            StatusMessage = "Your email is unchanged.";
            return RedirectToPage();
        }
    }
}
