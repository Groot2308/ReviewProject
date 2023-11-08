using Microsoft.AspNetCore.Mvc;

namespace ProjectReview.Areas.Admin.Controllers
{
    [Area("admin")]
    [Route("admin")]
    public class AdminController : Controller
    {
        [Route("Dashboard")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
