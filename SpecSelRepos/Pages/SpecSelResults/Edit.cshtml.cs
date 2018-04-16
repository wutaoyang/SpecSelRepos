using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SpecSelRepos.Controllers;
using SpecSelRepos.Models;

namespace SpecSelRepos.Pages.SpecSelResults
{
    [Authorize(Roles = AccountController.ADMIN)]
    public class EditModel : PageModel
    {
        private readonly SpecSelRepos.Models.SpecSelResultContext _context;

        public EditModel(SpecSelRepos.Models.SpecSelResultContext context)
        {
            _context = context;
        }

        [BindProperty]
        public SpecSelResult SpecSelResult { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            SpecSelResult = await _context.SpecSelResult.SingleOrDefaultAsync(m => m.ID == id);

            if (SpecSelResult == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(SpecSelResult).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SpecSelResultExists(SpecSelResult.ID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool SpecSelResultExists(int id)
        {
            return _context.SpecSelResult.Any(e => e.ID == id);
        }
    }
}
