//using Microsoft.Data.Sqlite;
//using System;
//using System.Collections.Generic;
//using System.Data.SqlClient;
//using System.Data;
//using System.Windows.Markup;
//// using static Microsoft.Data.Sqlite.SqliteCommand; // Эти статические using могут вызывать конфликты, лучше использовать полные имена
//// using static Microsoft.Data.Sqlite.SqliteConnection; // Эти статические using могут вызывать конфликты, лучше использовать полные имена
//using System.Windows.Controls.Primitives;
//using System.IO;
//using static System.Net.Mime.MediaTypeNames;
//using Microsoft.Win32;
//using System.Windows; // Для System.Windows.Window, System.Windows.Visibility и других общих WPF классов
//using System.Data.Common;
//using System.Linq;
//using System.Text;
//// using Microsoft.Office.Interop.Excel; // Удалить или использовать псевдоним, если он действительно нужен, но вызывает конфликты
//using DataTable = System.Data.DataTable;
//using System.Windows.Controls; // Для System.Windows.Controls.CheckBox, System.Windows.Controls.TextBox, System.Windows.Controls.RadioButton
//using System.Windows.Media.Imaging;
//using System.Windows.Media;
//using System.ComponentModel;
//using System.Windows.Input;
//using Image = System.Windows.Controls.Image; // Псевдоним для избежания конфликтов с System.Drawing.Image

//namespace wpf_тесты_для_обучения
//{

//    public abstract class BaseQuestion : UserControl, INotifyPropertyChanged
//    {
//        private static int questionCounter = 0; // Общий счетчик вопросов
//        public Questions question { get; set; } //

//        public StackPanel ParentStackPanel { get; set; }
//        public static bool ZeroErrors { get; set; }

//        public string ImagePath
//        {
//            get { return (string)GetValue(ImagePathProperty); }
//            set { SetValue(ImagePathProperty, value); }
//        }

//        public int Number
//        {
//            get { return (int)GetValue(NumberProperty); }
//            set { SetValue(NumberProperty, value); }
//        }

//        // ImageSource теперь является DependencyProperty, которое обновляется на основе ImagePath
//        public BitmapImage ImageSource
//        {
//            get { return (BitmapImage)GetValue(ImageSourceProperty); }
//            set { SetValue(ImageSourceProperty, value); }
//        }

//        public bool ShowImage
//        {
//            get { return (bool)GetValue(ShowImageProperty); }
//            set { SetValue(ShowImageProperty, value); }
//        }

//        public static readonly DependencyProperty ShowImageProperty =
//            DependencyProperty.Register(nameof(ShowImage), typeof(bool), typeof(BaseQuestion), new PropertyMetadata(true));

//        public static readonly DependencyProperty ImagePathProperty =
//            DependencyProperty.Register(nameof(ImagePath), typeof(string), typeof(BaseQuestion), new PropertyMetadata(null, OnImagePathChanged));

//        public static readonly DependencyProperty ImageSourceProperty =
//            DependencyProperty.Register(nameof(ImageSource), typeof(BitmapImage), typeof(BaseQuestion), new PropertyMetadata(null));

//        public static readonly DependencyProperty NumberProperty =
//            DependencyProperty.Register(nameof(Number), typeof(int), typeof(BaseQuestion), new PropertyMetadata(0));

//        public event PropertyChangedEventHandler PropertyChanged;
//        protected void OnPropertyChanged(string propertyName)
//        {
//            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
//        }

//        protected BaseQuestion()
//        {
//            Number = ++questionCounter;
//            DataContext = this;
//        }

//        private static void OnImagePathChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
//        {
//            if (d is BaseQuestion baseQuestion)
//            {
//                baseQuestion.ImageSource = baseQuestion.LoadImageFromPath(e.NewValue as string);
//            }
//        }

