using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows;
using System.Xml.Linq;
using wpf_тесты_для_обучения.Properties;
using System.ComponentModel;

namespace wpf_тесты_для_обучения
{
    public class Tests: INotifyPropertyChanged
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string FullTitle => $"{Id}. {Title}";
        public double IsCompleted { get; set; }
        public int Count {
            get
            {
                string query = $"Select count(Id) from Questions where Test_Id = {Id}";
                DatabaseHelper databaseHelper = new DatabaseHelper("Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=\"D:\\Проекты\\Тесты обучение WPF\\wpf тесты для обучения\\DB.mdf\";Integrated Security=True");
                int count = (int)databaseHelper.ExecuteScalar(query);
                return count;
            }
            }
        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    OnPropertyChanged(nameof(IsSelected));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        public Tests() { }

        public Tests(int id, string name, double isCompleted)
        {
            Id = id;
            Title = name;
            IsCompleted = isCompleted;
        }
        public List<Questions> Questions { get; set; } = new List<Questions>();
        // Метод для добавления вопроса в тест
        public void AddQuestion(Questions question)
        {
            if (question != null && question.TestId == Id)
            {
                Questions.Add(question);
            }
        }

        public void LoadQuestionsFromDatabase()
        {
            // Запрос для извлечения всех вопросов, связанных с текущим тестом
            string query = $"SELECT q.Id, q.Test_Id, q.Question_Text, q.Image," +
                $"CASE WHEN (SELECT COUNT(*) FROM Answers a WHERE a.Question_Id = q.Id AND a.Is_Correct = 1) > 1 " +
                $"THEN 1 ELSE 0 END AS IsMultipleAnswers FROM Questions q WHERE q.Test_Id = {Id}";

            // Создаем подключение и выполняем запрос
            DatabaseHelper databaseHelper = new DatabaseHelper("Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=\"D:\\Проекты\\Тесты обучение WPF\\wpf тесты для обучения\\DB.mdf\";Integrated Security=True");
            DataTable dataTable = databaseHelper.ExecuteSelectQuery(query);
            Questions.Clear();
            // Проходим по всем строкам DataTable и создаем экземпляры Questions
            foreach (DataRow row in dataTable.Rows)
            {
                // Создаем новый объект Questions и заполняем его данными из строки
                Questions question = new Questions(
                    id: Convert.ToInt32(row["Id"]),
                    testId: Convert.ToInt32(row["Test_Id"]),
                    questionText: row["Question_Text"].ToString(),
                    image: row["Image"].ToString(),
                    isMultiple: Convert.ToBoolean(row["IsMultipleAnswers"])
                );

                // Добавляем вопрос в список
                
                Questions.Add(question);
            }
        }

    }

    public class Questions
    {
        public int Id { get; set; }
        public int TestId { get; set; }
        public string QuestionText { get; set; }
        public string Image { get; set; }
        public List<Answers> Answers { get; set; } = new List<Answers>();
        public bool IsMultiple { get; set; }
        public string FullTitle => $"{Id}. {QuestionText}";
        public Questions() { }

        public Questions(int id, int testId, string questionText, string image, bool isMultiple=false)
        {
            Id = id;
            TestId = testId;
            QuestionText = questionText;
            Image = image;
            IsMultiple = isMultiple;
        }

        // Метод для добавления ответа к вопросу
        public void AddAnswer(Answers answer)
        {
            if (answer != null && answer.QuestionId == Id)
            {
                Answers.Add(answer);
            }
        }
        public Dictionary<int, bool> GetAnswerValidity()
        {
            return Answers.ToDictionary(answer => answer.Id, answer => answer.IsCorrect);
        }

        public void LoadAnswersFromDatabase()
        {
            // Формируем запрос: выбираем все ответы для вопросов, принадлежащих данному тесту
            string query = $"SELECT Id, Question_Id, Answer_Text, Is_Correct FROM Answers WHERE Question_Id = {Id}";

            // Создаем подключение и выполняем запрос
            DatabaseHelper databaseHelper = new DatabaseHelper("Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=\"D:\\Проекты\\Тесты обучение WPF\\wpf тесты для обучения\\DB.mdf\";Integrated Security=True");
            DataTable dataTable = databaseHelper.ExecuteSelectQuery(query);
            Answers.Clear();
            // Проходим по всем строкам DataTable и создаем экземпляры Answers
            foreach (DataRow row in dataTable.Rows)
            {
                // Создаем новый объект Answers
                Answers answer = new Answers(
                    id: Convert.ToInt32(row["Id"]),
                    questionId: Convert.ToInt32(row["Question_Id"]),
                    answerText: row["Answer_Text"].ToString(),
                    isCorrect: Convert.ToBoolean(row["Is_Correct"])
                );

                // Ищем соответствующий вопрос и добавляем ответ в него
                
                Answers.Add(answer);
            }
        }
        public static string SaveImage(string imagePath)
        {
            try
            {
                if (string.IsNullOrEmpty(imagePath) || !File.Exists(imagePath))
                {
                    return null;
                }
                string directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "Images");
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }
                string fileName = Path.GetFileName(imagePath);
                string filePath = Path.Combine(directoryPath, fileName);
                int count = 1;
                while (File.Exists(filePath))
                {
                    filePath = Path.Combine(directoryPath, $"{Path.GetFileNameWithoutExtension(fileName)}_{count++}{Path.GetExtension(fileName)}");
                }
                File.Copy(imagePath, filePath);
                string relativePath = Path.Combine("Images", fileName);
                imagePath = relativePath; 
                return relativePath; 
            }
            catch (Exception ex)
            {
                // Обработка ошибок
                MessageBox.Show("Ошибка при сохранении изображения: " + ex.Message);
                return null;
            }
        }

    }

    public class Answers
    {
        public int Id { get; set; }
        public int QuestionId { get; set; }
        public string AnswerText { get; set; }
        public bool IsCorrect { get; set; }

        public Answers() { }

        public Answers(int id, int questionId, string answerText, bool isCorrect)
        {
            Id = id;
            QuestionId = questionId;
            AnswerText = answerText;
            IsCorrect = isCorrect;
        }
    }
}
