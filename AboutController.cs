using Microsoft.AspNetCore.Mvc;

namespace ADAProject.Controllers
{
    public class AboutController : Controller
    {
        // GET /About or /About/Index
        public IActionResult Index()  
        {
            return View();    
        }
    }
}
