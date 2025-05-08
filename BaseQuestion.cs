using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using System.Windows.Media;
using System.IO;
using System.ComponentModel;
using System.Windows.Input;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;
using System.Windows.Controls.Primitives;

namespace wpf_тесты_для_обучения
{

    public abstract class BaseQuestion : UserControl, INotifyPropertyChanged
    {
        private static int questionCounter = 0; // Общий счетчик вопросов
        public Questions question { get; set; } //

        //public DockPanel ParentStackPanel { get; set; }
        public StackPanel ParentStackPanel { get; set; }
        public static bool ZeroErrors { get; set; }
        public string ImagePath
        {
            get { return (string)GetValue(ImagePathProperty); }
            set { SetValue(ImagePathProperty, value); }
        }
        public int Number
        {
            get { return (int)GetValue(NumberProperty); }
            set { SetValue(NumberProperty, value); }
        }

        private BitmapImage _imageSource;
        public BitmapImage ImageSource
        {
            get => _imageSource;
            set
            {
                //if (_imageSource == null)
                //    _imageSource = new BitmapImage(new Uri("pack://application:,,,/LoadImage.png"));
                //else
                    _imageSource = value;
                //MessageBox.Show("BitmapImage LocalPath: " + _imageSource.UriSource.LocalPath);
                //MessageBox.Show("BitmapImage AbsolutePath: " + _imageSource.UriSource.AbsolutePath);

                OnPropertyChanged(nameof(ImageSource));
            }
        }
        public bool ShowImage
        {
            get { return (bool)GetValue(ShowImageProperty); }
            set { SetValue(ShowImageProperty, value);}
        }

        public static readonly DependencyProperty ShowImageProperty =
            DependencyProperty.Register("ShowImage", typeof(bool), typeof(BaseQuestion), new PropertyMetadata(true));

        public static readonly DependencyProperty ImagePathProperty =
            DependencyProperty.Register("ImagePath", typeof(string), typeof(BaseQuestion), new PropertyMetadata(null));

        public static readonly DependencyProperty NumberProperty =
            DependencyProperty.Register("Number", typeof(int), typeof(BaseQuestion), new PropertyMetadata(0));

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected BaseQuestion()
        {
            Number = ++questionCounter;
            DataContext = this;
        }
        protected void ShowFullImage(object sender, RoutedEventArgs e, StackPanel answersPanel, string questionText)
        {
            FullImageForm fullImageForm = new FullImageForm(answersPanel, questionText, ImageSource);
            fullImageForm.Owner = Window.GetWindow(this);
            fullImageForm.ShowDialog();
        }

        public static void ShowFullImage(string imagePath)
        {
            if (string.IsNullOrEmpty(imagePath) || !File.Exists(imagePath))
                return; // Проверяем, что путь не пустой и файл существует

            // Создаём окно для просмотра изображения
            Window fullImageWindow = new Window
            {
                Title = "Просмотр изображения",
                WindowState = WindowState.Maximized, // Открыть в полноэкранном режиме
                Background = Brushes.Black,
                WindowStyle = WindowStyle.None, // Убираем границы и кнопки окна
                AllowsTransparency = true
            };

            MessageBox.Show("ShowFullImage" + ("file:///" + Path.Combine(Directory.GetCurrentDirectory(), imagePath).Replace("\\", "/")));
            // Загружаем изображение из файла
            Image image = new Image
            {
                Source = !string.IsNullOrEmpty(imagePath)
            ? new BitmapImage(new Uri("file:///" + Path.Combine(Directory.GetCurrentDirectory(), imagePath).Replace("\\", "/")))
            : null,
                Stretch = Stretch.Uniform, // Чтобы картинка не искажалась
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };

            // Закрытие окна по клику на картинку
            image.MouseDown += (s, args) => fullImageWindow.Close();

            fullImageWindow.Content = image;
            fullImageWindow.ShowDialog(); // Открываем окно
        }

