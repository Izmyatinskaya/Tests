using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
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
using wpf_тесты_для_обучения.Properties;

namespace wpf_тесты_для_обучения
{
    /// <summary>
    /// Логика взаимодействия для EditQuestionForm.xaml
    /// </summary>
    public partial class EditQuestionForm : Window
    {
        private DatabaseHelper _databaseHelper;
        public bool IsCorrect { get; set; }
        private string _path;
        private Questions _question;
        public EditQuestionForm(Questions question)
        {
            InitializeComponent();
            this.DataContext = this;
            _databaseHelper = new DatabaseHelper("Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=\"D:\\Проекты\\Тесты обучение WPF\\wpf тесты для обучения\\DB.mdf\";Integrated Security=True");
            LoadTestsIntoComboBox();
            _question = question;
            questionTextBox.Text = _question.QuestionText;
            testsComboBox.SelectedItem = testsComboBox.Items
            .Cast<Tests>()
            .FirstOrDefault(q => q.Id == _question.TestId);

            _path = question.Image;

            if (!string.IsNullOrEmpty(_path))
            {
                PreviewImage.Source = new BitmapImage(new Uri("file:///" + System.IO.Path.Combine(Directory.GetCurrentDirectory(), _path).Replace("\\", "/")));
            }
            else
            {
                PreviewImage.Source = null;
            }

        }

        private void LoadTestsIntoComboBox()
        {
            List<Tests> tests = _databaseHelper.GetTestsList();
            testsComboBox.Items.Clear();
            testsComboBox.ItemsSource = tests;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Tests test = testsComboBox.SelectedItem as Tests;
            string text = questionTextBox.Text;
            _path = Questions.SaveImage(_path);
            if (UpdateErrors() > 0) return;

            string query = @"UPDATE  Questions SET Test_Id = @testId, Question_Text = @text, Image = @image Where Id = @id";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@testId", test.Id),
                new SqlParameter("@text", text),
                new SqlParameter("@id", _question.Id),
                new SqlParameter("@image", _path != null ? _path : "")
            };

            _databaseHelper.ExecuteNonQuery(query, parameters);

            MessageBox.Show("Вопрос успешно отредактирован", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);
            this.Close();
        }
        
        private void LoadImage_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Title = "Выберите изображение",
                Filter = "Изображения (*.jpg;*.png;*.bmp)|*.jpg;*.png;*.bmp",
                Multiselect = false
            };

            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;
                _path = filePath;// Questions.SaveImage(filePath);
                PreviewImage.Source = new BitmapImage(new Uri(filePath));
            }
        }
        protected int UpdateErrors()
        {
            errorTextBlock.Inlines.Clear(); // Очищаем текущие инлайны
            if (testsComboBox.SelectedItem as Tests == null)
            {
                errorTextBlock.Inlines.Add(new LineBreak());
                errorTextBlock.Inlines.Add(new Run { Text = "! " });
                errorTextBlock.Inlines.Add(new Run { Text = "Вы не выбрали тест" });
            }
            if (questionTextBox.Text == "")
            {
                errorTextBlock.Inlines.Add(new LineBreak());
                errorTextBlock.Inlines.Add(new Run { Text = "! " });
                errorTextBlock.Inlines.Add(new Run { Text = "Вы не ввели вопрос" });
            }
            return errorTextBlock.Inlines.Count;
        }
        private void goBack_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }

        private void ClearImage_Click(object sender, RoutedEventArgs e)
        {
            _path = null;
            PreviewImage.Source = null;
        }
    }
}
