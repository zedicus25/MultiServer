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
            /*Task readTask = new Task(() =>
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

            });*/

            try
            {
                socket.Connect(iPEnd);
                Console.WriteLine("Send file");
                    byte[] data = File.ReadAllBytes(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), 
                        "foto"));
                Console.WriteLine(data);
                    socket.Send(data);
                    data = new byte[0];



            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                //socket.Shutdown(SocketShutdown.Both);
                //socket.Close();
            }
            Console.ReadLine();
        }
    }
}
