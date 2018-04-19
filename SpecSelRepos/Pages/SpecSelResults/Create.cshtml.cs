using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SpecSelRepos.Controllers;
using SpecSelRepos.Models;

namespace SpecSelRepos.Pages.SpecSelResults
{
    [Authorize(Roles = AccountController.USER)]

    public class CreateModel : PageModel
    {
        private readonly SpecSelRepos.Models.SpecSelResultContext _context;

        public CreateModel(SpecSelRepos.Models.SpecSelResultContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public SpecSelResult SpecSelResult { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.SpecSelResult.Add(SpecSelResult);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}