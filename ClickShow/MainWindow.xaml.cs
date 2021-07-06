using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
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
using System.Windows.Threading;
using Microsoft.Win32;
using Brush = System.Windows.Media.Brush;
using Brushes = System.Windows.Media.Brushes;
using Exception = System.Exception;
using MessageBox = System.Windows.MessageBox;
using MouseEventArgs = System.Windows.Forms.MouseEventArgs;
using MouseButtonState = ClickShow.Entities.MouseButtonState;
using Point = System.Drawing.Point;

namespace ClickShow
{


    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const double INDICATOR_SIZE = 150;  //波纹窗口大小
        private const double DOT_SIZE = 60;         //鼠标位置悬浮标大小
        private const long UP_SHOW_DISTANCE = 200;  //移动超过多少像素后显示抬起特效
        private const long UP_TICKS_DELTA = 500;   //长按多久后抬起显示抬起特效

        private MouseHook.MouseHook _mouseHook;

        // 窗口缓存,解决连续点击的显示问题
        private IList<ClickIndicator> _clickIndicators = new List<ClickIndicator>();

        private HoverDot _hoverDot = new HoverDot();

        private System.Windows.Forms.NotifyIcon _notifyIcon = null;

        #region 是否启用点击圈


        public static readonly DependencyProperty EnableClickCircleProperty = DependencyProperty.Register(
            "EnableClickCircle", typeof(bool), typeof(MainWindow), new PropertyMetadata(true));

        /// <summary>
        /// 是否开启显示
        /// </summary>
        public bool EnableClickCircle
        {
            get { return (bool)GetValue(EnableClickCircleProperty); }
            set { SetValue(EnableClickCircleProperty, value); }
        }

        #endregion

        #region 是否启用悬浮标

        public static readonly DependencyProperty EnableHoverProperty = DependencyProperty.Register(
            "EnableHover", typeof(bool), typeof(MainWindow), new PropertyMetadata(false, EnableHoverDotChanged));

