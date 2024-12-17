using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using SharpDX.Direct2D1;
using SharpDX.Windows;
using D2D1 = SharpDX.Direct2D1;
using WIC = SharpDX.WIC;
using Game.Network;
using ChatApp.Network;

namespace Game.Classes
{
    public class GameApp : IDisposable
    {
        private readonly RenderForm pic;
        private bool animateStop = true;

        private readonly List<IDrawable> drawObjects = new List<IDrawable>();
        private readonly List<IMoveable> moveableObjects = new List<IMoveable>();

        private readonly IShip[] ships = new IShip[2];
        private readonly List<Torpedo> torpedos = new List<Torpedo>();
        private readonly List<Rock> rocks = new List<Rock>();

        private int hod = 0;

        private readonly PointF[] allArea;
        private readonly D2D1.Factory factory;

        private D2D1.Bitmap backgroundBitmap;
        private WIC.ImagingFactory imagingFactory;
        private GameClient networkClient;


        private GameClient gameClient;

        public bool IsReady { get; private set; } = false;
        private readonly SharpDX.Direct2D1.Factory d2dFactory;
        private readonly SharpDX.Direct2D1.WindowRenderTarget d2dRenderTarget;

        private bool shipSelectionInProgress = false; // Для предотвращения повторного открытия формы
        private bool shipSelectionCompleted = false; // Указывает, завершён ли выбор



        public GameApp(D2D1.Factory factory, RenderForm pic, WindowRenderTarget renderTarget, int playerId)
        {
            this.factory = factory;
            this.pic = pic;
            this.d2dRenderTarget = renderTarget;
            this.playerId = playerId;

            imagingFactory = new WIC.ImagingFactory();
            LoadBackground(renderTarget, "D:/универ/4 курс/7 семестр/KyrsahPSP/Game/Assets/background.png");

            var b = GameConsts.AREA_BORDER;

            allArea = new PointF[]
            {
                new PointF(b, b),
                new PointF(pic.Width - b, b),
                new PointF(pic.Width - b, pic.Height - b),
                new PointF(b, pic.Height - b),
            };

            GenerateRock();
        }

        
        public void KeyDown(int keyValue)
        {
            switch (keyValue)
            {
                case 38: // Вперёд перемещение.
                    ships[hod].MoveForward();
                    ChangeHod();
                    break;

                case 40: // Назад перемещение.
                    ships[hod].MoveBack();
                    ChangeHod();
                    break;

                case 39: // Вправо вращение.
                    ships[hod].RotateRight();
                    ChangeHod();
                    break;

                case 37: // Влево вращение.
                    ships[hod].RotateLeft();
                    ChangeHod();
                    break;

                case 32: // Выстрел.
                    var torpedo = ships[hod].Fire();
                    drawObjects.Add(torpedo);
                    moveableObjects.Add(torpedo);
                    torpedos.Add(torpedo);
                    ChangeHod();
                    break;
            }
        }
        private void LoadBackground(D2D1.WindowRenderTarget renderTarget, string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException($"Файл {filePath} не найден!");

            using (var bitmapDecoder = new WIC.BitmapDecoder(imagingFactory, filePath, WIC.DecodeOptions.CacheOnDemand))
            {
                using (var formatConverter = new WIC.FormatConverter(imagingFactory))
                {
                    formatConverter.Initialize(
                        bitmapDecoder.GetFrame(0),
                        WIC.PixelFormat.Format32bppPRGBA);

                    backgroundBitmap = D2D1.Bitmap.FromWicBitmap(renderTarget, formatConverter);
                }
            }
        }

        
        public void Draw(D2D1.WindowRenderTarget windowRenderTarget)
        {
            if (backgroundBitmap != null)
            {
                windowRenderTarget.DrawBitmap(
                    backgroundBitmap,
                    new SharpDX.RectangleF(0, 0, pic.Width, pic.Height),
                    1.0f,
                    D2D1.BitmapInterpolationMode.Linear
                );
            }
            drawObjects.ForEach(obj => obj.Draw(windowRenderTarget));
        }

        private bool isShipSelectionInProgress = false;

