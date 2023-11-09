using Microsoft.EntityFrameworkCore;
using ProjectReview.Interfaces;
using ProjectReview.Models;
using System.ComponentModel.Design;


namespace ProjectReview.Repositories
{
    public interface IReplyRepository : IRepository<Reply>
    {
        //IEnumerable<Reply> GetRepliesForComment(int id);
        List<Reply> GetRepliesForComment(int id);
    }

    public class ReplyRepository : RepositoryBase<Reply>, IReplyRepository
    {
        public ReplyRepository(PROJECTREVIEWContext dbContext) : base(dbContext)
        {
        }

        public List<Reply> GetRepliesForComment(int id)
        {
            using (PROJECTREVIEWContext context = new PROJECTREVIEWContext())
            {
                var repliesForComment = context.Replies
                    .Include(r => r.User)
                    .Where(r => r.CommentId == id)
                    .ToList();
                return repliesForComment;
            }

        }
    }
}