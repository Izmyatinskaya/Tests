using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Windows.Markup;
using static Microsoft.Data.Sqlite.SqliteCommand;
using static Microsoft.Data.Sqlite.SqliteConnection;
using System.Windows.Controls.Primitives;
using System.IO;
using static System.Net.Mime.MediaTypeNames;
using Microsoft.Win32;
using System.Windows;
using System.Data.Common;
using System.Linq;
using System.Text;
using Microsoft.Office.Interop.Excel;
using DataTable = System.Data.DataTable;

namespace wpf_тесты_для_обучения.Properties
{

    public class DatabaseHelper
    {
        private string _connectionString;
        //        private SqlConnection _connection;
        //private  string dbPath = @"D:\Проекты\Тесты обучение WPF\wpf тесты для обучения\DB.mdf";
        //public int _currentRole { get; set; }
        public Users _currentUser { get; set; }
        private bool _allUsers;

        public bool AllUsers
        {
            get => _allUsers;
            set
            {
                if (_allUsers != value)
                {
                    _allUsers = value;

                    // Здесь реакция на изменение:
                    Properties.Settings.Default._allUsers = value;
                    Properties.Settings.Default.Save(); // Не забудь сохранять

                    // Можно вызвать событие или метод, если надо
                }
            }
        }
        private bool _hideAdministrators;

        public bool HideAdministrators
        {
            get => _hideAdministrators;
            set
            {
                if (_hideAdministrators != value)
                {
                    _hideAdministrators = value;

                    // Здесь реакция на изменение:
                    Properties.Settings.Default._hideAdministrators = value;
                    Properties.Settings.Default.Save(); // Не забудь сохранять

                    // Можно вызвать событие или метод, если надо
                }
            }
        }

        // Конструктор принимает строку подключения
        public DatabaseHelper(string connectionString)
        {
            _connectionString = connectionString;
        }

        // Метод для открытия соединения
        public SqlConnection GetConnection()
        {
            return new SqlConnection(_connectionString);
        }
        // Метод для выполнения SELECT-запроса
        public DataTable ExecuteSelectQuery(string query, SqlParameter[] parameters = null)
        {
            try
            {
                using (SqlConnection connection = GetConnection())
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        if (parameters != null)
                        {
                            command.Parameters.AddRange(parameters);
                        }

                        using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                        {
                            DataTable dataTable = new DataTable();
                            adapter.Fill(dataTable);
                            return dataTable;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Вывод сообщения об ошибке при поиске файла
                MessageBox.Show($"Исключение: {ex.Message}\n" +
                    $"Метод: {ex.TargetSite}\n" +
                    $"Трассировка стека: {ex.StackTrace}", "Ошибка SELECT запроса", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return null;
            }
        }

        // Метод для выполнения INSERT/UPDATE/DELETE-запроса
        public int ExecuteNonQuery(string query, SqlParameter[] parameters = null)
        {
            try
            {
                using (SqlConnection connection = GetConnection())
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        if (parameters != null)
                        {
                            command.Parameters.AddRange(parameters);
                        }

                        return command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                // Вывод сообщения об ошибке при поиске файла
                MessageBox.Show($"Исключение: {ex.Message}\n" +
                    $"Метод: {ex.TargetSite}\n" +
                    $"Трассировка стека: {ex.StackTrace}", "Ошибка выполнения INSERT/UPDATE/DELETE-запроса ", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return 0;
            }
        }

        // Метод для выполнения скалярного запроса (например, COUNT, MAX)
        public object ExecuteScalar(string query, SqlParameter[] parameters = null)
        {
            try
            {
                using (SqlConnection connection = GetConnection())
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        if (parameters != null)
                        {
                            command.Parameters.AddRange(parameters);
                        }

                        return command.ExecuteScalar();
                    }
                }
            }
            catch (Exception ex)
            {
                // Вывод сообщения об ошибке при поиске файла
                MessageBox.Show($"Исключение: {ex.Message}\n" +
                    $"Метод: {ex.TargetSite}\n" +
                    $"Трассировка стека: {ex.StackTrace}", "Ошибка числового запроса", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return null;
            }
        }

        public bool getIsDoneFactor(int roleID, int userID)
        {
            //всего тестов
            int totalTests = GetTestsList(roleID).Count;
            //пройдено тестов(прошли процентный порог)
            DataTable completedTests = GetCompletedTestsCount(userID);
            double averageOfAttempts = 0;
            foreach (DataRow row in completedTests.Rows)
            {
                averageOfAttempts += Convert.ToDouble(row[1]);
            }
            averageOfAttempts /= completedTests.Rows.Count;

            if (completedTests.Rows.Count == totalTests)
                return true;
            //testCountLabel.Text = $"Пройдено тестов: {completedTests.Rows.Count} из {totalTests}";
            // Метод для изменения пароля
            return false;
        }
        // Метод для получения пользователей с их ролями
        public List<Users> GetUsersWithRoles(List<List<int>> filters = null)
        {
            try
            {
                List<Users> users = new List<Users>();
                string query = "Select Users.Id as uId, First_Name, Name, Last_Name, Password, Roles.Id as rId,  Title, Is_Done from Users join Roles on Role_Id = Roles.Id";
                if (!AllUsers)
                { 
                    query += " where Is_Done = 0";
                    //if (HideAdministrators)
                    //    query += " AND Roles.Id != 1";
                }
                if (filters != null && filters.Count > 0 && filters[0] != null && filters[0].Count != 0)
                    query += $" AND Roles.Id in ({string.Join(",", filters[0])})";

                using (SqlConnection connection = GetConnection())
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                // Создаем объект роли
                                Roles role = new Roles
                                {
                                    Id = Convert.ToInt32(reader["rId"]),
                                    Title = reader["Title"].ToString()
                                };

                                // Создаем объект пользователя
                                Users user = new Users
                                {
                                    Id = Convert.ToInt32(reader["uId"]),
                                    FirstName = reader["First_Name"].ToString(),
                                    Name = reader["Name"].ToString(),
                                    Patronymic = reader["Last_Name"].ToString(),
                                    UserRole = role,
                                    Password = reader["Password"].ToString(),
                                    IsDone2 = getIsDoneFactor(role.Id, Convert.ToInt32(reader["uId"])),
                                    IsDone = (bool)reader["Is_Done"]
                                };

                                users.Add(user);
                            }
                        }
                    }
                }

