using PhotoOrganizer.DataAccess;
using PhotoOrganizer.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Threading.Tasks;

namespace PhotoOrganizer.UI.Data.Repositories
{
    public class PeopleRepository : GenericRepository<People, PhotoOrganizerDbContext>, IPeopleRepository
    {
        public PeopleRepository(PhotoOrganizerDbContext context) : base(context)
        {
        }

        public void AddAlias(Alias model)
        {
            throw new NotImplementedException();
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

        public Task<Alias> GetAliasByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<List<Alias>> GetAllAliasAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<bool> HasPeopleDisplayName(string displayName)
        {
            return await Context.People.AnyAsync(p => p.DisplayName == displayName);
        }

        public async Task<People> TryGetAnyPeopleByDisplayName(string displayName)
        {
            return await Context.People.SingleOrDefaultAsync(p => p.DisplayName == displayName);
        }
    }
}