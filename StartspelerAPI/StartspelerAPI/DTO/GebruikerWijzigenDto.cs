namespace StartspelerAPI.DTO
{
    public class GebruikerWijzigenDto
    {
        public string Id { get; set; }
        public string? Naam { get; set; }
        public DateTime? Geboortedatum { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string CurrentPassword { get; set; }
        public string Password { get; set; }
        public string PhoneNumber { get; set; }
    }
}
