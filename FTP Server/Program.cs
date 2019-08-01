using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FTP_Server
{
    public class Program
    {
        public static event Action<string> data_Event;
        public static event Action<string> user_Event;
        private static FtpServer server;
        private static bool work;

        /// <summary>
        /// Отправка сообщения о действиях пользователя
        /// </summary>
        /// <param name="message"></param>
        public static void DataSender(string message)
        {
            if (data_Event != null) data_Event(message);
        }

        /// <summary>
        /// Отправка сообщения о подключении пользователя 
        /// </summary>
        /// <param name="message"></param>
        public static void UsersSender(string message)
        {
            if (user_Event != null) user_Event(message);
        }

        public Program()
        {
            server = new FtpServer(IPAddress.Any, 21);
        }

        /// <summary>
        /// Отправка информации о зарегестрированных пользователях
        /// </summary>
        public static void ShowUsers()
        {
            string text = null;
            foreach (User user in UserStore._users)
            {
                text += user.Identification + " | " + user.Username + " | " + user.Password + " | " + user.HomeDir + "\n";
            }

            DataSender(text);
        }


        /// <summary>
        /// Удаление пользователя из системы по его ID
        /// </summary>
        /// <param name="iden"></param>
        public static void DeleteUser(int iden)
        {
            foreach (User user in UserStore._users)
            {
                if (iden == user.Identification)
                {
                    UserStore._users.Remove(user);

                    DataSender("User delete.");
                }
            }
        }

        /// <summary>
        /// Удаление пользователя из системы по его логину
        /// </summary>
        /// <param name="login"></param>
        public static void DeleteUser(string login)
        {
            foreach (User user in UserStore._users)
            {
                if (login == user.Username)
                {
                    UserStore._users.Remove(user);
                }
            }
        }


        /// <summary>
        /// Добавление пользователя в систему
        /// </summary>
        /// <param name="login"></param>
        /// <param name="password"></param>
        /// <param name="path">Стартовый каталог</param>
        public static void CreateUser(string login, string password, string path)
        {
            User user = new User();
            user.Username = login;
            user.Password = password;
            user.HomeDir = path;
            user.Identification = UserStore._users.Count;

            UserStore._users.Add(user);

            DataSender("Create user.");
        }


        /// <summary>
        /// Начало работы сервера
        /// </summary>
        public static void Start()
        {
            work = true;
            DataSender("Start work");
            server.Start();

            Thread thread = new Thread(TcpConnected);
            thread.Start();
        }

        /// <summary>
        /// Остановка работы сервера
        /// </summary>
        public static void Stop()
        {
            work = false;
            DataSender("End work");
            server.Stop();
            server.Dispose();
        }

        /// <summary>
        /// Проверка на состояние подключеных пользователей
        /// </summary>
        private static void TcpConnected()
        {
            while (work)
            {
                for (int i = 0; i < FtpServer.tcpListeners.Count; i++)
                {
                    if (!FtpServer.tcpListeners[i].client.Connected && !FtpServer.tcpListeners[i].ck)
                    {
                        DataSender("Not connectionn with ip = " + FtpServer.tcpListeners[i].ip);
                        FtpServer.tcpListeners.RemoveAt(i);
                    }

                    Thread.Sleep(2000);
                }
            }
        }

        public static void Main()
        {
            Program program = new Program();
            CreateUser("admin", "admin", "C:\\");
        
            Start();
            Console.WriteLine("Start server");
        
            Console.ReadLine();
            Stop();
        
        }
        
    }
}
