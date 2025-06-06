using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using wpf_тесты_для_обучения.Properties;
using Cursors = System.Windows.Input.Cursors;
using MessageBox = System.Windows.MessageBox;

namespace wpf_тесты_для_обучения
{
    /// <summary>
    /// Логика взаимодействия для SelectEmployeeForm.xaml
    /// </summary>
    public partial class SelectEmployeeForm : Window
    {
        private DatabaseHelper _databaseHelper;
        public SelectEmployeeForm(DatabaseHelper databaseHelper)
        {
            InitializeComponent();
            _databaseHelper = databaseHelper;
            LoadUsersIntoListBox();
        }

        private void LoadUsersIntoListBox()
        {
            try
            {
                // Получаем список пользователей с ролями
                List<Users> users = _databaseHelper.GetUsersWithRolesToCombobox();

                // Очищаем ComboBox перед добавлением данных
                usersListBox.Items.Clear();

                usersListBox.ItemsSource = users;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Исключение: {ex.Message}\n" +
                  $"Метод: {ex.TargetSite}\n" +
                  $"Трассировка стека: {ex.StackTrace}", "Ошибка загрузки комбобокса", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            List<int> selectedIds = GetSelectedUserIds();
            if (selectedIds.Count == 0)
            {
                MessageBox.Show($"Выберите хотя бы 1 пользователя", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }

            var dialog = new FolderBrowserDialog
            {
                Description = "Выберите папку для сохранения отчета",
                ShowNewFolderButton = true
            };

            if (dialog.ShowDialog() != System.Windows.Forms.DialogResult.OK)
            {
                Console.WriteLine("Выбор отменен пользователем.");
                return;
            }

            string selectedPath = dialog.SelectedPath;

            try
            {
                // Устанавливаем курсор "ожидание" (колесико загрузки)
                Mouse.OverrideCursor = Cursors.Wait;
                this.IsEnabled = false;
                // Запускаем генерацию отчетов в фоновом потоке
                await Task.Run(() =>
                {
                    Reports reports = new Reports(_databaseHelper);
                    reports.GenerateUserReports(selectedIds, selectedPath, true);
                });

                MessageBox.Show($"Отчеты успешно сформированы и сохранены в:\n{selectedPath}",
                              "Готово", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при формировании отчетов:\n{ex.Message}",
                              "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                // Восстанавливаем стандартный курсор
                Mouse.OverrideCursor = null;
                this.Close();
            }
        }
        private List<int> GetSelectedUserIds()
        {
            try
            {
                return usersListBox.ItemsSource
                        .Cast<Users>()
                        .Where(user => user.IsSelected)
                        .Select(user => user.Id)
                        .ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Исключение: {ex.Message}\n" +
                  $"Метод: {ex.TargetSite}\n" +
                  $"Трассировка стека: {ex.StackTrace}", "Ошибка при выборе теста", MessageBoxButton.OK, MessageBoxImage.Exclamation);

                return null;
            }
        }

        private void TextBlock_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }
    }
}
