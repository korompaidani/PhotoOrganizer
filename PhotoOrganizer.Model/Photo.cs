using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace PhotoOrganizer.Model
{
    public class Photo
    {
        public Photo()
        {
            Peoples = new Collection<People>();
            Albums = new Collection<Album>();
        }

        [Key]
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string FullPath { get; set; }
        [Timestamp]
        public byte[] RowVersion { get; set; }
        public int? LocationId { get; set; }
        public Location Location { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public int Day { get; set; }
        public TimeSpan HHMMSS { get; set; }        
        public ICollection<People> Peoples { get; set; }
        public ICollection<Album> Albums { get; set; }
        public string ColorFlag { get; set; }
    }
}
