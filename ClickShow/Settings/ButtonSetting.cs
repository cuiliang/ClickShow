using System.ComponentModel;
using System.Runtime.CompilerServices;
using ClickShow.Annotations;

namespace ClickShow.Settings
{
    /// <summary>
    /// 单个鼠标按钮的设置
    /// </summary>
    public class ButtonSetting: INotifyPropertyChanged
    {
        private bool _isEnabled = true;
        private string _color;

        public ButtonSetting()
        {
        }

        public ButtonSetting(bool isEnabled, string color)
        {
            IsEnabled = isEnabled;
            Color = color;
        }

        public ButtonSetting(string color)
            :this(true, color)
        {
            
        }

        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsEnabled
        {
            get => _isEnabled;
            set
            {
                _isEnabled = value; 
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// 波纹颜色
        /// </summary>
        public string Color
        {
            get => _color;
            set
            {
                _color = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}