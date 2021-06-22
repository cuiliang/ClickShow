using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ClickShow
{
    /// <summary>
    /// Interaction logic for ClickIndicator.xaml
    /// </summary>
    public partial class ClickIndicator : Window
    {
        private Storyboard _storyboard;

        public ClickIndicator()
        {
            ShowActivated = false;
            InitializeComponent();

            SourceInitialized += OnSourceInitialized;

            RenderOptions.SetBitmapScalingMode(TheCircle, BitmapScalingMode.LowQuality);

            // 初始化动画
            double interval = 0.4;
            _storyboard = new Storyboard();
            _storyboard.FillBehavior = FillBehavior.Stop;


            var widthAnimation = new DoubleAnimation(toValue: this.Width, new Duration(TimeSpan.FromSeconds(interval)));
            Storyboard.SetTargetProperty(widthAnimation, new PropertyPath("Width"));
            Storyboard.SetTarget(widthAnimation, TheCircle);
            _storyboard.Children.Add(widthAnimation);

            var heightAnimation = new DoubleAnimation(toValue: this.Height, new Duration(TimeSpan.FromSeconds(interval)));
            Storyboard.SetTargetProperty(heightAnimation, new PropertyPath("Height"));
            Storyboard.SetTarget(heightAnimation, TheCircle);
            _storyboard.Children.Add(heightAnimation);

            var opacityAnimation = new DoubleAnimation(toValue: 0, new Duration(TimeSpan.FromSeconds(interval)));
            Storyboard.SetTargetProperty(opacityAnimation, new PropertyPath("Opacity"));
            Storyboard.SetTarget(opacityAnimation, TheCircle);
            _storyboard.Children.Add(opacityAnimation);

            _storyboard.Completed += StoryboardOnCompleted;
            if (_storyboard.CanFreeze)
            {
                _storyboard.Freeze();
            }

            //Play();
        }


        public bool IsIdle { get; private set; } = false;

        public void Prepare()
        {
            IsIdle = false;
        }

        public void Play(Brush circleBrush)
        {
            Opacity = 1;
            TheCircle.Stroke = circleBrush;

            IsIdle = false;

            _storyboard.Begin();

            this.Show();
        }

        private void OnSourceInitialized(object sender, EventArgs e)
        {
            SetWindowExTransparent(new WindowInteropHelper(this).Handle);
        }

        private void StoryboardOnCompleted(object sender, EventArgs e)
        {

            TheCircle.Width = 0;
            TheCircle.Height = 0;
            Opacity = 0;

            IsIdle = true;

            //this.Top = -3000;
            //this.Left = -3000;
            //InvalidateVisual();

            //Task.Run(() =>
            //{
            //    Dispatcher.InvokeAsync(() =>
            //    {
            //     //   this.Hide();

            //        IsIdle = true;
            //    });
            //});


        }

        
        

        const int WS_EX_TRANSPARENT = 0x00000020;
        const int GWL_EXSTYLE = (-20);

        [DllImport("user32.dll")]
        static extern int GetWindowLong(IntPtr hwnd, int index);

        [DllImport("user32.dll")]
        static extern int SetWindowLong(IntPtr hwnd, int index, int newStyle);

        /// <summary>
        /// 让鼠标事件穿透
        /// https://stackoverflow.com/questions/2842667/how-to-create-a-semi-transparent-window-in-wpf-that-allows-mouse-events-to-pass
        /// </summary>
        /// <param name="hwnd"></param>
        public static void SetWindowExTransparent(IntPtr hwnd)
        {
            var extendedStyle = GetWindowLong(hwnd, GWL_EXSTYLE);
            SetWindowLong(hwnd, GWL_EXSTYLE, extendedStyle | WS_EX_TRANSPARENT);
        }
    }
}
