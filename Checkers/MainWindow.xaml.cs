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
    public partial class MainWindow : Window
    {
        public Ellipse CreatingMyCheckers(int x, int y)
        {
            var creativeForMe = new Ellipse();
            creativeForMe.Width = checkWidth;
            creativeForMe.Height = checkWidth;
            creativeForMe.Fill = Brushes.Yellow;
            creativeForMe.AddHandler(Ellipse.MouseDownEvent, new MouseButtonEventHandler(Btn_OnMouseDown));
            creativeForMe.AddHandler(Ellipse.MouseMoveEvent, new MouseEventHandler(Btn_OnMouseMove));
            creativeForMe.AddHandler(Ellipse.MouseUpEvent, new MouseButtonEventHandler(Btn_OnMouseUp));
            Canvas.SetLeft(creativeForMe, checkMinMove + 2 * checkWidth * x + checkWidth - checkWidth * (y % 2));
            Canvas.SetTop(creativeForMe, 5 * checkWidth + checkMinMove + checkWidth * y);
            canv.Children.Add(creativeForMe);
            return creativeForMe;
        }

        public Ellipse CreatingComputerCheckers(int x, int y)
        {
            var creatingForComputer = new Ellipse();
            creatingForComputer.Width = checkWidth;
            creatingForComputer.Height = checkWidth;
            creatingForComputer.Fill = Brushes.DarkRed;
            Canvas.SetLeft(creatingForComputer, checkMinMove + 2 * checkWidth * x + checkWidth * (y % 2));
            Canvas.SetTop(creatingForComputer, checkMinMove + checkWidth * y);
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
        int checkWidth = 50;
        int checkMinMove = 2;
        public MainWindow()
        {
            InitializeComponent();
            AddingCheckersForComputer();
            AddCheckersForMe();
            MessageBox.Show("Это - шашки!" +
                " Если Вы хотите передвинуть шашку, делайте это осторожно!" +
                "Дамки временно не поддерживаются, зато теперь все шашки могут" +
                " ходить назад!" +
                " Вы ходите первым! Удачной игры!");
        }

        private void AddCheckersForMe()
        {
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    myCheckersList.Add(CreatingMyCheckers(j, i));
                }
            }
        }

        private void AddingCheckersForComputer()
        {
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    chekersComputerList.Add(CreatingComputerCheckers(j, i));
                }
            }
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
                Canvas.SetLeft(select, pointing.X - checkWidth / 2);
                Canvas.SetTop(select, pointing.Y - checkWidth / 2);
            }
        }
        private void CorrectingPosition(Ellipse uncorrect)
        {
            var curX = Canvas.GetLeft(uncorrect);
            var curY = Canvas.GetTop(uncorrect);
            DoIfOverdrag(ref curX, ref curY);
            Canvas.SetLeft(uncorrect, curX);
            Canvas.SetTop(uncorrect, curY);
            int countCheck = CountCheckersForMe(curX, curY);
            int[,] positionList = CreatePositionTable();
            FillPositionTable(positionList);
            bool myEating = EatingCheck(curX, curY, positionList);
            CorrectMyAction(uncorrect, curX, curY, countCheck, positionList, myEating);

        }

        private void CorrectMyAction(Ellipse incorrect, double curX, double curY, int countCheck, int[,] positionList, bool myEating)
        {
            if ((Math.Abs(curX - oldX) == checkWidth) && (Math.Abs(curY - oldY) == checkWidth) && (countCheck == 1) && (!myEating) && positionList[Convert.ToInt32(curX) / checkWidth + 1, Convert.ToInt32(curY) / checkWidth + 1] != 1)
            {
                dragging = false;
                ComputerStickBack();
            }
            else if ((myEating) && (positionList[Convert.ToInt32(curX) / 50 + 1, Convert.ToInt32(curY) / 50 + 1] == 1) &&
                (positionList[2 * Convert.ToInt32(curX) / 50 - Convert.ToInt32(oldX) / 50 + 1, 2 * Convert.ToInt32(curY) / 50 - Convert.ToInt32(oldY) / 50 + 1] == 0))
            {
                EatAction(incorrect, curX, curY);
            }
            else
            {
                DoIfIncorrect(incorrect);
            }
        }

        private void DoIfIncorrect(Ellipse incorrect)
        {
            Canvas.SetLeft(incorrect, oldX);
            Canvas.SetTop(incorrect, oldY);
            dragging = false;
            MessageBox.Show("Ход, который Вы хотите сделать - неправильный." +
                " Проверьте, что Вы не бьёте ни одной шашки противника." +
                " Чтобы съесть шашку противника, необходимо поставить свою шашку" +
                " на её место. Удачной игры!");
        }

        private void EatAction(Ellipse uncorrect, double curX, double curY)
        {
            Canvas.SetLeft(uncorrect, 2 * curX - oldX);
            Canvas.SetTop(uncorrect, 2 * curY - oldY);
            delComputerChecker(Convert.ToInt32(curX) / checkWidth, Convert.ToInt32(curY) / checkWidth);
            dragging = false;
            ComputerStickBack();
        }

        private bool EatingCheck(double curX, double curY, int[,] positionList)
        {
            var myEating = false;
            foreach (var checker in myCheckersList)
            {
                myEating = CheckOneChecker(curX, curY, positionList, myEating, checker);
            }
            return myEating;
        }

        private bool CheckOneChecker(double curX, double curY,
            int[,] positionList, bool myEating, Ellipse checker)
        {
            var x = 1 + Convert.ToInt32(Canvas.GetLeft(checker)) / checkWidth;
            var y = 1 + Convert.ToInt32(Canvas.GetTop(checker)) / checkWidth;
            if ((x == 1 + Convert.ToInt32(curX) / checkWidth) && (y == 1 + Convert.ToInt32(curY) / checkWidth))
            {
                x = 1 + Convert.ToInt32(oldX) / checkWidth;
                y = 1 + Convert.ToInt32(oldY) / checkWidth;
            }
            if ((positionList[x + 1, y + 1] == 1)) myEating = DoIfCondicionOne(positionList, myEating, x, y);
            else if ((positionList[x - 1, y + 1] == 1)) myEating = DoIfConditionTwo(positionList, myEating, x, y);
            else if ((positionList[x + 1, y - 1] == 1)) myEating = DoIfConditionThree(positionList, myEating, x, y);
            else if ((positionList[x - 1, y - 1] == 1)) myEating = DoIfConditionFour(positionList, myEating, x, y);
            return myEating;
        }

        private static bool DoIfConditionFour(int[,] positionList, bool myEating, int x, int y)
        {
            if (positionList[x - 2, y - 2] == 0)
                myEating = true;
            return myEating;
        }

        private static bool DoIfConditionThree(int[,] positionList, bool myEating, int x, int y)
        {
            if (positionList[x + 2, y - 2] == 0)
                myEating = true;
            return myEating;
        }

        private static bool DoIfConditionTwo(int[,] positionList, bool myEating, int x, int y)
        {
            if (positionList[x - 2, y + 2] == 0)
                myEating = true;
            return myEating;
        }

        private static bool DoIfCondicionOne(int[,] positionList, bool myEating, int x, int y)
        {
            if (positionList[x + 2, y + 2] == 0)
                myEating = true;
            return myEating;
        }

        private void FillPositionTable(int[,] positionList)
        {
            foreach (var checker in chekersComputerList)
            {
                var x = 1 + Convert.ToInt32(Canvas.GetLeft(checker)) / checkWidth;
                var y = 1 + Convert.ToInt32(Canvas.GetTop(checker)) / checkWidth;
                positionList[x, y] = 1;
            }
            foreach (var checker in myCheckersList)
            {
                var x = 1 + Convert.ToInt32(Canvas.GetLeft(checker)) / checkWidth;
                var y = 1 + Convert.ToInt32(Canvas.GetTop(checker)) / checkWidth;
                if (positionList[x, y] != 1) positionList[x, y] = 2;
            }
        }

        private static int[,] CreatePositionTable()
        {
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

            return positionList;
        }

        private int CountCheckersForMe(double curX, double curY)
        {
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

            return countCheck;
        }

        private static void DoIfOverdrag(ref double curX, ref double curY)
        {
            int checkWidth = 50;
            int checkMinMove = 2;
            if (curY > 7 * checkWidth) curY = 7 * checkWidth + checkMinMove;
            else if (curY < 2) curY = checkMinMove;
            else curY = (Convert.ToInt32(curY) + checkWidth/2) / checkWidth * checkWidth + checkMinMove;
            if (curX > 7* checkWidth) curX = 6* checkWidth + Convert.ToInt32(curY) % (2* checkWidth);
            else if (curX < 0) curX = Convert.ToInt32(curY) % (2 * checkWidth);
            else curX = (Convert.ToInt32(curX) + checkWidth/2) / (checkWidth*2) * (checkWidth*2) + Convert.ToInt32(curY) % (2 * checkWidth);
        }
        public async void ComputerStickBack()
        {
            dragAwait = false;
            await Task.Delay(200);
            dragAwait = true;
            DoIfVictory();
            int[,] positionList = CreateComputerPositions();
            FillPositionsTable(positionList);
            CheckMyCheckersInPosTable(positionList);
            bool computerEat = CanComputerEat(positionList);
            var computerFall = true;
            var random = new Random();
            chekersComputerList = chekersComputerList.OrderBy(x => random.Next()).ToList<Ellipse>();
            computerFall = DoForMainComputerActions(positionList, computerEat, computerFall);
            CheckIfLose();
        }

        private void CheckIfLose()
        {
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

        private bool DoForMainComputerActions(int[,] positionList, bool computerEat, bool computerFall)
        {
            if (!computerEat)
            {
                computerFall = Move(positionList, computerFall);
                if (computerFall)
                {
                    HappyEnd();
                }
            }

            return computerFall;
        }

        private bool Move(int[,] positionList, bool computerFall)
        {
            foreach (var checker in chekersComputerList)
            {
                var x = 1 + Convert.ToInt32(Canvas.GetLeft(checker)) / checkWidth;
                var y = 1 + Convert.ToInt32(Canvas.GetTop(checker)) / checkWidth;
                var breaking = false;
                DoIfChangePosition(positionList, ref computerFall, checker, x, y, ref breaking);
                if (breaking) break;
            }
            return computerFall;
        }

        private static void DoIfChangePosition(int[,] positionList, ref bool computerFall,
            Ellipse checker, int x, int y, ref bool breaking)
        {
            if (positionList[x + 1, y + 1] == 0) ChangingOne(out computerFall, checker, x, y, out breaking);
            else if (positionList[x + 1, y - 1] == 0) ChangingTwo(out computerFall, checker, x, y, out breaking);
            else if (positionList[x - 1, y + 1] == 0) ChangingThree(out computerFall, checker, x, y, out breaking);
            else if (positionList[x - 1, y - 1] == 0) ChangingFour(out computerFall, checker, x, y, out breaking);
        }

        private static void ChangingFour(out bool computerFall, Ellipse checker,
            int x, int y, out bool breaking)
        {
            int checkWidth = 50;
            int checkMinMove = 2;
            Canvas.SetLeft(checker, checkMinMove + checkWidth * (x - 2));
            Canvas.SetTop(checker, checkMinMove + checkWidth * (y - 2));
            computerFall = false;
            breaking = true;
        }

        private static void ChangingThree(out bool computerFall, Ellipse checker,
            int x, int y, out bool breaking)
        {
            int checkWidth = 50;
            int checkMinMove = 2;
            Canvas.SetLeft(checker, checkMinMove + checkWidth * (x - 2));
            Canvas.SetTop(checker, checkMinMove + checkWidth * y);
            computerFall = false;
            breaking = true;
        }

        private static void ChangingTwo(out bool computerFall, Ellipse checker,
            int x, int y, out bool breaking)
        {
            int checkWidth = 50;
            int checkMinMove = 2;
            Canvas.SetLeft(checker, checkMinMove + checkWidth * x);
            Canvas.SetTop(checker, checkMinMove + checkWidth * (y - 2));
            computerFall = false;
            breaking = true;
        }

        private static void ChangingOne(out bool computerFall, Ellipse checker,
            int x, int y, out bool breaking)
        {
            int checkWidth = 50;
            int checkMinMove = 2;
            Canvas.SetLeft(checker, checkMinMove + checkWidth * x);
            Canvas.SetTop(checker, checkMinMove + checkWidth * y);
            computerFall = false;
            breaking = true;
        }

        private bool CanComputerEat(int[,] positionList)
        {
            var computerEat = false;
            foreach (var checker in chekersComputerList)
            {
                var x = 1 + Convert.ToInt32(Canvas.GetLeft(checker)) / checkWidth;
                var y = 1 + Convert.ToInt32(Canvas.GetTop(checker)) / checkWidth;
                var breaking = false;
                DoOneCheck(positionList, ref computerEat, checker, x, y, ref breaking);
                DoTwoChecks(positionList, ref computerEat, checker, x, y, ref breaking);
                DoThreeChecks(positionList, ref computerEat, checker, x, y, ref breaking);
                DoFourChecks(positionList, ref computerEat, checker, x, y, ref breaking);
                if (breaking) break;
            }

            return computerEat;
        }

        private void DoFourChecks(int[,] positionList, ref bool computerEat,
            Ellipse checker, int x, int y, ref bool breaking)
        {
            if ((positionList[x - 1, y - 1] == 2) && (positionList[x - 2, y - 2] == 0))
            {
                Canvas.SetLeft(checker, checkMinMove + checkWidth * (x - 3));
                Canvas.SetTop(checker, checkMinMove + checkWidth * (y - 3));
                delChecker(x - 2, y - 2);
                computerEat = true;
                breaking = true;
            }
        }

        private void DoThreeChecks(int[,] positionList, ref bool computerEat,
            Ellipse checker, int x, int y, ref bool breaking)
        {
            if ((positionList[x - 1, y + 1] == 2) && (positionList[x - 2, y + 2] == 0))
            {
                Canvas.SetLeft(checker, checkMinMove + checkWidth * (x - 3));
                Canvas.SetTop(checker, checkMinMove + checkWidth * (y + 1));
                delChecker(x - 2, y);
                computerEat = true;
                breaking = true;
            }
        }

        private void DoTwoChecks(int[,] positionList, ref bool computerEat,
            Ellipse checker, int x, int y, ref bool breaking)
        {
            if ((positionList[x + 1, y - 1] == 2) && (positionList[x + 2, y - 2] == 0))
            {
                Canvas.SetLeft(checker, checkMinMove + checkWidth * (x + 1));
                Canvas.SetTop(checker, checkMinMove + checkWidth * (y - 3));
                delChecker(x, y - 2);
                computerEat = true;
                breaking = true;
            }
        }

        private void DoOneCheck(int[,] positionList, ref bool computerEat,
            Ellipse checker, int x, int y, ref bool breaking)
        {
            if ((positionList[x + 1, y + 1] == 2) && (positionList[x + 2, y + 2] == 0))
            {
                Canvas.SetLeft(checker, checkMinMove + checkWidth * (x + 1));
                Canvas.SetTop(checker, checkMinMove + checkWidth * (y + 1));
                delChecker(x, y);
                computerEat = true;
                breaking = true;
            }
        }

        private void CheckMyCheckersInPosTable(int[,] positionList)
        {
            foreach (var checker in myCheckersList)
            {
                var x = 1 + Convert.ToInt32(Canvas.GetLeft(checker)) / checkWidth;
                var y = 1 + Convert.ToInt32(Canvas.GetTop(checker)) / checkWidth;
                positionList[x, y] = 2;
            }
        }

        private void FillPositionsTable(int[,] positionList)
        {
            foreach (var checker in chekersComputerList)
            {
                var x = 1 + Convert.ToInt32(Canvas.GetLeft(checker)) / checkWidth;
                var y = 1 + Convert.ToInt32(Canvas.GetTop(checker)) / checkWidth;
                positionList[x, y] = 1;
            }
        }

        private static int[,] CreateComputerPositions()
        {
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

            return positionList;
        }

        private void DoIfVictory()
        {
            if (chekersComputerList.Count == 0)
            {
                if (playing)
                {
                    playing = false;
                    MessageBox.Show("Мои поздравления! Вы победили!");
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
                double point = 10.0;
                if ((Math.Abs(checkX - x * checkWidth) <= point) && (Math.Abs(checkY - y * checkWidth) <= point))
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
                double point = 10.0;
                if ((Math.Abs(checkX - x * checkWidth) <= point) && (Math.Abs(checkY - y * checkWidth) <= point))
                {
                    checker.Visibility = Visibility.Hidden;
                    chekersComputerList.Remove(checker);
                    break;
                }
            }
        }
    }
}
