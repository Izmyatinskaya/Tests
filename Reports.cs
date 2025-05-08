using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Microsoft.Office.Interop.Word;
//using Microsoft.Office.Interop.Excel;

using System.Xml.Linq;
using wpf_тесты_для_обучения.Properties;
using DataTable = System.Data.DataTable;
using System.Windows.Documents;

namespace wpf_тесты_для_обучения
{
    internal class Reports
    {
        private readonly DatabaseHelper _dbHelper;

        public Reports(DatabaseHelper dbHelper)
        {
            _dbHelper = dbHelper;
        }

        /// <summary>
        /// Генерирует отчеты по пользователю в указанных форматах
        /// </summary>
        public void GenerateUserReports(int userId, string outputDirectory, bool generateWord = true, bool generateExcel = true)
        {
            var user = GetUser(userId);
            if (user == null)
                throw new ArgumentException("Пользователь не найден");

            var results = GetUserResults(userId);
            if (!results.Any())
                throw new InvalidOperationException("Нет данных о результатах тестирования");

            // Создаем директорию, если не существует
            Directory.CreateDirectory(outputDirectory);

            string baseFileName = $"Отчет_по_пользователю_{user.FirstName}_{user.Name}_{DateTime.Now:dd_MM_yyyy}";

            if (generateWord)
            {
                string wordFilePath = Path.Combine(outputDirectory, $"{baseFileName}.docx");
                GenerateWordReport(user, results, wordFilePath);
            }

            if (generateExcel)
            {
                string excelFilePath = Path.Combine(outputDirectory, $"{baseFileName}.xlsx");
                //GenerateExcelReport(user, results, excelFilePath);
            }
        }

        /// <summary>
        /// Генерирует отчет в Word
        /// </summary>
        private void GenerateWordReport(Users user, List<Results> results, string filePath)
        {
            var wordApp = new Application();
            var doc = wordApp.Documents.Add();
            try
            {
                // 1. Заголовок отчета
                AddWordHeader(doc, user);

                // 2. Информация о пользователе
                AddWordUserInfo(doc, user);

                // 3. Сводная таблица результатов
                AddWordResultsTable(doc, results);

                // 4. Статистика
                AddWordStatistics(doc, results);

                // 5. Детализация по тестам (опционально)
                AddWordTestDetails(doc, results);

                foreach (Microsoft.Office.Interop.Word.Paragraph paragraph in doc.Paragraphs)
                {
                    paragraph.FirstLineIndent = 0; // Убираем отступ первой строки
                    //paragraph.LeftIndent = 0;      // Убираем левый отступ
                }
                doc.SaveAs2(filePath);
            }
            finally
            {
                doc.Close();
                wordApp.Quit();
            }
        }

        public void GenerateAllUsersReport(DateTime startDate, DateTime endDate, string outputDirectory)
        {
            // Создаем директорию, если не существует
            Directory.CreateDirectory(outputDirectory);

            // Получаем всех пользователей
            var users = GetAllUsers();
            if (!users.Any())
            {
                throw new InvalidOperationException("Нет данных о пользователях");
            }

            // Создаем общий отчет
            string baseFileName = $"Отчет за период с {startDate:dd_MM_yyyy}-{endDate:dd_MM_yyyy}";
            string wordFilePath = Path.Combine(outputDirectory, $"{baseFileName}.docx");

            var wordApp = new Application();
            var doc = wordApp.Documents.Add();

            try
            {
                // 1. Заголовок отчета
                AddAllUsersReportHeader(doc, startDate, endDate);

                // 2. Сводная статистика по всем пользователям
                AddSummaryStatistics(doc, users, startDate, endDate);

                // 3. Детализация по каждому пользователю
                foreach (var user in users)
                {
                    var results = GetUserResultsByDate(user.Id, startDate, endDate);
                    if (!results.Any()) continue;

                    // Добавляем раздел для пользователя
                    AddUserSectionHeader(doc, user);

                    // Добавляем таблицу результатов
                    AddWordResultsTable(doc, results);

                    // Добавляем статистику по пользователю
                    AddWordStatistics(doc, results);

                    // Добавляем разделитель между пользователями
                    doc.Paragraphs.Add().Range.InsertParagraphAfter();
                    doc.Paragraphs.Add().Range.InsertParagraphAfter();
                }

                // Сохраняем документ
                doc.SaveAs2(wordFilePath);
            }
            finally
            {
                doc.Close();
                wordApp.Quit();
            }
        }

