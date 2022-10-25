using DAL;
using DAL.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace PersonQueries.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PersonQueriesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PersonQueriesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/PersonQueries
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Person>>> GetPersons()
        {
            return await _context.Persons.ToListAsync();
        }

        // GET: api/PersonQueries/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Person>> GetPerson(int id)
        {
            var person = await _context.Persons.FindAsync(id);

            if (person == null)
            {
                return NotFound();
            }

            return person;
        }
    }
}
