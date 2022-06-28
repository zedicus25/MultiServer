﻿using System;
using System.Collections.Generic;
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
        static Func<string, string> GetResult;
        static void Main(string[] args)
        {
            const int PORT = 8008;
            iPEnd = new IPEndPoint(IPAddress.Parse("127.0.0.1"), PORT);
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            Task readTask = new Task(() =>
            {
                StringBuilder sb = new StringBuilder();
                GetResult += GetRes;
                clientSocket = socket.Accept();
                do
                {
                    int byteCount = 0;
                    byte[] buffer = new byte[256];
                    do
                    {
                        byteCount = clientSocket.Receive(buffer);
                        sb.Append(Encoding.Unicode.GetString(buffer, 0, byteCount));
                    } while (clientSocket.Available > 0);

                    Console.WriteLine($"Client msg:\t{sb.ToString()}");
                    if (sb[0].Equals('-'))
                    {
                        byte[] data = Encoding.Unicode.GetBytes(GetResult?.Invoke(sb.ToString()));
                        clientSocket.Send(data);
                    }
                    sb.Clear();



                } while (!sb.ToString().Equals("-end"));
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
                //clientSocket.Shutdown(SocketShutdown.Both);
                //clientSocket.Close();
            }
        }
        static string GetRes(string command)
        {
            switch (command)
            {
                case "-date":
                    return DateTime.Now.ToShortDateString();
                case "-time":
                    return DateTime.Now.ToShortTimeString();
                case "-help":
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.AppendLine("-date \t displays current date");
                        sb.AppendLine("-time \t displays current time");
                        sb.AppendLine("-end \t close programm");
                        return sb.ToString();
                    }
                default:
                    return "Unknown command";
            }
        }
    }
}
