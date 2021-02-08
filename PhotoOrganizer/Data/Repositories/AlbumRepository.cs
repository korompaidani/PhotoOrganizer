﻿using PhotoOrganizer.DataAccess;
using PhotoOrganizer.Model;
using System.Threading.Tasks;
using System.Data.Entity;

namespace PhotoOrganizer.UI.Data.Repositories
{
    public class AlbumRepository : GenericRepository<Album, PhotoOrganizerDbContext>, IAlbumRepository
    {
        public AlbumRepository(PhotoOrganizerDbContext context) : base(context)
        {
        }

        public async override Task<Album> GetByIdAsync(int id)
        {
            return await Context.Albums
                .Include(a => a.Photos)
                .SingleAsync(a => a.Id == id);
        }
    }
}