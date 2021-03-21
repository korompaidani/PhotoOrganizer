using PhotoOrganizer.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PhotoOrganizer.UI.Data.Repositories
{
    public interface IPhotoRepository : IGenericRepository<Photo>
    {
        Task<bool> HasAlbums(int photoId);
        void RemovePeople(People model);
        Task<bool> HasPhotosAsync();
        Task RemoveAllPhotoFromTableAsync();
        Task<List<Photo>> GetModifiedPhotosAsync();
        Task<int?> GetMaxPhotoIdAsync();
        void AddRange(Photo[] photos);
        Task AddRangeAsync(Photo[] photos);

        Task<bool> HasPeopleDisplayName(string displayName);
        Task<People> TryGetAnyPeopleByDisplayName(string displayName);
        Task<People> AddGetPeopleByUniqueDisplayNameAsync(string displayName);
        void AddPeople(People model);
        Task<List<People>> GetAllPeopleAsync();
        Task<People> GetPeopleByIdAsync(int id);

        Task AddPhotoToShelveAsync(Photo photo);
        Task RemovePhotoToShelveAsync(Photo photo);
        bool IsPhotoExistOnShelve(int photoId);
        Task ReloadPhotoAsync(int photoId);
        List<Photo> GetAllPhotoOfShelve();
    }
}
