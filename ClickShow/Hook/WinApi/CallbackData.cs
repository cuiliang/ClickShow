// This code is distributed under MIT license. 
// Copyright (c) 2015 George Mamaladze
// See license.txt or https://mit-license.org/

using System;

namespace Gma.System.MouseKeyHook.WinApi
{
    internal struct CallbackData
    {
        public CallbackData(IntPtr wParam, IntPtr lParam)
        {
            WParam = wParam;
            LParam = lParam;
        }

        public IntPtr WParam { get; }

        public IntPtr LParam { get; }
    }
}