using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Program
{
    class Program
    {

        private static Object thisLock = new Object();

        static void Main(string[] args)
        {


            ThreadPool.QueueUserWorkItem(ThreadProcServer);
            ThreadPool.QueueUserWorkItem(ThreadProcClient);
            ThreadPool.QueueUserWorkItem(ThreadProcClient);
            ThreadPool.QueueUserWorkItem(ThreadProcClient);

            Console.WriteLine("ThreadMain");
            Thread.Sleep(5000);
        }

        static void ThreadProcServer(Object stateInfo)
        {
            TcpListener server = new TcpListener(IPAddress.Any, 2048);
            server.Start();
            while (true)
            {
                TcpClient client = server.AcceptTcpClient();

                ThreadPool.QueueUserWorkItem(ThreadProcNewClient, client);
            }
        }

        static void ThreadProcClient(Object stateInfo)
        {
            TcpClient client = new TcpClient();
            client.Connect(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 2048));
            byte[] message = new ASCIIEncoding().GetBytes("wiadomosc");
            client.GetStream().Write(message, 0, message.Length);
            NetworkStream stream = client.GetStream();
            stream.Read(message, 0, message.Length);

            lock (thisLock)
            {
                writeConsoleMessage("Klient: " + new ASCIIEncoding().GetString(message), ConsoleColor.Green);
            }
        }

        static void ThreadProcNewClient(Object stateInfo)
        {
            TcpClient client = (TcpClient)stateInfo;
            byte[] buffer = new byte[1024];
            client.GetStream().Read(buffer, 0, 1024);
            lock (thisLock)
            {
                writeConsoleMessage("Server: " + new ASCIIEncoding().GetString(buffer), ConsoleColor.Red);
            }
            client.GetStream().Write(buffer, 0, buffer.Length);
            client.Close();
        }

        static void writeConsoleMessage(string message, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            string newmessage = "";
            for (int i = 0; i < message.Length; i++)
            {
                if (message[i] != ' ') newmessage += message[i];
            }
            Console.WriteLine(message);
            Console.ResetColor();
        }
    }
}