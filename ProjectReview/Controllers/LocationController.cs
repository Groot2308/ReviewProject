using Microsoft.AspNetCore.Mvc;
using ProjectReview.Models;
using ProjectReview.Repositories;

namespace ProjectReview.Controllers
{
    public class LocationController : Controller
    {

        private readonly ILocationRepository _locationRepository;
        public LocationController(ILocationRepository locationRepository)
        {

            _locationRepository = locationRepository;
        }

        public IActionResult Location()
        {
            return View();
        }

        public IActionResult LocationDetail(int id)
        {
            Location location = _locationRepository.GetSingleByCondition(l => l.Id == id);
            return View(location); 

        }
    }
}
