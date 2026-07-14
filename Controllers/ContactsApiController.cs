using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using PhonebookApp.Models;
using PhonebookApp.Repositories;

namespace PhonebookApp.Controllers
{
    [ApiController]
    [Route("api/contacts")]
    public class ContactsApiController : ControllerBase
    {
        private const int DefaultPageSize = 10;
        private const int MinPageSize = 1;
        private const int MaxPageSize = 100;

        private const int DuplicateKeyError1 = 2601;
        private const int DuplicateKeyError2 = 2627;

        private readonly IContactRepository _repo;

        public ContactsApiController(IContactRepository repo)
        {
            _repo = repo;
        }

        // GET: api/contacts?page=1&pageSize=10&search=
        [HttpGet]
        public async Task<ActionResult<PagedResult<Contact>>> GetAll(int page = 1, int pageSize = DefaultPageSize, string search = null)
        {
            page = Math.Max(page, 1);
            pageSize = Math.Clamp(pageSize, MinPageSize, MaxPageSize);
            search = string.IsNullOrWhiteSpace(search) ? null : search.Trim();

            var result = await _repo.GetContactsPagedAsync(page, pageSize, search);
            return Ok(result);
        }

        // GET: api/contacts/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Contact>> GetById(int id)
        {
            var contact = await _repo.GetContactByIdAsync(id);

            if (contact == null)
            {
                return NotFound();
            }

            return Ok(contact);
        }

        // POST: api/contacts
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Contact contact)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            try
            {
                int newId = await _repo.InsertContactAsync(contact);
                contact.Id = newId;

                return CreatedAtAction(nameof(GetById), new { id = newId }, contact);
            }
            catch (SqlException ex) when (IsDuplicateKeyError(ex))
            {
                return Conflict(new { message = "Phone number already exists." });
            }
        }

        // PUT: api/contacts/5
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] Contact contact)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            contact.Id = id;

            try
            {
                bool success = await _repo.UpdateContactAsync(contact);

                if (!success)
                {
                    return NotFound();
                }

                return NoContent();
            }
            catch (SqlException ex) when (IsDuplicateKeyError(ex))
            {
                return Conflict(new { message = "Phone number already exists." });
            }
        }

        // DELETE: api/contacts/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            bool success = await _repo.DeleteContactAsync(id);

            if (!success)
            {
                return NotFound();
            }

            return NoContent();
        }

        private static bool IsDuplicateKeyError(SqlException ex)
        {
            return ex.Number == DuplicateKeyError1 || ex.Number == DuplicateKeyError2;
        }
    }
}
