using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicSchool.Core.Entities
{
    public class Teacher
    {
        public int Id { get; set; }

        [Required]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        public string LastName { get; set; } = string.Empty;

        [Required]
        public string IdentificationNumber { get; set; } = string.Empty;

        public ICollection<SchoolTeacher> SchoolTeachers { get; set; } = new List<SchoolTeacher>();

        public ICollection<TeacherStudent> TeacherStudents { get; set; } = new List<TeacherStudent>();

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }

}
