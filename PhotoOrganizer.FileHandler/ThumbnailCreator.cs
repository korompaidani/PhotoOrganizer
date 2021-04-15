using PhotoOrganizer.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Media.Imaging;

namespace PhotoOrganizer.FileHandler
{
    public class ThumbnailCreator : IThumbnailCreator
    {
        private const string TargetRootFolder = FilePaths.ThumbnailsRootFolder;
        private const string FileExtension = ".png";
        private const int MaxFileCountThreshold = 500;
        
        private string _actualFolder;
        private string _actualFileName;
        private string _actualFullFileName;

        public string ActualFullFileName => _actualFullFileName;

        public void DeleteThumbnail(string thumbnailPath)
        {
            if (File.Exists(thumbnailPath))
            {
                File.Delete(thumbnailPath);
            }
            var parentDirectory = Directory.GetParent(thumbnailPath);
            if(parentDirectory.GetFiles().Length == 0 && parentDirectory.GetDirectories().Length == 0)
            {
                Directory.Delete(parentDirectory.FullName);
            }
        }

        public string WriteThumbnailWithPath(string originalImagePath)
        {
            var tragetFullFilePath = DecideTargetLocationForThumbnail();
            var bitmapImage = CreateThumbnail(originalImagePath);
            SaveBitmapImageIntoFile(bitmapImage, tragetFullFilePath);
            return tragetFullFilePath;
        }

        private string DecideTargetLocationForThumbnail()
        {
            var root = Directory.CreateDirectory(TargetRootFolder);
            
            var subFolders = Directory.GetDirectories(root.FullName);
            var folderNamesAsNumbers = ReturnContentsAsNumbers(subFolders, true);

            if (folderNamesAsNumbers.Count == 0)
            {
                Directory.CreateDirectory(Path.Combine(root.FullName, 0.ToString()));
                folderNamesAsNumbers.Add(0);
            }

            var biggestNumber = folderNamesAsNumbers.Max();
            _actualFolder = Path.Combine(root.FullName, biggestNumber.ToString());

            if (Directory.GetFiles(Path.Combine(root.FullName, biggestNumber.ToString())).Length > MaxFileCountThreshold)
            {
                _actualFolder = Path.Combine(root.FullName, (biggestNumber + 1).ToString());
                Directory.CreateDirectory(_actualFolder);
            }            

            var files = Directory.GetFiles(_actualFolder);
            var fileNamesAsNumbers = ReturnContentsAsNumbers(files, false);

            if (fileNamesAsNumbers.Count == 0)
            {
                _actualFileName = 0.ToString();
            }
            else
            {
                _actualFileName = (fileNamesAsNumbers.Max() + 1).ToString();
            }

            _actualFullFileName = Path.Combine(_actualFolder, _actualFileName + FileExtension);

            return _actualFullFileName;
        }

        private List<int> ReturnContentsAsNumbers(string[] contents, bool isDirectory)
        {
            var partNamesAsNumbers = new List<int>();

            foreach (var content in contents)
            {
                string part = null;
                if (isDirectory)
                {
                    var parts = content.Split('\\');
                    part = parts[parts.Length - 1];
                }
                else
                {
                    part = Path.GetFileNameWithoutExtension(content);
                }
                
                partNamesAsNumbers.Add(Int32.Parse(part));
            }

            return partNamesAsNumbers;
        }

        private BitmapImage CreateThumbnail(string path)
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

        private void SaveBitmapImageIntoFile(BitmapImage bitmapImage, string filePath)
        {
            BitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bitmapImage));

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                encoder.Save(fileStream);
            }
        }
    }
}