        public void ShowShipSelectionForm()
        {
            if (isShipSelectionInProgress) return; // Если форма уже открыта, выходим
            isShipSelectionInProgress = true;

            using (var form = new FormParam(factory, playerId))
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    int selectedShip = form.SelectedShip;
                    networkClient.SendMessage($"SELECT_SHIP {playerId} {selectedShip}");
                    Console.WriteLine($"Выбран корабль: {selectedShip}");
                }
                else
                {
                    Console.WriteLine("Выбор корабля отменён.");
                }
            }

            isShipSelectionInProgress = false; // Завершение выбора
        }


        public string GetScore()
        {
            return $"{ships[0].GetLife()} : {ships[1].GetLife()}";
        }

        private void GenerateRock()
        {
            for (int i = 0; i < 5; i++)
            {
                Rock rock;
                bool can;
                do
                {
                    var point = GenerateCoord(GameConsts.ROCK_LENGTH, GameConsts.ROCK_SIZE);
                    rock = new Rock(factory, point.X, point.Y);
                    can = !CrossAny(rock);
                } while (!can);

                rocks.Add(rock);
                drawObjects.Add(rock);
            }
        }

        private bool CrossAny(IDrawable obj)
        {
            foreach (var t in drawObjects)
            {
                if (!t.Equals(obj) && Geometry.AnyInside(obj.GetPolygon(), t.GetPolygon()))
                {
                    return true;
                }
            }
            return false;
        }

        private PointF GenerateCoord(float widthObject, float heightObject)
        {
            var rand = new Random();
            var max = Math.Max(widthObject, heightObject);
            var x = rand.Next() % (pic.Width - 2 * max - 2 * GameConsts.AREA_BORDER) + GameConsts.AREA_BORDER + max;
            var y = rand.Next() % (pic.Height - 2 * max - 2 * GameConsts.AREA_BORDER) + GameConsts.AREA_BORDER + max;
            return new PointF(x, y);
        }

        public void SetShip(int gamer, IShip iship)
        {
            ships[gamer] = iship;
            drawObjects.Add(iship);
            moveableObjects.Add(iship);

            bool can;
            do
            {
                var point = GenerateCoord(GameConsts.SHIP_LENGTH, GameConsts.SHIP_SIZE);
                iship.SetX(point.X);
                iship.SetY(point.Y);
                can = !CrossAny(iship);
            }
            while (!can);
        }

        public void ConnectToServer(string ipAddress, int port)
        {
            networkClient = new GameClient(factory, playerId, this); // Передаём текущий объект GameApp

            try
            {
                networkClient.Connect(ipAddress, port);
                networkClient.OnMessageReceived += ProcessServerMessage;
                Console.WriteLine($"Подключено к серверу. Игрок {playerId}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка подключения: {ex.Message}");
                MessageBox.Show("Не удалось подключиться к серверу.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void ProcessServerMessage(string message)
        {
            Console.WriteLine($"Получено сообщение от сервера: {message}");

            if (message == "START_SELECT_SHIP" && !shipSelectionCompleted)
            {
                StartShipSelection();
            }
            else if (message == "WAIT_OTHER_PLAYER")
            {
                ShowWaitingMessage("Ожидаем второго игрока...");
            }
            else if (message == "START_GAME")
            {
                StartGame();
            }
            else
            {
                Console.WriteLine($"Необработанное сообщение от сервера: {message}");
            }
        }

        private void ShowWaitingMessage(string message)
        {
            using (var waitingForm = new Form())
            {
                waitingForm.Text = "Ожидание";
                waitingForm.Width = 300;
                waitingForm.Height = 100;
                waitingForm.StartPosition = FormStartPosition.CenterScreen;

                var label = new Label
                {
                    Text = message,
                    Dock = DockStyle.Fill,
                    TextAlign = ContentAlignment.MiddleCenter
                };

                waitingForm.Controls.Add(label);

                var timer = new System.Windows.Forms.Timer { Interval = 3000 }; // Закрыть через 3 секунды
                timer.Tick += (s, e) =>
                {
                    timer.Stop();
                    waitingForm.Close();
                };

                timer.Start();
                waitingForm.ShowDialog();
            }
        }


        public void StartGame() // Вставляем здесь
        {
            IsReady = true;
            StartAnimation();
        }

        public void StartAnimation() => animateStop = false;
        public void StopAnimation() => animateStop = true;

        public IShip GetShip(int gamer) => ships[gamer];

        public void ChangeHod() => hod = 1 - hod;
        public int GetHod() => hod;

        public void Animate()
        {
            if (!animateStop)
            {
                moveableObjects.ForEach(obj =>
                {
                    if (!Geometry.Inside(allArea, obj.GetNos()))
                        obj.Stop();
                });

                foreach (var ship in ships)
                {
                    foreach (var rock in rocks)
                    {
                        if (Geometry.Inside(rock.GetPolygon(), ship.GetNos()))
                            ship.Stop();
                    }
                }

                moveableObjects.ForEach(obj =>
                {
                    obj.Move();
                    obj.Rotate();
                });

                torpedos.RemoveAll(t =>
                {
                    if (!Geometry.Inside(allArea, t.GetNos()))
                    {
                        drawObjects.Remove(t);
                        moveableObjects.Remove(t);
                        return true;
                    }
                    return false;
                });

                torpedos.RemoveAll(t =>
                {
                    foreach (var rock in rocks)
                    {
                        if (Geometry.AnyInside(t.GetPolygon(), rock.GetPolygon()))
                        {
                            drawObjects.Remove(t);
                            moveableObjects.Remove(t);
                            return true;
                        }
                    }
                    return false;
                });

                foreach (var torpedo in new List<Torpedo>(torpedos))
                {
                    if (!torpedo.GetShip().Equals(ships[0]) && Geometry.Inside(ships[0].GetPolygon(), torpedo.GetNos()))
                    {
                        ships[0].MinusLife();
                        drawObjects.Remove(torpedo);
                        moveableObjects.Remove(torpedo);
                        torpedos.Remove(torpedo);
                    }
                    else if (!torpedo.GetShip().Equals(ships[1]) && Geometry.Inside(ships[1].GetPolygon(), torpedo.GetNos()))
                    {
                        ships[1].MinusLife();
                        drawObjects.Remove(torpedo);
                        moveableObjects.Remove(torpedo);
                        torpedos.Remove(torpedo);
                    }
                }

                if (GaveOver())
                {
                    MessageBox.Show($"Игра окончена. {(ships[1].GetLife() > 0 ? "2" : "1")} игрок выиграл!");
                    StopAnimation();
                }

                Thread.Sleep(10);
            }
        }


        // --- Сетевые функции --- //
        private int playerId;

        public void Dispose()
        {
            backgroundBitmap?.Dispose();
            imagingFactory?.Dispose();
            d2dRenderTarget?.Dispose();
            networkClient?.Disconnect();
        }

        public bool GaveOver() => ships[0].GetLife() < 1 || ships[1].GetLife() < 1;

        private void HandleUpdateMessage(string message)
        {
            try
            {
                var parts = message.Split(' ');
                if (parts[1] == "PLAYER")
                {
                    int playerIndex = int.Parse(parts[2]);
                    float x = float.Parse(parts[4].Split(':')[1]);
                    float y = float.Parse(parts[5].Split(':')[1]);
                    float life = float.Parse(parts[6].Split(':')[1]);

                    var player = GetShip(playerIndex);
                    player.SetX(x);
                    player.SetY(y);
                    while (player.GetLife() > life) player.MinusLife();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка обработки сообщения: {ex.Message}");
            }
        }

        public void SendCommand(string command)
        {
            try
            {
                gameClient.SendMessage(command);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка отправки команды: {ex.Message}");
            }
        }

        public void SetReady()
        {
            IsReady = true;
        }

        public void StartShipSelection()
        {
            if (!shipSelectionInProgress && !shipSelectionCompleted)
            {
                shipSelectionInProgress = true;
                using (var form = new FormParam(factory, playerId))
                {
                    if (form.ShowDialog() == DialogResult.OK && form.iship != null)
                    {
                        var selectedShip = form.iship.GetType().Name;
                        SendCommand($"SELECT_SHIP {playerId} {selectedShip}");
                        Console.WriteLine($"Игрок {playerId} выбрал корабль: {selectedShip}");
                        shipSelectionCompleted = true; // Завершаем выбор
                    }
                    else
                    {
                        MessageBox.Show("Выбор корабля отменён.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                shipSelectionInProgress = false;
            }
        }

    }
}
