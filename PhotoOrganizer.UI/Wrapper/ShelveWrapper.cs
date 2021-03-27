using PhotoOrganizer.Model;

namespace PhotoOrganizer.UI.Wrapper
{
    public class ShelveWrapper : NotifyDataErrorInfoBase
    {
        public Shelve Model { get; }
        public int Id { get { return Model.Id; } }

        public int Name
        {
            get { return Model.Name; }
            set
            {
                Model.Name = value;
                OnPropertyChanged();
            }
        }

        public ShelveWrapper(Shelve model)
        {
            Model = model;
        }
    }
}
