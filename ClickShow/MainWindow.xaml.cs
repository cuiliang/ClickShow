using Gma.System.MouseKeyHook;
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

namespace ClickShow
{


    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private IKeyboardMouseEvents m_GlobalHook;

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

            // Note: for the application hook, use the Hook.AppEvents() instead
            m_GlobalHook = Hook.GlobalEvents();

            m_GlobalHook.MouseDownExt += GlobalHookMouseDownExt;

            Closed += OnClosed;

            Loaded += (sender, args) =>
            {
                WindowState = WindowState.Minimized;
            };

        }

        private void OnClosed(object sender, EventArgs e)
        {

            m_GlobalHook.MouseDownExt -= GlobalHookMouseDownExt;

            //It is recommened to dispose it
            m_GlobalHook.Dispose();

            foreach (var x in _clickIndicators)
            {
                x.Close();
            }
        }

        private void GlobalHookMouseDownExt(object sender, MouseEventExtArgs e)
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

        private void BtnTest_OnClick(object sender, RoutedEventArgs e)
        {
            //var win = new ClickIndicator();
            //win.Show();
        }

        [DllImport("user32.dll", SetLastError = true)]
        internal static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

        private void BtnClose_OnClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
