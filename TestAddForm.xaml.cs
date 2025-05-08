using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;

using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using wpf_тесты_для_обучения.Properties;
using CheckBox = System.Windows.Controls.CheckBox;
using RadioButton = System.Windows.Controls.RadioButton;
using TextBox = System.Windows.Controls.TextBox;
using MessageBox = System.Windows.MessageBox;
using Panel = System.Windows.Controls.Panel;
using Window = System.Windows.Window;
using wpf_тесты_для_обучения.Enums;
using static System.Net.Mime.MediaTypeNames;

namespace wpf_тесты_для_обучения
{
    public partial class TestAddForm : Window, INotifyPropertyChanged
    {

        private DatabaseHelper _databaseHelper;
        private int currentTest;
        private TestMode _mode;
        public TestAddForm(DatabaseHelper databaseHelper)
        {
            try
            {
                InitializeComponent();
                _databaseHelper = databaseHelper;
                DataContext = this;
                LoadRolesIntoComboBox();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Исключение: {ex.Message}\n" +
                   $"Метод: {ex.TargetSite}\n" +
                   $"Трассировка стека: {ex.StackTrace}", "Ошибка создания формы", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }
        public TestAddForm(DatabaseHelper databaseHelper, TestMode mode, int id = 0)
        {
            try
            {
                InitializeComponent();
                DataContext = this;
                _databaseHelper = databaseHelper;
                CurrentTest = id;
                _mode = mode;
                switch (mode)
                {
                    case TestMode.Create:
                        LoadRolesIntoComboBox();
                        break;

                    case TestMode.Edit:
                        LoadRolesIntoComboBox();
                        LoadTest();
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Исключение: {ex.Message}\n" +
                   $"Метод: {ex.TargetSite}\n" +
                   $"Трассировка стека: {ex.StackTrace}", "Ошибка создания формы", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }


        public int CurrentTest
        {
            get; set;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        private void LoadRolesIntoComboBox()
        {
            // Получаем список пользователей с ролями
            List<Roles> roles = _databaseHelper.GetRolesList();

            // Очищаем ComboBox перед добавлением данных
            rolesListBox.Items.Clear();

            rolesListBox.ItemsSource = roles;
            if (_mode == TestMode.Edit)
            {
                // Получаем список Test_Id, которые связаны с данной ролью
                string query = "SELECT Role_Id FROM RoleAccess WHERE Test_Id = @testId";
                SqlParameter[] parameters = { new SqlParameter("@testId", CurrentTest) };

                DataTable testRolesTable = _databaseHelper.ExecuteSelectQuery(query, parameters);
                List<int> selectedRolesIds = testRolesTable.AsEnumerable()
                                                          .Select(row => row.Field<int>("Role_Id"))
                                                          .ToList();

                // Отмечаем соответствующие тесты в ListBox
                foreach (var role in roles)
                {
                    if (selectedRolesIds.Contains(role.Id))
                    {
                        role.IsSelected = true; // Устанавливаем свойство (нужно добавить в класс)
                    }
                }

                // Обновляем ListBox
                rolesListBox.Items.Refresh();
            }
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MultipleQuestion newQuestion = new MultipleQuestion(_databaseHelper, false);
            newQuestion.ParentStackPanel = questionsStackPanel;
            questionsStackPanel.Children.Add(newQuestion);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            SingleQuestion newQuestion = new SingleQuestion(_databaseHelper, false);
            newQuestion.ParentStackPanel = questionsStackPanel;
            questionsStackPanel.Children.Add(newQuestion);
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            SingleQuestion newQuestion = new SingleQuestion(_databaseHelper, true, "");
            newQuestion.ParentStackPanel = questionsStackPanel;
            questionsStackPanel.Children.Add(newQuestion);
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            MultipleQuestion newQuestion = new MultipleQuestion(_databaseHelper, true, "");
            newQuestion.ParentStackPanel = questionsStackPanel;
            questionsStackPanel.Children.Add(newQuestion);
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

                string title = result.Rows[0]["Title"].ToString();
                double percent = Convert.ToDouble(result.Rows[0]["Is_Completed"]);

                completedPercentTextBox.Text = percent.ToString();
                titleTextBox.Visibility = Visibility.Collapsed;

                if (_mode == TestMode.Edit)
                {
                    titleTextBox.Text = title;
                    titleTextBox.Visibility = Visibility.Visible;
                }


                Tests test = new Tests(CurrentTest, title, percent);
                test.LoadQuestionsFromDatabase(_databaseHelper);
                foreach (Questions question in test.Questions)
                {
                    if (!question.IsMultiple)
                    {
                        SingleQuestion newQuestion;
                        if (_mode == TestMode.Edit)
                        {
                            if (question.Image != "")
                            {
                                newQuestion = new SingleQuestion(_databaseHelper, true, question, false, question.Image);
                            }
                            else
                            {
                                newQuestion = new SingleQuestion(_databaseHelper, false, question, false);
                            }
                        }
                        else
                        {
                            if (question.Image != "")
                            {
                                newQuestion = new SingleQuestion(_databaseHelper, true, question, true, question.Image);
                            }
                            else
                            {
                                newQuestion = new SingleQuestion(_databaseHelper, false, question, true);
                            }
                        }
                        newQuestion.ParentStackPanel = questionsStackPanel;
                        questionsStackPanel.Children.Add(newQuestion);
                        BaseQuestion.RenumberQuestions(questionsStackPanel);

                    }
                    if (question.IsMultiple)
                    {
                        MultipleQuestion newQuestion;
                        if (_mode == TestMode.Edit)
                        {
                            if (question.Image != "")
                            {
                                newQuestion = new MultipleQuestion(_databaseHelper, true, question, false, question.Image);
                            }
                            else
                            {
                                newQuestion = new MultipleQuestion(_databaseHelper, false, question, false);
                            }
                        }
                        else
                        {
                            if (question.Image != "")
                            {
                                newQuestion = new MultipleQuestion(_databaseHelper, true, question, true, question.Image);

                            }
                            else
                            {
                                newQuestion = new MultipleQuestion(_databaseHelper, false, question, true);
                            }
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
        private void Button_Click_4(object sender, RoutedEventArgs e)
        {

            try
            {
                if (BaseQuestion.ZeroErrors)
                {
                    if (titleTextBox.Text == "Введите название теста ..." || titleTextBox.Text == null || titleTextBox.Text == "")
                    {
                        ErrorTextBlock.Text = "Введите название теста";
                        ErrorTextBlock.Visibility = Visibility.Visible;
                        return;
                    }

                    if (completedPercentTextBox.Text == null || completedPercentTextBox.Text == "")
                    {
                        ErrorTextBlock.Text = "Введите процент верных ответов для прохождения";
                        ErrorTextBlock.Visibility = Visibility.Visible;
                        return;
                    }

                    double percent = Convert.ToDouble(completedPercentTextBox.Text);
                    string title = titleTextBox.Text;
                    if (_mode == TestMode.Create)
                    {
                        string query = @"INSERT INTO Tests (Title, Is_Completed) OUTPUT INSERTED.Id VALUES (@title, @completed)";
                        SqlParameter[] parameters = new SqlParameter[]
                        {
                                    new SqlParameter("@title", title),
                                    new SqlParameter("@completed", percent)
                        };
                        // Получаем Id добавленного теста
                        int testId = Convert.ToInt32(_databaseHelper.ExecuteScalar(query, parameters));
                        if (testId <= 0)
                        {
                            throw new Exception("Ошибка при добавлении теста в базу данных.");
                        }
                        foreach (var child in questionsStackPanel.Children)
                        {
                            if (child is SingleQuestion singleQuestion)
                            {
                                string questionText = singleQuestion.questionTextBox.Text;
                                string imagePath = null;
                                if (singleQuestion.ShowImage && singleQuestion.image.Source != null)
                                {
                                    ImageSource imageSource = singleQuestion.image.Source;

                                    //MessageBox.Show(singleQuestion.ImagePath);
                                    //image.Source = new BitmapImage(new Uri(openFileDialog.FileName));
                                    //ImageSource = new BitmapImage(new Uri(openFileDialog.FileName));
                                    //ImagePath = openFileDialog.FileName;
                                    imagePath = singleQuestion.SaveImage();
                                    if (string.IsNullOrEmpty(imagePath))
                                    {
                                        MessageBox.Show("Ошибка при сохранении изображения вопроса.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                                        return;
                                    }
                                }
                                query = imagePath == null ?
                                    @"INSERT INTO Questions (Test_Id, Question_Text) OUTPUT INSERTED.Id VALUES (@testid, @text)" :
                                    @"INSERT INTO Questions (Test_Id, Question_Text, Image) OUTPUT INSERTED.Id VALUES (@testid, @text, @img)";

                                parameters = imagePath == null ?
                                    new SqlParameter[] { new SqlParameter("@testid", testId), new SqlParameter("@text", questionText) } :
                                    new SqlParameter[] { new SqlParameter("@testid", testId), new SqlParameter("@text", questionText), new SqlParameter("@img", imagePath) };

                                int questionId = Convert.ToInt32(_databaseHelper.ExecuteScalar(query, parameters));

                                if (questionId <= 0)
                                {
                                    MessageBox.Show("Ошибка при добавлении вопроса.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                                    return;
                                }
                                foreach (var answerChild in singleQuestion.AnswersPanel.Children)
                                {
                                    if (answerChild is StackPanel stackPanel)
                                    {
                                        SortAnswersOut(stackPanel, questionId);
                                    }
                                    if (answerChild is Grid grid)
                                    {
                                        SortAnswersOut(grid, questionId);
                                    }
                                }
                            }

                            if (child is MultipleQuestion multipleQuestion)
                            {
                                string questionText = multipleQuestion.questionTextBox.Text;
                                string imagePath = null;
                                if (multipleQuestion.ShowImage && multipleQuestion.image.Source != null)
                                {
                                    imagePath = multipleQuestion.SaveImage();
                                    if (string.IsNullOrEmpty(imagePath))
                                    {
                                        MessageBox.Show("Ошибка при сохранении изображения вопроса.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                                        return;
                                    }
                                }
                                query = imagePath == null ?
                                    @"INSERT INTO Questions (Test_Id, Question_Text) OUTPUT INSERTED.Id VALUES (@testid, @text)" :
                                    @"INSERT INTO Questions (Test_Id, Question_Text, Image) OUTPUT INSERTED.Id VALUES (@testid, @text, @img)";
                                parameters = imagePath == null ?
                                    new SqlParameter[] { new SqlParameter("@testid", testId), new SqlParameter("@text", questionText) } :
                                    new SqlParameter[] { new SqlParameter("@testid", testId), new SqlParameter("@text", questionText), new SqlParameter("@img", imagePath) };

                                int questionId = Convert.ToInt32(_databaseHelper.ExecuteScalar(query, parameters));
                                if (questionId <= 0)
                                {
                                    MessageBox.Show("Ошибка при добавлении вопроса.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                                    return;
                                }
                                foreach (var answerChild in multipleQuestion.AnswersPanel.Children)
                                {
                                    if (answerChild is StackPanel stackPanel)
                                    {
                                        SortAnswersOut(stackPanel, questionId);
                                    }
                                    if (answerChild is Grid grid)
                                    {
                                        SortAnswersOut(grid, questionId);
                                    }
                                }
                            }
                        }
                        if (rolesListBox.ItemsSource is List<Roles> roles)
                        {
                            var selectedRoles = roles.Where(r => r.IsSelected).Select(r => r.Id).ToList();

                            if (selectedRoles.Any())
                            {
                                foreach (int roleId in selectedRoles)
                                {
                                    string queryR = @"INSERT INTO RoleAccess (Role_Id, Test_Id) VALUES (@r_id, @t_id)";
                                    SqlParameter[] parametersR = new SqlParameter[]
                                    {
                                        new SqlParameter("@r_id", roleId),
                                        new SqlParameter("@t_id", testId)
                                    };
                                    _databaseHelper.ExecuteNonQuery(queryR, parametersR);
                                }
                            }
                        }

                    }
                    if (_mode == TestMode.Edit)
                    {
                        string getQuestionsQuery = "SELECT Id FROM Questions WHERE Test_Id = @testId";
                        SqlParameter[] getQuestionsParams = { new SqlParameter("@testId", CurrentTest) };

                        DataTable questionsTable = _databaseHelper.ExecuteSelectQuery(getQuestionsQuery, getQuestionsParams);
                        List<int> existingQuestionIds = questionsTable.AsEnumerable()
                            .Select(row => row.Field<int>("Id"))
                            .ToList();
                        List<int> updatedQuestionIds = questionsStackPanel.Children
                        .OfType<BaseQuestion>() // Базовый класс вопросов
                        .Where(q => q.question != null)
                        .Select(q => q.question.Id)
                        .ToList();
                        List<int> questionsToDelete = existingQuestionIds.Except(updatedQuestionIds).ToList();
                        foreach (int questionId in questionsToDelete)
                        {
                            string deleteAnswersQuery = "DELETE FROM Answers WHERE Question_Id = @questionId";
                            string deleteQuestionQuery = "DELETE FROM Questions WHERE Id = @questionId";

                            SqlParameter[] deleteParams = { new SqlParameter("@questionId", questionId) };
                            SqlParameter[] deleteParams2 = { new SqlParameter("@questionId", questionId) };

                            _databaseHelper.ExecuteNonQuery(deleteAnswersQuery, deleteParams);
                            _databaseHelper.ExecuteNonQuery(deleteQuestionQuery, deleteParams2);
                        }

                        string query = @"UPDATE Tests SET Title = @title, Is_Completed = @completed WHERE Id = @id";
                        SqlParameter[] parameters = new SqlParameter[] {
                                    new SqlParameter("@title", title),
                                    new SqlParameter("@id", CurrentTest),
                                    new SqlParameter("@completed", percent)
                                };

                        // Обновляем тест
                        int rowsAffected = _databaseHelper.ExecuteNonQuery(query, parameters);
                        if (rowsAffected <= 0)
                        {
                            throw new Exception("Ошибка при обновлении теста.");
                        }
                        foreach (var child in questionsStackPanel.Children)
                        {
                            if (child is SingleQuestion singleQuestion)
                            {
                                string questionText = singleQuestion.questionTextBox.Text;
                                string imagePath = null;
                                if (singleQuestion.ShowImage && singleQuestion.image.Source != null)
                                {
                                    ImageSource imageSource = singleQuestion.image.Source;
                                    if (singleQuestion.image.Source is BitmapImage bitmapImage && bitmapImage.UriSource != null)
                                    {
                                        singleQuestion.ImagePath = bitmapImage.UriSource.LocalPath;
                                    }
                                    singleQuestion.ImageSource = (BitmapImage)imageSource;
                                    imagePath = singleQuestion.SaveImage();
                                    if (string.IsNullOrEmpty(imagePath))
                                    {
                                        MessageBox.Show("Ошибка при сохранении изображения вопроса.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                                        return;
                                    }
                                }

                                if (singleQuestion.question != null && singleQuestion.question.Id != 0) // Проверяем, существует ли вопрос (Id > 0)
                                {
                                    // Обновляем существующий вопрос
                                    query = imagePath == null ?
                                        @"UPDATE Questions SET Question_Text = @text WHERE Id = @id" :
                                        @"UPDATE Questions SET Question_Text = @text, Image = @img WHERE Id = @id";

                                    parameters = imagePath == null ?
                                        new SqlParameter[] { new SqlParameter("@id", singleQuestion.question.Id), new SqlParameter("@text", questionText) } :
                                        new SqlParameter[] { new SqlParameter("@id", singleQuestion.question.Id), new SqlParameter("@text", questionText), new SqlParameter("@img", imagePath) };

                                    rowsAffected = _databaseHelper.ExecuteNonQuery(query, parameters);

                                    if (rowsAffected <= 0)
                                    {
                                        MessageBox.Show("Ошибка при обновлении вопроса.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                                        return;
                                    }
                                }
                                else
                                {
                                    // Вставляем новый вопрос, если Id не задан
                                    query = imagePath == null ?
                                        @"INSERT INTO Questions (Test_Id, Question_Text) OUTPUT INSERTED.Id VALUES (@testid, @text)" :
                                        @"INSERT INTO Questions (Test_Id, Question_Text, Image) OUTPUT INSERTED.Id VALUES (@testid, @text, @img)";

                                    parameters = imagePath == null ?
                                        new SqlParameter[] { new SqlParameter("@testid", CurrentTest), new SqlParameter("@text", questionText) } :
                                        new SqlParameter[] { new SqlParameter("@testid", CurrentTest), new SqlParameter("@text", questionText), new SqlParameter("@img", imagePath) };

                                    int newQuestionId = Convert.ToInt32(_databaseHelper.ExecuteScalar(query, parameters));

                                    if (newQuestionId > 0)
                                    {
                                        if (singleQuestion.question == null)
                                        {
                                            singleQuestion.question = new Questions(); // Создаем новый объект, если его нет
                                        }
                                        singleQuestion.question.Id = newQuestionId; // Присваиваем новый ID объекту
                                    }
                                    else
                                    {
                                        MessageBox.Show("Ошибка при добавлении вопроса.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                                        return;
                                    }
                                }
                                string selectAnswersForQuestionQuery = "SELECT Id FROM Answers WHERE Question_Id = @questionId";
                                SqlParameter[] answersQueryParams = { new SqlParameter("@questionId", singleQuestion.question.Id) };

                                DataTable answers = _databaseHelper.ExecuteSelectQuery(selectAnswersForQuestionQuery, answersQueryParams);
                                List<int> answerIdsDB = answers.AsEnumerable().Select(row => row.Field<int>("Id")).ToList();
                                List<int> answerIdsElements = GetAnswersIdsList(singleQuestion.AnswersPanel);
                                List<int> missingIds = answerIdsDB.Except(answerIdsElements).ToList();

                                if (missingIds.Count != 0)
                                {
                                    foreach (var answerId in missingIds)
                                    {
                                        string deleteQuery = @"DELETE FROM Answers WHERE Id = @answerId";

                                        SqlParameter[] deleteParameters = new SqlParameter[]
                                        {
                                                new SqlParameter("@answerId", answerId)
                                        };

                                        int rowsAffected2 = _databaseHelper.ExecuteNonQuery(deleteQuery, deleteParameters);

                                        if (rowsAffected2 <= 0)
                                        {
                                            MessageBox.Show("Ошибка при удалении ответа с ID: " + answerId, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                                            return; // Прерывание на ошибке, если нужно остановить выполнение
                                        }
                                    }
                                }
                                foreach (var answerChild in singleQuestion.AnswersPanel.Children)
                                {
                                    if (answerChild is StackPanel stackPanel)
                                    {
                                        SortAnswersOut(stackPanel, singleQuestion.question.Id, singleQuestion.question);
                                    }
                                    if (answerChild is Grid grid)
                                    {
                                        SortAnswersOut(grid, singleQuestion.question.Id, singleQuestion.question);
                                    }
                                }
                            }
                            if (child is MultipleQuestion multipleQuestion)
                            {
                                string questionText = multipleQuestion.questionTextBox.Text;
                                string imagePath = null;
                                if (multipleQuestion.ShowImage && multipleQuestion.image.Source != null)
                                {
                                    ImageSource imageSource = multipleQuestion.image.Source;
                                    if (multipleQuestion.image.Source is BitmapImage bitmapImage && bitmapImage.UriSource != null)
                                    {
                                        multipleQuestion.ImagePath = bitmapImage.UriSource.LocalPath;
                                    }
                                    multipleQuestion.ImageSource = (BitmapImage)imageSource;
                                    imagePath = multipleQuestion.SaveImage();
                                    if (string.IsNullOrEmpty(imagePath))
                                    {
                                        MessageBox.Show("Ошибка при сохранении изображения вопроса.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                                        return;
                                    }
                                }


                                if (multipleQuestion.question != null && multipleQuestion.question.Id != 0) // Проверяем, существует ли вопрос (Id > 0)
                                {
                                    query = imagePath == null ?
                                        @"UPDATE Questions SET Question_Text = @text WHERE Id = @id" :
                                        @"UPDATE Questions SET Question_Text = @text, Image = @img WHERE Id = @id";

                                    parameters = imagePath == null ?
                                        new SqlParameter[] { new SqlParameter("@id", multipleQuestion.question.Id), new SqlParameter("@text", questionText) } :
                                        new SqlParameter[] { new SqlParameter("@id", multipleQuestion.question.Id), new SqlParameter("@text", questionText), new SqlParameter("@img", imagePath) };

                                    rowsAffected = _databaseHelper.ExecuteNonQuery(query, parameters);

                                    if (rowsAffected <= 0)
                                    {
                                        MessageBox.Show("Ошибка при обновлении вопроса.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                                        return;
                                    }
                                }
                                else
                                {
                                    query = imagePath == null ?
                                        @"INSERT INTO Questions (Test_Id, Question_Text) OUTPUT INSERTED.Id VALUES (@testid, @text)" :
                                        @"INSERT INTO Questions (Test_Id, Question_Text, Image) OUTPUT INSERTED.Id VALUES (@testid, @text, @img)";

                                    parameters = imagePath == null ?
                                        new SqlParameter[] { new SqlParameter("@testid", CurrentTest), new SqlParameter("@text", questionText) } :
                                        new SqlParameter[] { new SqlParameter("@testid", CurrentTest), new SqlParameter("@text", questionText), new SqlParameter("@img", imagePath) };

                                    int newQuestionId = Convert.ToInt32(_databaseHelper.ExecuteScalar(query, parameters));

                                    if (newQuestionId > 0)
                                    {
                                        if (multipleQuestion.question == null)
                                        {
                                            multipleQuestion.question = new Questions(); // Создаем новый объект, если его нет
                                        }
                                        multipleQuestion.question.Id = newQuestionId;
                                    }
                                    else
                                    {
                                        MessageBox.Show("Ошибка при добавлении вопроса.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                                        return;
                                    }
                                }
                                string selectAnswersForQuestionQuery = "SELECT Id FROM Answers WHERE Question_Id = @questionId";
                                SqlParameter[] answersQueryParams = { new SqlParameter("@questionId", multipleQuestion.question.Id) };

                                DataTable answers = _databaseHelper.ExecuteSelectQuery(selectAnswersForQuestionQuery, answersQueryParams);
                                List<int> answerIdsDB = answers.AsEnumerable().Select(row => row.Field<int>("Id")).ToList();
                                List<int> answerIdsElements = GetAnswersIdsList(multipleQuestion.AnswersPanel);
                                List<int> missingIds = answerIdsDB.Except(answerIdsElements).ToList();

                                if (missingIds.Count != 0)
                                {
                                    foreach (var answerId in missingIds)
                                    {
                                        string deleteQuery = @"DELETE FROM Answers WHERE Id = @answerId";

                                        SqlParameter[] deleteParameters = new SqlParameter[]
                                        {
                                                new SqlParameter("@answerId", answerId)
                                        };

                                        int rowsAffected2 = _databaseHelper.ExecuteNonQuery(deleteQuery, deleteParameters);

                                        if (rowsAffected2 <= 0)
                                        {
                                            MessageBox.Show("Ошибка при удалении ответа с ID: " + answerId, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                                            return; // Прерывание на ошибке, если нужно остановить выполнение
                                        }
                                    }
                                }
                                foreach (var answerChild in multipleQuestion.AnswersPanel.Children)
                                {
                                    if (answerChild is StackPanel stackPanel)
                                    {
                                        SortAnswersOut(stackPanel, multipleQuestion.question.Id, multipleQuestion.question);
                                    }
                                    if (answerChild is Grid grid)
                                    {
                                        SortAnswersOut(grid, multipleQuestion.question.Id, multipleQuestion.question);
                                    }
                                }
                            }
                        }

                        if (rolesListBox.ItemsSource is List<Roles> roles)
                        {
                            var selectedRoles = roles.Where(r => r.IsSelected).Select(r => r.Id).ToList();

                            if (selectedRoles.Any())
                            {
                                using (SqlConnection connection = _databaseHelper.GetConnection())
                                {
                                    connection.Open();

                                    // Удаляем старые записи для теста
                                    string deleteQuery = "DELETE FROM RoleAccess WHERE Test_Id = @t_id";
                                    using (SqlCommand deleteCommand = new SqlCommand(deleteQuery, connection))
                                    {
                                        deleteCommand.Parameters.AddWithValue("@t_id", CurrentTest);
                                        deleteCommand.ExecuteNonQuery();
                                    }

                                    // Добавляем только новые выбранные роли
                                    string insertQuery = @"INSERT INTO RoleAccess (Role_Id, Test_Id) VALUES (@r_id, @t_id)";
                                    using (SqlCommand insertCommand = new SqlCommand(insertQuery, connection))
                                    {
                                        insertCommand.Parameters.Add("@r_id", SqlDbType.Int);
                                        insertCommand.Parameters.AddWithValue("@t_id", CurrentTest);

                                        foreach (int roleId in selectedRoles)
                                        {
                                            insertCommand.Parameters["@r_id"].Value = roleId;
                                            insertCommand.ExecuteNonQuery();
                                        }
                                    }
                                }
                            }
                        }

                    }

                    BaseQuestion.ResetQuestionCounter(); // Сбрасываем счётчик

                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Исключение: {ex.Message}\n" +
                   $"Метод: {ex.TargetSite}\n" +
                   $"Трассировка стека: {ex.StackTrace}", "Ошибка создания/редактирования теста", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }

        public void SortAnswersOut(StackPanel stackPanel, int questionId, Questions question = null)
        {
            try
            {
                string answerText = "";
                bool isCorrect = false;
                int? answerId = null;
                foreach (var stackChild in stackPanel.Children)
                {
                    if (stackChild is RadioButton radioButton)
                    {
                        if (radioButton.Tag is int rbId)
                        {
                            answerId = rbId;
                        }
                        isCorrect = radioButton.IsChecked == true;
                    }
                    if (stackChild is CheckBox checkBox)
                    {
                        if (checkBox.Tag is int cbId)
                        {
                            answerId = cbId;
                        }
                        isCorrect = checkBox.IsChecked == true;
                    }
                    if (stackChild is TextBox textBox)
                    {
                        answerText = textBox.Text;
                    }
                }

                answerId = question?.Answers.FirstOrDefault(a => a.Id == answerId)?.Id;


                if (answerId.HasValue && answerId > 0) // Если у ответа уже есть ID, обновляем его
                {
                    string updateQuery = @"UPDATE Answers SET Answer_Text = @text, Is_Correct = @correct WHERE Id = @answerId";

                    SqlParameter[] updateParameters = new SqlParameter[]
                    {
                    new SqlParameter("@answerId", answerId.Value),
                    new SqlParameter("@text", answerText),
                    new SqlParameter("@correct", isCorrect)
                    };

                    int rowsAffected = _databaseHelper.ExecuteNonQuery(updateQuery, updateParameters);

                    if (rowsAffected <= 0)
                    {
                        MessageBox.Show("Ошибка при обновлении ответа.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }
                else // Если ID нет, добавляем новый ответ
                {
                    string insertQuery = @"INSERT INTO Answers (Question_Id, Answer_Text, Is_Correct) OUTPUT INSERTED.Id VALUES (@questionid, @text, @correct)";

                    SqlParameter[] insertParameters = new SqlParameter[]
                    {
                    new SqlParameter("@questionid", questionId),
                    new SqlParameter("@text", answerText),
                    new SqlParameter("@correct", isCorrect)
                    };

                    int newAnswerId = Convert.ToInt32(_databaseHelper.ExecuteScalar(insertQuery, insertParameters));

                    if (newAnswerId <= 0)
                    {
                        MessageBox.Show("Ошибка при добавлении ответа.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    // Добавляем новый ответ в список question.Answers (если question не null)
                    question?.Answers.Add(new Answers(newAnswerId, questionId, answerText, isCorrect));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Исключение: {ex.Message}\n" +
                   $"Метод: {ex.TargetSite}\n" +
                   $"Трассировка стека: {ex.StackTrace}", "Ошибка перебора вопросов", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }
        public void SortAnswersOut(Grid stackPanel, int questionId, Questions question = null)
        {
            try
            {
                string answerText = "";
                bool isCorrect = false;
                int? answerId = null;
                foreach (var stackChild in stackPanel.Children)
                {
                    if (stackChild is RadioButton radioButton)
                    {
                        if (radioButton.Tag is int rbId)
                        {
                            answerId = rbId;
                        }
                        isCorrect = radioButton.IsChecked == true;
                    }
                    if (stackChild is CheckBox checkBox)
                    {
                        if (checkBox.Tag is int cbId)
                        {
                            answerId = cbId;
                        }
                        isCorrect = checkBox.IsChecked == true;
                    }
                    if (stackChild is TextBox textBox)
                    {
                        answerText = textBox.Text;
                    }
                }

                answerId = question?.Answers.FirstOrDefault(a => a.Id == answerId)?.Id;


                if (answerId.HasValue && answerId > 0) // Если у ответа уже есть ID, обновляем его
                {
                    string updateQuery = @"UPDATE Answers SET Answer_Text = @text, Is_Correct = @correct WHERE Id = @answerId";

                    SqlParameter[] updateParameters = new SqlParameter[]
                    {
                    new SqlParameter("@answerId", answerId.Value),
                    new SqlParameter("@text", answerText),
                    new SqlParameter("@correct", isCorrect)
                    };

                    int rowsAffected = _databaseHelper.ExecuteNonQuery(updateQuery, updateParameters);

                    if (rowsAffected <= 0)
                    {
                        MessageBox.Show("Ошибка при обновлении ответа.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }
                else // Если ID нет, добавляем новый ответ
                {
                    string insertQuery = @"INSERT INTO Answers (Question_Id, Answer_Text, Is_Correct) OUTPUT INSERTED.Id VALUES (@questionid, @text, @correct)";

                    SqlParameter[] insertParameters = new SqlParameter[]
                    {
                    new SqlParameter("@questionid", questionId),
                    new SqlParameter("@text", answerText),
                    new SqlParameter("@correct", isCorrect)
                    };

                    int newAnswerId = Convert.ToInt32(_databaseHelper.ExecuteScalar(insertQuery, insertParameters));

                    if (newAnswerId <= 0)
                    {
                        MessageBox.Show("Ошибка при добавлении ответа.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    // Добавляем новый ответ в список question.Answers (если question не null)
                    question?.Answers.Add(new Answers(newAnswerId, questionId, answerText, isCorrect));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Исключение: {ex.Message}\n" +
                   $"Метод: {ex.TargetSite}\n" +
                   $"Трассировка стека: {ex.StackTrace}", "Ошибка перебора вопросов", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }
        public List<int> GetAnswersIdsList(StackPanel answersPanel)
        {
            List<int> answersId = new List<int>(); // Создаём список ID

            // Перебираем все элементы в AnswersPanel
            foreach (var child in answersPanel.Children)
            {
                if (child is StackPanel stackPanel)
                {
                    // Получаем ID из каждого StackPanel
                    int? idFromStackPanel = ExtractIdFromStackPanel(stackPanel);
                    if (idFromStackPanel.HasValue)
                    {
                        answersId.Add(idFromStackPanel.Value); // Добавляем ID в список
                    }
                }
                if (child is Grid grid)
                {
                    // Получаем ID из каждого StackPanel
                    int? idFromStackPanel = ExtractIdFromStackPanel(grid);
                    if (idFromStackPanel.HasValue)
                    {
                        answersId.Add(idFromStackPanel.Value); // Добавляем ID в список
                    }
                }
            }

            return answersId; // Возвращаем список всех ID
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
        public int? ExtractIdFromStackPanel(Grid stackPanel)
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
        private void titleTextBox_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            titleTextBox.SelectAll();
        }
        private void titleTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (titleTextBox.Text != "Введите название теста ...")
                titleTextBox.Foreground = Brushes.Black;
        }
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            BaseQuestion.ResetQuestionCounter();
        }

    }
}
