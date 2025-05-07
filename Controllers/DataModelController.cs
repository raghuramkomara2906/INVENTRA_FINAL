using Microsoft.AspNetCore.Mvc;

namespace ADAProject.Controllers
{
    public class DataModelController : Controller
    {
        // GET /DataModel
        public IActionResult Index()
        {
            return View(); // will look for Views/DataModel/Index.cshtml
        }
    }
}
