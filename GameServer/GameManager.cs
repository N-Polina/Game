using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;

namespace GameServer
{
    public class GameManager
    {
        private Dictionary<int, Ship> players = new Dictionary<int, Ship>();
        private List<Torpedo> torpedoes = new List<Torpedo>();
        private List<Rock> rocks = new List<Rock>();

        public GameManager()
        {
            InitializeGame();
        }

        private void InitializeGame()
        {
            // Создаем препятствия
            for (int i = 0; i < 5; i++)
            {
                rocks.Add(new Rock(new PointF(i * 100 + 50, i * 50 + 50)));
            }
        }

        public void AddPlayer(int id)
        {
            var ship = new Ship(id, new PointF(100 + id * 200, 200));
            players[id] = ship;
        }

        public void ProcessCommand(int playerId, string command)
        {
            if (!players.ContainsKey(playerId)) return;

            var player = players[playerId];
            switch (command)
            {
                case "MOVE FORWARD":
                    player.MoveForward();
                    break;
                case "MOVE BACK":
                    player.MoveBack();
                    break;
                case "ROTATE LEFT":
                    player.RotateLeft();
                    break;
                case "ROTATE RIGHT":
                    player.RotateRight();
                    break;
                case "FIRE":
                    var torpedo = player.Fire();
                    torpedoes.Add(torpedo);
                    break;
            }
        }

        public string GetGameState()
        {
            // Собираем состояние игры для отправки клиентам
            string state = "";
            foreach (var player in players.Values)
            {
                state += $"PLAYER {player.Id} {player.Position.X} {player.Position.Y} {player.Health}\n";
            }
            foreach (var torpedo in torpedoes)
            {
                state += $"TORPEDO {torpedo.Position.X} {torpedo.Position.Y}\n";
            }
            return state;
        }
    }
}
