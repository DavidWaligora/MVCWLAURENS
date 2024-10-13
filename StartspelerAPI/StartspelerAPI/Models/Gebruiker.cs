using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace StartspelerAPI.Models
{
    public class Gebruiker : IdentityUser
    {
        [PersonalData]
        [Required]
        [MaxLength(50, ErrorMessage = "Niet te lang")]
        public string Voornaam { get; set; }
        [PersonalData]
        [Required]
        [MaxLength(50, ErrorMessage = "Niet te lang")]
        public string Achternaam { get; set; }
        [PersonalData]
        [Required]
        public DateTime Geboortedatum { get; set; }

        public List<Inschrijving>? Inschrijvingen { get; set; }
    }
}