        #region Additional Word Components for All Users Report

        public void GenerateAllUsersOverallReport(string outputDirectory)
        {
            // Создаем директорию, если не существует
            Directory.CreateDirectory(outputDirectory);

            // Получаем всех пользователей
            var users = GetAllUsers();
            if (!users.Any())
            {
                throw new InvalidOperationException("Нет данных о пользователях");
            }

            // Создаем общий отчет
            string baseFileName = $"Общий отчет по пользователям за все время {DateTime.Now:dd_MM_yyyy}";
            string wordFilePath = Path.Combine(outputDirectory, $"{baseFileName}.docx");

            var wordApp = new Application();
            var doc = wordApp.Documents.Add();

            try
            {
                // 1. Заголовок отчета
                AddOverallReportHeader(doc);

                // 2. Сводная статистика по всем пользователям за все время
                AddOverallSummaryStatistics(doc, users);

                // 3. Детализация по каждому пользователю
                foreach (var user in users)
                {
                    var results = GetUserResults(user.Id);
                    if (!results.Any()) continue;

                    // Добавляем раздел для пользователя
                    AddUserSectionHeader(doc, user);

                    // Добавляем таблицу результатов
                    AddWordResultsTable(doc, results);

                    // Добавляем статистику по пользователю
                    AddWordStatistics(doc, results);

                    // Добавляем разделитель между пользователями
                    doc.Paragraphs.Add().Range.InsertParagraphAfter();
                    doc.Paragraphs.Add().Range.InsertParagraphAfter();
                }

                // Сохраняем документ
                doc.SaveAs2(wordFilePath);
            }
            finally
            {
                doc.Close();
                wordApp.Quit();
            }
        }

        #region Additional Word Components for Overall Report

        private void AddOverallReportHeader(Document doc)
        {
            var title = doc.Paragraphs.Add();
            title.Range.Text = "Общий отчет по прохождению тестов за все время";
            title.Range.Font.Bold = 1;
            title.Range.Font.Size = 16;
            title.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
            title.Range.InsertParagraphAfter();

            var generationDate = doc.Paragraphs.Add();
            generationDate.Range.Text = $"Дата формирования: {DateTime.Now:dd.MM.yyyy}";
            generationDate.Range.Font.Italic = 0;
            generationDate.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
            generationDate.Range.InsertParagraphAfter();

            doc.Paragraphs.Add().Range.InsertParagraphAfter();
        }

        private void AddOverallSummaryStatistics(Document doc, List<Users> users)
        {
            var sectionTitle = doc.Paragraphs.Add();
            sectionTitle.Range.Text = "Общая статистика за все время:";
            sectionTitle.Range.Font.Bold = 1;
            sectionTitle.Range.Font.Size = 14;
            sectionTitle.Range.InsertParagraphAfter();

            int totalTestsTaken = 0;
            double totalAveragePercent = 0;
            int usersWithResults = 0;
            var allResults = new List<Results>();

            foreach (var user in users)
            {
                var results = GetUserResults(user.Id);
                if (!results.Any()) continue;

                allResults.AddRange(results);
                
                totalAveragePercent += results.Average(r => r.Percent);
                usersWithResults++;
            }

            if (usersWithResults > 0)
            {
                totalAveragePercent /= usersWithResults;

                var stats = doc.Paragraphs.Add();
                stats.Range.Text = $"Всего пользователей с результатами: {usersWithResults}";
                stats.Range.InsertParagraphAfter();

                stats = doc.Paragraphs.Add();
                totalTestsTaken += allResults.GroupBy(r => r.TestId).Count();
                stats.Range.Text = $"Всего пройдено тестов: {totalTestsTaken}";
                stats.Range.InsertParagraphAfter();

                stats = doc.Paragraphs.Add();
                stats.Range.Text = $"Средний процент выполнения по всем пользователям: {Math.Round(totalAveragePercent, 2)}%";
                stats.Range.InsertParagraphAfter();

                // Дополнительная статистика
                if (allResults.Any())
                {
                    var firstTestDate = allResults.Min(r => r.Date);
                    var lastTestDate = allResults.Max(r => r.Date);

                    stats = doc.Paragraphs.Add();
                    stats.Range.Text = $"Период тестирования: с {firstTestDate:dd.MM.yyyy} по {lastTestDate:dd.MM.yyyy}";
                    stats.Range.InsertParagraphAfter();

                    // Статистика по тестам
                    var testStats = allResults.GroupBy(r => r.TestId)
                        .Select(g => new {
                            Test = g.First().Test.Title,
                            Attempts = g.Count(),
                            AvgPercent = g.Average(r => r.Percent)
                        })
                        .OrderByDescending(x => x.Attempts);

                    // Добавляем таблицу статистики по тестам
                    AddTestStatisticsTable(doc, testStats);
                }
            }
            else
            {
                var noData = doc.Paragraphs.Add();
                noData.Range.Text = "Нет данных о результатах тестирования";
                noData.Range.InsertParagraphAfter();
            }

            doc.Paragraphs.Add().Range.InsertParagraphAfter();
        }

