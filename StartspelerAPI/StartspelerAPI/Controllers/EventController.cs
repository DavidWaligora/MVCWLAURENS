using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StartspelerAPI.Data.UnitOfWork;
using StartspelerAPI.DTO;
using StartspelerAPI.Models;
using System.Numerics;

namespace StartspelerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public EventController(UnitOfWork unitOfWork, Mapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpPost]
        [Authorize(Roles = "admin, communityManager")]

        public async Task<ActionResult<Event>> EventToevoegen(EventAanmakenDto eventAanmakenDto)
        {
            var events = await _unitOfWork.EventRepository.GetAllAsync();
            if (events == null)
            {
                return NotFound("De tabel Events bestaat niet in de database.");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Event eve = _mapper.Map<Event>(eventAanmakenDto);

            await _unitOfWork.EventRepository.AddAsync(eve);
            try
            {
                _unitOfWork.SaveChanges();
            }
            catch (DbUpdateException dbError)
            {
                return BadRequest(dbError);
            }
            return CreatedAtAction("GetEvents", new { id = eve.Id }, eve);
        }

        // GET api/<ProductController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Event>> GetProduct(int id)
        {
            Event? eve = await _unitOfWork.EventRepository.GetByIdAsync(id);
            if (eve == null)
            {
                return NotFound($"Product {id} kan niet worden gevonden in de database");
            }
            return Ok(eve);
        }

        [HttpGet("GetAllEvents")]
        public async Task<ActionResult<List<Event>>> GetAllEvents()
        {
            var events = await _unitOfWork.EventRepository.GetAllAsync();
            if (events.ToList().Count > 0)
            {
                return Ok(events);
            }
            else
            {
                return NotFound("Er zijn geen resultaten gevonden");
            }
        }

        [Authorize(Roles = "admin, communityManager")]
        [HttpPut("{id}")]
        public async Task<IActionResult> EventWijzigen(string naam, DateTime startMoment, EventWijzigenDto eventdto)
        {
            if (naam != eventdto.Naam)
            {
                return BadRequest("De opgegeven id's komen niet overeen.");
            }

            //
            var evenst = await _unitOfWork.EventRepository.GetAllAsync();
            var eve =
                evenst.Where(x => x.Startmoment.Date == startMoment.Date && x.Naam.Contains(naam)).OrderBy(x => x.Naam);
            if (eve == null)
            {
                return NotFound("Er zijn geen events in de database" +
                    $"waar {naam} voorkomt");
            }

            _unitOfWork.EventRepository.Update(eve.First());
            return NoContent();
        }

        [Authorize(Roles = "admin, communityManager")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> EventVerwijderen(int id)
        {
            var events = await _unitOfWork.EventRepository.GetAllAsync();
            if (events == null)
            {
                return NotFound("De tabel event bestaat niet.");
            }

            Event? eve = await _unitOfWork.EventRepository.GetByIdAsync(id);
            if (eve == null)
            {
                return NotFound("Het event met deze id is niet gevonden.");
            }

            eve.Inschrijvingen.Clear();

            _unitOfWork.EventRepository.Delete(eve);
            _unitOfWork.SaveChanges();

            return Ok($"Event met id {id} is verwijderd");
        }

    }
}
