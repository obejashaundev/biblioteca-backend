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
    public class BooksController : ControllerBase
    {
        private readonly IBookRepository _bookRepository;
        public BooksController(IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Book>>> GetAll()
        {
            var books = await _bookRepository.GetAllAsync();
            return Ok(books);
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<Book>>> GetList(string query)
        {
            var result = Enumerable.Empty<dynamic>();
            query = API.Utilities.FormatString(query);
            if (string.IsNullOrEmpty(query) || query.Length < 3)
            {
                goto SendResponse;
            }
            var lsBooks = await _bookRepository.GetMatchBookAsync(searchedText: query);
            if (lsBooks is null || lsBooks.Count() == 0) goto SendResponse;
            result = lsBooks.Select(x => new
            {
                value = x.Id.ToString(),
                label = x.Title
            }).ToList();
        SendResponse:
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<Book>>> Get(int id)
        {
            var book = await _bookRepository.FindByIdAsync(id);
            if (book is null)
            {
                return BadRequest(new { Message = "Couldn't find book." });
            }
            return Ok(book);
        }

        [HttpPost("add")]
        public async Task<ActionResult> Add([FromBody] BookModel book)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    Message = "Entries are required.",
                    Errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage))
                });
            }
            var newBook = new Book { Title = book.Title, Author = book.Author, Copies = book.Copies, AvaibleCopies = book.Copies, Active = true, Deleted = false };
            await _bookRepository.AddAsync(newBook);
            return Ok();
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, [FromBody] BookModel book)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    Message = "Entries are required.",
                    Errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage))
                });
            }
            var oldBook = new Book { Id = id, Title = book.Title, Author = book.Author, Copies = book.Copies, AvaibleCopies = book.AvaibleCopies, Active = true, Deleted = false };
            await _bookRepository.UpdateAsync(oldBook);
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return BadRequest(new { Message = "Can't find operation user" });
            await _bookRepository.DeleteAsync(id, userId);
            return Ok();
        }
    }
}
