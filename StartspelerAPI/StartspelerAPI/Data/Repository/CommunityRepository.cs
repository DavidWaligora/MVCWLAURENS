using StartspelerAPI.Models;

namespace StartspelerAPI.Data.Repository
{
    public class CommunityRepository : GenericRepository<Community>, ICommunityRepository
    {
        public CommunityRepository(StartspelerAPIContext context) : base(context) { }
    }
}
