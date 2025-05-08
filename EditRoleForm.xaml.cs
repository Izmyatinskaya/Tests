using System;
using System.Collections.Generic;
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
using Path = System.IO.Path;
using wpf_тесты_для_обучения.Properties;

namespace wpf_тесты_для_обучения
{
    /// <summary>
    /// Логика взаимодействия для EditRoleForm.xaml
    /// </summary>
    public partial class EditRoleForm : Window
    {
        private DatabaseHelper _databaseHelper;
        public bool IsCorrect { get; set; }
        private Roles _role;
        public EditRoleForm(Roles role, DatabaseHelper databaseHelper)
        {
            try
            {
                InitializeComponent();
                this.DataContext = this;
                _databaseHelper = databaseHelper;
                //string databasePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DB.mdf");
                //_databaseHelper = new DatabaseHelper($"Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename={databasePath};Integrated Security=True");
                //_databaseHelper = new DatabaseHelper("Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=\"D:\\Проекты\\Тесты обучение WPF\\wpf тесты для обучения\\DB.mdf\";Integrated Security=True");
                _role = role;
                titleRoleTextBox.Text = role.Title;
                LoadTestsIntoComboBox();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при отключении базы данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void LoadTestsIntoComboBox()
        {
            // Загружаем все тесты
            List<Tests> tests = _databaseHelper.GetTestsList();
            testsListBox.ItemsSource = tests;

            // Получаем список Test_Id, которые связаны с данной ролью
            string query = "SELECT Test_Id FROM RoleAccess WHERE Role_Id = @roleId";
            SqlParameter[] parameters = { new SqlParameter("@roleId", _role.Id) };

            DataTable roleTestsTable = _databaseHelper.ExecuteSelectQuery(query, parameters);
            List<int> selectedTestIds = roleTestsTable.AsEnumerable()
                                                      .Select(row => row.Field<int>("Test_Id"))
                                                      .ToList();

            // Отмечаем соответствующие тесты в ListBox
            foreach (var test in tests)
            {
                if (selectedTestIds.Contains(test.Id))
                {
                    test.IsSelected = true; // Устанавливаем свойство (нужно добавить в класс)
                }
            }

            // Обновляем ListBox
            testsListBox.Items.Refresh();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (UpdateErrors() > 0) return;
            string text = titleRoleTextBox.Text;
            List<int> selectedTestIds = GetSelectedTestIds();

            string query = @"UPDATE Roles SET Title = @text WHERE Id = @id";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@text", text),
                new SqlParameter("@id", _role.Id)
            };

            _databaseHelper.ExecuteNonQuery(query, parameters);

            string deleteQuery = "DELETE FROM RoleAccess WHERE Role_Id = @roleId";
            SqlParameter[] deleteParams = { new SqlParameter("@roleId", _role.Id) };
            _databaseHelper.ExecuteNonQuery(deleteQuery, deleteParams);

            if (selectedTestIds.Count > 0)
            {
                string insertQuery = "INSERT INTO RoleAccess (Role_Id, Test_Id) VALUES " +
                    string.Join(", ", selectedTestIds.Select((_, i) => $"(@rid, @tid{i})"));

                List<SqlParameter> insertParams = new List<SqlParameter>
                {
                    new SqlParameter("@rid", _role.Id)
                };
                insertParams.AddRange(selectedTestIds.Select((id, i) => new SqlParameter($"@tid{i}", id)));

                _databaseHelper.ExecuteNonQuery(insertQuery, insertParams.ToArray());
            }
            MessageBox.Show("Роль успешно добавлена", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);
            this.Close();
        }
        private List<int> GetSelectedTestIds()
        {
            return testsListBox.ItemsSource
                .Cast<Tests>()
                .Where(test => test.IsSelected)
                .Select(test => test.Id)
                .ToList();
        }
        protected int UpdateErrors()
        {
            errorTextBlock.Inlines.Clear(); // Очищаем текущие инлайны
            if (titleRoleTextBox.Text == "")
            {
                errorTextBlock.Inlines.Add(new LineBreak());
                errorTextBlock.Inlines.Add(new Run { Text = "! " });
                errorTextBlock.Inlines.Add(new Run { Text = "Вы не ввели название роли (должности)" });
            }
            return errorTextBlock.Inlines.Count;
        }
        private void goBack_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }
    }
}
