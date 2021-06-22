using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MessageBox = System.Windows.MessageBox;
using MouseEventArgs = System.Windows.Forms.MouseEventArgs;

namespace ClickShow
{


    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MouseHook.MouseHook _mouseHook;

        // 窗口缓存,解决连续点击的显示问题
        private IList<ClickIndicator> _clickIndicators = new List<ClickIndicator>();

        private System.Windows.Forms.NotifyIcon notifyIcon = null;

        public static readonly DependencyProperty EnabledProperty = DependencyProperty.Register(
            "Enabled", typeof(bool), typeof(MainWindow), new PropertyMetadata(true));

        /// <summary>
        /// 是否开启显示
        /// </summary>
        public bool Enabled
        {
            get { return (bool)GetValue(EnabledProperty); }
            set { SetValue(EnabledProperty, value); }
        }

        private ClickIndicator GetClickIndicatorWindow()
        {
            var indicator = _clickIndicators.FirstOrDefault(x => x.IsIdle);

            if (indicator != null)
            {
                indicator.Prepare();
                return indicator;
            }
            else
            {
                indicator = new ClickIndicator()
                {
                    WindowStartupLocation = WindowStartupLocation.Manual,
                    Topmost = true,
                    ShowActivated = false
                };
                _clickIndicators.Add(indicator);
                return indicator;
            }
        }

        public MainWindow()
        {
            InitializeComponent();

            


            Closed += OnClosed;

            Loaded += (sender, args) =>
            {
                WindowState = WindowState.Minimized;
            };

            // Note: for the application hook, use the Hook.AppEvents() instead
            _mouseHook = new MouseHook.MouseHook();
            _mouseHook.MouseDown += MouseHookOnMouseDown;
            _mouseHook.Start();
        }

        private void MouseHookOnMouseDown(object sender, MouseEventArgs e)
        {
            if (!Enabled)
            {
                return;
            }

            var point = e.Location;
            var button = e.Button;


            Dispatcher.InvokeAsync(() =>
            {

                var ratio = DpiHelper.GetPointDpiRatio(point);

                var indicator = GetClickIndicatorWindow();

                Brush brush = Brushes.DodgerBlue;

                switch (button)
                {
                    case MouseButtons.Left:
                        break;
                    case MouseButtons.Middle:
                        brush = Brushes.Green;
                        break;
                    case MouseButtons.Right:
                        brush = Brushes.OrangeRed;
                        break;
                    case MouseButtons.XButton1:
                        brush = Brushes.Gray;
                        break;
                    case MouseButtons.XButton2:
                        brush = Brushes.BlueViolet;
                        break;
                }

                indicator.Play(brush);

                var size = (int)(150 * ratio);
                MoveWindow(new WindowInteropHelper(indicator).Handle,
                    point.X - (int)(size / 2),
                    point.Y - (int)(size / 2), size, size, false);



            });
        }

        private void OnClosed(object sender, EventArgs e)
        {
            _mouseHook.MouseDown -= MouseHookOnMouseDown;
            _mouseHook.Stop();
           

            foreach (var x in _clickIndicators)
            {
                x.Close();
            }
        }



        [DllImport("user32.dll", SetLastError = true)]
        internal static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

        private void BtnClose_OnClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// 打开主页
        /// </summary>
        private void Hyperlink_OnRequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            try
            {
                Process.Start("https://github.com/cuiliang/clickshow");
            }
            catch (Exception ex)
            {
                MessageBox.Show("无法打开网址：https://github.com/cuiliang/clickshow");
            }
        }
    }
}
