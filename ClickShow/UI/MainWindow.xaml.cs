using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Navigation;
using System.Windows.Threading;
using ClickShow.Settings;
using ClickShow.UI;
using ClickShow.Utility;
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
        private const long UP_SHOW_DISTANCE = 200;  //移动超过多少像素后显示抬起特效
        private const long UP_TICKS_DELTA = 500;   //长按多久后抬起显示抬起特效


        private MouseHook.MouseHook _mouseHook;

        // 窗口缓存,解决连续点击的显示问题


        private NotifyIcon _notifyIcon = null;

        /// <summary>
        /// 强制关闭窗口
        /// </summary>
        private bool _forceClose = false;

        /// <summary>
        /// 程序设置
        /// </summary>
        public AppSetting AppSetting { get; private set; }


        #region 窗口事件

        public MainWindow()
        {
            InitializeComponent();


            Loaded += OnLoaded;
            Closing += OnClosing;
            Closed += OnClosed;
            StateChanged += OnStateChanged;

            CreateNotifyIcon();
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            LoadSettings();

            WindowState = WindowState.Minimized;

            // update auto startup checkbox
            LoadAutoStartStatus();

            // 设置
            PnlSettings.DataContext = AppSetting;
            

            //
            Title = $"ClickShow {Assembly.GetExecutingAssembly().GetName().Version.ToString()}";


            ApplySettings();


            // 最后启动挂钩
            _mouseHook = new MouseHook.MouseHook();
            _mouseHook.MouseDown += MouseHookOnMouseDown;
            _mouseHook.MouseMove += MouseHookOnMouseMove;
            _mouseHook.MouseUp += MouseHookOnMouseUp;
            _mouseHook.Start();

            // 检查版本更新
            Task.Run(async () =>
            {
                await Task.Delay(1000 * 60 * 1);
                CheckUpdate();
            });
            
        }

        /// <summary>
        /// 在主窗口上提示有新版本
        /// </summary>
        public bool ShowNewVersionTip { get; set; }
        public Version NewVersion { get; set; }

        /// <summary>
        /// 检查版本更新
        /// </summary>
        private void CheckUpdate()
        {
            string url = "https://raw.githubusercontent.com/cuiliang/ClickShow/main/version.txt";

            try
            {
                var client = new WebClient();
                var versionStr = client.DownloadString(url);
                NewVersion = new Version(versionStr);

                // 版本落后了
                if (NewVersion > Assembly.GetExecutingAssembly().GetName().Version)
                {
                    // 如果之前已经提示了此版本，则不再提示，不然每次开机都会有一个提示了。
                    if (String.Equals(AppSetting.LastNotifiedVersion, versionStr))
                    {
                        ShowNewVersionTip = true;
                    }
                    else
                    {
                        AppSetting.LastNotifiedVersion = versionStr;
                        SaveSettings();

                        if (MessageBox.Show($"ClickShow有新版本（{versionStr}），是否立即打开网页？", "ClickShow", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
                        {
                            try
                            {
                                Process.Start("https://github.com/cuiliang/ClickShow/releases");
                            }
                            catch
                            {
                                MessageBox.Show("无法打开网址：https://github.com/cuiliang/ClickShow/releases");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // ignore error
            }

        }

        /// <summary>
        /// 如果是点了x关闭窗口，则隐藏窗口而不是退出程序。
        /// </summary>
        private void OnClosing(object sender, CancelEventArgs e)
        {
            if (!_forceClose)
            {
                e.Cancel = true;
                this.Hide();
            }
        }

        private void OnClosed(object sender, EventArgs e)
        {
            SaveSettings();
            _mouseHook.MouseMove -= MouseHookOnMouseMove;
            _mouseHook.MouseDown -= MouseHookOnMouseDown;
            _mouseHook.Stop();

            _notifyIcon.Visible = false;

            foreach (var x in _clickIndicators)
            {
                x.Close();
            }

            _hoverDot?.Close();


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

        private void BtnClose_OnClick(object sender, RoutedEventArgs e)
        {
            _forceClose = true;
            this.Close();
        }

        /// <summary>
        /// 打开主页
        /// </summary>
        private void HyperlinkOpenHomepage_OnRequestNavigate(object sender, RequestNavigateEventArgs e)
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

        #endregion

        #region 鼠标位置浮标

        // 浮标窗口
        private HoverDot _hoverDot = null;

        /// <summary>
        /// 更新浮标的可见性
        /// </summary>
        private void UpdateHoverDot()
        {
            if (AppSetting.EnableHoverDot)
            {
                if (_hoverDot == null)
                {
                    _hoverDot = new HoverDot();
                }

                _hoverDot.Width = _hoverDot.Height = AppSetting.HoverDotSize;
                _hoverDot.SetDotBrush(AppSetting.HoverDotFill.ToBrush());

                _hoverDot.Show();
            }
            else
            {
                if (_hoverDot != null)
                {
                    _hoverDot.Close();
                    _hoverDot = null;
                }
            }
        }

        #endregion

        #region 点击波纹效果

        /// <summary>
        /// 可用的波纹窗口
        /// </summary>
        private IList<ClickIndicator> _clickIndicators = new List<ClickIndicator>();

        /// <summary>
        /// 各个鼠标按键对应的颜色画刷
        /// </summary>
        private IDictionary<MouseButtons, Brush> _buttonBrushes = new Dictionary<MouseButtons, Brush>()
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

            var indicator = _clickIndicators.FirstOrDefault(x => x.IsIdle 
                                                                 && x.Width > AppSetting.IndicatorSize -1
                                                                 && x.Width < AppSetting.IndicatorSize +1);
            if (indicator != null)
            {
                indicator.Prepare();

                KillDeadWindow();
                return indicator;
            }
            else
            {
                indicator = new ClickIndicator(AppSetting.IndicatorSize)
                {
                    WindowStartupLocation = WindowStartupLocation.Manual,
                    Topmost = true,
                    ShowActivated = false
                };
                _clickIndicators.Add(indicator);

                KillDeadWindow();
                return indicator;
            }
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

                var size = (int)(AppSetting.IndicatorSize * indicator.GetDpiScale());

                MoveWindowWrapper(indicator,
                    point.X - (int)(size / 2),
                    point.Y - (int)(size / 2), size, size);

                if (indicator.DpiHasChanged)
                {
                    size = (int)(AppSetting.IndicatorSize * indicator.GetDpiScale());
                    // 
                    MoveWindowWrapper(indicator,
                        point.X - (int)(size / 2),
                        point.Y - (int)(size / 2), size, size);
                }
            }
            catch
            {
            }
        }

        /// <summary>
        /// 关闭长时间未使用的indicator
        /// </summary>
        private void KillDeadWindow()
        {
            var deadIndicators = _clickIndicators
                .Where(x => x.IsIdle 
                            && (Environment.TickCount - x.LastLiveTime) / 1000 > 5)
                .ToList();

            foreach (var i in deadIndicators)
            {
                i.Close();
                _clickIndicators.Remove(i);
            }
        }


        #endregion

        #region 鼠标hook处理


        /// <summary>
        /// 鼠标按下事件
        /// </summary>
        private void MouseHookOnMouseDown(object sender, MouseEventArgs e)
        {
            if (!AppSetting.EnableClickCircle
                || !AppSetting.MouseButtonSettings[e.Button].IsEnabled)
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
        /// 鼠标抬起
        /// </summary>
        private void MouseHookOnMouseUp(object sender, MouseEventArgs e)
        {
            if (!AppSetting.EnableClickCircle
                || !AppSetting.MouseButtonSettings[e.Button].IsEnabled)
            {
                return;
            }

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

        /// <summary>
        /// 鼠标移动处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MouseHookOnMouseMove(object sender, MouseEventArgs e)
        {
            if (AppSetting.EnableHoverDot
                && _hoverDot != null)
            {
                Dispatcher.InvokeAsync(() =>
                {
                    var size = (int)(AppSetting.HoverDotSize * _hoverDot.GetDpiScale());
                    MoveWindowWrapper(_hoverDot,
                        e.Location.X - (int)(size / 2),
                        e.Location.Y - (int)(size / 2), size, size);
                });
            }
        }


        #endregion

        #region 托盘图标

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
            var menuItem = new System.Windows.Forms.MenuItem("退出(Exit)", (sender, args) =>
            {
                _forceClose = true;
                this.Close();
            });
            contextMenu.MenuItems.Add(menuItem);

            _notifyIcon.ContextMenu = contextMenu;
        }

        private void NotifyIconOnClick(object sender, EventArgs e)
        {

            this.Show();
            this.Activate();
            this.WindowState = WindowState.Normal;
        }

        #endregion


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

        /// <summary>
        /// 点击切换了自动启动选项
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        #region 设置读写

        /// <summary>
        /// 配置文件位置
        /// </summary>
        /// <returns></returns>
        private string GetSettingFilePath()
        {
            var docPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            return Path.Combine(docPath, "ClickShow.setting");
        }

        /// <summary>
        /// 读取设置
        /// </summary>
        private void LoadSettings()
        {
            string settingFilePath = GetSettingFilePath();
            if (File.Exists(settingFilePath))
            {
                try
                {
                    var data = File.ReadAllText(settingFilePath);
                    AppSetting = MyJsonConverter.Deserialize<AppSetting>(data);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("配置文件损坏了，设置已恢复为默认值。" + ex.Message);
                    AppSetting = new AppSetting();

                    SaveSettings();
                }

            }
            else
            {
                AppSetting = new AppSetting();
            }

            // 监听设置变更消息
            AppSetting.PropertyChanged += OnAppSettingChanged;
            foreach (var item in AppSetting.MouseButtonSettings)
            {
                item.Value.PropertyChanged += OnAppSettingChanged;
            }
        }


        /// <summary>
        /// 延迟更新
        /// </summary>
        private DebounceDispatcher _settingDebounceDispatcher;

        /// <summary>
        /// 设置改变后立即应用并保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnAppSettingChanged(object sender, PropertyChangedEventArgs e)
        {
            if (_settingDebounceDispatcher == null)
            {
                _settingDebounceDispatcher = new DebounceDispatcher();
            }

            _settingDebounceDispatcher.Debounce(100, o =>
            {
                SaveSettings();
                ApplySettings();
            });
        }

        /// <summary>
        /// 应用设置
        /// </summary>
        private void ApplySettings()
        {
            void ApplySettingsInternal()
            {
                UpdateHoverDot();

                foreach (var pair in AppSetting.MouseButtonSettings)
                {
                    _buttonBrushes[pair.Key] = pair.Value.Color.ToBrush();
                }
            }

            try
            {
                ApplySettingsInternal();
            }
            catch (Exception ex)
            {

                _notifyIcon.ShowBalloonTip(2000,"ClickShow", "应用设置出错，已重置设置。错误：" + ex.Message, ToolTipIcon.Warning);

                //如果遇到了问题，重置设置
                AppSetting = new AppSetting();

                ApplySettingsInternal();

                SaveSettings();
            }

        }

        /// <summary>
        /// 保存设置
        /// </summary>
        private void SaveSettings()
        {
            try
            {
                File.WriteAllText(GetSettingFilePath(), MyJsonConverter.Serialize(AppSetting));
            }
            catch (Exception ex)
            {
                _notifyIcon.ShowBalloonTip(2000, "ClickShow", "设置保存出错：" + ex.Message, ToolTipIcon.Warning);
            }
            
        }

#endregion



        private void BtnSettings_OnClick(object sender, RoutedEventArgs e)
        {
            var dlg = new SettingsWindow(AppSetting);
            dlg.Owner = this;

            dlg.ShowDialog();
        }

    }
}
