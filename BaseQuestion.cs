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

namespace wpf_тесты_для_обучения
{

    public abstract class BaseQuestion : UserControl, INotifyPropertyChanged
    {
        private static int questionCounter = 0; // Общий счетчик вопросов
        public Questions question { get; set; } //
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
                if (_imageSource == null)
                    _imageSource = value ?? new BitmapImage(new Uri("pack://application:,,,/images/LoadImage.png"));
                else _imageSource = value;                    
                

                OnPropertyChanged(nameof(ImageSource));
            }
        }

        /* public BitmapImage ImageSource
         {
             get { return (BitmapImage)GetValue(ImageSourceProperty); }
             set {
                 if (value == null)
                 {
                     ImageSource = new BitmapImage(new Uri("D:\\Проекты\\Тесты обучение WPF\\wpf тесты для обучения\\images\\LoadImage.png"));
                     SetValue(ImageSourceProperty, ImageSource);
                 }
                 SetValue(ImageSourceProperty, value);
                 OnPropertyChanged(nameof(ImageSource));

             }
         }*/
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

        //public static readonly DependencyProperty ImageSourceProperty =
        //    DependencyProperty.Register("ImageSource", typeof(BitmapImage), typeof(BaseQuestion), new PropertyMetadata(null));

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

        protected void ShowFullImage(object sender, RoutedEventArgs e)
        {
            if (ImageSource == null) return; // Если картинки нет, просто выходим

            Window fullImageWindow = new Window
            {
                Title = "Просмотр изображения",
                WindowState = WindowState.Maximized, // Открыть в полноэкранном режиме
                Background = Brushes.Black,
                WindowStyle = WindowStyle.None, // Убираем границы и кнопки окна
                AllowsTransparency = true
            };

            Image image = new Image
            {
                Source = ImageSource,
                Stretch = Stretch.Uniform, // Чтобы картинка не искажалась
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };

            // Закрытие окна по клику на картинку
            image.MouseDown += (s, args) => fullImageWindow.Close();

            fullImageWindow.Content = image;
            fullImageWindow.ShowDialog(); // Открываем окно
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

        // Метод для перенумерации вопросов
        public static void RenumberQuestions(StackPanel panel)
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
                    throw new Exception("Изображение не найдено.");
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

        // Абстрактный метод для добавления ответа (реализуется в наследниках)
        protected abstract void AddAnswer(object sender = null, RoutedEventArgs e = null);

        // Абстрактный метод для обновления ошибок
        protected abstract void UpdateErrorMessages();
        
    }
}



