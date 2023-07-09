using System.Windows.Forms;
using static System.Formats.Asn1.AsnWriter;

namespace WinFormsApp1
{

    public partial class Form1 : Form
    {
        Tetris game = new Tetris();
        public Form1()
        {
            InitializeComponent();
        }
        private void button1_Click_1(object sender, EventArgs e)
        {
            game.Init_pole();

            game.scores = 0;
            game.Gamer.gen = true;

            game.Border(pictureBox1);

            GameProcess.Enabled = true;

            Animator.Enabled = true;

            button1.Enabled = false;

            this.Focus();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            pictureBox1.Image = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            Graphics g = Graphics.FromImage(pictureBox1.Image);
            g.Clear(Color.Black);
        }
        private void Animator_Tick(object sender, EventArgs e)
        {
            game.Update_pole(pictureBox1);
            Draw_Figure();

            game.Border(pictureBox1);

            pictureBox1.Refresh();

            label1.Text = game.scores.ToString();
        }
        private void Draw_Figure()
        {
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    if (game.Gamer.Get_cell(i, j) > 0)
                    {
                        SolidBrush brush = new SolidBrush(Color.Yellow);
                        Graphics G = Graphics.FromImage(pictureBox1.Image);

                        G.FillRectangle(brush, (game.Gamer.coord.X + i) * 20 + 20,
                            (game.Gamer.coord.Y + j) * 20, 20, 20);
                    }
                }
            }
        }
        private void GameProcess_Tick(object sender, EventArgs e)
        {
            if (game.Gamer.gen == true)
            {
                game.Gamer.coord.X = 3;
                game.Gamer.coord.Y = 0;

                game.Gamer.generate();
                game.Gamer.gen = false;
            }
            if (game.check_step() == false)
            {
                if (game.Gamer.coord.Y == 0)
                {
                    GameProcess.Enabled = false;
                    Animator.Enabled = false;

                    button1.Enabled = true;
                    game.Draw_end(pictureBox1);
                    pictureBox1.Refresh();
                }
                game.Copy_Figure();
                game.Gamer.gen = true;
                return;
            }
            else
            {
                game.Gamer.coord.Y = game.Gamer.coord.Y + 1;
            }
            game.move_pole();
        }
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            int x;

           if ((e.KeyCode == Keys.Control) | (e.KeyCode == Keys.Shift) | (e.KeyCode == Keys.Up) | (e.KeyCode == Keys.ControlKey) | (e.KeyCode == Keys.ShiftKey))
            {
                game.Gamer.Rotate();
                if (game.check_step() == false)
                {
                    game.Gamer.Rotate();
                    game.Gamer.Rotate();
                    game.Gamer.Rotate();
                }
                if ((game.Gamer.coord.X < 0) & (game.Check_left() != -game.Gamer.coord.X))
                {
                    game.Gamer.Rotate();
                    game.Gamer.Rotate();
                    game.Gamer.Rotate();
                }
                if (game.Gamer.coord.X > (9 - game.Check_right()))
                {
                    game.Gamer.Rotate();
                    game.Gamer.Rotate();
                    game.Gamer.Rotate();
                }
            }
            if (e.KeyCode == Keys.Left | e.KeyCode == Keys.A)
            {
                game.Gamer.coord.X = game.Gamer.coord.X - 1;
                if (game.check_step() == false)
                {
                    game.Gamer.coord.X = game.Gamer.coord.X + 1;
                }
                else
                {
                    if (game.Gamer.coord.X < 0)
                    {
                        game.Gamer.coord.X = 0;
                    }
                    x = game.Check_left();
                    if (game.Gamer.coord.X == 0)
                    {
                        game.Gamer.coord.X = -x;
                    }
                }
            }
            if (e.KeyCode == Keys.Right | e.KeyCode == Keys.D)
            {
                game.Gamer.coord.X = game.Gamer.coord.X + 1;
                if (game.check_step() == false)
                {
                    game.Gamer.coord.X = game.Gamer.coord.X - 1;
                }
                else
                {
                    if (game.Gamer.coord.X + game.Check_right() > 9)
                    {
                        game.Gamer.coord.X = 9 - game.Check_right();
                    }
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void Form1_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {

        }
    }
    //GameProcess ia a timer, which will control figure generation.
    //draws figure on the game field
    public class Tetris
    {
        public int scores = 0;

        int[,] pole = new int[10, 20];

        public Gamer_tetris Gamer = new Gamer_tetris();
        public Tetris()
        {
            Init_pole();
        }
        public void move_pole()
        {
            int x = Check_full_line();
            while (x > -1)
            {
                copy_pole(x);
                x = Check_full_line();
            }
        }
        public void copy_pole(int y)
        {
            for (int i = y - 1; i >= 0; i = - 1)
            {
                for (int j = 0; j < 10; j = j + 1)
                {
                    pole[j, i + 1] = pole[j, i];
                }
            }
            for (int i = 0; i < 10; i++)
            {
                pole[i, 0] = 0;
            }

            scores = scores + 10;
        }
        public int Check_full_line()
        {
            for (int i = 0; i < 20; i++)
            {
                if (Check_Line(i) == true)
                {
                    return i;
                }
            }
            return -1;
        }
        public bool Check_Line(int y)
        {
            int x = 0;
            for (int i = 0; i < 10; i++)
            {
                if (pole[i, y] > 0)
                {
                    x++;
                }
                if (x == 10)
                {
                    return true;
                }
            }
            return false;
        }
        public int Check_right()
        {
            int a;
            for (int i = 3; i >= 0; i--)
            {
                a = Gamer.Get_cell(i, 0) + Gamer.Get_cell(i, 1) + Gamer.Get_cell(i, 2) + Gamer.Get_cell(i, 3);
                if (a > 0)
                {
                    return i;
                }
            }
            return 0;
        }
        public int Check_left()
        {
            int a;
            for (int i = 0; i < 4; i++)
            {
                a = Gamer.Get_cell(i, 0) + Gamer.Get_cell(i, 1) + Gamer.Get_cell(i, 2) + Gamer.Get_cell(i, 3);
                if (a > 0)
                {
                    return i;
                }
            }
            return 3;
        }
        public void Copy_Figure()
        {
            int a, x, y;

            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    a = Gamer.Get_cell(i, j);
                    if (a > 0)
                    {
                        x = Gamer.coord.X + i;
                        y = Gamer.coord.Y + j;
                        if (x < 10 & y < 20)
                        {
                            pole[x, y] = a;
                        }
                    }
                }
            }
        }
        public bool check_step()
        {
            int x = down_level();
            if (x + Gamer.coord.Y > 20)
            {
                return false;
            }
            if (checkPole(1) == true)
            {
                return false;
            }
            return true;
        }
        public bool checkPole(int y)
        {
            int a, b, c;
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    a = Gamer.coord.X + i;
                    b = Gamer.coord.Y + j + y;
                    if (a < 0 | b < 0 | a > 9 | b > 19)
                    {
                        c = 0;
                    }
                    else
                    {
                        c = pole[a, b];
                    }
                    if (c > 0 & Gamer.Get_cell(i, j) > 0)
                    {
                        return true;
                    }
                }
            }
            a = Gamer.coord.Y + down_level();
            if (a >= 18)
            {
                return true;
            }
            return false;
        }
        public int down_level()
        {
            int a = 0;
            for (int i = 0; i < 4; i  = i + 1)
            {
                a = Gamer.Get_cell(0, i) + Gamer.Get_cell(1, i) + Gamer.Get_cell(2, i) + Gamer.Get_cell(3, i);
                if (a > 0)
                {
                    return i;
                }
            }
            return 0;
        }
        public void Draw_end(PictureBox P)
        {
            Graphics G = Graphics.FromImage(P.Image);
            SolidBrush brush = new SolidBrush(Color.Red);
            Font myFont = new Font("Arial", 14);
            G.DrawString("GAME OVER!", myFont, brush, 50, 200);
        }
        public void Draw_Blue(PictureBox P, int x, int y)
        {
            Graphics G = Graphics.FromImage(P.Image);

            Image squad = Properties.Resources.square1;

            G.DrawImage(squad, new Point(x, y));
        }
        public void V_line(PictureBox P, int x)
        {
            int y = 0;
            for (int i = 0; i < 21; i++)
            {
                Draw_Blue(P, x, y);

                y = y + 20;
            }
        }
        public void H_line(PictureBox P, int y)
        {
            int x = 0;
            for (int i = 0; i < 12; i++)
            {
                Draw_Blue(P, x, y);

                x = x + 20;
            }
        }
        public void Border(PictureBox P)
        {
            V_line(P, 0);

            H_line(P, 400);

            V_line(P, 220);

            V_line(P, 0);
        }
        public void Draw_cell(PictureBox P, int x, int y)
        {
            SolidBrush brush = new SolidBrush(Color.Black);

            if (pole[x, y] == 0)
            {
                brush.Color = Color.Black;
            }
            else
            {
                brush.Color = Color.LightGreen;
            }
            Graphics G = Graphics.FromImage(P.Image);
            G.FillRectangle(brush, x * 20 + 20, y * 20, 20, 20);
        }
        public void Init_pole()
        {
            for (int i = 0; i < 10; i++)
            {
                for (int j = 1; j < 20; j++)
                {
                    pole[i, j] = 0;
                }
            }
        }
        public void Update_pole(PictureBox P)
        {
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 20; j++)
                {
                    Draw_cell(P, i, j);
                }
            }
        }
    }
    public class Gamer_tetris
    {
        public bool gen = true;
        int[,] figura = new int[4, 4];
        public Point coord = new Point(3, 0);
        public int Get_cell(int x, int y)
        {
            return figura[x, y];
        }
        public void Rotate()
        {
            int[,] figura2 = new int[4, 4];
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    figura2[i, j] = figura[j, 3 - i];
                }
            }
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    figura[i, j] = figura2[i, j];
                }
            }
        }
        public void clear_figure()
        {
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    figura[i, j] = 0;
                }
            }
        }
        public void generate()
        {
            Random rnd = new Random();
            clear_figure();
            int num = rnd.Next(1, 7);
            switch (num)
            {
                case 1:
                    gen_I();
                    break;
                case 2:
                    gen_J();
                    break;
                case 3:
                    gen_L();
                    break;
                case 4:
                    gen_O();
                    break;
                case 5:
                    gen_S();
                    break;
                case 6:
                    gen_T();
                    break;
                case 7:
                    gen_Z();
                    break;
            }
        }
        public void gen_J()
        {
            figura[1, 2] = 1;
            figura[2, 0] = 1;
            figura[2, 1] = 1;
            figura[2, 2] = 1;
        }
        public void gen_I()
        {
            figura[1, 0] = 1;
            figura[1, 1] = 1;
            figura[1, 2] = 1;
            figura[1, 3] = 1;
        }
        public void gen_L()
        {
            figura[1, 0] = 1;
            figura[1, 1] = 1;
            figura[1, 2] = 1;
            figura[2, 2] = 1;
        }
        public void gen_O()
        {
            figura[1, 0] = 1;
            figura[2, 0] = 1;
            figura[1, 1] = 1;
            figura[2, 1] = 1;
        }
        public void gen_Z()
        {
            figura[0, 0] = 1;
            figura[1, 0] = 1;
            figura[1, 1] = 1;
            figura[2, 1] = 1;
        }
        public void gen_T()
        {
            figura[0, 0] = 1;
            figura[1, 0] = 1;
            figura[2, 0] = 1;
            figura[1, 1] = 1;
        }
        public void gen_S()
        {
            figura[1, 0] = 1;
            figura[2, 0] = 1;
            figura[0, 1] = 1;
            figura[1, 1] = 1;
        }
    }
}