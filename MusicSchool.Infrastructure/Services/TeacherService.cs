using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MusicSchool.Core.DTOs.Inscriptions;
using MusicSchool.Core.DTOs.Student;
using MusicSchool.Core.DTOs.Teacher;
using MusicSchool.Core.Entities;
using MusicSchool.Core.Interfaces;
using MusicSchool.Infrastructure.Data;

namespace MusicSchool.Infrastructure.Services
{
    public class TeacherService : ITeacherService
    {
        private readonly AppDbContext _context;

        public TeacherService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TeacherDto>> GetAllAsync()
        {
            return await _context.Teachers
                .Include(s => s.SchoolTeachers)
                    .ThenInclude(ss => ss.School)
                .Select(s => new TeacherDto
                {
                    Id = s.Id,
                    FullName = $"{s.FirstName} {s.LastName}",
                    IdentificationNumber = s.IdentificationNumber,
                    SchoolName = s.SchoolTeachers
                        .Select(ss => ss.School.Name)
                        .FirstOrDefault()
                })
                .ToListAsync();
        }

        public async Task<TeacherDetailsDto?> GetByIdAsync(int id)
        {
            return await _context.Teachers
                .Include(s => s.SchoolTeachers)
                    .ThenInclude(ss => ss.School)
                .Where(s => s.Id == id)
                .Select(s => new TeacherDetailsDto
                {
                    Id = s.Id,
                    FirstName = s.FirstName,
                    LastName = s.LastName,
                    IdentificationNumber = s.IdentificationNumber,
                    SchoolId = s.SchoolTeachers
                        .Select(ss => ss.SchoolId)
                        .FirstOrDefault(),
                    SchoolName = s.SchoolTeachers
                        .Select(ss => ss.School.Name)
                        .FirstOrDefault()
                })
                .FirstOrDefaultAsync();
        }

        public async Task<TeacherDto> CreateAsync(TeacherCreateDto dto)
        {
            var teacher = new Teacher
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                IdentificationNumber = dto.IdentificationNumber
            };

            teacher.SchoolTeachers.Add(new SchoolTeacher
            {
                SchoolId = dto.SchoolId
            });

            _context.Teachers.Add(teacher);
            await _context.SaveChangesAsync();

            // Retorna DTO limpio
            var school = await _context.Schools.FindAsync(dto.SchoolId);
            return new TeacherDto
            {
                Id = teacher.Id,
                FullName = $"{teacher.FirstName} {teacher.LastName}",
                IdentificationNumber = teacher.IdentificationNumber,
                SchoolName = school?.Name
            };
        }

        public async Task<bool> UpdateAsync(TeacherUpdateDto dto)
        {
            var student = await _context.Teachers
                .Include(s => s.SchoolTeachers)
                .FirstOrDefaultAsync(s => s.Id == dto.Id);

            if (student == null)
                return false;

            student.FirstName = dto.FirstName;
            student.LastName = dto.LastName;
            student.IdentificationNumber = dto.IdentificationNumber;
            student.UpdatedAt = DateTime.UtcNow;

            var currentRelation = student.SchoolTeachers.FirstOrDefault();
            if (currentRelation != null && currentRelation.SchoolId != dto.SchoolId)
            {
                student.SchoolTeachers.Remove(currentRelation);
                student.SchoolTeachers.Add(new SchoolTeacher
                {
                    SchoolId = dto.SchoolId,
                });
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var teacher = await _context.Teachers.FindAsync(id);
            if (teacher == null)
                return false;

            _context.Teachers.Remove(teacher);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<TeacherDto>> GetBySchoolIdAsync(int schoolId)
        {
            return await _context.SchoolTeachers
                .Where(st => st.SchoolId == schoolId)
                .Include(st => st.Teacher)
                .Select(st => new TeacherDto
                {
                    Id = st.Teacher.Id,
                    FullName = st.Teacher.FirstName + " " + st.Teacher.LastName
                })
                .ToListAsync();
        }

        public async Task<int> AssignStudentsAsync(AssignStudentsDto dto)
        {
            // Obtener los IDs de estudiantes ya asignados a ese profesor
            var existingAssignments = await _context.TeacherStudents
                .Where(ts => ts.TeacherId == dto.TeacherId && dto.StudentIds.Contains(ts.StudentId))
                .Select(ts => ts.StudentId)
                .ToListAsync();

            // Filtrar los IDs que aún no están asignados
            var newAssignments = dto.StudentIds
                .Except(existingAssignments)
                .Select(studentId => new TeacherStudent
                {
                    TeacherId = dto.TeacherId,
                    StudentId = studentId,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });

            // Insertar solo los nuevos
            await _context.TeacherStudents.AddRangeAsync(newAssignments);
            var inserted = await _context.SaveChangesAsync();

            return inserted; // cantidad de registros nuevos insertados
        }

        public async Task<int> RemoveStudentsAsync(AssignStudentsDto dto)
        {
            // Buscar las asignaciones existentes que coinciden
            var toRemove = await _context.TeacherStudents
                .Where(ts => ts.TeacherId == dto.TeacherId && dto.StudentIds.Contains(ts.StudentId))
                .ToListAsync();

            if (!toRemove.Any())
                return 0;

            // Eliminar las asignaciones existentes
            _context.TeacherStudents.RemoveRange(toRemove);
            var removed = await _context.SaveChangesAsync();

            return removed; // cantidad de filas eliminadas
        }


    }
}
