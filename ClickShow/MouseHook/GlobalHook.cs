using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;

namespace ClickShow.MouseHook
{
    /// <summary>  
    /// Abstract base class for Mouse and Keyboard hooks  
    /// </summary>  
    public abstract class GlobalHook
    {
        #region Private Variables  

        protected HookType HookType { get; set; }
        protected IntPtr HandleToHook { get; set; }
        protected NativeMethods.HookProc HookCallback { get; set; }
        public bool IsStarted { get; set; }
        #endregion



        #region Constructor  

        public GlobalHook()
        {


            //Application.Current.Exit += CurrentApplicationOnExit;
        }

        private void CurrentApplicationOnExit(object sender, ExitEventArgs e)
        {
            if (IsStarted)
            {
                Stop();
            }

        }

        #endregion

        #region Methods  

        public void Start()
        {

            if (!IsStarted &&
                HookType != 0)
            {

                // Make sure we keep a reference to this delegate!  
                // If not, GC randomly collects it, and a NullReference exception is thrown  
                HookCallback = new NativeMethods.HookProc(HookCallbackProcedure);

                HandleToHook = NativeMethods.SetWindowsHookEx(
                    HookType,
                    HookCallback,
                    Marshal.GetHINSTANCE(Assembly.GetExecutingAssembly().GetModules()[0]),
                    0);

                // Were we able to sucessfully start hook?  
                if (HandleToHook != IntPtr.Zero)
                {
                    IsStarted = true;
                }
                else
                {
#if DEBUG
                    throw new InvalidOperationException("鼠标hook建立失败。");
#endif
                }

            }

        }

        public virtual void Stop()
        {

            if (IsStarted)
            {

#pragma warning disable CA1806 // Do not ignore method results
                NativeMethods.UnhookWindowsHookEx(HandleToHook);
#pragma warning restore CA1806 // Do not ignore method results

                IsStarted = false;

            }

        }

        protected virtual IntPtr HookCallbackProcedure(int nCode, IntPtr wParam, IntPtr lParam)
        {

            // This method must be overriden by each extending hook  
            return (IntPtr)0;

        }

        //protected void Application_ApplicationExit(object sender, EventArgs e)
        //{

        //    if (IsStarted)
        //    {
        //        Stop();
        //    }

        //}

        #endregion


        public void Restart()
        {
            Debug.WriteLine("重新加载挂钩！");
            Stop();
            Start();
        }
    }
}
