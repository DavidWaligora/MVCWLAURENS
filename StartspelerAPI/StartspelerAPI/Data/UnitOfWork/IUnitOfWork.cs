using StartspelerAPI.Data.Repository;
using StartspelerAPI.Models;

namespace StartspelerAPI.Data.UnitOfWork
{
    public interface IUnitOfWork
    {
        ICommunityRepository CommunityRepository { get; }
        IEventRepository EventRepository { get; }
        IInschrijvingRepository InschrijvingRepository { get; }

        public void SaveChanges();

    }
}
