using System;

namespace GameServer
{
    class Program
    {
        static void Main(string[] args)
        {
            // Создаем сервер
            var server = new GameServer();
            server.Start(12345); // Указываем порт для подключения

            Console.WriteLine("Нажмите любую клавишу для остановки сервера...");
            Console.ReadKey();

            // Останавливаем сервер
            server.Stop();
        }
    }
}
