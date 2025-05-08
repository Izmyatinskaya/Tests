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
using Path = System.IO.Path;
using wpf_тесты_для_обучения.Properties;

namespace wpf_тесты_для_обучения
{
    /// <summary>
    /// Логика взаимодействия для UserAddForm.xaml
    /// </summary>
    public partial class UserAddForm : Window
    {
        private DatabaseHelper _databaseHelper;
        public UserAddForm(DatabaseHelper databaseHelper)
        {
            try
            {
                InitializeComponent();
                _databaseHelper = databaseHelper;
                LoadRolesIntoComboBox();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Исключение: {ex.Message}\n" +
                  $"Метод: {ex.TargetSite}\n" +
                  $"Трассировка стека: {ex.StackTrace}", "Ошибка загрузки формы доб-я пользователя", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }

        public UserAddForm(DatabaseHelper databaseHelper, bool reg)
        {
            try
            {
                InitializeComponent();
                _databaseHelper = databaseHelper;
                LoadRolesIntoComboBox();
                titleTextBlock.Text = "Регистрация";
                addButton.Content = "Зарегистрироваться";
                goBackTextBlock.Text = "Вход";
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

        private void TextBlock_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
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
                if (result != null && int.TryParse(result.ToString(), out int count) && count > 0)
                {
                    MessageBox.Show("Пользователь с таким ФИО уже существует.", "Уведомление");
                    return;
                }

                if (!CheckPasswords())
                    return;

                Roles role = (Roles)rolesComboBox.SelectedItem;
                query = @"
            INSERT INTO Users (First_Name, Name, Last_Name, Password, Role_Id)
            VALUES (@firstName, @name, @lastName, @password, @roleId)";

                parameters = new SqlParameter[]
                {
                new SqlParameter("@firstName", fam),
                new SqlParameter("@name", name),
                new SqlParameter("@lastName", patronymic),
                new SqlParameter("@password", passwordBox1.Password),
                new SqlParameter("@roleId", role.Id)
                };

                _databaseHelper.ExecuteNonQuery(query, parameters);

                MessageBox.Show("Пользователь успешно добавлен", "Уведомление");
                if (titleTextBlock.Text == "Регистрация")
                {
                    LoginForm loginForm = new LoginForm();
                    loginForm.Show();
                }
                this.Close();
                
                
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Исключение: {ex.Message}\n" +
                  $"Метод: {ex.TargetSite}\n" +
                  $"Трассировка стека: {ex.StackTrace}", "Ошибка добавления пользователя", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }

        }
        private bool CheckPasswords()
        {
            try
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
            catch (Exception ex)
            {
                MessageBox.Show($"Исключение: {ex.Message}\n" +
                  $"Метод: {ex.TargetSite}\n" +
                  $"Трассировка стека: {ex.StackTrace}", "Ошибка проверки паролей", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return false;
            }
        }
        private bool CheckEmptyFields(string fam, string name, string patronymic)
        {
            try
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
            catch (Exception ex)
            {
                MessageBox.Show($"Исключение: {ex.Message}\n" +
                  $"Метод: {ex.TargetSite}\n" +
                  $"Трассировка стека: {ex.StackTrace}", "Ошибка проверки пустых полей", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return false;   
            }
        }

        private void SetFieldError(Control control, string message = null)
        {
            try
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
            catch (Exception ex)
            {
                MessageBox.Show($"Исключение: {ex.Message}\n" +
                  $"Метод: {ex.TargetSite}\n" +
                  $"Трассировка стека: {ex.StackTrace}", "Ошибка заполнени яполя ошибки", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }

        private void ResetFieldError(Control control)
        {
            try
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
            catch (Exception ex)
            {
                MessageBox.Show($"Исключение: {ex.Message}\n" +
                  $"Метод: {ex.TargetSite}\n" +
                  $"Трассировка стека: {ex.StackTrace}", "Ошибка обновления поля ошибки", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }


        private void ShowPassword_Checked(object sender, RoutedEventArgs e)
        {
            try
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
            catch (Exception ex)
            {
                MessageBox.Show($"Исключение: {ex.Message}\n" +
                  $"Метод: {ex.TargetSite}\n" +
                  $"Трассировка стека: {ex.StackTrace}", "Ошибка смены состояний пароля", MessageBoxButton.OK, MessageBoxImage.Exclamation);
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

        private void ShowPassword_Click(object sender, RoutedEventArgs e)
        {
            ShowHidePassword(passwordTextBox1, passwordBox1, eyeIcon);
        }

        private void ShowPassword_Click2(object sender, RoutedEventArgs e)
        {
            ShowHidePassword(passwordTextBox2, passwordBox2, eyeIcon2);
        }
        public void ShowHidePassword(TextBox passwordTextBox, PasswordBox passwordBox, FontAwesome.WPF.FontAwesome eye)
        {
            try
            {
                if (passwordTextBox.Visibility == Visibility.Collapsed)
                {
                    // Показать пароль
                    passwordTextBox.Text = passwordBox2.Password;
                    passwordTextBox.Visibility = Visibility.Visible;
                    passwordBox.Visibility = Visibility.Collapsed;

                    // Сменить иконку на закрытый глаз
                    eye.Icon = FontAwesome.WPF.FontAwesomeIcon.EyeSlash;
                }
                else
                {
                    // Скрыть пароль
                    passwordBox.Password = passwordTextBox.Text;
                    passwordBox.Visibility = Visibility.Visible;
                    passwordTextBox.Visibility = Visibility.Collapsed;

                    // Сменить иконку на открытый глаз
                    eye.Icon = FontAwesome.WPF.FontAwesomeIcon.Eye;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Исключение: {ex.Message}\n" +
                   $"Метод: {ex.TargetSite}\n" +
                   $"Трассировка стека: {ex.StackTrace}", "Ошибка ", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }
    }
}
