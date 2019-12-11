using System;
using System.Runtime.InteropServices;

namespace Labyrinth.Support.Interop {
    public static class X11 {
        public enum PropertyMode {
            Replace			= 0,
            Prepend			= 1,
            Append			= 2
        }

        [Flags]
        public enum MotifFlags {
            Functions = 1,
            Decorations = 2,
            InputMode = 4,
            Status = 8
        }

        [Flags]
        public enum MotifFunctions {
            All = 0x01,
            Resize = 0x02,
            Move = 0x04,
            Minimize = 0x08,
            Maximize = 0x10,
            Close = 0x20
        }

        [Flags]
        public enum MotifDecorations {
            All = 0x01,
            Border = 0x02,
            ResizeH = 0x04,
            Title = 0x08,
            Menu = 0x10,
            Minimize = 0x20,
            Maximize = 0x40,
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MotifWmHints {
            public IntPtr flags;
            public IntPtr functions;
            public IntPtr decorations;
            public IntPtr input_mode;
            public IntPtr status;
        }


        [DllImport("X11")]
        public static extern int XChangeProperty(IntPtr display, IntPtr window, IntPtr property, IntPtr type,
            int format, PropertyMode mode, ref MotifWmHints data, int nElements);
    }
}
