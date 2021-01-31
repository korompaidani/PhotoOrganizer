using System.ComponentModel.DataAnnotations;

namespace PhotoOrganizer.Model
{
    public class Year
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int PhotoTakenYear { get; set; }
    }
}
