using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ExchangeRateServer
{
    class Program
    {
        
        const int port = 8888;
        static TcpListener listener;
        static List<Thread> threads = new List<Thread>();
        const int maxClients = 2;
        static void Main(string[] args)
        {
            string writePath = @"D:\logging.txt";
            try
            {
                listener = new TcpListener(IPAddress.Parse("127.0.0.1"), port);
                listener.Start();
                Console.WriteLine("Ожидание подключений...");


                while (threads.Count <= maxClients)
                {
                    TcpClient client = listener.AcceptTcpClient();
                    ClientObject clientObject = new ClientObject(client);
                    // создаем новый поток для обслуживания нового клиента
                    Thread clientThread = new Thread(new ThreadStart(clientObject.Process));
                    clientThread.Start();
                    threads.Add(clientThread);
                    using (StreamWriter sw = new StreamWriter(writePath, true, System.Text.Encoding.Default))
                    {
                        sw.WriteLine("Time of connection: " + DateTime.Now);
                    }
                }
                ClientObject.TooManyClients();
                listener.Stop();
                                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                if (listener != null)
                    listener.Stop();
            }
        }
    }
}