//        private BitmapImage LoadImageFromPath(string imagePath)
//        {
//            // Исправление CS1501: Используем ToLowerInvariant() для сравнения без учета регистра, если не поддерживается Contains с StringComparison
//            if (string.IsNullOrEmpty(imagePath) || imagePath.ToLowerInvariant().Contains("LoadImage.png".ToLowerInvariant()))
//            {
//                // Возвращаем null для отсутствующего изображения или плейсхолдера
//                return null;
//            }

//            try
//            {
//                string fullPath = imagePath;
//                if (!Path.IsPathRooted(imagePath))
//                {
//                    // Предполагаем, что относительный путь является относительным к базовому каталогу приложения
//                    fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, imagePath);
//                }

//                if (!File.Exists(fullPath))
//                {
//                    // Опционально логируем или показываем предупреждение, если файл не существует по ожидаемому пути
//                    return null;
//                }

//                BitmapImage bitmap = new BitmapImage();
//                bitmap.BeginInit();
//                bitmap.UriSource = new Uri(fullPath, UriKind.Absolute);
//                bitmap.CacheOption = BitmapCacheOption.OnLoad; // Кэшируем данные изображения
//                bitmap.EndInit();
//                bitmap.Freeze(); // Делаем его доступным для других потоков
//                return bitmap;
//            }
//            catch (Exception ex)
//            {
//                System.Windows.MessageBox.Show($"Ошибка загрузки изображения: {ex.Message}", "Ошибка загрузки", MessageBoxButton.OK, MessageBoxImage.Error);
//                return null; // Возвращаем null при ошибке
//            }
//        }

//        protected void ShowFullImage(object sender, RoutedEventArgs e, StackPanel answersPanel, string questionText)
//        {
//            FullImageForm fullImageForm = new FullImageForm(answersPanel, questionText, ImageSource);
//            fullImageForm.Owner = System.Windows.Window.GetWindow(this); // Явно указываем System.Windows.Window
//            fullImageForm.ShowDialog();
//        }

//        public static void ShowFullImage(string imagePath)
//        {
//            // Исправление CS1501: Используем ToLowerInvariant() для сравнения без учета регистра
//            if (string.IsNullOrEmpty(imagePath) || imagePath.ToLowerInvariant().Contains("LoadImage.png".ToLowerInvariant()))
//                return; // Проверяем, что путь не пустой и не является плейсхолдером

//            // Создаем полный путь относительно базового каталога приложения для согласованности
//            string fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, imagePath);

//            if (!File.Exists(fullPath))
//            {
//                System.Windows.MessageBox.Show($"Изображение не найдено по пути: {fullPath}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
//                return;
//            }

//            System.Windows.Window fullImageWindow = new System.Windows.Window // Явно указываем System.Windows.Window
//            {
//                Title = "Просмотр изображения",
//                WindowState = WindowState.Maximized, // Открыть в полноэкранном режиме
//                Background = Brushes.Black,
//                WindowStyle = System.Windows.WindowStyle.None, // Убираем границы и кнопки окна
//                AllowsTransparency = true
//            };

//            // Загружаем изображение из полного пути
//            Image image = new Image
//            {
//                Source = new BitmapImage(new Uri(fullPath, UriKind.Absolute)),
//                Stretch = Stretch.Uniform, // Чтобы картинка не искажалась
//                HorizontalAlignment = HorizontalAlignment.Center,
//                VerticalAlignment = VerticalAlignment.Center
//            };

//            // Закрытие окна по клику на картинку
//            image.MouseDown += (s, args) => fullImageWindow.Close();

//            fullImageWindow.Content = image;
//            fullImageWindow.ShowDialog(); // Открываем окно
//        }

//        // Метод удаления вопроса
//        protected void DeleteBlock(object sender, RoutedEventArgs e)
//        {
//            ParentStackPanel?.Children.Remove(this);
//            RenumberQuestions(ParentStackPanel);
//        }

//        protected void CopyBlock(object sender, RoutedEventArgs e)
//        {
//            if (ParentStackPanel == null)
//                return;

