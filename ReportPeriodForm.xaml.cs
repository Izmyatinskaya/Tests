using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using wpf_тесты_для_обучения.Properties;
using Cursors = System.Windows.Input.Cursors;
using MessageBox = System.Windows.MessageBox;

namespace wpf_тесты_для_обучения
{
    public partial class ReportPeriodForm : Window
    {
        private DatabaseHelper _databaseHelper;

        public ReportPeriodForm(DatabaseHelper databaseHelper)
        {
            InitializeComponent();
            _databaseHelper = databaseHelper; 
            // Устанавливаем текущую дату по умолчанию
            datePicker1.SelectedDate = DateTime.Today;
            datePicker2.SelectedDate = DateTime.Today;
           
        }

        private void TextBlock_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Проверка выбранных дат
                if (datePicker1.SelectedDate == null || datePicker2.SelectedDate == null)
                {
                    MessageBox.Show("Пожалуйста, выберите обе даты периода.",
                                  "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                DateTime startDate = datePicker1.SelectedDate.Value;
                DateTime endDate = datePicker2.SelectedDate.Value;

                // Проверка корректности периода
                if (startDate > endDate)
                {
                    MessageBox.Show("Дата начала периода не может быть позже даты окончания.",
                                  "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var dialog = new FolderBrowserDialog
                {
                    Description = "Выберите папку для сохранения отчета",
                    ShowNewFolderButton = true
                };

                if (dialog.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                {
                    return;
                }

                // Включаем колесико загрузки и блокируем кнопку
                Mouse.OverrideCursor = Cursors.Wait;
                this.IsEnabled = false;

                try
                {
                    // Запускаем генерацию отчетов в фоновом потоке
                    await Task.Run(() =>
                    {
                        Reports reports = new Reports(_databaseHelper);
                        reports.GenerateAllUsersReport(startDate, endDate, dialog.SelectedPath);
                    });

                    MessageBox.Show("Отчеты успешно сформированы.",
                                  "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при формировании отчетов:\n{ex.Message}",
                                  "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            finally
            {
                // Восстанавливаем интерфейс
                Mouse.OverrideCursor = null;
                ((System.Windows.Controls.Button)sender).IsEnabled = true;
            }

            this.Close();
        }
    }
}