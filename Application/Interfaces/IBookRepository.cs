using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IBookRepository
    {
        Task AddAsync(Book book);
        Task DeleteAsync(int id, string userId);
        Task<IEnumerable<Book>> GetAllAsync();
        Task<Book> FindByIdAsync(int id);
    }
}
