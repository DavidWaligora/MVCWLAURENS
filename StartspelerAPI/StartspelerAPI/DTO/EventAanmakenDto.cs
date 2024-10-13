using System.ComponentModel.DataAnnotations;

namespace StartspelerAPI.DTO
{
    public class EventAanmakenDto
    {
        private DateTime _startMoment;

        [Required]
        public string Naam {  get; set; }
        [Required]
        public DateTime StartMoment
        {
            get { return _startMoment; }
            set
            {
                if (value > DateTime.Now)
                {
                    _startMoment = value;
                }
            }
        }
    }
}
