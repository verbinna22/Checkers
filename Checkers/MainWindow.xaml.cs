using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Checkers
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Ellipse Creating(int x, int y)
        {
            var btn2 = new Ellipse();
            btn2.Width = 50;
            btn2.Height = 50;
            
            btn2.Fill = Brushes.DarkGreen;
            btn2.AddHandler(Ellipse.MouseDownEvent, new MouseButtonEventHandler(Btn_OnMouseDown));
            btn2.AddHandler(Ellipse.MouseMoveEvent, new MouseEventHandler(Btn_OnMouseMove));
            btn2.AddHandler(Ellipse.MouseUpEvent, new MouseButtonEventHandler(Btn_OnMouseUp));
            var trans = new TranslateTransform();
            trans.X = 2 + 100*x + 50*(y%2);
            trans.Y = 2 + 50*y;

            //btn2.RenderTransform = trans;
            Canvas.SetLeft(btn2, trans.X);
            Canvas.SetTop(btn2, trans.Y);
            canv.Children.Add(btn2);
            return btn2;
        }
        List<Ellipse> chekersList = new List<Ellipse>();
        public MainWindow()
        {
            InitializeComponent();
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    chekersList.Add(Creating(j, i));
                }
            }
        }

        private Point? _movePoint;
        
        private void Btn_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            Ellipse select = (Ellipse)e.Source;
            _movePoint = e.GetPosition(this);
            select.CaptureMouse();
        }

        private void Btn_OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            Ellipse select = (Ellipse)e.Source;
            _movePoint = null;

            select.ReleaseMouseCapture();
            CorrectingPosition(select);

        }

        private void Btn_OnMouseMove(object sender, MouseEventArgs e)
        {
            Ellipse select = (Ellipse)e.Source;
            if (_movePoint == null)
                return;
            var p = e.GetPosition(this);
            //var trans = new TranslateTransform();
            //trans.X = p.X;
            //trans.Y = p.Y;

            //select.RenderTransform = trans;
            Canvas.SetLeft(select, p.X - 25);
            Canvas.SetTop(select, p.Y - 25);
        }
        private void CorrectingPosition(Ellipse uncorrect)
        {
            var curX = Canvas.GetLeft(uncorrect);
            var curY = Canvas.GetTop(uncorrect);
            //var trans = new TranslateTransform();
            //if (Convert.ToInt32(curY)/50 > 7)
            //{
            //    curY = 7 * 50 + 2;
            //}
            //else if (Convert.ToInt32(curY) / 50 < 0)
            //{
            //    curY = 2;
            //}
            //else
            //{
            //curY = Convert.ToDouble(Convert.ToInt32(curY) / 50 + 2);
            //}
            //if (Convert.ToInt32(curX) / 100 > 3)
            //{
            //    curX = 300 + Convert.ToInt32(curY)%100;
            //}
            //else if (Convert.ToInt32(curX) / 100 < 0)
            //{
            //    curX = Convert.ToInt32(curY) % 100;
            //}
            //else
            //{
            //curX = Convert.ToDouble(Convert.ToInt32(curY) / 100 + Convert.ToInt32(curY) % 100);
            //}
            //trans.X = 0;
            //trans.Y = 0;
            //uncorrect.LayoutTransform = trans;
            Canvas.SetLeft(uncorrect, 50);
            Canvas.SetTop(uncorrect, 50);
            MessageBox.Show(curX.ToString(), curY.ToString());
        }
    }
}
