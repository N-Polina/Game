using System;
using System.Windows.Forms;
using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SharpDX.Windows;
using AlphaMode = SharpDX.Direct2D1.AlphaMode;
using Device = SharpDX.Direct3D11.Device;
using Game.Classes;
using SharpDX.Direct3D;

namespace Game
{
    static class Program
    {
        private static Device device;
        private static RenderForm renderForm;
        private static SwapChain swapChain;
        private static GameApp gameApp = null;
        private static SharpDX.Direct2D1.Factory d2dFactory = new SharpDX.Direct2D1.Factory();
        private static WindowRenderTarget d2dRenderTarget;
        private static bool exitGame = false;
        private static int playerId; // Переменная для ID игрока

        private static void FormKeyDown(object sender, KeyEventArgs e)
        {
            gameApp?.KeyDown(e.KeyValue);
        }

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // 1. Показываем форму подключения
            using (var connectionForm = new ConnectionForm())
            {
                if (connectionForm.ShowDialog() != DialogResult.OK)
                {
                    MessageBox.Show("Подключение отменено.", "Информация");
                    return;
                }

                playerId = connectionForm.PlayerId; // Используем правильное имя свойства
            }

            // 2. Настраиваем RenderForm
            renderForm = new RenderForm("Морской бой")
            {
                Width = 800,
                Height = 600
            };
            renderForm.KeyDown += FormKeyDown;

            // 3. Создаем DirectX рендеринг
            SetupDirectX();

            // 4. Создаем GameApp
            gameApp = new GameApp(d2dFactory, renderForm, d2dRenderTarget, playerId);

            // 5. Показываем выбор корабля
            using (var formParam = new FormParam(d2dFactory, playerId))
            {
                if (formParam.ShowDialog() == DialogResult.OK)
                {
                    gameApp.SetShip(playerId - 1, formParam.iship);
                }
                else
                {
                    MessageBox.Show("Выбор корабля отменён.", "Ошибка");
                    return;
                }
            }

            // 6. Запускаем игровой рендер-цикл
            RunGameLoop();
        }

        private static void SetupDirectX()
        {
            var swapChainDesc = new SwapChainDescription
            {
                BufferCount = 1,
                ModeDescription = new ModeDescription(renderForm.ClientSize.Width, renderForm.ClientSize.Height,
                    new Rational(60, 1), Format.R8G8B8A8_UNorm),
                IsWindowed = true,
                OutputHandle = renderForm.Handle,
                SampleDescription = new SampleDescription(1, 0),
                SwapEffect = SwapEffect.Discard,
                Usage = Usage.RenderTargetOutput
            };

            Device.CreateWithSwapChain(DriverType.Hardware, DeviceCreationFlags.None, swapChainDesc, out device, out swapChain);

            var backBuffer = Texture2D.FromSwapChain<Texture2D>(swapChain, 0);
            var renderView = new RenderTargetView(device, backBuffer);

            var renderTargetProperties = new RenderTargetProperties
            {
                PixelFormat = new PixelFormat(Format.B8G8R8A8_UNorm, AlphaMode.Ignore)
            };

            var hwndRenderTargetProperties = new HwndRenderTargetProperties
            {
                Hwnd = renderForm.Handle,
                PixelSize = new SharpDX.Size2(renderForm.ClientSize.Width, renderForm.ClientSize.Height),
                PresentOptions = PresentOptions.Immediately
            };

            d2dRenderTarget = new WindowRenderTarget(d2dFactory, renderTargetProperties, hwndRenderTargetProperties);
        }

        private static void RunGameLoop()
        {
            RenderLoop.Run(renderForm, () =>
            {
                if (gameApp != null)
                {
                    d2dRenderTarget.BeginDraw();
                    d2dRenderTarget.Clear(SharpDX.Color.CornflowerBlue);

                    gameApp.Draw(d2dRenderTarget);
                    gameApp.Animate();

                    d2dRenderTarget.EndDraw();

                    renderForm.Text = $"Морской бой - Ход: {gameApp.GetHod() + 1} | Счёт: {gameApp.GetScore()}";

                    if (gameApp.GaveOver())
                    {
                        MessageBox.Show("Игра окончена!", "Результат");
                        exitGame = true;
                    }
                }

                if (exitGame)
                {
                    renderForm.Close();
                }
            });
        }
    }
}
