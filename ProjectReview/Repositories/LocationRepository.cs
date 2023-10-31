using ProjectReview.Interfaces;
using ProjectReview.Models;

namespace ProjectReview.Repositories
{
    public interface ILocationRepository : IRepository<Location>
    {
        // cái này sinh ra để ae có thể tạo thêm hàm nếu như hàm đó không có trong base (Custom fuiunction)
    }
    //còn đây là hàm để chiển khai hàm mới hoặc custom lại hàm base
    public class LocationRepository : RepositoryBase<Location>, ILocationRepository
    {
        public LocationRepository(PROJECTREVIEWContext dbContext) : base(dbContext)
        {
        }
    }
}
