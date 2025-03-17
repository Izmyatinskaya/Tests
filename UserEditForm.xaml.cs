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
    /// Логика взаимодействия для UserEditForm.xaml
    /// </summary>
    public partial class UserEditForm : Window
    {
        private Users _currentUser;
        private DatabaseHelper _databaseHelper;
      
        public UserEditForm(Users user)
        {
            _databaseHelper = new DatabaseHelper("Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=\"D:\\Проекты\\Тесты обучение WPF\\wpf тесты для обучения\\DB.mdf\";Integrated Security=True");
            InitializeComponent();
            _currentUser = user;
            LoadRolesIntoComboBox();
            LoadUserData();
            
        }

    private void LoadRolesIntoComboBox()
    {
        // Получаем список пользователей с ролями
        List<Roles> roles = _databaseHelper.GetRolesList();

        // Очищаем ComboBox перед добавлением данных
        rolesComboBox.Items.Clear();

        rolesComboBox.ItemsSource = roles;
    }


    private void LoadUserData()
        {
            familiyaTextBox.Text = _currentUser.FirstName;
            nameTextBox.Text = _currentUser.Name;
            patronymicTextBox.Text = _currentUser.Patronymic;
            rolesComboBox.SelectedItem = ((List<Roles>)rolesComboBox.ItemsSource)
        .FirstOrDefault(r => r.Id == _currentUser.UserRole.Id);

            passwordBox1.Password = _currentUser.Password.TrimEnd();
            passwordBox2.Password = _currentUser.Password.TrimEnd();

        }


        private bool CheckPasswords()
        {
            // Получить пароли из соответствующих полей
            string password1 = passwordBox1.Visibility == Visibility.Visible ? passwordBox1.Password : passwordTextBox1.Text;
            string password2 = passwordBox2.Visibility == Visibility.Visible ? passwordBox2.Password : passwordTextBox2.Text;

            // Проверить совпадение
            if (password1 == password2)
            {
                // Пароли совпадают — убрать сообщение об ошибке
                errorTextBlock.Visibility = Visibility.Collapsed;

                // Вернуть стандартный стиль границ
                passwordBox1.BorderBrush = Brushes.Gray;
                passwordBox2.BorderBrush = Brushes.Gray;
                return true;
            }
            else
            {
                // Пароли не совпадают — отобразить сообщение об ошибке
                errorTextBlock.Text = "Пароли не совпадают!";
                errorTextBlock.Visibility = Visibility.Visible;

                // Изменить цвет границ полей ввода
                passwordBox1.BorderBrush = Brushes.Red;
                passwordBox2.BorderBrush = Brushes.Red;
                return false;
            }
        }
        private bool CheckEmptyFields(string fam, string name, string patronymic)
        {
            bool isValid = true;

            // Проверка на пустые поля
            if (string.IsNullOrWhiteSpace(fam))
            {
                SetFieldError(familiyaTextBox, "Введите фамилию.");
                isValid = false;
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                SetFieldError(nameTextBox, "Введите имя.");
                isValid = false;
            }

            if (string.IsNullOrWhiteSpace(patronymic))
            {
                SetFieldError(patronymicTextBox, "Введите отчество.");
                isValid = false;
            }

            if (rolesComboBox.SelectedItem == null)
            {
                SetFieldError(rolesComboBox, "Выберите роль пользователя.");
                isValid = false;
            }
            errorTextBlock.Text = "Заполните пустые поля";
            errorTextBlock.Visibility = Visibility.Visible;
            return isValid;
        }

        private void SetFieldError(Control control, string message = null)
        {
            if (control is TextBox textBox)
            {
                textBox.BorderBrush = Brushes.Red;
                textBox.ToolTip = message; // Отображение подсказки с сообщением об ошибке

                // Сброс ошибки при изменении текста в поле
                textBox.TextChanged += (s, e) => ResetFieldError(textBox);
            }
            else if (control is ComboBox comboBox)
            {
                comboBox.BorderBrush = Brushes.Red;
                comboBox.ToolTip = message;

                // Сброс ошибки при выборе значения
                comboBox.SelectionChanged += (s, e) => ResetFieldError(comboBox);
            }
        }

        private void ResetFieldError(Control control)
        {
            if (control is TextBox textBox)
            {
                textBox.BorderBrush = Brushes.Gray;
                textBox.ToolTip = null;
            }
            else if (control is ComboBox comboBox)
            {
                comboBox.BorderBrush = Brushes.Gray;
                comboBox.ToolTip = null;
            }
        }


        private void ShowPassword_Checked(object sender, RoutedEventArgs e)
        {
            bool isChecked = (sender as CheckBox).IsChecked ?? false;

            if (isChecked)
            {
                // Отобразить текстовые поля
                passwordTextBox1.Text = passwordBox1.Password;
                passwordTextBox1.Visibility = Visibility.Visible;
                passwordBox1.Visibility = Visibility.Collapsed;

                passwordTextBox2.Text = passwordBox2.Password;
                passwordTextBox2.Visibility = Visibility.Visible;
                passwordBox2.Visibility = Visibility.Collapsed;
            }
            else
            {
                // Скрыть текстовые поля и вернуть PasswordBox
                passwordBox1.Password = passwordTextBox1.Text;
                passwordTextBox1.Visibility = Visibility.Collapsed;
                passwordBox1.Visibility = Visibility.Visible;

                passwordBox2.Password = passwordTextBox2.Text;
                passwordTextBox2.Visibility = Visibility.Collapsed;
                passwordBox2.Visibility = Visibility.Visible;
            }
        }

        private void PasswordBox1_PasswordChanged(object sender, RoutedEventArgs e)
        {
            // Синхронизировать текст с TextBox
            if (passwordTextBox1.Visibility == Visibility.Visible)
            {
                passwordTextBox1.Text = passwordBox1.Password;
            }
        }

        private void PasswordBox2_PasswordChanged(object sender, RoutedEventArgs e)
        {
            // Синхронизировать текст с TextBox
            if (passwordTextBox2.Visibility == Visibility.Visible)
            {
                passwordTextBox2.Text = passwordBox2.Password;
            }
        }

        private void TextBlock_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string fam = familiyaTextBox.Text;
            string name = nameTextBox.Text;
            string patronymic = patronymicTextBox.Text;

            if (!CheckEmptyFields(fam, name, patronymic))
            {
                return;
            }


            // SQL-запрос для проверки наличия пользователя с таким ФИО
            string query = @"
            SELECT COUNT(*) 
            FROM Users 
            WHERE First_Name = @fam AND Name = @name AND Last_Name = @patronymic";

            // Создание массива параметров
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@fam", fam),
                new SqlParameter("@name", name),
                new SqlParameter("@patronymic", patronymic)
            };

            // Выполнение запроса через метод ExecuteScalar
            object result = _databaseHelper.ExecuteScalar(query, parameters);

            // Проверка результата
            if (result != null && int.TryParse(result.ToString(), out int count) && count > 1)
            {
                MessageBox.Show("Пользователь с таким ФИО уже существует.");
                return;
            }

            if (!CheckPasswords())
                return;

            Roles role = (Roles)rolesComboBox.SelectedItem;
            query = @" UPDATE Users SET First_Name = @firstName, Name = @name,
            Last_Name = @lastName, Password = @password, Role_Id = @roleId
            WHERE Id = @userId";    


            parameters = new SqlParameter[]
            {
                new SqlParameter("@firstName", fam),
                new SqlParameter("@name", name),
                new SqlParameter("@lastName", patronymic),
                new SqlParameter("@password", passwordBox1.Password),
                new SqlParameter("@roleId", role.Id),
                new SqlParameter("@userId", _currentUser.Id)
            };

            _databaseHelper.ExecuteNonQuery(query, parameters);

            MessageBox.Show("Уведомление", "Данные пользователя успешно отредактированы");
            this.Close();

        }
    }
}
