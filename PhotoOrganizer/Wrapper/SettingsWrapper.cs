using PhotoOrganizer.Model;

namespace PhotoOrganizer.UI.Wrapper
{
    public class SettingsWrapper : NotifyDataErrorInfoBase
    {
        public Settings Model { get; }

        public int PageSize
        {
            get { return Model.PageSize; }
            set
            {
                Model.PageSize = value;
                OnPropertyChanged();
            }
        }

        public string Language
        {
            get { return Model.Language; }
            set
            {
                Model.Language = value;
                OnPropertyChanged();
            }
        }

        public SettingsWrapper(Settings settings)
        {
            Model = settings;
        }
    }
}
