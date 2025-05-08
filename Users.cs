using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wpf_тесты_для_обучения.Properties;

namespace wpf_тесты_для_обучения
{
    public static class UserSession
    {
        public static Users SelectedUser { get; set; }
    }

    public class Users
    {
        public int Id { get; set; } // ID пользователя
        public string FirstName { get; set; }
        public string Name { get; set; }
        public string Patronymic { get; set; }
        public Roles UserRole { get; set; } // Связь с ролью
        public string Password { get; set; }
        public bool IsDone { get; set; }
        public bool IsDone2 { get; set; }

        public string FullName => $"{FirstName} {Name} {Patronymic}";
        public string FullNameId => $"{Id}. {FirstName} {Name} {Patronymic}";
        public string FullNameRole => $"{FirstName} {Name} {Patronymic} - {UserRole.Title}";
        public Users() { }

        public Users(int id, string firstName, string name, string patronymic, Roles userRole, string password, bool isDone)
        {
            Id = id;
            FirstName = firstName;
            Name = name;
            Patronymic = patronymic;
            UserRole = userRole;
            Password = password;
            IsDone = isDone;
        }

        // Метод для получения строки в формате "Ф.И.О. - Название роли"
        public string GetDisplayText()
        {
            return $"{FirstName} {Name} {Patronymic} - {UserRole.Title}";
        }

        
        public void ChangePassword(string newPassword)
            {
                if (string.IsNullOrWhiteSpace(newPassword))
                {
                    throw new ArgumentException("Password cannot be empty or whitespace.");
                }

                Password = newPassword;
            }


    }
}
