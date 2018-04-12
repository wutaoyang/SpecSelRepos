using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SpecSelRepos.Models;

namespace SpecSelRepos.Pages.SpecSelResults
{
    public class IndexModel : PageModel
    {
        private readonly SpecSelRepos.Models.SpecSelResultContext _context;

        public IndexModel(SpecSelRepos.Models.SpecSelResultContext context)
        {
            _context = context;
        }

        public IList<SpecSelResult> SpecSelResult { get; set; }
        public SelectList Options;//contains the list of options. This allows the user to select an option from the list.
        public string SpecSelResultOption { get; set; }//contains the specific option the user selects

        /// <summary>
        /// Add search capability to the index page
        /// </summary>
        /// <param name="searchString"></param>
        /// <returns></returns>
        public async Task OnGetAsync(string specSelResultOption, string searchString)
        {
            // Use LINQ to get list of options from database
            IQueryable<string> optionQuery = from m in _context.SpecSelResult
                                             orderby m.Option
                                             select m.Option;

            var specSelResults = from m in _context.SpecSelResult
                                 select m;

            if (!String.IsNullOrEmpty(searchString))
            {
                specSelResults = specSelResults.Where(s => s.DataSet.Contains(searchString));
            }

            if (!String.IsNullOrEmpty(specSelResultOption))
            {
                specSelResults = specSelResults.Where(x => x.Option == specSelResultOption);
            }
            Options = new SelectList(await optionQuery.Distinct().ToListAsync());
            SpecSelResult = await specSelResults.ToListAsync();
        }
    }
}
