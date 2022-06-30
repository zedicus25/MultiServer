using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Client
{
    internal class ClientNetwork
    {
        private const int PORT = 8008;
        private const string IP_ADDR = "127.0.0.1";
        private IPEndPoint iPEnd;
        private Socket socket;
        public ClientNetwork()
        {
            iPEnd = new IPEndPoint(IPAddress.Parse(IP_ADDR), PORT);
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Connect(iPEnd);
        }



        public void SendToServer(string data, bool isFile = false)
        {
            if (socket.Connected)
            {
                try
                {
                    byte[] byteData;
                    if (isFile)
                    {
                        string extension = data.Substring(data.LastIndexOf('.'), data.Length - data.LastIndexOf('.'));
                        byteData = Encoding.Unicode.GetBytes(extension);
                        socket.Send(byteData);
                        Thread.Sleep(1000);
                        byteData = File.ReadAllBytes(data);
                        socket.Send(byteData);
                        return;
                    }
                    else
                    {
                        byteData = Encoding.Unicode.GetBytes(data);
                        socket.Send(byteData);
                        return;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            
        }
        ~ClientNetwork()
        {
            socket.Shutdown(SocketShutdown.Both);
            socket.Close();
        }
    }
}
