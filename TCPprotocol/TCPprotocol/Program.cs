using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Threading;

namespace TCPprotocol
{
    class Program
    {
        private static int defaultPort = 3535;

        static void Main(string[] args)
        {
            TcpListener server = new TcpListener(IPAddress.Parse("127.0.0.1"), defaultPort);

            try
            {
                server.Start();

                Console.WriteLine("Сервер запущен и ждет подключения...");

                while (true)
                {
                    TcpClient client = server.AcceptTcpClient();

                    GetClientData(client);
                }
            }
            catch (SocketException ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                server.Stop();
            }
        }

        public static async void GetClientData(TcpClient client)
        {
            await Task.Run(() => { GetMetadata(client); });

            client.Close();
        }

        private static void GetMetadata(TcpClient client)
        {
            int bytes;
            byte[] metadataBytes = new byte[512];
            Metadata metadata;

            using (var networkStream = client.GetStream())
            {
                Console.WriteLine("Получение данных...");

                if (networkStream.DataAvailable)
                {
                    bytes = networkStream.Read(metadataBytes, 0, 512);

                    string metadataJson = Encoding.Default.GetString(metadataBytes);
                    metadata = JsonConvert.DeserializeObject<Metadata>(metadataJson);

                    byte[] fileData = new byte[metadata.FileSize];
                    bytes = networkStream.Read(fileData, 0, (int)metadata.FileSize);

                    SaveFile(metadata, fileData);
                }
            }
        }

        private static void SaveFile(Metadata metadata, byte[] fileData)
        {
            Task.Run(() =>
            {
                Console.WriteLine("Сохранение файла...");

                File.WriteAllBytes(Directory.GetCurrentDirectory() + "\\" + metadata.FileName, fileData);

                Console.WriteLine("Файл успешно сохранен");
            });
        }
    }
}
