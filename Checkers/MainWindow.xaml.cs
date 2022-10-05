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
            trans.X = 2 + 100*x + 50 - 50*(y%2);
            trans.Y = 250 + 2 + 50*y;

            
            Canvas.SetLeft(btn2, trans.X);
            Canvas.SetTop(btn2, trans.Y);
            canv.Children.Add(btn2);
            return btn2;
        }

        public Ellipse CreatingComputerCheckers(int x, int y)
        {
            var btn2 = new Ellipse();
            btn2.Width = 50;
            btn2.Height = 50;

            btn2.Fill = Brushes.GhostWhite;
            var trans = new TranslateTransform();
            trans.X = 2 + 100 * x + 50 * (y % 2);
            trans.Y = 2 + 50 * y;


            Canvas.SetLeft(btn2, trans.X);
            Canvas.SetTop(btn2, trans.Y);
            canv.Children.Add(btn2);
            return btn2;
        }
        List<Ellipse> chekersList = new List<Ellipse>();
        List<Ellipse> chekersComputerList = new List<Ellipse>();
        double oldX;
        double oldY;
        bool dragging = false;
        public MainWindow()
        {
            InitializeComponent();
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    chekersList.Add(Creating(j, i));
                    chekersComputerList.Add(CreatingComputerCheckers(j, i));
                }
            }
        }

        private Point? _movePoint;
        
        private void Btn_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            Ellipse select = (Ellipse)e.Source;
            _movePoint = e.GetPosition(this);
            select.CaptureMouse();
            if (!dragging)
            {
                dragging = true;
                oldX = Canvas.GetLeft(select);
                oldY = Canvas.GetTop(select);
            }
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
            Canvas.SetLeft(select, p.X - 25);
            Canvas.SetTop(select, p.Y - 25);
        }
        private void CorrectingPosition(Ellipse uncorrect)
        {
            var curX = Canvas.GetLeft(uncorrect);
            var curY = Canvas.GetTop(uncorrect);
            
            if (curY > 350)
            {
                curY = 352;
            }
            else if (curY < 2)
            {
                curY = 2;
            }
            else
            {
                curY = Convert.ToInt32(curY) / 50 * 50 + 2;
            }
            if (curX > 350)
            {
                curX = 300 + Convert.ToInt32(curY)%100;
            }
            else if (curX < 0)
            {
                curX = Convert.ToInt32(curY) % 100;
            }
            else
            {
                curX = Convert.ToInt32(curX) / 100 * 100 + Convert.ToInt32(curY) % 100;
            }
            
            Canvas.SetLeft(uncorrect, curX);
            Canvas.SetTop(uncorrect, curY);
            int countCheck = 0;
            foreach (var checker in chekersList)
            {
                var varX = Canvas.GetLeft(checker);
                var varY = Canvas.GetTop(checker);
                if ((curX == varX) && (curY == varY))
                {
                    countCheck++;
                }
                if (countCheck == 2)
                {
                    break;
                }
            }
            if ((Math.Abs(curX - oldX) == 50) && (Math.Abs(curY - oldY) == 50) && (countCheck == 1))
            {
                dragging = false;
                ComputerStickBack();
            }
            else
            {
                Canvas.SetLeft(uncorrect, oldX);
                Canvas.SetTop(uncorrect, oldY);
                dragging = false;
            }
            
        }

        public void ComputerStickBack()
        {
            var positionList = new int[10, 10];
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    positionList[i, j] = 0;
                    if ((i%9 == 0) || (j%9 == 0))
                    {
                        positionList[i, j] = 1;
                    }
                }
            }
            foreach (var checker in chekersComputerList)
            {
                var x = 1 + Convert.ToInt32(Canvas.GetLeft(checker))/50;
                var y = 1 + Convert.ToInt32(Canvas.GetTop(checker))/50;
                positionList[x, y] = 1;
            }
            var computerFall = true;
            foreach (var checker in chekersComputerList)
            {
                var x = 1 + Convert.ToInt32(Canvas.GetLeft(checker)) / 50;
                var y = 1 + Convert.ToInt32(Canvas.GetTop(checker)) / 50;
                if (positionList[x + 1, y + 1] == 0)
                {
                    Canvas.SetLeft(checker, 50*x);
                    Canvas.SetTop(checker, 50*y);
                    computerFall = false;
                    break;
                }
                if (positionList[x + 1, y - 1] == 0)
                {
                    Canvas.SetLeft(checker, 50*x);
                    Canvas.SetTop(checker, 50*(y - 2));
                    computerFall = false;
                    break;
                }
                if (positionList[x - 1, y + 1] == 0)
                {
                    Canvas.SetLeft(checker, 50*(x - 2));
                    Canvas.SetTop(checker, 50*y);
                    computerFall = false;
                    break;
                }
                if (positionList[x - 1, y - 1] == 0)
                {
                    Canvas.SetLeft(checker, 50*(x - 2));
                    Canvas.SetTop(checker, 50*(y - 2));
                    computerFall = false;
                    break;
                }
            }
            if (computerFall)
            {
                HappyEnd();
            }
        }
        public void HappyEnd()
        {
            MessageBox.Show("Противник не может сделать ход." +
                "Победила дружба!");
        }
    }
}
