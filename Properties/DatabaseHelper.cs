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

namespace wpf_тесты_для_обучения.Properties
{

    public class DatabaseHelper
    {
        private string _connectionString;
        private SqlConnection _connection;
        private  string dbPath = @"D:\Проекты\Тесты обучение WPF\wpf тесты для обучения\DB.mdf";
        //public int _currentRole { get; set; }
        public Users _currentUser { get; set; }
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

        private static void DetachDatabase()
        {
            try
            {
                string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;Integrated Security=True";
                string databaseName = "DB";  // Имя базы данных

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Проверяем, существует ли база данных
                    string checkDbCommand = $"SELECT database_id FROM sys.databases WHERE name = '{databaseName}'";
                    using (SqlCommand command = new SqlCommand(checkDbCommand, connection))
                    {
                        object result = command.ExecuteScalar();

                        if (result == null)
                        {
                            MessageBox.Show($"База данных {databaseName} не найдена.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }
                    }

                    // Если база данных существует, выполняем detach
                    string detachCommand = $"sp_detach_db '{databaseName}'";
                    using (SqlCommand command = new SqlCommand(detachCommand, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("База данных успешно отключена.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при отключении базы данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private static void AttachDatabase(string databaseFilePath)
        {
            try
            {
                string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;Integrated Security=True";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Подключаем базу данных
                    string attachCommand = $"sp_attach_db 'DB', '{databaseFilePath}', '{databaseFilePath.Replace(".mdf", "_log.ldf")}'";
                    using (SqlCommand command = new SqlCommand(attachCommand, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("База данных успешно подключена.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при подключении базы данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Метод для закрытия соединения
        private void CloseDatabaseConnection()
        {
            DetachDatabase();
        }


        // Метод для выполнения SELECT-запроса
        public DataTable ExecuteSelectQuery(string query, SqlParameter[] parameters = null)
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

        // Метод для выполнения INSERT/UPDATE/DELETE-запроса
        public int ExecuteNonQuery(string query, SqlParameter[] parameters = null)
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

        // Метод для выполнения скалярного запроса (например, COUNT, MAX)
        public object ExecuteScalar(string query, SqlParameter[] parameters = null)
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

        // Метод для получения пользователей с их ролями
        public List<Users> GetUsersWithRoles()
        {
            List<Users> users = new List<Users>();
            string query = "Select Users.Id as uId, First_Name, Name, Last_Name, Password, Roles.Id as rId,  Title from Users join Roles on Role_Id = Roles.Id\r\n";

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
                                Password = reader["Password"].ToString()
                            };

                            users.Add(user);
                        }
                    }
                }
            }

            return users;
        }

        public List<Roles> GetRolesList()
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

        public List<Tests> GetTestsList(int id)
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
                                IsCompleted = Convert.ToDouble(reader["Is_Completed"])
                            };



                            tests.Add(test);
                        }
                    }
                }
            }

            return tests;
        }

        public List<Tests> GetTestsList()
        {
            List<Tests> tests = new List<Tests>();
            string query = $"Select Tests.Id, Title from Tests";

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
                                Title = reader["Title"].ToString()
                            };
                            tests.Add(test);
                        }
                    }
                }
            }

            return tests;
        }

        public List<Results> GetResultsWithUserAndTest()
        {
            List<Results> results = new List<Results>();
            string query = @"
        SELECT r.Id AS ResultId, r.User_Id, r.Test_Id, r.Score, 
               u.First_Name, u.Name, u.Last_Name, u.Role_Id, 
               t.Title AS TestTitle, 
               role.Title AS RoleTitle
        FROM Results r
        JOIN Users u ON r.User_Id = u.Id
        JOIN Tests t ON r.Test_Id = t.Id
        JOIN Roles role ON u.Role_Id = role.Id"; // Добавляем JOIN с таблицей Roles для получения названия роли

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
                                Title = reader["TestTitle"].ToString()
                            };

                            // Создаем объект Results
                            Results result = new Results
                            {
                                Id = Convert.ToInt32(reader["ResultId"]),
                                UserId = Convert.ToInt32(reader["User_Id"]),
                                TestId = Convert.ToInt32(reader["Test_Id"]),
                                Score = Convert.ToDouble(reader["Score"]),
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


        public List<Questions> GetQuestionsList()
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

        public List<Answers> GetAnswersList()
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
        public List<Tests> GetTestsForRole(int roleId)
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
                    Title = row["Title"].ToString()
                });
            }

            return tests;
        }

        public void ExportDataBase()
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

        // Метод для экранирования значений, чтобы они корректно отображались в CSV
        static string EscapeCsvValue(string value)
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

        public int GetCompletedTestsCount(int userId)
        {
            string query = $"SELECT COUNT(DISTINCT Test_Id) FROM Results WHERE User_Id = {userId} AND Score > 0";
            return (int)ExecuteScalar(query);
        }

        public int GetQuestionCount(int testId)
        {
            string query = $"SELECT COUNT(*) FROM Questions WHERE Test_Id = {testId}";
            return (int)ExecuteScalar(query);
        }

        public int GetUserAttemptCount(int userId, int testId)
        {
            string query = $"SELECT COUNT(*) FROM Results WHERE User_Id = {userId} AND Test_Id = {testId}";
            return (int)ExecuteScalar(query);
        }
        public bool IsTestCompleted(int userId, int testId)
        {
            Tests tests = new Tests();
            tests.Id = testId;
            string query = $"SELECT COUNT(*) FROM Results WHERE User_Id = {userId} AND Test_Id = {testId} AND (Score/{tests.Count})*100 >= (SELECT Is_Completed FROM Tests WHERE Id = {testId})";
            return (int)ExecuteScalar(query) > 0;
        }
        public double GetUserResult(int userId, int testId)
        {
            string query = $" SELECT TOP 1 Score FROM Results WHERE User_Id = {userId} AND Test_Id = {testId} ORDER BY id DESC";

            DataTable resultTable = ExecuteSelectQuery(query);

            if (resultTable.Rows.Count > 0)
            {
                return Convert.ToDouble(resultTable.Rows[0]["Score"]);
            }

            return 0; // Если записей нет, возвращаем 0

        }
        public List<Results> GetResultsByTestId(int testId)
        {
            List<Results> results = new List<Results>();
            string query = @"
        SELECT r.Id AS ResultId, r.User_Id, r.Test_Id, r.Score, 
               u.First_Name, u.Name, u.Last_Name, u.Role_Id, 
               t.Title AS TestTitle, 
               role.Title AS RoleTitle
        FROM Results r
        JOIN Users u ON r.User_Id = u.Id
        JOIN Tests t ON r.Test_Id = t.Id
        JOIN Roles role ON u.Role_Id = role.Id
        WHERE r.Test_Id = @testId"; // Добавлен фильтр по Test_Id

            using (SqlConnection connection = GetConnection())
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@testId", testId);

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
                                Title = reader["TestTitle"].ToString()
                            };

                            // Создаем объект результата
                            Results result = new Results
                            {
                                Id = Convert.ToInt32(reader["ResultId"]),
                                UserId = Convert.ToInt32(reader["User_Id"]),
                                TestId = Convert.ToInt32(reader["Test_Id"]),
                                Score = Convert.ToDouble(reader["Score"]),
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

                        Console.WriteLine($"Таблица {table} успешно экспортирована в файл: {outputCsvPath}");
                    }
                }
                return outputFolderPath;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Произошла ошибка: {ex.Message}");
                return null;
            }
        }
        public  void ImpotDatabase(string selectedPath)
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
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Произошла ошибка: {ex.Message}");
                if (ex.StackTrace != null)
                {
                    Console.WriteLine($"Строка, где произошла ошибка: {ex.StackTrace}");
                }
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
    }
}


