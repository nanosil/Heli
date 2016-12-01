using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.ComponentModel;
using System.Threading;
namespace heli
{
    public partial class Form1 : Form
    {
        BufferedGraphics g;
        Rectangle rec = new Rectangle(100, 100, 40, 40), r;
        Rectangle[] walls = new Rectangle[2];
        Random rand = new Random();
        Font f = new Font("Arial", 15);
        Pen p = new Pen(Color.Black);
        bool isDead = false, isStright = false;
        Bitmap back, heli;
        int start = Environment.TickCount, currnet = Environment.TickCount, best = 0,
            count = 0, num = 0, score = 0, frames = 0;
        ComponentResourceManager resources = new ComponentResourceManager(typeof(Form1));
        public Form1()
        {
            InitializeComponent();
            p.Width = 5;
            g = BufferedGraphicsManager.Current.Allocate(this.CreateGraphics(), this.ClientRectangle);
            walls[0] = new Rectangle(this.Width + 100, rand.Next(0, this.Height - 100), 30, 100);
            walls[1] = new Rectangle(this.Width + 300, rand.Next(0, this.Height - 100), 30, 100);
            heli = (Bitmap)resources.GetObject("heli");
            back = (Bitmap)resources.GetObject("background");
        }
        private void Form1_FirstClick(object sender, MouseEventArgs e)
        {
            this.MouseDown -= new MouseEventHandler(Form1_FirstClick);
            this.MouseDown += new MouseEventHandler(Form1_MouseDown);
            this.timer1.Tick -= new EventHandler(timer1_TickFirst);
            this.timer1.Tick += new EventHandler(timer1_Tick);
            timer1.Interval = 70;
        }
        private void timer1_TickFirst(object sender, EventArgs e)
        {
            g.Graphics.DrawImage(back, 0, 0, this.Width, this.Height);
            g.Graphics.DrawString("Click to start!", f, Brushes.Black, 125, 150);
            g.Render();
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            score++;
            frames += 2;
            g.Graphics.DrawImage(back, -frames, 0, this.Width, this.Height);
            g.Graphics.DrawImage(back, -frames + this.Width, 0, this.Width, this.Height);
            if (frames > this.Width)
                frames = 0;
            rec.Y += num;
            g.Graphics.DrawImage(heli, rec);
            for (int x = 0; x < walls.Length; x++)
            {
                walls[x].X -= 10;
                if (walls[x].X < -walls[x].Width)
                {
                    walls[x].X = this.Width;
                    walls[x].Y = rand.Next(0, this.Height - 150);
                    try { timer1.Interval -= 1; }
                    catch (Exception) { timer1.Interval = 1; }
                }
                g.Graphics.FillRectangle(Brushes.Red, walls[x]);
                g.Graphics.DrawRectangle(p, walls[x]);
                if (walls[x].IntersectsWith(rec))
                {
                    isDead = true;
                    r = Rectangle.Intersect(walls[x], rec);
                    goto here;
                }
            }
            if (rec.Y < 0)
            {
                isDead = true;
                r = new Rectangle(120, 0, 10, 10);
                goto here;
            }
            if (rec.Y + (2 * rec.Height) > this.Height)
            {
                isDead = true;
                r = new Rectangle(125, this.Height - 50, 10, 10);
            }
        here:
            g.Graphics.DrawString("Distance: " + score + "\nBest: " + best, f, Brushes.Blue, 0, 0);
            if (isDead)
            {
                this.MouseDown -= new MouseEventHandler(Form1_MouseDown);
                timer1.Enabled = false;
                for (int y = 0; y < 5; y++)
                {
                    Application.DoEvents();
                    Thread.Sleep(200);
                    r.Height += 5;
                    r.Width += 5;
                    r.Y -= 3;
                    r.X -= 3;
                    g.Graphics.FillRectangle(Brushes.Yellow, r);
                    g.Render();
                }
                g.Graphics.DrawString("\tYou Lost!!\n         Your score was: " + score +
                    "\nClick or Press Enter to start again!", f, Brushes.Blue, 40, 125);
                this.MouseDown += new MouseEventHandler(button1_Click);
                this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.Form1_KeyPress);
                if (score > best)
                    best = score;
            }
            g.Render();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            score = 0;
            frames = 0;
            start = Environment.TickCount;
            currnet = Environment.TickCount;
            count = 0;
            heli = (Bitmap)resources.GetObject("heli");
            walls[0] = new Rectangle(this.Width + 100, rand.Next(0, this.Height - 150), 30, 100);
            walls[1] = new Rectangle(this.Width + 300, rand.Next(0, this.Height - 150), 30, 100);
            timer1.Enabled = true;
            isDead = false;
            timer1.Interval = 70;
            rec.Y = 100;
            rec.X = 100;
            this.MouseDown -= new MouseEventHandler(button1_Click);
            this.MouseDown += new MouseEventHandler(Form1_MouseDown);
            this.KeyPress -= new KeyPressEventHandler(Form1_KeyPress);
        }
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left)
            {
                rec.X -= 10;
                return;
            }
            else if (e.KeyCode == Keys.Right)
            {
                rec.X += 10;
                return;
            }
            currnet = Environment.TickCount;
            int time = currnet - start;
            if (time < 400)
            {
                if (time < 150)
                    count = 0;
                count++;
            }
            else
                count = 0;
            if (count > 2)
            {
                heli = (Bitmap)resources.GetObject("heli");
                num = -5;
                isStright = true;
            }
            else
            {
                heli = (Bitmap)resources.GetObject("heli2");
                isStright = false;
                num = -8;
            }
            start = Environment.TickCount;
        }
        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            if (isStright)
                num = 5;
            else
            {
                heli = (Bitmap)resources.GetObject("heli1");
                num = 10;
            }
        }
        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            Form1_KeyDown(new object(), new KeyEventArgs(Keys.A));
        }
        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            Form1_KeyUp(new object(), new KeyEventArgs(Keys.A));
        }

        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == ' ' || e.KeyChar == '\r')
                button1_Click(new object(), EventArgs.Empty);
        }
    }
}