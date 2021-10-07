using System;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace HolydayRadar
{
    public class ScreenCapture
    {
        public static Bitmap FullScreen()
        {
            return ScreenCapture.ScreenRectangle(SystemInformation.VirtualScreen);
        }

        public static Bitmap DisplayMonitor(Screen monitor)
        {
            Rectangle rect;
            try
            {
                rect = monitor.Bounds;
            }
            catch (Exception ex)
            {
                throw new ArgumentException("Invalid parameter.", "monitor", ex);
            }
            return ScreenCapture.ScreenRectangle(monitor.Bounds);
        }

        public static Bitmap ActiveWindow()
        {
            IntPtr hwnd;
            Win32.RECT wRect = new Win32.RECT();
            hwnd = Win32.GetForegroundWindow();
            if (hwnd != IntPtr.Zero)
            {
                if (Win32.GetWindowRect(hwnd, ref wRect))
                {
                    Rectangle rect = new Rectangle(wRect.Left, wRect.Top, wRect.Right - wRect.Left, wRect.Bottom - wRect.Top);
                    return ScreenCapture.ScreenRectangle(rect);
                }
                else
                {
                    throw new System.ComponentModel.Win32Exception();
                }
            }
            else
            {
                throw new Exception("Could not find any active window.");
            }
        }

        public static Bitmap Window(IntPtr hwnd)
        {
            Win32.RECT wRect = new Win32.RECT();
            if (hwnd != IntPtr.Zero)
            {
                if (Win32.GetWindowRect(hwnd, ref wRect))
                {
                    Rectangle rect = new Rectangle(wRect.Left, wRect.Top, wRect.Right - wRect.Left, wRect.Bottom - wRect.Top);
                    return ScreenCapture.ScreenRectangle(rect);
                }
                else
                {
                    throw new System.ComponentModel.Win32Exception();
                }
            }
            else
            {
                throw new ArgumentException("Invalid window handle.", "hwnd");
            }
        }

        public static Bitmap Control(Point p)
        {
            IntPtr hwnd;
            Win32.RECT wRect = new Win32.RECT();
            hwnd = Win32.WindowFromPoint(p);
            if (hwnd != IntPtr.Zero)
            {
                if (Win32.GetWindowRect(hwnd, ref wRect))
                {
                    Rectangle rect = new Rectangle(wRect.Left, wRect.Top, wRect.Right - wRect.Left, wRect.Bottom - wRect.Top);
                    return ScreenCapture.ScreenRectangle(rect);
                }
                else
                {
                    throw new System.ComponentModel.Win32Exception();
                }
            }
            else
            {
                throw new Exception("Could not find any window at the specified point.");
            }
        }

        public static Bitmap Control(IntPtr hwnd)
        {
            Win32.RECT wRect = new Win32.RECT();
            if (hwnd != IntPtr.Zero)
            {
                if (Win32.GetWindowRect(hwnd, ref wRect))
                {
                    Rectangle rect = new Rectangle(wRect.Left, wRect.Top, wRect.Right - wRect.Left, wRect.Bottom - wRect.Top);
                    return ScreenCapture.ScreenRectangle(rect);
                }
                else
                {
                    throw new System.ComponentModel.Win32Exception();
                }
            }
            else
            {
                throw new ArgumentException("Invalid control handle.", "hwnd");
            }
        }

        public static Bitmap ScreenRectangle(Rectangle rect)
        {
            if (!(rect.IsEmpty) && rect.Width != 0 && rect.Height != 0)
            {
                System.ComponentModel.Win32Exception win32Ex = null;
                IntPtr wHdc = Win32.GetDC(IntPtr.Zero);
                if (wHdc == IntPtr.Zero)
                {
                    throw new System.ComponentModel.Win32Exception();
                }
                Graphics g;
                Bitmap img = new Bitmap(rect.Width, rect.Height);
                img.MakeTransparent();
                g = Graphics.FromImage(img);
                IntPtr gHdc = g.GetHdc();
                if (!(Win32.BitBlt(gHdc, 0, 0, rect.Width, rect.Height, wHdc, rect.X, rect.Y, Win32.SRCCOPY | Win32.CAPTUREBLT)))
                {
                    win32Ex = new System.ComponentModel.Win32Exception();
                }
                g.ReleaseHdc(gHdc);
                g.Dispose();
                Win32.ReleaseDC(IntPtr.Zero, wHdc);
                if (!(win32Ex == null))
                {
                    throw win32Ex;
                }
                else
                {
                    return img;
                }
            }
            else
            {
                throw new ArgumentException("Invalid parameter.", "rect");
            }
        }
        private class Win32
        {
            public const int CAPTUREBLT = 1073741824;
            public const int BLACKNESS = 66;
            public const int DSTINVERT = 5570569;
            public const int MERGECOPY = 12583114;
            public const int MERGEPAINT = 12255782;
            public const int NOTSRCCOPY = 3342344;
            public const int NOTSRCERASE = 1114278;
            public const int PATCOPY = 15728673;
            public const int PATINVERT = 5898313;
            public const int PATPAINT = 16452105;
            public const int SRCAND = 8913094;
            public const int SRCCOPY = 13369376;
            public const int SRCERASE = 4457256;
            public const int SRCINVERT = 6684742;
            public const int SRCPAINT = 15597702;
            public const int WHITENESS = 16711778;
            public const int HORZRES = 8;
            public const int VERTRES = 10;
            [StructLayout(LayoutKind.Sequential)]
            public struct RECT
            {
                public int Left;
                public int Top;
                public int Right;
                public int Bottom;
            }

            [DllImport("user32", SetLastError = true)]
            public static extern bool GetWindowRect(IntPtr hWnd, ref RECT lpRect);

            [DllImport("user32", SetLastError = true)]
            public static extern int ReleaseDC(IntPtr hWnd, IntPtr hdc);

            [DllImport("user32", SetLastError = true)]
            public static extern IntPtr WindowFromPoint(Point pt);

            [DllImport("user32", SetLastError = true)]
            public static extern IntPtr GetForegroundWindow();

            [DllImport("user32", SetLastError = true)]
            public static extern IntPtr GetDC(IntPtr hwnd);

            [DllImport("gdi32", SetLastError = true)]
            public static extern int GetDeviceCaps(IntPtr hdc, int nIndex);

            [DllImport("gdi32", SetLastError = true)]
            public static extern bool BitBlt(IntPtr hdcDest, int nXDest, int nYDest, int nWidth, int nHeight, IntPtr hdcSrc, int nXSrc, int nYSrc, uint dwRop);
        }
    }
}
