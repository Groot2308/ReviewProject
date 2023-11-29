using ProjectReview.Models;

namespace ProjectReview.Areas.Admin.Models
{
    public class CommentReport
    {
        public Comment? Comment { get; set; }
        public Reply? Reply { get; set; }

    }
}
