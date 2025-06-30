using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
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

        private DbParameter CreateParameter(DbCommand command, string name, object value)
        {
            var param = command.CreateParameter();
            param.ParameterName = name;
            param.Value = value ?? DBNull.Value;
            return param;
        }


        public async Task<TeacherDto> CreateAsync(TeacherCreateDto dto)
        {
            var createdAt = DateTime.UtcNow;
            int newTeacherId;

            // Obtener el nombre de la escuela antes de abrir la conexión
            var school = await _context.Schools.FindAsync(dto.SchoolId);

            using (var connection = _context.Database.GetDbConnection())
            {
                await connection.OpenAsync();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SP_CreateTeacher";
                    command.CommandType = CommandType.StoredProcedure;

                    var parameters = new[]
                    {
                        CreateParameter(command, "@FirstName", dto.FirstName),
                        CreateParameter(command, "@LastName", dto.LastName),
                        CreateParameter(command, "@IdentificationNumber", dto.IdentificationNumber),
                        CreateParameter(command, "@SchoolId", dto.SchoolId),
                        CreateParameter(command, "@CreatedAt", createdAt),
                        CreateParameter(command, "@UpdatedAt", createdAt)
                    };

                    foreach (var param in parameters)
                        command.Parameters.Add(param);

                    var result = await command.ExecuteScalarAsync();
                    newTeacherId = Convert.ToInt32(result);
                }
            }

            return new TeacherDto
            {
                Id = newTeacherId,
                FullName = $"{dto.FirstName} {dto.LastName}",
                IdentificationNumber = dto.IdentificationNumber,
                SchoolName = school?.Name
            };
        }


        public async Task<bool> UpdateAsync(TeacherUpdateDto dto)
        {
            using (var connection = _context.Database.GetDbConnection())
            {
                await connection.OpenAsync();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SP_UpdateTeacher";
                    command.CommandType = CommandType.StoredProcedure;

                    var now = DateTime.UtcNow;

                    var parameters = new[]
                    {
                        CreateParameter(command, "@Id", dto.Id),
                        CreateParameter(command, "@FirstName", dto.FirstName),
                        CreateParameter(command, "@LastName", dto.LastName),
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
                        // Puedes loguear el error si deseas
                        return false;
                    }
                }
            }
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
