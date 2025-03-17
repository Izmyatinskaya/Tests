using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wpf_тесты_для_обучения.Properties;

namespace wpf_тесты_для_обучения
{
    public class Results
    {
        public int Id { get; set; } // Уникальный идентификатор роли
        public int UserId { get; set; }
        public int TestId { get; set; }
        private double _score;

        public double Score
        {
            get => Math.Round(_score, 2);
            set => _score = value;
        }

        public double Percent { 
            get {
                double percent = Math.Round(Score/Test.Count * 100, 2);
                return percent; 
            }}
        public Tests Test { get; set; }
        public Users User { get; set; }
        public int RowNumber { get; set; }
        public Results() { }

        public Results(int id, int userId, int testId, double score)
        {
            Id = id;
            UserId = userId;
            TestId = testId;
            Score = score;
        }
    }
}
