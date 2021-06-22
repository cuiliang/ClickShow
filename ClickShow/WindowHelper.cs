using System;
using System.Runtime.InteropServices;

namespace ClickShow
{
    public class WindowHelper
    {
        private const int WS_EX_TRANSPARENT = 0x00000020;
        private const int WS_EX_NOACTIVATE = 0x08000000;
        private const int GWL_EXSTYLE = (-20);

        [DllImport("user32.dll")]
        private static extern int GetWindowLong(IntPtr hwnd, int index);

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hwnd, int index, int newStyle);

        /// <summary>
        /// 让鼠标事件穿透
        /// https://stackoverflow.com/questions/2842667/how-to-create-a-semi-transparent-window-in-wpf-that-allows-mouse-events-to-pass
        /// </summary>
        /// <param name="hwnd"></param>
        public static void SetWindowExTransparent(IntPtr hwnd)
        {
            var extendedStyle = GetWindowLong(hwnd, GWL_EXSTYLE);
            SetWindowLong(hwnd, GWL_EXSTYLE, extendedStyle | WS_EX_TRANSPARENT | WS_EX_NOACTIVATE);
        }
    }
}