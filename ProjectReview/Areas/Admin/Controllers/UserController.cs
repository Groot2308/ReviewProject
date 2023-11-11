using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProjectReview.Models;
using ProjectReview.Repositories;
using ProjectReview.Validator;
using X.PagedList;

namespace ProjectReview.Areas.Admin.Controllers
{
    [Area("admin")]
    [Route("UserManager")]
    public class UserController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;

        public UserController(IUserRepository userRepository, IRoleRepository roleRepository)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
        }

        [Route("Users")]

        public IActionResult ListUser(int page = 1, int pageSize = 5, string searchKeyword = "")
        {
            var includes = new string[] { "Status", "Role" };

            int total;
            var users = _userRepository.GetMulti(
                user => string.IsNullOrEmpty(searchKeyword) || user.Name.Contains(searchKeyword) || user.Email.Contains(searchKeyword),
                includes: includes
            ).ToPagedList(TempData["page"] == null ? page : (int)TempData["page"], pageSize);


            ViewBag.SearchKeyword = searchKeyword; // Lưu từ khóa tìm kiếm vào ViewBag

            return View(users);
        }


        [Route("CreateUser")]
        [HttpGet]
        public IActionResult Create()
        {

            if (TempData["CreateSuccess"] != null)
            {
                ViewBag.MessSuccess = TempData["CreateSuccess"]?.ToString();
            }


            return View();
        }
        [Route("CreateUser")]
        [HttpPost]

        public IActionResult Create(User user)
        {
            var validator = new UserValidator();
            var result = validator.Validate(user);
            var checkEmail = _userRepository.GetSingleByCondition(us => us.Email.Equals(user.Email));
            if (checkEmail != null)
            {
                ViewBag.ErrorEmail = "Email has been exist!";
                return View(user);
            }
            if (result.IsValid)
            {
                user.CreateDate = DateTime.Today;
                user.StatusId = 1;
                _userRepository.Add(user);
                _userRepository.SaveChanges();
                TempData["CreateSuccess"] = "Create Successfully!";
                return RedirectToAction("Create");
            }

            return View(user);
        }
        [Route("ChangeStatus")]
        [HttpPost]
        public IActionResult ChangeStatus(int id, int page)
        {
            var user = _userRepository.GetSingleById(id);
            if (user != null)
            {
                user.StatusId = (user.StatusId == 1) ? 2 : 1;
                _userRepository.Update(user);
                _userRepository.SaveChanges();
            }
            TempData["Page"] = page;
            return RedirectToAction("ListUser");
        }




    }
}
