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
using wpf_тесты_для_обучения.Properties;
using wpf_тесты_для_обучения.Enums;
namespace wpf_тесты_для_обучения
{
    /// <summary>
    /// Логика взаимодействия для TestViewPassForm.xaml
    /// </summary>
    public partial class TestViewPassForm : Window, INotifyPropertyChanged
    {
        private DatabaseHelper _databaseHelper;
        private int currentTest;
        private TestMode _mode; 

        public TestViewPassForm(DatabaseHelper databaseHelper, TestMode mode, int id = 0)
        {
            try
            {
                InitializeComponent();
                questionsStackPanel.PreviewMouseWheel += QuestionsStackPanel_PreviewMouseWheel;
                DataContext = this;
                _databaseHelper = databaseHelper;
                CurrentTest = id;
                IsViewMode = !(mode == TestMode.View);
                _mode = mode;
                LoadTest(false);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Исключение: {ex.Message}\n" +
                   $"Метод: {ex.TargetSite}\n" +
                   $"Трассировка стека: {ex.StackTrace}", "Ошибка создания формы", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }

        private bool _viewMode = true;
        public bool IsViewMode
        {
            get { return _viewMode; }
            set
            {
                _viewMode = value;
                OnPropertyChanged(nameof(IsViewMode));
                OnPropertyChanged(nameof(ViewMode)); // Уведомляем, что ViewMode тоже изменился
            }
        }
        public int CurrentTest
        {
            get; set;
        }

        public Visibility ViewMode => IsViewMode ? Visibility.Visible : Visibility.Collapsed;

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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
        public void LoadTest(bool editMode = false)
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

                string title = result.Rows[0]["Title"].ToString();
                double percent = Convert.ToDouble(result.Rows[0]["Is_Completed"]);


                titleTextBlock.Text = title;
                titleTextBlock.Visibility = Visibility.Visible;
                zoomPanel.Visibility = Visibility.Visible;
                if (_mode == TestMode.Pass)
                    endTestButton.Visibility = Visibility.Visible;
                else endTestButton.Visibility = Visibility.Collapsed;
                   
                Tests test = new Tests(CurrentTest, title, percent);
                test.LoadQuestionsFromDatabase(_databaseHelper);
                foreach (Questions question in test.Questions)
                {
                    if (!question.IsMultiple)
                    {
                        SingleQuestion newQuestion;
                        
                        if (question.Image != "")
                        {
                            newQuestion = new SingleQuestion(_databaseHelper, true, question, true, question.Image);
                        }
                        else
                        {
                            newQuestion = new SingleQuestion(_databaseHelper, false, question, true);
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
                            newQuestion = new MultipleQuestion(_databaseHelper, true, question, true, question.Image);

                        }
                        else
                        {
                            newQuestion = new MultipleQuestion(_databaseHelper, false, question, true);
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
        
        public int? ExtractIdFromStackPanel(StackPanel stackPanel)
        {
            // Перебираем все дочерние элементы StackPanel
            foreach (var stackChild in stackPanel.Children)
            {
                // Извлекаем ID из радио-кнопок
                if (stackChild is RadioButton radioButton && radioButton.Tag is int rbId)
                {
                    return rbId; // Возвращаем ID радио-кнопки
                }

                // Извлекаем ID из чекбоксов
                if (stackChild is CheckBox checkBox && checkBox.Tag is int cbId)
                {
                    return cbId; // Возвращаем ID чекбокса
                }
            }

            return null; // Если ID не найден, возвращаем null
        }
        public List<int> GetSelectedAnswersIds(Panel answersPanel)
        {
            List<int> selectedAnswerIds = new List<int>();

            foreach (var child in answersPanel.Children)
            {
                if (child is Grid grid)
                {
                    foreach (UIElement element in grid.Children)
                    {
                        if (element is CheckBox checkBox && checkBox.IsChecked == true)
                        {
                            if (checkBox.Tag is int id)
                            {
                                selectedAnswerIds.Add(id);
                            }
                        }
                        if (element is RadioButton radioButton && radioButton.IsChecked == true)
                        {
                            if (radioButton.Tag is int id)
                            {
                                selectedAnswerIds.Add(id);
                            }
                        }
                    }
                }
            }

            return selectedAnswerIds;
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            BaseQuestion.ResetQuestionCounter();
        }
        private bool ValidateAllQuestions()
        {
            bool isValid = true;

            foreach (var child in questionsStackPanel.Children)
            {
                if (child is SingleQuestion singleQuestion)
                {
                    singleQuestion.ClearError();
                    List<int> selectedAnswers = GetSelectedAnswersIds(singleQuestion.AnswersPanel);

                    if (selectedAnswers.Count == 0)
                    {
                        singleQuestion.SetError("Выберите один вариант ответа");
                        isValid = false;
                    }
                }
                else if (child is MultipleQuestion multipleQuestion)
                {
                    multipleQuestion.ClearError();
                    List<int> selectedAnswers = GetSelectedAnswersIds(multipleQuestion.AnswersPanel);

                    if (selectedAnswers.Count < 2)
                    {
                        multipleQuestion.SetError("Выберите минимум два варианта");
                        isValid = false;
                    }
                }
            }

            return isValid;
        }

        private void endingTest_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!ValidateAllQuestions())
                    return;
                double result = 0;
                int insertedId = 0;
                List<int> answerIds = new List<int>();
                foreach (var child in questionsStackPanel.Children)
                {
                    if (child is SingleQuestion singleQuestion)
                    {
                        Dictionary<int, bool> boolListBD = singleQuestion.question.GetAnswerValidity();
                        List<int> selectedAnswerId = GetSelectedAnswersIds(singleQuestion.AnswersPanel);
                        if (selectedAnswerId.Count == 0)
                        { MessageBox.Show($"Вы не выбрали ответ на вопрос", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Exclamation); return; }
                        boolListBD.TryGetValue(selectedAnswerId[0], out bool t);
                        if (t)
                            result += 1;
                        if (_databaseHelper._currentUser.Id != 1)
                        {
                            string query = @"INSERT INTO Mistakes (Question_Id, User_Answer, User_Id) OUTPUT INSERTED.Id VALUES (@idQ, @userAnsw, @user)";
                            SqlParameter[] parameters = new SqlParameter[]
                            {
                                    new SqlParameter("@user", UserSession.SelectedUser.Id),
                                    new SqlParameter("@idQ", singleQuestion.question.Id),
                                    new SqlParameter("@userAnsw", selectedAnswerId[0])
                            };
                            answerIds.Add((int)_databaseHelper.ExecuteScalar(query, parameters));
                        }
                    }
                    if (child is MultipleQuestion multipleQuestion)
                    {
                        Dictionary<int, bool> boolListBD = multipleQuestion.question.GetAnswerValidity();
                        List<int> selectedAnswerIds = GetSelectedAnswersIds(multipleQuestion.AnswersPanel);
                        if (selectedAnswerIds.Count == 0)
                        { MessageBox.Show($"Вы не выбрали ответ на вопрос", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Exclamation); return; }
                        // Количество верных ответов в базе
                        int totalCorrectAnswers = boolListBD.Count(kvp => kvp.Value);
                        double priceAnswer = 1.0 / totalCorrectAnswers;
                        priceAnswer = Math.Round(priceAnswer, 4);

                        // Количество верных ответов, выбранных пользователем
                        int correctAnswersCount = selectedAnswerIds.Count(answerId =>
                            boolListBD.TryGetValue(answerId, out bool isCorrect) && isCorrect);
                        int incorrectAnswersCount = selectedAnswerIds.Count(answerId =>
                            boolListBD.TryGetValue(answerId, out bool isCorrect) && !isCorrect);


                        result += Math.Round(correctAnswersCount * priceAnswer - incorrectAnswersCount * priceAnswer, 2);
                        if (_databaseHelper._currentUser.Id != 1)
                        {
                            foreach (int selectedId in selectedAnswerIds)
                            {
                                string query = @" INSERT INTO Mistakes (Question_Id, User_Answer, User_Id) OUTPUT INSERTED.Id VALUES (@idQ, @userAnsw, @user)";

                                SqlParameter[] parameters = new SqlParameter[]
                                {
                                    new SqlParameter("@user", UserSession.SelectedUser.Id),
                                    new SqlParameter("@idQ", multipleQuestion.question.Id),
                                    new SqlParameter("@userAnsw", selectedId)
                                };

                                answerIds.Add((int)_databaseHelper.ExecuteScalar(query, parameters));
                            }
                        }
                    }
                }
                if (_databaseHelper._currentUser.UserRole.Id != 1)
                {
                    string query = @"  INSERT INTO Results (User_Id, Test_Id, Score) OUTPUT INSERTED.Id VALUES (@user, @testid, @score);";

                    SqlParameter[] parameters = new SqlParameter[]
                    {
                        new SqlParameter("@user", UserSession.SelectedUser.Id),
                        new SqlParameter("@testid", CurrentTest),
                        new SqlParameter("@score", result)
                    };

                    // Выполняем ExecuteScalar, чтобы получить возвращённый ID
                    insertedId = (int)_databaseHelper.ExecuteScalar(query, parameters);


                }
                Tests test = new Tests(_databaseHelper);
                test.Id = CurrentTest;
                test.LoadIfEmpty();
                double percent = Math.Round(result / test.Count * 100, 2);
                string status = percent >= test.IsCompleted ? "успешно завершили" : "провалили";

                MessageBox.Show(
                    $"Тест завершен\r\n" +
                    $"Ваш результат прохождения теста составил:\r\n" +
                    $"{result} из {test.Count}\r\n" +
                    $"{percent}%\r\n" +
                    $"Вы {status} тест",
                    "Результат", MessageBoxButton.OK, MessageBoxImage.Information
                );

                this.Close();

                if (_databaseHelper._currentUser.Id != 1)
                {
                    string idList = string.Join(",", answerIds); // "1,2,3"

                    string query = $@"UPDATE Mistakes SET Result_Id = @resId WHERE Id IN ({idList})";
                    SqlParameter[] parameters = new SqlParameter[]
                    {
                        new SqlParameter("@resId", insertedId)
                    };
                    answerIds.Add(_databaseHelper.ExecuteNonQuery(query, parameters));
                    TestViewMistakesForm testViewMistakesForm = new TestViewMistakesForm(_databaseHelper, CurrentTest, insertedId, _databaseHelper._currentUser.Id);
                    testViewMistakesForm.Show();
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Исключение: {ex.Message}\n" +
                   $"Метод: {ex.TargetSite}\n" +
                   $"Трассировка стека: {ex.StackTrace}", "Ошибка загрузки формы входазаевршения теста", MessageBoxButton.OK, MessageBoxImage.Exclamation);
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
    }
}

