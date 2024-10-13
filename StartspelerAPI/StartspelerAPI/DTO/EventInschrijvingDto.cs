using StartspelerAPI.Models;

namespace StartspelerAPI.DTO
{
    public class EventInschrijvingDto
    {
        public int Id { get; set; }
        public List<Inschrijving> Inschrijvingen { get; set; }
    }
}