//            SingleQuestion single = this as SingleQuestion;
//            MultipleQuestion multiple = this as MultipleQuestion;
//            BaseQuestion original = single as BaseQuestion ?? multiple as BaseQuestion;

//            // Если объект вопроса еще не заполнен, собираем данные из UI
//            if (original.question == null)
//            {
//                string questionText = "";
//                if (single != null)
//                {
//                    questionText = single.questionTextBox?.Text?.Trim();
//                }
//                else
//                {
//                    questionText = multiple.questionTextBox?.Text?.Trim();
//                }
//                if (string.IsNullOrWhiteSpace(questionText))
//                {
//                    System.Windows.MessageBox.Show("Текст вопроса не может быть пустым.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
//                    return;
//                }

//                // Создаем новый объект вопроса
//                Questions newQuestion = new Questions
//                {
//                    QuestionText = questionText,
//                    Image = original.ImagePath ?? "", // Используем ImagePath (который должен быть относительным путем после сохранения)
//                    Answers = new List<Answers>()
//                };

//                // Получаем ответы
//                var children = single != null ? single.AnswersPanel.Children : multiple.AnswersPanel.Children;

//                foreach (var child in children)
//                {
//                    if (child is StackPanel answerPanel)
//                    {
//                        System.Windows.Controls.RadioButton radioButton = answerPanel.Children.OfType<System.Windows.Controls.RadioButton>().FirstOrDefault();
//                        System.Windows.Controls.CheckBox checkBox = answerPanel.Children.OfType<System.Windows.Controls.CheckBox>().FirstOrDefault(); // Явно указываем System.Windows.Controls.CheckBox
//                        System.Windows.Controls.TextBox answerTextBox = answerPanel.Children.OfType<System.Windows.Controls.TextBox>().FirstOrDefault(); // Явно указываем System.Windows.Controls.TextBox

//                        if (answerTextBox != null && !string.IsNullOrWhiteSpace(answerTextBox.Text))
//                        {
//                            bool isCorrect = false;
//                            if (radioButton != null)
//                            {
//                                isCorrect = radioButton.IsChecked == true;
//                            }
//                            else if (checkBox != null)
//                            {
//                                isCorrect = checkBox.IsChecked == true;
//                            }

//                            Answers answer = new Answers
//                            {
//                                AnswerText = answerTextBox.Text.Trim(),
//                                IsCorrect = isCorrect
//                            };
//                            newQuestion.Answers.Add(answer);
//                        }
//                    }
//                }
//                newQuestion.IsMultiple = newQuestion.Answers.Count(a => a.IsCorrect) > 1;
//                original.question = newQuestion;
//            }

//            // Определяем путь к изображению для клона, который должен быть относительным путем из исходного объекта вопроса
//            string imagePathForClone = original.question?.Image ?? "";

//            BaseQuestion clone;
//            if (!original.question.IsMultiple)
//            {
//                clone = new SingleQuestion(single._databaseHelper, original.ShowImage, CloneQuestion(original.question), false, imagePathForClone, true);
//            }
//            else
//            {
//                clone = new MultipleQuestion(multiple._databaseHelper, original.ShowImage, CloneQuestion(original.question), false, imagePathForClone, true);
//            }

//            // Свойство ImagePath клона будет установлено в его конструкторе, что затем вызовет OnImagePathChanged.
//            // Не нужно повторно устанавливать clone.ImagePath здесь из ImagePath оригинала, так как это обрабатывается конструктором.
//            // Удалено: if (clone.ShowImage) clone.ImagePath = ImagePath;

//            int index = ParentStackPanel.Children.IndexOf(this) + 1; // Вставляем после текущего вопроса
//            ParentStackPanel.Children.Insert(index, clone);
//            var stackPanel = FindParent<StackPanel>(clone);
//            clone.ParentStackPanel = stackPanel;
//            RenumberQuestions(ParentStackPanel);
//        }

