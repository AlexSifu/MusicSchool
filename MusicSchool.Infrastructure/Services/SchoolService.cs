using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MusicSchool.Core.Entities;
using MusicSchool.Core.Interfaces;
using MusicSchool.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

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
            _context.Schools.Add(school);
            await _context.SaveChangesAsync();
            return school;
        }

        public async Task<bool> UpdateAsync(School school)
        {
            var exists = await _context.Schools.AnyAsync(s => s.Id == school.Id);
            if (!exists)
                return false;

            school.UpdatedAt = DateTime.UtcNow;
            _context.Schools.Update(school);
            await _context.SaveChangesAsync();
            return true;
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
