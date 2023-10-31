using ProjectReview.Interfaces;
using ProjectReview.Models;

namespace ProjectReview.Repositories
{
    public interface IUserRepository : IRepository<User>
    {

    }
    public class UserRepository : RepositoryBase<User>, IUserRepository
    {
        public UserRepository(PROJECTREVIEWContext dbContext) : base(dbContext)
        {
        }
    }
}
