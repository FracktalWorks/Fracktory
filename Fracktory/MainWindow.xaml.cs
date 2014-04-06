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

namespace Fracktory
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

        private void Grid_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton== MouseButtonState.Pressed)
            this.DragMove();
        }

        private void bMin_MouseMove(object sender, MouseEventArgs e)
        {
            imgMin.Source = new BitmapImage(new Uri("image/minh.png",UriKind.Relative));
        }

        private void bMin_MouseLeave(object sender, MouseEventArgs e)
        {
            imgMin.Source = new BitmapImage(new Uri("image/min.png", UriKind.Relative));
        }

        private void bMin_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void bRestore_MouseEnter(object sender, MouseEventArgs e)
        {
            imgRestore.Source = new BitmapImage(new Uri("image/restoreh.png", UriKind.Relative));
        }

        private void bRestore_MouseLeave(object sender, MouseEventArgs e)
        {
            imgRestore.Source = new BitmapImage(new Uri("image/restore.png", UriKind.Relative));
        }

        private void bRestore_Click(object sender, RoutedEventArgs e)
        {
            if (WindowState==WindowState.Maximized)
            {
                this.WindowState = WindowState.Normal;
            }
            else
            {
                this.WindowState = WindowState.Maximized;
            }
           
        }

        private void bClose_MouseEnter(object sender, MouseEventArgs e)
        {
            imgClose.Source = new BitmapImage(new Uri("image/closeh.png", UriKind.Relative));
        }

        private void bClose_MouseLeave(object sender, MouseEventArgs e)
        {
            imgClose.Source = new BitmapImage(new Uri("image/close.png", UriKind.Relative));
        }

        private void bClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void togleRestore_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                this.WindowState = WindowState.Normal;
            }
            else
            {
                this.WindowState = WindowState.Maximized;
            }
        }

    }


}
