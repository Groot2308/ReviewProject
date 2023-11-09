using ProjectReview.Interfaces;
using ProjectReview.Models;

namespace ProjectReview.Repositories
{
    public interface IRoleRepository : IRepository<Role>
    {

    }
    public class RoleRepository : RepositoryBase<Role>, IRoleRepository
    {
        public RoleRepository(PROJECTREVIEWContext dbContext) : base(dbContext)
        {
        }
    }
}
