using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    internal class Program
    {
        static void Main(string[] args)
        {
            const int PORT = 8008;
            const string IP_ADDR = "127.0.0.1";
            IPEndPoint iPEnd = new IPEndPoint(IPAddress.Parse(IP_ADDR), PORT);
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            Task readTask = new Task(() =>
            {

                StringBuilder sb = new StringBuilder();
                do
                {
                    if (socket.Available > 0)
                    {
                        int byteCount = 0;
                        byte[] buffer = new byte[256];
                        do
                        {
                            byteCount = socket.Receive(buffer);
                            sb.Append(Encoding.Unicode.GetString(buffer, 0, byteCount));
                        } while (socket.Available > 0);
                        Console.WriteLine(sb.ToString());
                        sb.Clear();
                    }
                } while (true);

            });

            try
            {
                //readTask.Start();
                string msg = String.Empty;
                socket.Connect(iPEnd);

                Console.WriteLine("Send extension");
                byte[] extansion = Encoding.Unicode.GetBytes(".png");
                socket.Send(extansion);

                byte[] data = File.ReadAllBytes(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures),
                        "foto.png"));
                Console.WriteLine("Send file");
                socket.Send(data);

                /*do
                {
                    msg = String.Empty;
                    msg = Console.ReadLine();
                    byte[] data1 = Encoding.Unicode.GetBytes(msg);
                    socket.Send(data1);
                    data = new byte[0];

                }
                while (!msg.Equals("-end"));*/



            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                socket.Shutdown(SocketShutdown.Both);
            }
            Console.ReadLine();
        }
    }
}
