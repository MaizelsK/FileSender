using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

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
                    GetClientData(server);
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

        public static async void GetClientData(TcpListener server)
        {
            Metadata metadata = await GetMetadata(server);

            SaveFile(server, metadata);
        }

        private static Task<Metadata> GetMetadata(TcpListener server)
        {
            return Task.Run(() =>
            {
                TcpClient client = server.AcceptTcpClient();

                Console.WriteLine("Входщее соеденение для получения метаданных...");

                int bytes;
                byte[] data = new byte[1024];

                StringBuilder builder = new StringBuilder();
                Metadata metadata;

                using (var networkStream = client.GetStream())
                {
                    do
                    {
                        bytes = networkStream.Read(data, 0, data.Length);
                    }
                    while (networkStream.DataAvailable);

                    string json = Encoding.UTF8.GetString(data);

                    metadata = JsonConvert.DeserializeObject<Metadata>(Encoding.UTF8.GetString(data));
                }

                client.Close();
                return metadata;
            });
        }

        private static void SaveFile(TcpListener server, Metadata metadata)
        {
            Task.Run(() =>
            {
                TcpClient client = server.AcceptTcpClient();

                Console.WriteLine("Входщее соеденение для получения файла...");

                int bytes;
                byte[] data = new byte[metadata.FileSize];

                using (var networkStream = client.GetStream())
                {
                    do
                    {
                        bytes = networkStream.Read(data, 0, data.Length);
                    }
                    while (networkStream.DataAvailable);

                    File.WriteAllBytes(Directory.GetCurrentDirectory() + "\\" + metadata.FileName, data);
                }

                client.Close();
            });
        }
    }
}
