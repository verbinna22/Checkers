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
        public Label Creating(int x, int y)
        {
            Label btn2 = new Label();
            btn2.Width = 50;
            btn2.Height = 50;
            
            btn2.Background = Brushes.Brown;
            btn2.AddHandler(Label.MouseDownEvent, new MouseButtonEventHandler(Btn_OnMouseDown));
            btn2.AddHandler(Label.MouseMoveEvent, new MouseEventHandler(Btn_OnMouseMove));
            btn2.AddHandler(Label.MouseUpEvent, new MouseButtonEventHandler(Btn_OnMouseUp));
            var trans = new TranslateTransform();
            trans.X = 50*x + 5 + 50*(y%2);
            trans.Y = 50*y + 5 + 50*(x%2);
            
            btn2.RenderTransform = trans;
            canv.Children.Add(btn2);
            return btn2;
        }
        List<Label> chekersList = new List<Label>();
        public MainWindow()
        {
            InitializeComponent();
            for (int i = 0; i < 6; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    chekersList.Add(Creating(i, j));
                }
            }
        }

        private Point? _movePoint;
        
        private void Btn_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            Label select = (Label)e.Source;
            _movePoint = e.GetPosition(select);
            select.CaptureMouse();
        }

        private void Btn_OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            Label select = (Label)e.Source;
            _movePoint = null;
            select.ReleaseMouseCapture();
        }

        private void Btn_OnMouseMove(object sender, MouseEventArgs e)
        {
            Label select = (Label)e.Source;
            if (_movePoint == null)
                return;
            var p = e.GetPosition(select) - (Vector)_movePoint.Value;
            Canvas.SetLeft(select, p.X);
            Canvas.SetTop(select, p.Y);
        }
    }
}
