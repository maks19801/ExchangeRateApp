using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace ExchangeRateServer
{
    internal class ClientObject
    {
        public string fullMessage;
        static public TcpClient client;
        public const int maxRequestsCount = 3;
        static int counter = 0;

        public List<CurrenciesExchangeRate> currenciesExchanges = new List<CurrenciesExchangeRate>{
            new CurrenciesExchangeRate("EURO USD", 1.03m),
            new CurrenciesExchangeRate("USD EURO", 0.8m)
        }; 
        public ClientObject(TcpClient tcpClient)
        { 
            client = tcpClient;
            Console.WriteLine("Client connected" );
        }

        public static void TooManyClients()
        {
            NetworkStream stream = client.GetStream();
            byte[] tooManyClients = Encoding.Unicode.GetBytes("too many clients! wait for a minute and try again");
            stream.Write(tooManyClients, 0, tooManyClients.Length);
        }
        public void Process()
        {
            string writePath = @"D:\logging.txt";
            NetworkStream stream = null;
            try
            {
                stream = client.GetStream();
                byte[] data = new byte[64]; // буфер для получаемых данных
                while (counter < maxRequestsCount)
                {
                   
                        // получаем сообщение
                        StringBuilder builder = new StringBuilder();
                        int bytes = 0;
                        do
                        {
                            bytes = stream.Read(data, 0, data.Length);
                            builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                        }
                        while (stream.DataAvailable);

                        fullMessage = builder.ToString();
                        string message = fullMessage.Substring(fullMessage.IndexOf(':') + 1).Trim().ToUpper();
                        Console.WriteLine(fullMessage);
                        foreach (CurrenciesExchangeRate item in currenciesExchanges)
                        {
                            if (item.CurrencyPair == message)
                            {
                                byte[] currencyRatio = Encoding.Unicode.GetBytes(item.CurrencyRatio.ToString());
                                stream.Write(currencyRatio, 0, currencyRatio.Length);
                                using (StreamWriter sw = new StreamWriter(writePath, true, Encoding.Default))
                                {
                                    sw.WriteLine(fullMessage.Substring(0, fullMessage.IndexOf(':')));

                                    sw.WriteLine(item.CurrencyPair + ' ' + item.CurrencyRatio);
                                }
                            }
                        }
                        counter++;     
                }
                byte[] maxRequests = Encoding.Unicode.GetBytes("too many requests, try again later");
                stream.Write(maxRequests, 0, maxRequests.Length);
                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {

                if (stream != null)
                    stream.Close();
                if (client != null)
                    client.Close();
                using (StreamWriter sw = new StreamWriter(writePath, true, Encoding.Default))
                {
                    if(fullMessage != null)
                    {
                        sw.WriteLine("Time of disconnection: " + fullMessage.Substring(0, fullMessage.IndexOf(':')) + ' ' + DateTime.Now);
                    }
                    
                    
                }
            }
        }
    }
}