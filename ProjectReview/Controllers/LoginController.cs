using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
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

        public ActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
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
        }

        public async Task<IActionResult> ExternalLoginCallbackAsync()
        {
            var authenticateResult = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);

            if (!authenticateResult.Succeeded)
            {
                return BadRequest();
            }
            var userEmail = authenticateResult.Principal.FindFirst(ClaimTypes.Email)?.Value;
            var userName = authenticateResult.Principal.FindFirst(ClaimTypes.Name)?.Value;


            User users = _userRepository.GetSingleByCondition(x => x.Email == userEmail);
            if (users == null)
            {

                var user = new User { Email = userEmail, Name = userName, CreateDate = DateTime.Now, StatusId = 1, RoleId = 2 };
                _userRepository.Add(user);
                _userRepository.SaveChanges();
                string userJson = JsonConvert.SerializeObject(user);
                HttpContext.Session.SetString("User", userJson);
                return RedirectToAction("Index", "Home");
            }
            else
            {
                string userJson = JsonConvert.SerializeObject(users);
                HttpContext.Session.SetString("User", userJson);
                return RedirectToAction("Index", "Home");
            }
        }
    }
}
