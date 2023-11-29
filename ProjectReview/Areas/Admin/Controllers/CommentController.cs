using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ProjectReview.Repositories;
using X.PagedList;

namespace ProjectReview.Areas.Admin.Controllers
{
    [Area("admin")]
    [Route("CommentManager")]
    public class CommentController : Controller
    {
        private readonly ICommentRepository _commentRepository;
        private readonly IReplyRepository _replyRepository;
        private readonly IUserRepository _userRepository;

        public CommentController(ICommentRepository commentRepository, IReplyRepository replyRepository, IUserRepository userRepository)
        {
            _commentRepository = commentRepository;
            _replyRepository = replyRepository;
            _userRepository = userRepository;
        }

        [Route("ListReport")]
        [HttpGet]
        public IActionResult ListReportOfComment(int page = 1, int pageSize = 5, string searchKeyword = "")
        {
            var includes = new string[] { "Location", "Status", "User" };

            int total;
            var cmt = _commentRepository.GetMulti(
                cmt => cmt.StatusId == 3 && (string.IsNullOrEmpty(searchKeyword) && cmt.Content.Contains(searchKeyword) || cmt.User.Email.Contains(searchKeyword)),
                includes: includes
            )
            .OrderByDescending(cmt => cmt.Id) // Order by Id in descending order
            .ToPagedList(TempData["pagecmt"] == null ? page : (int)TempData["pagecmt"], pageSize);

            ViewBag.SearchKeyword = searchKeyword; // Lưu từ khóa tìm kiếm vào ViewBag

            return View(cmt);
        }
        [Route("ListReplyReport")]
        [HttpGet]
        public IActionResult ListReplyReport(int page = 1, int pageSize = 5, string searchKeyword = "")
        {
            var includes = new string[] { "Comment", "Status", "User" };

            int total;
            var reply = _replyRepository.GetMulti(
                reply => reply.StatusId == 3 && (string.IsNullOrEmpty(searchKeyword) && reply.Content.Contains(searchKeyword) || reply.User.Email.Contains(searchKeyword)),
                includes: includes
            )
            .OrderByDescending(cmt => cmt.Id) // Order by Id in descending order
            .ToPagedList(TempData["pagereply"] == null ? page : (int)TempData["pagereply"], pageSize);

            ViewBag.SearchKeyword = searchKeyword; // Lưu từ khóa tìm kiếm vào ViewBag

            return View(reply);
        }

        [Route("ChangeStatuss")]
        [HttpPost]
        public IActionResult ChangeStatus(int id, string check, string work, int page)
        {

            if (check.Equals("cmt"))
            {
                var cmt = _commentRepository.GetSingleById(id);
                if (work.Equals("active"))
                {
                    cmt.StatusId = 1;
                }
                else
                {
                    cmt.StatusId = 2;
                }

                _commentRepository.Update(cmt);
                _commentRepository.SaveChanges();
                TempData["pagecmt"] = page;
                return RedirectToAction("ListReport", "CommentManager");
            }
            else if (check.Equals("reply"))
            {

                var reply = _replyRepository.GetSingleById(id);
                if (work.Equals("active"))
                {
                    reply.StatusId = 1;
                }
                else
                {
                    reply.StatusId = 2;
                }

                _replyRepository.Update(reply);
                _replyRepository.SaveChanges();
                TempData["pagereply"] = page;
                return RedirectToAction("ListReplyReport", "CommentManager");

            }

            return View();
        }

        [Route("BlockUser")]
        [HttpPost]
        public IActionResult BlockUser(int id, string check, int page)
        {

            if (check.Equals("cmt"))
            {
                var user = _userRepository.GetSingleById(id);

                user.StatusId = 3;


                _userRepository.Update(user);
                _userRepository.SaveChanges();
                TempData["pagecmt"] = page;
                return RedirectToAction("ListReport", "CommentManager");
            }
            else if (check.Equals("reply"))
            {

                var user = _userRepository.GetSingleById(id);

                user.StatusId = 3;


                _userRepository.Update(user);
                _userRepository.SaveChanges();
                TempData["pagereply"] = page;
                return RedirectToAction("ListReplyReport", "CommentManager");

            }

            return View();
        }


    }
}
