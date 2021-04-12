using PhotoOrganizer.Common;
using System;
using System.IO;
using System.Windows.Media.Imaging;

namespace PhotoOrganizer.FileHandler
{
    public class Picture
    {
        private static BitmapSource defaultPicture;

        static Picture()
        {
            var defaultUri = new Uri(Path.GetFullPath(FilePaths.DefaultPicturePath));
            defaultPicture = BitmapFrame.Create(defaultUri).Thumbnail;
        }

        public Picture(string path)
        {
            if (path == null || !File.Exists(Path.GetFullPath(path)))
            {
                path = Path.GetFullPath(FilePaths.DefaultPicturePath);
                Source = path;
                Uri = new Uri(path);
                Thumbnail = defaultPicture;
                return;
            }

            path = Path.GetFullPath(path);
            Source = path;
            Uri = new Uri(path);
            Thumbnail = BitmapFrame.Create(Uri).Thumbnail;
            if (Thumbnail == null)
            {
                Thumbnail = CreateThumbnail(path);
            }
        }

        public string Source { get; }
        public Uri Uri { get; set; }
        public BitmapSource Thumbnail { get; set; }

        private BitmapSource CreateThumbnail(string path)
        {
            using (var stream = File.OpenRead(path))
            {
                BitmapImage image = new BitmapImage();
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.StreamSource = stream;
                image.DecodePixelWidth = 120;
                image.EndInit();
                return image;
            }
        }
    }
}
