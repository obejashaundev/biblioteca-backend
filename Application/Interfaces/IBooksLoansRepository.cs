using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IBooksLoansRepository
    {
        Task<IEnumerable<BookLoan>> GetAllAsync();
        Task<BookLoan> FindByIdAsync(int id);
        Task Add(BookLoan bookLoan);
        Task DeleteAsync(int id, string userId);
        Task UpdateReturnDateAsync(int id, DateTime? returnDate);
        Task ReturnBookLoan(int id, string userId);
    }
}
