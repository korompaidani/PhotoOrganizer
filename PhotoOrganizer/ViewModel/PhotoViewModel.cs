using System.IO;

namespace PhotoOrganizer.UI.ViewModel
{
    public class PhotoViewModel : ViewModelBase
    {
        public string FullPath { get; }

        public PhotoViewModel(string fullPath)
        {
            FullPath = Path.GetFullPath(fullPath);
        }
    }
}
