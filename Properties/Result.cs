using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wpf_тесты_для_обучения.Properties
{
    public class Result
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int TestId { get; set; }
        public float Score { get; set; }

        // Навигационные свойства
        public User User { get; set; } // Убрали ?
        public Test Test { get; set; } // Убрали ?
    }
}
