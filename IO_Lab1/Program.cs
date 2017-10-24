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
    public class SumTab
    {
        public int numberThread;
        public int[] tab;
        public int size;

        public SumTab(int nr, int[] ltab, int lsize)
        {
            numberThread = nr;
            tab = ltab;
            size = lsize;
        }
    }

    class Program
    {

        private static Object thisLock = new Object();

        public static int sum;

        static void Main(string[] args)
        {
            /*Zad4
            ThreadPool.QueueUserWorkItem(ThreadProcServer);
            ThreadPool.QueueUserWorkItem(ThreadProcClient);
            ThreadPool.QueueUserWorkItem(ThreadProcClient);
            ThreadPool.QueueUserWorkItem(ThreadProcClient);

            Console.WriteLine("ThreadMain");
            Thread.Sleep(5000);*/

            Zad5(12, 3);
            Console.ReadKey();
        }

        static void Zad5(int table_size, int size_thread)
        {
            sum = 0;

            int[] tab = new int[table_size];
            Random rnd = new Random();
            Console.Write("Tablica: ");
            int tmp = 0;
            for (int i=0; i<table_size; i++)
            {
                tab[i] = rnd.Next(1, 100);
                Console.Write(tab[i] + ", ");
                tmp += tab[i];
            }
            Console.Write("\nSuma = "+tmp);

            for (int i=0; i<table_size/size_thread; i++)
            {
                int[] tab_thread = new int[size_thread];
                int k = 0;
                Console.Write("\nTablica Watku "+i+": ");
                for (int j=(i*size_thread); j< (i * size_thread+size_thread); j++)
                {
                    tab_thread[k] = tab[j];
                    Console.Write(tab_thread[k] + ", ");
                    k++;
                }
                SumTab sb = new SumTab(i,tab_thread, size_thread);
                ThreadPool.QueueUserWorkItem(new WaitCallback(ThreadSum),  sb );
            }
            
            Console.Write("\nSum = " + sum);
        }

        static void ThreadSum(Object stateInfo)
        {
            int tmp = 0;
            SumTab sb = (SumTab)stateInfo;
            for (int i = 0; i < sb.size; i++)
            {
                tmp += sb.tab[i];
            }
            sum += tmp;
            Console.Write("\nWątek " + sb.numberThread +" sum = " + sum);
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
                //Console.Write("["+message[i]+"]="+(int)message[i]+", ");
                if ((int)message[i] != 0) newmessage += message[i];
            }
            Console.WriteLine(newmessage);
            Console.ResetColor();
        }
    }
}