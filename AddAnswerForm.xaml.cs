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
using System.Xml.Linq;
using wpf_тесты_для_обучения.Properties;
using static MaterialDesignThemes.Wpf.Theme;

namespace wpf_тесты_для_обучения
{
    /// <summary>
    /// Логика взаимодействия для AddAnswerForm.xaml
    /// </summary>
    public partial class AddAnswerForm : Window
    {
        private DatabaseHelper _databaseHelper;
        public bool IsCorrect { get; set; }
        public AddAnswerForm()
        {
            IsCorrect = false;
            InitializeComponent();
            this.DataContext = this;
            _databaseHelper = new DatabaseHelper("Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=\"D:\\Проекты\\Тесты обучение WPF\\wpf тесты для обучения\\DB.mdf\";Integrated Security=True");
            LoadQuestionsIntoComboBox();
        }

        private void LoadQuestionsIntoComboBox()
        {
            // Получаем список пользователей с ролями
            List<Questions> questions = _databaseHelper.GetQuestionsList();

            // Очищаем ComboBox перед добавлением данных
            questionsComboBox.Items.Clear();

            questionsComboBox.ItemsSource = questions;
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
            INSERT INTO Answers (Question_Id, Answer_Text, Is_Correct)
            VALUES (@questionId, @text, @correct)";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@questionId", question.Id),
                new SqlParameter("@text", text),
                new SqlParameter("@correct", IsCorrect)
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