                return users;
            }
            catch (Exception ex)
            {
                // Вывод сообщения об ошибке при поиске файла
                MessageBox.Show($"Исключение: {ex.Message}\n" +
                    $"Метод: {ex.TargetSite}\n" +
                    $"Трассировка стека: {ex.StackTrace}", "Ошибка получения пользователей с их ролями", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return null;
            }
        }
        public List<Users> GetUsersWithRolesToCombobox()
        {
            try
            {
                List<Users> users = new List<Users>();
                string query = "Select Users.Id as uId, First_Name, Name, Last_Name, Password, Roles.Id as rId,  Title, Is_Done " +
                    "from Users join Roles on Role_Id = Roles.Id Where Role_Id != 1";
                if (!AllUsers)
                    query += " AND Is_Done = 0";

                using (SqlConnection connection = GetConnection())
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                // Создаем объект роли
                                Roles role = new Roles
                                {
                                    Id = Convert.ToInt32(reader["rId"]),
                                    Title = reader["Title"].ToString()
                                };

                                // Создаем объект пользователя
                                Users user = new Users
                                {
                                    Id = Convert.ToInt32(reader["uId"]),
                                    FirstName = reader["First_Name"].ToString(),
                                    Name = reader["Name"].ToString(),
                                    Patronymic = reader["Last_Name"].ToString(),
                                    UserRole = role,
                                    Password = reader["Password"].ToString(),
                                    IsDone = (bool)reader["Is_Done"]
                                };

                                users.Add(user);
                            }
                        }
                    }
                }

                return users;
            }
            catch (Exception ex)
            {
                // Вывод сообщения об ошибке при поиске файла
                MessageBox.Show($"Исключение: {ex.Message}\n" +
                    $"Метод: {ex.TargetSite}\n" +
                    $"Трассировка стека: {ex.StackTrace}", "Ошибка получения пользователей с их ролями", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return null;
            }
        }

        public List<Roles> GetRolesList()
        {
            try
            {
                List<Roles> roles = new List<Roles>();
                string query = "Select * from Roles";

                using (SqlConnection connection = GetConnection())
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                // Создаем объект роли
                                Roles role = new Roles
                                {
                                    Id = Convert.ToInt32(reader["Id"]),
                                    Title = reader["Title"].ToString()
                                };



                                roles.Add(role);
                            }
                        }
                    }
                }

                return roles;
            }
            catch (Exception ex)
            {
                // Вывод сообщения об ошибке при поиске файла
                MessageBox.Show($"Исключение: {ex.Message}\n" +
                    $"Метод: {ex.TargetSite}\n" +
                    $"Трассировка стека: {ex.StackTrace}", "Ошибка получения списка ролей", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return null;
            }
        }

        public List<Tests> GetTestsList(int id)
        {
            try
            {
                List<Tests> tests = new List<Tests>();
                string query = $"Select Tests.Id, Title, Is_Completed from Tests join RoleAccess on Tests.Id = RoleAccess.Test_Id where RoleAccess.Role_Id={id}";

                using (SqlConnection connection = GetConnection())
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                // Создаем объект роли
                                Tests test = new Tests
                                {
                                    Id = Convert.ToInt32(reader["Id"]),
                                    Title = reader["Title"].ToString(),
                                    IsCompleted = Convert.ToDouble(reader["Is_Completed"]),
                                    _databaseHelper = this
                                };



                                tests.Add(test);
                            }
                        }
                    }
                }

                return tests;
            }
            catch (Exception ex)
            {
                // Вывод сообщения об ошибке при поиске файла
                MessageBox.Show($"Исключение: {ex.Message}\n" +
                    $"Метод: {ex.TargetSite}\n" +
                    $"Трассировка стека: {ex.StackTrace}", "Ошибка получения списка тестов по роли", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return null;
            }
        }

        public List<Tests> GetTestsList(bool resParam = false)
        {
            try
            {
                List<Tests> tests = new List<Tests>();
                string query = $"Select Tests.Id, Title from Tests";
                //if (resParam)
                //    query += ", Results Where Results.Test_Id = Tests.Id";
                using (SqlConnection connection = GetConnection())
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                // Создаем объект роли
                                Tests test = new Tests
                                {
                                    Id = Convert.ToInt32(reader["Id"]),
                                    Title = reader["Title"].ToString(),
                                    _databaseHelper = this
                                };
                                tests.Add(test);
                            }
                        }
                    }
                }

                return tests;
            }
            catch (Exception ex)
            {
                // Вывод сообщения об ошибке при поиске файла
                MessageBox.Show($"Исключение: {ex.Message}\n" +
                    $"Метод: {ex.TargetSite}\n" +
                    $"Трассировка стека: {ex.StackTrace}", "Ошибка получения списка тестов", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return null;
            }
        }

        public List<Results> GetResultsWithUserAndTest(List<List<int>> filters = null)
        {
            try
            {
                List<Results> results = new List<Results>();
                string query = @"
        SELECT r.Id AS ResultId, r.User_Id, r.Test_Id, r.Date, r.Score, 
               u.First_Name, u.Name, u.Last_Name, u.Role_Id, 
               t.Title AS TestTitle, 
               role.Title AS RoleTitle
        FROM Results r
        JOIN Users u ON r.User_Id = u.Id
        JOIN Tests t ON r.Test_Id = t.Id
        JOIN Roles role ON u.Role_Id = role.Id"; // Добавляем JOIN с таблицей Roles для получения названия роли
                if (!AllUsers)
                    query += " where u.Is_Done = 0 ";

                if (filters != null && filters.Count > 0 && filters[0] != null && filters[0].Count != 0)
                    query += $" AND r.User_Id in ({string.Join(",", filters[0])})";

                if (filters != null && filters.Count > 0 && filters[1] != null && filters[1].Count != 0)
                    //if (filters[1] != null && filters[1].Count != 0)
                    query += $" AND u.Role_Id in ({string.Join(",", filters[1])})";

                if (filters != null && filters.Count > 0 && filters[2] != null && filters[2].Count != 0)
                    //if (filters[2] != null && filters[2].Count != 0)
                    query += $" AND r.Test_Id in ({string.Join(",", filters[2])})";

                using (SqlConnection connection = GetConnection())
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                // Создаем объект роли
                                Roles userRole = new Roles
                                {
                                    Id = Convert.ToInt32(reader["Role_Id"]),
                                    Title = reader["RoleTitle"].ToString() // Получаем название роли из запроса
                                };

                                // Создаем объект пользователя
                                Users user = new Users
                                {
                                    Id = Convert.ToInt32(reader["User_Id"]),
                                    FirstName = reader["First_Name"].ToString(),
                                    Name = reader["Name"].ToString(),
                                    Patronymic = reader["Last_Name"].ToString(),
                                    UserRole = userRole
                                };

                                // Создаем объект теста
                                Tests test = new Tests
                                {
                                    Id = Convert.ToInt32(reader["Test_Id"]),
                                    Title = reader["TestTitle"].ToString(),
                                    _databaseHelper = this
                                };

                                // Создаем объект Results
                                Results result = new Results
                                {
                                    Id = Convert.ToInt32(reader["ResultId"]),
                                    UserId = Convert.ToInt32(reader["User_Id"]),
                                    TestId = Convert.ToInt32(reader["Test_Id"]),
                                    Score = Convert.ToDouble(reader["Score"]),
                                    User = user,
                                    Test = test,
                                    Date = Convert.ToDateTime(reader["Date"]),
                                };

                                results.Add(result);
                            }
                        }
                    }
                }

                return results;
            }
            catch (Exception ex)
            {
                // Вывод сообщения об ошибке при поиске файла
                MessageBox.Show($"Исключение: {ex.Message}\n" +
                    $"Метод: {ex.TargetSite}\n" +
                    $"Трассировка стека: {ex.StackTrace}", "Ошибка получения результатов прохождения", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return null;
            }
        }


        public List<Questions> GetQuestionsList()
        {
            try
            {
                List<Questions> questions = new List<Questions>();
                string query = $"Select Id, Test_Id, Question_Text, Image from Questions";

                using (SqlConnection connection = GetConnection())
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                // Создаем объект роли
                                Questions question = new Questions
                                {
                                    Id = Convert.ToInt32(reader["Id"]),
                                    TestId = Convert.ToInt32(reader["Test_Id"]),
                                    QuestionText = reader["Question_Text"].ToString(),
                                    Image = reader["Image"].ToString()
                                };
                                questions.Add(question);
                            }
                        }
                    }
                }

                return questions;
            }
            catch (Exception ex)
            {
                // Вывод сообщения об ошибке при поиске файла
                MessageBox.Show($"Исключение: {ex.Message}\n" +
                    $"Метод: {ex.TargetSite}\n" +
                    $"Трассировка стека: {ex.StackTrace}", "Ошибка получения списка вопросов", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return null;
            }
        }

        public List<Answers> GetAnswersList()
        {
            try
            {
                List<Answers> answers = new List<Answers>();
                string query = $"Select Id, Question_Id, Answer_Text, Is_Correct from Answers";

                using (SqlConnection connection = GetConnection())
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                // Создаем объект роли
                                Answers answer = new Answers
                                {
                                    Id = Convert.ToInt32(reader["Id"]),
                                    QuestionId = Convert.ToInt32(reader["Question_Id"]),
                                    AnswerText = reader["Answer_Text"].ToString(),
                                    IsCorrect = (bool)reader["Is_Correct"]
                                };
                                answers.Add(answer);
                            }
                        }
                    }
                }

                return answers;
            }
            catch (Exception ex)
            {
                // Вывод сообщения об ошибке при поиске файла
                MessageBox.Show($"Исключение: {ex.Message}\n" +
                    $"Метод: {ex.TargetSite}\n" +
                    $"Трассировка стека: {ex.StackTrace}", "Ошибка получения списка ответов", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return null;
            }
        }
        public List<Tests> GetTestsForRole(int roleId)
        {
            try
            {
                List<Tests> tests = new List<Tests>();

                string query = "SELECT T.Id, T.Title " +
                               "FROM Tests T " +
                               "JOIN RoleAccess RA ON T.Id = RA.Test_Id " +
                               "WHERE RA.Role_Id = @roleId";

                SqlParameter[] parameters = new SqlParameter[]
                {
        new SqlParameter("@roleId", roleId)
                };

                DataTable dt = ExecuteSelectQuery(query, parameters);

                foreach (DataRow row in dt.Rows)
                {
                    tests.Add(new Tests
                    {
                        Id = Convert.ToInt32(row["Id"]),
                        Title = row["Title"].ToString(),
                        _databaseHelper = this
                    });
                }

                return tests;
            }
            catch (Exception ex)
            {
                // Вывод сообщения об ошибке при поиске файла
                MessageBox.Show($"Исключение: {ex.Message}\n" +
                    $"Метод: {ex.TargetSite}\n" +
                    $"Трассировка стека: {ex.StackTrace}", "Ошибка получения теста по роли", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return null;
            }
        }

        public void ExportDataBase()
        {
            try
            {
                string connectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=\"D:\\Проекты\\Тесты обучение WPF\\wpf тесты для обучения\\DB.mdf\";Integrated Security=True";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Получаем список всех таблиц в базе данных
                    string getTablesQuery = "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE'";

                    using (SqlCommand command = new SqlCommand(getTablesQuery, connection))
                    {
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            string tableName = reader["TABLE_NAME"].ToString();
                            Console.WriteLine($"Начинаю выгрузку данных из таблицы: {tableName}");

                            // Для каждой таблицы выполняем запрос на получение всех данных
                            string selectQuery = $"SELECT * FROM {tableName}";
                            SqlDataAdapter dataAdapter = new SqlDataAdapter(selectQuery, connection);
                            DataTable dataTable = new DataTable();
                            dataAdapter.Fill(dataTable);

                            // Создаем файл CSV для каждой таблицы
                            string filePath = Path.Combine(@"C:\path\to\output", $"{tableName}.csv"); // Путь к папке, где будут сохраняться CSV файлы

                            using (StreamWriter sw = new StreamWriter(filePath))
                            {
                                // Записываем заголовки (названия столбцов)
                                string[] columnNames = new string[dataTable.Columns.Count];
                                for (int i = 0; i < dataTable.Columns.Count; i++)
                                {
                                    columnNames[i] = EscapeCsvValue(dataTable.Columns[i].ColumnName);
                                }
                                sw.WriteLine(string.Join(",", columnNames)); // Заголовки

                                // Записываем данные
                                foreach (DataRow row in dataTable.Rows)
                                {
                                    string[] rowData = new string[dataTable.Columns.Count];
                                    for (int i = 0; i < dataTable.Columns.Count; i++)
                                    {
                                        rowData[i] = EscapeCsvValue(row[i].ToString()); // Экранируем данные
                                    }
                                    sw.WriteLine(string.Join(",", rowData)); // Строка данных
                                }
                            }

                            Console.WriteLine($"Данные из таблицы {tableName} были успешно выгружены в файл {filePath}");
                        }
                    }
                }

                Console.WriteLine("Выгрузка завершена.");
            }
            catch (Exception ex)
            {
                // Вывод сообщения об ошибке при поиске файла
                MessageBox.Show($"Исключение: {ex.Message}\n" +
                    $"Метод: {ex.TargetSite}\n" +
                    $"Трассировка стека: {ex.StackTrace}", "Ошибка экспорта БД", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }

        // Метод для экранирования значений, чтобы они корректно отображались в CSV
        static string EscapeCsvValue(string value)
        {
            try
            {
                if (string.IsNullOrEmpty(value))
                    return "\"\""; // Для пустых строк, возвращаем пустую строку с кавычками

                // Если значение содержит запятую или кавычку, заключаем его в кавычки и экранируем кавычки
                if (value.Contains(",") || value.Contains("\""))
                {
                    value = "\"" + value.Replace("\"", "\"\"") + "\"";
                }

                return value;
            }
            catch (Exception ex)
            {
                // Вывод сообщения об ошибке при поиске файла
                MessageBox.Show($"Исключение: {ex.Message}\n" +
                    $"Метод: {ex.TargetSite}\n" +
                    $"Трассировка стека: {ex.StackTrace}", "Ошибка экранирования значений", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return null;
            }
        }

        public DataTable GetCompletedTestsCount(int userId)
        {
            try
            {
                string query = $"SELECT DISTINCT  r.Test_Id AS test,(SELECT COUNT(Id) FROM Results WHERE Results.User_Id = {userId} AND Results.Test_Id = r.Test_Id) AS attempt_count FROM Results r JOIN Tests t ON r.Test_Id = t.Id WHERE r.User_Id = {userId} AND r.Score >= (SELECT COUNT(q.Id) * (t.Is_Completed / 100) FROM Questions q WHERE q.Test_Id = r.Test_Id)";
                return ExecuteSelectQuery(query);
            }
            catch (Exception ex)
            {
                // Вывод сообщения об ошибке при поиске файла
                MessageBox.Show($"Исключение: {ex.Message}\n" +
                    $"Метод: {ex.TargetSite}\n" +
                    $"Трассировка стека: {ex.StackTrace}", "Ошибка получения кол-ва завершенных тестов", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return null;
            }
        }
        //public int GetCompletedTestsCount(int userId)
        //{
        //    string query = $"SELECT COUNT(DISTINCT Results.Test_Id) FROM Results JOIN Tests ON Results.Test_Id = Tests.Id WHERE Results.User_Id = {userId} AND Results.Score >= (SELECT COUNT(Questions.Id) * (Tests.Is_Completed / 100) FROM Questions WHERE Questions.Test_Id = Results.Test_Id)";
        //    return (int)ExecuteScalar(query);
        //}
        public int GetQuestionCount(int testId)
        {
            try
            {
                string query = $"SELECT COUNT(*) FROM Questions WHERE Test_Id = {testId}";
                return (int)ExecuteScalar(query);
            }
            catch (Exception ex)
            {
                // Вывод сообщения об ошибке при поиске файла
                MessageBox.Show($"Исключение: {ex.Message}\n" +
                    $"Метод: {ex.TargetSite}\n" +
                    $"Трассировка стека: {ex.StackTrace}", "Ошибка получения количества вопросов", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return -1;
            }
        }

        public int GetUserAttemptCount(int userId, int testId)
        {
            try
            {
                string query = $"SELECT COUNT(*) FROM Results WHERE User_Id = {userId} AND Test_Id = {testId}";
                return (int)ExecuteScalar(query);
            }
            catch (Exception ex)
            {
                // Вывод сообщения об ошибке при поиске файла
                MessageBox.Show($"Исключение: {ex.Message}\n" +
                    $"Метод: {ex.TargetSite}\n" +
                    $"Трассировка стека: {ex.StackTrace}", "Ошибка GetUserAttemptCount", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return -1;
            }
        }
        public bool IsTestCompleted(int userId, int testId)
        {
            try
            {
                Tests tests = new Tests(this);
                tests.Id = testId;
                string query = $"SELECT COUNT(*) FROM Results WHERE User_Id = {userId} AND Test_Id = {testId} AND (Score/{tests.Count})*100 >= (SELECT Is_Completed FROM Tests WHERE Id = {testId})";
                return (int)ExecuteScalar(query) > 0;
            }
            catch (Exception ex)
            {
                // Вывод сообщения об ошибке при поиске файла
                MessageBox.Show($"Исключение: {ex.Message}\n" +
                    $"Метод: {ex.TargetSite}\n" +
                    $"Трассировка стека: {ex.StackTrace}", "Ошибка проверки выполнения теста", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return false;
            }
        }
        public double GetUserResult(int userId, int testId)
        {
            try
            {
                string query = $" SELECT TOP 1 Score FROM Results WHERE User_Id = {userId} AND Test_Id = {testId} ORDER BY id DESC";

                DataTable resultTable = ExecuteSelectQuery(query);

                if (resultTable.Rows.Count > 0)
                {
                    return Convert.ToDouble(resultTable.Rows[0]["Score"]);
                }

                return 0; // Если записей нет, возвращаем 0
            }
            catch (Exception ex)
            {
                // Вывод сообщения об ошибке при поиске файла
                MessageBox.Show($"Исключение: {ex.Message}\n" +
                    $"Метод: {ex.TargetSite}\n" +
                    $"Трассировка стека: {ex.StackTrace}", "Ошибка получения результатов пользователя", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return -1;
            }

        }
        public List<Results> GetResultsByTestId(int testId)
        {
            try
            {
                List<Results> results = new List<Results>();
                string query = @"
        SELECT r.Id AS ResultId, r.User_Id, r.Test_Id, r.Score, r.Date, ROW_NUMBER() OVER (ORDER BY r.Id ASC) AS RowNumber,
               u.First_Name, u.Name, u.Last_Name, u.Role_Id, 
               t.Title AS TestTitle, 
               role.Title AS RoleTitle
        FROM Results r
        JOIN Users u ON r.User_Id = u.Id
        JOIN Tests t ON r.Test_Id = t.Id
        JOIN Roles role ON u.Role_Id = role.Id
        WHERE r.Test_Id = @testId AND r.User_Id = @userId"; // Добавлен фильтр по Test_Id

                using (SqlConnection connection = GetConnection())
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@testId", testId);
                        command.Parameters.AddWithValue("@userId", _currentUser.Id);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                // Создаем объект роли
                                Roles userRole = new Roles
                                {
                                    Id = Convert.ToInt32(reader["Role_Id"]),
                                    Title = reader["RoleTitle"].ToString()
                                };

                                // Создаем объект пользователя
                                Users user = new Users
                                {
                                    Id = Convert.ToInt32(reader["User_Id"]),
                                    FirstName = reader["First_Name"].ToString(),
                                    Name = reader["Name"].ToString(),
                                    Patronymic = reader["Last_Name"].ToString(),
                                    UserRole = userRole
                                };

                                // Создаем объект теста
                                Tests test = new Tests
                                {
                                    Id = Convert.ToInt32(reader["Test_Id"]),
                                    Title = reader["TestTitle"].ToString(),
                                    _databaseHelper = this
                                };

                                // Создаем объект результата
                                Results result = new Results
                                {
                                    Id = Convert.ToInt32(reader["ResultId"]),
                                    UserId = Convert.ToInt32(reader["User_Id"]),
                                    TestId = Convert.ToInt32(reader["Test_Id"]),
                                    Score = Convert.ToDouble(reader["Score"]),
                                    RowNumber = Convert.ToInt32(reader["RowNumber"]),
                                    User = user,
                                    Test = test
                                };

                                results.Add(result);
                            }
                        }
                    }
                }

                return results;
            }
            catch (Exception ex)
            {
                // Вывод сообщения об ошибке при поиске файла
                MessageBox.Show($"Исключение: {ex.Message}\n" +
                    $"Метод: {ex.TargetSite}\n" +
                    $"Трассировка стека: {ex.StackTrace}", "Ошибка получения результатов по Id теста и пользователя", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return null;
            }
        }


        //------------------------- DATABASE ----------------------------------
        public string ExportDatabase(string selectedPath)
        {
            //string outputFolderPath = selectedPath + $"Экспорт БД({DateTime.Now.Day}_{DateTime.Now.Month}_{DateTime.Now.Year})";
            string folderName = $"Экспорт БД({DateTime.Now.Day}_{DateTime.Now.Month}_{DateTime.Now.Year})";
            string outputFolderPath = Path.Combine(selectedPath, folderName);

            // Проверка на существование папки
            if (Directory.Exists(outputFolderPath))
            {
                MessageBoxResult result = MessageBox.Show(
                    $"Папка с таким именем уже существует. Хотите перезаписать ее?\r\n\r\nПерезаписать содержимое папки - Да\r\nСоздать новую - Нет",
                    "Подтверждение",
                    MessageBoxButton.YesNoCancel,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Cancel)
                {
                    return null;
                }
                else if (result == MessageBoxResult.No)
                {
                    // Генерируем уникальное имя папки
                    int counter = 1;
                    string newFolderPath;
                    do
                    {
                        newFolderPath = $"{outputFolderPath} ({counter})";
                        counter++;
                    } while (Directory.Exists(newFolderPath));

                    outputFolderPath = newFolderPath;
                }
                else
                {
                    // Удаляем старую папку и создаем заново
                    Directory.Delete(outputFolderPath, true);
                }
            }
            try
            {
                // Создаем папку для сохранения CSV файлов
                Directory.CreateDirectory(outputFolderPath);

                string[] tables = { "Users", "Tests", "Roles", "RoleAccess", "Results", "Questions", "Answers" };

                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    Console.WriteLine("Соединение с базой данных успешно установлено.");

                    foreach (var table in tables)
                    {
                        string query = $"SELECT * FROM {table}";
                        string outputCsvPath = Path.Combine(outputFolderPath, $"{table}.csv");

                        using (SqlCommand command = new SqlCommand(query, connection))
                        using (SqlDataReader reader = command.ExecuteReader())
                        using (StreamWriter writer = new StreamWriter(outputCsvPath, false, System.Text.Encoding.UTF8))
                        {
                            // Запись заголовков
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                writer.Write($"\"{reader.GetName(i)}\"");
                                if (i < reader.FieldCount - 1)
                                    writer.Write(";"); // Разделитель - точка с запятой
                            }
                            writer.WriteLine();

                            // Запись данных
                            while (reader.Read())
                            {
                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    var value = reader.GetValue(i)?.ToString().Replace("\"", "\"\""); // Экранирование кавычек
                                    writer.Write($"\"{value}\"");
                                    if (i < reader.FieldCount - 1)
                                        writer.Write(";"); // Разделитель - точка с запятой
                                }
                                writer.WriteLine();
                            }
                        }

                        // Копируем папку Images
                        string imagesSourcePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images");
                        string imagesDestinationPath = Path.Combine(outputFolderPath, "Images");

                        if (Directory.Exists(imagesSourcePath))
                        {
                            CopyDirectory(imagesSourcePath, imagesDestinationPath);
                        }

                        Console.WriteLine($"Таблица {table} успешно экспортирована в файл: {outputCsvPath}");
                    }
                }
                return outputFolderPath;
            }
            catch (Exception ex)
            {
                // Вывод сообщения об ошибке при поиске файла
                MessageBox.Show($"Исключение: {ex.Message}\n" +
                    $"Метод: {ex.TargetSite}\n" +
                    $"Трассировка стека: {ex.StackTrace}", "Ошибка Экспорта БД", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return null;
            }
        }
        public void ImpotDatabase(string selectedPath)
        {
            string inputFolderPath = selectedPath;

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    Console.WriteLine("Соединение с базой данных успешно установлено.");

                    // Импортировать каждую таблицу
                    string[] tables = { "Users", "Tests", "Roles", "RoleAccess", "Results", "Questions", "Answers" };

                    foreach (var table in tables)
                    {
                        string csvPath = Path.Combine(inputFolderPath, $"{table}.csv");
                        if (!File.Exists(csvPath))
                        {
                            Console.WriteLine($"Файл {csvPath} не найден, пропускаем импорт.");
                            continue;
                        }

                        // Запускаем импорт с учётом уникальности
                        ImportCsvWithUpsert(connection, csvPath, table);
                    }
                }
                // Восстанавливаем папку Images
                string imagesSourcePath = Path.Combine(selectedPath, "Images");
                string imagesDestinationPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images");

                if (Directory.Exists(imagesSourcePath))
                {
                    CopyDirectory(imagesSourcePath, imagesDestinationPath);
                }
            }
            catch (Exception ex)
            {
                // Вывод сообщения об ошибке при поиске файла
                MessageBox.Show($"Исключение: {ex.Message}\n" +
                    $"Метод: {ex.TargetSite}\n" +
                    $"Трассировка стека: {ex.StackTrace}", "Ошибка Импорта БД", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }

        }
        static void ImportCsvWithUpsert(SqlConnection connection, string csvPath, string tableName)
        {
            DataTable dataTable = ReadCsvToDataTable(csvPath);

            string idColumn = "Id"; // Предполагаем, что всегда есть колонка Id
            bool hasIdentityColumn = dataTable.Columns.Contains(idColumn);
            if (hasIdentityColumn)
            {
                string enableIdentityInsert = $"SET IDENTITY_INSERT {tableName} ON";
                using (SqlCommand enableCommand = new SqlCommand(enableIdentityInsert, connection))
                {
                    enableCommand.ExecuteNonQuery();
                }
            }

            foreach (DataRow row in dataTable.Rows)
            {
                try
                {
                    // Обработка значений REAL
                    foreach (DataColumn column in dataTable.Columns)
                    {
                        if (column.DataType == typeof(float) || column.ColumnName == "Score") // Обрабатываем колонку Score (REAL)
                        {
                            if (!float.TryParse(row[column].ToString(), out _))
                            {
                                throw new InvalidOperationException($"Некорректное значение для колонки {column.ColumnName}: {row[column]}");
                            }
                        }
                    }

                    // Проверяем, существует ли запись
                    string checkQuery = $"SELECT COUNT(*) FROM {tableName} WHERE {idColumn} = @Id";
                    using (SqlCommand checkCommand = new SqlCommand(checkQuery, connection))
                    {
                        checkCommand.Parameters.AddWithValue("@Id", row[idColumn]);
                        int count = (int)checkCommand.ExecuteScalar();

                        if (count > 0)
                        {
                            // Обновляем запись
                            string updateQuery = GenerateUpdateQuery(dataTable, tableName, idColumn);
                            using (SqlCommand updateCommand = new SqlCommand(updateQuery, connection))
                            {
                                foreach (DataColumn column in dataTable.Columns)
                                {
                                    updateCommand.Parameters.AddWithValue($"@{column.ColumnName}", row[column.ColumnName]);
                                }
                                updateCommand.ExecuteNonQuery();
                            }
                        }
                        else
                        {
                            // Вставляем новую запись
                            string insertQuery = GenerateInsertQuery(dataTable, tableName);
                            using (SqlCommand insertCommand = new SqlCommand(insertQuery, connection))
                            {
                                foreach (DataColumn column in dataTable.Columns)
                                {
                                    insertCommand.Parameters.AddWithValue($"@{column.ColumnName}", row[column.ColumnName]);
                                }
                                insertCommand.ExecuteNonQuery();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Произошла ошибка на строке данных: {string.Join(", ", row.ItemArray)}");
                    Console.WriteLine($"Ошибка: {ex.Message}");
                    if (ex.StackTrace != null)
                    {
                        Console.WriteLine($"Строка кода, вызвавшая ошибку: {ex.StackTrace}");
                    }
                }
            }

            if (hasIdentityColumn)
            {
                string disableIdentityInsert = $"SET IDENTITY_INSERT {tableName} OFF";
                using (SqlCommand disableCommand = new SqlCommand(disableIdentityInsert, connection))
                {
                    disableCommand.ExecuteNonQuery();
                }
            }

            Console.WriteLine($"Таблица {tableName} успешно импортирована с учётом уникальности.");
        }
        static string GenerateUpdateQuery(DataTable dataTable, string tableName, string idColumn)
        {
            string setClause = string.Join(", ", dataTable.Columns.Cast<DataColumn>()
                .Where(c => c.ColumnName != idColumn)
                .Select(c => $"{c.ColumnName} = @{c.ColumnName}"));

            return $"UPDATE {tableName} SET {setClause} WHERE {idColumn} = @{idColumn}";
        }
        static string GenerateInsertQuery(DataTable dataTable, string tableName)
        {
            string columns = string.Join(", ", dataTable.Columns.Cast<DataColumn>().Select(c => c.ColumnName));
            string values = string.Join(", ", dataTable.Columns.Cast<DataColumn>().Select(c => $"@{c.ColumnName}"));

            return $"INSERT INTO {tableName} ({columns}) VALUES ({values})";
        }
        static DataTable ReadCsvToDataTable(string filePath)
        {
            DataTable dataTable = new DataTable();

            using (StreamReader reader = new StreamReader(filePath))
            {
                string[] headers = reader.ReadLine().Split(';'); // Разделитель - точка с запятой
                foreach (var header in headers)
                {
                    dataTable.Columns.Add(header.Replace("\"", "").Trim());
                }

                while (!reader.EndOfStream)
                {
                    string[] rows = reader.ReadLine().Split(';');
                    DataRow dataRow = dataTable.NewRow();
                    for (int i = 0; i < headers.Length; i++)
                    {
                        dataRow[i] = rows[i].Replace("\"", "").Trim();
                    }
                    dataTable.Rows.Add(dataRow);
                }
            }

            return dataTable;
        }
        static void CopyDirectory(string sourceDir, string destinationDir)
        {
            Directory.CreateDirectory(destinationDir);

            foreach (var file in Directory.GetFiles(sourceDir))
            {
                string destFile = Path.Combine(destinationDir, Path.GetFileName(file));
                File.Copy(file, destFile, true);
            }

            foreach (var dir in Directory.GetDirectories(sourceDir))
            {
                string destDir = Path.Combine(destinationDir, Path.GetFileName(dir));
                CopyDirectory(dir, destDir);
            }
        }

        //-------------------------------------------------------------



        //--------------------- ЭКСПОРТ ТЕСТА --------------------
        public int ExportTestToCsv(string savePath, Tests test)
        {
            int testId = test.Id;
            string testTitle = test.Title;
            double isCompleted;

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    Console.WriteLine("Соединение с базой данных установлено.");

                    // Получаем Is_Completed из БД
                    isCompleted = GetTestCompletionStatus(connection, testId);

                    using (StreamWriter writer = new StreamWriter(savePath, false, Encoding.UTF8))
                    {
                        writer.WriteLine($"### Экспорт теста: {testTitle} ###");
                        writer.WriteLine($"### Завершен: {isCompleted} ###"); // Добавили статус теста
                        writer.WriteLine();

                        ExportTableToCsv(writer, connection, "Questions", $"WHERE Test_Id = {testId}");
                        ExportTableToCsv(writer, connection, "Answers", $"WHERE Question_Id IN (SELECT Id FROM Questions WHERE Test_Id = {testId})");
                    }
                }
                return 1;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка экспорта: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return 0;
            }
        }

        // Метод для получения Is_Completed
        private double GetTestCompletionStatus(SqlConnection connection, int testId)
        {
            string query = "SELECT Is_Completed FROM Tests WHERE Id = @TestId";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@TestId", testId)
            };

            // Выполняем запрос один раз
            DataTable result = ExecuteSelectQuery(query, parameters);
            return Convert.ToDouble(result.Rows[0]["Is_Completed"]);
        }


        private void ExportTableToCsv(StreamWriter writer, SqlConnection connection, string tableName, string whereClause)
        {
            writer.WriteLine($"### {tableName} ###");

            string query = $"SELECT * FROM {tableName} {whereClause}";

            using (SqlCommand command = new SqlCommand(query, connection))
            using (SqlDataReader reader = command.ExecuteReader())
            {
                // Запись заголовков
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    writer.Write($"\"{reader.GetName(i)}\"");
                    if (i < reader.FieldCount - 1)
                        writer.Write(";");
                }
                writer.WriteLine();

                // Запись данных
                while (reader.Read())
                {
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        var value = reader.GetValue(i)?.ToString().Replace("\"", "\"\""); // Экранирование кавычек
                        writer.Write($"\"{value}\"");
                        if (i < reader.FieldCount - 1)
                            writer.Write(";");
                    }
                    writer.WriteLine();
                }
            }

            writer.WriteLine(); // Пустая строка между таблицами
        }
        //--------------------------------------------------------

        //--------------------- ИМПОРТ ТЕСТА --------------------

        public void ImportTestFromCsv(string csvPath)
        {
            try
            {
                DataTable questionsTable, answersTable;
                int originalTestId, newTestId;
                string testTitle;
                double isCompleted;

                using (StreamReader reader = new StreamReader(csvPath, Encoding.UTF8))
                {
                    // Читаем заголовок теста
                    string titleLine = reader.ReadLine();
                    string completedLine = reader.ReadLine();

                    testTitle = titleLine.Replace("### Экспорт теста: ", "").Trim('#');
                    isCompleted = Convert.ToDouble(completedLine.Replace("### Завершен: ", "").Trim('#'));// completedLine.Contains("True"); // Читаем статус завершенности

                    reader.ReadLine();
                    questionsTable = ReadCsvToDataTable(reader, "### Questions ###");
                    answersTable = ReadCsvToDataTable(reader, "### Answers ###");
                }

                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    if (questionsTable.Rows.Count == 0)
                        throw new Exception("Файл не содержит вопросов!");

                    // Получаем ID теста
                    originalTestId = Convert.ToInt32(questionsTable.Rows[0]["Test_Id"]);

                    // Проверяем, есть ли тест в БД
                    bool testExists = CheckTestExists(connection, originalTestId);

                    if (testExists)
                    {
                        var result = MessageBox.Show(
                            $"Тест \"{testTitle}\" (ID {originalTestId}) уже существует.\nЗаменить его или создать копию?",
                            "Выбор действия",
                            MessageBoxButton.YesNoCancel,
                            MessageBoxImage.Question);

                        if (result == MessageBoxResult.Cancel)
                            return;

                        if (result == MessageBoxResult.Yes)
                        {
                            DeleteTestData(connection, originalTestId);
                            newTestId = originalTestId; // Используем тот же ID
                        }
                        else
                        {
                            newTestId = InsertNewTest(connection, testTitle, isCompleted);
                        }
                    }
                    else
                    {
                        newTestId = InsertNewTest(connection, testTitle, isCompleted);
                    }

                    // Вставляем вопросы и ответы с обновленными ID
                    Dictionary<int, int> questionIdMap = ImportQuestions(connection, questionsTable, newTestId);
                    ImportAnswers(connection, answersTable, questionIdMap);
                }

                MessageBox.Show("Тест успешно импортирован!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при импорте: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Вставляет новый тест с учетом Is_Completed
        private int InsertNewTest(SqlConnection connection, string originalTitle, double isCompleted)
        {
            string newTitle = $"Копия {originalTitle}";
            string query = "INSERT INTO Tests (Title, Is_Completed) OUTPUT INSERTED.Id VALUES (@Title, @IsCompleted)";
            using (SqlCommand cmd = new SqlCommand(query, connection))
            {
                cmd.Parameters.AddWithValue("@Title", newTitle);
                cmd.Parameters.AddWithValue("@IsCompleted", isCompleted);
                return (int)cmd.ExecuteScalar();
            }
        }

        // Получает название теста по ID
        private string GetTestTitle(SqlConnection connection, int testId)
        {
            string query = "SELECT Title FROM Tests WHERE Id = @TestId";
            using (SqlCommand cmd = new SqlCommand(query, connection))
            {
                cmd.Parameters.AddWithValue("@TestId", testId);
                return cmd.ExecuteScalar()?.ToString() ?? $"Без названия ({testId})";
            }
        }

        // Проверяет, существует ли тест в таблице Tests
        private bool CheckTestExists(SqlConnection connection, int testId)
        {
            string query = "SELECT COUNT(*) FROM Tests WHERE Id = @TestId";
            using (SqlCommand cmd = new SqlCommand(query, connection))
            {
                cmd.Parameters.AddWithValue("@TestId", testId);
                return (int)cmd.ExecuteScalar() > 0;
            }
        }

        // Вставляет новый тест и возвращает его ID
        private int InsertNewTest(SqlConnection connection, string originalTitle)
        {
            string newTitle = $"Копия {originalTitle}";
            string query = "INSERT INTO Tests (Title) OUTPUT INSERTED.Id VALUES (@Title)";
            using (SqlCommand cmd = new SqlCommand(query, connection))
            {
                cmd.Parameters.AddWithValue("@Title", newTitle);
                return (int)cmd.ExecuteScalar();
            }
        }

        // Вставляет вопросы и возвращает отображение старых ID на новые
        private Dictionary<int, int> ImportQuestions(SqlConnection connection, DataTable questionsTable, int newTestId)
        {
            Dictionary<int, int> questionIdMap = new Dictionary<int, int>();

            foreach (DataRow row in questionsTable.Rows)
            {
                string query = "INSERT INTO Questions (Test_Id, Question_Text) OUTPUT INSERTED.Id VALUES (@TestId, @Text)";
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@TestId", newTestId);
                    cmd.Parameters.AddWithValue("@Text", row["Question_Text"]);

                    int newQuestionId = (int)cmd.ExecuteScalar();
                    int oldQuestionId = Convert.ToInt32(row["Id"]);

                    questionIdMap[oldQuestionId] = newQuestionId;
                }
            }

            return questionIdMap;
        }

        // Вставляет ответы, используя новую карту ID вопросов
        private void ImportAnswers(SqlConnection connection, DataTable answersTable, Dictionary<int, int> questionIdMap)
        {
            foreach (DataRow row in answersTable.Rows)
            {
                int oldQuestionId = Convert.ToInt32(row["Question_Id"]);

                if (!questionIdMap.ContainsKey(oldQuestionId))
                    continue;

                int newQuestionId = questionIdMap[oldQuestionId];

                string query = "INSERT INTO Answers (Question_Id, Answer_Text, Is_Correct) VALUES (@QuestionId, @Text, @IsCorrect)";
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@QuestionId", newQuestionId);
                    cmd.Parameters.AddWithValue("@Text", row["Answer_Text"]);
                    cmd.Parameters.AddWithValue("@IsCorrect", row["Is_Correct"]);
                    cmd.ExecuteNonQuery();
                }
            }
        }


        private void DeleteTestData(SqlConnection connection, int testId)
        {
            using (SqlCommand cmd = new SqlCommand("DELETE FROM Answers WHERE Question_Id IN (SELECT Id FROM Questions WHERE Test_Id = @TestId)", connection))
            {
                cmd.Parameters.AddWithValue("@TestId", testId);
                cmd.ExecuteNonQuery();
            }
            using (SqlCommand cmd = new SqlCommand("DELETE FROM Questions WHERE Test_Id = @TestId", connection))
            {
                cmd.Parameters.AddWithValue("@TestId", testId);
                cmd.ExecuteNonQuery();
            }
            using (SqlCommand cmd = new SqlCommand("DELETE FROM Tests WHERE Id = @TestId", connection))
            {
                cmd.Parameters.AddWithValue("@TestId", testId);
                cmd.ExecuteNonQuery();
            }
        }

        private DataTable ReadCsvToDataTable(StreamReader reader, string expectedHeader)
        {
            DataTable dataTable = new DataTable();

            string header = reader.ReadLine();
            if (!header.Contains(expectedHeader))
                throw new Exception($"Ожидался заголовок {expectedHeader}, но получен: {header}");

            header = reader.ReadLine();
            string[] columns = header.Split(';').Select(c => c.Replace("\"", "").Trim()).ToArray();
            foreach (string col in columns)
                dataTable.Columns.Add(col);

            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();
                if (string.IsNullOrWhiteSpace(line))
                    break;

                string[] values = line.Split(';');
                DataRow row = dataTable.NewRow();
                for (int i = 0; i < columns.Length; i++)
                    row[i] = values[i].Replace("\"", "").Trim();

                dataTable.Rows.Add(row);
            }

            return dataTable;
        }
    }

}


