using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StartspelerAPI.DTO;
using StartspelerAPI.Helper;
using StartspelerAPI.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace StartspelerAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class GebruikerController : ControllerBase
    {
        private UserManager<Gebruiker> _userManager;
        private SignInManager<Gebruiker> _signInManager;
        private RoleManager<IdentityRole> _roleManager;
        private IMapper _mapper;

        public GebruikerController(UserManager<Gebruiker> userManager, RoleManager<IdentityRole> roleManager, SignInManager<Gebruiker> signInManager, IMapper mapper)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _mapper = mapper;
        }

        // Registeren als nieuwe gebruiker
        [HttpPost("register"), AllowAnonymous]
        public async Task<IActionResult> Register(RegistratieDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var gebruiker = await _userManager.FindByEmailAsync(request.Email);
            if (gebruiker == null)
            {
                var user = new Gebruiker
                {
                    UserName = request.Email,
                    NormalizedUserName = request.Email,
                    Email = request.Email,
                    PhoneNumber = request.PhoneNumber,
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true
                };
                var result = await _userManager.CreateAsync(user, request.Password);
                //await _userManager.AddToRoleAsync(user, request.Rol);

                if (result.Succeeded)
                    return Ok();
                else
                {
                    if (result.Errors.Count() > 0)
                    {
                        foreach (var error in result.Errors)
                            ModelState.AddModelError("message", error.Description);
                    }
                    return BadRequest(ModelState);
                }
            }
            else
            {
                ModelState.AddModelError("message", "Gebruiker is aanwezig is database.");
                return BadRequest(ModelState);
            }
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user != null && !user.EmailConfirmed)
            {
                ModelState.AddModelError("message", "Emailadres is nog niet bevestigd.");
                return BadRequest(model);
            }
            if (await _userManager.CheckPasswordAsync(user, model.Password) == false)
            {
                // Nooit exacte informatie geven: zeg alleen dat combinatie vekeerd is...
                ModelState.AddModelError("message", "Verkeerde logincombinatie!");
                return BadRequest(model);
            }

            var result = await _signInManager.PasswordSignInAsync(user.UserName, model.Password, false, true);

            if (result.IsLockedOut)
                ModelState.AddModelError("message", "Account geblokkeerd!!");

            if (result.Succeeded)
            {
                var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

                var userRoles = await _userManager.GetRolesAsync(user);
                if (userRoles != null)
                {
                    foreach (var userRole in userRoles)
                        authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                }

                var token = Token.GetToken(authClaims);

                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    expiration = token.ValidTo
                });
            }

            ModelState.AddModelError("message", "Ongeldige loginpoging");
            return Unauthorized(ModelState);
        }


        [HttpPost("GetAlleUsersMetRollen")]
        public async Task<IActionResult> GetAlleUsersMetRollen()
        {
            var users = await _userManager.Users.ToListAsync();

            var userWithRolesList = new List<GebruikerMetRollenDto>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userWithRolesList.Add(new GebruikerMetRollenDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    UserName = user.UserName,
                    Roles = roles
                });
            }

            return Ok(userWithRolesList);
        }

        [HttpPost("GrantPermission")]
        public async Task<IActionResult> GrantPermission(GrantPermissionDto gpm)
        {
            Gebruiker? gb = await _userManager.FindByEmailAsync(gpm.Email); // Gebruiker zoeken via email opgegeven
            IdentityRole? rol = await _roleManager.FindByNameAsync(gpm.RolNaam); // nieuwe rol!

            // Gebruiker bestaat en de nieuwe is ook beschikbaar?
            if (gb != null && rol != null)
            {
                // Huidige rollen opvragen in systeem
                var huidigeRoles = await _userManager.GetRolesAsync(gb);

                if (huidigeRoles.Any())
                {
                    var huidigeRole = huidigeRoles.First(); // Slechts een rol
                    var removeResult = await _userManager.RemoveFromRoleAsync(gb, huidigeRole);
                    if (!removeResult.Succeeded)
                    {
                        if (removeResult.Errors.Count() > 0)
                        {
                            foreach (var error in removeResult.Errors)
                                ModelState.AddModelError("message", error.Description);
                        }
                        return BadRequest(ModelState);
                    }
                }

                IdentityResult res = await _userManager.AddToRoleAsync(gb, rol.Name);

                if (res.Succeeded)
                    return Ok();
                else
                {
                    foreach (IdentityError error in res.Errors)
                        ModelState.AddModelError("", error.Description);

                    return BadRequest(ModelState);
                }
            }
            else
                ModelState.AddModelError("", "De gebruiker bestaat niet.");

            ModelState.AddModelError("message", "Onbekende fout");
            return Unauthorized(ModelState);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, GebruikerWijzigenDto userDto)
        {
            if (userDto == null || id != userDto.Id) return BadRequest(ModelState);

            Gebruiker? gebruiker = await _userManager.FindByIdAsync(id);

            if (gebruiker == null) return NotFound(ModelState);

            if (ModelState.IsValid)
            {
                try
                {
                    _mapper.Map(userDto, gebruiker);


                    if (!string.IsNullOrWhiteSpace(userDto.Password))
                    {
                        // Het huidige wachtwoord zou moeten worden doorgegeven door de gebruiker, niet de hash
                        var result = await _userManager.ChangePasswordAsync(gebruiker, userDto.CurrentPassword, userDto.Password);

                        if (!result.Succeeded)
                        {
                            foreach (var error in result.Errors)
                            {
                                ModelState.AddModelError("message", error.Description);
                            }
                            return BadRequest(ModelState);
                        }
                    }

                    IdentityResult updateResult = await _userManager.UpdateAsync(gebruiker);
                    if (updateResult.Succeeded)
                        return Ok("De gebruiker is gewijzigd");
                    else
                    {
                        if (updateResult.Errors.Count() > 0)
                        {
                            foreach (var error in updateResult.Errors)
                                ModelState.AddModelError("message", error.Description);
                        }
                        return BadRequest(ModelState);
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    throw;
                }
            }
            return BadRequest();
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            Gebruiker? gebruiker = await _userManager.FindByIdAsync(id);
            if (gebruiker != null)
            {
                IdentityResult result = await _userManager.DeleteAsync(gebruiker);
                if (result.Succeeded)
                    return Ok("De gebruiker is succesvol verwijderd.");
                else
                {
                    if (result.Errors.Count() > 0)
                    {
                        foreach (var error in result.Errors)
                            ModelState.AddModelError("message", error.Description);
                    }
                    return BadRequest(ModelState);
                }
            }
            return NotFound();
        }
    }
}
