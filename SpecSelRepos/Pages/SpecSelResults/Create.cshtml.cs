using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using SpecSelRepos.Models;

namespace SpecSelRepos.Pages.SpecSelResults
{
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