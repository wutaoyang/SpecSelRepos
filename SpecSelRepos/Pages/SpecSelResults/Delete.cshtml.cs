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
    public class DeleteModel : PageModel
    {
        private readonly SpecSelRepos.Models.SpecSelResultContext _context;

        public DeleteModel(SpecSelRepos.Models.SpecSelResultContext context)
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

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            SpecSelResult = await _context.SpecSelResult.FindAsync(id);

            if (SpecSelResult != null)
            {
                _context.SpecSelResult.Remove(SpecSelResult);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
