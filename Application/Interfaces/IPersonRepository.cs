using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Domain.Entities;

namespace Application.Interfaces
{
    public interface IPersonRepository
    {
        Task AddAsync(Person person);
        Task DeleteAsync(int id, string userId);
        Task UpdateAsync(Person oldPerson);
        Task<IEnumerable<Person>> GetAllAsync();
        Task<Person> FindByIdAsync(int id);
    }
}
