using PhotoOrganizer.Model;

namespace PhotoOrganizer.UI.Helpers
{
    public class PhotoDetailInfo
    {
        public int Id { get; set; }
        public string FullFilePath { get; set; }
        public string FullTempFilePath { get; set; }
        public FileEntry FileEntry { get; set; }
    }
}
