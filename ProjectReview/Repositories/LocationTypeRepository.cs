using ProjectReview.Interfaces;
using ProjectReview.Models;

namespace ProjectReview.Repositories
{
    public interface ILocationTypeRepository : IRepository<LocationType>
    {

    }
    public class LocationTypeRepository : RepositoryBase<LocationType>, ILocationTypeRepository
    {
        public LocationTypeRepository(PROJECTREVIEWContext dbContext) : base(dbContext)
        {
        }
    }
}
