using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
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

        // Helper para crear parámetros
        private DbParameter CreateParameter(DbCommand command, string name, object value)
        {
            var param = command.CreateParameter();
            param.ParameterName = name;
            param.Value = value ?? DBNull.Value;
            return param;
        }

        public async Task<StudentDto> CreateAsync(StudentCreateDto dto)
        {
            // Recuperar nombre de la escuela para el DTO
            var school = await _context.Schools.FindAsync(dto.SchoolId);

            var createdAt = DateTime.UtcNow;

            int newStudentId;

            using (var connection = _context.Database.GetDbConnection())
            {
                await connection.OpenAsync();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SP_CreateStudent";
                    command.CommandType = CommandType.StoredProcedure;

                    // Parámetros
                    var parameters = new[]
                    {
                        CreateParameter(command, "@FirstName", dto.FirstName),
                        CreateParameter(command, "@LastName", dto.LastName),
                        CreateParameter(command, "@BirthDate", dto.BirthDate),
                        CreateParameter(command, "@IdentificationNumber", dto.IdentificationNumber),
                        CreateParameter(command, "@SchoolId", dto.SchoolId),
                        CreateParameter(command, "@CreatedAt", createdAt),
                        CreateParameter(command, "@UpdatedAt", createdAt)
                    };

                    foreach (var param in parameters)
                        command.Parameters.Add(param);

                    // Ejecutar SP
                    var result = await command.ExecuteScalarAsync();
                    newStudentId = Convert.ToInt32(result);
                }
            }

            return new StudentDto
            {
                Id = newStudentId,
                FullName = $"{dto.FirstName} {dto.LastName}",
                IdentificationNumber = dto.IdentificationNumber,
                BirthDate = dto.BirthDate,
                SchoolName = school?.Name
            };
        }

        public async Task<bool> UpdateAsync(StudentUpdateDto dto)
        {
            using (var connection = _context.Database.GetDbConnection())
            {
                await connection.OpenAsync();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SP_UpdateStudent";
                    command.CommandType = CommandType.StoredProcedure;

                    var now = DateTime.UtcNow;

                    var parameters = new[]
                    {
                        CreateParameter(command, "@Id", dto.Id),
                        CreateParameter(command, "@FirstName", dto.FirstName),
                        CreateParameter(command, "@LastName", dto.LastName),
                        CreateParameter(command, "@BirthDate", dto.BirthDate),
                        CreateParameter(command, "@IdentificationNumber", dto.IdentificationNumber),
                        CreateParameter(command, "@SchoolId", dto.SchoolId),
                        CreateParameter(command, "@UpdatedAt", now)
                    };

                    foreach (var param in parameters)
                        command.Parameters.Add(param);

                    try
                    {
                        var result = await command.ExecuteScalarAsync();
                        return Convert.ToInt32(result) == 1;
                    }
                    catch (SqlException)
                    {
                        // Errores personalizados aquí
                        return false;
                    }
                }
            }
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