        private void AddTestStatisticsTable(Document doc, IEnumerable<dynamic> testStats)
        {
            doc.Paragraphs.Add();
            var sectionTitle = doc.Paragraphs.Add();
            sectionTitle.Range.Text = "Статистика по тестам:";
            sectionTitle.Range.Font.Bold = 1;
            sectionTitle.Range.Font.Size = 14;
            sectionTitle.Range.InsertParagraphAfter();
            doc.Paragraphs.Add();

            // Создаем пустой параграф для таблицы
            var tableParagraph = doc.Paragraphs.Add();
            var tableRange = tableParagraph.Range;

            // Создаем таблицу
            var table = doc.Tables.Add(
                Range: tableRange,
                NumRows: testStats.Count() + 1,
                NumColumns: 3,
                DefaultTableBehavior: WdDefaultTableBehavior.wdWord9TableBehavior,
                AutoFitBehavior: WdAutoFitBehavior.wdAutoFitWindow
            );

            // Заголовки таблицы
            table.Cell(1, 1).Range.Text = "Тест";
            table.Cell(1, 2).Range.Text = "Количество прохождений";
            table.Cell(1, 3).Range.Text = "Средний процент";

            // Заполнение данными
            int row = 2;
            foreach (var stat in testStats)
            {
                table.Cell(row, 1).Range.Text = stat.Test;
                table.Cell(row, 2).Range.Text = stat.Attempts.ToString();
                table.Cell(row, 3).Range.Text = $"{Math.Round(stat.AvgPercent, 2)}%";
                row++;
            }

            // Форматирование таблицы
            table.Range.Font.Size = 12;
            table.Borders.Enable = 1;
            table.Rows[1].Range.Font.Bold = 1;
            table.Rows[1].Range.Shading.BackgroundPatternColor = WdColor.wdColorGray15;
        }

        #endregion
        private void AddAllUsersReportHeader(Document doc, DateTime startDate, DateTime endDate)
        {
            var title = doc.Paragraphs.Add();
            title.Range.Text = "Сводный отчет по прохождению тестов";
            title.Range.Font.Bold = 1;
            title.Range.Font.Size = 16;
            title.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
            title.Range.InsertParagraphAfter();

            var period = doc.Paragraphs.Add();
            period.Range.Text = $"За период: {startDate:dd.MM.yyyy} - {endDate:dd.MM.yyyy}";
            period.Range.Font.Size = 14;
            title.Range.Font.Bold = 0;
            period.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
            period.Range.InsertParagraphAfter();

            var generationDate = doc.Paragraphs.Add();
            generationDate.Range.Text = $"Дата формирования: {DateTime.Now:dd.MM.yyyy}";
            generationDate.Range.Font.Italic = 0;
            generationDate.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
            generationDate.Range.InsertParagraphAfter();

            doc.Paragraphs.Add().Range.InsertParagraphAfter();
        }

