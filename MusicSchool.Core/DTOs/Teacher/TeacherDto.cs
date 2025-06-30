using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicSchool.Core.DTOs.Teacher
{
    public class TeacherDto
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string IdentificationNumber { get; set; } = string.Empty;
        public string? SchoolName { get; set; }
    }
}
