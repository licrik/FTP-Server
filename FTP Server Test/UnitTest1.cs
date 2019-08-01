using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FTP_Server;
using System.Net.Sockets;

namespace FTP_Server_Test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void Directory()
        {
            FTP_Server.ClientConnection.ListOperation("C:\\");
        }
    }
}
