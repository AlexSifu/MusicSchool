using Microsoft.AspNetCore.Mvc;
using MusicSchool.Core.DTOs.Teacher;
using MusicSchool.Core.Entities;
using MusicSchool.Core.Interfaces;
using MusicSchool.Infrastructure.Services;

namespace MusicSchool.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TeachersController : Controller
    {
        private readonly ITeacherService _teacherService;
        private readonly ISchoolService _schoolService;

        public TeachersController(ITeacherService teacherService, ISchoolService schoolService)
        {
            _teacherService = teacherService;
            _schoolService = schoolService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var students = await _teacherService.GetAllAsync();
            return Ok(students);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var teacher = await _teacherService.GetByIdAsync(id);
            if (teacher == null)
                return NotFound();

            return Ok(teacher);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] TeacherCreateDto teacher)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var created = await _teacherService.CreateAsync(teacher);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] TeacherUpdateDto teacher)
        {
            if (id != teacher.Id)
                return BadRequest("ID mismatch");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var success = await _teacherService.UpdateAsync(teacher);
            return success ? NoContent() : NotFound();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _teacherService.DeleteAsync(id);
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
