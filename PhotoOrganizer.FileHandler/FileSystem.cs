using PhotoOrganizer.Common;
using System;
using System.IO;
using System.Text;

namespace PhotoOrganizer.FileHandler
{
    public class FileSystem
    {
        public void CreateTemp(string fullFileNamePath, out string fullTempFileNamePath)
        {
            try
            {
                fullTempFileNamePath = CreateTempFilePath(fullFileNamePath);

                if (File.Exists(fullFileNamePath))
                {
                    if (!File.Exists(fullTempFileNamePath))
                    {
                        File.Copy(fullFileNamePath, fullTempFileNamePath);
                    }
                }
                else
                {
                    fullTempFileNamePath = null;
                }
            }
            catch
            {
                fullTempFileNamePath = null;
            }
        }

        public bool DeleteTemp(string fullTempFileNamePath)
        {
            try
            {
                if (File.Exists(fullTempFileNamePath))
                {
                    File.Delete(fullTempFileNamePath);
                }
                else
                {
                    return false;
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool OverWriteOriginalByTemp(string fullFileNamePath, string fullTempFileNamePath)
        {
            try
            {
                if (File.Exists(fullFileNamePath) && File.Exists(fullTempFileNamePath))
                {
                    File.Delete(fullFileNamePath);
                }
                else
                {
                    return false;
                }

                if (!File.Exists(fullFileNamePath) && File.Exists(fullTempFileNamePath))
                {
                    File.Move(fullTempFileNamePath, fullFileNamePath);
                }
                else
                {
                    return false;
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        private string CreateTempFilePath(string originalFullPath)
        {
            var PathOnly = Path.GetDirectoryName(originalFullPath);
            var FileNameWithoutExtension = new StringBuilder(Path.GetFileNameWithoutExtension(originalFullPath));
            FileNameWithoutExtension.Append(FilePaths.TempFilePostfix);
            var FileExtension = Path.GetExtension(originalFullPath);

            return Path.Combine(PathOnly, FileNameWithoutExtension.ToString() + FileExtension);
        }
    }
}
