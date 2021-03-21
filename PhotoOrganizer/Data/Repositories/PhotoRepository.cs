using PhotoOrganizer.Common;
using PhotoOrganizer.DataAccess;
using PhotoOrganizer.Model;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;

namespace PhotoOrganizer.UI.Data.Repositories
{
    public class PhotoRepository : GenericRepository<Photo, PhotoOrganizerDbContext>, IPhotoRepository
    {
        public PhotoRepository(PhotoOrganizerDbContext context) : base(context)
        {
        }

        public async Task<List<Photo>> GetModifiedPhotosAsync()
        {
            return await Context.Photos.Where(p => p.ColorFlag == ColorSign.Modified).ToListAsync();
        }

        public override async Task<Photo> GetByIdAsync(int photoId)
        {
            return await Context.Photos
                .Include(p => p.Peoples)
                .SingleAsync(p => p.Id == photoId);
        }

        public async Task<bool> HasAlbums(int photoId)
        {
            return await Context.Albums.AsNoTracking()
                .Include(a => a.Photos)
                .AnyAsync(a => a.Photos.Any(p => p.Id == photoId));
        }

        public void RemovePeople(People model)
        {
            Context.People.Remove(model);
        }

        public async Task<bool> HasPhotosAsync()
        {
            return await Context.Photos.CountAsync() != 0 ? true : false;
        }

        public void AddRange(Photo[] photos)
        {
            Context.Photos.AddRange(photos);
        }

        public async Task AddRangeAsync(Photo[] photos)
        {
            using (var scope = new TransactionScope())
            {
                PhotoOrganizerDbContext context = null;
                try
                {
                    context = new PhotoOrganizerDbContext();
                    context.Configuration.AutoDetectChangesEnabled = false;

                    int count = 0;
                    foreach (var entityToInsert in photos)
                    {
                        ++count;
                        context = AddToContext(context, entityToInsert, count, 100, true);
                    }

                    await context.SaveChangesAsync();
                }
                finally
                {
                    if (context != null)
                        context.Dispose();
                }

                scope.Complete();
            }
        }

        public async Task<int?> GetMaxPhotoIdAsync()
        {
            return await Context.Photos.MaxAsync(p => (int?)p.Id);
        }

        public async Task RemoveAllPhotoFromTableAsync()
        {
            var photos = await GetAllAsync();
            foreach (var photo in photos)
            {
                Context.Photos.Remove(photo);
            }

            await SaveAsync();
        }

        private PhotoOrganizerDbContext AddToContext(PhotoOrganizerDbContext context,
            Photo entity, int count, int commitCount, bool recreateContext)
        {
            context.Photos.Add(entity);

            if (count % commitCount == 0)
            {
                context.SaveChanges();
                if (recreateContext)
                {
                    context.Dispose();
                    context = new PhotoOrganizerDbContext();
                    context.Configuration.AutoDetectChangesEnabled = false;
                }
            }

            return context;
        }

        public async Task<People> AddGetPeopleByUniqueDisplayNameAsync(string displayName)
        {
            var people = await TryGetAnyPeopleByDisplayName(displayName);
            if (people == null)
            {
                people = new People { DisplayName = displayName };
                Context.People.Add(people);
            }

            return people;
        }

        public async Task<bool> HasPeopleDisplayName(string displayName)
        {
            return await Context.People.AnyAsync(p => p.DisplayName == displayName);
        }

        public async Task<People> TryGetAnyPeopleByDisplayName(string displayName)
        {
            return await Context.People.SingleOrDefaultAsync(p => p.DisplayName == displayName);
        }

        public void AddPeople(People model)
        {
            Context.People.Add(model);
        }

        public async Task<List<People>> GetAllPeopleAsync()
        {
            return await Context.People.ToListAsync();
        }

        public virtual async Task<People> GetPeopleByIdAsync(int id)
        {
            return await Context.People.FindAsync(id);
        }

        public async Task AddPhotoToShelveAsync(Photo photo)
        {
            var shelve = GetShelve();
            shelve.Photos.Add(photo);
            await Context.SaveChangesAsync();
        }

        public List<Photo> GetAllPhotoOfShelve()
        {
            var shelve = GetShelve();
            return shelve.Photos.ToList();
        }

        public bool IsPhotoExistOnShelve(int photoId)
        {
            var shelve = GetShelve();
            return shelve.Photos.Any(p => p.Id == photoId);
        }

        public async Task ReloadPhotoAsync(int photoId)
        {
            var dbEntitiyEntry = Context.ChangeTracker.Entries<Photo>()
                .SingleOrDefault(db => db.Entity.Id == photoId);
            if (dbEntitiyEntry != null)
            {
                await dbEntitiyEntry.ReloadAsync();
            }
        }

        public async Task RemovePhotoToShelveAsync(Photo photo)
        {
            var shelve = GetShelve();
            shelve.Photos.Remove(photo);
            await Context.SaveChangesAsync();
        }

        private Shelve GetShelve()
        {
            return Context.Shelves.FirstOrDefault(s => s.Id > -1);            
        }
    }
}