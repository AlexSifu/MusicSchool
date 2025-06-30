using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MusicSchool.Core.Entities;

namespace MusicSchool.Core.Interfaces
{
    public interface ISchoolService
    {
        Task<IEnumerable<School>> GetAllAsync();
        Task<School?> GetByIdAsync(int id);
        Task<School> CreateAsync(School school);
        Task<bool> UpdateAsync(School school);
        Task<bool> DeleteAsync(int id);
    }
}
