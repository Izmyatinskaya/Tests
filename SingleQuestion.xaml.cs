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

namespace wpf_тесты_для_обучения
{
    /// <summary>
    /// Логика взаимодействия для SingleQuestion.xaml
    /// </summary>
    /// 

    public partial class SingleQuestion : BaseQuestion
    {
        private string _groupName;
        private DatabaseHelper _databaseHelper;
        private bool viewMode;
       // public Questions question = null;
        public SingleQuestion(bool showImage, Questions q, bool isViewMode, string imageSource = "")
        {
            InitializeComponent();
            _databaseHelper = new DatabaseHelper("Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=\"D:\\Проекты\\Тесты обучение WPF\\wpf тесты для обучения\\DB.mdf\";Integrated Security=True");
            _groupName = "Group_" + Number;
            ShowImage = showImage;
            viewMode = isViewMode;
            question = q;
            ImageSource = !string.IsNullOrEmpty(imageSource)
            ? new BitmapImage(new Uri("file:///" + Path.Combine(Directory.GetCurrentDirectory(), imageSource).Replace("\\", "/")))
            : null;

            LoadFromDatabase(q); 
        }
        public SingleQuestion(bool showImage, string imageSource = "")
        {
            InitializeComponent();
            _groupName = "Group_" + Number;
            ShowImage = showImage;
            ImageSource = imageSource != "" ? new BitmapImage(new Uri(imageSource)) : null;
            AddAnswer();
        }
        public SingleQuestion()
        {
            
        }
        private void LoadFromDatabase(Questions q)
        {
            string query = "SELECT Question_Text FROM Questions WHERE Id = @id";
            SqlParameter[] parameters = { new SqlParameter("@id", q.Id) };
            string questionText = _databaseHelper.ExecuteScalar(query, parameters)?.ToString();

            if(viewMode)
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

            // Загружаем ответы с их ID
            query = "SELECT Id, Answer_Text, Is_Correct FROM Answers WHERE Question_Id = @id";
            SqlParameter[] parameters2 = { new SqlParameter("@id", q.Id) };
            DataTable answersTable = _databaseHelper.ExecuteSelectQuery(query, parameters2);

            q.Answers = new List<Answers>(); // Очищаем перед новой загрузкой

            foreach (DataRow row in answersTable.Rows)
            {
                int answerId = Convert.ToInt32(row["Id"]);
                string answerText = row["Answer_Text"].ToString();
                bool isCorrect = (bool)row["Is_Correct"];

                q.Answers.Add(new Answers(answerId, q.Id, answerText, isCorrect)); // Добавляем в список ответов
                AddAnswerFromDatabase(answerText, isCorrect, answerId); // Передаём ID в метод
            }
        }

        private void AddAnswerFromDatabase(string answerText, bool isCorrect, int answerId)
        {
            StackPanel answerPanel = new StackPanel { Orientation = Orientation.Horizontal };
            RadioButton radioButton = viewMode 
                ? new RadioButton { GroupName = _groupName, IsChecked = false, Margin = new Thickness(0,2,5,0)} 
                : new RadioButton { GroupName = _groupName, IsChecked = isCorrect, Margin = new Thickness(0, 2, 5, 0) };

            radioButton.Tag = answerId;
            answerPanel.Children.Add(radioButton);
            if (viewMode)
            {
                TextBlock textBlock = new TextBlock
                {
                    Text = answerText,
                    MinWidth = 200,
                    MaxWidth = 700,
                    Foreground = Brushes.Black,
                    FontSize = 14,
                    TextWrapping = TextWrapping.Wrap, // Включает перенос строк
                    TextTrimming = TextTrimming.CharacterEllipsis,
                    Margin = new Thickness(0, 0, 0, 5)
                };
                answerPanel.Children.Add(textBlock);
                addAnswerButton.Visibility = Visibility.Collapsed;
                deleteQuestionButton.Visibility = Visibility.Collapsed;
            }
            else
            {
                TextBox textBox = new TextBox
                {
                    Text = answerText,
                    MinWidth = 200,
                    MaxWidth = 500,
                    Foreground = Brushes.Black,
                    FontSize = 14,
                    TextWrapping = TextWrapping.Wrap,
                    Margin = new Thickness(0, 0, 0, 5)
                };
                answerPanel.Children.Add(textBox);
                var deleteButton = new Button { Content = "❌", Width = 20, Height = 20, Margin = new Thickness(5, 0, 0, 4) };
                deleteButton.Click += DeleteAnswer;
                textBox.PreviewMouseLeftButtonUp += TextBox_SelectAll;
                textBox.TextChanged += (s, ee) => UpdateErrorMessages();
                radioButton.Checked += (s, t) => UpdateErrorMessages();
                answerPanel.Children.Add(deleteButton);
            }
            AnswersPanel.Children.Add(answerPanel);
            if(!viewMode)UpdateErrorMessages();


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
            var textBox = new TextBox { Text = "Введите ответ",
                MinWidth = 200,
                MaxWidth = 500,
                Foreground = Brushes.Black,
                FontSize = 14,
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(0, 0, 0, 5)
            };
            var deleteButton = new Button { Content = "❌", Width = 20, Height = 20, Margin = new Thickness(5, 0, 0, 4) };
            deleteButton.Click += DeleteAnswer;
            textBox.PreviewMouseLeftButtonUp += TextBox_SelectAll;
            textBox.TextChanged += (s, ee) => UpdateErrorMessages();
            radioButton.Checked += (s, t ) => UpdateErrorMessages();

            newAnswer.Children.Add(radioButton);
            newAnswer.Children.Add(textBox);
            newAnswer.Children.Add(deleteButton);

            AnswersPanel.Children.Add(newAnswer);
            UpdateErrorMessages();
        }

        private void DeleteAnswer(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var stackPanel = button?.Parent as StackPanel;
            AnswersPanel.Children.Remove(stackPanel);
            UpdateErrorMessages();
        }

        protected override void UpdateErrorMessages()
        {
            string errorMessages = (string.IsNullOrWhiteSpace(questionTextBox.Text) ) ? "Вопрос не может быть пустым.\n" : "";
             errorMessages += (questionTextBox.Text == "Введите вопрос?") ? "Введите вопрос.\n" : "";
            int count = 0;
            int count2 = 0;
            
            foreach (var answerChild in AnswersPanel.Children)
            {
                if (answerChild is StackPanel stackPanel)
                {
                    foreach (var stackPanelChild in stackPanel.Children)
                    {
                        if (stackPanelChild is RadioButton radioButton)
                        {
                            if((bool)radioButton.IsChecked) count++;
                        }

                        if (stackPanelChild is TextBox textBox)
                        {
                            if (textBox.Text == "" || textBox.Text == "Введите ответ")
                                count2++;
                        }
                    }
                }
            }
            errorMessages += (count2 > 0) ? "Введите варианты ответа!\n" : "";
            errorMessages += (count < 1) ? "Выберите вариант ответа!\n":"";

            if (ShowImage && ImageSource == null)
                errorMessages += "Добавьте картинку!\n";
            ErrorTextBlock.Text = errorMessages;
            ErrorTextBlock.Visibility = string.IsNullOrEmpty(errorMessages) ? Visibility.Collapsed : Visibility.Visible;

            ZeroErrors =  ErrorTextBlock.Visibility == Visibility.Visible ? false : true;
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
                ShowFullImage(sender, e);
            }

        }

        private void questionTextBox_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            questionTextBox.SelectAll();
        }

        private void questionTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            UpdateErrorMessages();
        }

        private void questionTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(AnswersPanel != null)
            UpdateErrorMessages();
        }
    }


}
