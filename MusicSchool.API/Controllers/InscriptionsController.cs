using Microsoft.AspNetCore.Mvc;
using MusicSchool.Core.DTOs.Inscriptions;
using MusicSchool.Core.Interfaces;
using MusicSchool.Infrastructure.Services;

namespace MusicSchool.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InscriptionsController : Controller
    {
        private readonly ITeacherService _teacherService;
        private readonly ISchoolService _schoolService;
        private readonly IStudentService _studentService;

        public InscriptionsController(ITeacherService teacherService, ISchoolService schoolService, IStudentService studentService)
        {
            _teacherService = teacherService;
            _schoolService = schoolService;
            _studentService = studentService;
        }

        [HttpGet("Schools")]
        public async Task<IActionResult> GetSchools()
        {
            var schools = await _schoolService.GetAllAsync();
            return Ok(schools);
        }

        [HttpGet("teachers-by-school/{schoolId}")]
        public async Task<IActionResult> GetTeachersBySchool(int schoolId)
        {
            var teachers = await _teacherService.GetBySchoolIdAsync(schoolId);
            return Ok(teachers);
        }

        [HttpGet("students-by-school/{schoolId}")]
        public async Task<IActionResult> GetStudentsBySchool(int schoolId)
        {
            var students = await _studentService.GetBySchoolIdAsync(schoolId);
            return Ok(students);
        }

        [HttpGet("students-by-teacher/{teacherId}")]
        public async Task<IActionResult> GetStudentsByTeacher(int teacherId)
        {
            var students = await _studentService.GetStudentsByTeacherIdAsync(teacherId);
            return Ok(students);
        }

        [HttpPost("assign-students")]
        public async Task<IActionResult> AssignStudents([FromBody] AssignStudentsDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var count = await _teacherService.AssignStudentsAsync(dto);
            return Ok(new
            {
                message = $"Se asignaron {count} estudiantes nuevos al profesor."
            });
        }

        [HttpPost("remove-students")]
        public async Task<IActionResult> RemoveStudents([FromBody] AssignStudentsDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var count = await _teacherService.RemoveStudentsAsync(dto);
            return Ok(new
            {
                message = $"Se eliminaron {count} asignaciones de estudiantes."
            });
        }

    }
}
