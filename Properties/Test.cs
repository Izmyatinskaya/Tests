using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wpf_тесты_для_обучения.Properties
{
    public class Test
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;

        // Навигационные свойства
        public List<Question> Questions { get; set; } = new List<Question>();
        public List<Result> Results { get; set; } = new List<Result>();
        public List<RoleAccess> RoleAccesses { get; set; } = new List<RoleAccess>();
    }
}
