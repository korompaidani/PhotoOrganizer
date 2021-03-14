using PhotoOrganizer.Common;
using System;
using System.Collections.Generic;
using System.IO;

namespace PhotoOrganizer.FileHandler
{
    public class DirectoryReader
    {
        private string _rootDirectoryPath;
        private IDictionary<string, string> _fileList;

        public IDictionary<string, string> FileList => _fileList;

        public DirectoryReader(string directoryPath = FilePaths.TestResources)
        {
            _rootDirectoryPath = directoryPath;
            _fileList = new Dictionary<string, string>();

            ReadDirectory(_rootDirectoryPath);
        }

        public void ReadDirectory(string dir)
        {
            try
            {
                foreach (var directory in Directory.GetDirectories(dir))
                {
                    foreach (var file in Directory.GetFiles(directory))
                    {
                        if (IsImageFile(Path.GetExtension(file)))
                        {
                            _fileList.Add(file, Path.GetFileNameWithoutExtension(file));
                        }                                                
                    }

                    ReadDirectory(directory);
                }
            }
            catch (Exception ex)
            {
            }
            finally
            {
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
