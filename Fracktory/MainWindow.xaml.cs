using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using WinInterop = System.Windows.Interop;
using System.Runtime.InteropServices;
using System.Windows.Media.Media3D;
using Newtonsoft.Json.Linq;
using System.Reflection;
using System.IO;
//using CefSharp;
using System.Windows.Shapes;
using System.Windows.Media;
namespace Fracktory
{

    public partial class MainWindow : Window
    {
        public Model3D currentModel;
        public PrintConfiguration config;
        
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = new MainViewModel(new FileDialogService(), Viewport, RotatorX, RotatorY, RotatorZ, ScaleXYZ, config);
            FracktoryWindow.Loaded += new RoutedEventHandler(win_Loaded);
            FracktoryWindow.SourceInitialized += new EventHandler(win_SourceInitialized);
            ((MainViewModel) (this.DataContext)).GcodeCanvas = GcodeCanvas;
            ((MainViewModel) (this.DataContext)).GcodeCanvasWidth = FracktoryWindow.Width - 272;
            ((MainViewModel) (this.DataContext)).GcodeCanvasHeight = FracktoryWindow.Height - 150;
        //    CefSharp.Settings settings = new CefSharp.Settings();
        //    CefSharp.BrowserSettings browserSettings = new BrowserSettings();
        //    browserSettings.FileAccessFromFileUrlsAllowed = true;
        //    browserSettings.UniversalAccessFromFileUrlsAllowed = true;
        //    browserSettings.TextAreaResizeDisabled = true;
            
        //settings.PackLoadingDisabled = true;
        //if (CEF.Initialize(settings)){
        //    CefSharp.Wpf.WebView web_view = new CefSharp.Wpf.WebView(AssemblyDirectory+  @"/gCodeViewer/index.html",browserSettings);
        //    grid1.Children.Add(web_view);
            
            //730 x 460

           // web_view.Address = "file:///E:/Fracktal/gCodeViewer-master2/index.html";
        //}
            //Uri uri = new Uri(@"pack://application:,,,/gCodeViewer/index.html");
            //Stream source = Application.GetContentStream(uri).Stream;
            //wbMain.NavigateToStream(source);
            
        }

        #region others

        #region  size_details
        void win_SourceInitialized(object sender, EventArgs e)
        {
            System.IntPtr handle = (new WinInterop.WindowInteropHelper(this)).Handle;
            WinInterop.HwndSource.FromHwnd(handle).AddHook(new WinInterop.HwndSourceHook(WindowProc));
        }



        private static System.IntPtr WindowProc(System.IntPtr hwnd, int msg, System.IntPtr wParam, System.IntPtr lParam, ref bool handled)
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

        #endregion

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
            imgMin.Source = new BitmapImage(new Uri("image/minh.png", UriKind.Relative));
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
            if (WindowState == WindowState.Maximized)
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



        private void ExpanderExpert_Expanded(object sender, RoutedEventArgs e)
        {
            ExpanderPrint.IsExpanded = false;
        }


        private void ExpanderPrint_Expanded(object sender, RoutedEventArgs e)
        {
            ExpanderExpert.IsExpanded = false;
        }


        private void Slicer_tab_label_MouseUp(object sender, MouseButtonEventArgs e)
        {
            TabSlicer.IsSelected = true;
        }

        private void Pronterface_tab_label_MouseUp(object sender, MouseButtonEventArgs e)
        {
            TabPronterface.IsSelected = true;
        }


        #endregion

        private void tglRotate_Click(object sender, RoutedEventArgs e)
        {
            //first time
            if (RotatePanel.Visibility == Visibility.Collapsed && ScalePanel.Visibility == Visibility.Collapsed)
            {
                RotatePanel.Visibility = Visibility.Visible;
            }
            else if (RotatePanel.Visibility == Visibility.Collapsed && ScalePanel.Visibility == Visibility.Visible)
            {
                RotatePanel.Visibility = Visibility.Visible;
                ScalePanel.Visibility = Visibility.Collapsed;
            }
            else if (RotatePanel.Visibility == Visibility.Visible && ScalePanel.Visibility == Visibility.Collapsed)
            {
                RotatePanel.Visibility = Visibility.Collapsed;
                ScalePanel.Visibility = Visibility.Collapsed;
            }
        }