        private static void EnableHoverDotChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as MainWindow).UpdateHoverDotVisibility();
        }

        private void UpdateHoverDotVisibility()
        {
            if (EnableHover)
            {
                _hoverDot.Show();
            }
            else
            {
                _hoverDot.Hide();
            }
        }

        public bool EnableHover
        {
            get { return (bool) GetValue(EnableHoverProperty); }
            set { SetValue(EnableHoverProperty, value); }
        }

        #endregion

        /// <summary>
        /// 各个鼠标按键对应的颜色画刷
        /// </summary>
        private readonly IDictionary<MouseButtons, Brush> _buttonBrushes = new Dictionary<MouseButtons, Brush>()
        {
            {MouseButtons.None, Brushes.Black},
            {MouseButtons.Left, Brushes.DodgerBlue},
            {MouseButtons.Middle, Brushes.Green},
            {MouseButtons.Right, Brushes.OrangeRed},
            {MouseButtons.XButton1, Brushes.Gray},
            {MouseButtons.XButton2, Brushes.BlueViolet},
        };

        /// <summary>
        /// 各按键的状态，用于判断是否应该显示抬起特效。
        /// </summary>
        private readonly IDictionary<MouseButtons, MouseButtonState> _buttonStates =
            new Dictionary<MouseButtons, MouseButtonState>()
            {
                {MouseButtons.Left, new MouseButtonState()},
                {MouseButtons.Middle, new MouseButtonState()},
                {MouseButtons.Right, new MouseButtonState()},
                {MouseButtons.XButton1, new MouseButtonState()},
                {MouseButtons.XButton2, new MouseButtonState()},
            };

        /// <summary>
        /// 获取一个可用的波纹窗口。
        /// </summary>
        /// <returns>波纹窗口对象</returns>
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

            Loaded += OnLoaded;

            StateChanged += OnStateChanged;


            

            CreateNotifyIcon();
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {

            WindowState = WindowState.Minimized;

            UpdateHoverDotVisibility();

            // 
            _mouseHook = new MouseHook.MouseHook();
            _mouseHook.MouseDown += MouseHookOnMouseDown;
            _mouseHook.MouseMove += MouseHookOnMouseMove;
            _mouseHook.MouseUp += MouseHookOnMouseUp;
            _mouseHook.Start();

            // update auto startup checkbox
            LoadAutoStartStatus();

            //
            Title = $"ClickShow {Assembly.GetExecutingAssembly().GetName().Version.ToString()}";
        }

        /// <summary>
        /// 鼠标抬起
        /// </summary>
        private void MouseHookOnMouseUp(object sender, MouseEventArgs e)
        {
            var point = e.Location;
            var downState = _buttonStates[e.Button];

            // 距离超过设定，或者抬起时间超过设定，显示抬起特效。
            if (((point.X - downState.DownPosition.X) * (point.X - downState.DownPosition.X)
                 + (point.Y - downState.DownPosition.Y) * (point.Y - downState.DownPosition.Y)) > UP_SHOW_DISTANCE * UP_SHOW_DISTANCE
                || (Environment.TickCount > (downState.DownTimeTicks + UP_TICKS_DELTA))
                )
            {
                Dispatcher.InvokeAsync(() =>
                {
                    ShowIndicator(e.Button, point, false);
                });
            }
        }

        private void CreateNotifyIcon()
        {
            _notifyIcon = new System.Windows.Forms.NotifyIcon();
            using (Stream iconStream = System.Windows.Application
                .GetResourceStream(new Uri(
                    $"pack://application:,,,/{Assembly.GetEntryAssembly().GetName().Name};component/clickshow.ico"))
                .Stream)
            {
                _notifyIcon.Icon = new System.Drawing.Icon(iconStream);
            }

            _notifyIcon.BalloonTipText = "ClickShow\n鼠标点击提示器\n点击打开";
            _notifyIcon.Click += NotifyIconOnClick;
            _notifyIcon.Visible = true;

            var contextMenu = new System.Windows.Forms.ContextMenu();
            var menuItem = new System.Windows.Forms.MenuItem("退出(Exit)", (sender, args) => { this.Close(); });
            contextMenu.MenuItems.Add(menuItem);

            _notifyIcon.ContextMenu = contextMenu;
        }

        /// <summary>
        /// 窗口状态改变了
        /// </summary>
        private void OnStateChanged(object sender, EventArgs e)
        {
            if (WindowState == WindowState.Minimized)
            {
                this.Hide();
            }
        }

        private void NotifyIconOnClick(object sender, EventArgs e)
        {
           
            this.Show();
            this.Activate();
            this.WindowState = WindowState.Normal;
        }

        /// <summary>
        /// 鼠标按下事件
        /// </summary>
        private void MouseHookOnMouseDown(object sender, MouseEventArgs e)
        {
            if (!EnableClickCircle)
            {
                return;
            }


            var point = e.Location;
            
            var button = e.Button;

            // 记录按下状态（时间与位置）
            _buttonStates[button].DownPosition = point;
            _buttonStates[button].DownTimeTicks = Environment.TickCount;


            // 显示特效
            Dispatcher.InvokeAsync(() => { ShowIndicator(button, point, true); });
        }

        /// <summary>
        /// 显示波纹特效
        /// </summary>
        /// <param name="button">按键</param>
        /// <param name="point">位置</param>
        /// <param name="isDown">是否按下</param>
        private void ShowIndicator(MouseButtons button, Point point, bool isDown)
        {
            try
            {
                var indicator = GetClickIndicatorWindow();

                Brush brush = _buttonBrushes[button];

                indicator.Play(brush, isDown);

                var size = (int) (INDICATOR_SIZE * indicator.GetDpiScale());

                MoveWindowWrapper(indicator,
                    point.X - (int) (size / 2),
                    point.Y - (int) (size / 2), size, size);

                if (indicator.DpiHasChanged)
                {
                    size = (int) (INDICATOR_SIZE * indicator.GetDpiScale());
                    // 
                    MoveWindowWrapper(indicator,
                        point.X - (int) (size / 2),
                        point.Y - (int) (size / 2), size, size);
                }
            }
            catch
            {
            }
        }

        /// <summary>
        /// 鼠标移动处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MouseHookOnMouseMove(object sender, MouseEventArgs e)
        {
            if (EnableHover)
            {
                Dispatcher.InvokeAsync(() =>
                {
                    var size = (int) (DOT_SIZE * _hoverDot.GetDpiScale());
                    MoveWindowWrapper(_hoverDot,
                        e.Location.X - (int) (size / 2),
                        e.Location.Y - (int) (size / 2), size, size);
                });
            }
        }

        private void OnClosed(object sender, EventArgs e)
        {
            _mouseHook.MouseMove -= MouseHookOnMouseMove;
            _mouseHook.MouseDown -= MouseHookOnMouseDown;
            _mouseHook.Stop();

            _notifyIcon.Visible = false;

            foreach (var x in _clickIndicators)
            {
                x.Close();
            }

            _hoverDot.Close();
        }


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
            catch 
            {
                MessageBox.Show("无法打开网址：https://github.com/cuiliang/clickshow");
            }
        }


        #region 自动启动

        // The path to the key where Windows looks for startup applications
        RegistryKey rkApp = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
        private string AppName = "ClickShow";
        private void LoadAutoStartStatus()
        {
            try
            {
                // Check to see the current state (running at startup or not)
                if (rkApp.GetValue(AppName) == null)
                {
                    // The value doesn't exist, the application is not set to run at startup
                    ChkStartWithWindows.IsChecked = false;
                }
                else
                {
                    // The value exists, the application is set to run at startup
                    ChkStartWithWindows.IsChecked = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("无法加载开机自动启动状态。" + ex.Message);
            }
        }


        private void SaveAutoStart()
        {
            try
            {
                if (ChkStartWithWindows.IsChecked == true)
                {
                    // Add the value in the registry so that the application runs at startup
                    rkApp.SetValue(AppName, Process.GetCurrentProcess().MainModule.FileName);
                }
                else
                {
                    // Remove the value from the registry so that the application doesn't start
                    rkApp.DeleteValue(AppName, false);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("无法保存开机自动启动状态。" + ex.Message);
            }
            
        }

        private void ChkStartWithWindows_OnClick(object sender, RoutedEventArgs e)
        {
            SaveAutoStart();
        }
        #endregion


        #region Native 调用

        public void MoveWindowWrapper(Window window, int X, int Y, int nWidth, int nHeight)
        {
            var handle = new WindowInteropHelper(window).Handle;

            WindowHelper.SetWindowPos(handle,
                (IntPtr)WindowHelper.SpecialWindowHandles.HWND_TOPMOST,
                X, Y,
                nWidth, nHeight,
                WindowHelper.SetWindowPosFlags.SWP_NOACTIVATE
                );

            //MoveWindow(handle, X, Y, nWidth, nHeight, false);
        }

        [DllImport("user32.dll", SetLastError = true)]
        internal static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

        #endregion

       
    }
}