        private void AddSummaryStatistics(Document doc, List<Users> users, DateTime startDate, DateTime endDate)
        {
            var sectionTitle = doc.Paragraphs.Add();
            sectionTitle.Range.Text = "Общая статистика:";
            sectionTitle.Range.Font.Bold = 1;
            sectionTitle.Range.Font.Size = 14;
            sectionTitle.Range.InsertParagraphAfter();

            int totalTestsTaken = 0;
            double totalAveragePercent = 0;
            int usersWithResults = 0;

            foreach (var user in users)
            {
                var results = GetUserResultsByDate(user.Id, startDate, endDate);
                if (!results.Any()) continue;

                totalTestsTaken += results.GroupBy(r => r.TestId).Count();
                totalAveragePercent += results.Average(r => r.Percent);
                usersWithResults++;
            }

            if (usersWithResults > 0)
            {
                totalAveragePercent /= usersWithResults;

                var stats = doc.Paragraphs.Add();
                stats.Range.Text = $"Всего пользователей с результатами: {usersWithResults}";
                sectionTitle.Range.Font.Bold = 0;
                stats.Range.InsertParagraphAfter();

                stats = doc.Paragraphs.Add();
                stats.Range.Text = $"Всего пройдено тестов: {totalTestsTaken}";
                stats.Range.InsertParagraphAfter();

                stats = doc.Paragraphs.Add();
                stats.Range.Text = $"Средний процент выполнения по всем пользователям: {Math.Round(totalAveragePercent, 2)}%";
                stats.Range.InsertParagraphAfter();
            }
            else
            {
                var noData = doc.Paragraphs.Add();
                noData.Range.Text = "Нет данных о результатах тестирования за выбранный период";
                noData.Range.InsertParagraphAfter();
            }

            doc.Paragraphs.Add().Range.InsertParagraphAfter();
        }

        private void AddUserSectionHeader(Document doc, Users user)
        {
            var userHeader = doc.Paragraphs.Add();
            userHeader.Range.Text = $"Пользователь: {user.FullName}";
            userHeader.Range.Font.Bold = 1;
            userHeader.Range.Font.Size = 14;
            userHeader.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
            userHeader.Range.InsertParagraphAfter();

            var roleInfo = doc.Paragraphs.Add();
            roleInfo.Range.Text = $"Должность: {user.UserRole.Title}";
            roleInfo.Range.Font.Size = 12;
            userHeader.Range.Font.Bold = 0;
            roleInfo.Range.InsertParagraphAfter();
        }

        #endregion

        #region Word Report Components

        private void AddWordHeader(Document doc, Users user)
        {
            var title = doc.Paragraphs.Add();
            title.Range.Text = $"Отчет по пользователю";
            title.Range.Font.Bold = 1;
            title.Range.Font.Size = 16;
            title.Range.Font.AllCaps = 1;
            title.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
            title.Range.InsertParagraphAfter();

            doc.Paragraphs.Add();
            var date = doc.Paragraphs.Add();
            date.Range.Text = $"Дата формирования: {DateTime.Now:dd.MM.yyyy}";
            title.Range.Font.Size = 14;
            title.Range.Font.AllCaps = 0;
            title.Range.Font.Bold = 0;
            date.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
            date.Range.InsertParagraphAfter();
        }

        private void AddWordUserInfo(Document doc, Users user)
        {
            doc.Paragraphs.Add();
            var info = doc.Paragraphs.Add();
            info.Range.Text = "Информация о пользователе:";
            info.Range.Font.Size = 14;
            info.Range.InsertParagraphAfter();

            var details = doc.Paragraphs.Add();
            details.Range.Text = $"{user.FullName}\n{user.UserRole.Title}";
            details.Range.InsertParagraphAfter();
        }

        private void AddWordResultsTable(Document doc, List<Results> results)
        {
            var groupedResults = results.GroupBy(r => r.TestId);

            doc.Paragraphs.Add();
            var sectionTitle = doc.Paragraphs.Add();
            sectionTitle.Range.Text = "Результаты тестирования:";
            sectionTitle.Range.Font.Bold = 1;
            sectionTitle.Range.Font.Size = 14;
            sectionTitle.Range.InsertParagraphAfter();
            doc.Paragraphs.Add();

            // Создаем пустой параграф для таблицы
            var tableParagraph = doc.Paragraphs.Add();
            // Получаем диапазон для этого параграфа
            var tableRange = tableParagraph.Range;
            // Создаем таблицу с правильными параметрами
            var numRows = groupedResults.Count() + 1;
            var numColumns = 5;
            var table = doc.Tables.Add(
                Range: tableRange,
                NumRows: numRows,
                NumColumns: numColumns,
                DefaultTableBehavior: WdDefaultTableBehavior.wdWord9TableBehavior,
                AutoFitBehavior: WdAutoFitBehavior.wdAutoFitWindow
            );

            // Альтернативный вариант (более простой):
            // var table = doc.Tables.Add(tableRange, numRows, numColumns);


            // Заголовки таблицы
            table.Cell(1, 1).Range.Text = "№";
            table.Cell(1, 2).Range.Text = "Тест";
            table.Cell(1, 3).Range.Text = "Баллы";
            table.Cell(1, 4).Range.Text = "Процент";
            table.Cell(1, 5).Range.Text = "Дата";

            // Заполнение данными
            int row = 2;
            foreach (var group in groupedResults)
            {
                var bestResult = group.OrderByDescending(r => r.Percent).First();
                table.Cell(row, 1).Range.Text = (row - 1).ToString();
                table.Cell(row, 2).Range.Text = bestResult.Test.Title;
                table.Cell(row, 3).Range.Text = $"{bestResult.Score} из {bestResult.Test.Count}";
                table.Cell(row, 4).Range.Text = $"{bestResult.Percent}%";
                table.Cell(row, 5).Range.Text = bestResult.Date.ToString("dd.MM.yyyy");
                table.Rows[row].Range.Font.Bold = 0;
                row++;
            }

            // Форматирование таблицы
            table.Range.Font.Size = 12;
            table.Borders.Enable = 1;
            table.Rows[1].Range.Font.Bold = 1;
            table.Rows[1].Range.Shading.BackgroundPatternColor = WdColor.wdColorGray15;
        }