//        public string GetFilePathFromUri(string uriString)
//        {
//            Uri uri = new Uri(uriString);
//            if (uri.IsFile)
//            {
//                return uri.LocalPath.Replace('/', '\\');
//            }
//            return null; // Для URI, не являющихся файлами, возвращаем null, указывая на отсутствие прямого пути к файлу.
//        }

//        public static T FindParent<T>(DependencyObject child) where T : DependencyObject
//        {
//            DependencyObject parentObject = VisualTreeHelper.GetParent(child);

//            while (parentObject != null)
//            {
//                if (parentObject is T parent)
//                    return parent;

//                parentObject = VisualTreeHelper.GetParent(parentObject);
//            }
//            return null;
//        }

//        private Questions CloneQuestion(Questions original)
//        {
//            var clonedAnswers = original.Answers.Select(a =>
//                new Answers(0, 0, a.AnswerText, a.IsCorrect)
//            ).ToList();

//            return new Questions
//            {
//                Id = 0, // Чтобы не было коллизии с БД
//                QuestionText = original.QuestionText,
//                Answers = clonedAnswers,
//                Image = original.Image // Путь к изображению копируется напрямую
//            };
//        }

//        // Метод для перенумерации вопросов
//        public static void RenumberQuestions(StackPanel panel)
//        {
//            questionCounter = 0;
//            foreach (var child in panel.Children.OfType<BaseQuestion>())
//            {
//                child.Number = ++questionCounter;
//            }
//        }

//        public static void ResetQuestionCounter()
//        {
//            questionCounter = 0;
//        }

//        public string SaveImage()
//        {
//            // Исправление CS1501: Используем ToLowerInvariant() для сравнения без учета регистра
//            if (string.IsNullOrEmpty(this.ImagePath) || this.ImagePath.ToLowerInvariant().Contains("LoadImage.png".ToLowerInvariant()))
//            {
//                return ""; // Возвращаем пустую строку, чтобы указать, что изображение не сохранено
//            }

//            string sourcePath;
//            // Если ImagePath уже является относительным путем, указывающим на папку "Images", предполагаем, что он уже сохранен.
//            if (this.ImagePath.StartsWith("Images\\", StringComparison.OrdinalIgnoreCase) ||
//                this.ImagePath.StartsWith("Images/", StringComparison.OrdinalIgnoreCase))
//            {
//                sourcePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, this.ImagePath);
//                if (File.Exists(sourcePath))
//                {
//                    return this.ImagePath; // Уже сохранено, возвращаем существующий относительный путь.
//                }
//            }

//            // Если это не существующий относительный путь, предполагаем, что это новый файл, выбранный из диалогового окна.
//            sourcePath = this.ImagePath;
//            if (!Path.IsPathRooted(sourcePath))
//            {
//                // Этот случай в идеале не должен происходить, если ImagePath установлен из OpenFileDialog,
//                // но обрабатываем его защитно, пытаясь использовать текущий каталог.
//                sourcePath = Path.Combine(Directory.GetCurrentDirectory(), sourcePath);
//            }

//            if (!File.Exists(sourcePath))
//            {
//                System.Windows.MessageBox.Show($"Ошибка: Исходное изображение не найдено по пути: {sourcePath}", "Ошибка сохранения", MessageBoxButton.OK, MessageBoxImage.Error);
//                return null;
//            }

//            try
//            {
//                string directoryPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images");
//                if (!Directory.Exists(directoryPath))
//                {
//                    Directory.CreateDirectory(directoryPath);
//                }

//                string fileName = Path.GetFileName(sourcePath);
//                string originalFileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
//                string fileExtension = Path.GetExtension(fileName);
//                string targetFilePath = Path.Combine(directoryPath, fileName);
//                string relativeFilePath = Path.Combine("Images", fileName); // Путь для сохранения и возврата

