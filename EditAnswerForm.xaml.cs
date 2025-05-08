using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Path = System.IO.Path;
using wpf_тесты_для_обучения.Properties;

namespace wpf_тесты_для_обучения
{
    /// <summary>
    /// Логика взаимодействия для EditAnswerForm.xaml
    /// </summary>
    public partial class EditAnswerForm : Window
    {
        private DatabaseHelper _databaseHelper;
        public bool IsCorrect { get; set; }
        private Answers _answer;
        public EditAnswerForm(Answers answer, DatabaseHelper databaseHelper)
        {
            try
            {
                IsCorrect = answer.IsCorrect;
                InitializeComponent();
                this.DataContext = this;
                _databaseHelper = databaseHelper;
                //string databasePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DB.mdf");
                //_databaseHelper = new DatabaseHelper($"Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename={databasePath};Integrated Security=True");
                //_databaseHelper = new DatabaseHelper("Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=\"D:\\Проекты\\Тесты обучение WPF\\wpf тесты для обучения\\DB.mdf\";Integrated Security=True");
                LoadQuestionsIntoComboBox();
                _answer = answer;
                answerTextBox.Text = answer.AnswerText;
                questionsComboBox.SelectedItem = questionsComboBox.Items
                .Cast<Questions>()
                .FirstOrDefault(q => q.Id == answer.QuestionId);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при отключении базы данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
       
           
            private void LoadQuestionsIntoComboBox()
        {
            // Получаем список пользователей с ролями
            List<Questions> questions = _databaseHelper.GetQuestionsList();

            // Очищаем ComboBox перед добавлением данных
            questionsComboBox.Items.Clear();

            questionsComboBox.ItemsSource = questions;
        }


        private void goToAddQuestionsForm_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void goBack_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Questions question = questionsComboBox.SelectedItem as Questions;
            string text = answerTextBox.Text;
            if (UpdateErrors() > 0) return;

            string query = @"
            UPDATE Answers SET Question_Id = @questionId , Answer_Text = @text, Is_Correct = @correct
            WHERE Id = @id";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@questionId", question.Id),
                new SqlParameter("@text", text),
                new SqlParameter("@correct", IsCorrect),
                new SqlParameter("@id", _answer.Id)
            };

            _databaseHelper.ExecuteNonQuery(query, parameters);

            MessageBox.Show("Ответ успешно добавлен", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);
            this.Close();
        }
        protected int UpdateErrors()
        {
            errorTextBlock.Inlines.Clear(); // Очищаем текущие инлайны
            if (questionsComboBox.SelectedItem as Questions == null)
            {
                errorTextBlock.Inlines.Add(new LineBreak());
                errorTextBlock.Inlines.Add(new Run { Text = "! " });
                errorTextBlock.Inlines.Add(new Run { Text = "Вы не выбрали вопрос" });
            }
            if (answerTextBox.Text == "")
            {
                errorTextBlock.Inlines.Add(new LineBreak());
                errorTextBlock.Inlines.Add(new Run { Text = "! " });
                errorTextBlock.Inlines.Add(new Run { Text = "Вы не ввели ответ" });
            }
            return errorTextBlock.Inlines.Count;
        }
    }
}