        // Метод удаления вопроса
        protected void DeleteBlock(object sender, RoutedEventArgs e)
        {
            ParentStackPanel?.Children.Remove(this);
            RenumberQuestions(ParentStackPanel);
        }
        protected void CopyBlock(object sender, RoutedEventArgs e)
        {
            if (ParentStackPanel == null)
                return;

            SingleQuestion single = this as SingleQuestion;
            MultipleQuestion multiple = this as MultipleQuestion;
            BaseQuestion original = single as BaseQuestion ?? multiple as BaseQuestion;
            BaseQuestion clone;
            string imagePath = "";
            if (original.question == null)
            {
                string questionText = "";
                if (single != null)
                {
                    questionText = single.questionTextBox?.Text?.Trim();
                    imagePath = single.ImagePath;
                }
                else
                {
                    questionText = multiple.questionTextBox?.Text?.Trim();
                    imagePath = multiple.ImagePath;
                }
                if (string.IsNullOrWhiteSpace(questionText))
                {
                    MessageBox.Show("Текст вопроса не может быть пустым.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }


                // 2. Создаем объект вопроса
                Questions newQuestion = new Questions
                {
                    QuestionText = questionText,
                    Image = imagePath ?? "", // если есть изображение
                    Answers = new List<Answers>()
                };

                // 3. Получаем ответы
                var children = single != null ? single.AnswersPanel.Children : multiple.AnswersPanel.Children;


                foreach (var child in children)
                {
                    if (child is StackPanel answerPanel)
                    {
                        RadioButton radioButton = answerPanel.Children.OfType<RadioButton>().FirstOrDefault();
                        CheckBox checkBox = answerPanel.Children.OfType<CheckBox>().FirstOrDefault();
                        TextBox answerTextBox = answerPanel.Children.OfType<TextBox>().FirstOrDefault();

                        if (answerTextBox != null && !string.IsNullOrWhiteSpace(answerTextBox.Text))
                        {
                            // Определяем, является ли ответ правильным
                            bool isCorrect = false;

                            // Проверяем, если существует RadioButton или CheckBox, то выбираем правильное состояние
                            if (radioButton != null)
                            {
                                isCorrect = radioButton.IsChecked == true;  // Если радио-кнопка выбрана, ответ правильный
                            }
                            else if (checkBox != null)
                            {
                                isCorrect = checkBox.IsChecked == true;  // Если чекбокс выбран, ответ правильный
                            }

                            // Создаем и добавляем ответ
                            Answers answer = new Answers
                            {
                                AnswerText = answerTextBox.Text.Trim(),
                                IsCorrect = isCorrect
                            };

                            newQuestion.Answers.Add(answer);
                        }
                    }
                }

                // 4. Устанавливаем IsMultiple (если более одного правильного ответа)
                newQuestion.IsMultiple = newQuestion.Answers.Count(a => a.IsCorrect) > 1;
                original.question = newQuestion;
                original.ImagePath = imagePath;
            }
            

            if(!original.question.IsMultiple)
                if(original.ImageSource != null && original.ImageSource.ToString() != "pack://application:,,,/LoadImage.png")
                    clone = new SingleQuestion(single._databaseHelper, original.ShowImage, CloneQuestion(original.question), false, GetFilePathFromUri(original.ImageSource.ToString()), true);
                else if (original.ImagePath != null)
                    clone = new SingleQuestion(single._databaseHelper, original.ShowImage, CloneQuestion(original.question), false, original.ImagePath, true);
                else
                    clone = new SingleQuestion(single._databaseHelper, original.ShowImage, CloneQuestion(original.question), false, "", true);
            else if (original.ImageSource != null && original.ImageSource.ToString() != "pack://application:,,,/LoadImage.png")
                    clone = new MultipleQuestion(multiple._databaseHelper, original.ShowImage, CloneQuestion(original.question), false, GetFilePathFromUri(original.ImageSource.ToString()), true);
                else if (original.ImagePath != null)
                    clone = new MultipleQuestion(multiple._databaseHelper, original.ShowImage, CloneQuestion(original.question), false, original.ImagePath, true);
                else
                    clone = new MultipleQuestion(multiple._databaseHelper, original.ShowImage, CloneQuestion(original.question), false, "", true);


            if (clone.ShowImage)
                clone.ImagePath = ImagePath;
            int index = ParentStackPanel.Children.Count;
            ParentStackPanel.Children.Insert(index, clone);
            var stackPanel = FindParent<StackPanel>(clone);
            clone.ParentStackPanel = stackPanel;
            RenumberQuestions(ParentStackPanel);
        }
        public string GetFilePathFromUri(string uriString)
        {
            return new Uri(uriString).LocalPath.Replace('/', '\\');
        }

        public static T FindParent<T>(DependencyObject child) where T : DependencyObject
        {
            DependencyObject parentObject = VisualTreeHelper.GetParent(child);

            while (parentObject != null)
            {
                if (parentObject is T parent)
                    return parent;

                parentObject = VisualTreeHelper.GetParent(parentObject);
            }

            return null;
        }

        private Questions CloneQuestion(Questions original)
        {
            var clonedAnswers = original.Answers.Select(a =>
                new Answers(0, 0, a.AnswerText, a.IsCorrect)
            ).ToList();

            return new Questions
            {
                Id = 0, // Чтобы не было коллизии с БД
                QuestionText = original.QuestionText,
                Answers = clonedAnswers,
                Image = original.Image
            };
        }

        // Метод для перенумерации вопросов
        public static void RenumberQuestions(StackPanel panel)//StackPanel panel
        {
            questionCounter = 0;
            foreach (var child in panel.Children.OfType<BaseQuestion>())
            {
                child.Number = ++questionCounter;
            }
        }
        public static void ResetQuestionCounter()
        {
            questionCounter = 0;
        }
        public string SaveImage()
        {
            try
            {
                // Проверка, если путь изображения правильный
                if (string.IsNullOrEmpty(this.ImagePath) || !File.Exists(this.ImagePath))
                {
                    throw new Exception("Изображение не найдено." + this.ImagePath);
                }

                // Путь для сохранения изображения
                string directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "Images");

                // Проверяем, существует ли папка "Images", если нет - создаём
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                // Получаем имя файла из исходного пути
                string fileName = Path.GetFileName(this.ImagePath);

                // Полный путь к новому файлу
                string filePath = Path.Combine(directoryPath, fileName);

                // Если файл с таким именем уже существует, создаем уникальное имя
                int count = 1;
                while (File.Exists(filePath))
                {
                    filePath = Path.Combine(directoryPath, $"{Path.GetFileNameWithoutExtension(fileName)}_{count++}{Path.GetExtension(fileName)}");
                }

                // Копируем файл изображения в новый каталог
                File.Copy(this.ImagePath, filePath);

                // Загружаем изображение в ImageSource (если необходимо для отображения)
                this.ImageSource = new BitmapImage(new Uri(filePath));

                // Для возврата относительного пути, используя базовую директорию приложения (относительно директории с исполнимым файлом)
                string relativePath = Path.Combine("Images", fileName);
                this.ImagePath = relativePath;  // Обновляем ImagePath, чтобы использовать относительный путь

                return relativePath; // Возвращаем относительный путь
            }
            catch (Exception ex)
            {
                // Обработка ошибок
                MessageBox.Show("Ошибка при сохранении изображения: " + ex.Message);
                return null;
            }
        }
        protected void TextBox_SelectAll(object sender, MouseButtonEventArgs e)
        {
            (sender as TextBox)?.SelectAll();
        }
        protected void TextBox_SelectAll(object sender, MouseButtonEventArgs e, string text)
        {
            if(text == "Введите ответ")
            (sender as TextBox)?.SelectAll();
        }
        // Абстрактный метод для добавления ответа (реализуется в наследниках)
        protected abstract void AddAnswer(object sender = null, RoutedEventArgs e = null);

        // Абстрактный метод для обновления ошибок
        protected abstract void UpdateErrorMessages();

        public abstract void SetError(string message);
        public abstract void ClearError();



    }
}



