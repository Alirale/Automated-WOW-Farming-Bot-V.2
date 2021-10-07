using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace HolydayRadar
{
    public static class ImageHashing
    {
        #region Private constants and utility methods
        private static byte[] bitCounts = {
            0,1,1,2,1,2,2,3,1,2,2,3,2,3,3,4,1,2,2,3,2,3,3,4,2,3,3,4,3,4,4,5,1,2,2,3,2,3,3,4,
            2,3,3,4,3,4,4,5,2,3,3,4,3,4,4,5,3,4,4,5,4,5,5,6,1,2,2,3,2,3,3,4,2,3,3,4,3,4,4,5,
            2,3,3,4,3,4,4,5,3,4,4,5,4,5,5,6,2,3,3,4,3,4,4,5,3,4,4,5,4,5,5,6,3,4,4,5,4,5,5,6,
            4,5,5,6,5,6,6,7,1,2,2,3,2,3,3,4,2,3,3,4,3,4,4,5,2,3,3,4,3,4,4,5,3,4,4,5,4,5,5,6,
            2,3,3,4,3,4,4,5,3,4,4,5,4,5,5,6,3,4,4,5,4,5,5,6,4,5,5,6,5,6,6,7,2,3,3,4,3,4,4,5,
            3,4,4,5,4,5,5,6,3,4,4,5,4,5,5,6,4,5,5,6,5,6,6,7,3,4,4,5,4,5,5,6,4,5,5,6,5,6,6,7,
            4,5,5,6,5,6,6,7,5,6,6,7,6,7,7,8
        };

        private static uint BitCount(ulong num)
        {
            uint count = 0;
            for (; num > 0; num >>= 8)
                count += bitCounts[(num & 0xff)];
            return count;
        }
        #endregion

        #region Public interface methods
        public static ulong AverageHash(Image image)
        {
            try
            {
                if (image != null)
                {
                    // Squeeze the image into an 8x8 canvas
                    Bitmap squeezed = new Bitmap(8, 8, PixelFormat.Format32bppRgb);
                    Graphics canvas = Graphics.FromImage(squeezed);
                    canvas.CompositingQuality = CompositingQuality.HighQuality;
                    canvas.InterpolationMode = InterpolationMode.HighQualityBilinear;
                    canvas.SmoothingMode = SmoothingMode.HighQuality;
                    canvas.DrawImage(image, 0, 0, 8, 8);

                    // Reduce colors to 6-bit grayscale and calculate average color value
                    byte[] grayscale = new byte[64];
                    uint averageValue = 0;
                    for (int y = 0; y < 8; y++)
                        for (int x = 0; x < 8; x++)
                        {
                            uint pixel = (uint)squeezed.GetPixel(x, y).ToArgb();
                            uint gray = (pixel & 0x00ff0000) >> 16;
                            gray += (pixel & 0x0000ff00) >> 8;
                            gray += (pixel & 0x000000ff);
                            gray /= 12;

                            grayscale[x + (y * 8)] = (byte)gray;
                            averageValue += gray;
                        }
                    averageValue /= 64;

                    // Compute the hash: each bit is a pixel
                    // 1 = higher than average, 0 = lower than average
                    ulong hash = 0;
                    for (int i = 0; i < 64; i++)
                        if (grayscale[i] >= averageValue)
                            hash |= (1UL << (63 - i));
                    return hash;
                }
                }
            catch 
            {

               
            }
            return 0;
        }

        public static ulong AverageHash(String path)
        {
            Bitmap bmp = new Bitmap(path);
            return AverageHash(bmp);
        }

        public static double Similarity(ulong hash1, ulong hash2)
        {
            return ((64 - BitCount(hash1 ^ hash2)) * 100) / 64.0;
        }

        public static double Similarity(Image image1, Image image2)
        {
            ulong hash1 = AverageHash(image1);
            ulong hash2 = AverageHash(image2);
            return Similarity(hash1, hash2);
        }

        public static double Similarity(String path1, String path2)
        {
            ulong hash1 = AverageHash(path1);
            ulong hash2 = AverageHash(path2);
            return Similarity(hash1, hash2);
        }
        #endregion
    }
}
