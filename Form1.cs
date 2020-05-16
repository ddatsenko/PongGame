using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PongGame
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            this.KeyPreview = true;                                         // Задамо глобальні змінні:
        }
        const int limit_Pad = 170;                                          // нижня границя для платформи гравця
        const int limit_Ball = 245;                                         // нижня границя м'ячика перед відбиванням
        const int x = 227, y = 120;

        int computer_won = 0;                                               // змінні, в яких зберігається рахунок
        int player_won = 0;                                                 // гравця та комп'ютера

        int speed_Top;                                                      // встановлення напрямку руху кулі
        int speed_Left;

        bool up = false;                                                    // булеві змінні, які запобігають руху
        bool down = false;                                                  // платформи гравця до початку гри
        bool game = false;

        Random r = new Random();
        private void Pressed(object sender, KeyEventArgs e)
        {
            if (game)                                                       // Спочатку була створена структура руху
            {                                                               // платформи гравця. Для виконання цієї цілі
                if (e.KeyCode == Keys.Up || e.KeyCode == Keys.W)            // мені знадобилися два методи: Pressed, який
                {
                    up = true;                                              // визначає чи натиснута клавіша W/↑ або S/↑ за
                }                                                           // допомогою активації таймера, який пересуває платформу
                else if (e.KeyCode == Keys.Down || e.KeyCode == Keys.S)     // на 3 одиниці вгору/вниз. Released визначає коли клавіша
                {                                                           // була відпущена і зупиняє таймер, що контролює платформу.
                    down = true;
                }
                timer1.Start();
            }
        }
        private void Released(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Up || e.KeyCode == Keys.W)
            {
                up = false;
            }
            else if (e.KeyCode == Keys.Up || e.KeyCode == Keys.S)
            {
                down = false;
            }
            timer1.Stop();
        }
        private void MovePaddle(object sender, EventArgs e)
        {
            if (up && Player.Location.Y > 0)
            {
                Player.Top -= 3; 
            }
            else if (down && Player.Location.Y < limit_Pad)
            {
                Player.Top += 3;
            }
        }
        private void Computer(object sender, EventArgs e)                   // Рух платформи комп'ютера
        {
            if (PC.Location.Y <= 0)                                         // перший блок if-else if обмежує рух
            {                                                               // платформи зверху і знизу, а другий
                PC.Location = new Point(PC.Location.X, 0);                  // слідкує за рухом м'ячика незалежно
            }                                                               // від того, чи м'ячик знаходиться нижче
            else if (PC.Location.Y >= limit_Pad)                            // чи вище середини платформи комп'ютера
            {
                PC.Location = new Point(PC.Location.X, limit_Pad);
            }
            if (Ball.Location.Y < PC.Top + (PC.Height / 2))
            {
                PC.Top -= 3;
            }
            else if (Ball.Location.Y > PC.Top + (PC.Height / 2))
            {
                PC.Top += 3;
            }
        }
        private void StartValues()                                          // Рух м'ячика
        {
            speed_Top = 0;                                                  // Спочатку задано значення напрямку
            speed_Left = -5;                                                // м'ячика. Після початку гри м'ячик
        }                                                                   // рухається до гравця. Для цього спочатку
        private void BallMoves()                                            // були задані змінні, а BallMoves власне
        {                                                                   // змушує м'ячик рухатися.
            Ball.Top += speed_Top;
            Ball.Left += speed_Left;
        }
        private void HitBorder()                                            // Вертикальне та горизонтальне обмеження
        {                                                                   // руху м'ячика
            if (Ball.Location.Y <= 0 || Ball.Location.Y >= limit_Ball)
            {                                                               // Вертикально: Коли м'ячик відбивається від "стелі",
                speed_Top *= -1;                                            // він рухається у протилежному напрямку.
            }                                                               // Отже, для того, щоб він відбивався незалежно
        }                                                                   // від того, верхня це чи нижня границя, домножимо speed_Top на -1
        private void BallLeftField()
        {
            if (player_won == 10 || computer_won == 10)
            {
                EndGame();
            }

            if (Ball.Location.X < 0)
            {
                NewPoint(5);
                ComputerWon();
            }
            else if (Ball.Location.X > this.ClientSize.Width)
            {
                NewPoint(-5);
                PlayerWon();
            }
        }
        private void PlayerWon()                                // у цих методах додаються очки,
        {                                                       // які отримує гравець/комп'ютер, коли
            player_won++;                                       // хтось із них не встигає відбити м'ячик
            label1.Text = player_won.ToString();
        }
        private void ComputerWon()
        {
            computer_won++;
            label3.Text = computer_won.ToString();
        }
        private void MoveBall(object sender, EventArgs e)                               
        {
            if (Ball.Bounds.IntersectsWith(Player.Bounds))
            {
                Collision(Player);
            }
            else if (Ball.Bounds.IntersectsWith(PC.Bounds))
            {
                Collision(PC);
            }
            HitBorder();
            BallLeftField();
            BallMoves();
        }
        private void Collision(PictureBox Paddle)                       // процедура зіткнення м'ячика
        {                                                               // з платформою
            switch (true)
            {                                                           // Із світч-стейтментів які перевіряють,
                case true when Upper(Paddle):                           // в якій частині платформи м'ячик зіткнувся
                    speed_Top = Negative(4, 6);                         // з нею, передаючи м'ячику належні координати
                    speed_Left = AdjustCoordinates(5, 6);               // для руху в іншу сторону
                    break;
                case true when High(Paddle):
                    speed_Top = Negative(2, 3);
                    speed_Left = AdjustCoordinates(6, 7);
                    break;
                case true when Middle(Paddle):
                    speed_Top = 0;
                    speed_Left = AdjustCoordinates(5, 5);
                    break;
                case true when Low(Paddle):
                    speed_Top = r.Next(2, 3);
                    speed_Left = AdjustCoordinates(6, 7);
                    break;
                case true when Bot(Paddle):
                    speed_Top = r.Next(4, 6);
                    speed_Left = AdjustCoordinates(5, 6);
                    break;                   
            }
            Edge(); 
        }
        private int Negative(int i, int n)                              // Ми маємо знати які саме координати треба
        {                                                               // передати м'ячику в залежності від того, від
            int myval = r.Next(i, n) * -1;                              // якої платформи він відбився. В цій функції
            return myval;                                               // повертається від'ємне рандомне значення будь-якого числа
        }
        private int AdjustCoordinates(int i, int n)                     // Потім в залежності від позиції м'ячика (вище чи нижче
        {                                                               // середини форми), платформа відіб'є м'ячик у протилежну
            int res = 0;                                                // сторону надаючи змінній speed_Left додатнє чи
                                                                        // від'ємне значення
            if (Ball.Location.X < this.Width / 2)
            {
                res = r.Next(i, n);
            }
            else if (Ball.Location.X > this.Width / 2)
            {
                res = Negative(i, n);
            }
            return res;
        }

        private bool Upper(PictureBox Pad)                                                                          // Однією з найважливіших частин руху м'ячика
        {                                                                                                           // є визначення місця, де саме м'ячик зіткнувся
            return Ball.Location.Y >= Pad.Top - Ball.Height && Ball.Location.Y <= Pad.Top + Ball.Height;            // з платформою. Плафторма поділена на 5 частин 
        }                                                                                                           // і було створено п'ять булевих методів
        private bool High(PictureBox Pad)
        {
            return Ball.Location.Y > Pad.Top + Ball.Height && Ball.Location.Y <= Pad.Top + 2 * Ball.Height;
        }
        private bool Middle(PictureBox Pad)
        {
            return Ball.Location.Y > Pad.Top + 2 * Ball.Height && Ball.Location.Y <= Pad.Top + 3 * Ball.Height;
        }
        private bool Low(PictureBox Pad)
        {
            return Ball.Location.Y > Pad.Top + 3 * Ball.Height && Ball.Location.Y <= Pad.Top + 4 * Ball.Height;
        }
        private bool Bot(PictureBox Pad)
        {
            return Ball.Location.Y > Pad.Top + 4 * Ball.Height && Ball.Location.Y <= Pad.Bottom + Ball.Height;
        }

        
        private void Edge()                                                     // Під час тестування гри я помітив, що
        {                                                                       // під час стикання м'ячика з краями платформи
            if (Ball.Location.X < this.Width / 2)                               // прямо перед тим, як він покине ігрове поле,
            {                                                                   // він може відбитися і гравець не отримає свій
                if (Ball.Location.X < 0 + Ball.Height / 3)                      // бал, що суперечить логіці гри. В цьому методі
                {                                                               // такий випадок виключається і м'яч все одно
                    speed_Left *= -1;                                           // покине ігрове поле навіть тоді, коли
                }                                                               // він зіткнеться з краєм однієї з платформ,
            }                                                                   // а не відіб'ється у протилежний бік
            else if (Ball.Location.X > this.Width / 2)
            {
                if (Ball.Location.X > PC.Location.X + (Ball.Width /3))
                {
                    speed_Left *= -1;
                }
            }
        }
        private void button1_Click(object sender, EventArgs e)  // натискання кнопки "Start Game" запускає
        {                                                       // таймери і ховає цю кнопку
            StartValues(); 
            game = true;
            button1.Visible = false;
            timer1.Start();
            timer2.Start();
            timer3.Start(); 
        }

        private void EndGame()                                  // Переможцем стає той гравець, який першим
        {                                                       // набрав 10 очок. Після цього всі змінні мають
            Player.Location = new Point(0, 75);                 // бути приведені до початкового стану.
            PC.Location = new Point(454, 75);
            game = false;
            player_won = 0;
            computer_won = 0;
            label1.Text = player_won.ToString();
            label3.Text = computer_won.ToString();
            timer1.Stop();
            timer2.Stop();
            timer3.Stop();
            button1.Visible = true; 
        }
        private void NewPoint(int n)                            // Після завершення гри треба поставити м'ячик
        {                                                       // там, де він був, тобто у центрі, а програвший
            Ball.Location = new Point(x, y);                    // гравець подає м'ячик
            speed_Top = 0;
            speed_Left = n;
        }
    }
}
