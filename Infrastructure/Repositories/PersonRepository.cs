using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class PersonRepository : IPersonRepository
    {
        private readonly ApplicationDbContext _db;
        public PersonRepository(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task AddAsync(Person person)
        {
            _db.Persons.Add(person);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id, string userId)
        {
            var person = _db.Persons.FirstOrDefault(x => x.Id == id && x.Active && !x.Deleted);
            if (person is not null)
            {
                person.WhoDeleted = userId;
                person.Deleted = true;
                person.Active = false;
                person.DateDeleted = DateTime.Now;
                await _db.SaveChangesAsync();
            }
        }

        public async Task<Person> FindByIdAsync(int id)
        {
            var person = _db.Persons.AsNoTracking().FirstOrDefault(x => x.Id == id && x.Active && !x.Deleted);
            return person;
        }

        public async Task<IEnumerable<Person>> GetAllAsync()
        {
            var listPersons = _db.Persons.Where(x => x.Active && !x.Deleted).AsNoTracking().AsEnumerable();
            return listPersons ?? Enumerable.Empty<Person>();
        }

        public async Task UpdateAsync(Person oldPerson)
        {
            var person = _db.Persons.FirstOrDefault(x => x.Id == oldPerson.Id && x.Active && !x.Deleted);
            if (person is not null)
            {
                person.FullName = oldPerson.FullName;
                person.Email = oldPerson.Email;
                person.CellPhone = oldPerson.CellPhone;
                await _db.SaveChangesAsync();
            }
        }
    }
}
