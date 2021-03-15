using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PhotoOrganizer.Model
{
    public class Shelve
    {
        public Shelve()
        {
        }

        [Key]
        public int Id { get; set; }     
        public int Name { get; set; }
        public int PhotoId { get; set; }
        public virtual ICollection<Photo> Photos { get; set; }
    }
}
