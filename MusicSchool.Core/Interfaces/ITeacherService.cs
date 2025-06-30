using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MusicSchool.Core.DTOs.Inscriptions;
using MusicSchool.Core.DTOs.Teacher;
using MusicSchool.Core.Entities;

namespace MusicSchool.Core.Interfaces
{
    public interface ITeacherService
    {
        Task<IEnumerable<TeacherDto>> GetAllAsync();
        Task<TeacherDetailsDto?> GetByIdAsync(int id);
        Task<TeacherDto> CreateAsync(TeacherCreateDto dto);
        Task<bool> UpdateAsync(TeacherUpdateDto dto);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<TeacherDto>> GetBySchoolIdAsync(int schoolId);
        Task<int> AssignStudentsAsync(AssignStudentsDto dto);
        Task<int> RemoveStudentsAsync(AssignStudentsDto dto);
    }
}
