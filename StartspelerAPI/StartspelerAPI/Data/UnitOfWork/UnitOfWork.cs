using StartspelerAPI.Data.Repository;
using StartspelerAPI.Models;

namespace StartspelerAPI.Data.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly StartspelerAPIContext _context;
        private ICommunityRepository communityRepository;
        private IEventRepository eventRepository;
        private IInschrijvingRepository inschrijvingRepository;


        public UnitOfWork(StartspelerAPIContext context) { 
        _context = context;
        }
        public ICommunityRepository CommunityRepository
        {
            get 
            {
                if (this.communityRepository == null)
                {
                    this.communityRepository = new CommunityRepository(_context);
                }
                    return communityRepository;
            }
        }

        public IEventRepository EventRepository
        {
            get
            {
                if (this.eventRepository == null)
                {
                    this.eventRepository = new EventRepository(_context);
                }
                return eventRepository;
            }
        }

        public IInschrijvingRepository InschrijvingRepository
        {
            get
            {
                if (this.inschrijvingRepository == null)
                {
                    this.inschrijvingRepository = new InschrijvingRepository(_context);
                }
                return inschrijvingRepository;
            }
        }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }
    }
}