        private void AddWordStatistics(Document doc, List<Results> results)
        {
            doc.Paragraphs.Add();
            var stats = doc.Paragraphs.Add();
            stats.Range.Text = $"Всего пройдено тестов: {results.GroupBy(r => r.TestId).Count()}";
            stats.Range.Font.Size = 14;
            stats.Range.Font.Bold = 0;
            stats.Range.InsertParagraphAfter();

            var avgPercent = results.Average(r => r.Percent);
            stats = doc.Paragraphs.Add();
            stats.Range.Text = $"Средний процент выполнения: {Math.Round(avgPercent, 2)}%";
            stats.Range.InsertParagraphAfter();
        }

        private void AddWordTestDetails(Document doc, List<Results> results)
        {
            // Добавляем заголовок раздела
            doc.Paragraphs.Add();
            var sectionTitle = doc.Paragraphs.Add();
            sectionTitle.Range.Text = "Детализация по тестам:";
            sectionTitle.Range.Font.Bold = 1;
            sectionTitle.Range.Font.Size = 14;
            sectionTitle.Range.InsertParagraphAfter();

            // Группируем результаты по тестам
            var testGroups = results.GroupBy(r => r.TestId).ToList();

            foreach (var group in testGroups)
            {
                // Добавляем заголовок теста
                var testTitle = doc.Paragraphs.Add();
                testTitle.Range.Text = group.First().Test.Title;
                testTitle.Range.Font.Bold = 1;
                testTitle.Range.Font.Size = 12;
                testTitle.Range.InsertParagraphAfter();

                // Создаем новый параграф для таблицы
                var tableParagraph = doc.Paragraphs.Add();
                var tableRange = tableParagraph.Range;

                // Создаем таблицу с явным указанием всех параметров
                var attemptsTable = doc.Tables.Add(
                    Range: tableRange,
                    NumRows: group.Count() + 1,  // +1 для заголовков
                    NumColumns: 4,
                    DefaultTableBehavior: WdDefaultTableBehavior.wdWord9TableBehavior,
                    AutoFitBehavior: WdAutoFitBehavior.wdAutoFitWindow
                );

                // Альтернативный вариант (если предыдущий не работает):
                // var attemptsTable = doc.Tables.Add(tableRange, group.Count() + 1, 4);

                // Заполняем заголовки таблицы
                attemptsTable.Cell(1, 1).Range.Text = "№";
                attemptsTable.Cell(1, 2).Range.Text = "Дата";
                attemptsTable.Cell(1, 3).Range.Text = "Баллы";
                attemptsTable.Cell(1, 4).Range.Text = "Процент";

                // Заполняем таблицу данными
                int attemptRow = 2;
                foreach (var attempt in group.OrderByDescending(r => r.Date))
                {
                    attemptsTable.Cell(attemptRow, 1).Range.Text = (attemptRow - 1).ToString();
                    attemptsTable.Cell(attemptRow, 2).Range.Text = attempt.Date.ToString("dd.MM.yyyy");
                    attemptsTable.Cell(attemptRow, 3).Range.Text = $"{attempt.Score} из {attempt.Test.Count}";
                    attemptsTable.Cell(attemptRow, 4).Range.Text = $"{attempt.Percent}%";
                    attemptsTable.Rows[attemptRow].Range.Font.Bold = 0;
                    attemptRow++;
                }

                // Форматируем таблицу
                attemptsTable.Range.Font.Size = 11;
                attemptsTable.Borders.Enable = 1;
                attemptsTable.Rows[1].Range.Font.Bold = 1;
                attemptsTable.Rows[1].Range.Shading.BackgroundPatternColor = WdColor.wdColorGray15;

                // Добавляем пустую строку между таблицами
                doc.Paragraphs.Add().Range.InsertParagraphAfter();
            }
        }

