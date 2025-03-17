using System;
using System.Collections.Generic;
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
    /// Логика взаимодействия для LoginForm.xaml
    /// </summary>
    public partial class LoginForm : Window
    {

        private DatabaseHelper _databaseHelper;
        public LoginForm()
        {
            InitializeComponent();
            _databaseHelper = new DatabaseHelper("Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=\"D:\\Проекты\\Тесты обучение WPF\\wpf тесты для обучения\\DB.mdf\";Integrated Security=True");
            LoadUsersIntoComboBox();
        }


        private void LoadUsersIntoComboBox()
        {
            // Получаем список пользователей с ролями
            List<Users> users = _databaseHelper.GetUsersWithRoles();
            // Очищаем ComboBox перед добавлением данных
            userComboBox.Items.Clear();

            userComboBox.ItemsSource = users;
        }

        // Обработчик для кнопки вход
        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            if (userComboBox.SelectedItem != null)
            {
                Users selectedItem = (Users)userComboBox.SelectedItem;
                string passwordDB = selectedItem.Password.ToString().Trim(' ');
                string password = passwordBox1.Password.ToString().Trim(' ');
                if (passwordDB != password)
                {
                    MessageBox.Show("Ошибка", "Неверный пароль");
                    passwordBox1.Clear();
                    return;
                }
                if (_databaseHelper._currentUser.UserRole.Id == 1)
                {
                    AdminForm adminForm = new AdminForm();
                    adminForm.Show();
                }
                else
                {
                    MainForm mainForm = new MainForm(_databaseHelper);
                    mainForm.Show();
                }
                this.Close();
            }
            else
                MessageBox.Show($"Вы не выбрали пользователя");
        }


        private bool isPasswordVisible = false;

        private void ShowPassword_Click(object sender, RoutedEventArgs e)
        {
            isPasswordVisible = !isPasswordVisible;

            if (isPasswordVisible)
            {
                // Показать пароль
                passwordTextBox1.Text = passwordBox1.Password;
                passwordTextBox1.Visibility = Visibility.Visible;
                passwordBox1.Visibility = Visibility.Collapsed;

                // Сменить иконку на закрытый глаз
                eyeIcon.Icon = FontAwesome.WPF.FontAwesomeIcon.EyeSlash;
            }
            else
            {
                // Скрыть пароль
                passwordBox1.Password = passwordTextBox1.Text;
                passwordBox1.Visibility = Visibility.Visible;
                passwordTextBox1.Visibility = Visibility.Collapsed;

                // Сменить иконку на открытый глаз
                eyeIcon.Icon = FontAwesome.WPF.FontAwesomeIcon.Eye;
            }
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (!isPasswordVisible)
            {
                passwordTextBox1.Text = passwordBox1.Password;
            }
        }

        private void TextBlock_MouseDown(object sender, MouseButtonEventArgs e)
        {
            RegistrationForm registrationForm = new RegistrationForm();
            registrationForm.Show();
            this.Close();
            
        }

        private void userComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Users selectedItem = (Users)userComboBox.SelectedItem;
            _databaseHelper._currentUser = selectedItem;
            UserSession.SelectedUser = selectedItem;
            
        }
    }
}
