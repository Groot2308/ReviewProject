using Microsoft.AspNetCore.Mvc;

namespace ProjectReview.Controllers
{
    public class ProfileController : Controller
    {
        public IActionResult Profile()
        {
            return View();
        }
    }
}
