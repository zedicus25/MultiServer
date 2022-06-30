using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    internal class Program
    {
        static IPEndPoint iPEnd;
        static Socket socket;
        static Socket clientSocket;
        static FileStream fs;
        static string pathForHistory;
        static void Main(string[] args)
        {
            const int PORT = 8008;
            iPEnd = new IPEndPoint(IPAddress.Parse("127.0.0.1"), PORT);
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            pathForHistory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"/Server", "history.txt");
            Task readTask = new Task(() =>
            {
                clientSocket = socket.Accept();
                StringBuilder sb = new StringBuilder();
                bool isFile = false;
                string extension = String.Empty;
                string command = String.Empty;
                try
                {
                    do
                    {
                        int byteCount = 0;
                        byte[] buffer = new byte[256];
                        do
                        {
                            byteCount = clientSocket.Receive(buffer);

                            if (isFile)
                            {
                                WriteFile(buffer, byteCount);
                            }

                            if (isFile == false)
                            {
                                sb.Append(Encoding.Unicode.GetString(buffer, 0, byteCount));
                            }

                            

                        } while (clientSocket.Available > 0);

                        isFile = false;

                        if(fs != null)
                            fs.Close();

                        if (sb.Length > 0)
                        {
                            if (sb[0].Equals('.'))
                            {
                                extension = sb.ToString();
                                isFile = true;
                                
                                CreateStream(extension);
                            }

                            if (sb[0].Equals('-'))
                            {
                                command = sb.ToString();
                            }
                            if (!sb[0].Equals('.'))
                                if (File.Exists(pathForHistory))
                                    File.AppendAllText(pathForHistory, sb.ToString()+"\n");
                            sb.Clear();
                        }

                    } while (command.Equals("-end") == false);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
              
            });
            try
            {
                socket.Bind(iPEnd);
                socket.Listen(10);
                readTask.Start();
                readTask.Wait();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                clientSocket.Shutdown(SocketShutdown.Both);
                clientSocket.Close();
                if(fs != null)
                    fs.Close();
            }
        }

        private static void CreateStream(string extension)
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"/Server";
            string ip = iPEnd.Address.ToString().Replace('.', ';');
            string file = ip + "_"+ DateTime.Now.Ticks + extension;

            if(File.Exists(pathForHistory))
                File.AppendAllText(pathForHistory, $"Send file {file}\n");

            fs = new FileStream(Path.Combine(path, file), FileMode.CreateNew, FileAccess.ReadWrite,  FileShare.None);
        }

        private static void WriteFile(byte[] data, int count)
        {
            if (fs == null)
                return;
            fs.Write(data, 0, count);
        }
    }
}
