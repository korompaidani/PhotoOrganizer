using PhotoOrganizer.Model;

namespace PhotoOrganizer.UI.Data.Repositories
{

    public interface IPhotoRepository : IGenericRepository<Photo>
    {        
        void RemovePeople(People model);
    }
}
