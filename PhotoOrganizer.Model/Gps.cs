using System.ComponentModel.DataAnnotations;

namespace PhotoOrganizer.Model
{
    public class Gps
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Coordinates { get; set; }

        public string LocationName { get; set; }
    }
}
