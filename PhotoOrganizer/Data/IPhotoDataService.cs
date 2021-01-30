using PhotoOrganizer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoOrganizer
{
    public interface IPhotoDataService
    {
        Task<Photo> GetByIdAsync(int photoId);
        Task SaveAsync(Photo photo);
    }
}
