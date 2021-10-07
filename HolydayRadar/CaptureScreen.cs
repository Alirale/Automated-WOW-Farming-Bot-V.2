using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;
using static HolydayRadar.Win32Stuff;
using System.Drawing.Imaging;
using System.Windows.Forms;
using System.Windows.Input;

namespace HolydayRadar
{
    public class CaptureScreen
    {
        private static Form1 frm = new Form1();
        //This structure shall be used to keep the size of the screen.
        public struct SIZE
        {
            public int cx;
            public int cy;
        }
        internal const int CursorShowing = 0x00000001;

        static public Bitmap CaptureDesktop()
        {
            try
            {
                SIZE size;
                IntPtr hBitmap;
                IntPtr hDC = Win32Stuff.GetDC(Win32Stuff.GetDesktopWindow());
                IntPtr hMemDC = GDIStuff.CreateCompatibleDC(hDC);

                size.cx = Win32Stuff.GetSystemMetrics
                          (Win32Stuff.SM_CXSCREEN);

                size.cy = Win32Stuff.GetSystemMetrics
                          (Win32Stuff.SM_CYSCREEN);

                hBitmap = GDIStuff.CreateCompatibleBitmap(hDC, size.cx, size.cy);

                if (hBitmap != IntPtr.Zero)
                {
                    IntPtr hOld = (IntPtr)GDIStuff.SelectObject
                                           (hMemDC, hBitmap);

                    GDIStuff.BitBlt(hMemDC, 0, 0, size.cx, size.cy, hDC,
                                                   0, 0, GDIStuff.SRCCOPY);

                    GDIStuff.SelectObject(hMemDC, hOld);
                    GDIStuff.DeleteDC(hMemDC);
                    Win32Stuff.ReleaseDC(Win32Stuff.GetDesktopWindow(), hDC);
                    Bitmap bmp = System.Drawing.Image.FromHbitmap(hBitmap);
                    GDIStuff.DeleteObject(hBitmap);
                    GC.Collect();
                    return bmp;
                }
                return null;
            }
            catch
            {
                return null;
            }
        }


        static public Bitmap CaptureCursor(ref int x, ref int y)
        {
            //begin:;
            Bitmap bmp;
            IntPtr hicon;
            Win32Stuff.CURSORINFO ci = new Win32Stuff.CURSORINFO();
            Win32Stuff.ICONINFO icInfo;
            ci.cbSize = Marshal.SizeOf(ci);
            if (Win32Stuff.GetCursorInfo(out ci))
            {
                if (ci.flags == Win32Stuff.CURSOR_SHOWING)
                {
                    hicon = Win32Stuff.CopyIcon(ci.hCursor);
                    if (Win32Stuff.GetIconInfo(hicon, out icInfo))
                    {
                        x = ci.ptScreenPos.x - ((int)icInfo.xHotspot);
                        y = ci.ptScreenPos.y - ((int)icInfo.yHotspot);

                        Icon ic = Icon.FromHandle(hicon);

                        bmp = ic.ToBitmap();
                        return bmp;

                    }
                }
            }

            return null;
        }

        public Bitmap CaptureDesktopWithCursor()
        {
            int cursorX = 0;
            int cursorY = 0;
            double scale = 1;
            Point REFPOINT = Point.Empty;
            REFPOINT.X = cursorX;
            REFPOINT.Y = cursorY;
            Bitmap desktopBMP;
            Bitmap cursorBMP;
            //Bitmap finalBMP;
            Graphics g;
            Rectangle r;

            desktopBMP = CaptureDesktop();
            cursorBMP = CaptureImageCursor(ref REFPOINT, scale);
            if (desktopBMP != null)
            {
                if (cursorBMP != null)
                {
                    r = new Rectangle(cursorX, cursorY, cursorBMP.Width, cursorBMP.Height);
                    g = Graphics.FromImage(desktopBMP);
                    g.DrawImage(cursorBMP, r);
                    g.Flush();

                    return desktopBMP;
                }
                else 
                { 
                    return desktopBMP;
                }


            }

            return null;

        }


        [DllImport("user32.dll", SetLastError = true)]
        internal static extern bool DestroyIcon(IntPtr hIcon);

        [DllImport("user32.dll")]
        internal static extern IntPtr GetWindowDC(IntPtr ptr);

