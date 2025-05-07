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
using MessageBox = System.Windows.MessageBox;

namespace wpf_тесты_для_обучения
{
    /// <summary>
    /// Логика взаимодействия для ReportPeriodForm.xaml
    /// </summary>
    public partial class ReportPeriodForm : Window
    {
        private DatabaseHelper _databaseHelper;
        public ReportPeriodForm(DatabaseHelper databaseHelper)
        {
            InitializeComponent();
            _databaseHelper = databaseHelper;   
        }

        private void TextBlock_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new FolderBrowserDialog
            {
                Description = "Выберите папку для сохранения отчета",
                ShowNewFolderButton = true
            };

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Reports reports = new Reports(_databaseHelper);
                DateTime startDate = (DateTime)datePicker1.SelectedDate;
                DateTime endDate = (DateTime)datePicker2.SelectedDate;
                reports.GenerateAllUsersReport(startDate, endDate, dialog.SelectedPath);
                MessageBox.Show("Отчеты успешно сформированы.");
            }
        }
    }
}
