using Microsoft.AspNetCore.Mvc;
using ProjectReview.Models;
using ProjectReview.Repositories;
using System.Collections;
using System.Collections.Generic;

namespace ProjectReview.Controllers
{
    public class LocationController : Controller
    {

        private readonly ILocationRepository _locationRepository;
        private readonly ICommentRepository _commentRepository;
        private readonly IReplyRepository _replyRepository;
        public LocationController(ILocationRepository locationRepository,
            ICommentRepository commentRepository, IReplyRepository replyRepository)
        {

            _locationRepository = locationRepository;
            _commentRepository = commentRepository;
            _replyRepository = replyRepository;
        }

        public IActionResult Location()
        {
            return View();
        }

        public IActionResult LocationDetail(int id)
        {
            if (TempData.ContainsKey("locationId"))
            {
                int locationId = (int)TempData["locationId"];
                Location location = _locationRepository.GetSingleByCondition(l => l.Id == locationId);

                IEnumerable<Comment> comments = _commentRepository.GetCommentsForLocation(locationId);

                foreach (var comment in comments)
                {
                    comment.Replies = _replyRepository.GetRepliesForComment(comment.Id);
                }

                var locationWithComments = new LocationWithComments
                {
                    Location = location,
                    Comments = comments
                };

                return View(locationWithComments);

            }
            else
            {
                Location location = _locationRepository.GetSingleByCondition(l => l.Id == id);

                IEnumerable<Comment> comments = _commentRepository.GetCommentsForLocation(id);

                foreach (var comment in comments)
                {
                    comment.Replies = _replyRepository.GetRepliesForComment(comment.Id);
                }

                var locationWithComments = new LocationWithComments
                {
                    Location = location,
                    Comments = comments
                };

                return View(locationWithComments);
            }
        }
    }
}
