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
using System.Diagnostics.Eventing.Reader;
using SaveFileDialog = Microsoft.Win32.SaveFileDialog;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;
using Path = System.IO.Path;
using CheckBox = System.Windows.Controls.CheckBox;
using wpf_тесты_для_обучения.Enums;
using Button = System.Windows.Controls.Button;
using ComboBox = System.Windows.Controls.ComboBox;
using System.Collections.ObjectModel;
using static MaterialDesignThemes.Wpf.Theme;
using DataGrid = System.Windows.Controls.DataGrid;
using Panel = System.Windows.Controls.Panel;
namespace wpf_тесты_для_обучения
{
    /// <summary>
    /// Логика взаимодействия для AdminForm.xaml
    /// </summary>
    public partial class AdminForm : Window
    {
        private DatabaseHelper _databaseHelper;
        List<List<int>> allFilters = new List<List<int>>
        {
            new List<int>(),
            new List<int>(),
            new List<int>()
        };
        List<List<int>> allFiltersUsers = new List<List<int>>
        {
            new List<int>()
        };
        public AdminForm(DatabaseHelper databaseHelper)
        {
            try
            {
                InitializeComponent();
                DataContext = this;
                _databaseHelper = databaseHelper;
                InitializeMenu();
                //checkBoxAllUsers.IsChecked = _databaseHelper.AllUsers;
                LoadAll();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при отключении базы данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }
        private void InitializeMenu()
        {
            // Создаем коллекцию пунктов меню
            var menuItems = new ObservableCollection<MenuItem>
        {
            new MenuItem
            {
                Title = "Тесты",
                Style = (Style)FindResource("MenuButtonStyle"),
                Level = 0,
                ClickAction = () =>{TablesVisibility(testsPanel); },
                Children = new List<MenuItem>
                {
                    new MenuItem
                    {
                        Title = "Управление",
                        Style = (Style)FindResource("SubMenuButtonStyle1"),
                        Level = 1,
                        Children = new List<MenuItem>
                        {
                            new MenuItem
                            {
                                Title = "Добавить",
                                Style = (Style)FindResource("SubMenuButtonStyle2"),
                                Level = 2,
                                ClickAction = () =>
                                {
                                    TestAddForm testAddForm = new TestAddForm(_databaseHelper);
                                    testAddForm.ShowDialog();
                                    LoadAll();
                                }
                            },
                            new MenuItem
                            {
                                Title = "Удалить",
                                Style = (Style)FindResource("SubMenuButtonStyle2"),
                                Level = 2,
                                ClickAction = deleteTest
                            },
                            new MenuItem
                            {
                                Title = "Изменить",
                                Style = (Style)FindResource("SubMenuButtonStyle2"),
                                Level = 2,
                                ClickAction = edtiTest
                            }
                        }
                    },
                    new MenuItem
                    {
                        Title = "Вопросы",
                        Style = (Style)FindResource("SubMenuButtonStyle1"),
                        ClickAction = () =>{TablesVisibility(QAPanel); },
                        Level = 1,
                        Children = new List<MenuItem>
                        {
                            new MenuItem
                            {
                                Title = "Добавить",
                                Style = (Style)FindResource("SubMenuButtonStyle2"),
                                Level = 2,
                                ClickAction = addQuestion
                            },
                            new MenuItem
                            {
                                Title = "Удалить",
                                Style = (Style)FindResource("SubMenuButtonStyle2"),
                                Level = 2,
                                ClickAction = deleteQuestions
                            },
                            new MenuItem
                            {
                                Title = "Изменить",
                                Style = (Style)FindResource("SubMenuButtonStyle2"),
                                Level = 2,
                                ClickAction = editQuestion
                            }
                        }
                    },
                    new MenuItem
                    {
                        Title = "Ответы",
                        Style = (Style)FindResource("SubMenuButtonStyle1"),
                        ClickAction = () =>{TablesVisibility(QAPanel); },
                        Level = 1,
                        Children = new List<MenuItem>
                        {
                            new MenuItem
                            {
                                Title = "Добавить",
                                Style = (Style)FindResource("SubMenuButtonStyle2"),
                                Level = 2,
                                ClickAction = addAnswer
                            },
                            new MenuItem
                            {
                                Title = "Удалить",
                                Style = (Style)FindResource("SubMenuButtonStyle2"),
                                Level = 2,
                                ClickAction = deleteAnswers
                            },
                            new MenuItem
                            {
                                Title = "Изменить",
                                Style = (Style)FindResource("SubMenuButtonStyle2"),
                                Level = 2,
                                ClickAction = editAnswer
                            }
                        }
                    },
                     new MenuItem
                    {
                        Title = "Режим просмотра",
                        Style = (Style)FindResource("SubMenuButtonStyle1"),
                        Level = 1,
                        ClickAction = viewTest
                        
                    },
                    new MenuItem
                    {
                        Title = "Импорт/Экспорт",
                        Style = (Style)FindResource("SubMenuButtonStyle1"),
                        Level = 1,
                        Children = new List<MenuItem>
                        {
                            new MenuItem
                            {
                                Title = "Импорт",
                                Style = (Style)FindResource("SubMenuButtonStyle2"),
                                Level = 2,
                                ClickAction = ImportTest
                            },
                            new MenuItem
                            {
                                Title = "Экспорт",
                                Style = (Style)FindResource("SubMenuButtonStyle2"),
                                Level = 2,
                                ClickAction = ExportTest
                            }
                        }
                    }
                }
            },
            new MenuItem
            {
                Title = "Пользователи",
                Style = (Style)FindResource("MenuButtonStyle"),
                Level = 0,
                ClickAction = () =>{TablesVisibility(usersPanel); },
                Children = new List<MenuItem>
                {
                    new MenuItem
                    {
                        Title = "Управление",
                        Style = (Style)FindResource("SubMenuButtonStyle1"),
                        Level = 1,
                        Children = new List<MenuItem>
                        {
                            new MenuItem
                            {
                                Title = "Добавить",
                                Style = (Style)FindResource("SubMenuButtonStyle2"),
                                Level = 2,
                                ClickAction = addUser
                            },
                            new MenuItem
                            {
                                Title = "Изменить",
                                Style = (Style)FindResource("SubMenuButtonStyle2"),
                                Level = 2,
                                ClickAction = editUser 
                            },
                            new MenuItem
                            {
                                Title = "Удалить",
                                Style = (Style)FindResource("SubMenuButtonStyle2"),
                                Level = 2,
                                ClickAction = deleteUser 
                            }
                        }
                    },
                    new MenuItem
                    {
                        Title = "Установить роль",
                        Style = (Style)FindResource("SubMenuButtonStyle1"),
                        Level = 1,
                        ClickAction = setUsersRole
                    },
                    new MenuItem
                    {
                        Title = "Адаптация пройдена",
                        Style = (Style)FindResource("SubMenuButtonStyle1"),
                        Level = 1,
                        ClickAction = setDoneAdaptUsers
                    }
                }
            },
            new MenuItem
            {
                Title = "Должности",
                Style = (Style)FindResource("MenuButtonStyle"),
                Level = 0,
                ClickAction = () =>{TablesVisibility(rolessPanel); },
                Children = new List<MenuItem>
                {
                    new MenuItem
                    {
                        Title = "Управление",
                        Style = (Style)FindResource("SubMenuButtonStyle1"),
                        Level = 1,
                        Children = new List<MenuItem>
                        {
                            new MenuItem
                            {
                                Title = "Добавить",
                                Style = (Style)FindResource("SubMenuButtonStyle2"),
                                Level = 2,
                                ClickAction = addRole
                            },
                            new MenuItem
                            {
                                Title = "Изменить",
                                Style = (Style)FindResource("SubMenuButtonStyle2"),
                                Level = 2,
                                ClickAction = editRole
                            },
                            new MenuItem
                            {
                                Title = "Удалить",
                                Style = (Style)FindResource("SubMenuButtonStyle2"),
                                Level = 2,
                                ClickAction = deleteRole
                            }
                        }
                    },
                }
            },
            new MenuItem
            {
                Title = "Результаты",
                Style = (Style)FindResource("MenuButtonStyle"),
                Level = 0,
                ClickAction = () =>{TablesVisibility(resultsPanel); },
                Children = new List<MenuItem>
                {
                    new MenuItem
                    {
                        Title = "Просмотреть ошибки",
                        Style = (Style)FindResource("SubMenuButtonStyle1"),
                        Level = 1,
                        ClickAction = ViewMistakes
                    },
                    new MenuItem
                    {
                        Title = "Отчеты",
                        Style = (Style)FindResource("SubMenuButtonStyle1"),
                        Level = 1,
                        Children = new List<MenuItem>
                        {
                            new MenuItem
                            {
                                Title = "по человеку",
                                Style = (Style)FindResource("SubMenuButtonStyle2"),
                                Level = 2,
                                ClickAction = UserReport,
                            },
                            new MenuItem
                            {
                                Title = "за период",
                                Style = (Style)FindResource("SubMenuButtonStyle2"),
                                Level = 2,
                                ClickAction = PeriodReport
                            },
                            new MenuItem
                            {
                                Title = "за все время",
                                Style = (Style)FindResource("SubMenuButtonStyle2"),
                                Level = 2,
                                ClickAction = AllTimeReport
                            }
                        }
                    },
                }
            },
            new MenuItem
            {
                Title = "БД, настройки",
                Style = (Style)FindResource("MenuButtonStyle"),
                Level = 0,
                ClickAction = () =>{TablesVisibility(dbPanel); },
            },
            new MenuItem
            {
                Title = "Выход",
                Style = (Style)FindResource("MenuButtonStyle"),
                Level = 0,
                ClickAction = () =>{
                    TablesVisibility(); 
                    LoginForm loginForm = new LoginForm();
                    loginForm.Show();
                    this.Close();
                },
            }
        };

            // Присваиваем созданную коллекцию свойству MenuItems нашего контрола
            MainMenu.MenuItems = menuItems;
        }
        private void TablesVisibility(Panel panelToShow = null)
        {
            // Список всех DataGrid на странице
            var allPanels = new List<Panel>
            {
                testsPanel, usersPanel, rolessPanel, resultsPanel, dbPanel, QAPanel
            };
            foreach (var panel in allPanels)
            {
                panel.Visibility = panel == panelToShow
                ? Visibility.Visible
                : Visibility.Collapsed;
            }
            
        }
        
        public void LoadAll()
        {
            LoadRoles();
            LoadTests();
            LoadUsers();
            LoadResults();
            LoadUsersComboBox();
            LoadRolesComboBox(rolesComboBox4);
            LoadRolesComboBox(rolesComboBox2);
            LoadTestsComboBox();
            LoadTestsToComboBox();
            LoadQuestions();
            LoadAnswers();
        }
        public void LoadUsers()
        {
            List<Users> users = _databaseHelper.GetUsersWithRoles();
            usersDataGrid.ItemsSource = null;
            usersDataGrid.ItemsSource = users;
        }
        public void LoadUsersComboBox()
        {
            List<Users> users = _databaseHelper.GetUsersWithRolesToCombobox();
            usersComboBox4.ItemsSource = null;
            usersComboBox4.ItemsSource = users;
        }
        public void LoadTestsComboBox()
        {
            List<Tests> tests = _databaseHelper.GetTestsList();
            testComboBox4.ItemsSource = null;
            testComboBox4.ItemsSource = tests;
        }
        public void LoadRolesComboBox(ComboBox comboBox)
        {
            List<Roles> roles = _databaseHelper.GetRolesList();
            comboBox.ItemsSource = null;
            comboBox.ItemsSource = roles;
        }
        public void LoadResults()
        {
            List<Results> results = _databaseHelper.GetResultsWithUserAndTest();
            resultsDataGrid.ItemsSource = results;
        }
        public void LoadTestsToComboBox()
        {
            List<Tests> tests = _databaseHelper.GetTestsList();
            testsComboBox1.ItemsSource = null;
            testsComboBox1.ItemsSource = tests;
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
            //questionsDataGrid.Items.Clear();
            questionsDataGrid.ItemsSource = null;
            questionsDataGrid.ItemsSource = questions;
        }
        public void LoadAnswers()
        {
            List<Answers> answers = _databaseHelper.GetAnswersList();
            answersDataGrid.ItemsSource = null;
            answersDataGrid.ItemsSource = answers;
        }

        private void addUser()
        {
            UserAddForm userAddForm = new UserAddForm(_databaseHelper);
            userAddForm.ShowDialog();
            LoadAll();
        }

        private void editUser()
        {
            Users selectedUser = GetSelectedUser();

            if (selectedUser != null)
            {
                UserEditForm editForm = new UserEditForm(selectedUser, _databaseHelper);
                editForm.ShowDialog();
                LoadAll();
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
            if (testsDataGrid.SelectedItems.Count > 1)
            {
                MessageBox.Show("Выберите только одну запись", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return null;
            }
            return testsDataGrid.SelectedItem as Tests;
        }
      
        private void deleteUser()
        {
            var selectedUsers = usersDataGrid.SelectedItems.Cast<Users>().ToList();

            if (selectedUsers == null || selectedUsers.Count == 0)
            {
                MessageBox.Show("Выберите хотя бы одного пользователя для удаления", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            MessageBoxResult result = MessageBox.Show(
                $"Вы действительно хотите удалить выбранных пользователей ({selectedUsers.Count})? Все связанные с ними данные также будут удалены.",
                "Подтверждение удаления",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result != MessageBoxResult.Yes)
                return;

            try
            {
                foreach (var user in selectedUsers)
                {

                    // 1. Удаляем Mistakes, где есть связь с Results
                    string deleteMistakesWithResultsQuery = $@"
                        DELETE FROM Mistakes
                        WHERE Result_Id IN (
                            SELECT Id FROM Results WHERE User_Id = {user.Id}
                        )";
                    _databaseHelper.ExecuteNonQuery(deleteMistakesWithResultsQuery);

                    // 2. Удаляем Mistakes напрямую связанные с User_Id
                    string deleteMistakesQuery = $@"DELETE FROM Mistakes WHERE User_Id = {user.Id}";
                    _databaseHelper.ExecuteNonQuery(deleteMistakesQuery);

                    // 3. Удаляем Results
                    string deleteResultsQuery = $@"DELETE FROM Results WHERE User_Id = {user.Id}";
                    _databaseHelper.ExecuteNonQuery(deleteResultsQuery);

                    // 4. Удаляем пользователя
                    string deleteUserQuery = $@"DELETE FROM Users WHERE Id = {user.Id}";
                    _databaseHelper.ExecuteNonQuery(deleteUserQuery);
                }

                MessageBox.Show("Пользователи успешно удалены.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                LoadAll();
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Ошибка при удалении пользователей: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        //        private void TabItem_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        //        {
        //            LoginForm loginForm = new LoginForm();
        //            loginForm.Show();
        //            this.Close();
        //        }

        private void viewTest()
        {
            try
            {
                if (GetSelectedTest() == null)
                {
                    MessageBox.Show("Выберите тест для просмотра", "Ошибка");
                    return;
                }
                TestViewPassForm testViewPassForm = new TestViewPassForm(_databaseHelper, TestMode.View, GetSelectedTest().Id);
                testViewPassForm.ShowDialog();
            }
            catch (Exception ex)
            {
                // Вывод сообщения об ошибке при поиске файла
                MessageBox.Show($"Исключение: {ex.Message}\n" +
                    $"Метод: {ex.TargetSite}\n" +
                    $"Трассировка стека: {ex.StackTrace}", "Ошибка поиска файла", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }
        //        private void startTest_Click(object sender, RoutedEventArgs e)
        //        {
        //            if (GetSelectedTest() == null)
        //            {
        //                MessageBox.Show("Выберите тест для прохождения", "Ошибка");
        //                return;
        //            }
        //            TestViewPassForm testViewPassForm = new TestViewPassForm(_databaseHelper, TestMode.Pass, GetSelectedTest().Id);
        //            testViewPassForm.ShowDialog();
        //            LoadResults();
        //            LoadAll();
        //        }
        private void edtiTest()
        {
            if (GetSelectedTest() == null)
            {
                MessageBox.Show("Выберите тест для редактирования", "Ошибка");
                return;
            }
            TestAddForm testAddForm = new TestAddForm(_databaseHelper, TestMode.Edit, GetSelectedTest().Id);
            testAddForm.ShowDialog();
            LoadAll();
        }
        private void deleteTest()
        {
            try
            {
                var selectedTests = testsDataGrid.SelectedItems.Cast<Tests>().ToList();

                if (selectedTests == null || selectedTests.Count == 0)
                {
                    MessageBox.Show("Выберите хотя бы один тест для удаления", "Ошибка");
                    return;
                }

                if (MessageBox.Show(
                        $"Вы действительно хотите удалить {selectedTests.Count} тест(ов)? Все связанные с ними вопросы, ответы, результаты и ошибки также будут удалены.",
                        "Подтверждение удаления",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Question) != MessageBoxResult.Yes)
                {
                    return;
                }

                // Получаем строку ID через запятую
                string idsString = string.Join(",", selectedTests.Select(t => t.Id));

                // 1. Удаляем Mistakes, связанные с результатами выбранных тестов
                string deleteMistakesQuery = $@"
            DELETE FROM Mistakes
            WHERE Result_Id IN (
                SELECT Id FROM Results WHERE Test_Id IN ({idsString})
            )";
                _databaseHelper.ExecuteNonQuery(deleteMistakesQuery);

                // 2. Удаляем RoleAccess
                string deleteRoleAccessQuery = $"DELETE FROM RoleAccess WHERE Test_Id IN ({idsString})";
                _databaseHelper.ExecuteNonQuery(deleteRoleAccessQuery);

                // 3. Удаляем Results
                string deleteResultsQuery = $"DELETE FROM Results WHERE Test_Id IN ({idsString})";
                _databaseHelper.ExecuteNonQuery(deleteResultsQuery);

                // 4. Удаляем Answers
                string deleteAnswersQuery = $@"
            DELETE FROM Answers
            WHERE Question_Id IN (
                SELECT Id FROM Questions WHERE Test_Id IN ({idsString})
            )";
                _databaseHelper.ExecuteNonQuery(deleteAnswersQuery);

                // 5. Удаляем Questions
                string deleteQuestionsQuery = $"DELETE FROM Questions WHERE Test_Id IN ({idsString})";
                _databaseHelper.ExecuteNonQuery(deleteQuestionsQuery);

                // 6. Удаляем Tests
                string deleteTestsQuery = $"DELETE FROM Tests WHERE Id IN ({idsString})";
                _databaseHelper.ExecuteNonQuery(deleteTestsQuery);

                MessageBox.Show("Удаление завершено успешно.", "Готово", MessageBoxButton.OK, MessageBoxImage.Information);

                LoadAll();
            }
            catch (SqlException sqlEx)
            {
                MessageBox.Show($"Ошибка базы данных: {sqlEx.Message}", "Ошибка SQL", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла непредвиденная ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        //        private void Button_Click_4(object sender, RoutedEventArgs e)
        //        {
        //            try
        //            {
        //                comboStackPanel.Visibility = Visibility.Visible;
        //                doubleGrid.Visibility = Visibility.Visible;
        //                answersQuestionsToolBar.Visibility = Visibility.Visible;
        //                testsDataGrid.Visibility = Visibility.Collapsed;
        //                testsToolBar.Visibility = Visibility.Collapsed;
        //                LoadTestsToComboBox();
        //                LoadAnswers();
        //                LoadQuestions();
        //                LoadAll();
        //            }
        //            catch (Exception ex)
        //            {
        //                MessageBox.Show($"Исключение: {ex.Message}\n" +
        //                  $"Метод: {ex.TargetSite}\n" +
        //                  $"Трассировка стека: {ex.StackTrace}", "Ошибка загрузки формы входа", MessageBoxButton.OK, MessageBoxImage.Exclamation);
        //            }
        //        }

        private void testsTitleComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Tests t1 = testsComboBox1.SelectedItem as Tests;
            if (t1 == null) return;
            t1.LoadQuestionsFromDatabase(_databaseHelper);
            foreach (Questions q1 in t1.Questions)
            {
                q1.LoadAnswersFromDatabase(_databaseHelper);
            }
            questionsDataGrid.ItemsSource = t1.Questions;
            List<Answers> allAnswers = t1.Questions.SelectMany(q => q.Answers).ToList();
            answersDataGrid.ItemsSource = allAnswers;
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
                LoadAll();
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
                LoadAll();
            }
            else
            {
                MessageBox.Show("Нет выбранных элементов для удаления.");
            }
        }

        private void addAnswer()
        {
            AddAnswerForm addAnswerForm = new AddAnswerForm(_databaseHelper);
            addAnswerForm.ShowDialog();
            LoadAll();
        }

        private void editAnswer()
        {
            Answers answer = answersDataGrid.SelectedItem as Answers;
            if (answer == null) { MessageBox.Show("Выберите строку таблицы с ответами", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Exclamation); return; }
            EditAnswerForm editAnswerForm = new EditAnswerForm(answer, _databaseHelper);
            editAnswerForm.ShowDialog();
            LoadAll();
        }

        private void addQuestion()
        {
            AddQuestionForm addQuestionForm = new AddQuestionForm(_databaseHelper);
            addQuestionForm.ShowDialog();
            LoadAll();
        }
      
        private void editQuestion()
        {
            Questions question = questionsDataGrid.SelectedItem as Questions;
            if (question == null) { MessageBox.Show("Выберите строку таблицы с ответами", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Exclamation); return; }
            EditQuestionForm editQuestionForm = new EditQuestionForm(question, _databaseHelper);
            editQuestionForm.ShowDialog();
            LoadAll();
        }
      

        private void addRole()
        {
            AddRoleForm addRoleForm = new AddRoleForm(_databaseHelper);
            addRoleForm.ShowDialog();
            LoadRoles();
            LoadAll();
        }
        private void deleteRole()
        {
            try
            {
                var selectedRoles = rolessDataGrid.SelectedItems.Cast<Roles>().ToList();
                if (!selectedRoles.Any())
                {
                    MessageBox.Show("Нет выбранных элементов для удаления.");
                    return;
                }

                // Подтверждение удаления
                if (MessageBox.Show(
                        $"Вы действительно хотите удалить {selectedRoles.Count} роль(ей)? Все связанные с ними доступы также будут удалены.",
                        "Подтверждение удаления",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Question) != MessageBoxResult.Yes)
                {
                    return;
                }

                // Получаем строку ID через запятую
                string roleIds = string.Join(",", selectedRoles.Select(r => r.Id));

                // Проверка: есть ли пользователи с этими ролями
                string queryUsersCountWithRole = $"SELECT COUNT(id) FROM Users WHERE Role_Id IN ({roleIds})";
                int userCount = (int)_databaseHelper.ExecuteScalar(queryUsersCountWithRole);

                if (userCount >= 1)
                {
                    MessageBox.Show(
                        $"Прежде чем удалять роль, установите пользователям с этой ролью/-ми другую роль\nПользователей: {userCount}",
                        "Уведомление",
                        MessageBoxButton.OK,
                        MessageBoxImage.Exclamation);
                    return;
                }

                // Удаляем из RoleAccess
                string deleteRoleAccessQuery = $"DELETE FROM RoleAccess WHERE Role_Id IN ({roleIds})";
                _databaseHelper.ExecuteNonQuery(deleteRoleAccessQuery);

                // Удаляем роли
                string deleteRolesQuery = $"DELETE FROM Roles WHERE Id IN ({roleIds})";
                _databaseHelper.ExecuteNonQuery(deleteRolesQuery);

                LoadRoles();
                LoadAll();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Исключение: {ex.Message}\n" +
                    $"Метод: {ex.TargetSite}\n" +
                    $"Трассировка стека: {ex.StackTrace}", "Ошибка удаления роли", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }


        private void editRole()
        {
            Roles role = rolessDataGrid.SelectedItem as Roles;
            if (role == null) { MessageBox.Show("Выберите одну строку таблицы", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Exclamation); return; }
            EditRoleForm editRoleForm = new EditRoleForm(role, _databaseHelper);
            editRoleForm.ShowDialog();
            LoadRoles();
            LoadAll();
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
                    LoadAll();
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

        private void ExportTest()
        {
            var selectedTest = GetSelectedTest();
            if (selectedTest == null)
            {
                MessageBox.Show("Выберите тест для экспорта", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                // Пользователь выбирает место сохранения
                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    Title = "Сохранить экспорт",
                    Filter = "CSV файлы (*.csv)|*.csv",
                    FileName = "Экспорт тестов.csv"
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    int directory = _databaseHelper.ExportTestToCsv(saveFileDialog.FileName, selectedTest);
                    if (directory == 1)
                        MessageBox.Show($"Файлы успешно созранены в: {saveFileDialog.FileName}", "Уведомление");
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

        private void ImportTest()
        {
            try
            {
                // Открытие диалога выбора файла
                OpenFileDialog openFileDialog = new OpenFileDialog
                {
                    Title = "Выберите файл для импорта",
                    Filter = "CSV файлы (*.csv)|*.csv"
                };

                if (openFileDialog.ShowDialog() == true)
                {
                    _databaseHelper.ImportTestFromCsv(openFileDialog.FileName);
                    LoadTests();
                    //LoadQuestions();
                    //LoadAnswers();
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

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            _databaseHelper.AllUsers = true;
            LoadAll();
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            _databaseHelper.AllUsers = false;
            LoadAll();
        }

        private void CheckBox2_Checked(object sender, RoutedEventArgs e)
        {
            //_databaseHelper.AllUsers = true;
            //LoadAll();
        }

        private void CheckBox2_Unchecked(object sender, RoutedEventArgs e)
        {
            //_databaseHelper.AllUsers = false;
            //LoadAll();
        }
        private void ViewMistakes()
        {
            if (resultsDataGrid.SelectedItems.Count > 1)
            {
                MessageBox.Show("Выберите только одну запись", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }
            Results results = resultsDataGrid.SelectedItem as Results;
            if (results == null)
            {
                MessageBox.Show("Выберите запись", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }
            TestViewMistakesForm testViewMistakesForm = new TestViewMistakesForm(_databaseHelper, results.TestId, results.Id, results.UserId);
            testViewMistakesForm.Show();

        }

        private void setUsersRole()
        {
            List<Users> selectedAnswers = usersDataGrid.SelectedItems.Cast<Users>().ToList();
            if (selectedAnswers.Count < 1)
            {
                 MessageBox.Show("Выберите пользователя(-ей)", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                 return;
            }
            int[] userIds = selectedAnswers.Select(user => user.Id).ToArray();
            SetRoleForm setRoleForm = new SetRoleForm(_databaseHelper, userIds);
            setRoleForm.ShowDialog();
            LoadAll();
        }

        private void setDoneAdaptUsers()
        {
            if (MessageBox.Show("Вы уверены что выбранные(-ый) пользователи(-ь) завершил(-и) адаптацию и прошл(-и) все тесты?", "уведомление",
               MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                List<Users> selectedUsers = usersDataGrid.SelectedItems.Cast<Users>().ToList();
                foreach (Users user in selectedUsers)
                {
                    string query = $@"UPDATE Users SET Is_Done = 1 WHERE Id = {user.Id}";
                    _databaseHelper.ExecuteNonQuery(query);
                }
                LoadAll();
            }
        }

        private void UserReport()
        {
            if (resultsDataGrid.SelectedItems.Count > 1)
            {
                MessageBox.Show("Выберите только одного пользователя", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }
            Results results = resultsDataGrid.SelectedItem as Results;
            if (results == null)
            {
                MessageBox.Show("Выберите пользователя", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }

            var dialog = new FolderBrowserDialog
            {
                Description = "Выберите папку для сохранения отчета",
                ShowNewFolderButton = true
            };

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string selectedPath = dialog.SelectedPath;
                Reports reports = new Reports(_databaseHelper);
                reports.GenerateUserReports(results.UserId, selectedPath, true);
            }
            else
            {
                Console.WriteLine("Выбор отменен пользователем.");
            };
        }

        private void AllTimeReport()
        {
            var dialog = new FolderBrowserDialog
            {
                Description = "Выберите папку для сохранения отчета",
                ShowNewFolderButton = true
            };

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string selectedPath = dialog.SelectedPath;
                Reports reports = new Reports(_databaseHelper);
                reports.GenerateAllUsersOverallReport(selectedPath);
            }
            else
            {
                Console.WriteLine("Выбор отменен пользователем.");
            };
        }
        private void usersComboBox4_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Users user = usersComboBox4.SelectedItem as Users;
            if (user != null)
            {
                if (allFilters[0].Contains(user.Id))
                {
                    usersComboBox4.SelectedIndex = -1;
                    return;
                }
                allFilters[0].Add(user.Id);
                Button button = new Button
                {
                    Style = (Style)FindResource("FilterButtonStyle"),
                    Tag = user.Id,//filtersDockPanel
                    Content = "пользователь: " + user.FirstName + " " + user.Name.Substring(0, 1) + ". " + user.Patronymic.Substring(0, 1) + "."
                };

                button.Loaded += (sender2, e2) =>
                {
                    var closeButton = FindVisualChild<Button>(button, b => b.Content?.ToString() == "×");
                    if (closeButton != null)
                    {
                        closeButton.Click += (s, args) =>
                        {

                            int id = Convert.ToInt32(button.Tag);
                            // 1. Удаляем кнопку из панели
                            filtersDockPanel4.Children.Remove(button);

                            allFilters[0].Remove(id);
                            // 2. Обновляем фильтрацию данных
                            List<Results> results2 = _databaseHelper.GetResultsWithUserAndTest(allFilters);
                            resultsDataGrid.ItemsSource = null;
                            resultsDataGrid.ItemsSource = results2;

                            args.Handled = true;
                        };
                    }
                };

                filtersDockPanel4.Children.Add(button);

                // Первоначальное применение фильтра
                List<Results> results = _databaseHelper.GetResultsWithUserAndTest(allFilters);
                resultsDataGrid.ItemsSource = null;
                resultsDataGrid.ItemsSource = results;
                usersComboBox4.SelectedIndex = -1;
            }

        }
        private void rolesComboBox4_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Roles role = rolesComboBox4.SelectedItem as Roles;
            if (role != null)
            {
                if (allFilters[1].Contains(role.Id))
                {
                    rolesComboBox4.SelectedIndex = -1;
                    return;
                }
                allFilters[1].Add(role.Id);
                Button button = new Button
                {
                    Style = (Style)FindResource("FilterButtonStyle"),
                    Tag = role.Id,//filtersDockPanel
                    Content = "должность: " + role.Title
                };

                button.Loaded += (sender2, e2) =>
                {
                    var closeButton = FindVisualChild<Button>(button, b => b.Content?.ToString() == "×");
                    if (closeButton != null)
                    {
                        closeButton.Click += (s, args) =>
                        {

                            int id = Convert.ToInt32(button.Tag);
                            // 1. Удаляем кнопку из панели
                            filtersDockPanel4.Children.Remove(button);

                            allFilters[1].Remove(id);
                            // 2. Обновляем фильтрацию данных
                            List<Results> results2 = _databaseHelper.GetResultsWithUserAndTest(allFilters);
                            resultsDataGrid.ItemsSource = null;
                            resultsDataGrid.ItemsSource = results2;

                            args.Handled = true;
                        };
                    }
                };

                filtersDockPanel4.Children.Add(button);

                // Первоначальное применение фильтра
                List<Results> results = _databaseHelper.GetResultsWithUserAndTest(allFilters);
                resultsDataGrid.ItemsSource = null;
                resultsDataGrid.ItemsSource = results;
            }
            rolesComboBox4.SelectedIndex = -1;

        }
        private void testsComboBox4_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Tests test = testComboBox4.SelectedItem as Tests;
            if (test != null)
            {
                if (allFilters[2].Contains(test.Id))
                {
                    testComboBox4.SelectedIndex = -1;
                    return;
                }
                allFilters[2].Add(test.Id);
                Button button = new Button
                {
                    Style = (Style)FindResource("FilterButtonStyle"),
                    Tag = test.Id,//filtersDockPanel
                    Content = "тест: " + test.Title
                };

                button.Loaded += (sender2, e2) =>
                {
                    var closeButton = FindVisualChild<Button>(button, b => b.Content?.ToString() == "×");
                    if (closeButton != null)
                    {
                        closeButton.Click += (s, args) =>
                        {

                            int id = Convert.ToInt32(button.Tag);
                            // 1. Удаляем кнопку из панели
                            filtersDockPanel4.Children.Remove(button);

                            allFilters[2].Remove(id);
                            // 2. Обновляем фильтрацию данных
                            List<Results> results2 = _databaseHelper.GetResultsWithUserAndTest(allFilters);
                            resultsDataGrid.ItemsSource = null;
                            resultsDataGrid.ItemsSource = results2;

                            args.Handled = true;
                        };
                    }
                };

                filtersDockPanel4.Children.Add(button);

                // Первоначальное применение фильтра
                List<Results> results = _databaseHelper.GetResultsWithUserAndTest(allFilters);
                resultsDataGrid.ItemsSource = null;
                resultsDataGrid.ItemsSource = results;
            }
            testComboBox4.SelectedIndex = -1;

        }
        private T FindVisualChild<T>(DependencyObject parent, Func<T, bool> predicate = null) where T : DependencyObject
        {
            if (parent == null) return null;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is T result && (predicate == null || predicate(result)))
                {
                    return result;
                }

                var foundChild = FindVisualChild<T>(child, predicate);
                if (foundChild != null) return foundChild;
            }

            return null;
        }

        private void rolesComboBox2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Roles role = rolesComboBox2.SelectedItem as Roles;
            if (role != null)
            {
                if (allFiltersUsers[0].Contains(role.Id))
                {
                    rolesComboBox2.SelectedIndex = -1;
                    return;
                }
                allFiltersUsers[0].Add(role.Id);
                Button button = new Button
                {
                    Style = (Style)FindResource("FilterButtonStyle"),
                    Tag = role.Id,//filtersDockPanel
                    Content = "должность: " + role.Title
                };

                button.Loaded += (sender2, e2) =>
                {
                    var closeButton = FindVisualChild<Button>(button, b => b.Content?.ToString() == "×");
                    if (closeButton != null)
                    {
                        closeButton.Click += (s, args) =>
                        {

                            int id = Convert.ToInt32(button.Tag);
                            // 1. Удаляем кнопку из панели
                            filtersDockPanel2.Children.Remove(button);

                            allFiltersUsers[0].Remove(id);
                            // 2. Обновляем фильтрацию данных
                            List<Users> users2 = _databaseHelper.GetUsersWithRoles(allFiltersUsers);
                            usersDataGrid.ItemsSource = null;
                            usersDataGrid.ItemsSource = users2;

                            args.Handled = true;
                        };
                    }
                };

                filtersDockPanel2.Children.Add(button);

                // Первоначальное применение фильтра
                List<Users> users = _databaseHelper.GetUsersWithRoles(allFiltersUsers);
                usersDataGrid.ItemsSource = null;
                usersDataGrid.ItemsSource = users;
            }
            rolesComboBox2.SelectedIndex = -1;

        }

        private void PeriodReport()
        {
            ReportPeriodForm reportPeriodForm = new ReportPeriodForm(_databaseHelper);
            reportPeriodForm.ShowDialog();
        }

        private void refreshButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            LoadAll();
        }

        //        private Questions GetSelectedQuestion()
        //        {
        //            return questionsDataGrid.SelectedItem as Questions;
        //        }
        //        private Answers GetSelectedAnswer()
        //        {
        //            return answersDataGrid.SelectedItem as Answers;
        //        }
        //        //new test
        //        private void Button_Click_2(object sender, RoutedEventArgs e)
        //        {
        //            TestAddForm testAddForm = new TestAddForm(_databaseHelper);
        //            testAddForm.ShowDialog();
        //            LoadAll();
        //        }

        //        private void reloadTests_Click(object sender, RoutedEventArgs e)
        //        {
        //            LoadTests();
        //        }

        //        private void reloadUsers_Click(object sender, RoutedEventArgs e)
        //        {
        //            LoadUsers();
        //        }

        //        private void reloadRoles_Click(object sender, RoutedEventArgs e)
        //        {
        //            LoadRoles();
        //        }

        //        private void reloadResults_Click(object sender, RoutedEventArgs e)
        //        {
        //            LoadResults();
        //        }

        //        private void reloadQuestionsAnswers_Click(object sender, RoutedEventArgs e)
        //        {
        //            LoadQuestions();
        //            LoadAnswers();
        //        }


        //        private void questionsDataGrid_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        //        {
        //            // Ищем, на какую ячейку кликнули
        //            DataGridCell cell = FindParent<DataGridCell>(e.OriginalSource as DependencyObject);

        //            if (cell != null)
        //            {
        //                // Получаем индекс колонки
        //                int columnIndex = cell.Column.DisplayIndex;

        //                // Проверяем, что кликнули именно во **вторую колонку** (индекс 1)
        //                if (columnIndex == 2)
        //                {
        //                    // Получаем данные строки, к которой принадлежит ячейка
        //                    var row = FindParent<DataGridRow>(cell);
        //                    if (row != null)
        //                    {
        //                        // Получаем объект, связанный с этой строкой
        //                        Questions question = row.Item as Questions;

        //                        if (question != null)
        //                        {
        //                            // Получаем содержимое ячейки (текст вопроса)
        //                            string cellContent = question.Image;

        //                            BaseQuestion.ShowFullImage(question.Image);
        //                        }
        //                    }
        //                }
        //            }
        //        }

        //        // Вспомогательный метод для поиска родителя нужного типа
        //        private static T FindParent<T>(DependencyObject child) where T : DependencyObject
        //        {
        //            while (child != null)
        //            {
        //                if (child is T parent)
        //                    return parent;
        //                child = VisualTreeHelper.GetParent(child);
        //            }
        //            return null;
        //        }

        //        private void questionsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //        {
        //            if (questionsDataGrid.SelectedItem is Questions selectedQuestion)
        //            {
        //                //MessageBox.Show($"Вы выбрали вопрос: {selectedQuestion.QuestionText}");
        //                Questions q1 = questionsDataGrid.SelectedItem as Questions;
        //                q1.LoadAnswersFromDatabase(_databaseHelper);
        //                answersDataGrid.ItemsSource = null;
        //                answersDataGrid.ItemsSource = q1.Answers;
        //            }

        //        }

        //        private void deleteQuestion_Click(object sender, RoutedEventArgs e)
        //        {
        //            deleteQuestions(); LoadAll();
        //        }

        //        private void deleteAnswer_Click(object sender, RoutedEventArgs e)
        //        {

        //            deleteAnswers(); LoadAll();
        //        }

        //        private void Button_Click_5(object sender, RoutedEventArgs e)
        //        {
        //            comboStackPanel.Visibility = Visibility.Collapsed;
        //            doubleGrid.Visibility = Visibility.Collapsed;
        //            answersQuestionsToolBar.Visibility = Visibility.Collapsed;
        //            testsDataGrid.Visibility = Visibility.Visible;
        //            testsToolBar.Visibility = Visibility.Visible;
        //            LoadTests(); LoadAll();
        //        }

        //        private void resetTables_Click(object sender, RoutedEventArgs e)
        //        {
        //            testsTitleComboBox.SelectedIndex = -1;
        //            LoadAll();
        //            LoadQuestions();
        //            LoadAnswers();
        //        }


    }
}
