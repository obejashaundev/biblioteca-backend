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
        Task UpdateAsync(Book oldBook);
        Task UpdateAvaibleCopiesAsync(int id, int avaibleCopies);
        Task<IEnumerable<Book>> GetAllAsync();
        Task<IEnumerable<Book>> GetMatchBookAsync(string searchedText);
        Task<Book> FindByIdAsync(int id);
    }
}
