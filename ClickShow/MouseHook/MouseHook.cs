using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using MouseEventArgs = System.Windows.Forms.MouseEventArgs;
using MouseEventHandler = System.Windows.Forms.MouseEventHandler;

namespace ClickShow.MouseHook
{
    /*************************************************************************************
 * 文件名：MouseHook.cs
 * 创建人：CMF
 * 日期：2018-5-20
 * 描述：实现鼠标钩子安装
 * 使用:
        MouseHook mouseHook = new MouseHook();  //实例化鼠标钩子类
        KeyboardHook keyboardHook = new KeyboardHook();  //实例化键盘钩子类
        mouseHook.MouseMove += new MouseEventHandler(mouseHook_MouseMove);  
        mouseHook.MouseDown += new MouseEventHandler(mouseHook_MouseDown);  
        mouseHook.MouseUp += new MouseEventHandler(mouseHook_MouseUp);  
        mouseHook.MouseWheel += new MouseEventHandler(mouseHook_MouseWheel);  
        keyboardHook.KeyDown += new KeyEventHandler(keyboardHook_KeyDown);  
        keyboardHook.KeyUp += new KeyEventHandler(keyboardHook_KeyUp);  
        keyboardHook.KeyPress += new KeyPressEventHandler(keyboardHook_KeyPress);  
        mouseHook.Start();  
        keyboardHook.Start(); 
************************************************************************************/

    

    /// <summary>  
    /// Captures global mouse events  
    /// </summary>  
    public class MouseHook : GlobalHook
    {

        

        #region MouseEventType Enum  

        private enum MouseEventType
        {
            None,
            MouseDown,
            MouseUp,
            DoubleClick,
            MouseWheel,
            MouseMove,
            MouseHWheel, // 水平滚动
        }

        #endregion

        #region Events  

        public event MouseEventHandler MouseDown;
        public event MouseEventHandler MouseUp;
        public event MouseEventHandler MouseMove;
        public event MouseEventHandler MouseWheel;
        public event MouseEventHandler MouseHWheel;

        public event EventHandler Click;
        public event EventHandler DoubleClick;

        #endregion

        #region Constructor  

        public MouseHook()
        {

            HookType = HookType.WH_MOUSE_LL;

        }

        #endregion

        #region Methods  

        protected override IntPtr HookCallbackProcedure(int nCode, IntPtr wParam, IntPtr lParam)
        {
            bool handled = false;

            try
            {
                handled = ProcessHook(nCode, wParam, lParam);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("处理鼠标回调消息出错！" + ex.Message, ex);
#if DEBUG
                throw;
#endif
            }



            if (handled)
            {
                return (IntPtr)1;
            }
            else
            {
                return NativeMethods.CallNextHookEx(HandleToHook, nCode, wParam, lParam);
            }
        }

