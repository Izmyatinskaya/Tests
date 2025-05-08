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
using static MaterialDesignThemes.Wpf.Theme;

namespace wpf_тесты_для_обучения
{
    /// <summary>
    /// Логика взаимодействия для SetRoleForm.xaml
    /// </summary>
    public partial class SetRoleForm : Window
    {
        private DatabaseHelper _databaseHelper;
        private int[] _users;
        public SetRoleForm(DatabaseHelper databaseHelper, int[] users)
        {
            try
            {
                InitializeComponent();
                DataContext = this;
                _databaseHelper = databaseHelper;
                _users = users;
                LoadRolesIntoComboBox();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Исключение: {ex.Message}\n" +
                  $"Метод: {ex.TargetSite}\n" +
                  $"Трассировка стека: {ex.StackTrace}", "Ошибка загрузки формы доб-я пользователя", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }

        private void LoadRolesIntoComboBox()
        {
            try
            {
                // Получаем список пользователей с ролями
                List<Roles> roles = _databaseHelper.GetRolesList();

                // Очищаем ComboBox перед добавлением данных
                rolesComboBox.ItemsSource = null;

                rolesComboBox.ItemsSource = roles;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Исключение: {ex.Message}\n" +
                  $"Метод: {ex.TargetSite}\n" +
                  $"Трассировка стека: {ex.StackTrace}", "Ошибка загрузки комбобокса", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }
        public SetRoleForm()
        {
            InitializeComponent();
        }

        private void goToAddNewRole_Click(object sender, MouseButtonEventArgs e)
        {
            try
            {
                AddRoleForm addRoleForm = new AddRoleForm(_databaseHelper);
                addRoleForm.ShowDialog();
                LoadRolesIntoComboBox();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Исключение: {ex.Message}\n" +
                  $"Метод: {ex.TargetSite}\n" +
                  $"Трассировка стека: {ex.StackTrace}", "Ошибка открытия формы добавления ролм", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }

        private void TextBlock_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Roles role = (Roles)rolesComboBox.SelectedItem;
                string users_id = string.Join(",", _users);

                string query = $@"Update Users SET Role_Id = {role.Id} where Users.Id in ({users_id})";

                _databaseHelper.ExecuteNonQuery(query);

                MessageBox.Show("Роли установлены успешно", "Уведомление");
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Исключение: {ex.Message}\n" +
                  $"Метод: {ex.TargetSite}\n" +
                  $"Трассировка стека: {ex.StackTrace}", "Ошибка установки ролей", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }
    }
}
