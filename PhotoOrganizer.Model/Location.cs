using System.ComponentModel.DataAnnotations;

namespace PhotoOrganizer.Model
{
    public class Location
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Coordinates { get; set; }
        public string LocationName { get; set; }
    }
}
