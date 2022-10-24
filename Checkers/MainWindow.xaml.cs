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
        public Ellipse CreatingMyCheckers(int x, int y)
        {
            var creativeForMe = new Ellipse();
            creativeForMe.Width = 50;
            creativeForMe.Height = 50;

            creativeForMe.Fill = Brushes.Yellow;
            creativeForMe.AddHandler(Ellipse.MouseDownEvent, new MouseButtonEventHandler(Btn_OnMouseDown));
            creativeForMe.AddHandler(Ellipse.MouseMoveEvent, new MouseEventHandler(Btn_OnMouseMove));
            creativeForMe.AddHandler(Ellipse.MouseUpEvent, new MouseButtonEventHandler(Btn_OnMouseUp));

            Canvas.SetLeft(creativeForMe, 2 + 100 * x + 50 - 50 * (y % 2));
            Canvas.SetTop(creativeForMe, 250 + 2 + 50 * y);
            canv.Children.Add(creativeForMe);
            return creativeForMe;
        }

        public Ellipse CreatingComputerCheckers(int x, int y)
        {
            var creatingForComputer = new Ellipse();
            creatingForComputer.Width = 50;
            creatingForComputer.Height = 50;

            creatingForComputer.Fill = Brushes.DarkRed;

            Canvas.SetLeft(creatingForComputer, 2 + 100 * x + 50 * (y % 2));
            Canvas.SetTop(creatingForComputer, 2 + 50 * y);
            canv.Children.Add(creatingForComputer);
            return creatingForComputer;
        }
        List<Ellipse> myCheckersList = new List<Ellipse>();
        List<Ellipse> chekersComputerList = new List<Ellipse>();
        double oldX;
        double oldY;
        bool dragging = false;
        bool dragAwait = true;
        bool playing = true;
        public MainWindow()
        {
            InitializeComponent();
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    chekersComputerList.Add(CreatingComputerCheckers(j, i));
                }
            }
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    myCheckersList.Add(CreatingMyCheckers(j, i));
                }
            }
            MessageBox.Show("Это - шашки!" +
                " Если Вы хотите передвинуть шашку, делайте это осторожно!" +
                "Дамки временно не поддерживаются, зато теперь все шашки могут" +
                " ходить назад!" +
                " Вы ходите первым! Удачной игры!");
        }

        private Point? lastPointsOfWindow;

        private void Btn_OnMouseDown(object someObjectEvent, MouseButtonEventArgs evention)
        {
            Ellipse select = (Ellipse)evention.Source;
            lastPointsOfWindow = evention.GetPosition(this);
            select.CaptureMouse();
            if (!dragging)
            {
                dragging = true;
                oldX = Canvas.GetLeft(select);
                oldY = Canvas.GetTop(select);
            }
        }

        private void Btn_OnMouseUp(object someObjectEvent, MouseButtonEventArgs evention)
        {
            Ellipse select = (Ellipse)evention.Source;
            lastPointsOfWindow = null;

            select.ReleaseMouseCapture();
            CorrectingPosition(select);

        }

        private void Btn_OnMouseMove(object someObjectOfEvent, MouseEventArgs evention)
        {
            if (dragAwait)
            {
                Ellipse select = (Ellipse)evention.Source;
                if (lastPointsOfWindow == null)
                    return;
                var pointing = evention.GetPosition(this);
                Canvas.SetLeft(select, pointing.X - 25);
                Canvas.SetTop(select, pointing.Y - 25);
            }
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
                curY = (Convert.ToInt32(curY) + 25)/ 50 * 50 + 2;
            }
            if (curX > 350)
            {
                curX = 300 + Convert.ToInt32(curY) % 100;
            }
            else if (curX < 0)
            {
                curX = Convert.ToInt32(curY) % 100;
            }
            else
            {
                curX = (Convert.ToInt32(curX) + 25) / 100 * 100 + Convert.ToInt32(curY) % 100;
            }

            Canvas.SetLeft(uncorrect, curX);
            Canvas.SetTop(uncorrect, curY);
            int countCheck = 0;
            foreach (var checker in myCheckersList)
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
            var positionList = new int[10, 10];
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    positionList[i, j] = 0;
                    if ((i % 9 == 0) || (j % 9 == 0))
                    {
                        positionList[i, j] = 3;
                    }
                }
            }
            foreach (var checker in chekersComputerList)
            {
                var x = 1 + Convert.ToInt32(Canvas.GetLeft(checker)) / 50;
                var y = 1 + Convert.ToInt32(Canvas.GetTop(checker)) / 50;
                positionList[x, y] = 1;
            }
            foreach (var checker in myCheckersList)
            {
                var x = 1 + Convert.ToInt32(Canvas.GetLeft(checker)) / 50;
                var y = 1 + Convert.ToInt32(Canvas.GetTop(checker)) / 50;
                if (positionList[x, y] != 1) positionList[x, y] = 2;
            }
            var myEating = false;
            foreach (var checker in myCheckersList)
            {
                var x = 1 + Convert.ToInt32(Canvas.GetLeft(checker)) / 50;
                var y = 1 + Convert.ToInt32(Canvas.GetTop(checker)) / 50;
                if ((x == 1 + Convert.ToInt32(curX) / 50) && (y == 1 + Convert.ToInt32(curY) / 50))
                {
                    x = 1 + Convert.ToInt32(oldX) / 50;
                    y = 1 + Convert.ToInt32(oldY) / 50;
                }
                if ((positionList[x + 1, y + 1] == 1))
                {
                    if (positionList[x + 2, y + 2] == 0)
                        myEating = true;
                }
                else if ((positionList[x - 1, y + 1] == 1))
                {
                    if (positionList[x - 2, y + 2] == 0)
                        myEating = true;
                }
                else if ((positionList[x + 1, y - 1] == 1))
                {
                    if (positionList[x + 2, y - 2] == 0)
                        myEating = true;
                }
                else if ((positionList[x - 1, y - 1] == 1))
                {
                    if (positionList[x - 2, y - 2] == 0)
                        myEating = true;
                }
            }
            if ((Math.Abs(curX - oldX) == 50) && (Math.Abs(curY - oldY) == 50) && (countCheck == 1) && (!myEating) && positionList[Convert.ToInt32(curX) / 50 + 1, Convert.ToInt32(curY) / 50 + 1] != 1)
            {
                dragging = false;
                ComputerStickBack();
            }
            else if ((myEating) && (positionList[Convert.ToInt32(curX) / 50 + 1, Convert.ToInt32(curY) / 50 + 1] == 1) &&
                (positionList[2 * Convert.ToInt32(curX) / 50 - Convert.ToInt32(oldX) / 50 + 1, 2 * Convert.ToInt32(curY) / 50 - Convert.ToInt32(oldY) / 50 + 1] == 0))
            {
                Canvas.SetLeft(uncorrect, 2 * curX - oldX);
                Canvas.SetTop(uncorrect, 2 * curY - oldY);
                delComputerChecker(Convert.ToInt32(curX) / 50, Convert.ToInt32(curY) / 50);
                dragging = false;
                ComputerStickBack();
            }
            else
            {
                Canvas.SetLeft(uncorrect, oldX);
                Canvas.SetTop(uncorrect, oldY);
                dragging = false;
                MessageBox.Show("Ход, который Вы хотите сделать - неправильный." +
                    " Проверьте, что Вы не бьёте ни одной шашки противника." +
                    " Чтобы съесть шашку противника, необходимо поставить свою шашку" +
                    " на её место. Удачной игры!");
            }

        }

        public async void ComputerStickBack()
        {
            dragAwait = false;
            await Task.Delay(200);
            dragAwait = true;
            if (chekersComputerList.Count == 0)
            {
                    if (playing)
                    {
                        playing = false;
                        MessageBox.Show("Мои поздравления! Вы победили!");
                        System.Windows.Application.Current.Shutdown();
                    }
            }
            var positionList = new int[10, 10];
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    positionList[i, j] = 0;
                    if ((i % 9 == 0) || (j % 9 == 0))
                    {
                        positionList[i, j] = 1;
                    }
                }
            }
            foreach (var checker in chekersComputerList)
            {
                var x = 1 + Convert.ToInt32(Canvas.GetLeft(checker)) / 50;
                var y = 1 + Convert.ToInt32(Canvas.GetTop(checker)) / 50;
                positionList[x, y] = 1;
            }

            foreach (var checker in myCheckersList)
            {
                var x = 1 + Convert.ToInt32(Canvas.GetLeft(checker)) / 50;
                var y = 1 + Convert.ToInt32(Canvas.GetTop(checker)) / 50;
                positionList[x, y] = 2;
            }
            var computerEat = false;
            foreach (var checker in chekersComputerList)
            {
                var x = 1 + Convert.ToInt32(Canvas.GetLeft(checker)) / 50;
                var y = 1 + Convert.ToInt32(Canvas.GetTop(checker)) / 50;
                if ((positionList[x + 1, y + 1] == 2) && (positionList[x + 2, y + 2] == 0))
                {
                    Canvas.SetLeft(checker, 2 + 50 * (x + 1));
                    Canvas.SetTop(checker, 2 + 50 * (y + 1));
                    delChecker(x, y);
                    computerEat = true;
                    break;
                }
                if ((positionList[x + 1, y - 1] == 2) && (positionList[x + 2, y - 2] == 0))
                {
                    Canvas.SetLeft(checker, 2 + 50 * (x + 1));
                    Canvas.SetTop(checker, 2 + 50 * (y - 3));
                    delChecker(x, y - 2);
                    computerEat = true;
                    break;
                }
                if ((positionList[x - 1, y + 1] == 2) && (positionList[x - 2, y + 2] == 0))
                {
                    Canvas.SetLeft(checker, 2 + 50 * (x - 3));
                    Canvas.SetTop(checker, 2 + 50 * (y + 1));
                    delChecker(x - 2, y);
                    computerEat = true;
                    break;
                }
                if ((positionList[x - 1, y - 1] == 2) && (positionList[x - 2, y - 2] == 0))
                {
                    Canvas.SetLeft(checker, 2 + 50 * (x - 3));
                    Canvas.SetTop(checker, 2 + 50 * (y - 3));
                    delChecker(x - 2, y - 2);
                    computerEat = true;
                    break;
                }
            }

            var computerFall = true;
            var random = new Random();
            chekersComputerList = chekersComputerList.OrderBy(x => random.Next()).ToList<Ellipse>();
            if (!computerEat)
            {
                foreach (var checker in chekersComputerList)
                {
                    var x = 1 + Convert.ToInt32(Canvas.GetLeft(checker)) / 50;
                    var y = 1 + Convert.ToInt32(Canvas.GetTop(checker)) / 50;
                    if (positionList[x + 1, y + 1] == 0)
                    {
                        Canvas.SetLeft(checker, 2 + 50 * x);
                        Canvas.SetTop(checker, 2 + 50 * y);
                        computerFall = false;
                        break;
                    }
                    if (positionList[x + 1, y - 1] == 0)
                    {
                        Canvas.SetLeft(checker, 2 + 50 * x);
                        Canvas.SetTop(checker, 2 + 50 * (y - 2));
                        computerFall = false;
                        break;
                    }
                    if (positionList[x - 1, y + 1] == 0)
                    {
                        Canvas.SetLeft(checker, 2 + 50 * (x - 2));
                        Canvas.SetTop(checker, 2 + 50 * y);
                        computerFall = false;
                        break;
                    }
                    if (positionList[x - 1, y - 1] == 0)
                    {
                        Canvas.SetLeft(checker, 2 + 50 * (x - 2));
                        Canvas.SetTop(checker, 2 + 50 * (y - 2));
                        computerFall = false;
                        break;
                    }
                }
                if (computerFall)
                {
                    HappyEnd();
                }
            }
            if (myCheckersList.Count == 0)
            {

                    if (playing)
                    {
                        MessageBox.Show("К сожалению, Вы проиграли!");
                        playing = false;
                        System.Windows.Application.Current.Shutdown();
                    }
            }
        }
        public void HappyEnd()
        {

               if (playing)
                {
                playing = false;
                    MessageBox.Show("Противник не может сделать ход. " +
                    "Победила дружба!");
                    System.Windows.Application.Current.Shutdown();
                }
        }

        public void delChecker(int x, int y)
        {
            foreach (var checker in myCheckersList)
            {
                var checkX = Canvas.GetLeft(checker);
                var checkY = Canvas.GetTop(checker);
                if ((Math.Abs(checkX - x * 50) <= 10.0) && (Math.Abs(checkY - y * 50) <= 10.0))
                {
                    checker.Visibility = Visibility.Hidden;
                    myCheckersList.Remove(checker);
                    break;
                }
            }
        }
        public void delComputerChecker(int x, int y)
        {
            foreach (var checker in chekersComputerList)
            {
                var checkX = Canvas.GetLeft(checker);
                var checkY = Canvas.GetTop(checker);
                if ((Math.Abs(checkX - x * 50) <= 10.0) && (Math.Abs(checkY - y * 50) <= 10.0))
                {
                    checker.Visibility = Visibility.Hidden;
                    chekersComputerList.Remove(checker);
                    break;
                }
            }
        }
    }
}
