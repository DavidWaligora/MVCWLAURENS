using StartspelerAPI.Models;

namespace StartspelerAPI.Data.Repository
{
    public class EventRepository : GenericRepository<Event>, IEventRepository
    {
        public EventRepository(StartspelerAPIContext context) : base(context) { }
    }
}
