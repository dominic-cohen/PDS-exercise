using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UKParliament.CodeTest.Data;

namespace UKParliament.CodeTest.Data.Repositories
{
    public class PersonRepository : IPersonRepository
    {
        private readonly PersonManagerContext _context;

        public PersonRepository(PersonManagerContext context)
        {
            _context = context;
        }

        public async Task<int> AddPersonAsync(Person person)
        {
            if (person == null)
            {
                throw new ArgumentNullException(nameof(person));
            }
            await _context.People.AddAsync(person);
            return person.Id;
        }

        public async Task<Person?> GetPersonByIdAsync(int id)
        {
            return await _context.People.FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<Person>> GetAllPeopleAsync()
        {
            return await _context.People
                .OrderBy(p => p.LastName)
                .ThenBy(p => p.FirstName)
                .ToListAsync();
        }

        public async Task<IEnumerable<Department>> GetAllDepartmentsAsync()
        {
            return await _context.Departments.ToListAsync();
        }

        public async Task DeletePersonAsync(int id)
        {
            var person = await _context.People.FirstOrDefaultAsync(p => p.Id == id);
            if (person == null)
            {
                throw new InvalidOperationException($"Person with ID {id} does not exist.");
            }
            _context.People.Remove(person);
        }

        public async Task SaveChangesAsync()
        {
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                throw new InvalidOperationException("An error occurred while saving changes to the database.", ex);
            }
        }
    }
}
