using API.Models;
using Application.Interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class PersonsController : ControllerBase
    {
        private readonly IPersonRepository _personRepository;
        public PersonsController(IPersonRepository personRepository) 
        {
            _personRepository = personRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Person>>> GetAll()
        {
            var persons = await _personRepository.GetAllAsync();
            return Ok(persons);
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
            var lsPersons = await _personRepository.GetMatchPersonAsync(searchedText: query);
            if (lsPersons is null || lsPersons.Count() == 0) goto SendResponse;
            result = lsPersons.Select(x => new {
                value = x.Id.ToString(),
                label = x.FullName
            }).ToList();

            SendResponse:
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<Person>>> Get(int id)
        {
            var person = await _personRepository.FindByIdAsync(id);
            if (person is null)
            {
                return BadRequest(new { Message = "Couldn't find person." });
            }
            return Ok(person);
        }

        [HttpPost("add")]
        public async Task<ActionResult> Add([FromBody] PersonModel person)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    Message = "Entries are required.",
                    Errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage))
                });
            }
            var newPerson = new Person { FullName = person.FullName, Email = person.Email, CellPhone = person.CellPhone, Active = true, Deleted = false };
            await _personRepository.AddAsync(newPerson);
            return Ok();
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, [FromBody] PersonModel person)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    Message = "Entries are required.",
                    Errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage))
                });
            }
            var oldPerson = new Person { Id = id, FullName = person.FullName, Email = person.Email, CellPhone = person.CellPhone, Active = true, Deleted = false };
            await _personRepository.UpdateAsync(oldPerson);
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return BadRequest(new { Message = "Can't find operation user" });
            await _personRepository.DeleteAsync(id, userId);
            return Ok();
        }
    }
}
