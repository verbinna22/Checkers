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
        public MainWindow()
        {
            InitializeComponent();
        }

        private Point? _movePoint;

        private void Btn_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            _movePoint = e.GetPosition(btn2);
            btn2.CaptureMouse();
        }

        private void Btn_OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            _movePoint = null;
            btn2.ReleaseMouseCapture();
        }

        private void Btn_OnMouseMove(object sender, MouseEventArgs e)
        {
            if (_movePoint == null)
                return;
            var p = e.GetPosition(this) - (Vector)_movePoint.Value;
            Canvas.SetLeft(btn2, p.X);
            Canvas.SetTop(btn2, p.Y);
        }
    }
}
