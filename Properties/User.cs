using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wpf_тесты_для_обучения.Properties
{
    public class User
    {
        public int Id { get; set; }
        public string LastName { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Patronymic { get; set; } = string.Empty;
        public int RoleId { get; set; }
        public string Password { get; set; } = string.Empty;

        // Навигационные свойства
        public Role Role { get; set; } // Убрали ?
        public List<Result> Results { get; set; } = new List<Result>();
    }
}
