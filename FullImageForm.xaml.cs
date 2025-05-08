using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Shapes;

namespace wpf_тесты_для_обучения
{
    /// <summary>
    /// Логика взаимодействия для FullImageForm.xaml
    /// </summary>
    public partial class FullImageForm : Window, INotifyPropertyChanged
    {

        private Panel originalParent;
        private Panel originalAnswersPanel;

        private BitmapImage _imageSource;
        public BitmapImage ImageSource
        {
            get => _imageSource;
            set
            {
                _imageSource = value;
                OnPropertyChanged(nameof(ImageSource));
            }
        }
      

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public FullImageForm()
        {
            InitializeComponent();
        }
        public FullImageForm(StackPanel originalAnswersPanel, string question, BitmapImage imageSource)
        {
            InitializeComponent();
            DataContext = this;
            ImageSource = imageSource;
            questionTextBlock.Text = question;

            // Сохраняем родителя
            originalParent = originalAnswersPanel.Parent as Panel;

            if (originalParent != null)
            {
                originalParent.Children.Remove(originalAnswersPanel);
            }

            // Добавляем в новое место
            answersPanel.Children.Add(originalAnswersPanel);
            this.originalAnswersPanel = originalAnswersPanel;
            foreach (StackPanel panel in answersPanel.Children)
                foreach (Grid grid in panel.Children)
                {
                    // Если элемент имеет имя
                    foreach (var element in grid.Children)
                    {
                        if (element is RadioButton radioButton)
                            radioButton.FontSize = 18;
                        if (element is CheckBox checkBox)
                            checkBox.FontSize = 18; 
                        if (element is TextBlock textBlock)
                            textBlock.FontSize = 18; 
                    }
                    panel.Margin = new Thickness(8, 0, 0, 0);
                }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if (originalParent != null && originalAnswersPanel != null)
            {
                foreach (StackPanel panel in answersPanel.Children)
                    foreach (Grid grid in panel.Children)
                    {
                        // Если элемент имеет имя
                        foreach (var element in grid.Children)
                        {
                            if (element is RadioButton radioButton)
                                radioButton.FontSize = 15;
                            if (element is CheckBox checkBox)
                                checkBox.FontSize = 15;
                            if (element is TextBlock textBlock)
                                textBlock.FontSize = 15;
                        }
                    }
                answersPanel.Children.Remove(originalAnswersPanel);
                originalParent.Children.Add(originalAnswersPanel);
            }
       
        }
}
}
