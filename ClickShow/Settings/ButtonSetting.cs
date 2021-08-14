namespace ClickShow.Settings
{
    /// <summary>
    /// 单个鼠标按钮的设置
    /// </summary>
    public class ButtonSetting
    {
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
        public bool IsEnabled { get; set; } = true;

        /// <summary>
        /// 波纹颜色
        /// </summary>
        public string Color { get; set; }
    }
}