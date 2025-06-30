using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MusicSchool.Core.DTOs.Student;
using MusicSchool.Core.Entities;
using MusicSchool.Core.Interfaces;
using MusicSchool.Infrastructure.Data;

namespace MusicSchool.Infrastructure.Services
{
    public class StudentService : IStudentService
    {
        private readonly AppDbContext _context;

        public StudentService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<StudentDto>> GetAllAsync()
        {
            return await _context.Students
                .Include(s => s.SchoolStudents)
                    .ThenInclude(ss => ss.School)
                .Select(s => new StudentDto
                {
                    Id = s.Id,
                    FullName = $"{s.FirstName} {s.LastName}",
                    IdentificationNumber = s.IdentificationNumber,
                    BirthDate = s.BirthDate,
                    SchoolName = s.SchoolStudents
                        .OrderByDescending(ss => ss.EnrollmentDate)
                        .Select(ss => ss.School.Name)
                        .FirstOrDefault()
                })
                .ToListAsync();
        }


        public async Task<StudentDetailsDto?> GetByIdAsync(int id)
        {
            return await _context.Students
                .Include(s => s.SchoolStudents)
                    .ThenInclude(ss => ss.School)
                .Where(s => s.Id == id)
                .Select(s => new StudentDetailsDto
                {
                    Id = s.Id,
                    FirstName = s.FirstName,
                    LastName = s.LastName,
                    BirthDate = s.BirthDate,
                    IdentificationNumber = s.IdentificationNumber,
                    SchoolId = s.SchoolStudents
                        .OrderByDescending(ss => ss.EnrollmentDate)
                        .Select(ss => ss.SchoolId)
                        .FirstOrDefault(),
                    SchoolName = s.SchoolStudents
                        .OrderByDescending(ss => ss.EnrollmentDate)
                        .Select(ss => ss.School.Name)
                        .FirstOrDefault()
                })
                .FirstOrDefaultAsync();
        }


        public async Task<StudentDto> CreateAsync(StudentCreateDto dto)
        {
            var student = new Student
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                BirthDate = dto.BirthDate,
                IdentificationNumber = dto.IdentificationNumber
            };

            student.SchoolStudents.Add(new SchoolStudent
            {
                SchoolId = dto.SchoolId
            });

            _context.Students.Add(student);
            await _context.SaveChangesAsync();

            // Retorna DTO limpio
            var school = await _context.Schools.FindAsync(dto.SchoolId);
            return new StudentDto
            {
                Id = student.Id,
                FullName = $"{student.FirstName} {student.LastName}",
                IdentificationNumber = student.IdentificationNumber,
                BirthDate = student.BirthDate,
                SchoolName = school?.Name
            };
        }

        public async Task<bool> UpdateAsync(StudentUpdateDto dto)
        {
            var student = await _context.Students
                .Include(s => s.SchoolStudents)
                .FirstOrDefaultAsync(s => s.Id == dto.Id);

            if (student == null)
                return false;

            student.FirstName = dto.FirstName;
            student.LastName = dto.LastName;
            student.BirthDate = dto.BirthDate;
            student.IdentificationNumber = dto.IdentificationNumber;
            student.UpdatedAt = DateTime.UtcNow;

            var currentRelation = student.SchoolStudents.FirstOrDefault();
            if (currentRelation != null && currentRelation.SchoolId != dto.SchoolId)
            {
                student.SchoolStudents.Remove(currentRelation);
                student.SchoolStudents.Add(new SchoolStudent
                {
                    SchoolId = dto.SchoolId,
                    EnrollmentDate = DateTime.UtcNow
                });
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student == null)
                return false;

            _context.Students.Remove(student);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<StudentDto>> GetBySchoolIdAsync(int schoolId)
        {
            return await _context.SchoolStudents
                .Where(ss => ss.SchoolId == schoolId)
                .Select(ss => new StudentDto
                {
                    Id = ss.StudentId,
                    FullName = ss.Student.FirstName + " " + ss.Student.LastName,
                    IdentificationNumber = ss.Student.IdentificationNumber,
                    BirthDate = ss.Student.BirthDate,
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<StudentDto>> GetStudentsByTeacherIdAsync(int teacherId)
        {
            return await _context.TeacherStudents
                .Where(ts => ts.TeacherId == teacherId)
                .Select(ts => new StudentDto
                {
                    Id = ts.StudentId,
                    FullName = ts.Student.FirstName + " " + ts.Student.LastName,
                    IdentificationNumber = ts.Student.IdentificationNumber,
                    BirthDate = ts.Student.BirthDate,
                })
                .ToListAsync();
        }
    }
}
