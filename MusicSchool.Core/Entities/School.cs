using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicSchool.Core.Entities
{
    public class School
    {
        public int Id { get; set; }

        [Required]
        public string Code { get; set; } = string.Empty;

        [Required]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public ICollection<SchoolTeacher> SchoolTeachers { get; set; } = new List<SchoolTeacher>();

        public ICollection<SchoolStudent> SchoolStudents { get; set; } = new List<SchoolStudent>();

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
