using Game.Classes;
using System;
using System.Drawing;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

//4
namespace Game.Network
{
    public class GameClient
    {
        private TcpClient tcpClient;
        private NetworkStream networkStream;
        private readonly SharpDX.Direct2D1.Factory factory;
        private readonly int playerId;

        public event Action<string> OnMessageReceived;

        private GameApp gameApp; // Поле для хранения GameApp

        public GameClient(SharpDX.Direct2D1.Factory factory, int playerId, GameApp gameApp)
        {
            this.factory = factory;
            this.playerId = playerId;
            this.gameApp = gameApp;
        }

        public void Connect(string ipAddress, int port)
        {
            try
            {
                tcpClient = new TcpClient();
                tcpClient.Connect(ipAddress, port);
                networkStream = tcpClient.GetStream();

                StartPing(); // Запускаем пинг

                var listenThread = new Thread(ListenForMessages) { IsBackground = true };
                listenThread.Start();

                Console.WriteLine("Подключение к серверу успешно.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка подключения: {ex.Message}");
            }
        }


        public void SendMessage(string message)
        {
            try
            {
                if (networkStream != null && tcpClient.Connected)
                {
                    Console.WriteLine($"Отправка сообщения серверу: {message}");
                    var buffer = Encoding.UTF8.GetBytes(message);
                    networkStream.Write(buffer, 0, buffer.Length);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при отправке сообщения: {ex.Message}");
            }
        }


        private void ListenForMessages()
        {
            try
            {
                var buffer = new byte[1024];
                while (true)
                {
                    int bytesRead = networkStream.Read(buffer, 0, buffer.Length);
                    if (bytesRead > 0)
                    {
                        string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                        Console.WriteLine($"Сообщение от сервера: {message}");

                        // Обработка сообщения
                        OnMessageReceived?.Invoke(message);
                    }
                }
            }
            catch (IOException ex)
            {
                Console.WriteLine($"Ошибка сети: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Необработанное исключение: {ex.Message}");
            }
            finally
            {
                Disconnect();
            }
        }

        private bool IsReady = false; // Отслеживание готовности игрока
        private bool shipSelectionCompleted = false; // Указано ли, что выбор корабля завершён


        public void ProcessServerMessage(string message)
        {
            Console.WriteLine($"Получено сообщение от сервера: {message}");

            if (message == "START_SELECT_SHIP" && !shipSelectionCompleted)
            {
                gameApp?.StartShipSelection(); // Используем метод из GameApp
            }
            else if (message == "WAIT_OTHER_PLAYER")
            {
                ShowWaitingMessage("Ожидаем второго игрока...");
            }
            else if (message == "START_GAME")
            {
                gameApp?.SetReady(); // Устанавливаем состояние через метод
                gameApp.StartAnimation();
            }
            else
            {
                Console.WriteLine($"Необработанное сообщение от сервера: {message}");
            }
        }



        private void ShowShipSelectionForm()
        {
            // Создаем и отображаем форму выбора корабля
            using (var form = new FormParam(factory, playerId))
            {
                if (form.ShowDialog() == DialogResult.OK) // Ждём, пока пользователь выберет корабль
                {
                    if (form.iship != null) // Проверяем, что корабль выбран
                    {
                        var shipType = form.iship.GetType().Name; // Получаем тип выбранного корабля
                        SendMessage($"SELECT_SHIP {playerId} {shipType}"); // Отправляем выбор на сервер
                        Console.WriteLine($"Игрок {playerId} выбрал корабль {shipType}.");
                    }
                    else
                    {
                        MessageBox.Show("Выбор корабля не был завершён.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void StartGame()
        {
            MessageBox.Show("Игра начинается!", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
            IsReady = true; // Обновляем состояние клиента
                            // Логика запуска игрового процесса
        }

        private void ShowWaitingMessage(string message)
        {
            using (var waitingForm = new Form
            {
                Text = "Ожидание...",
                Width = 300,
                Height = 150,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                StartPosition = FormStartPosition.CenterScreen,
                ControlBox = false
            })
            {
                var label = new Label
                {
                    Text = message,
                    Dock = DockStyle.Fill,
                    TextAlign = ContentAlignment.MiddleCenter
                };
                waitingForm.Controls.Add(label);

                // Закрываем окно через 2 секунды
                var timer = new System.Windows.Forms.Timer { Interval = 2000 };
                timer.Tick += (s, e) =>
                {
                    timer.Stop();
                    waitingForm.Close();
                };
                timer.Start();

                waitingForm.ShowDialog();
            }
        }


        public void SelectShip(int shipType)
        {
            try
            {
                SendMessage($"SELECT_SHIP {playerId} {shipType}"); // Добавляем playerId в сообщение
                Console.WriteLine($"Игрок {playerId} выбрал корабль {shipType}.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при выборе корабля: {ex.Message}");
            }
        }
        public void StartPing()
        {
            Task.Run(async () =>
            {
                while (tcpClient.Connected)
                {
                    try
                    {
                        SendMessage("PING");
                        await Task.Delay(5000); // Интервал 5 секунд
                    }
                    catch
                    {
                        Console.WriteLine("Соединение с сервером потеряно.");
                        Disconnect();
                        break;
                    }
                }
            });
        }

        public void Disconnect()
        {
            try
            {
                networkStream?.Close();
                tcpClient?.Close();
                Console.WriteLine("Соединение с сервером закрыто.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при закрытии соединения: {ex.Message}");
            }
        }
    }
}
