using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MusicSchool.Core.Entities;
using MusicSchool.Core.Interfaces;
using MusicSchool.Infrastructure.Data;

namespace MusicSchool.Infrastructure.Services
{
    public class SchoolService : ISchoolService
    {
        private readonly AppDbContext _context;

        public SchoolService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<School>> GetAllAsync()
        {
            return await _context.Schools.ToListAsync();
        }

        public async Task<School?> GetByIdAsync(int id)
        {
            return await _context.Schools.FindAsync(id);
        }

        public async Task<School> CreateAsync(School school)
        {
            using (var connection = _context.Database.GetDbConnection())
            {
                await connection.OpenAsync();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SP_CreateSchool";
                    command.CommandType = CommandType.StoredProcedure;

                    // Parámetros
                    var codeParam = command.CreateParameter();
                    codeParam.ParameterName = "@Code";
                    codeParam.Value = school.Code;
                    command.Parameters.Add(codeParam);

                    var nameParam = command.CreateParameter();
                    nameParam.ParameterName = "@Name";
                    nameParam.Value = school.Name;
                    command.Parameters.Add(nameParam);

                    var descParam = command.CreateParameter();
                    descParam.ParameterName = "@Description";
                    descParam.Value = (object?)school.Description ?? DBNull.Value;
                    command.Parameters.Add(descParam);

                    var createdAtParam = command.CreateParameter();
                    createdAtParam.ParameterName = "@CreatedAt";
                    createdAtParam.Value = school.CreatedAt;
                    command.Parameters.Add(createdAtParam);

                    var updatedAtParam = command.CreateParameter();
                    updatedAtParam.ParameterName = "@UpdatedAt";
                    updatedAtParam.Value = school.UpdatedAt;
                    command.Parameters.Add(updatedAtParam);

                    // Ejecutar SP y capturar ID generado
                    var result = await command.ExecuteScalarAsync();
                    school.Id = Convert.ToInt32(result);

                    return school;
                }
            }
        }

        public async Task<bool> UpdateAsync(School school)
        {
            using (var connection = _context.Database.GetDbConnection())
            {
                await connection.OpenAsync();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SP_UpdateSchool";
                    command.CommandType = CommandType.StoredProcedure;

                    // Parámetros
                    var idParam = command.CreateParameter();
                    idParam.ParameterName = "@Id";
                    idParam.Value = school.Id;
                    command.Parameters.Add(idParam);

                    var codeParam = command.CreateParameter();
                    codeParam.ParameterName = "@Code";
                    codeParam.Value = school.Code;
                    command.Parameters.Add(codeParam);

                    var nameParam = command.CreateParameter();
                    nameParam.ParameterName = "@Name";
                    nameParam.Value = school.Name;
                    command.Parameters.Add(nameParam);

                    var descParam = command.CreateParameter();
                    descParam.ParameterName = "@Description";
                    descParam.Value = (object?)school.Description ?? DBNull.Value;
                    command.Parameters.Add(descParam);

                    var updatedAtParam = command.CreateParameter();
                    updatedAtParam.ParameterName = "@UpdatedAt";
                    updatedAtParam.Value = DateTime.UtcNow;
                    command.Parameters.Add(updatedAtParam);

                    // Ejecutar SP y verificar cuántas filas se actualizaron
                    var result = await command.ExecuteScalarAsync();
                    var affectedRows = Convert.ToInt32(result);

                    return affectedRows > 0;
                }
            }
        }


        public async Task<bool> DeleteAsync(int id)
        {
            var school = await _context.Schools.FindAsync(id);
            if (school == null)
                return false;

            _context.Schools.Remove(school);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
