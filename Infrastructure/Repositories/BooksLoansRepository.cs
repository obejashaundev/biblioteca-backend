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
    public class BooksLoansRepository : IBooksLoansRepository
    {
        private readonly ApplicationDbContext _db;
        public BooksLoansRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task AddBookLoan(BookLoan bookLoan)
        {
            _db.BookLoans.Add(bookLoan);
            await _db.SaveChangesAsync();
        }

        public async Task<IEnumerable<BookLoan>> GetAllAsync()
        {
            var loansBooks = _db.BookLoans.Where(x => !x.Returned).AsNoTracking().AsEnumerable();
            return loansBooks ?? Enumerable.Empty<BookLoan>();
        }

        public async Task ReturnBookLoan(int id, string userId)
        {
            var bookLoan = await _db.BookLoans.FirstOrDefaultAsync(x => x.Id == id && !x.Returned);
            if (bookLoan is not null)
            {
                bookLoan.Returned = true;
                bookLoan.WhoReceived = userId;
                bookLoan.RealReturnDate = DateTime.Now;
                await _db.SaveChangesAsync();
            }
        }
    }
}
