using Microsoft.AspNetCore.Mvc;
using ProjectReview.Repositories;
using Newtonsoft.Json;
using ProjectReview.Models;
namespace ProjectReview.Controllers
{
    public class CommentController : Controller
    {

        private readonly IReplyRepository _replyRepository;
        private readonly ICommentRepository _commentRepository ;
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

        public IActionResult Comment(int locationId, int userId, string commentContent)
        {
            var newComment = new Comment {
                
                UserId = userId, 
                Content = commentContent,
                LocationId = locationId,
                Date = DateTime.Now,
                LikeNumber = 1    
            };
            _commentRepository.Add(newComment);
            _commentRepository.SaveChanges();

            TempData["locationId"] = locationId;
            return RedirectToAction("LocationDetail", "Location");
        }
    }
}
