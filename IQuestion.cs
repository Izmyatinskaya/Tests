using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace wpf_тесты_для_обучения
{
    public interface IQuestion
    {
        int Number { get; set; }
        void AddAnswer(object sender, RoutedEventArgs e);
        void DeleteAnswer(object sender, RoutedEventArgs e);
        void UpdateErrorMessages();
    }


}
