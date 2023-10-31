using Microsoft.AspNetCore.Mvc;
using MimeKit;
using ProjectReview.Models;
using ProjectReview.Repositories;
using MailKit.Net.Smtp;

namespace ProjectReview.Controllers
{
    public class ResetController : Controller
    {
        private readonly IUserRepository _userRepository;

        public ResetController(IUserRepository userRepository)
        {
            this._userRepository = userRepository;
        }

        public IActionResult Reset(string mod)
        {
            if (mod != null)
            {
                return View();
            }
            else
            {
                ViewBag.ErrorMessage = TempData["ErrorMessage"];
                return View();
            }
        }

        [HttpPost]
        public IActionResult ResetPassword(string verifyEmail, User emailData)
        {
            User user = _userRepository.GetSingleByCondition(u => u.Email == verifyEmail);
            if (user == null)
            {
                TempData["ErrorMessage"] = "Email invalid !!!";
                return RedirectToAction("Reset");
            }
            else
            {             
                var email = new MimeMessage();
                email.From.Add(MailboxAddress.Parse(emailData.From));
                email.To.Add(MailboxAddress.Parse(verifyEmail));
                email.Subject = "Reset Password";
                int number = GenerateRandom5DigitNumber();
                string emailBody = "Your single-use code is: " + number.ToString();
                email.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = emailBody };
                HttpContext.Session.SetInt32("RandomNumber", number);
                HttpContext.Session.SetString("Email",verifyEmail);
                using var smtp = new SmtpClient();
                smtp.Connect("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
                smtp.Authenticate(emailData.From, emailData.PasswordSendMail);
                smtp.Send(email);
                smtp.Disconnect(true);
                TempData["mod"] = 1;
                return RedirectToAction("VerifyReset");
            }
            
        }
        public IActionResult VerifyReset()
        {
            var modValue = TempData["mod"];
            if (modValue != null)
            {
                return View();
            }
            else
            {
                ViewBag.ErrorMessage = "Verify email incorrect! Please input again.";
                return View();
            }
        }

        public IActionResult Verify(string verify)
        {

            int? storedRandomNumber = HttpContext.Session.GetInt32("RandomNumber");

            if (storedRandomNumber.HasValue && !string.IsNullOrEmpty(verify))
            {
                int userEnteredNumber;

                if (int.TryParse(verify, out userEnteredNumber) && userEnteredNumber == storedRandomNumber.Value)
                {
                    TempData["mod"] = 1;
                    return RedirectToAction("changepassword");
                }
            }
            return RedirectToAction("VerifyRegister");
        }

        public IActionResult changepassword()
        {
            var modValue = TempData["mod"];
            if (modValue != null)
            {
                return View();
            }
            else
            {
                ViewBag.ErrorMessage = "Confirm Password Incorrect!!";
                return View();
            }
        }

        [HttpPost]
        public IActionResult changepassword(string newpass, string confirmpass)
        {
            if(newpass != confirmpass)
            {
                return RedirectToAction("changepassword");
            }
            else
            {
                string? mail = HttpContext.Session.GetString("Email");
                User user = _userRepository.GetSingleByCondition(u => u.Email == mail);
                user.Password = newpass;
                _userRepository.Update(user);
                _userRepository.SaveChanges();
                return RedirectToAction("Login", "Login");
            }      
        }

        private int GenerateRandom5DigitNumber()
        {
            Random rand = new Random();
            int min = 10000;
            int max = 99999;
            return rand.Next(min, max);
        }
    }
}
