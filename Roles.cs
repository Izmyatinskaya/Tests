using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wpf_тесты_для_обучения.Properties;

namespace wpf_тесты_для_обучения
{
    public  class Roles
    {
        public int Id { get; set; } // Уникальный идентификатор роли
        public string Title { get; set; } // Название роли (например, "Администратор", "Пользователь")
        public List<Tests> Tests { get; set; } = new List<Tests>(); // Связанные тесты
        public int Count
        {
            get
            {
                string query = $"Select count(Id) from RoleAccess where Role_Id = {Id}";
                DatabaseHelper databaseHelper = new DatabaseHelper("Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=\"D:\\Проекты\\Тесты обучение WPF\\wpf тесты для обучения\\DB.mdf\";Integrated Security=True");
                int count = (int)databaseHelper.ExecuteScalar(query);
                return count;
            }
        }
        public Roles() { }

        public Roles(int id, string name)
        {
            Id = id;
            Title = name;
        }

    }
}
