using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;


namespace lab_07
{
    public partial class Form1 : Form
    {
        List<Line> lines;
        Line last_line;
        Cutter cutter;

        Bitmap saved_picture;
        Graphics g;
        Graphics g_move;

        Pen pen_cutter;
        Pen pen_lines;
        Pen pen_choosen;

        public Form1()
        {
            InitializeComponent();

            lines = new List<Line>();
            lines.Add(new Line());
            last_line = lines[0];
            cutter = new Cutter();

            saved_picture = new Bitmap(canvasBase.Width, canvasBase.Height);
            g = Graphics.FromImage(saved_picture);
            g_move = canvasBase.CreateGraphics();
            canvasBase.Image = saved_picture;

            pen_cutter = new Pen(Color.Black, 1);
            pen_lines = new Pen(Color.Red, 1);
            pen_choosen = new Pen(Color.Blue, 1);
        }

        static void Swap<T>(ref T a, ref T b)
        {
            T temp = a;
            a = b;
            b = temp;
        }

        // Проверяет, используюя коды двух точек, пересечение бесконечной стороны и линии
        bool Intersect(int T1, int T2, int side)
        {
            int coord = (int)Math.Pow(2, 3 - side);

            return (T1 & coord) != (T2 & coord);
        }

        // Алгоритм Коэна — Сазерленда для отрезка
        void CohenSutherland(PointF a, PointF b)
        {
            int orientation_flag = 0; // общего
            float slope = 1; // наклон

            if (b.X - a.X == 0)
                orientation_flag = -1; // вертикальный
            else
            {
                slope = (b.Y - a.Y) / (b.X - a.X);
                if (slope == 0)
                    orientation_flag = 1; // горизонтальный
            }

            for (int i = 0; i < 4; i++)
            {
                // Находим коды
                int SumA = cutter.GetCode(a);
                int SumB = cutter.GetCode(b);
                
                int visible = cutter.IsVisible(a, b);
                if (visible == 1) // Тривиальная видимость
                {
                    g.DrawLine(pen_choosen, a, b);
                    return;
                }
                else if (visible == 0) // Тривиальная невидимость
                    return;

                // Проверяем пересечение отрезка и стороны окна
                if (!Intersect(SumA, SumB, i))
                    continue;

                // Если точка а внутри стороны
                if ((SumA & (int)Math.Pow(2, 3 - i)) == 0)
                    Swap(ref a, ref b);

                // Поиск пересечений отрезка со стороной
                if (orientation_flag != -1) // не вертикальный
                {
                    if (i < 2) // если рассматриваем левую или правую сторону отсекателя
                    {
                        a.Y = slope * (cutter[i] - a.X) + a.Y;
                        a.X = cutter[i];
                        continue;
                    }
                    else
                        a.X = (1 / slope) * (cutter[i] - a.Y) + a.X;
                }
                a.Y = cutter[i];
            }
            g.DrawLine(pen_choosen, a, b);
        }

        private void buttonGetCutter_Click(object sender, EventArgs e)
        {
            lines.Clear();
            lines.Add(new Line());
            last_line = lines[0];

            g.Clear(Color.White);
            canvasBase.Refresh();

            try
            {
                cutter.left = int.Parse(textBoxLeft.Text);
                cutter.right = int.Parse(textBoxRight.Text);
                cutter.down = int.Parse(textBoxDown.Text);
                cutter.up = int.Parse(textBoxUp.Text);

                for (int i = 0; i < 4; i++)
                {
                    int point_border = i < 2 ? canvasBase.Width : canvasBase.Height;
                    if (cutter[i] < 0 || cutter[i] > point_border)
                        throw new Exception("Границы должны быть больше нуля и меньше длины/ширины полотна");
                }

                g.DrawRectangle(pen_cutter, cutter.left, cutter.up, cutter.right - cutter.left, cutter.down - cutter.up);
                canvasBase.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка ввода отсекателя");
            }
        }

        private void canvasBase_MouseClick(object sender, MouseEventArgs e)
        {
            PointF mousePos = new PointF(e.X, e.Y);

            if (e.Button == MouseButtons.Left)
            {
                if (last_line.a.IsEmpty)
                    last_line.a = mousePos;
                else
                {
                    if (ModifierKeys == Keys.Control) // горизонтальная линия
                        last_line.b = new PointF(mousePos.X, last_line.a.Y);
                    else if (ModifierKeys == Keys.Alt) // вертикальная линия
                        last_line.b = new PointF(last_line.a.X, mousePos.Y);
                    else
                        last_line.b = mousePos;

                    g.DrawLine(pen_lines, last_line.a, last_line.b);
                    canvasBase.Refresh();
                    lines.Add(new Line());
                    last_line = lines[lines.Count() - 1];
                }
            }
        }


        private void buttonClear_Click(object sender, EventArgs e)
        {
            lines.Clear();
            lines.Add(new Line());
            last_line = lines[0];

            g.Clear(Color.White);
            canvasBase.Refresh();
        }


        private void buttonCut_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < lines.Count() -1; i++)
            {
                CohenSutherland(lines[i].a, lines[i].b);
                canvasBase.Refresh();
            }
            canvasBase.Refresh();
        }

        private void canvasBase_MouseMove(object sender, MouseEventArgs e)
        {
            labelLocation.Text = "Положение курсора: " + canvasBase.PointToClient(MousePosition).ToString();
            canvasBase.Refresh();

            if (last_line.b.IsEmpty && !last_line.a.IsEmpty)
            {
                PointF a = last_line.a;
                PointF b = canvasBase.PointToClient(MousePosition);

                if (ModifierKeys == Keys.Control)
                    b.Y = a.Y;
                else if (ModifierKeys == Keys.Alt)
                    b.X = a.X;

                g_move.DrawLine(pen_lines, a, b);
            }
        }

        private void buttonInfo_Click(object sender, EventArgs e)
        {

        }
    }
}
