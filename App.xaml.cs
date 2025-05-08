using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace wpf_тесты_для_обучения
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button closeButton && closeButton.TemplatedParent is Button parentButton)
            {
                if (parentButton.Tag is Panel panel && panel.Children.Contains(parentButton))
                {
                    panel.Children.Remove(parentButton);
                }
            }
        }
    }

    public static class ButtonProperties
    {
        public static readonly DependencyProperty ShowIconProperty =
            DependencyProperty.RegisterAttached(
                "ShowIcon",
                typeof(bool),
                typeof(ButtonProperties),
                new PropertyMetadata(false, OnShowIconChanged));

        public static bool GetShowIcon(DependencyObject obj)
        {
            return (bool)obj.GetValue(ShowIconProperty);
        }

        public static void SetShowIcon(DependencyObject obj, bool value)
        {
            obj.SetValue(ShowIconProperty, value);
        }

        private static void OnShowIconChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // Можно добавить логику при изменении свойства
        }
    }
}
