using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace GameServer
{
    public class ClientHandler
    {
        private readonly TcpClient client;
        private readonly GameServer server;

        public ClientHandler(TcpClient client, GameServer server)
        {
            this.client = client;
            this.server = server;
        }

        public void Handle()
        {
            Task.Run(() =>
            {
                NetworkStream stream = null;
                try
                {
                    stream = client.GetStream();
                    var buffer = new byte[1024];

                    while (true)
                    {
                        int bytesRead = stream.Read(buffer, 0, buffer.Length);
                        if (bytesRead > 0)
                        {
                            string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                            Console.WriteLine($"Получено сообщение: {message}");

                            // Ответ клиенту
                            byte[] response = Encoding.UTF8.GetBytes("Сообщение обработано");
                            stream.Write(response, 0, response.Length);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка клиента: {ex.Message}");
                }
                finally
                {
                    if (stream != null)
                        stream.Dispose();

                    client.Close();
                }
            });
        }
    }
}
