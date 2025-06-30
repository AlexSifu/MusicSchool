using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicSchool.Core.DTOs.Inscriptions
{
    public class AssignStudentsDto
    {
        public int TeacherId { get; set; }
        public List<int> StudentIds { get; set; } = new();
    }
}
