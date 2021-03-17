using System.ComponentModel.DataAnnotations;

namespace PhotoOrganizer.Model
{
    public class FileEntry
    {
        [Key]
        public int Id { get; set; }
        public string ImageFilePath { get; set; }
    }
}
