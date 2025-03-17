using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wpf_тесты_для_обучения.Properties
{
    public class RoleAccess
    {
        public int Id { get; set; }
        public int RoleId { get; set; }
        public int TestId { get; set; }

        // Навигационные свойства
        public Role Role { get; set; } // Убрали ?
        public Test Test { get; set; } // Убрали ?
    }
}
