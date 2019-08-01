using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace FTP_Server
{
    [Obsolete]
    public static class UserStore
    {
        public static List<User> _users = new List<User>();

        static UserStore()
        {
            User user = new User();
            user.Username = "admin";
            user.Password = "admin";
            user.HomeDir = "C:\\";
            user.Identification = 0;

            _users.Add(user);
        }

        /// <summary>
        /// Проверка првильности ввода даных пользователем
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static User Validate(string username, string password)
        {
           foreach(User user in _users)
           {
                if (username == user.Username && password == user.Password)
                {
                    return user;
                }
           }

            return null;
        }
    }
}
