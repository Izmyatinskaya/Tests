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
using System.Windows.Forms;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml.Linq;
using wpf_тесты_для_обучения.Properties;
using MessageBox = System.Windows.MessageBox;
using DataGridCell = System.Windows.Controls.DataGridCell;

namespace wpf_тесты_для_обучения
{
    /// <summary>
    /// Логика взаимодействия для AdminForm.xaml
    /// </summary>
    public partial class AdminForm : Window
    {
        private DatabaseHelper _databaseHelper;
        public AdminForm()
        {
            InitializeComponent();
            _databaseHelper = new DatabaseHelper("Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=\"D:\\Проекты\\Тесты обучение WPF\\wpf тесты для обучения\\DB.mdf\";Integrated Security=True");
            LoadRoles();
            LoadTests();
            LoadUsers();
            LoadResults();

        }

        public void LoadUsers()
        {
            List<Users> users = _databaseHelper.GetUsersWithRoles();
            usersDataGrid.ItemsSource = users;
        }
        public void LoadResults()
        {
            List<Results> results = _databaseHelper.GetResultsWithUserAndTest();
            resultsDataGrid.ItemsSource = results;
        }
        public void LoadTestsToComboBox()
        {
            List<Tests> tests = _databaseHelper.GetTestsList();
            testsTitleComboBox.ItemsSource = tests;
        }
        public void LoadTests()
        {
            List<Tests> tests = _databaseHelper.GetTestsList();
            testsDataGrid.ItemsSource = tests;
        }
        public void LoadRoles()
        {
            List<Roles> roles = _databaseHelper.GetRolesList();
            foreach (var role in roles)
            {
                role.Tests = _databaseHelper.GetTestsForRole(role.Id);
            }
            rolessDataGrid.ItemsSource = roles; 
        }
        public void LoadQuestions()
        {
            List<Questions> questions = _databaseHelper.GetQuestionsList();
            questionsDataGrid.ItemsSource = questions;
        }
        public void LoadAnswers()
        {
            List<Answers> answers = _databaseHelper.GetAnswersList();
            answersDataGrid.ItemsSource = answers;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            UserAddForm userAddForm = new UserAddForm();
            userAddForm.ShowDialog();
            LoadUsers();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Users selectedUser = GetSelectedUser();

            if (selectedUser != null)
            {
                UserEditForm editForm = new UserEditForm(selectedUser);
                editForm.ShowDialog();
                LoadUsers();
            }
            else
            {
                MessageBox.Show("Выберите пользователя для редактирования.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private Users GetSelectedUser()
        {
            return usersDataGrid.SelectedItem as Users;
        }
        private Tests GetSelectedTest()
        {
            return testsDataGrid.SelectedItem as Tests;
        }
        private Questions GetSelectedQuestion()
        {
            return questionsDataGrid.SelectedItem as Questions;
        }
        private Answers GetSelectedAnswer()
        {
            return answersDataGrid.SelectedItem as Answers;
        }
        //new test
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            TestAddForm testAddForm = new TestAddForm();
            testAddForm.ShowDialog();
            LoadTests();
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            var selectedUser = GetSelectedUser();
            if (selectedUser == null)
            {
                MessageBox.Show("Выберите одного пользователя для удаления", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                string query = "DELETE FROM Users WHERE Id = @id";
                SqlParameter[] parameters = new SqlParameter[]
                {
            new SqlParameter("@id", selectedUser.Id)
                };
                _databaseHelper.ExecuteNonQuery(query, parameters);
                LoadUsers();
            }
            catch (SqlException ex)
            {
                if (ex.Number == 547)
                {
                    MessageBoxResult result = MessageBox.Show(
                        "Если вы удалите этого пользователя, все данные о результатах прохождения им тестов будут утеряны. Вы точно хотите это сделать?",
                        "Подтверждение удаления",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Warning);

                    if (result == MessageBoxResult.Yes)
                    {
                        string deleteResultsQuery = "DELETE FROM Results WHERE User_Id = @id";
                        _databaseHelper.ExecuteNonQuery(deleteResultsQuery, new SqlParameter[] { new SqlParameter("@id", selectedUser.Id) });

                        string query = "DELETE FROM Users WHERE Id = @id";
                        SqlParameter[] parameters = new SqlParameter[]
                        {
                            new SqlParameter("@id", selectedUser.Id)
                        };
                        _databaseHelper.ExecuteNonQuery(query, parameters);

                        LoadUsers();
                    }
                }
                else
                {
                    MessageBox.Show("Ошибка при удалении пользователя: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }


        private void TabItem_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            LoginForm loginForm = new LoginForm();
            loginForm.Show();
            this.Close();
        }

        private void viewTestClick(object sender, RoutedEventArgs e)
        {
            if (GetSelectedTest() == null)
            {       
                MessageBox.Show("Выберите тест для просмотра", "Ошибка");
                return;
            }
            TestAddForm testAddForm = new TestAddForm(TestAddForm.TestMode.View, GetSelectedTest().Id);
            testAddForm.ShowDialog();
        }

        private void edtiTestClick(object sender, RoutedEventArgs e)
        {
            if (GetSelectedTest() == null)
            {
                MessageBox.Show("Выберите тест для редактирования", "Ошибка");
                return;
            }
            TestAddForm testAddForm = new TestAddForm(TestAddForm.TestMode.Edit, GetSelectedTest().Id);
            testAddForm.ShowDialog();
            LoadTests();
        }

        private void deleteTestClick(object sender, RoutedEventArgs e)
        {
            try
            {
                if (GetSelectedTest() == null)
                {
                    MessageBox.Show("Выберите один тест для удаления", "Ошибка");
                    return;
                }
                string query = $"Delete from Tests Where Id = {GetSelectedTest().Id}";
                _databaseHelper.ExecuteNonQuery(query);

                LoadTests();
            }
            
            catch (SqlException sqlEx) // Ошибки, связанные с базой данных
            {
                MessageBox.Show($"Ошибка базы данных: {sqlEx.Message}", "Ошибка SQL", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (NullReferenceException nullEx) // Ошибка при обращении к null-объектам
            {
                MessageBox.Show($"Попытка обращения к несуществующему объекту: {nullEx.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (FormatException formatEx) // Ошибки преобразования данных
            {
                MessageBox.Show($"Ошибка формата данных: {formatEx.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex) // Общее исключение для всех остальных ошибок
            {
                MessageBox.Show($"Произошла непредвиденная ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            comboStackPanel.Visibility = Visibility.Visible;
            doubleGrid.Visibility = Visibility.Visible;
            answersQuestionsToolBar.Visibility = Visibility.Visible;
            testsDataGrid.Visibility = Visibility.Collapsed;
            testsToolBar.Visibility = Visibility.Collapsed;
            LoadTestsToComboBox();
            LoadAnswers();
            LoadQuestions();

        }

        private void testsTitleComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Tests t1 = testsTitleComboBox.SelectedItem as Tests;
            if (t1 == null) return;
            t1.LoadQuestionsFromDatabase();
            foreach (Questions q1 in t1.Questions)
            {
                q1.LoadAnswersFromDatabase();
            }
            questionsDataGrid.ItemsSource = t1.Questions;
            List<Answers> allAnswers = t1.Questions.SelectMany(q => q.Answers).ToList();
            answersDataGrid.ItemsSource = allAnswers;
        }

        private void questionsDataGrid_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Ищем, на какую ячейку кликнули
            DataGridCell cell = FindParent<DataGridCell>(e.OriginalSource as DependencyObject);

            if (cell != null)
            {
                // Получаем индекс колонки
                int columnIndex = cell.Column.DisplayIndex;

                // Проверяем, что кликнули именно во **вторую колонку** (индекс 1)
                if (columnIndex == 2)
                {
                    // Получаем данные строки, к которой принадлежит ячейка
                    var row = FindParent<DataGridRow>(cell);
                    if (row != null)
                    {
                        // Получаем объект, связанный с этой строкой
                        Questions question = row.Item as Questions;

                        if (question != null)
                        {
                            // Получаем содержимое ячейки (текст вопроса)
                            string cellContent = question.Image;

                            BaseQuestion.ShowFullImage(question.Image);
                        }
                    }
                }
            }
        }

        // Вспомогательный метод для поиска родителя нужного типа
        private static T FindParent<T>(DependencyObject child) where T : DependencyObject
        {
            while (child != null)
            {
                if (child is T parent)
                    return parent;
                child = VisualTreeHelper.GetParent(child);
            }
            return null;
        }

        private void questionsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (questionsDataGrid.SelectedItem is Questions selectedQuestion)
            {
                //MessageBox.Show($"Вы выбрали вопрос: {selectedQuestion.QuestionText}");
                Questions q1 = questionsDataGrid.SelectedItem as Questions;
                q1.LoadAnswersFromDatabase();
                answersDataGrid.ItemsSource = null;
                answersDataGrid.ItemsSource = q1.Answers;
            }

        }

        private void deleteQuestion_Click(object sender, RoutedEventArgs e)
        {
            deleteQuestions();
        }

        private void deleteAnswer_Click(object sender, RoutedEventArgs e)
        {

            deleteAnswers();
        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            comboStackPanel.Visibility = Visibility.Collapsed;
            doubleGrid.Visibility = Visibility.Collapsed;
            answersQuestionsToolBar.Visibility = Visibility.Collapsed;
            testsDataGrid.Visibility = Visibility.Visible;
            testsToolBar.Visibility = Visibility.Visible;
            LoadTests();
        }

        private void resetTables_Click(object sender, RoutedEventArgs e)
        {
            testsTitleComboBox.SelectedIndex = -1;
            LoadQuestions(); LoadAnswers();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            deleteAnswers();
        }
        protected void deleteAnswers()
        {
            // Получаем все выбранные элементы из таблицы
            var selectedAnswers = answersDataGrid.SelectedItems.Cast<Answers>().ToList();

            // Проверяем, есть ли выбранные элементы
            if (selectedAnswers.Any())
            {
                // Строим строку с параметрами для массового удаления
                var query = "DELETE FROM Answers WHERE Id IN (";

                // Добавляем параметры в запрос
                var parametersList = new List<SqlParameter>();
                for (int i = 0; i < selectedAnswers.Count; i++)
                {
                    query += $"@id{i},";
                    parametersList.Add(new SqlParameter($"@id{i}", selectedAnswers[i].Id));
                }

                // Убираем последнюю запятую из запроса
                query = query.TrimEnd(',');

                // Закрываем список параметров в запросе
                query += ")";

                // Выполняем запрос
                _databaseHelper.ExecuteNonQuery(query, parametersList.ToArray());


                // Если необходимо, обновите таблицу после удаления
                LoadAnswers();
                LoadQuestions();
            }
            else
            {
                MessageBox.Show("Нет выбранных элементов для удаления.");
            }
        }
        protected void deleteQuestions()
        {
            // Получаем все выбранные элементы из таблицы
            var selectedQuestions = questionsDataGrid.SelectedItems.Cast<Questions>().ToList();

            // Проверяем, есть ли выбранные элементы
            if (selectedQuestions.Any())
            {
                // Строим строку с параметрами для массового удаления
                var query = "DELETE FROM Questions WHERE Id IN (";

                // Добавляем параметры в запрос
                var parametersList = new List<SqlParameter>();
                for (int i = 0; i < selectedQuestions.Count; i++)
                {
                    query += $"@id{i},";
                    parametersList.Add(new SqlParameter($"@id{i}", selectedQuestions[i].Id));
                }

                // Убираем последнюю запятую из запроса
                query = query.TrimEnd(',');

                // Закрываем список параметров в запросе
                query += ")";

                // Выполняем запрос
                _databaseHelper.ExecuteNonQuery(query, parametersList.ToArray());


                // Если необходимо, обновите таблицу после удаления
                LoadAnswers();
                LoadQuestions();
            }
            else
            {
                MessageBox.Show("Нет выбранных элементов для удаления.");
            }
        }
        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            deleteQuestions();
        }

        private void addAnswer_Click(object sender, RoutedEventArgs e)
        {
            AddAnswerForm addAnswerForm = new AddAnswerForm();
            addAnswerForm.ShowDialog();
            LoadAnswers();
            LoadQuestions();
        }

        private void editAnswer_Click(object sender, RoutedEventArgs e)
        {
            Answers answer =  answersDataGrid.SelectedItem as Answers;
            if (answer == null) { MessageBox.Show("Выберите строку таблицы с ответами", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Exclamation); return; }
            EditAnswerForm editAnswerForm = new EditAnswerForm(answer);
            editAnswerForm.ShowDialog();
            LoadAnswers();
            LoadQuestions();
        }

        private void editAnswer_ContextMenu_Click(object sender, RoutedEventArgs e)
        {
            Answers answer = answersDataGrid.SelectedItem as Answers;
            if (answer == null) { MessageBox.Show("Выберите строку таблицы с ответами", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Exclamation); return; }
            EditAnswerForm editAnswerForm = new EditAnswerForm(answer);
            editAnswerForm.ShowDialog();
            LoadAnswers();
            LoadQuestions();
        }

        private void addQuestion_Click(object sender, RoutedEventArgs e)
        {
            AddQuestionForm addQuestionForm = new AddQuestionForm();
            addQuestionForm.ShowDialog();
            LoadAnswers();
            LoadQuestions();
        }

        private void editQuestion_Click(object sender, RoutedEventArgs e)
        {
            Questions question = questionsDataGrid.SelectedItem as Questions;
            if (question == null) { MessageBox.Show("Выберите строку таблицы с ответами", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Exclamation); return; }
            EditQuestionForm editQuestionForm = new EditQuestionForm(question);
            editQuestionForm.ShowDialog();
            LoadAnswers();
            LoadQuestions();
        }

        private void addRole_Click(object sender, RoutedEventArgs e)
        {
            AddRoleForm addRoleForm = new AddRoleForm();
            addRoleForm.ShowDialog();
            LoadRoles();
        }

        private void deleteRole_Click(object sender, RoutedEventArgs e)
        {
            var selectedRole = rolessDataGrid.SelectedItems.Cast<Roles>().ToList();
            if (selectedRole.Any())
            {
                var queryDeleteRoleAccess = "DELETE FROM RoleAccess WHERE Role_Id IN (";
                var queryDeleteRole = "DELETE FROM Roles WHERE Id IN (";

                var parametersListRoleAccess = new List<SqlParameter>();
                var parametersListRole = new List<SqlParameter>();

                // Формируем запросы для удаления
                for (int i = 0; i < selectedRole.Count; i++)
                {
                    queryDeleteRoleAccess += $"@roleId{i},";
                    queryDeleteRole += $"@id{i},";

                    parametersListRoleAccess.Add(new SqlParameter($"@roleId{i}", selectedRole[i].Id));
                    parametersListRole.Add(new SqlParameter($"@id{i}", selectedRole[i].Id));
                }

                // Обрезаем лишнюю запятую в конце
                queryDeleteRoleAccess = queryDeleteRoleAccess.TrimEnd(',') + ")";
                queryDeleteRole = queryDeleteRole.TrimEnd(',') + ")";

                // Выполняем удаление записей из таблицы RoleAccess
                _databaseHelper.ExecuteNonQuery(queryDeleteRoleAccess, parametersListRoleAccess.ToArray());

                // Затем удаляем записи из таблицы Roles
                _databaseHelper.ExecuteNonQuery(queryDeleteRole, parametersListRole.ToArray());

                // Перезагружаем список ролей
                LoadRoles();
            }
            else
            {
                MessageBox.Show("Нет выбранных элементов для удаления.");
            }
        
        }

        private void editRole_Click(object sender, RoutedEventArgs e)
        {
            Roles role = rolessDataGrid.SelectedItem as Roles;
            if (role == null) { MessageBox.Show("Выберите одну строку таблицы", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Exclamation); return; }
            EditRoleForm editRoleForm = new EditRoleForm(role);
            editRoleForm.ShowDialog();
            LoadRoles();
        }

        private void startTest_Click(object sender, RoutedEventArgs e)
        {
            if(GetSelectedTest() == null)
            {
                MessageBox.Show("Выберите тест для прохождения", "Ошибка");
                return;
            }
            TestAddForm testAddForm = new TestAddForm(TestAddForm.TestMode.Pass, GetSelectedTest().Id);
            testAddForm.ShowDialog();
            LoadResults();
        }

        private void Button_Click_6(object sender, RoutedEventArgs e)
        {
            try
            {
                var dialog = new FolderBrowserDialog
                {
                    Description = "Выберите папку для сохранения",
                    ShowNewFolderButton = true
                };

                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    string selectedPath = dialog.SelectedPath;
                    string directory = _databaseHelper.ExportDatabase(selectedPath);
                    MessageBox.Show($"База данных успешно экспортирована в: {directory}", "Уведомление");
                }
                else
                {
                    Console.WriteLine("Сохранение отменено пользователем.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Произошла ошибка: {ex.Message}");
            }
        }

        private void Button_Click_7(object sender, RoutedEventArgs e)
        {
            try
            {
                var dialog = new FolderBrowserDialog
                {
                    Description = "Выберите папку с копией базы для загрузки",
                    ShowNewFolderButton = true
                };

                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    string selectedPath = dialog.SelectedPath;
                    _databaseHelper.ImpotDatabase(selectedPath);
                    MessageBox.Show($"База данных успешно импортирована!", "Уведомление");
                }
                else
                {
                    Console.WriteLine("Выбор отменен пользователем.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Произошла ошибка: {ex.Message}");
            }
        }
    }

}
