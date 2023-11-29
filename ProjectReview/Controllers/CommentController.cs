using Microsoft.AspNetCore.Mvc;
using ProjectReview.Repositories;
using Newtonsoft.Json;
using ProjectReview.Models;
using Microsoft.EntityFrameworkCore;

namespace ProjectReview.Controllers
{
    public class CommentController : Controller
    {

        private readonly IReplyRepository _replyRepository;
        private readonly ICommentRepository _commentRepository;
        public CommentController(IReplyRepository replyRepository, ICommentRepository commentRepository)
        {
            _replyRepository = replyRepository;
            _commentRepository = commentRepository;
        }

        [HttpPost]
        public IActionResult Index(int commentId, string replyContent, int locationId)
        {
            string? accJson = HttpContext.Session.GetString("User");
            User? acc = JsonConvert.DeserializeObject<User>(accJson);
            var newReply = new Reply
            {
                Content = replyContent,
                UserId = acc.Id,
                CommentId = commentId,
                Date = DateTime.Now,
                LikeNumber = 1
            };
            _replyRepository.Add(newReply);
            _replyRepository.SaveChanges();

            TempData["locationId"] = locationId;
            return RedirectToAction("LocationDetail", "Location");
        }

        public IActionResult Comment(int locationId, int userId, string commentContent, IFormFile image)
        {
            var newComment = new Comment
            {
                UserId = userId,
                Content = commentContent,
                LocationId = locationId,
                Date = DateTime.Now,
                LikeNumber = 1
            };
            _commentRepository.Add(newComment);
            _commentRepository.SaveChanges();

            if (image != null && image.Length > 0)
            {
                var uniqueFileName = Guid.NewGuid().ToString() + "_" + image.FileName;
                var filePath = Path.Combine("wwwroot/images/uploads", uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    image.CopyTo(stream);
                }

                newComment.Image = "/images/uploads/" + uniqueFileName;
                _commentRepository.SaveChanges();
            }

            TempData["locationId"] = locationId;
            return RedirectToAction("LocationDetail", "Location");
        }

        [HttpGet]
        public IActionResult Report(int id, int locaId)
        {
            var comment = _commentRepository.GetSingleById(id);
            if (comment != null)
            {
                comment.StatusId = 3;
                _commentRepository.Update(comment);
                _commentRepository.SaveChanges();
            }
            else
            {
                var reply = _replyRepository.GetSingleById(id);
                reply.StatusId = 3;
                _replyRepository.Update(reply);
                _replyRepository.SaveChanges();
            }
            TempData["locationId"] = locaId;
            return RedirectToAction("LocationDetail", "Location", new { id = locaId });
        }

        [HttpPost]
        public IActionResult ToggleLike(int commentId, bool isLiked)
        {
            var comment = _commentRepository.GetSingleByCondition(c => c.Id == commentId);

            // Tăng giảm LikeNumber tùy thuộc vào trạng thái hiện tại
            if (isLiked)
            {
                comment.LikeNumber--;
            }
            else
            {
                comment.LikeNumber++;
            }
            _commentRepository.SaveChanges();

            // Trả về số lượt like mới để cập nhật giao diện
            return Json(new { likeCount = comment.LikeNumber });
        }
    }
}
