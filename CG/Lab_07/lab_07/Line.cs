using System.Drawing;


namespace lab_07
{
    public partial class Form1
    {
        class Line
        {
            public PointF a, b;

            public Line(PointF a, PointF b)
            {
                this.a = a;
                this.b = b;
            }

            public Line()
            {
                a = PointF.Empty;
                b = PointF.Empty;
            }
        }
    }
}
