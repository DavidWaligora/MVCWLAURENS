using StartspelerAPI.Models;

namespace StartspelerAPI.DTO
{
    public class CommunityEventDto
    {
        public string Naam { get; set; }
        public List<Event> Events { get; set; }
    }
}

