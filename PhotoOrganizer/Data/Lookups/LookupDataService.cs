﻿using PhotoOrganizer.DataAccess;
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

        public async Task<IEnumerable<LookupItem>> GetPhotoLookupAsync()
        {
            using (var context = _contextCreator())
            {
                return await context.Photos.AsNoTracking()
                    .Select(p =>
                    new LookupItem
                    {
                        Id = p.Id,
                        DisplayMemberItem = p.Title
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
    }
}
