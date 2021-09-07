using System;
using System.CodeDom;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media;
using ClickShow.Annotations;
using ClickShow.Utility;

namespace ClickShow.Settings
{
    /// <summary>
    /// 设置信息
    /// </summary>
    public class AppSetting : INotifyPropertyChanged
    {
        private bool _enableClickCircle = true;
        private bool _enableHoverDot = false;
        private Dictionary<MouseButtons, ButtonSetting> _mouseButtonSettings = new Dictionary<MouseButtons, ButtonSetting>()
        {
            {MouseButtons.Left, new ButtonSetting(Colors.DodgerBlue.ToArgb())},
            {MouseButtons.Middle, new ButtonSetting(Colors.Green.ToArgb())},
            {MouseButtons.Right, new ButtonSetting(Colors.OrangeRed.ToArgb())},
            {MouseButtons.XButton1, new ButtonSetting(Colors.Gray.ToArgb())},
            {MouseButtons.XButton2, new ButtonSetting(Colors.BlueViolet.ToArgb())}   
        };

        private double _indicatorSize = 150;
        private double _hoverDotSize = 60;
        private string _hoverDotFill = "#60FFFF5A";

        /// <summary>
        /// 开启波纹特效
        /// </summary>
        public bool EnableClickCircle
        {
            get => _enableClickCircle;
            set
            {
                _enableClickCircle = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// 开启浮标显示
        /// </summary>
        public bool EnableHoverDot
        {
            get => _enableHoverDot;
            set
            {
                _enableHoverDot = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// 鼠标按键设置
        /// </summary>
        public Dictionary<MouseButtons, ButtonSetting> MouseButtonSettings
        {
            get => _mouseButtonSettings;
            set => _mouseButtonSettings = value;
        }

        /// <summary>
        /// 波纹尺寸
        /// </summary>
        public double IndicatorSize
        {
            get => _indicatorSize;
            set
            {
                _indicatorSize = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// 浮标尺寸
        /// </summary>
        public double HoverDotSize
        {
            get => _hoverDotSize;
            set
            {
                _hoverDotSize = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// 浮标颜色
        /// </summary>
        public string HoverDotFill
        {
            get => _hoverDotFill;
            set
            {
                _hoverDotFill = value; 
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// 上次提醒的版本
        /// </summary>
        public string LastNotifiedVersion { get; set; }
    }
}
