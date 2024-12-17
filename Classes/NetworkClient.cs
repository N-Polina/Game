using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Network
{
    public class NetworkClient
    {
        private NetworkStream stream;
        private TcpClient client;

        public event Action<string> OnMessageReceived; // Событие для обработки входящих сообщений

        public NetworkClient(string serverAddress, int port)
        {
            client = new TcpClient(serverAddress, port);
            stream = client.GetStream();
        }

        public void StartListening()
        {
            // Запускаем прослушивание сообщений в отдельном потоке
            Task.Run(() => ListenForMessages());
        }

        private void ListenForMessages()
        {
            try
            {
                var buffer = new byte[1024];
                while (true)
                {
                    int bytesRead = stream.Read(buffer, 0, buffer.Length);
                    if (bytesRead > 0)
                    {
                        string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                        OnMessageReceived?.Invoke(message); // Вызываем событие
                    }
                }
            }
            catch (IOException ex)
            {
                Console.WriteLine($"Ошибка сети: {ex.Message}");
                Disconnect(); // Корректно закрываем соединение
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Необработанное исключение: {ex.Message}");
            }
        }

        public void Disconnect()
        {
            stream?.Close();
            client?.Close();
        }

        public void SendMessage(string message)
        {
            if (client?.Connected == true)
            {
                var data = Encoding.UTF8.GetBytes(message);
                stream.Write(data, 0, data.Length);
            }
        }
    }
}
