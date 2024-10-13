using System.ComponentModel.DataAnnotations;

namespace StartspelerAPI.DTO
{
    public class RegistratieDto
    {
        [Required(ErrorMessage = "Naam is benodigd!")]
        [StringLength(100)]
        public string Name { get; set; } = "";

        [EmailAddress(ErrorMessage = "Ongeldig emailadres")]
        [Required(ErrorMessage = "Email is verplicht!")]
        public string Email { get; set; } = "";

        [Required(ErrorMessage = "Password is verplicht!")]
        public string Password { get; set; } = "";

        [Required(ErrorMessage = "Tweede wachtwoord is verplicht in te vullen.")]
        [Compare("Password", ErrorMessage = "De wachtwoorden komen niet overeen.")]
        public string ConfirmPassword { get; set; } = "";

        public string PhoneNumber { get; set; } = "";
    }
}
