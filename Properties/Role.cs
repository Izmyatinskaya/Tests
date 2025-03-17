using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wpf_тесты_для_обучения.Properties
{
    public class Role
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;

        // Навигационные свойства
        public List<User> Users { get; set; } = new List<User>();
        public List<RoleAccess> RoleAccesses { get; set; } = new List<RoleAccess>();
    }
}
