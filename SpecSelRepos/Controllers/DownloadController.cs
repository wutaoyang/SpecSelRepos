using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SpecSelRepos.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    public class DownloadController : Controller
    {
        
        public FileResult SpecSel()
        {
            var fileName = $"SpeciesSelection.jar";
            var filepath = $"Download/{fileName}";
            byte[] fileBytes = System.IO.File.ReadAllBytes(filepath);
            return File(fileBytes, "application/jar", fileName);
        }



        public FileResult ReadMe()
        {
            var fileName = $"ReadMe_2.1.txt";
            var filepath = $"Download/{fileName}";
            byte[] fileBytes = System.IO.File.ReadAllBytes(filepath);
            return File(fileBytes, "application/txt", fileName);
        }
    }
}