using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wpf_тесты_для_обучения.Properties;

namespace wpf_тесты_для_обучения
{
    public  class Roles: INotifyPropertyChanged
    {
        public int Id { get; set; } // Уникальный идентификатор роли
        public string Title { get; set; } // Название роли (например, "Администратор", "Пользователь")
        public List<Tests> Tests { get; set; } = new List<Tests>(); // Связанные тесты

        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    OnPropertyChanged(nameof(IsSelected));
                }
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public Roles() { }

        public Roles(int id, string name)
        {
            Id = id;
            Title = name;
        }

    }
}
