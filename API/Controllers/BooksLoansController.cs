using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class BooksLoansController : ControllerBase
    {
        private readonly IBooksLoansRepository _booksLoansRepository;
        private readonly IBookRepository _bookRepository;
        public BooksLoansController(IBooksLoansRepository booksLoansRepository, IBookRepository bookRepository)
        {
            _booksLoansRepository = booksLoansRepository;
            _bookRepository = bookRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Book>>> GetAll()
        {
            var booksLoans = await _booksLoansRepository.GetAllAsync();
            return Ok(booksLoans);
        }

        [HttpPost("new")]
        public async Task<ActionResult> NewBookLoan(int bookId, int personId, DateTime? returnDate)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return BadRequest(new { Message = "Can't find operation user" });
            var book = await _bookRepository.FindByIdAsync(bookId);
            if (book is not null && book.Active && !book.Deleted && book.AvaibleCopies > 0 && book.Copies > 0)
            {
                var bookLoan = new BookLoan { BookId = bookId, PersonId = personId, LoanDate = DateTime.Now, ReturnDate = returnDate, WhoLent = userId, Active = true, Deleted = false };
                await _booksLoansRepository.Add(bookLoan);
                await _bookRepository.UpdateAvaibleCopiesAsync(bookId, book.AvaibleCopies - 1);
                return Ok(bookLoan);
            }
            return BadRequest(new { Message = "The book is not avaible." });
        }

        [HttpDelete("remove/{id}")]
        public async Task<ActionResult> DeleteBookLoan(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return BadRequest(new { Message = "Can't find operation user" });
            var bookLoan = await _booksLoansRepository.FindByIdAsync(id);
            if (bookLoan is not null)
            {
                await _bookRepository.UpdateAvaibleCopiesAsync(bookLoan.BookId, bookLoan.Book.AvaibleCopies + 1);
                await _booksLoansRepository.DeleteAsync(id, userId);
                return Ok();
            }
            return BadRequest(new { Message = "Can't find loan" });
        }

        [HttpPatch("return/{id}")]
        public async Task<ActionResult> ReturnBook(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return BadRequest(new { Message = "Can't find operation user" });
            await _booksLoansRepository.ReturnBookLoan(id, userId);
            return Ok();
        }
    }
}
