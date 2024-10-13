using StartspelerAPI.Models;

namespace StartspelerAPI.Data.Repository
{
    public class InschrijvingRepository : GenericRepository<Inschrijving>, IInschrijvingRepository
    {
        public InschrijvingRepository(StartspelerAPIContext context) : base(context) { }
    }
}
