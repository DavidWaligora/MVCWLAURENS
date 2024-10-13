using System.ComponentModel.DataAnnotations;

namespace StartspelerAPI.DTO
{
    public class CommunityAanmakenDto
    {
        [Required]
        public string Naam { get; set; }
    }
}
