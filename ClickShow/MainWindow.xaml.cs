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

namespace ClickShow
{


    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const double INDICATOR_SIZE = 150;
        private const double DOT_SIZE = 60;

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
                UpdateHoverDotVisibility();

                // Note: for the application hook, use the Hook.AppEvents() instead
                _mouseHook = new MouseHook.MouseHook();
                _mouseHook.MouseDown += MouseHookOnMouseDown;
                _mouseHook.MouseMove += MouseHookOnMouseMove;
                _mouseHook.Start();

                LoadAutoStartStatus();
            };
            StateChanged += OnStateChanged;


            

            CreateNotifyIcon();
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

        private void MouseHookOnMouseDown(object sender, MouseEventArgs e)
        {
            if (!EnableClickCircle)
            {
                return;
            }





            var point = e.Location;
            
            var button = e.Button;


            Dispatcher.InvokeAsync(() =>
            {
                try
                {
                    

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

                    var size = (int) (INDICATOR_SIZE * indicator.GetDpiScale());

                    MoveWindowWrapper(indicator,
                        point.X - (int) (size / 2),
                        point.Y - (int) (size / 2), size, size);



                    if (indicator.DpiHasChanged)
                    {
                        size = (int)(INDICATOR_SIZE * indicator.GetDpiScale());
                        // 
                        MoveWindowWrapper(indicator,
                            point.X - (int) (size / 2),
                            point.Y - (int) (size / 2), size, size);
                    }

                   
                }
                catch 
                {

                }

            });
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
