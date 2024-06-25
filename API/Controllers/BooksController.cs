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

        [HttpPost]
        public async Task<ActionResult> Add(Book book)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    Message = "Entries are required.",
                    Errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage))
                });
            }
            book.Active = true;
            book.Deleted = false;
            await _bookRepository.AddAsync(book);
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
