using Microsoft.AspNetCore.Mvc;
using MimeKit;
using ProjectReview.Models;
using MailKit.Net.Smtp;
using ProjectReview.Repositories;

namespace ProjectReview.Controllers
{
    public class RegisterController : Controller
    {
        private readonly IUserRepository _userRepository;

        public RegisterController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public IActionResult Register()
        {
            ViewBag.ErrorMessage = TempData["ErrorMessage"];
            return View();
        }

        [HttpPost]
        public IActionResult SendEmail(User emailData)
        {

            User u = _userRepository.GetSingleByCondition(u => u.Email == emailData.Email);
            if (u != null && u.StatusId == 2)
            {
                var email = new MimeMessage();
                email.From.Add(MailboxAddress.Parse(emailData.From));
                email.To.Add(MailboxAddress.Parse(emailData.Email));
                email.Subject = "Vui lòng xác thực email của bạn";
                int number = GenerateRandom5DigitNumber();
                string emailBody = "Your single-use code is: " + number.ToString();
                email.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = emailBody };
                HttpContext.Session.SetInt32("RandomNumber", number);
                HttpContext.Session.SetString("Email", u.Email.ToString());
                //email.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = emailData.Body };
                using var smtp = new SmtpClient();
                //smtp.Connect("smtp.ethereal.email", 587, MailKit.Security.SecureSocketOptions.StartTls);
                smtp.Connect("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
                smtp.Authenticate(emailData.From, emailData.PasswordSendMail);
                smtp.Send(email);
                smtp.Disconnect(true);
                TempData["mod"] = 1;
                return RedirectToAction("VerifyRegister");
            }
            else
            {

                if (u != null && u.StatusId == 1)
                {
                    TempData["ErrorMessage"] = "Email đã tồn tại!!!";
                    return RedirectToAction("Register");
                }
                string confirmPassword = Request.Form["confirmPassword"];
                if (confirmPassword != emailData.Password)
                {
                    TempData["ErrorMessage"] = "Xác thực mật khẩu không chính xác";
                    return RedirectToAction("Register");
                }
                else
                {

                    var email = new MimeMessage();
                    email.From.Add(MailboxAddress.Parse(emailData.From));
                    email.To.Add(MailboxAddress.Parse(emailData.Email));
                    email.Subject = "Vui lòng xác thực email của bạn";
                    int number = GenerateRandom5DigitNumber();
                    string emailBody = "Your single-use code is: " + number.ToString();
                    email.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = emailBody };
                    HttpContext.Session.SetInt32("RandomNumber", number);

                    User newUser = new User();
                    newUser.Name = emailData.Name;
                    newUser.Email = emailData.Email;
                    newUser.Password = emailData.Password;
                    newUser.RoleId = 2;
                    newUser.StatusId = 2;
                    newUser.CreateDate = DateTime.Now;
                    _userRepository.Add(newUser);
                    _userRepository.SaveChanges();

                    HttpContext.Session.SetString("Email", newUser.Email.ToString());

                    //email.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = emailData.Body };
                    using var smtp = new SmtpClient();
                    //smtp.Connect("smtp.ethereal.email", 587, MailKit.Security.SecureSocketOptions.StartTls);
                    smtp.Connect("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
                    smtp.Authenticate(emailData.From, emailData.PasswordSendMail);
                    smtp.Send(email);
                    smtp.Disconnect(true);
                    TempData["mod"] = 1;
                    return RedirectToAction("VerifyRegister");
                }
            }

        }
        public IActionResult Send(User emailData)
        {
            string? mail = HttpContext.Session.GetString("Email");
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(emailData.From));
            email.To.Add(MailboxAddress.Parse(mail));
            email.Subject = "Vui lòng xác thực email của bạn";
            int number = GenerateRandom5DigitNumber();
            string emailBody = "Your single-use code is: " + number.ToString();
            email.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = emailBody };
            HttpContext.Session.SetInt32("RandomNumber", number);
            //HttpContext.Session.SetString("Email", u.Email.ToString());
            //email.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = emailData.Body };
            using var smtp = new SmtpClient();
            //smtp.Connect("smtp.ethereal.email", 587, MailKit.Security.SecureSocketOptions.StartTls);
            smtp.Connect("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
            smtp.Authenticate(emailData.From, emailData.PasswordSendMail);
            smtp.Send(email);
            smtp.Disconnect(true);
            TempData["mod"] = 1;
            return RedirectToAction("VerifyRegister");
        }


        private int GenerateRandom5DigitNumber()
        {
            Random rand = new Random();
            int min = 10000;
            int max = 99999;
            return rand.Next(min, max);
        }

        public IActionResult VerifyRegister()
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
                    string email = HttpContext.Session.GetString("Email");
                    User user = _userRepository.GetSingleByCondition(u => u.Email == email);
                    user.StatusId = 1;
                    _userRepository.Update(user);
                    _userRepository.SaveChanges();
                    return RedirectToAction("Login", "Login");
                }
            }
            return RedirectToAction("VerifyRegister");
        }


    }
}
