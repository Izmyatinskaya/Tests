using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Data;
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
using wpf_тесты_для_обучения.Enums;
using wpf_тесты_для_обучения.Properties;
using System.Windows.Media.Media3D;

namespace wpf_тесты_для_обучения
{
    /// <summary>
    /// Логика взаимодействия для TestViewMistakesForm.xaml
    /// </summary>
    public partial class TestViewMistakesForm : Window, INotifyPropertyChanged
    {
        private DatabaseHelper _databaseHelper;
        private int currentTest;
        public int CurrentTest
        {
            get; set;
        }

        private int currentRes;
        public int CurrentRes
        {
            get; set;
        }
        private int currentUser;
        public int CurrentUser
        {
            get; set;
        }

        public TestViewMistakesForm(DatabaseHelper databaseHelper, int idTest, int idResult, int idUser)
        {
            try
            {
                InitializeComponent();
                questionsStackPanel.PreviewMouseWheel += QuestionsStackPanel_PreviewMouseWheel;
                DataContext = this;
                _databaseHelper = databaseHelper;
                CurrentTest = idTest;
                CurrentRes = idResult;
                CurrentUser = idUser;
                LoadTest();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Исключение: {ex.Message}\n" +
                   $"Метод: {ex.TargetSite}\n" +
                   $"Трассировка стека: {ex.StackTrace}", "Ошибка создания формы", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }
        public TestViewMistakesForm()
        {
            InitializeComponent();
            questionsStackPanel.PreviewMouseWheel += QuestionsStackPanel_PreviewMouseWheel;
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public void LoadTest()
        {
            try
            {
                string query = @"SELECT Title, Is_Completed FROM Tests WHERE Id = @id";

                SqlParameter[] parameters = new SqlParameter[]
                {
                new SqlParameter("@id", CurrentTest)
                };

                // Выполняем запрос один раз
                DataTable result = _databaseHelper.ExecuteSelectQuery(query, parameters);

                query = $"SELECT Count(Id) as idC FROM Questions WHERE Test_Id = {CurrentTest}";
                int countBals = (int)_databaseHelper.ExecuteSelectQuery(query).Rows[0]["idC"];

                query = $"SELECT Score FROM Results WHERE Test_Id = {CurrentTest} And Results.Id = {CurrentRes}";
                double countUsersBals = Convert.ToDouble(_databaseHelper.ExecuteSelectQuery(query).Rows[0]["Score"]);


                string title = result.Rows[0]["Title"].ToString();
                double percent = Convert.ToDouble(result.Rows[0]["Is_Completed"]);


                titleTextBlock.Text = title;

                ballsTextBlock.Text = $"Набрано баллов: {countUsersBals} из {countBals}";
                percentTextBlock.Text = $"Тест пройден на {Math.Round(countUsersBals/countBals, 2) * 100}% из 100%";
                Tests test = new Tests(CurrentTest, title, percent);
                test.LoadQuestionsFromDatabase(_databaseHelper);
                foreach (Questions question in test.Questions)
                {
                    if (!question.IsMultiple)
                    {
                        SingleQuestion newQuestion;

                        if (question.Image != "")
                        {
                            newQuestion = new SingleQuestion(_databaseHelper, true, question, TestMode.ViewMistakes, CurrentRes, CurrentUser,  question.Image);
                        }
                        else
                        { 
                            newQuestion = new SingleQuestion(_databaseHelper, false, question, TestMode.ViewMistakes, CurrentRes, CurrentUser);
                        }
                        newQuestion.ParentStackPanel = questionsStackPanel;
                        questionsStackPanel.Children.Add(newQuestion);
                        BaseQuestion.RenumberQuestions(questionsStackPanel);

                    }
                    if (question.IsMultiple)
                    {
                        MultipleQuestion newQuestion;

                        if (question.Image != "")
                        {
                            newQuestion = new MultipleQuestion(_databaseHelper, true, question, TestMode.ViewMistakes, CurrentRes, CurrentUser, question.Image);

                        }
                        else
                        {
                            newQuestion = new MultipleQuestion(_databaseHelper, false, question, TestMode.ViewMistakes, CurrentRes, CurrentUser);
                        }
                        newQuestion.ParentStackPanel = questionsStackPanel;
                        questionsStackPanel.Children.Add(newQuestion);
                        BaseQuestion.RenumberQuestions(questionsStackPanel);
                    }
                }
            }
            catch (Exception ex)
            {
                // Вывод сообщения об ошибке при поиске файла
                MessageBox.Show($"Исключение: {ex.Message}\n" +
                    $"Метод: {ex.TargetSite}\n" +
                    $"Трассировка стека: {ex.StackTrace}", "Ошибка загрузки теста", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }

     
        }

        private void Button_Click_5(object sender, RoutedEventArgs e) // Увеличить
        {
            ChangeZoom(0.1);
        }

        private void Button_Click_6(object sender, RoutedEventArgs e) // Уменьшить
        {
            ChangeZoom(-0.1);
        }

        private void ChangeZoom(double delta)
        {
            double newScale = Math.Max(0.5, Math.Min(2.5, scaleTransform.ScaleX + delta)); // Ограничим от 0.5 до 3
            scaleTransform.ScaleX = newScale;
            scaleTransform.ScaleY = newScale;
        }
        private void QuestionsStackPanel_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            {
                double zoomDelta = e.Delta > 0 ? 0.1 : -0.1;
                scaleTransform.ScaleX = Math.Max(0.5, scaleTransform.ScaleX + zoomDelta);
                scaleTransform.ScaleY = Math.Max(0.5, scaleTransform.ScaleY + zoomDelta);
                e.Handled = true;
            }
        }
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            BaseQuestion.ResetQuestionCounter();
        }
    }
}
