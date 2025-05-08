using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
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
using Path = System.IO.Path;
using wpf_тесты_для_обучения.Enums;
using System.Collections;

namespace wpf_тесты_для_обучения
{
    /// <summary>
    /// Логика взаимодействия для SingleQuestion.xaml
    /// </summary>
    /// 

    public partial class SingleQuestion : BaseQuestion
    {
        private string _groupName;
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
        public SingleQuestion(DatabaseHelper databaseHelper, bool showImage, Questions q, bool isViewMode, string imageSource = "", bool isCopy = false)
        {
            try
            {
                InitializeComponent();
                DataContext = this;
                _databaseHelper = databaseHelper;
                _groupName = "Group_" + Number;
                ShowImage = showImage;
                viewMode = isViewMode;
                question = q;
                ImageSource = !string.IsNullOrEmpty(imageSource)
                ? new BitmapImage(new Uri("file:///" + Path.Combine(Directory.GetCurrentDirectory(), imageSource).Replace("\\", "/")))
                : null;
                if(!isCopy ) 
                LoadFromDatabase(q);
                else
                    CopyTest(q);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при отключении базы данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        public SingleQuestion(DatabaseHelper databaseHelper, bool showImage, Questions q, TestMode mode, int idRes, int User, string imageSource = "" )
        {
            try
            {
                InitializeComponent();
                DataContext = this;
                _databaseHelper = databaseHelper;
                _groupName = "Group_" + Number;
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

        public SingleQuestion(DatabaseHelper databaseHelper, bool showImage, string imageSource = "")
        {
            InitializeComponent();
            DataContext = this;
            _databaseHelper = databaseHelper;
            _groupName = "Group_" + Number;
            ShowImage = showImage;
            ImageSource = imageSource != "" ? new BitmapImage(new Uri(imageSource)) : new BitmapImage(new Uri("pack://application:,,,/LoadImage.png"));
            AddAnswer();
        }
        public SingleQuestion()
        {

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

            q.Answers = new List<Answers>();

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
            var answerGrid = new Grid
            {
                Margin = new Thickness(0, 0, 0, 5),
            };

            // Колонки: Иконка (только в viewMode), RadioButton, Text, (в режиме редактирования) кнопка удаления
            answerGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto }); // Иконка (если есть)
            answerGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto }); // RadioButton
            answerGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }); // Текст
            if (!viewMode)
                answerGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto }); // Кнопка удаления

            // --- RadioButton ---
            var radioButton = new RadioButton
            {
                GroupName = _groupName,
                IsChecked = viewMode ? false : isCorrect,
                //Margin = new Thickness(5, 0, 5, 0),
                Tag = answerId,
                Style = (Style)FindResource("OrangeRadioButtonStyle"),
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Left // Важно: Left вместо Center
            };
            Grid.SetColumn(radioButton, 1); // Всегда вторая колонка (после иконки)
            answerGrid.Children.Add(radioButton);

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

                    if (isCorrect)
                    { // Зеленая галочка
                        var greenCheckMark = new TextBlock
                        {
                            Text = "✔",
                            Foreground = (SolidColorBrush)FindResource("DoneColorBrush"),
                            FontSize = 16,
                            VerticalAlignment = VerticalAlignment.Center,
                            HorizontalAlignment = HorizontalAlignment.Left
                        };
                        Grid.SetColumn(greenCheckMark, 0);
                        answerGrid.Children.Add(greenCheckMark);
                    }
                    else if (!isCorrect)
                    {
                        // Красный крестик
                        var redCross = new TextBlock
                        {
                            Text = "✖",
                            Foreground = (SolidColorBrush)FindResource("FailColorBrush"),
                            FontSize = 16,
                            VerticalAlignment = VerticalAlignment.Center,
                            HorizontalAlignment = HorizontalAlignment.Left
                        };
                        Grid.SetColumn(redCross, 0);
                        answerGrid.Children.Add(redCross);
                    }

                    if ((int)answersTable.Rows[0][0] == answerId)
                    {
                        radioButton.IsChecked = true; 
                        //if(isCorrect)
                            
                    }

                    addAnswerButton.Visibility = Visibility.Collapsed;
                    deleteQuestionButton.Visibility = Visibility.Collapsed;
                }

                var textBlock = new TextBlock
                {
                    Text = answerText,
                    Style=(Style)FindResource("BaseTextStyle"),
                    TextTrimming = TextTrimming.CharacterEllipsis,
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    ////Margin = new Thickness(5, 0, 0, 0)
                };
                Grid.SetColumn(textBlock, 2); // Текст в третьей колонке
                answerGrid.Children.Add(textBlock);

                textBlock.PreviewMouseLeftButtonDown += (s, ev) =>
                {
                    radioButton.IsChecked = !radioButton.IsChecked;
                };

                addAnswerButton.Visibility = Visibility.Collapsed;
                deleteQuestionButton.Visibility = Visibility.Collapsed;
            }
            else
            {
                var textBox = new TextBox
                {
                    Text = answerText,
                    MinWidth = 200,
                    Foreground = Brushes.Black,
                    FontSize = 14,
                    TextWrapping = TextWrapping.Wrap,
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    Margin = new Thickness(5, 0, 0, 0)
                };
                Grid.SetColumn(textBox, 2); // Текстбокс в третьей колонке
                answerGrid.Children.Add(textBox);

                var deleteButton = new Button
                {
                    Content = "❌",
                    Style = (Style)FindResource("AddAnswerButtonStyle"),
                    Width = 20,
                    Height = 20,
                    Margin = new Thickness(5, 0, 0, 0),
                    VerticalAlignment = VerticalAlignment.Center
                };
                Grid.SetColumn(deleteButton, 3); // Кнопка удаления в четвертой колонке
                answerGrid.Children.Add(deleteButton);

                deleteButton.Click += DeleteAnswer;
                textBox.PreviewMouseLeftButtonUp += (s, er) => TextBox_SelectAll(s, er, textBox.Text);
                textBox.TextChanged += (s, ee) => UpdateErrorMessages();
                radioButton.Checked += (s, t) => UpdateErrorMessages();
            }

            AnswersPanel.Children.Add(answerGrid);

            if (!viewMode)
                UpdateErrorMessages();
        }

        private void UpdateAnswersGroup()
        {
            foreach (var item in AnswersPanel.Children.OfType<StackPanel>())
            {
                var radioButton = item.Children.OfType<RadioButton>().FirstOrDefault();
                if (radioButton != null)
                {
                    radioButton.GroupName = _groupName;
                }
            }
        }

        protected override void AddAnswer(object sender = null, RoutedEventArgs e = null)
        {
            var newAnswer = new StackPanel { Orientation = Orientation.Horizontal };

            var radioButton = new RadioButton { GroupName = _groupName, Margin = new Thickness(0, 2, 5, 0) };
            var textBox = new TextBox
            {
                Text = "Введите ответ",
                Background = Brushes.White,
                MinWidth = 200,
                MaxWidth = 500,
                FontSize = 14,
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(0, 0, 0, 5)
            };
            var deleteButton = new Button
            {
                Style = (Style)FindResource("AddAnswerButtonStyle"),
                Content = "❌",
                Width = 20,
                Height = 20,
                Margin = new Thickness(5, 0, 0, 4)
            };
            deleteButton.Click += DeleteAnswer;
            //textBox.PreviewMouseLeftButtonUp += TextBox_SelectAll;
            textBox.PreviewMouseLeftButtonUp += (s, er) => TextBox_SelectAll(s, er, textBox.Text);
            //textBox.MouseDoubleClick += TextBox_SelectAll;
            textBox.TextChanged += (s, ee) => UpdateErrorMessages();
            radioButton.Checked += (s, t) => UpdateErrorMessages();

            newAnswer.Children.Add(radioButton);
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
            string errorMessages = string.IsNullOrWhiteSpace(questionTextBox.Text)
                ? "Вопрос не может быть пустым.\n"
                : "";

            if (questionTextBox.Text == "Введите вопрос?")
                errorMessages += "Введите вопрос.\n";

            int checkedCount = 0;
            int emptyAnswersCount = 0;

            foreach (var child in AnswersPanel.Children)
            {
                if (child is StackPanel panel)
                {
                    // Ожидаем: RadioButton (0), TextBox или TextBlock (1), DeleteButton (2, только в редактировании)
                    foreach (var element in panel.Children)
                    {
                        if (element is RadioButton radioButton && radioButton.IsChecked == true)
                            checkedCount++;

                        if (element is TextBox textBox)
                        {
                            if (string.IsNullOrWhiteSpace(textBox.Text) || textBox.Text == "Введите ответ")
                                emptyAnswersCount++;
                        }
                    }
                }
                if (child is Grid grid)
                {
                    // Ожидаем: RadioButton (0), TextBox или TextBlock (1), DeleteButton (2, только в редактировании)
                    foreach (var element in grid.Children)
                    {
                        if (element is RadioButton radioButton && radioButton.IsChecked == true)
                            checkedCount++;

                        if (element is TextBox textBox)
                        {
                            if (string.IsNullOrWhiteSpace(textBox.Text) || textBox.Text == "Введите ответ")
                                emptyAnswersCount++;
                        }
                    }
                }
            }

            if (emptyAnswersCount > 0)
                errorMessages += "Введите варианты ответа!\n";

            if (checkedCount < 1)
                errorMessages += "Выберите вариант ответа!\n";

            if (ShowImage && ImageSource == null)
                errorMessages += "Добавьте картинку!\n";

            ErrorTextBlock.Text = errorMessages;
            ErrorTextBlock.Visibility = string.IsNullOrEmpty(errorMessages) ? Visibility.Collapsed : Visibility.Visible;

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
        private void questionTextBox_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if(questionTextBox.Text == "Введите вопрос?")
            questionTextBox.SelectAll();
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
        private void questionTextBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //TextBox_SelectAll(sender, e);
        }

    }


}
