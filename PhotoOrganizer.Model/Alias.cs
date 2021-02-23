using System.ComponentModel.DataAnnotations;

namespace PhotoOrganizer.Model
{
    public class Alias
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Nick { get; set; }
        public int PeopleId { get; set; }
        public People People { get; set; }
    }
}
