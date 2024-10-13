using AutoMapper;
using StartspelerAPI.DTO;
using StartspelerAPI.Models;

namespace StartspelerAPI.Configuration
{
    public class MapperProfile: Profile
    {
        public MapperProfile()
        {


            CreateMap<CommunityEventDto, Community>();
            CreateMap<EventInschrijvingDto, Event>();
            CreateMap<GebruikerWijzigenDto, Gebruiker>();
        }
    }
}
