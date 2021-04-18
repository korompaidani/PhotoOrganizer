using System;
using System.Collections.Generic;
using System.IO;

namespace PhotoOrganizer.FileHandler
{
    public class DirectoryReader
    {
        private IDictionary<string, string> _fileList;

        public IDictionary<string, string> FileList => _fileList;

        public DirectoryReader()
        {
            _fileList = new Dictionary<string, string>();
        }

        public void ReadDirectory(string dir)
        {
            try
            {                
                foreach (var file in Directory.GetFiles(dir))
                {
                    if (IsImageFile(Path.GetExtension(file)))
                    {
                        _fileList.Add(file, Path.GetFileNameWithoutExtension(file));
                    }                                                
                }

                foreach (var directory in Directory.GetDirectories(dir))
                {
                    ReadDirectory(directory);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private bool IsImageFile(string extension)
        {
            if(extension == ".jpg" || 
                extension == ".JPG" ||
                extension == ".png" ||
                extension == ".PNG" ||
                extension == ".jpeg" ||
                extension == ".JPEG" ||
                extension == ".BMP" ||
                extension == ".bmp" ||
                extension == ".TIFF" ||
                extension == ".tiff")
            {
                return true;
            }
            return false;
        }
    }
}