        /// <summary>
        /// 处理回调消息
        /// </summary>
        /// <param name="nCode"></param>
        /// <param name="wParam"></param>
        /// <param name="lParam"></param>
        /// <returns></returns>
        private bool ProcessHook(int nCode, IntPtr wParam, IntPtr lParam)
        {
            bool handled = false;


            if (nCode > -1 && (MouseDown != null || MouseUp != null || MouseMove != null || MouseWheel != null ||
                               MouseHWheel != null))
            {
                // https://docs.microsoft.com/en-us/windows/desktop/api/winuser/ns-winuser-tagmsllhookstruct
                // https://docs.microsoft.com/zh-cn/windows/win32/api/winuser/ns-winuser-msllhookstruct?redirectedfrom=MSDN
                NativeMethods.MouseLLHookStruct mouseHookStruct =
                    (NativeMethods.MouseLLHookStruct)Marshal.PtrToStructure(lParam, typeof(NativeMethods.MouseLLHookStruct));

                MouseButtons button = GetButton((int)wParam, mouseHookStruct);
                MouseEventType eventType = GetEventType((int)wParam);

                //  https://docs.microsoft.com/en-us/windows/win32/tablet/system-events-and-mouse-messages
                const uint MI_WP_SIGNATURE = 0xFF515700;
                const uint SIGNATURE_MASK = 0xFFFFFF00;
                bool isPenOrTouch = ((uint)mouseHookStruct.dwExtraInfo & SIGNATURE_MASK) == MI_WP_SIGNATURE;

                var e = new MouseEventArgs(
                    button,
                    (eventType == MouseEventType.DoubleClick ? 2 : 1),
                    mouseHookStruct.pt.x,
                    mouseHookStruct.pt.y,
                    (eventType == MouseEventType.MouseWheel || eventType == MouseEventType.MouseHWheel
                        ? (short)((mouseHookStruct.mouseData >> 16) & 0xffff)
                        : 0))
                {
                  
                };



                switch (eventType)
                {
                    case MouseEventType.MouseDown:
                        MouseDown?.Invoke(this, e);
                        break;
                    case MouseEventType.MouseUp:
                        MouseUp?.Invoke(this, e);
                        Click?.Invoke(this, e);
                        break;
                    case MouseEventType.DoubleClick:
                        DoubleClick?.Invoke(this, e);
                        break;
                    case MouseEventType.MouseWheel:
                        MouseWheel?.Invoke(this, e);
                        break;
                    case MouseEventType.MouseHWheel:
                        MouseHWheel?.Invoke(this, e);
                        break;
                    case MouseEventType.MouseMove:
                        MouseMove?.Invoke(this, e);
                        break;
                    default:

                        Debug.WriteLine("未处理的鼠标消息：" + eventType);

                        break;
                }


            }

            return handled;
        }

        private static MouseButtons GetButton(int wParam, NativeMethods.MouseLLHookStruct mouseHookStruct)
        {

            switch (wParam)
            {

                case NativeMethods.WM_LBUTTONDOWN:
                case NativeMethods.WM_LBUTTONUP:
                case NativeMethods.WM_LBUTTONDBLCLK:
                    return MouseButtons.Left;
                case NativeMethods.WM_RBUTTONDOWN:
                case NativeMethods.WM_RBUTTONUP:
                case NativeMethods.WM_RBUTTONDBLCLK:
                    return MouseButtons.Right;
                case NativeMethods.WM_MBUTTONDOWN:
                case NativeMethods.WM_MBUTTONUP:
                case NativeMethods.WM_MBUTTONDBLCLK:
                    return MouseButtons.Middle;
                case NativeMethods.WM_XBUTTONDOWN:
                case NativeMethods.WM_XBUTTONUP:
                    if ((mouseHookStruct.mouseData & 0x010000) > 0)
                    {
                        return MouseButtons.XButton1;
                    }
                    else if ((mouseHookStruct.mouseData & 0x020000) > 0)
                    {
                        return MouseButtons.XButton2;
                    }
                    else
                    {
                        return MouseButtons.XButton1;
                    }

                default:
                    return MouseButtons.None;

            }
        }

        private static MouseEventType GetEventType(int wParam)
        {

            switch (wParam)
            {

                case NativeMethods.WM_LBUTTONDOWN:
                case NativeMethods.WM_RBUTTONDOWN:
                case NativeMethods.WM_MBUTTONDOWN:
                case NativeMethods.WM_XBUTTONDOWN:
                    return MouseEventType.MouseDown;
                case NativeMethods.WM_LBUTTONUP:
                case NativeMethods.WM_RBUTTONUP:
                case NativeMethods.WM_MBUTTONUP:
                case NativeMethods.WM_XBUTTONUP:
                    return MouseEventType.MouseUp;

                case NativeMethods.WM_LBUTTONDBLCLK:
                case NativeMethods.WM_RBUTTONDBLCLK:
                case NativeMethods.WM_MBUTTONDBLCLK:
                    return MouseEventType.DoubleClick;
                case NativeMethods.WM_MOUSEWHEEL:
                    return MouseEventType.MouseWheel;
                case NativeMethods.WM_MOUSEHWHEEL:
                    return MouseEventType.MouseHWheel;
                case NativeMethods.WM_MOUSEMOVE:
                    return MouseEventType.MouseMove;
                default:
                    return MouseEventType.None;

            }
        }

        #endregion

        /// <summary>
        /// 停止挂钩处理
        /// </summary>
        public override void Stop()
        {
            base.Stop();

        }


    }
}
