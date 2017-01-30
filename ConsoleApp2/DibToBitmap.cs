using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Runtime.InteropServices;

namespace ConsoleApp2
{
    public class DibToBitmap
    {

        /// <summary>
        /// Convert DIB to Bitmap.
        /// </summary>
        /// <param name="dibPtrArg">Pointer to memory DIB, starting with BITMAPINFOHEADER.</param>
        /// <returns>A Bitmap</returns>
        public static Bitmap Convert(IntPtr dibPtrArg)
        {
            BITMAPINFOHEADER bmi;
            IntPtr pixptr;

            GetPixelInfo(dibPtrArg, out pixptr, out bmi);

            Bitmap bitMap = new Bitmap(bmi.biWidth, bmi.biHeight);

            Graphics scannedImageGraphics = Graphics.FromImage(bitMap);

            IntPtr hdc = scannedImageGraphics.GetHdc();

            SetDIBitsToDevice(
               hdc,
               0,             // XDest
               0,             // YDest
               bmi.biWidth,
               bmi.biHeight,
               0,             // XSrc
               0,             // YSrc
               0,             // uStartScan
               bmi.biHeight,  // cScanLines
               pixptr,        // lpvBits
               dibPtrArg,     // lpbmi
               0);            // 0 = literal RGB values rather than palette

            scannedImageGraphics.ReleaseHdc(hdc);

            const float inchPerMeter = 39.3700787F;
            bitMap.SetResolution(bmi.biXPelsPerMeter / inchPerMeter, bmi.biYPelsPerMeter / inchPerMeter);

            // bitMap.Save(@"c:\0\2.bmp", ImageFormat.Bmp); // debug code

            return bitMap;
        }


        static private void GetPixelInfo(IntPtr bmpptr, out IntPtr pix, out BITMAPINFOHEADER bmi)
        {

            bmi = new BITMAPINFOHEADER();
            Marshal.PtrToStructure(bmpptr, bmi); // copy into struct.

            if (bmi.biSizeImage == 0)
            {
                bmi.biSizeImage = ((((bmi.biWidth * bmi.biBitCount) + 31) & ~31) >> 3) * bmi.biHeight;
            }

            int p = bmi.biClrUsed;

            if ((p == 0) && (bmi.biBitCount <= 8))
            {
                p = 1 << bmi.biBitCount;
            }

            pix = (IntPtr)((p * 4) + bmi.biSize + (int)bmpptr);

        }

        [StructLayout(LayoutKind.Sequential, Pack = 2)]
        private class BITMAPINFOHEADER
        {
            public int biSize;
            public int biWidth;
            public int biHeight;
            public short biPlanes;
            public short biBitCount;
            public int biCompression;
            public int biSizeImage;
            public int biXPelsPerMeter;
            public int biYPelsPerMeter;
            public int biClrUsed;
            public int biClrImportant;
        }

        [DllImport("gdi32.dll", ExactSpelling = true)]
        internal static extern int SetDIBitsToDevice(
           IntPtr hdc,
           int xdst,
           int ydst,
           int width,
           int height,
           int xsrc,
           int ysrc,
           int start,
           int lines,
           IntPtr bitsptr,
           IntPtr bmiptr,
           int color);

    } // class DibToImage
}
