using System.Collections.Generic;
using System.Drawing;
using System.Reflection.Emit;
using System.Runtime.Intrinsics.X86;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;


namespace hm7
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }


        Bitmap b,b2;
        Graphics g,g2;
        Random r = new Random();
        Pen Pen = new Pen(Color.Blue, 1);
        double lambda=0.0;

        private void button1_Click(object sender, EventArgs e)
        {
            b = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            g = Graphics.FromImage(b);
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            g.Clear(Color.White);


            lambda=(int)trackBar1.Value;
            int numberoftrial = 100;
            double successProbability = lambda / (double)numberoftrial;
            int TrajectoryNumber = 30;

            double minX = 0;
            double maxX = (double)numberoftrial;
            double minY = 0;
            double maxY = (double)numberoftrial;

            Rectangle rect = new Rectangle(20, 20, b.Width - 300, b.Height - 40);
            g.DrawRectangle(Pens.Black, rect);

            Dictionary<int, int> interarrivalDist = new Dictionary<int, int>();
            List<Point> lasttrial = new List<Point>();

            int interarrival = 0;

            for (int i = 0; i < TrajectoryNumber; i++)
            {
                List<Point> points = new List<Point>();
                
                double Yt = 0.0;
                for (int X = 0; X < numberoftrial; X++)
                {
                    double uniform = r.NextDouble();

                    if (uniform < successProbability)
                    {
                        Yt = Yt + 1; ;
                        if (interarrival != 0) {
                            if (!interarrivalDist.ContainsKey(interarrival))
                            {
                                interarrivalDist.Add(interarrival, 1);
                            }
                            else
                            {
                                interarrivalDist[interarrival]++;
                            }
                            interarrival = 0;
                        }
                    }
                    else
                    {
                        interarrival++;
                    }

                    int xCord = FromXrealToXVirtual(X, minX, maxX, rect.Left, rect.Width);
                    int yCord = FromYRealToYVirtual(Yt, minY, maxY, rect.Top, rect.Height);

                    Point p = new Point(xCord, yCord);
                    points.Add(p);

                    if (X == numberoftrial - 1)
                    {
                        lasttrial.Add(p);
                    }
                }
                g.DrawLines(Pen, points.ToArray());
            }

            int min_y = lasttrial.Min(p => p.Y);
            int max_y = lasttrial.Max(p => p.Y);

            Rectangle r2 = new Rectangle(rect.Right + 10, 20, 260, b.Height - 40);
            g.DrawRectangle(Pens.Black, r2);

            if (TrajectoryNumber == 1)
            {
                foreach (Point p in lasttrial)
                {
                    Rectangle re = new Rectangle(r2.Left + 10, p.Y - 10, r2.Right - r2.Left - 20, 20);
                    g.FillRectangle(Brushes.Green, re);
                    g.DrawRectangle(Pens.White, re);
                }
            }
            else
            {
                int intervals_number = TrajectoryNumber / 2;
                if (intervals_number > 15)
                {
                    intervals_number = 15;
                }

                int span = max_y - min_y;
                int interval_size = (max_y - min_y) / intervals_number;

                while (min_y + interval_size * intervals_number < max_y + 1)
                {
                    intervals_number++;
                }

                int minimo = min_y;

                Dictionary<Interval, int> intervalli = new Dictionary<Interval, int>();

                for (int j = 0; j < intervals_number; j++)
                {
                    Interval intervallo = new Interval(minimo, minimo + interval_size);
                    minimo = minimo + interval_size;
                    intervalli[intervallo] = 0;
                }

                foreach (Point p in lasttrial)
                {
                    List<Interval> inter = intervalli.Keys.ToList();
                    int intY = (int)p.Y;

                    foreach (Interval i in inter)
                    {
                        if (intY >= i.down && intY < i.up)
                        {
                            intervalli[i]++;
                        }
                    }
                }

                List<Rectangle> chart = new List<Rectangle>();

                int dimdisp = r2.Right - r2.Left - 20;
                int maxValue = intervalli.Values.Max();

                foreach (var v in intervalli)
                {
                    double intensity = ((double)v.Value / (double)maxValue) * dimdisp;
                    Rectangle rect1 = new Rectangle(r2.Left + 10, v.Key.down, (int)intensity, interval_size);
                    chart.Add(rect1);
                }

                foreach (Rectangle re in chart)
                {
                    g.FillRectangle(Brushes.Green, re);
                    g.DrawRectangle(Pens.White, re);
                }

            }

            pictureBox1.Image = b;

            interarrivalHistogram(interarrivalDist, numberoftrial);
        }


      

        public void interarrivalHistogram(Dictionary<int, int> interarrivalDist, int numberoftrial)
        {

            b2 = new Bitmap(pictureBox3.Width, pictureBox3.Height);
            g2 = Graphics.FromImage(b2);
            g2.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            g2.Clear(Color.White);

            int j = 0;
            int step = pictureBox3.Width / interarrivalDist.Count;

            g2.DrawRectangle(new Pen(Color.Blue), 0, 0, pictureBox3.Width - 1, pictureBox3.Height - 1);

            

            foreach (var item in interarrivalDist)
            {
                double virtualX = FromXrealToXVirtual(item.Value, 0, numberoftrial,0, pictureBox3.Height);
                g2.DrawRectangle(new Pen(Color.Black), j + 1, pictureBox3.Height - (int)virtualX - 1, step, (int)virtualX);
                g2.FillRectangle(Brushes.Red, j + 1, pictureBox3.Height - (int)virtualX - 1, step, (int)virtualX);
                j += step;
            }

            pictureBox3.Image = b2;

        }

       

        private void button2_Click(object sender, EventArgs e)
        {
            b = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            g = Graphics.FromImage(b);
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            g.Clear(Color.White);


            lambda = (int)trackBar1.Value;
            int numberoftrial = 100;
            double successProbability = lambda / (double)numberoftrial;
            int TrajectoryNumber = 30;

            double minX = 0;
            double maxX = (double)numberoftrial;
            double minY = 0;
            double maxY = 1;

            Rectangle rect = new Rectangle(20, 20, b.Width - 300, b.Height - 40);
            g.DrawRectangle(Pens.Black, rect);

            Dictionary<int, int> interarrivalDist = new Dictionary<int, int>();
            List<Point> lasttrial = new List<Point>();

            int interarrival = 0;
          

            for (int i = 0; i < TrajectoryNumber; i++)
            {
                List<Point> points = new List<Point>();

                double Yt = 0.0;

                for (int X = 0; X < numberoftrial; X++)
                {
                    double uniform = r.NextDouble();

                    if (uniform < successProbability)
                    {
                     
                        Yt = Yt + 1;
                     
                        if (interarrival != 0)
                        {
                            if (!interarrivalDist.ContainsKey(interarrival))
                            {
                                interarrivalDist.Add(interarrival, 1);
                            }
                            else
                            {
                                interarrivalDist[interarrival]++;
                            }
                            interarrival = 0;
                        }
                    }
                    else
                    {
                        interarrival++;
                    }

                    double Y = Yt /  (X + 1);
                  
                    int xCord = FromXrealToXVirtual(X, minX, maxX, rect.Left,rect.Width);
                    int yCord = FromYRealToYVirtual(Y, minY, maxY, rect.Top, rect.Height);

                    Point p = new Point(xCord, yCord);
                    points.Add(p);

                    if (X == numberoftrial - 1)
                    {
                        lasttrial.Add(p);
                    }
                }
                g.DrawLines(Pen, points.ToArray());
            }

            int min_y = lasttrial.Min(p => p.Y);
            int max_y = lasttrial.Max(p => p.Y);

            Rectangle r2 = new Rectangle(rect.Right + 10, 20, 260, b.Height - 40);
            g.DrawRectangle(Pens.Black, r2);

            if (TrajectoryNumber == 1)
            {
                foreach (Point p in lasttrial)
                {
                    Rectangle re = new Rectangle(r2.Left + 10, p.Y - 10, r2.Right - r2.Left - 20, 20);
                    g.FillRectangle(Brushes.Green, re);
                    g.DrawRectangle(Pens.White, re);
                }
            }
            else
            {
                int intervals_number = TrajectoryNumber / 2;
                if (intervals_number > 15)
                {
                    intervals_number = 15;
                }

                int span = max_y - min_y;
                int interval_size = (max_y - min_y) / intervals_number;

                while (min_y + interval_size * intervals_number < max_y + 1)
                {
                    intervals_number++;
                }

                int minimo = min_y;

                Dictionary<Interval, int> intervalli = new Dictionary<Interval, int>();

                for (int j = 0; j < intervals_number; j++)
                {
                    Interval intervallo = new Interval(minimo, minimo + interval_size);
                    minimo = minimo + interval_size;
                    intervalli[intervallo] = 0;
                }

                foreach (Point p in lasttrial)
                {
                    List<Interval> inter = intervalli.Keys.ToList();
                    int intY = (int)p.Y;

                    foreach (Interval i in inter)
                    {
                        if (intY >= i.down && intY < i.up)
                        {
                            intervalli[i]++;
                        }
                    }
                }

                List<Rectangle> chart = new List<Rectangle>();

                int dimdisp = r2.Right - r2.Left - 20;
                int maxValue = intervalli.Values.Max();

                foreach (var v in intervalli)
                {
                    double intensity = ((double)v.Value / (double)maxValue) * dimdisp;
                    Rectangle rect1 = new Rectangle(r2.Left + 10, v.Key.down, (int)intensity, interval_size);
                    chart.Add(rect1);
                }

                foreach (Rectangle re in chart)
                {
                    g.FillRectangle(Brushes.Green, re);
                    g.DrawRectangle(Pens.White, re);
                }

            }

            pictureBox1.Image = b;

            interarrivalHistogram(interarrivalDist, numberoftrial);
        }

        public int FromXrealToXVirtual(double X, double minX, double maxX,int Left, int W)
        {
            return Left+ (int)(W * ((X - minX) / (maxX - minX)));
        }
        public int FromYRealToYVirtual(double Y, double minY, double maxY, int Top,int H)
        {
            return Top+ (int)(H - H * ((Y - minY) / (maxY - minY)));
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            lambda = trackBar1.Value;
            label1.Text = "Lambda parameter - " + lambda.ToString();
        }

    }
}