        private void tglScale_Click(object sender, RoutedEventArgs e)
        {
            //first time
            if (RotatePanel.Visibility == Visibility.Collapsed && ScalePanel.Visibility == Visibility.Collapsed)
            {
                ScalePanel.Visibility = Visibility.Visible;
            }
            else if (RotatePanel.Visibility == Visibility.Collapsed && ScalePanel.Visibility == Visibility.Visible)
            {
                RotatePanel.Visibility = Visibility.Collapsed;
                ScalePanel.Visibility = Visibility.Collapsed;
            }
            else if (RotatePanel.Visibility == Visibility.Visible && ScalePanel.Visibility == Visibility.Collapsed)
            {
                RotatePanel.Visibility = Visibility.Collapsed;
                ScalePanel.Visibility = Visibility.Visible;
            }
        }

        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            Menu.IsEnabled = false;




            if (FilamentType.SelectedIndex == 0)//abs
            {
                config = new PrintConfiguration(PrintMaterial.ABS);
            }
            else if (FilamentType.SelectedIndex == 1)//pla
            {
                config = new PrintConfiguration(PrintMaterial.PLA);
            }
            else //other
            {
                config = new PrintConfiguration(PrintMaterial.ABS);
                config.ExtraConfiguration["temperature"] = ExtruderOtherLayers.Value.ToString();
                config.ExtraConfiguration["first-layer-temperature"] = ExtruderFirstLayer.Value.ToString();
                config.ExtraConfiguration["bed-temperature"] = BedOtherLayers.Value.ToString();
                config.ExtraConfiguration["first-layer-bed-temperature"] = BedFirstLayer.Value.ToString();
                config.ExtraConfiguration["skirt-distance"] = DistanceFromObject.Value.ToString();
                config.ExtraConfiguration["skirt-height"] = SkirtHeight.Value.ToString();
                config.ExtraConfiguration["skirts"] = Loops.Value.ToString();
                config.ExtraConfiguration["min-skirt-length"] = MinimumExtrusionLength.Value.ToString();
                config.ExtraConfiguration["brim-width"] = BrimWidth.Value.ToString();
            }

            if ((bool) SpiralVase.IsChecked)
            {
                config.ExtraConfiguration["spiral-vase"] = "1";
            }
            else
            {
                config.ExtraConfiguration["spiral-vase"] = "0";
            }

            if (HeightResolution.SelectedIndex == 0)
            {
                config.ExtraConfiguration["layer-height"] = "0.1"; //low
            }
            else if (HeightResolution.SelectedIndex == 1)
            {
                config.ExtraConfiguration["layer-height"] = "0.2"; //med
            }
            else if (HeightResolution.SelectedIndex == 2)
            {
                config.ExtraConfiguration["layer-height"] = "0.3"; //high
            }

            //config.ExtraConfiguration["fill-density"] = "0.15";
            if (FillDensity.SelectedIndex == 0)
            {
                config.ExtraConfiguration["fill-density"] = "0"; //hollow
            }
            else if (FillDensity.SelectedIndex == 1)
            {
                config.ExtraConfiguration["fill-density"] = "0.06"; //low
            }
            else if (FillDensity.SelectedIndex == 2)
            {
                config.ExtraConfiguration["fill-density"] = "0.15"; //medium
            }
            else if (FillDensity.SelectedIndex == 3)
            {
                config.ExtraConfiguration["fill-density"] = "0.4"; //medium
            }

            config.ExtraConfiguration["infill-every-layers"] = CombineInfillEvery.Value.ToString();
            if ((bool) OnlyInfillWhenNeeded.IsChecked)
            {
                config.ExtraConfiguration["infill-only-where-needed"] = "1";
            }
            else
            {
                config.ExtraConfiguration["infill-only-where-needed"] = "0";
            }

            config.ExtraConfiguration["solid-infill-every-layers"] = SolidInfillEvery.Value.ToString();
            config.ExtraConfiguration["solid-infill-below-area"] = SolidInfillThresholdLevels.Value.ToString();

