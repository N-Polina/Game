using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
//1
namespace GameServer
{
    public class GameServer
    {
        private TcpListener tcpListener;
        private bool isRunning;
        private readonly Dictionary<TcpClient, int> playerShips = new Dictionary<TcpClient, int>();
        private readonly List<TcpClient> connectedClients = new List<TcpClient>();

        public async Task ListenForClients()
        {
            while (isRunning)
            {
                try
                {
                    Console.WriteLine("Ожидание подключения клиента...");
                    var client = await tcpListener.AcceptTcpClientAsync();
                    connectedClients.Add(client);
                    Console.WriteLine($"Клиент подключен: {client.Client.RemoteEndPoint}");

                    // Сразу отправляем сообщение для выбора корабля
                    SendMessage(client, "START_SELECT_SHIP");

                    // Обрабатываем клиента
                    _ = Task.Run(() => HandleClient(client));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка при подключении клиента: {ex.Message}");
                }
            }
        }

        public event Action<int, string> OnClientMessageReceived;

        private void HandleClient(TcpClient client)
        {
            int clientId = connectedClients.IndexOf(client) + 1; // Генерация ID клиента на основе индекса в списке
            try
            {
                NetworkStream stream = client.GetStream();
                byte[] buffer = new byte[1024];

                // Отправляем клиенту приветственное сообщение
                SendMessage(client, $"WELCOME Client {clientId}");

                while (isRunning && client.Connected)
                {
                    int bytesRead = stream.Read(buffer, 0, buffer.Length);
                    if (bytesRead > 0)
                    {
                        string message = Encoding.UTF8.GetString(buffer, 0, bytesRead).Trim();
                        Console.WriteLine($"[Клиент {clientId}] {message}");

                        // Обработка команды SELECT_SHIP
                        if (message.StartsWith("SELECT_SHIP"))
                        {
                            var parts = message.Split(' ');
                            if (parts.Length == 3 && int.TryParse(parts[1], out int playerId))
                            {
                                Console.WriteLine($"Игрок {playerId} выбрал корабль: {parts[2]}");

                                // Сохраняем выбор корабля
                                lock (playerShips)
                                {
                                    playerShips[client] = playerId;
                                }

                                // Проверяем, выбрали ли оба игрока корабли
                                if (playerShips.Count == 2)
                                {
                                    Console.WriteLine("Оба игрока выбрали корабли. Игра начинается!");
                                    BroadcastMessage("START_GAME");
                                }
                                else
                                {
                                    SendMessage(client, "WAIT_OTHER_PLAYER");
                                }
                            }
                        }

                        // Обработка команды READY
                        else if (message == "READY")
                        {
                            Console.WriteLine($"Клиент {clientId} готов.");
                            CheckAllClientsReady(); // Проверяем, все ли клиенты готовы
                        }

                        // Обработка команды PING
                        else if (message == "PING")
                        {
                            SendMessage(client, "PONG");
                            Console.WriteLine($"[Клиент {clientId}] отправил PING. Ответ: PONG");
                        }

                        // Неизвестная команда
                        else
                        {
                            Console.WriteLine($"[Клиент {clientId}] неизвестная команда: {message}");
                            SendMessage(client, "UNKNOWN_COMMAND");
                        }

                        // Дополнительные сообщения через событие
                        OnClientMessageReceived?.Invoke(clientId, message);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка с клиентом {clientId}: {ex.Message}");
            }
            finally
            {
                DisconnectClient(client); // Отключение клиента при ошибке
            }
        }



        private void DisconnectClient(TcpClient client)
        {
            if (client != null)
            {
                connectedClients.Remove(client);
                client.Close();
                Console.WriteLine("Клиент отключен.");
            }
        }

        private void CheckAllClientsReady()
        {
            if (connectedClients.Count >= 2) // Проверяем, достаточно ли подключено клиентов
            {
                Console.WriteLine("Все клиенты готовы! Начинаем игру.");
                BroadcastMessage("START_GAME");
            }
            else
            {
                Console.WriteLine("Ожидание подключения и готовности всех клиентов.");
            }
        }


        private void BroadcastMessage(string message)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(message);
            foreach (var client in connectedClients)
            {
                try
                {
                    client.GetStream().Write(buffer, 0, buffer.Length);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка отправки сообщения клиенту: {ex.Message}");
                }
            }
        }



        private void SendMessage(TcpClient client, string message)
        {
            try
            {
                Console.WriteLine($"Отправка сообщения клиенту: {message}");
                var stream = client.GetStream();
                var buffer = Encoding.UTF8.GetBytes(message);
                stream.Write(buffer, 0, buffer.Length);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при отправке сообщения: {ex.Message}");
            }
        }

        public void Start(int port)
        {
            try
            {
                tcpListener = new TcpListener(IPAddress.Any, port);
                tcpListener.Start();
                isRunning = true;

                Console.WriteLine($"Сервер запущен на порту {port}");

                _ = ListenForClients();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при запуске сервера: {ex.Message}");
            }
        }

        public void Stop()
        {
            try
            {
                isRunning = false;
                tcpListener?.Stop();

                foreach (var client in connectedClients)
                {
                    client.Close();
                }

                connectedClients.Clear();
                Console.WriteLine("Сервер остановлен.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при остановке сервера: {ex.Message}");
            }
        }
    }
}
