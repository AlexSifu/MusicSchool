using Microsoft.AspNetCore.Mvc;
using MusicSchool.Core.DTOs.Student;
using MusicSchool.Core.Entities;
using MusicSchool.Core.Interfaces;

namespace MusicSchool.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StudentsController : Controller
    {

        private readonly IStudentService _studentService;
        private readonly ISchoolService _schoolService;

        public StudentsController(IStudentService studentService, ISchoolService schoolService)
        {
            _studentService = studentService;
            _schoolService = schoolService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var students = await _studentService.GetAllAsync();
            return Ok(students);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var student = await _studentService.GetByIdAsync(id);
            if (student == null)
                return NotFound();

            return Ok(student);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] StudentCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var student = await _studentService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = student.Id }, student);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] StudentUpdateDto dto)
        {
            if (id != dto.Id)
                return BadRequest("ID mismatch between route and body");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var success = await _studentService.UpdateAsync(dto);
            return success ? NoContent() : NotFound();
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _studentService.DeleteAsync(id);
            return success ? NoContent() : NotFound();
        }

        [HttpGet("Schools")]
        public async Task<IActionResult> GetSchools()
        {
            var schools = await _schoolService.GetAllAsync();
            return Ok(schools);
        }
    }
}
