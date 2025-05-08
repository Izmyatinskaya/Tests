using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace wpf_тесты_для_обучения
{
    /// <summary>
    /// Логика взаимодействия для Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            RestyleButton(sender);
        }

        private void RestyleButton(object sender)
        {
            // Получаем нажатую кнопку
            var clickedButton = sender as Button;
            if (clickedButton == null) return;

            // Если у кнопки уже установлен стиль ClickMenuButtonStyle, ничего не делаем
            if (clickedButton.Style == (Style)FindResource("ClickMenuButtonStyle"))
            {
                clickedButton.Style = (Style)FindResource("MenuButtonStyle");
                return;
            }

            // Находим все кнопки в StackPanel
            var stackPanel = clickedButton.Parent as StackPanel;
            if (stackPanel == null) return;

            // Проходим по всем кнопкам и сбрасываем их стили
            foreach (var child in stackPanel.Children)
            {
                if (child is Button button)
                {
                    // Если это не нажатая кнопка и у неё стиль ClickMenuButtonStyle
                    if (button != clickedButton && button.Style == (Style)FindResource("ClickMenuButtonStyle"))
                    {
                        button.Style = (Style)FindResource("MenuButtonStyle");
                    }
                }
            }

            // Устанавливаем новый стиль для нажатой кнопки
            clickedButton.Style = (Style)FindResource("ClickMenuButtonStyle");
        }
        private void AnimateButtonStyleChange(Button button, Style newStyle)
        {
            // Создаем анимацию перехода
            ColorAnimation animation = new ColorAnimation
            {
                Duration = TimeSpan.FromSeconds(0.3),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
            };

            Storyboard storyboard = new Storyboard();
            storyboard.Children.Add(animation);

            // Применяем анимацию к текущему фону
            var currentBackground = (SolidColorBrush)button.Background;
            animation.From = currentBackground.Color;

            // Получаем цвет нового стиля
            var newBackground = (SolidColorBrush)newStyle.Setters
                .OfType<Setter>()
                .First(s => s.Property == Control.BackgroundProperty)
                .Value;
            animation.To = newBackground.Color;

            // Запускаем анимацию
            Storyboard.SetTarget(animation, button);
            Storyboard.SetTargetProperty(animation, new PropertyPath("Background.Color"));
            storyboard.Begin();

            // Устанавливаем новый стиль после завершения анимации
            storyboard.Completed += (s, e) =>
            {
                button.Style = newStyle;
            };
        }

    }
}
