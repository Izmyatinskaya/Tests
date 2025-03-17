using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using wpf_тесты_для_обучения.Properties;
using static System.Net.Mime.MediaTypeNames;

namespace wpf_тесты_для_обучения
{
    /// <summary>
    /// Логика взаимодействия для MainForm.xaml
    /// </summary>
    public partial class MainForm : Window
    {
        private DatabaseHelper _databaseHelper;
        public MainForm(DatabaseHelper databaseHelper)
        {
            InitializeComponent();
            _databaseHelper = databaseHelper;
            LoadTests();
        }

        public void LoadTests()
        {
            List<Tests> tests = _databaseHelper.GetTestsList(_databaseHelper._currentUser.UserRole.Id);
            wrapPanel.Children.Clear();

            foreach (var test in tests)
            {
                int questionCount = _databaseHelper.GetQuestionCount(test.Id);
                int attemptNumber = _databaseHelper.GetUserAttemptCount(_databaseHelper._currentUser.Id, test.Id) + 1;
                bool isCompleted = _databaseHelper.IsTestCompleted(_databaseHelper._currentUser.Id, test.Id);

                // Основной контейнер
                StackPanel testContainer = new StackPanel { Orientation = Orientation.Vertical, Margin = new Thickness(10) };

                // Контент кнопки теста
                StackPanel buttonContent = new StackPanel { Orientation = Orientation.Vertical, VerticalAlignment = VerticalAlignment.Center };

                TextBlock titleBlock = new TextBlock
                {
                    Text = test.Title,
                    FontWeight = FontWeights.Bold,
                    FontSize = 16,
                    TextAlignment = TextAlignment.Center,
                    TextWrapping = TextWrapping.Wrap,
                    MaxWidth = 180,
                    Margin = new Thickness(0, 0, 0, 5)
                };

                TextBlock questionCountBlock = new TextBlock
                {
                    Text = $"Вопросов: {questionCount}",
                    FontSize = 12,
                    TextAlignment = TextAlignment.Center
                };

                TextBlock percentBlock = new TextBlock
                {
                    Text = $"Порог для прохождения: {test.IsCompleted}% ({Math.Round((test.Count * test.IsCompleted / 100), 2)})",
                    FontSize = 12,
                    TextAlignment = TextAlignment.Center,
                    Margin = new Thickness(0, 0, 0, 5)
                };

                double res = Math.Round(_databaseHelper.GetUserResult(_databaseHelper._currentUser.Id, test.Id),2);
                double percent = res==0 ? 0 : Math.Round(( res/ test.Count * 100), 2);
                TextBlock resultBlock = new TextBlock
                {
                    Text = $"Результат последнего прохождения: {res} ({percent})% ",
                    FontSize = 12,
                    FontWeight = FontWeights.Bold,
                    TextWrapping = TextWrapping.Wrap,
                    TextAlignment = TextAlignment.Center
                };

                buttonContent.Children.Add(titleBlock);
                buttonContent.Children.Add(questionCountBlock);
                buttonContent.Children.Add(percentBlock);
                buttonContent.Children.Add(resultBlock);

                // Цвета кнопки
                SolidColorBrush borderColor = (SolidColorBrush)new BrushConverter().ConvertFromString("#FFFB9100");
                SolidColorBrush backgroundColor = Brushes.White;

                Button testButton = new Button
                {
                    Content = buttonContent,
                    Width = 200,
                    Height = 150,
                    BorderBrush = borderColor,
                    Background = backgroundColor,
                    FontFamily = new FontFamily("Arial"),
                    Tag = test
                };
                testButton.Click += TestButton_Click;
                testContainer.Children.Add(testButton);

                // Кнопка "Результаты"
                Button resultsButton = new Button
                {
                    Width = 200,
                    Background = Brushes.White,
                    BorderBrush = (SolidColorBrush)new BrushConverter().ConvertFromString("#FFFFA500"),
                    BorderThickness = new Thickness(1,0,1,1),
                    Padding = new Thickness(5),
                    Tag = test.Id
                };

                StackPanel resultsButtonContent = new StackPanel { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Center };
                TextBlock resultsText = new TextBlock { Text = "Результаты", VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(5, 0, 0, 0) };
                TextBlock arrowIcon = new TextBlock { Text = "▼", FontSize = 14, VerticalAlignment = VerticalAlignment.Center };

                resultsButtonContent.Children.Add(resultsText);
                resultsButtonContent.Children.Add(arrowIcon);
                resultsButton.Content = resultsButtonContent;

                // Popup с таблицей результатов
                Popup resultsPopup = new Popup
                {
                    PlacementTarget = resultsButton,
                    Placement = PlacementMode.Bottom,
                    StaysOpen = false,
                    IsOpen = false
                };

                Border popupBorder = new Border
                {
                    BorderBrush = Brushes.Gray,
                    BorderThickness = new Thickness(1),
                    Background = Brushes.White,
                    Padding = new Thickness(10),
                    MaxHeight = 170,
                    Width= 300
                };

                DataGrid resultsGrid = new DataGrid
                {
                    AutoGenerateColumns = false,
                    HeadersVisibility = DataGridHeadersVisibility.Column,
                    Height = 140,
                    Width = 270,
                    Background = Brushes.White,
                    BorderBrush = Brushes.Gray,
                    FontFamily = new FontFamily("Arial"), // Исправлено
                    FontSize = 14, // Исправлено (был строковый тип)
                    IsReadOnly = true // Исправлено (был строковый тип)
                };


                resultsGrid.Columns.Add(new DataGridTextColumn { Header = "№ попытки", Binding = new Binding("RowNumber") });
                resultsGrid.Columns.Add(new DataGridTextColumn { Header = "Результат", Binding = new Binding("Score") });
                resultsGrid.Columns.Add(new DataGridTextColumn { Header = "Результат в %", Binding = new Binding("Percent") });
                

                popupBorder.Child = resultsGrid;
                resultsPopup.Child = popupBorder;

                // Добавляем кнопку и popup в контейнер
                testContainer.Children.Add(resultsButton);
                testContainer.Children.Add(resultsPopup);
                wrapPanel.Children.Add(testContainer);

                // Обработчик клика по кнопке "Результаты"
                resultsButton.Click += (s, e) =>
                {
                    int testId = (int)((Button)s).Tag;
                    List<Results> testResults = _databaseHelper.GetResultsByTestId(test.Id);

                    // Добавляем порядковый номер к каждому результату
                    for (int i = 0; i < testResults.Count; i++)
                    {
                        testResults[i].RowNumber = i + 1; // Нумерация с 1
                    }

                    // Настроим таблицу на отображение результатов с добавленным номером строки
                    resultsGrid.ItemsSource = testResults;
                    resultsPopup.IsOpen = !resultsPopup.IsOpen;
                };

                UpdateTestButtonColors();
            }

            // Обновляем метку выполненных тестов
            UpdateCompletedTestsLabel();
        }




