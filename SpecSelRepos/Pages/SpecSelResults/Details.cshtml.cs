using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SpecSelRepos.Models;

namespace SpecSelRepos.Pages.SpecSelResults
{
    public class DetailsModel : PageModel
    {
        private readonly SpecSelRepos.Models.SpecSelResultContext _context;

        public DetailsModel(SpecSelRepos.Models.SpecSelResultContext context)
        {
            _context = context;
        }

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
    }
}
