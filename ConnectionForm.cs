using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;

namespace Game.Classes
{
    public class ConnectionForm : Form
    {
        private TextBox txtIp;
        private TextBox txtPort;
        private Button btnConnect;

        public string IpAddress => txtIp.Text;

        public int Port => int.TryParse(txtPort.Text, out int port) ? port : 0;

        public int PlayerId { get; private set; } // Полученный ID игрока

        public ConnectionForm()
        {
            Text = "Подключение к серверу";
            Width = 400;
            Height = 200;
            StartPosition = FormStartPosition.CenterScreen;

            // Метка для IP
            var lblIp = new Label { Text = "IP-адрес:", Left = 20, Top = 20, Width = 100 };
            Controls.Add(lblIp);

            // Текстовое поле для IP
            txtIp = new TextBox { Left = 120, Top = 20, Width = 200, Text = "192.168.100.6" };
            Controls.Add(txtIp);

            // Метка для порта
            var lblPort = new Label { Text = "Порт:", Left = 20, Top = 60, Width = 100 };
            Controls.Add(lblPort);

            // Текстовое поле для порта
            txtPort = new TextBox { Left = 120, Top = 60, Width = 200, Text = "12345" };
            Controls.Add(txtPort);

            // Кнопка подключения
            btnConnect = new Button { Text = "Подключиться", Left = 120, Top = 100, Width = 200 };
            btnConnect.Click += BtnConnect_Click;
            Controls.Add(btnConnect);
        }

        private void BtnConnect_Click(object sender, EventArgs e)
        {
            // Валидация IP и порта
            if (string.IsNullOrWhiteSpace(txtIp.Text) || !IPAddress.TryParse(txtIp.Text, out _))
            {
                MessageBox.Show("Введите корректный IP-адрес!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!int.TryParse(txtPort.Text, out int port) || port <= 0 || port > 65535)
            {
                MessageBox.Show("Порт должен быть числом от 1 до 65535!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Устанавливаем соединение
            try
            {
                PlayerId = GetPlayerIdFromServer(txtIp.Text, port);
                MessageBox.Show($"Подключено! Ваш ID: {PlayerId}", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                DialogResult = DialogResult.OK; // Закрываем форму с успешным результатом
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка подключения: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private int GetPlayerIdFromServer(string ip, int port)
        {
            using (var client = new TcpClient())
            {
                client.Connect(ip, port);
                using (var stream = client.GetStream())
                {
                    // Читаем сообщение от сервера
                    var buffer = new byte[256];
                    int bytesRead = stream.Read(buffer, 0, buffer.Length);
                    string response = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                    // Ожидаем сообщение вида "PlayerId~1"
                    if (response.StartsWith("PlayerId~"))
                    {
                        return int.Parse(response.Split('~')[1]);
                    }
                    else
                    {
                        throw new Exception("Некорректный ответ от сервера.");
                    }
                }
            }
        }
    }
}
