using API.Models;
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

        [HttpPost("add")]
        public async Task<ActionResult> Add([FromBody] BookLoanModel bookLoan)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    Message = "Entries are required.",
                    Errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage))
                });
            }
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return BadRequest(new { Message = "Can't find operation user" });
            var hasAnyCopy = await _booksLoansRepository.HasAnyCopyAsync(bookLoan.PersonId, bookLoan.BookId);
            if (hasAnyCopy) return BadRequest(new { Message = "The person has a book copy not returned" });
            var book = await _bookRepository.FindByIdAsync(bookLoan.BookId);
            if (book is not null && book.Active && !book.Deleted && book.AvaibleCopies > 0 && book.Copies > 0)
            {
                var newBookLoan = new BookLoan { BookId = bookLoan.BookId, PersonId = bookLoan.PersonId, LoanDate = DateTime.Now, ReturnDate = bookLoan.ReturnDate, WhoLent = userId, Active = true, Deleted = false };
                await _booksLoansRepository.AddAsync(newBookLoan);
                await _bookRepository.UpdateAvaibleCopiesAsync(bookLoan.BookId, book.AvaibleCopies - 1);
                return Ok(bookLoan);
            }
            return BadRequest(new { Message = "The book is not avaible." });
        }

        [HttpDelete("{id}/remove")]
        public async Task<ActionResult> Delete(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return BadRequest(new { Message = "Can't find operation user" });
            var bookLoan = await _booksLoansRepository.FindByIdAsync(id);
            if (bookLoan is not null)
            {
                var book = await _bookRepository.FindByIdAsync(bookLoan.BookId);
                if (book is not null)
                {
                    await _bookRepository.UpdateAvaibleCopiesAsync(bookLoan.BookId, book.AvaibleCopies + 1);
                    await _booksLoansRepository.DeleteAsync(id, userId);
                    return Ok();
                }
            }
            return BadRequest(new { Message = "Can't find loan" });
        }

        [HttpPatch("{id}/return")]
        public async Task<ActionResult> ReturnBook(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return BadRequest(new { Message = "Can't find operation user" });
            await _booksLoansRepository.ReturnBookLoan(id, userId);
            return Ok();
        }

        [HttpPatch("{id}/updateReturnDate")]
        public async Task<ActionResult> UpdateReturnDate(int id, [FromBody] UpdateReturnDateModel model)
        {
            await _booksLoansRepository.UpdateReturnDateAsync(id, model.returnDate);
            return Ok();
        }
    }
}
