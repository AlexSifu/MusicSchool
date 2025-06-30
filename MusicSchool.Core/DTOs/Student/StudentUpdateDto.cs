using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicSchool.Core.DTOs.Student
{
    public class StudentUpdateDto
    {
        public int Id { get; set; } // Ahora se incluye aquí también
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public DateTime BirthDate { get; set; }
        public string IdentificationNumber { get; set; } = string.Empty;
        public int SchoolId { get; set; }
    }

}
