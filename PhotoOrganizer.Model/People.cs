using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace PhotoOrganizer.Model
{
    public class People
    {
        public People()
        {
            Aliases = new Collection<Alias>();
        }

        [Key]
        public int Id { get; set; }

        [Required]
        public string DisplayName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int PhotoId { get; set; }
        public virtual ICollection<Photo> Photos { get; set; }
        public ICollection<Alias> Aliases { get; set; }
    }
}
