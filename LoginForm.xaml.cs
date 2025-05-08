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
using System.IO;
using wpf_тесты_для_обучения.Properties;
using Path = System.IO.Path;

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
            try {
                InitializeComponent();
                string databasePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DB.mdf");
                _databaseHelper = new DatabaseHelper($"Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename={databasePath};Integrated Security=True");
                //D:\Проекты\Тесты обучение WPF\wpf тесты для обучения\DB.mdf Server=(localdb)\MSSQLLocalDB;Integrated Security=true;

                //_databaseHelper = new DatabaseHelper("Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=\"D:\\Проекты\\Тесты обучение WPF\\wpf тесты для обучения\\DB.mdf\";Integrated Security=True");
                _databaseHelper.AllUsers = Properties.Settings.Default._allUsers;
                LoadUsersIntoComboBox();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Исключение: {ex.Message}\n" +
                   $"Метод: {ex.TargetSite}\n" +
                   $"Трассировка стека: {ex.StackTrace}", "Ошибка загрузки формы входа", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }
        private void LoadUsersIntoComboBox()
        {
            try
            {
                // Получаем список пользователей с ролями
                List<Users> users = _databaseHelper.GetUsersWithRoles();
                // Очищаем ComboBox перед добавлением данных
                userComboBox.Items.Clear();

                userComboBox.ItemsSource = users;
            }
            
            catch (Exception ex)
            {
                MessageBox.Show($"Исключение: {ex.Message}\n" +
                   $"Метод: {ex.TargetSite}\n" +
                   $"Трассировка стека: {ex.StackTrace}", "Ошибка загрузки пользователей с ролями в комбобокс", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
}

        // Обработчик для кнопки вход
        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            try
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
                        AdminForm adminForm = new AdminForm(_databaseHelper);
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
            catch (Exception ex)
            {
                // Вывод сообщения об ошибке при поиске файла
                MessageBox.Show($"Исключение: {ex.Message}\n" +
                    $"Метод: {ex.TargetSite}\n" +
                    $"Трассировка стека: {ex.StackTrace}", "Ошибка входа", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }

        }


        private bool isPasswordVisible = false;

        private void ShowPassword_Click(object sender, RoutedEventArgs e)
        {
            try
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
            catch (Exception ex)
            {
                MessageBox.Show($"Исключение: {ex.Message}\n" +
                   $"Метод: {ex.TargetSite}\n" +
                   $"Трассировка стека: {ex.StackTrace}", "Ошибка ", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
}

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            try { 
            if (!isPasswordVisible)
            {
                passwordTextBox1.Text = passwordBox1.Password;
            }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Исключение: {ex.Message}\n" +
                   $"Метод: {ex.TargetSite}\n" +
                   $"Трассировка стека: {ex.StackTrace}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }

        private void TextBlock_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try {
                UserAddForm userAddForm = new UserAddForm(_databaseHelper, true);
                userAddForm.Show();
            //RegistrationForm registrationForm = new RegistrationForm(_databaseHelper);
            //registrationForm.Show();
            this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Исключение: {ex.Message}\n" +
                   $"Метод: {ex.TargetSite}\n" +
                   $"Трассировка стека: {ex.StackTrace}", "Ошибка загрузки формы регистрации", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }

        private void userComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                Users selectedItem = (Users)userComboBox.SelectedItem;
                _databaseHelper._currentUser = selectedItem;
                UserSession.SelectedUser = selectedItem;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Исключение: {ex.Message}\n" +
                   $"Метод: {ex.TargetSite}\n" +
                   $"Трассировка стека: {ex.StackTrace}", "Ошибка выбора пользоватлея", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }
    }
}
