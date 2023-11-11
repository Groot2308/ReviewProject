using System;
using System.Collections.Generic;

namespace ProjectReview.Models
{
    public partial class CommentStatus
    {
        public CommentStatus()
        {
            Comments = new HashSet<Comment>();
        }

        public int Id { get; set; }
        public string Name { get; set; } = null!;

        public virtual ICollection<Comment> Comments { get; set; }
    }
}