//                int count = 1;
//                string tempFileName = fileName;
//                string tempTargetFilePath = targetFilePath;
//                string tempRelativeFilePath = relativeFilePath;

//                // Проверяем, существует ли файл в целевом каталоге и НЕ является ли он тем же файлом, что и исходный.
//                // Если это тот же файл, копировать или переименовывать не нужно.
//                while (File.Exists(tempTargetFilePath) &&
//                       !Path.GetFullPath(sourcePath).Equals(Path.GetFullPath(tempTargetFilePath), StringComparison.OrdinalIgnoreCase))
//                {
//                    tempFileName = $"{originalFileNameWithoutExtension}_{count++}{fileExtension}";
//                    tempTargetFilePath = Path.Combine(directoryPath, tempFileName);
//                    tempRelativeFilePath = Path.Combine("Images", tempFileName);
//                }

//                // Копируем файл, только если исходный файл отличается от целевого файла или если целевой файл не существует.
//                if (!File.Exists(tempTargetFilePath) ||
//                    !Path.GetFullPath(sourcePath).Equals(Path.GetFullPath(tempTargetFilePath), StringComparison.OrdinalIgnoreCase))
//                {
//                    File.Copy(sourcePath, tempTargetFilePath, true); // Перезаписываем, если цель существует и отличается
//                }

//                // Обновляем свойство ImagePath до нового относительного пути, который фактически использовался.
//                // Это вызовет OnImagePathChanged для обновления ImageSource.
//                this.ImagePath = tempRelativeFilePath;

//                return tempRelativeFilePath; // Возвращаем относительный путь для сохранения в базе данных
//            }
//            catch (Exception ex)
//            {
//                System.Windows.MessageBox.Show("Ошибка при сохранении изображения: " + ex.Message, "Ошибка сохранения", MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
//                return null;
//            }
//        }

//        protected void TextBox_SelectAll(object sender, MouseButtonEventArgs e)
//        {
//            (sender as System.Windows.Controls.TextBox)?.SelectAll(); // Явно указываем System.Windows.Controls.TextBox
//        }

//        protected void TextBox_SelectAll(object sender, MouseButtonEventArgs e, string text)
//        {
//            if (text == "Введите ответ")
//                (sender as System.Windows.Controls.TextBox)?.SelectAll(); // Явно указываем System.Windows.Controls.TextBox
//        }

//        protected void TextBox_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
//        {
//            if (sender is System.Windows.Controls.TextBox textBox) // Явно указываем System.Windows.Controls.TextBox
//            {
//                if (textBox.Text == "Введите ответ")
//                    (sender as System.Windows.Controls.TextBox)?.SelectAll(); // Явно указываем System.Windows.Controls.TextBox

//                if (textBox.Text == "Введите вопрос?")
//                    (sender as System.Windows.Controls.TextBox)?.SelectAll(); // Явно указываем System.Windows.Controls.TextBox
//            }
//        }

//        protected void TextBox_TextChanged(object sender, TextChangedEventArgs e)
//        {
//            if (sender is System.Windows.Controls.TextBox textBox) // Явно указываем System.Windows.Controls.TextBox
//            {
//                textBox.Foreground = string.IsNullOrEmpty(textBox.Text) ||
//                                     textBox.Text == "Введите вопрос?" ||
//                                     textBox.Text == "Введите ответ"
//                                     ? Brushes.Gray
//                                     : Brushes.Black;
//            }
//        }

//        // Абстрактный метод для добавления ответа (реализуется в наследниках)
//        protected abstract void AddAnswer(object sender = null, RoutedEventArgs e = null);

//        // Абстрактный метод для обновления ошибок
//        protected abstract void UpdateErrorMessages();

