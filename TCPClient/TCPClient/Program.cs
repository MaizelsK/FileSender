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
                    TcpClient tcpClient = new TcpClient();
                    tcpClient.Connect(new IPEndPoint(IPAddress.Parse("127.0.0.1"), defaultPort));

                    SendFile(tcpClient);

                    tcpClient.Close();
                }
            }
            catch (SocketException ex)
            {
                Console.WriteLine(ex.Message);
                Console.ReadLine();
            }
        }

        public static Metadata GetMetadata()
        {
            string filePath;

            while (true)
            {
                Console.Clear();
                Console.Write("Укажите путь к файлу: ");

                filePath = Console.ReadLine();
                if (File.Exists(filePath))
                    break;
            }

            FileInfo fileInfo = new FileInfo(filePath);

            Metadata metadata = new Metadata { FileName = fileInfo.Name, FileSize = fileInfo.Length, FilePath = filePath };

            return metadata;
        }

        private static void SendFile(TcpClient client)
        {
            Metadata metadata = GetMetadata();

            byte[] data = new byte[metadata.FileSize + 513];

            // Вставка метаданных в начало массива всех данных
            byte[] metadataJson = Encoding.Default.GetBytes(JsonConvert.SerializeObject(metadata));
            Array.Resize(ref metadataJson, 512);
            Array.Copy(metadataJson, data, 512);

            byte[] fileData;
            using (FileStream fs = File.OpenRead(metadata.FilePath))
            {
                fileData = new byte[metadata.FileSize];
                fs.Read(fileData, 0, (int)metadata.FileSize);
            }

            Array.Copy(fileData, 0, data, 512, fileData.Length);

            client.Client.Send(data);

            Console.WriteLine("Файл успешно отправлен");
        }
    }
}
