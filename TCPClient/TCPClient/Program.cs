using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TCPClient
{
    class Program
    {
        private static int defaultPort = 3535;

        static void Main(string[] args)
        {
            try
            {
                while (true)
                {
                    string filePath = SendMetadata();

                    if (String.IsNullOrWhiteSpace(filePath))
                        continue;

                    SendFile(filePath);
                }
            }
            catch (SocketException ex)
            {
                Console.WriteLine(ex.Message);
                Console.ReadLine();
            }
        }

        public static string SendMetadata()
        {
            TcpClient tcpClient = new TcpClient();
            tcpClient.Connect(new IPEndPoint(IPAddress.Parse("127.0.0.1"), defaultPort));

            Console.Clear();
            Console.Write("Укажите путь к файлу: ");

            string filePath = Console.ReadLine();

            if (String.IsNullOrWhiteSpace(filePath))
                return "";

            FileInfo fileInfo = new FileInfo(filePath);

            Metadata metadata = new Metadata { FileName = fileInfo.Name, FileSize = fileInfo.Length };
            string jsonData = JsonConvert.SerializeObject(metadata);

            tcpClient.Client.Send(Encoding.UTF8.GetBytes(jsonData));
            tcpClient.Close();

            return filePath;
        }

        private static void SendFile(string filePath)
        {
            TcpClient tcpClient = new TcpClient();
            tcpClient.Connect(new IPEndPoint(IPAddress.Parse("127.0.0.1"), defaultPort));

            try
            {
                tcpClient.Client.SendFile(filePath);
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine(ex.Message);
                tcpClient.Close();
                return;
            }

            Console.WriteLine("Файл успешно отправлен");
            tcpClient.Close();
        }
    }
}
