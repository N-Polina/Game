using System;
using System.Drawing;
using System.Windows.Forms;
using Game.Classes;
using SharpDX;
using Rectangle = System.Drawing.Rectangle;

namespace Game
{
    public partial class FormParam : Form
    {
        public ShipDecorator iship = null;
        public int PlayerId { get; private set; } // Добавляем свойство PlayerId

        private readonly SharpDX.Direct2D1.Factory factory;

        public FormParam(SharpDX.Direct2D1.Factory factory, int gamer)
        {
            InitializeComponent();
            this.factory = factory;
            Text = "Выбор корабля: игрок " + gamer;
        }

        private void Set()
        {
            // Создание базового корабля
            Ship baseShip = new Ship(factory, 50, 50, SharpDX.Color.White);

            // Проверяем, какая радиокнопка выбрана, и устанавливаем корабль через фабрику
            if (radioButton1.Checked)
            {
                iship = ShipFactory.GetShipType1(baseShip); // Тип 1 - красный
            }
            else if (radioButton2.Checked)
            {
                iship = ShipFactory.GetShipType2(baseShip); // Тип 2 - зелёный
            }
            else if (radioButton3.Checked)
            {
                iship = ShipFactory.GetShipType3(baseShip); // Тип 3 - синий
            }
            else if (radioButton4.Checked)
            {
                iship = ShipFactory.GetShipType4(baseShip); // Тип 4 - жёлтый
            }
            else if (radioButton5.Checked)
            {
                iship = ShipFactory.GetShipType5(baseShip); // Тип 5 - пурпурный
            }
        }

        private void radioButton5_CheckedChanged(object sender, EventArgs e)
        {
            Set();
            if (iship != null)
            {
                // Обновляем информацию о выбранном корабле
                label1.Text = "При попадании отнимается " + iship.Bron() + " жизней";
                label2.Text = "Скорость движения корабля: " + iship.Speed();
                label3.Text = "Скорость движения торпеды: " + iship.Weapon();
                label4.Text = "Запас хода корабля: " + iship.ZapasHoda();
            }
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            // Если выбран корабль, рисуем его в предварительном просмотре
            if (iship != null)
            {
                var baseShip = (iship as ShipDecorator)?.GetBaseShip();
                if (baseShip != null)
                {
                    // Преобразуем SharpDX.Color в System.Drawing.Color
                    var color = ConvertToDrawingColor(baseShip.ShipColor);
                    e.Graphics.Clear(System.Drawing.Color.White);
                    using (var brush = new SolidBrush(color))
                    {
                        e.Graphics.FillRectangle(brush, new Rectangle(100, 100, 50, 20)); // Примерная визуализация
                    }
                }
            }
        }

        private void FormParam_Load(object sender, EventArgs e)
        {
            radioButton5_CheckedChanged(sender, e);
        }

        // Метод для преобразования SharpDX.Color в System.Drawing.Color
        private System.Drawing.Color ConvertToDrawingColor(SharpDX.Color color)
        {
            return System.Drawing.Color.FromArgb(
                (int)(color.A * 255),
                (int)(color.R * 255),
                (int)(color.G * 255),
                (int)(color.B * 255)
            );
        }
    }
}
