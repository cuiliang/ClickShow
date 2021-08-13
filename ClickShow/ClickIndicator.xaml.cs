using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
        private Storyboard _mouseDownStoryBoard;
        private Storyboard _mouseUpStoryBoard;
        public int LastLiveTime { get; set; } = Environment.TickCount;
        public ClickIndicator(double size)
        {
            ShowActivated = false;
            InitializeComponent();
            this.Width = size;
            this.Height = size;

            //SourceInitialized += OnSourceInitialized;
            DpiChanged += OnDpiChanged;

            RenderOptions.SetBitmapScalingMode(TheCircle, BitmapScalingMode.LowQuality);

            CreateMouseDownStoryBoard();
            CreateMouseUpStoryBoard();
            //Play();
        }

        /// <summary>
        /// 鼠标抬起特效
        /// </summary>
        private void CreateMouseUpStoryBoard()
        {
            // 初始化动画
            double interval = 0.3;
            _mouseUpStoryBoard = new Storyboard();
            _mouseUpStoryBoard.FillBehavior = FillBehavior.Stop;


            var widthAnimation = new DoubleAnimation(toValue: this.Width / 2, new Duration(TimeSpan.FromSeconds(interval)));
            Storyboard.SetTargetProperty(widthAnimation, new PropertyPath("Width"));
            Storyboard.SetTarget(widthAnimation, TheCircle);
            _mouseUpStoryBoard.Children.Add(widthAnimation);

            var heightAnimation = new DoubleAnimation(toValue: this.Height / 2, new Duration(TimeSpan.FromSeconds(interval)));
            Storyboard.SetTargetProperty(heightAnimation, new PropertyPath("Height"));
            Storyboard.SetTarget(heightAnimation, TheCircle);
            _mouseUpStoryBoard.Children.Add(heightAnimation);

            var opacityAnimation = new DoubleAnimation(toValue: 0, new Duration(TimeSpan.FromSeconds(interval)));
            Storyboard.SetTargetProperty(opacityAnimation, new PropertyPath("Opacity"));
            Storyboard.SetTarget(opacityAnimation, TheCircle);
            _mouseUpStoryBoard.Children.Add(opacityAnimation);

            _mouseUpStoryBoard.Completed += MouseDownStoryBoardOnCompleted;
            if (_mouseUpStoryBoard.CanFreeze)
            {
                _mouseUpStoryBoard.Freeze();
            }
        }

        /// <summary>
        /// 鼠标按下特效
        /// </summary>
        private void CreateMouseDownStoryBoard()
        {
            // 初始化动画
            double interval = 0.4;
            _mouseDownStoryBoard = new Storyboard();
            _mouseDownStoryBoard.FillBehavior = FillBehavior.Stop;


            var widthAnimation = new DoubleAnimation(toValue: this.Width, new Duration(TimeSpan.FromSeconds(interval)));
            Storyboard.SetTargetProperty(widthAnimation, new PropertyPath("Width"));
            Storyboard.SetTarget(widthAnimation, TheCircle);
            _mouseDownStoryBoard.Children.Add(widthAnimation);

            var heightAnimation = new DoubleAnimation(toValue: this.Height, new Duration(TimeSpan.FromSeconds(interval)));
            Storyboard.SetTargetProperty(heightAnimation, new PropertyPath("Height"));
            Storyboard.SetTarget(heightAnimation, TheCircle);
            _mouseDownStoryBoard.Children.Add(heightAnimation);

            var opacityAnimation = new DoubleAnimation(toValue: 0, new Duration(TimeSpan.FromSeconds(interval)));
            Storyboard.SetTargetProperty(opacityAnimation, new PropertyPath("Opacity"));
            Storyboard.SetTarget(opacityAnimation, TheCircle);
            _mouseDownStoryBoard.Children.Add(opacityAnimation);

            _mouseDownStoryBoard.Completed += MouseDownStoryBoardOnCompleted;
            if (_mouseDownStoryBoard.CanFreeze)
            {
                _mouseDownStoryBoard.Freeze();
            }
        }

        private void OnDpiChanged(object sender, DpiChangedEventArgs e)
        {
            DpiHasChanged = true;
            _currentDpi = e.NewDpi;
        }

        public double GetDpiScale()
        {
            if (_currentDpi.DpiScaleX < 0.1)
            {
                _currentDpi = VisualTreeHelper.GetDpi(this);


            }

            return _currentDpi.DpiScaleX;
        }


        public bool IsIdle { get; private set; } = false;

        private DpiScale _currentDpi;



        public bool DpiHasChanged { get; private set; } = false;

        public void Prepare()
        {
            IsIdle = false;
        }

        public void Play(Brush circleBrush, bool isDown)
        {
            this.LastLiveTime = Environment.TickCount;
            Opacity = isDown ? 0.95 : 0.7;

            // 抬起特效


            TheCircle.Stroke = circleBrush;

            IsIdle = false;

            if (isDown)
            {
                TheCircle.Width = this.Width * 0.2;
                TheCircle.Height = this.Height * 0.2;
                _mouseDownStoryBoard.Begin();
            }
            else
            {
                _mouseUpStoryBoard.Begin();
            }


            this.Show();
        }



        private void OnSourceInitialized(object sender, EventArgs e)
        {
            WindowHelper.SetWindowExTransparent(new WindowInteropHelper(this).Handle);
        }

        private void MouseDownStoryBoardOnCompleted(object sender, EventArgs e)
        {

            TheCircle.Width = 0;
            TheCircle.Height = 0;
            this.Opacity = 0;

            IsIdle = true;
            DpiHasChanged = false;
        }
    }
}
