using System;
using System.IO;
using System.Windows.Media.Imaging;

namespace PhotoOrganizer.Image
{
    public class Picture
    {
        private const string DefaultPicturePath = @".\..\..\Resources\Pictures\DefaultPicture.jpg";
        private static BitmapSource defaultPicture;
        private static ExifMetadata defaultExifMetadata;

        static Picture()
        {
            var defaultUri = new Uri(Path.GetFullPath(DefaultPicturePath));
            defaultPicture = BitmapFrame.Create(defaultUri).Thumbnail;
            defaultExifMetadata = new ExifMetadata(defaultUri);
        }

        public Picture(string path)
        {
            if (path == null || !File.Exists(Path.GetFullPath(path)))
            {
                path = Path.GetFullPath(DefaultPicturePath);
                Source = path;
                Uri = new Uri(path);
                Thumbnail = defaultPicture;
                Metadata = defaultExifMetadata;
                return;
            }

            path = Path.GetFullPath(path);
            Source = path;
            Uri = new Uri(path);
            Thumbnail = BitmapFrame.Create(Uri).Thumbnail;
            Metadata = new ExifMetadata(Uri);
        }

        public string Source { get; }
        public Uri Uri { get; set; }
        public BitmapSource Thumbnail { get; set; }
        public ExifMetadata Metadata { get; }
    }
}
