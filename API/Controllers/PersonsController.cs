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

        [HttpPost]
        public async Task<ActionResult> Add(Person person)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    Message = "Entries are required.",
                    Errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage))
                });
            }
            person.Active = true;
            person.Deleted = false;
            await _personRepository.AddAsync(person);
            return Ok();
        }

        [HttpPut]
        public async Task<ActionResult> Update(Person person)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    Message = "Entries are required.",
                    Errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage))
                });
            }
            await _personRepository.UpdateAsync(person);
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
