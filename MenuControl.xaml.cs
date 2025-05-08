using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace wpf_тесты_для_обучения
{
    /// <summary>
    /// Логика взаимодействия для MenuControl.xaml
    /// </summary>
    // MenuControl.xaml.cs
    public class MenuItem : INotifyPropertyChanged
    {
        private bool _isExpanded;

        public string Title { get; set; }
        public Style Style { get; set; }
        public Action ClickAction { get; set; }
        public List<MenuItem> Children { get; set; } = new List<MenuItem>();
        public int Level { get; set; }

        public bool IsExpanded
        {
            get => _isExpanded;
            set
            {
                if (_isExpanded != value)
                {
                    _isExpanded = value;
                    OnPropertyChanged(nameof(IsExpanded));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public partial class MenuControl : UserControl
    {
        public static readonly DependencyProperty MenuItemsProperty =
            DependencyProperty.Register("MenuItems", typeof(ObservableCollection<MenuItem>), typeof(MenuControl), new PropertyMetadata(null));

        public ObservableCollection<MenuItem> MenuItems
        {
            get => (ObservableCollection<MenuItem>)GetValue(MenuItemsProperty);
            set => SetValue(MenuItemsProperty, value);
        }

        public MenuControl()
        {
            InitializeComponent();
            MenuItems = new ObservableCollection<MenuItem>();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            var menuItem = (MenuItem)button.DataContext;

            // Всегда вызываем действие, если оно есть (для всех уровней)
            menuItem.ClickAction?.Invoke();

            // Обрабатываем раскрытие/сворачивание только для уровней 0 и 1
            if (menuItem.Level == 0) // Level 1 - главные пункты
            {
                foreach (var item in MenuItems.Where(i => i != menuItem))
                {
                    item.IsExpanded = false;
                }
                menuItem.IsExpanded = !menuItem.IsExpanded;
            }
            else if (menuItem.Level == 1) // Level 2 - подпункты
            {
                var parent = FindParent(menuItem, MenuItems);
                if (parent != null)
                {
                    foreach (var child in parent.Children.Where(c => c != menuItem))
                    {
                        child.IsExpanded = false;
                    }
                    menuItem.IsExpanded = !menuItem.IsExpanded;
                }
            }

            // Останавливаем всплытие события
            e.Handled = true;
        }

        private void CollapseChildren(MenuItem item)
        {
            foreach (var child in item.Children)
            {
                child.IsExpanded = false;
                CollapseChildren(child);
            }
        }

        private MenuItem FindParent(MenuItem child, IEnumerable<MenuItem> items)
        {
            foreach (var item in items)
            {
                if (item.Children.Contains(child))
                    return item;

                var parent = FindParent(child, item.Children);
                if (parent != null)
                    return parent;
            }
            return null;
        }
    }

    //public class RelayCommand : ICommand
    //{
    //    private readonly Action<object> _execute;
    //    private readonly Func<object, bool> _canExecute;

    //    public RelayCommand(Action<object> execute, Func<object, bool> canExecute = null)
    //    {
    //        _execute = execute ?? throw new ArgumentNullException(nameof(execute));
    //        _canExecute = canExecute;
    //    }

    //    public bool CanExecute(object parameter) => _canExecute == null || _canExecute(parameter);

    //    public void Execute(object parameter) => _execute(parameter);

    //    public event EventHandler CanExecuteChanged
    //    {
    //        add => CommandManager.RequerySuggested += value;
    //        remove => CommandManager.RequerySuggested -= value;
    //    }
    //}
}
