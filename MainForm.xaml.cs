using System;
using System.Collections.Generic;
using System.Data;
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
using Path = System.IO.Path;
using wpf_тесты_для_обучения.Properties;
using static System.Net.Mime.MediaTypeNames;
using wpf_тесты_для_обучения.Enums;
using System.Windows.Media.Effects;
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
            try
            {
                InitializeComponent();
                _databaseHelper = databaseHelper;
                LoadTests();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Исключение: {ex.Message}\n" +
                   $"Метод: {ex.TargetSite}\n" +
                   $"Трассировка стека: {ex.StackTrace}", "Ошибка создания формы MainForm", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }

        public void LoadTests()
        {
            try
            {
                List<Tests> tests = _databaseHelper.GetTestsList(_databaseHelper._currentUser.UserRole.Id);
                wrapPanel.Children.Clear();

                foreach (var test in tests)
                {
                    int questionCount = _databaseHelper.GetQuestionCount(test.Id);
                    int passingThreshold = (int)Math.Round(questionCount * test.IsCompleted / 100.0);
                    bool isCompleted = _databaseHelper.IsTestCompleted(_databaseHelper._currentUser.Id, test.Id);

                    // Основной контейнер
                    StackPanel testContainer = new StackPanel
                    {
                        Orientation = Orientation.Vertical,
                        Margin = new Thickness(10, 10, 20, 20),
                        Effect = new DropShadowEffect
                        {
                            BlurRadius = 7,
                            Color = (Color)FindResource("LightStroke"),
                            ShadowDepth = 5,
                            Direction = 25
                        }
                    };

                    // Верхняя часть с Border и Button
                    Border darkBorder = new Border
                    {
                        Style = (Style)FindResource("DarkBorder"),
                        Padding = new Thickness(0),
                        BorderThickness = new Thickness(1, 1, 1, 0),
                        Margin = new Thickness(0, 0, 0, 0)
                    };

                    Button mainButton = new Button
                    {
                        Height = 270,
                        Width = 262,
                        Style = (Style)FindResource("HoverButtonStyle")
                    };
                    mainButton.SetValue(Panel.ZIndexProperty, 1);
                    // Контент основной кнопки
                    StackPanel buttonContent = new StackPanel
                    {
                        VerticalAlignment = VerticalAlignment.Top,
                        Margin = new Thickness(10, 10, 10, 0)
                    };

                    TextBlock titleBlock = new TextBlock
                    {
                        Text = test.Title,
                        Style = (Style)FindResource("SubheaderTextStyle"),
                        VerticalAlignment = VerticalAlignment.Top
                    };

                    TextBlock questionCountBlock = new TextBlock
                    {
                        Text = $"{questionCount} {GetQuestionWord(questionCount)}",
                        Style = (Style)FindResource("BaseTextStyle"),
                        Margin = new Thickness(0, 25, 0, 0),
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center
                    };

                    TextBlock thresholdLabelBlock = new TextBlock
                    {
                        Text = "порог прохождения:",
                        Style = (Style)FindResource("BaseTextStyle"),
                        Margin = new Thickness(0, 15, 0, 0),
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center
                    };

                    TextBlock thresholdValueBlock = new TextBlock
                    {
                        Text = $"{test.IsCompleted}% ({passingThreshold} {GetQuestionWord(passingThreshold)})",
                        Style = (Style)FindResource("BaseTextStyle"),
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center,
                        Margin = new Thickness(0, 0, 0, 50)
                    };

                    buttonContent.Children.Add(titleBlock);
                    buttonContent.Children.Add(questionCountBlock);
                    buttonContent.Children.Add(thresholdLabelBlock);
                    buttonContent.Children.Add(thresholdValueBlock);

                    mainButton.Content = buttonContent;
                    darkBorder.Child = mainButton;
                    testContainer.Children.Add(darkBorder);
                    // "Щит" от наведения
                    Border hoverBlocker = new Border
                    {
                        Width = 264,
                        Height = 20,
                        Background = Brushes.Transparent, // Важно: прозрачный!
                        Margin = new Thickness(0, -10, 0, 0),
                        IsHitTestVisible = true
                    };
                    hoverBlocker.SetValue(Panel.ZIndexProperty, 15);

                    testContainer.Children.Add(hoverBlocker);

                    // Кнопка "Результаты"
                    Button resultsButton = new Button
                    {
                        Style = (Style)FindResource("GreyButtonStyle"),
                        Content = "Результаты ▼",
                        Margin = new Thickness(0, -20, 0, 0),
                        Width = 264
                    };

                    testContainer.Children.Add(resultsButton);
                    resultsButton.SetValue(Panel.ZIndexProperty, 10);

                    // ListBox с результатами
                    ListBox resultsListBox = new ListBox
                    {
                        Style = (Style)FindResource("ListBoxStyle1")

                    };
                    resultsListBox.Visibility = Visibility.Collapsed;
                    // Получаем результаты теста
                    List<Results> testResults = _databaseHelper.GetResultsByTestId(test.Id);
                    foreach (var result in testResults)
                    {
                        ListBoxItem item = new ListBoxItem();
                        StackPanel resultPanel = new StackPanel { Orientation = Orientation.Horizontal };

                        TextBlock attemptBlock = new TextBlock { Text = $"Попытка {result.RowNumber}({result.Date.ToString("dd.MM.yyyy")}): " };
                        TextBlock scoreBlock = new TextBlock { Text = $"{result.Score} {GetBallWord(result.Score)} (" };
                        TextBlock percentBlock = new TextBlock { Text = $"{result.Percent}%)" };

                        resultPanel.Children.Add(attemptBlock);
                        resultPanel.Children.Add(scoreBlock);
                        resultPanel.Children.Add(percentBlock);

                        item.Tag = result;
                        item.Content = resultPanel;
                        item.MouseLeftButtonUp += Item_MouseLeftButtonUp;
                        resultsListBox.Items.Add(item);
                    }
                    testContainer.Children.Add(resultsListBox);
                    wrapPanel.Children.Add(testContainer);

                    // Обработчики событий
                    mainButton.Tag = test;
                    mainButton.Click += TestButton_Click;

                    resultsButton.Tag = test.Id;
                    resultsButton.Click += (s, e) =>
                    {
                        resultsListBox.Visibility = resultsListBox.Visibility == Visibility.Visible
                            ? Visibility.Collapsed
                            : Visibility.Visible;
                    };
                }

                UpdateCompletedTestsLabel();
                UpdateTestButtonColors();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки тестов: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void Item_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (sender is ListBoxItem item && item.Tag is Results result)
            {
                var form = new TestViewMistakesForm(
                    _databaseHelper,
                    result.TestId,
                    result.Id, // или другой нужный ID
                    _databaseHelper._currentUser.Id);

                form.ShowDialog(); // или .Show(), если не модально
            }
        }

        public static string GetQuestionWord(int number)
        {
            int lastDigit = number % 10;
            int lastTwoDigits = number % 100;

            // Исключение для чисел 11-14
            if (lastTwoDigits >= 11 && lastTwoDigits <= 14)
            {
                return "вопросов";
            }

            switch (lastDigit)
            {
                case 1:
                    return "вопрос";
                case 2:
                case 3:
                case 4:
                    return "вопроса";
                default:
                    return "вопросов";
            }
        }
        public static string GetBallWord(double number)
        {
            double lastDigit = number % 10;
            double lastTwoDigits = number % 100;

            // Исключение для чисел 11-14
            if (lastTwoDigits >= 11 && lastTwoDigits <= 14)
            {
                return "баллов";
            }

            switch (lastDigit)
            {
                case 1:
                    return "балл";
                case 2:
                case 3:
                case 4:
                    return "балла";
                default:
                    return "баллов";
            }
        }



        private void UpdateCompletedTestsLabel()
        {
            try
            {
                //всего тестов
                int totalTests = _databaseHelper.GetTestsList(_databaseHelper._currentUser.UserRole.Id).Count;
                //пройдено тестов(прошли процентный порог)
                DataTable completedTests = _databaseHelper.GetCompletedTestsCount(_databaseHelper._currentUser.Id);
                double averageOfAttempts = 0;
                foreach (DataRow row in completedTests.Rows)
                {
                    averageOfAttempts += Convert.ToDouble(row[1]);
                }
                averageOfAttempts /= completedTests.Rows.Count;
                testCountLabel.Text = $"Пройдено тестов: {completedTests.Rows.Count} из {totalTests}";
                countLabel.Text = $"Всего тестов: {totalTests}";
                userNameTextBlock.Text = _databaseHelper._currentUser.FullName;
                //averageCountAttemptsLabel.Text = $"Среднее количество попыток для прохождения: {(double.IsNaN(averageOfAttempts) ? 0 : averageOfAttempts)}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Исключение: {ex.Message}\n" +
                   $"Метод: {ex.TargetSite}\n" +
                   $"Трассировка стека: {ex.StackTrace}", "Ошибка обновления информации о тестах", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
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
            try
            {
                TestViewPassForm testViewPassForm = new TestViewPassForm(_databaseHelper, TestMode.Pass, test.Id);
                testViewPassForm.ShowDialog();
                LoadTests();
                UpdateCompletedTestsLabel();
                UpdateTestButtonColors();

                UpdateResultBlock(test.Id, test.Count);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Исключение: {ex.Message}\n" +
                   $"Метод: {ex.TargetSite}\n" +
                   $"Трассировка стека: {ex.StackTrace}", "Ошибка открытия теста", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }
        private void UpdateResultBlock(int testId, int testCount)
        {
            try
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
            catch (Exception ex)
            {
                MessageBox.Show($"Исключение: {ex.Message}\n" +
                   $"Метод: {ex.TargetSite}\n" +
                   $"Трассировка стека: {ex.StackTrace}", "Ошибка обновления блока результата", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }


        private void UpdateTestButtonColors()
        {
            try
            {
                foreach (var child in wrapPanel.Children)
                {
                    if (child is StackPanel testContainer)
                    {
                        Button testButton = testContainer.Children.OfType<Button>().FirstOrDefault();
                        Border border = testContainer.Children.OfType<Border>().FirstOrDefault();
                        Button ttt = border?.Child as Button;
                        StackPanel ttt2 = ttt?.Content as StackPanel;

                        TextBlock [] texts = ttt2.Children.OfType<TextBlock>().ToArray();
                        if (testButton != null && testButton.Tag is int test_id)
                        {
                            bool isCompleted = _databaseHelper.IsTestCompleted(_databaseHelper._currentUser.Id, test_id);
                            int attemptCount = _databaseHelper.GetUserAttemptCount(_databaseHelper._currentUser.Id, test_id);

                            if (isCompleted)
                            {
                                ttt.Style = (Style)FindResource("HoverButtonStyleDone");
                                testButton.Style = (Style)FindResource("GreyButtonStyleDone");
                                border.BorderBrush = (SolidColorBrush)FindResource("BackgroundBrush");
                                foreach (var item in ttt2.Children)
                                {
                                    if (item is TextBlock textBlock)
                                        textBlock.Foreground = (SolidColorBrush)FindResource("BackgroundBrush");
                                }
                            }
                            else if (attemptCount > 0)
                            {
                                ttt.Style = (Style)FindResource("HoverButtonStyleFail");
                                testButton.Style = (Style)FindResource("GreyButtonStyleFail");
                                border.BorderBrush = (SolidColorBrush)FindResource("BackgroundBrush");
                                foreach (var item in ttt2.Children)
                                {
                                    if (item is TextBlock textBlock)
                                        textBlock.Foreground = (SolidColorBrush)FindResource("BackgroundBrush");
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Исключение: {ex.Message}\n" +
                   $"Метод: {ex.TargetSite}\n" +
                   $"Трассировка стека: {ex.StackTrace}", "Ошибка обновления цветов блоков", MessageBoxButton.OK, MessageBoxImage.Exclamation);
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
