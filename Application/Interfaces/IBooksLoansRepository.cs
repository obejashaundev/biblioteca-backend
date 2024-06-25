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
        Task AddBookLoan(BookLoan bookLoan);
        Task ReturnBookLoan(int id, string userId);
    }
}