//        public abstract void SetError(string message);
//        public abstract void ClearError();
//    }
//}

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
using static System.Net.Mime.MediaTypeNames;
using Image = System.Windows.Controls.Image;

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
            set { SetValue(ShowImageProperty, value); }
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


            if (!original.question.IsMultiple)
                if (original.ImageSource != null && original.ImageSource.ToString() != "pack://application:,,,/LoadImage.png")
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
        //public string SaveImage()
        //{
        //    try
        //    {
        //        // Проверка, если путь изображения правильный
        //        if (string.IsNullOrEmpty(this.ImagePath) || !File.Exists(this.ImagePath))
        //        {
        //            throw new Exception("Изображение не найдено." + this.ImagePath);
        //        }

        //        // Путь для сохранения изображения в папке Images рядом с исполняемым файлом
        //        string directoryPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images"); // Использовать AppDomain.CurrentDomain.BaseDirectory

        //        // Проверяем, существует ли папка "Images", если нет - создаём
        //        if (!Directory.Exists(directoryPath))
        //        {
        //            Directory.CreateDirectory(directoryPath);
        //        }

        //        // Получаем имя файла из исходного пути
        //        string fileName = Path.GetFileName(this.ImagePath);

        //        // Полный путь к новому файлу
        //        string filePath = Path.Combine(directoryPath, fileName);

        //        // Если файл с таким именем уже существует, создаем уникальное имя
        //        int count = 1;
        //        string originalFileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
        //        string fileExtension = Path.GetExtension(fileName);

        //        while (File.Exists(filePath))
        //        {
        //            fileName = $"{originalFileNameWithoutExtension}_{count++}{fileExtension}";
        //            filePath = Path.Combine(directoryPath, fileName);
        //        }

        //        // Копируем файл изображения в новый каталог
        //        File.Copy(this.ImagePath, filePath);

        //        // Загружаем изображение в ImageSource (если необходимо для отображения)
        //        // Если вы загружаете из filePath, это должен быть абсолютный путь для BitmapImage
        //        this.ImageSource = new BitmapImage(new Uri(filePath));

        //        // ВАЖНО: Возвращаем только имя файла для сохранения в БД
        //        // ImagePath лучше не обновлять здесь, если ImagePath - это текущий путь для выбора/отображения
        //        // this.ImagePath = relativePath; // Закомментировать или удалить, если ImagePath используется для UI-связки

        //        return fileName; // Возвращаем ТОЛЬКО имя файла
        //    }
        //    catch (Exception ex)
        //    {
        //        // Обработка ошибок
        //        MessageBox.Show("Ошибка при сохранении изображения: " + ex.Message);
        //        return null;
        //    }
        //}
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
                if (!File.Exists(filePath))
                {
                    File.Copy(this.ImagePath, filePath);
                }
                //int count = 1;
                //while (File.Exists(filePath))
                //{
                //    filePath = Path.Combine(directoryPath, $"{Path.GetFileNameWithoutExtension(fileName)}_{count++}{Path.GetExtension(fileName)}");
                //}

                //// Копируем файл изображения в новый каталог
                //File.Copy(this.ImagePath, filePath);

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
            if (text == "Введите ответ")
                (sender as TextBox)?.SelectAll();
        }

        protected void TextBox_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                if (textBox.Text == "Введите ответ")
                    (sender as TextBox)?.SelectAll();

                if (textBox.Text == "Введите вопрос?")
                    (sender as TextBox)?.SelectAll();
            }
        }

        protected void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                textBox.Foreground = string.IsNullOrEmpty(textBox.Text) ||
                                   textBox.Text == "Введите вопрос?" ||
                                   textBox.Text == "Введите ответ"
                                   ? Brushes.Gray
                                   : Brushes.Black;
            }
        }

        // Абстрактный метод для добавления ответа (реализуется в наследниках)
        protected abstract void AddAnswer(object sender = null, RoutedEventArgs e = null);

        // Абстрактный метод для обновления ошибок
        protected abstract void UpdateErrorMessages();

        public abstract void SetError(string message);
        public abstract void ClearError();



    }
}



