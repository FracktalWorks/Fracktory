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
using WinInterop = System.Windows.Interop;
using System.Runtime.InteropServices;

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
            FracktoryWindow.Loaded += new RoutedEventHandler(win_Loaded);
            FracktoryWindow.SourceInitialized += new EventHandler(win_SourceInitialized);
        }

        void win_SourceInitialized(object sender, EventArgs e)
        {
            System.IntPtr handle = (new WinInterop.WindowInteropHelper(this)).Handle;
            WinInterop.HwndSource.FromHwnd(handle).AddHook(new WinInterop.HwndSourceHook(WindowProc));
        }



        private static System.IntPtr WindowProc(
              System.IntPtr hwnd,
              int msg,
              System.IntPtr wParam,
              System.IntPtr lParam,
              ref bool handled)
        {
            switch (msg)
            {
                case 0x0024:
                    WinInterop.HwndSource _win = WinInterop.HwndSource.FromHwnd(hwnd);
                    Window _root = (Window) _win.RootVisual;

                    //only 
                    if (_root.WindowState != WindowState.Maximized)
                    {
                        break;
                    }

                    WmGetMinMaxInfo(hwnd, lParam);
                    handled = true;
                    break;
            }

            return (System.IntPtr) 0;
        }

        private static void WmGetMinMaxInfo(System.IntPtr hwnd, System.IntPtr lParam)
        {

            MINMAXINFO mmi = (MINMAXINFO) Marshal.PtrToStructure(lParam, typeof(MINMAXINFO));

            // Adjust the maximized size and position to fit the work area of the correct monitor
            int MONITOR_DEFAULTTONEAREST = 0x00000002;
            System.IntPtr monitor = MonitorFromWindow(hwnd, MONITOR_DEFAULTTONEAREST);

            if (monitor != System.IntPtr.Zero)
            {

                MONITORINFO monitorInfo = new MONITORINFO();
                GetMonitorInfo(monitor, monitorInfo);
                RECT rcWorkArea = monitorInfo.rcWork;
                RECT rcMonitorArea = monitorInfo.rcMonitor;
                mmi.ptMaxPosition.x = Math.Abs(rcWorkArea.left - rcMonitorArea.left);
                mmi.ptMaxPosition.y = Math.Abs(rcWorkArea.top - rcMonitorArea.top);
                mmi.ptMaxSize.x = Math.Abs(rcWorkArea.right - rcWorkArea.left);
                mmi.ptMaxSize.y = Math.Abs(rcWorkArea.bottom - rcWorkArea.top);
            }

            Marshal.StructureToPtr(mmi, lParam, true);
        }


        /// <summary>
        /// POINT aka POINTAPI
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            /// <summary>
            /// x coordinate of point.
            /// </summary>
            public int x;
            /// <summary>
            /// y coordinate of point.
            /// </summary>
            public int y;

            /// <summary>
            /// Construct a point of coordinates (x,y).
            /// </summary>
            public POINT(int x, int y)
            {
                this.x = x;
                this.y = y;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MINMAXINFO
        {
            public POINT ptReserved;
            public POINT ptMaxSize;
            public POINT ptMaxPosition;
            public POINT ptMinTrackSize;
            public POINT ptMaxTrackSize;
        };

        void win_Loaded(object sender, RoutedEventArgs e)
        {
           // FracktoryWindow.WindowState = WindowState.Maximized;
        }


        /// <summary>
        /// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public class MONITORINFO
        {
            /// <summary>
            /// </summary>            
            public int cbSize = Marshal.SizeOf(typeof(MONITORINFO));

            /// <summary>
            /// </summary>            
            public RECT rcMonitor = new RECT();

            /// <summary>
            /// </summary>            
            public RECT rcWork = new RECT();

            /// <summary>
            /// </summary>            
            public int dwFlags = 0;
        }


        /// <summary> Win32 </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 0)]
        public struct RECT
        {
            /// <summary> Win32 </summary>
            public int left;
            /// <summary> Win32 </summary>
            public int top;
            /// <summary> Win32 </summary>
            public int right;
            /// <summary> Win32 </summary>
            public int bottom;

            /// <summary> Win32 </summary>
            public static readonly RECT Empty = new RECT();

            /// <summary> Win32 </summary>
            public int Width
            {
                get
                {
                    return Math.Abs(right - left);
                }  // Abs needed for BIDI OS
            }
            /// <summary> Win32 </summary>
            public int Height
            {
                get
                {
                    return bottom - top;
                }
            }

            /// <summary> Win32 </summary>
            public RECT(int left, int top, int right, int bottom)
            {
                this.left = left;
                this.top = top;
                this.right = right;
                this.bottom = bottom;
            }


            /// <summary> Win32 </summary>
            public RECT(RECT rcSrc)
            {
                this.left = rcSrc.left;
                this.top = rcSrc.top;
                this.right = rcSrc.right;
                this.bottom = rcSrc.bottom;
            }

            /// <summary> Win32 </summary>
            public bool IsEmpty
            {
                get
                {
                    // BUGBUG : On Bidi OS (hebrew arabic) left > right
                    return left >= right || top >= bottom;
                }
            }
            /// <summary> Return a user friendly representation of this struct </summary>
            public override string ToString()
            {
                if (this == RECT.Empty)
                {
                    return "RECT {Empty}";
                }
                return "RECT { left : " + left + " / top : " + top + " / right : " + right + " / bottom : " + bottom + " }";
            }

            /// <summary> Determine if 2 RECT are equal (deep compare) </summary>
            public override bool Equals(object obj)
            {
                if (!(obj is Rect))
                {
                    return false;
                }
                return (this == (RECT) obj);
            }

            /// <summary>Return the HashCode for this struct (not garanteed to be unique)</summary>
            public override int GetHashCode()
            {
                return left.GetHashCode() + top.GetHashCode() + right.GetHashCode() + bottom.GetHashCode();
            }


            /// <summary> Determine if 2 RECT are equal (deep compare)</summary>
            public static bool operator ==(RECT rect1, RECT rect2)
            {
                return (rect1.left == rect2.left && rect1.top == rect2.top && rect1.right == rect2.right && rect1.bottom == rect2.bottom);
            }

            /// <summary> Determine if 2 RECT are different(deep compare)</summary>
            public static bool operator !=(RECT rect1, RECT rect2)
            {
                return !(rect1 == rect2);
            }


        }

        [DllImport("user32")]
        internal static extern bool GetMonitorInfo(IntPtr hMonitor, MONITORINFO lpmi);

        /// <summary>
        /// 
        /// </summary>
        [DllImport("User32")]
        internal static extern IntPtr MonitorFromWindow(IntPtr handle, int flags);

        private void Grid_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (WindowState == WindowState.Maximized)
                {
                    this.WindowState = WindowState.Normal;
                  
                }

                this.DragMove();
            }
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ExpanderPrint_Expanded(object sender, RoutedEventArgs e)
        {
            ExpanderFilament.IsExpanded = false;
        }

        private void ExpanderFilament_Expanded(object sender, RoutedEventArgs e)
        {
            ExpanderPrint.IsExpanded = false;
        }

        private void Slicer_tab_label_MouseUp(object sender, MouseButtonEventArgs e)
        {
            TabSlicer.IsSelected = true;
        }

        private void Pronterface_tab_label_MouseUp(object sender, MouseButtonEventArgs e)
        {
            TabPronterface.IsSelected = true;
        }

        private void chkExpertMode_Click(object sender, RoutedEventArgs e)
        {
            if (chkExpertMode.IsChecked==true)
            {
                ExpanderExpert.Visibility = Visibility.Visible;
            }
            else
            {
                ExpanderExpert.Visibility = Visibility.Collapsed;
            }
        }
       


    }


}
