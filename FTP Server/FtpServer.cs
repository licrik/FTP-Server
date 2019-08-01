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
    public class FtpServer : IDisposable
    {
        /// <summary>
        /// Структура для обработки проверки разрыва подключения
        /// </summary>
        public struct tcpList
        {
            public TcpClient client;
            public string ip;
            public bool ck;

            public void CK()
            {
               this.ck = true;
            }
        }

        private bool _disposed = false;
        private bool _listening = false;

        static public List<tcpList> tcpListeners = new List<tcpList>();

        private TcpListener _listener;
        private List<ClientConnection> _activeConnections;

        private IPEndPoint _localEndPoint;
        private Program program;

        public FtpServer()
            : this(IPAddress.Any, 21)
        {
        }

        /// <summary>
        /// СОздани
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <param name="port"></param>
        public FtpServer(IPAddress ipAddress, int port)
        {
            _localEndPoint = new IPEndPoint(ipAddress, port);
        }

        /// <summary>
        /// Старт FTP сервер
        /// </summary>
        public void Start()
        {
            _listener = new TcpListener(_localEndPoint);

            _listening = true;
            _listener.Start();

            _activeConnections = new List<ClientConnection>();

            _listener.BeginAcceptTcpClient(HandleAcceptTcpClient, _listener);
        }


        /// <summary>
        /// Обработка конекта пользователя к серверу
        /// </summary>
        /// <param name="result"></param>
        private void HandleAcceptTcpClient(IAsyncResult result)
        {
            if (_listening)
            {
                _listener.BeginAcceptTcpClient(HandleAcceptTcpClient, _listener);

                tcpList tcpList = new tcpList();
                TcpClient client = _listener.EndAcceptTcpClient(result);
                tcpList.client = client;
                tcpList.ip = ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString();
                tcpList.ck = false;

                tcpListeners.Add(tcpList);

                ClientConnection connection = new ClientConnection(client);

                _activeConnections.Add(connection);

                ThreadPool.QueueUserWorkItem(connection.HandleClient, client);
            }
        }

        /// <summary>
        /// Остановка сервера 
        /// </summary>
        public void Stop()
        {
            _listening = false;
            _listener.Stop();

            _listener = null;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    Stop();

                    foreach (ClientConnection conn in _activeConnections)
                    {
                        conn.Dispose();
                    }
                }
            }

            _disposed = true;
        }
    }
}
