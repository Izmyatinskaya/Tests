using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wpf_тесты_для_обучения.Properties
{
    public class Question
    {
        public int Id { get; set; }
        public int TestId { get; set; }
        public string Text { get; set; } = string.Empty;
        public string Image { get; set; }

        // Навигационные свойства
        public Test Test { get; set; } // Убрали ?
        public List<Answer> Answers { get; set; } = new List<Answer>();
    }
}
