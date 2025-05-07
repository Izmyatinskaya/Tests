using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
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
using static System.Net.Mime.MediaTypeNames;

namespace wpf_тесты_для_обучения
{
    /// <summary>
    /// Логика взаимодействия для AddQuestionForm.xaml
    /// </summary>
    public partial class AddQuestionForm : Window
    {

        private DatabaseHelper _databaseHelper;
        public bool IsCorrect { get; set; }
        private string _path;
        private bool returnIdOnClose= false;
        public AddQuestionForm(DatabaseHelper databaseHelper)
        {
            try
            {
                InitializeComponent();
                _databaseHelper = databaseHelper;
                this.DataContext = this;
                //_databaseHelper = new DatabaseHelper("Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=\"D:\\Проекты\\Тесты обучение WPF\\wpf тесты для обучения\\DB.mdf\";Integrated Security=True");
                LoadTestsIntoComboBox();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Исключение: {ex.Message}\n" +
                  $"Метод: {ex.TargetSite}\n" +
                  $"Трассировка стека: {ex.StackTrace}", "Ошибка загрузки формы доб вопроса", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }

        private void LoadTestsIntoComboBox()
        {
            try
            {
                // Получаем список пользователей с ролями
                List<Tests> tests = _databaseHelper.GetTestsList();

                // Очищаем ComboBox перед добавлением данных
                testsComboBox.Items.Clear();

                testsComboBox.ItemsSource = tests;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Исключение: {ex.Message}\n" +
                  $"Метод: {ex.TargetSite}\n" +
                  $"Трассировка стека: {ex.StackTrace}", "Ошибка загрузки в комбобокс", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Tests test = testsComboBox.SelectedItem as Tests;
                string text = questionTextBox.Text;
                _path = Questions.SaveImage(_path);
                if (UpdateErrors() > 0) return;

                string query = _path != null ?
                    @"INSERT INTO Questions (Test_Id, Question_Text, Image) VALUES (@testId, @text, @image)" :
                    @"INSERT INTO Questions (Test_Id, Question_Text) VALUES (@testId, @text)";


                SqlParameter[] parameters = _path != null ? new SqlParameter[]
                {
                new SqlParameter("@testId", test.Id),
                new SqlParameter("@text", text),
                new SqlParameter("@image", _path)
                } :
                new SqlParameter[]
                {
                new SqlParameter("@testId", test.Id),
                new SqlParameter("@text", text)
                };

                _databaseHelper.ExecuteNonQuery(query, parameters);

                MessageBox.Show("Вопрос успешно добавлен", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);

                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Исключение: {ex.Message}\n" +
                  $"Метод: {ex.TargetSite}\n" +
                  $"Трассировка стека: {ex.StackTrace}", "Ошибка добавления вопроса", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }

        private void LoadImage_Click(object sender, RoutedEventArgs e)
        {
            try
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
            catch (Exception ex)
            {
                MessageBox.Show($"Исключение: {ex.Message}\n" +
                  $"Метод: {ex.TargetSite}\n" +
                  $"Трассировка стека: {ex.StackTrace}", "Ошибка загрузки картинки", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }
        protected int UpdateErrors()
        {
            try
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
            catch (Exception ex)
            {
                MessageBox.Show($"Исключение: {ex.Message}\n" +
                  $"Метод: {ex.TargetSite}\n" +
                  $"Трассировка стека: {ex.StackTrace}", "Ошибка обновления поля ошибки", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return -1;
            }
            
        }
        private void goBack_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }
    }
}
