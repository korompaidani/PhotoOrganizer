﻿using PhotoOrganizer.Model;
using System.Threading.Tasks;

namespace PhotoOrganizer.UI.Data.Repositories
{
    public interface ILocationRepository : IGenericRepository<Location>
    {
        Task<bool> IsReferencedByPhotoAsync(int locationId);
    }
}