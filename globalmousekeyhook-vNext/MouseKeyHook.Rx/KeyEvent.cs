// This code is distributed under MIT license. 
// Copyright (c) 2010-2018 George Mamaladze
// See license.txt or https://mit-license.org/

using System.Windows.Forms;
using Gma.System.MouseKeyHook.Implementation;

namespace MouseKeyHook.Rx
{
    public struct KeyEvent
    {
        public KeyEvent(Keys keyCode, KeyEventKind kind = KeyEventKind.Down)
        {
            KeyCode = keyCode;
            Kind = kind;
        }

        public KeyEventKind Kind { get; }
        public Keys KeyCode { get; private set; }

        public override string ToString()
        {
            return string.Format("{0}-{1}", KeyCode, Kind);
        }

        public KeyEvent ApplyMofidifers(KeyboardState state)
        {
            if (state.IsDown(Keys.LControlKey) || state.IsDown(Keys.RControlKey)) KeyCode = KeyCode | Keys.Control;
            if (state.IsDown(Keys.LShiftKey) || state.IsDown(Keys.RShiftKey)) KeyCode = KeyCode | Keys.Shift;
            if (state.IsDown(Keys.LMenu) || state.IsDown(Keys.RMenu)) KeyCode = KeyCode | Keys.Alt;
            return this;
        }
    }
}