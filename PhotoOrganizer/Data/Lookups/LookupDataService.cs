using PhotoOrganizer.DataAccess;
using PhotoOrganizer.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace PhotoOrganizer.UI.Data.Lookups
{
    public class LookupDataService : IPhotoLookupDataService, IYearLookupDataService, IAlbumLookupDataService
    {
        private Func<PhotoOrganizerDbContext> _contextCreator;

        public LookupDataService(Func<PhotoOrganizerDbContext> contextCreator)
        {
            _contextCreator = contextCreator;
        }

        public async Task<IEnumerable<LookupItem>> GetPhotoFromBasedOnPageSizeAsync(int from, int pageSize)
        {
            using (var context = _contextCreator())
            {
                return await context.Photos.AsNoTracking().OrderBy(o => o.Id).Skip(from).Take(pageSize)
                    .Select(p =>
                    new LookupItem
                    {
                        Id = p.Id,
                        DisplayMemberItem = p.FullPath
                    }).ToListAsync();
            }
        }

        public async Task<IEnumerable<LookupItem>> GetYearLookupAsync()
        {
            using (var context = _contextCreator())
            {
                return await context.Years.AsNoTracking()
                    .Select(p =>
                    new LookupItem
                    {
                        Id = p.Id,
                        DisplayMemberItem = p.PhotoTakenYear.ToString()
                    }).ToListAsync();
            }
        }

        public async Task<List<LookupItem>> GetAlbumLookupAsync()
        {
            using (var context = _contextCreator())
            {
                var items = await context.Albums.AsNoTracking()
                    .Select(a =>
                    new LookupItem
                    {
                        Id = a.Id,
                        DisplayMemberItem = a.Title
                    }).ToListAsync();
                return items;
            }
        }

        public async Task<int> GetPhotoCountAsync()
        {
            using (var context = _contextCreator())
            {
                return await context.Photos.AsNoTracking().CountAsync();
            }
        }
    }
}
