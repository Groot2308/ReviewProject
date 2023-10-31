using ProjectReview.Interfaces;
using ProjectReview.Models;


namespace ProjectReview.Repositories
{
    public interface ICommentRepository : IRepository<Comment>
    {

    }

    public class CommentRepository : RepositoryBase<Comment>, ICommentRepository
    {
        public CommentRepository(PROJECTREVIEWContext dbContext) : base(dbContext)
        {
        }
    }
}