            if ((bool) OnlyRetractWhenCrossingPerimeters.IsChecked)
            {
                config.ExtraConfiguration["only-retract-when-crossing-perimeters"] = "1";
            }
            else
            {
                config.ExtraConfiguration["only-retract-when-crossing-perimeters"] = "0";
            }
            if ((bool) GenerateSupportMaterial.IsChecked)
            {
                config.ExtraConfiguration["support-material"] = "1";
            }
            else
            {
                config.ExtraConfiguration["support-material"] = "0";
            }

            if ((bool) InfillBeforePerimeters.IsChecked)
            {
                config.ExtraConfiguration["infil-first"] = "1";
            }
            else
            {
                config.ExtraConfiguration["infill-first"] = "0";
            }


                ((MainViewModel) (this.DataContext)).PrintConfig = config;

                Menu.IsEnabled = true;
            
        }

        private void FilamentType_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (ExtruderFirstLayer == null)
            {
                return;
            }
            if (FilamentType.SelectedIndex == 0)
            {
                ExpanderExpert.Visibility = Visibility.Collapsed;
                updateEntries(@"\configuration\ABS.json");

            }
            else if (FilamentType.SelectedIndex == 1)
            {
                ExpanderExpert.Visibility = Visibility.Collapsed;
                updateEntries(@"\configuration\PLA.json");
            }
            else
            {
                ExpanderExpert.Visibility = Visibility.Visible;
                ExpanderExpert.IsExpanded = true;
                ExpanderPrint.IsExpanded = false;
            }
        }
        private void updateEntries(string jsonPath)
        {
            if (ExtruderFirstLayer == null)
            {
                return;
            }
            
            string path = System.IO.Path.Combine(AssemblyDirectory + jsonPath);
            string configuration = System.IO.File.ReadAllText(path);
            JObject JsonConfiguration = JObject.Parse(configuration);
            ExtruderFirstLayer.Value = Convert.ToInt32(JsonConfiguration["ExtruderFirstLayer"].ToString());
            ExtruderOtherLayers.Value = Convert.ToInt32(JsonConfiguration["ExtruderOtherLayers"].ToString());

            BedFirstLayer.Value = Convert.ToInt32(JsonConfiguration["BedFirstLayer"].ToString());
            BedOtherLayers.Value = Convert.ToInt32(JsonConfiguration["BedOtherLayers"].ToString());

            Loops.Value = Convert.ToInt32(JsonConfiguration["Loops"].ToString());
            DistanceFromObject.Value = Convert.ToInt32(JsonConfiguration["DistanceFromObject"].ToString());
            SkirtHeight.Value = Convert.ToInt32(JsonConfiguration["SkirtHeight"].ToString());
            MinimumExtrusionLength.Value = Convert.ToInt32(JsonConfiguration["MinimumExtrusionLength"].ToString());

            BrimWidth.Value = Convert.ToInt32(JsonConfiguration["BrimWidth"].ToString());

        }
        static public string AssemblyDirectory
        {
            get
            {
                string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return System.IO.Path.GetDirectoryName(path);
            }
        }
        private void OpenPronterface(object sender, RoutedEventArgs e)
        {
            TabPronterface.IsSelected = true;
        }
        private void wbMain_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void canvas_MouseMove(object sender, MouseEventArgs e)
        {
            canvas.Children.Clear();
            Line l = new Line();
            l.X1 = e.GetPosition(canvas).X;
            l.X2 = e.GetPosition(canvas).X;
            l.Y1 = 0;
            l.Y2 = 200;
            l.Stroke = SystemColors.WindowFrameBrush;

            Line l1 = new Line();
            l1.Y1 = e.GetPosition(canvas).Y;
            l1.Y2 = e.GetPosition(canvas).Y;
            l1.X1 = 0;
            l1.X2 = 200;
            l1.Stroke = SystemColors.WindowFrameBrush;
            canvas.Children.Add(l);
            canvas.Children.Add(l1);
            currentX.Value = (int?) Math.Truncate(e.GetPosition(canvas).X);
            currentY.Value = (int?) Math.Truncate(e.GetPosition(canvas).Y);
            currentZ.Value = (int?) ZSlider.Value;
            
        }

        private void canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void FracktoryWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ((MainViewModel) (this.DataContext)).GcodeCanvasWidth = FracktoryWindow.Width - 272;
            ((MainViewModel) (this.DataContext)).GcodeCanvasHeight = FracktoryWindow.Height - 150;

        }
    }
}
