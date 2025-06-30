using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MusicSchool.Core.DTOs.Student;
using MusicSchool.Core.Entities;

namespace MusicSchool.Core.Interfaces
{
    public interface IStudentService
    {
        Task<IEnumerable<StudentDto>> GetAllAsync();
        Task<StudentDetailsDto?> GetByIdAsync(int id);
        Task<StudentDto> CreateAsync(StudentCreateDto dto);
        Task<bool> UpdateAsync(StudentUpdateDto dto);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<StudentDto>> GetBySchoolIdAsync(int schoolId);
        Task<IEnumerable<StudentDto>> GetStudentsByTeacherIdAsync(int teacherId);
    }
}
