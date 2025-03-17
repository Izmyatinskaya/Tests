using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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

namespace wpf_тесты_для_обучения
{
    /// <summary>
    /// Логика взаимодействия для AddRoleForm.xaml
    /// </summary>
    public partial class AddRoleForm : Window
    {
        private DatabaseHelper _databaseHelper;
        public AddRoleForm()
        {
            InitializeComponent();
            _databaseHelper = new DatabaseHelper("Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=\"D:\\Проекты\\Тесты обучение WPF\\wpf тесты для обучения\\DB.mdf\";Integrated Security=True");
            LoadTestsIntoComboBox();
        }
        private void LoadTestsIntoComboBox()
        {
            // Получаем список пользователей с ролями
            List<Tests> tests = _databaseHelper.GetTestsList();

            // Очищаем ComboBox перед добавлением данных
            testsListBox.Items.Clear();

            testsListBox.ItemsSource = tests;
        }
        private void goBack_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if(UpdateErrors()>0) return;
            string text = titleRoleTextBox.Text;
            List<int> selectedTestIds = GetSelectedTestIds();

            string query = @"INSERT INTO Roles (Title) OUTPUT INSERTED.Id VALUES (@text)";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@text", text)
            };

            int rid = (int)_databaseHelper.ExecuteScalar(query, parameters);

            if (selectedTestIds.Count > 0)
            {
                string query2 = "INSERT INTO RoleAccess (Role_Id, Test_Id) VALUES " +
                    string.Join(", ", selectedTestIds.Select((_, i) => $"(@rid, @tid{i})"));

                // Формируем список параметров
                List<SqlParameter> parameters2 = new List<SqlParameter>
                {
                    new SqlParameter("@rid", rid)
                };

                parameters2.AddRange(selectedTestIds.Select((id, i) => new SqlParameter($"@tid{i}", id)));

                _databaseHelper.ExecuteNonQuery(query2, parameters2.ToArray());
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
    }
}
