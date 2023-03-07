using PhotoOrganizer.DataAccess;
using PhotoOrganizer.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace PhotoOrganizer.UI.Data.Lookups
{
    public class LookupDataService : IPhotoLookupDataService, ILocationLookupDataService, IAlbumLookupDataService, IShelveLookupDataService
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
                        DisplayMemberItem = p.Title,
                        PhotoPath = p.FullPath,
                        ColorFlag = p.ColorFlag
                    }).ToListAsync();
            }
        }

        public async Task<IEnumerable<LookupItem>> GetLocationLookupAsync()
        {
            using (var context = _contextCreator())
            {
                return await context.Locations.AsNoTracking()
                    .Select(p =>
                    new LookupItem
                    {
                        Id = p.Id,
                        DisplayMemberItem = p.LocationName,
                        Coordinates = p.Coordinates
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

        public List<LookupItem> GetShelveLookup()
        {
            using (var context = _contextCreator())
            {
                var shelve = GetOrCreateShelve(context);

                if(shelve.Photos is null || shelve.Photos.Count == 0)
                {
                    return null;
                }
                return shelve.Photos.OrderBy(p => p.Id)
                    .Select(p =>
                    new LookupItem
                    {
                        Id = p.Id,
                        DisplayMemberItem = p.Title,
                        PhotoPath = p.FullPath,
                        ColorFlag = p.ColorFlag
                    }).ToList();
            }
        }

        private Shelve GetOrCreateShelve(PhotoOrganizerDbContext context)
        {
            var shelve = context.Shelves.FirstOrDefault(s => s.Id > -1);
            if (shelve == null)
            {
                shelve = new Shelve { Id = 0 };
                context.Shelves.Add(shelve);
                context.SaveChanges();
            }

            return shelve;
        }
    }
}