        [DllImport("gdi32.dll", EntryPoint = "BitBlt", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool BitBlt([In] IntPtr hdc, int nXDest, int nYDest, int nWidth, int nHeight, [In] IntPtr hdcSrc, int nXSrc, int nYSrc, CopyPixelOperation dwRop);

        [DllImport("user32.dll", SetLastError = true)]
        internal static extern bool DrawIconEx(IntPtr hdc, int xLeft, int yTop, IntPtr hIcon, int cxWidth, int cyHeight, int istepIfAniCur, IntPtr hbrFlickerFreeDraw, int diFlags);

        [DllImport("gdi32.dll", EntryPoint = "DeleteObject")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool DeleteObject([In] IntPtr hObject);

        [DllImport("gdi32.dll", EntryPoint = "DeleteDC")]
        internal static extern bool DeleteDC([In] IntPtr hdc);

        [DllImport("user32.dll", EntryPoint = "GetCursorInfo")]
        internal static extern bool GetCursorInfo(out CursorInfo pci);

        public static System.Drawing.Bitmap CaptureImageCursor(ref Point point, double scale)
        {
            var cursorInfo = new CursorInfo();
            cursorInfo.cbSize = Marshal.SizeOf(cursorInfo);

            if (!GetCursorInfo(out cursorInfo))
                return null;

            if (cursorInfo.flags != CursorShowing)
                return null;

            var hicon = CopyIcon(cursorInfo.hCursor);
            if (hicon == IntPtr.Zero)
                return null;

            if (!GetIconInfo(hicon, out var iconInfo))
            {
                DeleteObject(hicon);
                return null;
            }

            point.X = cursorInfo.ptScreenPos.X - iconInfo.xHotspot;
            point.Y = cursorInfo.ptScreenPos.Y - iconInfo.yHotspot;

            using (var maskBitmap = Image.FromHbitmap(iconInfo.hbmMask))
            {
                //Is this a monochrome cursor?  
                if (maskBitmap.Height == maskBitmap.Width * 2 && iconInfo.hbmColor == IntPtr.Zero)
                {
                    var final = new System.Drawing.Bitmap(maskBitmap.Width, maskBitmap.Width);
                    var hDesktop = GetDesktopWindow();
                    var dcDesktop = GetWindowDC(hDesktop);

                    using (var resultGraphics = Graphics.FromImage(final))
                    {
                        var resultHdc = resultGraphics.GetHdc();
                        var offsetX = (int)((point.X + 3) * scale);
                        var offsetY = (int)((point.Y + 3) * scale);

                        BitBlt(resultHdc, 0, 0, final.Width, final.Height, dcDesktop, offsetX, offsetY, CopyPixelOperation.SourceCopy);
                        DrawIconEx(resultHdc, 0, 0, cursorInfo.hCursor, 0, 0, 0, IntPtr.Zero, 0x0003);

                        //TODO: I have to try removing the background of this cursor capture.
                        //Native.BitBlt(resultHdc, 0, 0, final.Width, final.Height, dcDesktop, (int)point.X + 3, (int)point.Y + 3, Native.CopyPixelOperation.SourceErase);

                        //Original, ignores the screen as background.
                        //Native.BitBlt(resultHdc, 0, 0, resultBitmap.Width, resultBitmap.Height, maskHdc, 0, resultBitmap.Height, Native.CopyPixelOperation.SourceCopy); //SourceCopy
                        //Native.BitBlt(resultHdc, 0, 0, resultBitmap.Width, resultBitmap.Height, maskHdc, 0, 0, Native.CopyPixelOperation.PatInvert); //SourceInvert

                        resultGraphics.ReleaseHdc(resultHdc);
                        ReleaseDC(hDesktop, dcDesktop);
                    }

                    DeleteObject(iconInfo.hbmMask);
                    DeleteDC(dcDesktop);

                    return final;
                }

                DeleteObject(iconInfo.hbmColor);
                DeleteObject(iconInfo.hbmMask);
                DeleteObject(hicon);
            }

            var icon = Icon.FromHandle(hicon);
            return icon.ToBitmap();
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct CursorInfo
        {
            /// <summary>
            /// Specifies the size, in bytes, of the structure. 
            /// </summary>
            public int cbSize;

            /// <summary>
            /// Specifies the cursor state. This parameter can be one of the following values:
            /// </summary>
            public int flags;

            ///<summary>
            ///Handle to the cursor. 
            ///</summary>
            public IntPtr hCursor;

            /// <summary>
            /// A POINT structure that receives the screen coordinates of the cursor. 
            /// </summary>
            public PointW ptScreenPos;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct PointW
        {
            public int X;
            public int Y;
        }

    }
}
