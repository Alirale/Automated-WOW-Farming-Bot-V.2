using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Threading;

namespace HolydayRadar
{
    public class CursorHash
    {
        public Bitmap bmp1;
        public Bitmap bmp2;

        public const Int32 CURSOR_SHOWING = 0x00000001;
        [StructLayout(LayoutKind.Sequential)]
        public struct ICONINFO
        {
            public bool fIcon;         // Specifies whether this structure defines an icon or a cursor. A value of TRUE specifies 
            public Int32 xHotspot;     // Specifies the x-coordinate of a cursor's hot spot. If this structure defines an icon, the hot 
            public Int32 yHotspot;     // Specifies the y-coordinate of the cursor's hot spot. If this structure defines an icon, the hot 
            public IntPtr hbmMask;     // (HBITMAP) Specifies the icon bitmask bitmap. If this structure defines a black and white icon, 
            public IntPtr hbmColor;    // (HBITMAP) Handle to the icon color bitmap. This member can be optional if this 
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct CURSORINFO
        {
            public Int32 cbSize;        // Specifies the size, in bytes, of the structure. 
            public Int32 flags;         // Specifies the cursor state. This parameter can be one of the following values:
            public IntPtr hCursor;          // Handle to the cursor. 
            public Point ptScreenPos;       // A POINT structure that receives the screen coordinates of the cursor. 
        }


        [DllImport("user32.dll", EntryPoint = "GetCursorInfo")]
        public static extern bool GetCursorInfo(out CURSORINFO pci);

        [DllImport("user32.dll", EntryPoint = "CopyIcon")]
        public static extern IntPtr CopyIcon(IntPtr hIcon);

        [DllImport("user32.dll", EntryPoint = "GetIconInfo")]
        public static extern bool GetIconInfo(IntPtr hIcon, out ICONINFO piconinfo);

        //Bitmap bmp;
        //IntPtr hicon;
        //CURSORINFO ci = new CURSORINFO();
        //ICONINFO icInfo;
        //Icon ic;

        public static Bitmap CaptureCursor()
        {
            Bitmap bmp;
            IntPtr hicon;
            CURSORINFO ci = new CURSORINFO();
            ICONINFO icInfo;
            ci.cbSize = Marshal.SizeOf(ci);
            if (GetCursorInfo(out ci))
            {
                if (ci.flags == CURSOR_SHOWING)
                {
                    hicon = CopyIcon(ci.hCursor);
                    if (GetIconInfo(hicon, out icInfo))
                    {
                        try
                        {
                            Icon ic = Icon.FromHandle(hicon);
                            bmp = ic.ToBitmap();
                            if (bmp == null)
                            {
                                Console.WriteLine("icon null");
                            }
                            return bmp;
                        }
                        catch 
                        {
                            CaptureCursor();
                        }
                    }
                }
            }
            return null;
        }


        public void CaptureLootCursor()
        {
            Bitmap bmp;
            IntPtr hicon;
            CURSORINFO ci = new CURSORINFO();
            ICONINFO icInfo;
            ci.cbSize = Marshal.SizeOf(ci);
            if (GetCursorInfo(out ci))
            {
                if (ci.flags == CURSOR_SHOWING)
                {
                    hicon = CopyIcon(ci.hCursor);
                    if (GetIconInfo(hicon, out icInfo))
                    {
                        Icon ic = Icon.FromHandle(hicon);
                        bmp = ic.ToBitmap();

                        bmp.Save(AppDomain.CurrentDomain.BaseDirectory + "Loot.bmp", System.Drawing.Imaging.ImageFormat.Icon);
                    }
                }
            }
        }

        public void CaptureCurrentCursor()
        {
            Bitmap bmp;
            IntPtr hicon;
            CURSORINFO ci = new CURSORINFO();
            ICONINFO icInfo;
            ci.cbSize = Marshal.SizeOf(ci);
            if (GetCursorInfo(out ci))
            {
                if (ci.flags == CURSOR_SHOWING)
                {
                    hicon = CopyIcon(ci.hCursor);
                    if (GetIconInfo(hicon, out icInfo))
                    {
                        Icon ic = Icon.FromHandle(hicon);
                        bmp = ic.ToBitmap();

                        bmp.Save(AppDomain.CurrentDomain.BaseDirectory + "Current.bmp", System.Drawing.Imaging.ImageFormat.Bmp);
                    }
                }
            }
        }


        public bool CompareCursorImages(Bitmap bmp1, Bitmap bmp2)
        {
            try
            {
                //create instance or System.Drawing.ImageConverter to convert
                //each image to a byte array
                ImageConverter converter = new ImageConverter();
                //create 2 byte arrays, one for each image
                byte[] imgBytes1 = new byte[1];
                byte[] imgBytes2 = new byte[1];

                //convert images to byte array
                imgBytes1 = (byte[])converter.ConvertTo(bmp1, imgBytes2.GetType());
                imgBytes2 = (byte[])converter.ConvertTo(bmp2, imgBytes1.GetType());

                //now compute a hash for each image from the byte arrays
                SHA256Managed sha = new SHA256Managed();
                byte[] imgHash1 = sha.ComputeHash(imgBytes1);
                byte[] imgHash2 = sha.ComputeHash(imgBytes2);

                //now let's compare the hashes
                for (int i = 0; i < imgHash1.Length && i < imgHash2.Length; i++)
                {
                    //whoops, found a non-match, exit the loop
                    //with a false value
                    if (!(imgHash1[i] == imgHash2[i]))
                        return false;
                }
            }
            catch
            {
                //MessageBox.Show(ex.Message);
                return false;
            }
            //we made it this far so the images must match
            return true;
        }

    }
}
