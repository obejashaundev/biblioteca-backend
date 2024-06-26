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

        public async Task AddAsync(BookLoan bookLoan)
        {
            _db.BookLoans.Add(bookLoan);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id, string userId)
        {
            var bookLoan = _db.BookLoans.FirstOrDefault(x => x.Id == id && !x.Returned);
            if (bookLoan != null)
            {
                bookLoan.WhoDeleted = userId;
                bookLoan.Deleted = true;
                bookLoan.Active = false;
                bookLoan.DateDeleted = DateTime.Now;
                await _db.SaveChangesAsync();
            }
        }

        public async Task<BookLoan> FindByIdAsync(int id)
        {
            var bookLoan = await _db.BookLoans.FirstOrDefaultAsync(x => x.Id == id && x.Active && !x.Deleted && !x.Returned);
            return bookLoan;
        }

        public async Task<IEnumerable<BookLoan>> GetAllAsync()
        {
            var loansBooks = _db.BookLoans.Where(x =>  x.Active && !x.Deleted && !x.Returned).AsNoTracking().AsEnumerable();
            return loansBooks ?? Enumerable.Empty<BookLoan>();
        }

        public async Task<bool> HasAnyCopyAsync(int personId, int bookId)
        {
            return await _db.BookLoans.AnyAsync(x => x.PersonId == personId && x.BookId == bookId && !x.Returned && x.Active && !x.Deleted);
        }

        public async Task ReturnBookLoan(int id, string userId)
        {
            var bookLoan = await _db.BookLoans.FirstOrDefaultAsync(x => x.Id == id && x.Active && !x.Deleted && !x.Returned);
            if (bookLoan is not null)
            {
                bookLoan.Returned = true;
                bookLoan.WhoReceived = userId;
                bookLoan.RealReturnDate = DateTime.Now;
                await _db.SaveChangesAsync();
            }
        }

        public async Task UpdateReturnDateAsync(int id, DateTime? returnDate)
        {
            var bookLoan = await _db.BookLoans.FirstOrDefaultAsync(x => x.Id == id && !x.Returned);
            if (bookLoan is not null)
            {
                bookLoan.ReturnDate = returnDate;
                await _db.SaveChangesAsync();
            }
        }
    }
}
