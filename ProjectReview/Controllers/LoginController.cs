using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ProjectReview.Models;
using ProjectReview.Repositories;
using System.Security.Claims;

namespace ProjectReview.Controllers
{
    public class LoginController : Controller
    {
        private readonly IUserRepository _userRepository;

        public LoginController(IUserRepository userRepository)
        {
            this._userRepository = userRepository;
        }

        public ActionResult Login()
        {
            ViewBag.ErrorMessage = TempData["ErrorMessage"];
            return View();
        }

        [HttpPost]
        public ActionResult Login(string email, string password)
        {

            User user = _userRepository.GetSingleByCondition(u => u.Email == email && u.Password == password);
            if (user != null)
            {
                if (user.StatusId == 2)
                {
                    HttpContext.Session.SetString("Email", user.Email.ToString());
                    return RedirectToAction("Send", "Register");
                }
                else
                {

                    string userJson = JsonConvert.SerializeObject(user);
                    HttpContext.Session.SetString("User", userJson);

                    /*Đây là câu lệnh khi lấy user từ session xuống*/
                    /*string? accJson = HttpContext.Session.GetString("User"); lấy về 
                    User? acc = JsonConvert.DeserializeObject<User>(accJson); đổi kiểu cho nó*/

                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Invalid email or password.";
                return RedirectToAction("Login");
            }
        }

        public IActionResult ExternalLogin(string provider, string returnUrl = null)
        {
            var redirectUrl = Url.Action("ExternalLoginCallback", "Login", new { ReturnUrl = returnUrl });
            var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
            return new ChallengeResult(provider, properties);

            // Chuyển hướng người dùng đến quá trình xác thực Google
            return Challenge(properties, provider);
        }

        public  IActionResult ExternalLoginCallback()
        {

            return Content("Logged in with Google");
        }

    }
}
