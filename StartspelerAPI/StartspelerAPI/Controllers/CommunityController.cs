using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StartspelerAPI.Data;
using StartspelerAPI.Data.UnitOfWork;
using StartspelerAPI.DTO;
using StartspelerAPI.Models;

namespace StartspelerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommunityController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CommunityController(UnitOfWork unitOfWork, Mapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpPost]
        [Authorize(Roles ="admin")]
        public async Task<ActionResult<Community>> CommunityToevoegen(CommunityAanmakenDto communityDto)
        {

            var communities = await _unitOfWork.CommunityRepository.GetAllAsync();
            if (communities == null)
            {
                return NotFound("De tabel Communities bestaat niet in de database.");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Community community = _mapper.Map<Community>(communityDto);

            await _unitOfWork.CommunityRepository.AddAsync(community);
            try
            {
                _unitOfWork.SaveChanges();
            }
            catch (DbUpdateException dbError)
            {
                return BadRequest(dbError);
            }
            return CreatedAtAction("GetCommunity", new { id = community.Id }, community);
        }

        [HttpGet("GetAll")]
        [Authorize(Roles = "admin, communityManager")]
        public async Task<ActionResult<List<Community>>> GetAllCommunities()
        {
            var communities = await _unitOfWork.CommunityRepository.GetAllAsync();
            if (communities.ToList().Count > 0)
            {
                return Ok(communities);
            }
            else
            {
                return NotFound("Er zijn geen resultaten gevonden");
            }
        }

        // GET api/<CommunityController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Community>> GetCommunity(int id)
        {
            Community? community = await _unitOfWork.CommunityRepository.GetByIdAsync(id);
            if (community == null)
            {
                return NotFound($"Community {id} kan niet worden gevonden in de database");
            }
            return Ok(community);
        }

        [HttpGet("Search")]
        [Authorize(Roles = "admin, communityManager")]
        public async Task<ActionResult<List<Community>>> Search(string zoekwaarde)
        {
            var communities = await _unitOfWork.CommunityRepository.GetAllAsync();
            var community =
                communities.Where(x => x.Naam.Contains(zoekwaarde)).OrderBy(x => x.Naam);
            if (community == null)
            {
                return NotFound("Er zijn geen communities in de database" +
                    $"waar {zoekwaarde} voorkomt in de naam");
            }
            return Ok(community);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "admin")]

        public async Task<ActionResult> CommunityWijzigen(string naam, Community community)
        {
            if (naam != community.Naam)
            {
                return BadRequest("De opgegeven id's komen niet overeen.");
            }
            _unitOfWork.CommunityRepository.Update(community);

            try
            {
                _unitOfWork.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                var communities = await _unitOfWork.CommunityRepository.GetAllAsync();
                if (communities.Any(x => x.Naam == naam))
                {
                    return NotFound("Er is geen community met dit id gevonden");
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> CommunityVerwijderen(int id)
        {
            var communities = await _unitOfWork.CommunityRepository.GetAllAsync();
            if (communities == null)
            {
                return NotFound("De tabel community bestaat niet.");
            }
            Community? community = await _unitOfWork.CommunityRepository.GetByIdAsync(id);
            if (community == null)
            {
                return NotFound("Het community met deze id is niet gevonden.");
            }

            _unitOfWork.CommunityRepository.Delete(community);
            _unitOfWork.SaveChanges();

            return Ok($"Community met id {id} is verwijderd");
        }
    }
}
