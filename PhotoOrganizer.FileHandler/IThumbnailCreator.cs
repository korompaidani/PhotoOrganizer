namespace PhotoOrganizer.FileHandler
{
    public interface IThumbnailCreator
    {
        string WriteThumbnailWithPath(string originalImagePath);
        void DeleteThumbnail(string thumbnailPath);
    }
}
