using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicSchool.Core.DTOs.Teacher
{
    public class TeacherUpdateDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string IdentificationNumber { get; set; } = string.Empty;
        public int SchoolId { get; set; }
    }
}
