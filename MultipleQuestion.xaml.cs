using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using wpf_тесты_для_обучения.Properties;
using System.IO;
using Path = System.IO.Path;
using static MaterialDesignThemes.Wpf.Theme;
using TextBox = System.Windows.Controls.TextBox;
using Button = System.Windows.Controls.Button;
using CheckBox = System.Windows.Controls.CheckBox;
using wpf_тесты_для_обучения.Enums;

namespace wpf_тесты_для_обучения
{
    /// <summary>
    /// Логика взаимодействия для MultipleQuestion.xaml
    /// </summary>
    /// 

    public partial class MultipleQuestion : BaseQuestion
    {
        public DatabaseHelper _databaseHelper;
        private bool viewMode;
        private int user;
        private int currentRes;
        public int CurrentRes
        {
            get; set;
        }

        // public Questions question = null;
        TestMode _mode;
        //  public Questions question = null;
        public MultipleQuestion( DatabaseHelper databaseHelper, bool showImage, Questions q, bool isViewMode, string imageSource = "", bool isCopy = false)
        {
            try
            {
                InitializeComponent();
                DataContext = this;
                _databaseHelper = databaseHelper;
                ShowImage = showImage;
                viewMode = isViewMode;
                question = q;
                ImageSource = !string.IsNullOrEmpty(imageSource)
                   ? new BitmapImage(new Uri("file:///" + Path.Combine(Directory.GetCurrentDirectory(), imageSource).Replace("\\", "/")))
                   : null;
                // MessageBox.Show("MultiQ: " + "file:///" + Path.Combine(Directory.GetCurrentDirectory(), imageSource).Replace("\\", "/"));
                if (!isCopy)
                    LoadFromDatabase(q);
                else
                    CopyTest(q);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при отключении базы данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }
        public MultipleQuestion(DatabaseHelper databaseHelper, bool showImage, Questions q, TestMode mode, int idRes, int User, string imageSource = "")
        {
            try
            {
                InitializeComponent();
                DataContext = this;
                _databaseHelper = databaseHelper;
                ShowImage = showImage;
                viewMode = true;
                _mode = mode;
                question = q;
                CurrentRes = idRes;
                user = User;
                ImageSource = !string.IsNullOrEmpty(imageSource)
                ? new BitmapImage(new Uri("file:///" + Path.Combine(Directory.GetCurrentDirectory(), imageSource).Replace("\\", "/")))
                : null;
                LoadFromDatabase(q);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при отключении базы данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        public MultipleQuestion(DatabaseHelper databaseHelper, bool showImage, string imageSource = "")
        {
            InitializeComponent();
            DataContext = this;
            _databaseHelper = databaseHelper;
            ShowImage = showImage;
            ImageSource = imageSource != "" ? new BitmapImage(new Uri(imageSource)) : new BitmapImage(new Uri("pack://application:,,,/LoadImage.png"));
            AddAnswer();
        }

        private void CopyTest(Questions question)
        {
            questionTextBlock.Visibility = Visibility.Collapsed;
            questionTextBox.Visibility = Visibility.Visible;
            questionTextBox.Text = question.QuestionText;
            foreach (var answer in question.Answers)
            {
                AddAnswerFromDatabase(answer.AnswerText, answer.IsCorrect, answer.Id);
            }
        }
        private void LoadFromDatabase(Questions q)
        {
            string query = "SELECT Question_Text FROM Questions WHERE Id = @id";
            SqlParameter[] parameters = { new SqlParameter("@id", q.Id) };
            string questionText = _databaseHelper.ExecuteScalar(query, parameters)?.ToString();

            if (viewMode)
            {
                questionTextBox.Visibility = Visibility.Collapsed;
                questionTextBlock.Visibility = Visibility.Visible;
                questionTextBlock.Text = questionText;
            }
            else
            {
                questionTextBlock.Visibility = Visibility.Collapsed;
                questionTextBox.Visibility = Visibility.Visible;
                questionTextBox.Text = questionText;
            }

            // Загружаем ответы
            query = "SELECT Id, Answer_Text, Is_Correct FROM Answers WHERE Question_Id = @id";
            SqlParameter[] parameters2 = { new SqlParameter("@id", q.Id) };
            DataTable answersTable = _databaseHelper.ExecuteSelectQuery(query, parameters2);

            q.Answers = new List<Answers>(); // Очищаем перед новой загрузкой
            AnswersPanel.Children.Clear();   // Очищаем UI

            foreach (DataRow row in answersTable.Rows)
            {
                int answerId = Convert.ToInt32(row["Id"]);
                string answerText = row["Answer_Text"].ToString();
                bool isCorrect = (bool)row["Is_Correct"];

                q.Answers.Add(new Answers(answerId, q.Id, answerText, isCorrect));
                AddAnswerFromDatabase(answerText, isCorrect, answerId);

            }
            if (_mode == TestMode.ViewMistakes)
            {
                query = "Select COUNT(Answers.Id) From Answers JOIN Questions ON Answers.Question_Id = Questions.Id Where Question_Id = @idQ and Is_Correct = 1";
                SqlParameter[] parameters3 = {
                        new SqlParameter("@idQ", q.Id)};

                int countCorrectAnswers = (int)_databaseHelper.ExecuteScalar(query, parameters3);
            }
        }

        private void AddAnswerFromDatabase(string answerText, bool isCorrect, int answerId)
        {
            Grid answerGrid = new Grid
            {
                Margin = new Thickness(0, 0, 0, 5)
            };
            // Колонки: Иконка (только в viewMode), RadioButton, Text, (в режиме редактирования) кнопка удаления
            answerGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto }); // Иконка (если есть)
            answerGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto }); // RadioButton
            answerGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }); // Текст
            if (!viewMode)
                answerGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto }); // Кнопка удаления

            CheckBox checkBox = new CheckBox
            {
                //Margin = new Thickness(0, 2, 5, 0),
                IsChecked = viewMode ? false : isCorrect,
                Tag = answerId,
                Style=(Style)FindResource("OrangeCheckBoxStyle"),
                VerticalAlignment = VerticalAlignment.Center, // Центрируем по вертикали
                HorizontalAlignment = HorizontalAlignment.Center // Центрируем по горизонтали
            };
            Grid.SetColumn(checkBox, 1);
            answerGrid.Children.Add(checkBox);

            if (viewMode)
            {
                if (_mode == TestMode.ViewMistakes)
                {

                    string query = "SELECT Mistakes.User_Answer FROM Mistakes " +
                        "JOIN Answers ON Mistakes.Question_Id = Answers.Question_Id " +
                        "WHERE Answers.Id = @id AND Mistakes.User_Id = @userId And Mistakes.Result_Id = @resId ";
                    SqlParameter[] parameters2 = {
                        new SqlParameter("@id", answerId),
                        new SqlParameter("@resId", CurrentRes),
                        new SqlParameter("@userId", user) };
                    DataTable answersTable = _databaseHelper.ExecuteSelectQuery(query, parameters2);
                    foreach (DataRow row in answersTable.Rows)
                    {
                        if ((int)row["User_Answer"] == answerId)
                        {
                            checkBox.IsChecked = true;
                        }
                        if (isCorrect)
                        {
                            var greenCheckMark = new TextBlock
                            {
                                Text = "✔", // Unicode для галочки
                                Foreground = (SolidColorBrush)FindResource("DoneColorBrush"),
                                FontSize = 16,
                                VerticalAlignment = VerticalAlignment.Center,
                                HorizontalAlignment = HorizontalAlignment.Center // Центрируем по горизонтали
                            };
                            Grid.SetColumn(greenCheckMark, 0); // Указываем расположение в сетке
                            answerGrid.Children.Add(greenCheckMark);
                        }
                        else
                        {
                            var redCross = new TextBlock
                            {
                                Text = "✖", // Unicode для крестика
                                Foreground = (SolidColorBrush)FindResource("FailColorBrush"),
                                FontSize = 16,
                                VerticalAlignment = VerticalAlignment.Center,
                                HorizontalAlignment = HorizontalAlignment.Center // Центрируем по горизонтали
                            };
                            Grid.SetColumn(redCross, 0); // Указываем расположение в сетке
                            answerGrid.Children.Add(redCross);
                        }
                    }
                    addAnswerButton.Visibility = Visibility.Collapsed;
                    deleteQuestionButton.Visibility = Visibility.Collapsed;
                }

                TextBlock textBlock = new TextBlock
                {
                    Text = answerText,
                    Style = (Style)FindResource("BaseTextStyle"),
                    TextTrimming = TextTrimming.CharacterEllipsis,
                    VerticalAlignment = VerticalAlignment.Center, // Центрируем по вертикали
                    HorizontalAlignment = HorizontalAlignment.Left, // Выравниваем по левому краю
                    Margin = new Thickness(0)
                };
                Grid.SetColumn(textBlock, 2);
                answerGrid.Children.Add(textBlock);

                addAnswerButton.Visibility = Visibility.Collapsed;
                deleteQuestionButton.Visibility = Visibility.Collapsed;

                textBlock.PreviewMouseLeftButtonDown += (s, ev) =>
                {
                    checkBox.IsChecked = !checkBox.IsChecked;
                };

            }
            else
            {
                TextBox textBox = new TextBox
                {
                    Text = answerText,
                    Foreground = Brushes.Black,
                    FontSize = 14,
                    MinWidth = 200,
                    TextWrapping = TextWrapping.Wrap,
                    Margin = new Thickness(0),
                    VerticalAlignment = VerticalAlignment.Center, // Центрируем по вертикали
                    HorizontalAlignment = HorizontalAlignment.Left
                };
                Grid.SetColumn(textBox, 2);
                answerGrid.Children.Add(textBox);

                Button deleteButton = new Button
                {
                    Content = "❌",
                    Style = (Style)FindResource("AddAnswerButtonStyle"),
                    Width = 20,
                    Height = 20,
                    Margin = new Thickness(5, 0, 0, 4),
                    VerticalAlignment = VerticalAlignment.Center
                };
                deleteButton.Click += DeleteAnswer;
                Grid.SetColumn(deleteButton, 3);
                answerGrid.Children.Add(deleteButton);

                //textBox.PreviewMouseLeftButtonUp += TextBox_SelectAll;
                textBox.PreviewMouseLeftButtonUp += (s, er) => TextBox_SelectAll(s, er, textBox.Text);
                //textBox.MouseDoubleClick += TextBox_SelectAll;
                textBox.TextChanged += (s, ee) => UpdateErrorMessages();
                checkBox.Checked += (s, t) => UpdateErrorMessages();
            }

            AnswersPanel.Children.Add(answerGrid);
            if (!viewMode) UpdateErrorMessages();
        }

        protected override void AddAnswer(object sender = null, RoutedEventArgs e = null)
        {
            var newAnswer = new StackPanel { Orientation = Orientation.Horizontal };

            var checkBox = new CheckBox { Margin = new Thickness(0, 2, 5, 0) };
            var textBox = new TextBox { Text = "Введите ответ",
                MinWidth = 200,
                MaxWidth = 500,
                Foreground = Brushes.Black,
                FontSize = 14,
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(0, 0, 0, 5)
            };
            var deleteButton = new Button { Content = "❌",
                Style = (Style)FindResource("AddAnswerButtonStyle"),
                Width = 20, Height = 20, Margin = new Thickness(5, 0, 0, 4) };
            deleteButton.Click += DeleteAnswer;
           // textBox.PreviewMouseLeftButtonUp += TextBox_SelectAll;
            textBox.PreviewMouseLeftButtonUp += (s, er) => TextBox_SelectAll(s, er, textBox.Text);
            //textBox.MouseDoubleClick += TextBox_SelectAll;
            textBox.TextChanged += (s, ee) => UpdateErrorMessages();
            checkBox.Checked += (s, ee) => UpdateErrorMessages();

            newAnswer.Children.Add(checkBox);
            newAnswer.Children.Add(textBox);
            newAnswer.Children.Add(deleteButton);

            AnswersPanel.Children.Add(newAnswer);
            UpdateErrorMessages();
        }

        private void DeleteAnswer(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Parent is Grid grid)
            {
                AnswersPanel.Children.Remove(grid);
                UpdateErrorMessages();
            }
            if (sender is Button button1 && button1.Parent is StackPanel panel)
            {
                AnswersPanel.Children.Remove(panel);
                UpdateErrorMessages();
            }
        }
        protected override void UpdateErrorMessages()
        {
            string errorMessages = string.IsNullOrWhiteSpace(questionTextBox.Text) || questionTextBox.Text == "Введите вопрос?"
                ? "Вопрос не может быть пустым.\n"
                : "";

            int checkedCount = 0;
            int emptyAnswerCount = 0;

            foreach (var child in AnswersPanel.Children)
            {
                if (child is StackPanel panel)
                {
                    foreach (UIElement element in panel.Children)
                    {
                        if (element is CheckBox checkBox && checkBox.IsChecked == true)
                        {
                            checkedCount++;
                        }
                        else if (element is TextBox textBox)
                        {
                            if (string.IsNullOrWhiteSpace(textBox.Text) || textBox.Text == "Введите ответ")
                                emptyAnswerCount++;
                        }
                    }
                }
                if (child is Grid grid)
                {
                    foreach (UIElement element in grid.Children)
                    {
                        if (element is CheckBox checkBox && checkBox.IsChecked == true)
                        {
                            checkedCount++;
                        }
                        else if (element is TextBox textBox)
                        {
                            if (string.IsNullOrWhiteSpace(textBox.Text) || textBox.Text == "Введите ответ")
                                emptyAnswerCount++;
                        }
                    }
                }
            }

            if (emptyAnswerCount > 0)
                errorMessages += "Введите варианты ответа!\n";

            if (checkedCount < 2)
                errorMessages += "Выберите минимум 2 варианта ответа!\n";

            if (ShowImage && ImageSource == null)
                errorMessages += "Добавьте картинку!\n";

            ErrorTextBlock.Text = errorMessages;
            ErrorTextBlock.Visibility = string.IsNullOrEmpty(errorMessages) ? Visibility.Collapsed : Visibility.Visible;
            //ZeroErrors = ErrorTextBlock.Visibility != Visibility.Visible;
            ZeroErrors = ErrorTextBlock.Visibility != Visibility.Visible;
        }


        private void image_MouseDown_1(object sender, MouseButtonEventArgs e)
        {
            if (!viewMode)
            {
                OpenFileDialog openFileDialog = new OpenFileDialog
                {
                    Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp;*.gif",
                    Title = "Выберите изображение"
                };

                if (openFileDialog.ShowDialog() == true)
                {
                    image.Source = new BitmapImage(new Uri(openFileDialog.FileName));
                    ImageSource = new BitmapImage(new Uri(openFileDialog.FileName));
                    ImagePath = openFileDialog.FileName;
                }

                UpdateErrorMessages();
            }
            if (viewMode)
            {
                ShowFullImage(sender, e, AnswersPanel, questionTextBlock.Text);
            }
        }

        private void questionTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            UpdateErrorMessages();
        }

        private void questionTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (AnswersPanel != null)
                UpdateErrorMessages();
        }

        private void questionTextBox_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (questionTextBox.Text == "Введите вопрос?")
                questionTextBox.SelectAll();
        }
        public override void SetError(string message)
        {
            ErrorTextBlock.Text = message;
            ErrorTextBlock.Visibility = Visibility.Visible;
        }

        public override void ClearError()
        {
            ErrorTextBlock.Text = string.Empty;
            ErrorTextBlock.Visibility = Visibility.Collapsed;
        }
        private void image_MouseEnter(object sender, MouseEventArgs e)
        {
        }
    }

    
}