        #endregion

     

        #region Data Access Methods

        private Users GetUser(int userId)
        {
            string query = $"SELECT * FROM Users WHERE Id = {userId}";
            DataTable table = _dbHelper.ExecuteSelectQuery(query);

            if (table.Rows.Count == 0) return null;

            var row = table.Rows[0];
            var role = GetRole(Convert.ToInt32(row["Role_Id"]));

            return new Users(
                id: userId,
                firstName: row["First_Name"].ToString(),
                name: row["Name"].ToString(),
                patronymic: row["Last_Name"].ToString(),
                userRole: role,
                password: row["Password"].ToString(),
                isDone: Convert.ToBoolean(row["Is_Done"])
            );
        }

        private Roles GetRole(int roleId)
        {
            string query = $"SELECT * FROM Roles WHERE Id = {roleId}";
            DataTable table = _dbHelper.ExecuteSelectQuery(query);

            if (table.Rows.Count == 0) return null;

            var row = table.Rows[0];
            return new Roles(roleId, row["Title"].ToString());
        }

        private List<Results> GetUserResults(int userId)
        {
            var results = new List<Results>();

            string query = $@"
                SELECT r.*, t.Title, t.Is_Completed 
                FROM Results r
                JOIN Tests t ON r.Test_Id = t.Id
                WHERE r.User_Id = {userId}";

            DataTable table = _dbHelper.ExecuteSelectQuery(query);

            foreach (DataRow row in table.Rows)
            {
                var test = new Tests(
                    id: Convert.ToInt32(row["Test_Id"]),
                    name: row["Title"].ToString(),
                    isCompleted: Convert.ToDouble(row["Is_Completed"]),
                    databaseHelper: _dbHelper
                );

                var result = new Results(
                    id: Convert.ToInt32(row["Id"]),
                    userId: userId,
                    testId: Convert.ToInt32(row["Test_Id"]),
                    score: Convert.ToDouble(row["Score"]),
                    date: Convert.ToDateTime(row["Date"])
                )
                {
                    Test = test,
                    User = GetUser(userId)
                };

                results.Add(result);
            }

            return results;
        }

        private List<Users> GetAllUsers()
        {
            var users = new List<Users>();
            string query = "SELECT * FROM Users";
            DataTable table = _dbHelper.ExecuteSelectQuery(query);

            foreach (DataRow row in table.Rows)
            {
                int id = Convert.ToInt32(row["Id"]);
                var role = GetRole(Convert.ToInt32(row["Role_Id"]));

                var user = new Users(
                    id: id,
                    firstName: row["First_Name"].ToString(),
                    name: row["Name"].ToString(),
                    patronymic: row["Last_Name"].ToString(),
                    userRole: role,
                    password: row["Password"].ToString(),
                    isDone: Convert.ToBoolean(row["Is_Done"])
                );

                users.Add(user);
            }

            return users;
        }
        private List<Results> GetUserResultsByDate(int userId, DateTime startDate, DateTime endDate)
        {
            var results = new List<Results>();

            string query = $@"
        SELECT r.*, t.Title, t.Is_Completed 
        FROM Results r
        JOIN Tests t ON r.Test_Id = t.Id
        WHERE r.User_Id = {userId}
        AND r.Date >= '{startDate:yyyy-MM-dd}' 
        AND r.Date <= '{endDate:yyyy-MM-dd 23:59:59}'";

            DataTable table = _dbHelper.ExecuteSelectQuery(query);

            foreach (DataRow row in table.Rows)
            {
                var test = new Tests(
                    id: Convert.ToInt32(row["Test_Id"]),
                    name: row["Title"].ToString(),
                    isCompleted: Convert.ToDouble(row["Is_Completed"]),
                    databaseHelper: _dbHelper
                );

                var result = new Results(
                    id: Convert.ToInt32(row["Id"]),
                    userId: userId,
                    testId: Convert.ToInt32(row["Test_Id"]),
                    score: Convert.ToDouble(row["Score"]),
                    date: Convert.ToDateTime(row["Date"])
                )
                {
                    Test = test,
                    User = GetUser(userId)
                };

                results.Add(result);
            }

            return results;
        }


        #endregion

    }
}
