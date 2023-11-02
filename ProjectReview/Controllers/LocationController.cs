using Microsoft.AspNetCore.Mvc;

namespace ProjectReview.Controllers
{
    public class LocationController : Controller
    {
        public IActionResult Location()
        {
            return View();
        }
    }
}
