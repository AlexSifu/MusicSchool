using Microsoft.AspNetCore.Mvc;
using MusicSchool.Core.Entities;
using MusicSchool.Core.Interfaces;

namespace MusicSchool.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SchoolsController : Controller
    {
        private readonly ISchoolService _schoolService;

        public SchoolsController(ISchoolService schoolService)
        {
            _schoolService = schoolService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var students = await _schoolService.GetAllAsync();
            return Ok(students);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var school = await _schoolService.GetByIdAsync(id);
            if (school == null)
                return NotFound();

            return Ok(school);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] School school)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var created = await _schoolService.CreateAsync(school);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] School school)
        {
            if (id != school.Id)
                return BadRequest("ID mismatch");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var success = await _schoolService.UpdateAsync(school);
            return success ? NoContent() : NotFound();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            Console.WriteLine("Eliminando");
            var success = await _schoolService.DeleteAsync(id);
            return success ? NoContent() : NotFound();
        }
    }
}
