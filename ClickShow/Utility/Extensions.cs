using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace ClickShow.Utility
{
    public static class Extensions
    {
        /// <summary>
        /// 转换颜色为ARGB格式
        /// </summary>
        /// <param name="c">颜色值</param>
        /// <returns>#AARRGGBB文本</returns>
        public static string ToArgb(this System.Windows.Media.Color c)
        {
            return $"#{c.A:X2}{c.R:X2}{c.G:X2}{c.B:X2}";
        }

        /// <summary>
        /// 颜色值转换为画刷对象
        /// </summary>
        /// <param name="colorStr">颜色值</param>
        /// <returns></returns>
        public static SolidColorBrush ToBrush(this string colorStr)
        {
            return new SolidColorBrush((Color)ColorConverter.ConvertFromString(colorStr));
        }
    }
}
