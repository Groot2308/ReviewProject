using Microsoft.EntityFrameworkCore;
using ProjectReview.Interfaces;
using ProjectReview.Models;

namespace ProjectReview.Repositories
{
    public interface ICommentRepository : IRepository<Comment>
    {
        IEnumerable<Comment> GetCommentsForLocation(int id);
    }

    public class CommentRepository : RepositoryBase<Comment>, ICommentRepository
    {
        public CommentRepository(PROJECTREVIEWContext dbContext) : base(dbContext)
        {
        }

        public IEnumerable<Comment> GetCommentsForLocation(int id)
        {
            using (PROJECTREVIEWContext context = new PROJECTREVIEWContext())
            {
                var commentsForLocation = context.Comments
                   .Include(c => c.User)
                   .Where(c => c.LocationId == id)
                   .ToList();
                return commentsForLocation;
            }
        }
    }
}
