using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StartspelerAPI.Data.UnitOfWork;
using StartspelerAPI.DTO;
using StartspelerAPI.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace StartspelerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InschrijvingController : ControllerBase
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public InschrijvingController(UnitOfWork unitOfWork, Mapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        //POST
        [HttpPost("EventInschrijvingAPI")]
        public async Task<ActionResult<Event>> Inschrijven(InschrijvingEventDto toewijzenDto)
        {
            var events = await _unitOfWork.EventRepository.GetAllAsync();
            if (events == null || !events.Any())
            {
                return Problem("De tabel Event bestaat niet in de database.");
            }

            var eventToUpdate = await _unitOfWork.EventRepository.GetByIdAsync(toewijzenDto.EventId);
            if (eventToUpdate == null)
            {
                return NotFound($"Het event met ID {toewijzenDto.EventId} is niet gevonden.");
            }

            var existingInschrijving = await _unitOfWork.InschrijvingRepository
                .Find(x => x.GebruikerId == toewijzenDto.GebruikerId && x.EventId == toewijzenDto.EventId)
                .FirstOrDefaultAsync();

            if (existingInschrijving.Any())
            {
                return Conflict("Deze gebruiker is al ingeschreven voor dit event.");
            }

            // Map the DTO to an Inschrijving entity.
            Inschrijving newInschrijving = _mapper.Map<Inschrijving>(toewijzenDto);
            newInschrijving.TimestampInschrijving = DateTime.UtcNow;

            // Add the registration to the event's list of Inschrijvingen.
            eventToUpdate.Inschrijvingen ??= new List<Inschrijving>();
            eventToUpdate.Inschrijvingen.Add(newInschrijving);

            // Update the event in the repository (if tracking is needed).
            _unitOfWork.EventRepository.Update(eventToUpdate);

            // Save the changes to the database.
            try
            {
                _unitOfWork.SaveChanges();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }

            // Return a 201 Created response with the new Inschrijving.
            return CreatedAtAction("GetInschrijving", new { id = newInschrijving.Id }, newInschrijving);
        }

        [HttpDelete("UitschrijvenAPI")]
        public async Task<IActionResult> Uitschrijven(InschrijvingEventDto uitschrijvenDto)
        {
            // Fetch the specific event by its ID.
            var eventToUpdate = await _unitOfWork.EventRepository.GetByIdAsync(uitschrijvenDto.EventId);
            if (eventToUpdate == null)
            {
                return NotFound($"Het event met ID {uitschrijvenDto.EventId} is niet gevonden.");
            }

            // Find the registration for this event and user.
            var inschrijving = await _unitOfWork.InschrijvingRepository
                .Find(x => x.GebruikerId == uitschrijvenDto.GebruikerId && x.EventId == uitschrijvenDto.EventId)
                .FirstOrDefaultAsync();

            if (inschrijving == null)
            {
                return NotFound($"Er is geen inschrijving gevonden voor gebruiker {uitschrijvenDto.GebruikerId} bij event {uitschrijvenDto.EventId}.");
            }

            // Remove the inschrijving from the event's list.
            eventToUpdate.Inschrijvingen?.Remove(inschrijving);

            // Delete the inschrijving from the repository.
            _unitOfWork.InschrijvingRepository.Delete(inschrijving);

            // Save changes asynchronously.
            try
            {
                _unitOfWork.SaveChanges();  // <-- Changed to async
            }
            catch (Exception ex)
            {
                return BadRequest($"Er is iets fout gegaan: {ex.Message}");  // <-- Added detailed error response
            }

            return Ok($"Gebruiker {uitschrijvenDto.GebruikerId} is uitgeschreven voor event {uitschrijvenDto.EventId}.");
        }

        [HttpGet("InschrijvingenVanEvent/{eventId}")]
        public async Task<ActionResult<List<Inschrijving>>> GetInschrijvingenVanEvent(int eventId)
        {
            // Fetch the event with its registrations
            var eventWithRegistrations = await _unitOfWork.EventRepository
                .Find(e => e.Id == eventId)
                .Include(e => e.Inschrijvingen)  // Eager load the registrations
                .FirstOrDefaultAsync();

            if (eventWithRegistrations == null)
            {
                return NotFound($"Het event met ID {eventId} is niet gevonden.");
            }

            if (eventWithRegistrations.Inschrijvingen == null || !eventWithRegistrations.Inschrijvingen.Any())
            {
                return Ok(new List<Inschrijving>());  // Return empty list if no registrations
            }

            return Ok(eventWithRegistrations.Inschrijvingen);
        }
    }
}