        private void UpdateCompletedTestsLabel()
        {
            int totalTests = _databaseHelper.GetTestsList(_databaseHelper._currentUser.UserRole.Id).Count;
            int completedTests = _databaseHelper.GetCompletedTestsCount(_databaseHelper._currentUser.Id);

            testCountLabel.Text = $"Пройдено тестов: {completedTests} из {totalTests}";
        }


        private void TestButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is Tests selectedTest)
            {
                //MessageBox.Show($"Вы выбрали тест: {selectedTest.Title}");
                OpenTest(selectedTest);
            }
        }

        private void OpenTest(Tests test)
        {
            TestAddForm testAddForm = new TestAddForm(TestAddForm.TestMode.Pass, test.Id);
            testAddForm.ShowDialog();
            UpdateCompletedTestsLabel();
            UpdateTestButtonColors();
            
            UpdateResultBlock(test.Id, test.Count);
        }
        private void UpdateResultBlock(int testId, int testCount)
        {
            double res = Math.Round(_databaseHelper.GetUserResult(_databaseHelper._currentUser.Id, testId), 2);
            double percent = res == 0 ? 0 : Math.Round((res / testCount * 100), 2);
            foreach (var child in wrapPanel.Children)
            {
                if (child is StackPanel testContainer)
                {
                    // Ищем кнопку теста, чтобы проверить принадлежность к нужному testId
                    Button testButton = testContainer.Children.OfType<Button>().FirstOrDefault();
                    if (testButton?.Tag is int buttonTestId && buttonTestId == testId)
                    {
                        // Ищем блок с результатами в этом контейнере
                        TextBlock resultBlock = testContainer.Children
                            .OfType<TextBlock>()
                            .FirstOrDefault(tb => tb.Text.StartsWith("Результат"));

                        if (resultBlock != null)
                        {
                            resultBlock.Text = $"Результат последнего прохождения: {res}% ({percent}%)";
                        }
                        break;
                    }
                }
            }
        }


        private void UpdateTestButtonColors()
        {
            foreach (var child in wrapPanel.Children)
            {
                if (child is StackPanel testContainer)
                {
                    Button testButton = testContainer.Children.OfType<Button>().FirstOrDefault();
                    if (testButton != null && testButton.Tag is Tests test)
                    {
                        bool isCompleted = _databaseHelper.IsTestCompleted(_databaseHelper._currentUser.Id, test.Id);
                        int attemptCount = _databaseHelper.GetUserAttemptCount(_databaseHelper._currentUser.Id, test.Id);

                        // Определяем цвет кнопки
                        SolidColorBrush borderColor;
                        SolidColorBrush backgroundColor;

                        if (isCompleted)
                        {
                            borderColor = Brushes.Green;
                            backgroundColor = Brushes.LightGreen;
                        }
                        else if (attemptCount > 0)
                        {
                            borderColor = Brushes.Red;
                            backgroundColor = Brushes.LightCoral;
                        }
                        else
                        {
                            borderColor = Brushes.Gray;
                            backgroundColor = Brushes.White;
                        }

                        // Обновляем цвета кнопки
                        testButton.BorderBrush = borderColor;
                        testButton.Background = backgroundColor;
                    }
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            GoToLogin();
        }
        private void GoToLogin()
        {
            LoginForm loginForm = new LoginForm();
            loginForm.Show();
            this.Close();
        }
    }
}
