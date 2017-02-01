using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Windows.Media.Imaging;

namespace ConsoleApp2
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            IDataObject clipboardData;
            clipboardData = Clipboard.GetDataObject();

            // print out a list of the formats currently on the clipboard
            string formatList = GetFormatList(clipboardData);
            Console.WriteLine(formatList);

            bool success = CopyTransparentImageToClipboard(); // main part of script
            if (success)
                Console.WriteLine("Successful!");
            else
                Console.WriteLine("Unsucessful");

            Console.ReadKey(); // pause execution of script
        }

        // returns true/false if successful/unsuccessful
        public static bool CopyTransparentImageToClipboard()
        {
            // Parse in the image from the clipboard
            Image img = GetImageFromClipboard();
            if (img == null)
                return false;

            img.RotateFlip(System.Drawing.RotateFlipType.Rotate180FlipX); // gotta rotate it for some reason

            using (var ms = new MemoryStream())
            {
                img.Save(ms, ImageFormat.Png);

                IDataObject dataObj = new DataObject();
                dataObj.SetData("PNG", ms);
                dataObj.SetData(DataFormats.Dib, Clipboard.GetData(DataFormats.Dib));
                dataObj.SetData("Format17", Clipboard.GetData("Format17"));
                dataObj.SetData("Bitmap", Clipboard.GetData("Bitmap"));

                System.Windows.Forms.Clipboard.SetDataObject(dataObj, true);
            }

            return true;
        }

        public static string GetFormatList(IDataObject clipboardData)
        {
            string separator = "---------";
            String[] allFormats = clipboardData.GetFormats();
            string theResult;

            if (allFormats.Length == 0)
            {
                theResult = "Nothing in Clipboard\n";
            }
            else
            {
                theResult = "Clipboard Format(s):\n" + separator + "\n";
                for (int i = 0; i < allFormats.Length; i++)
                    theResult += allFormats[i] + "\n";
                theResult += separator + "\n";
            }

            return theResult;
        }

        public static Image GetImageFromClipboard()
        {
            if (Clipboard.GetDataObject() == null) return null;
            if (Clipboard.GetDataObject().GetDataPresent(DataFormats.Dib))
            {
                var dib = ((System.IO.MemoryStream)Clipboard.GetData(DataFormats.Dib)).ToArray();
                var width = BitConverter.ToInt32(dib, 4);
                var height = BitConverter.ToInt32(dib, 8);
                var bpp = BitConverter.ToInt16(dib, 14);
                if (bpp == 32)
                {
                    var gch = GCHandle.Alloc(dib, GCHandleType.Pinned);
                    Bitmap bmp = null;
                    try
                    {
                        var ptr = new IntPtr((long)gch.AddrOfPinnedObject() + 40);
                        bmp = new Bitmap(width, height, width * 4, System.Drawing.Imaging.PixelFormat.Format32bppArgb, ptr);
                        return new Bitmap(bmp);
                    }
                    finally
                    {
                        gch.Free();
                        if (bmp != null) bmp.Dispose();
                    }
                }
            }

            return Clipboard.ContainsImage() ? Clipboard.GetImage() : null;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct BITMAPV5HEADER
        {
            public uint bV5Size;
            public int bV5Width;
            public int bV5Height;
            public UInt16 bV5Planes;
            public UInt16 bV5BitCount;
            public uint bV5Compression;
            public uint bV5SizeImage;
            public int bV5XPelsPerMeter;
            public int bV5YPelsPerMeter;
            public UInt16 bV5ClrUsed;
            public UInt16 bV5ClrImportant;
            public UInt16 bV5RedMask;
            public UInt16 bV5GreenMask;
            public UInt16 bV5BlueMask;
            public UInt16 bV5AlphaMask;
            public UInt16 bV5CSType;
            public IntPtr bV5Endpoints;
            public UInt16 bV5GammaRed;
            public UInt16 bV5GammaGreen;
            public UInt16 bV5GammaBlue;
            public UInt16 bV5Intent;
            public UInt16 bV5ProfileData;
            public UInt16 bV5ProfileSize;
            public UInt16 bV5Reserved;
        }
        public static Bitmap CF_DIBV5ToBitmap(byte[] data)
        {
            GCHandle handle = GCHandle.Alloc(data, GCHandleType.Pinned);
            var bmi = (BITMAPV5HEADER)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(BITMAPV5HEADER));
            Bitmap bitmap = new Bitmap((int)bmi.bV5Width, (int)bmi.bV5Height, -
                                       (int)(bmi.bV5SizeImage / bmi.bV5Height), PixelFormat.Format32bppArgb,
                                       new IntPtr(handle.AddrOfPinnedObject().ToInt32()
                                       + bmi.bV5Size + (bmi.bV5Height - 1)
                                       * (int)(bmi.bV5SizeImage / bmi.bV5Height)));
            handle.Free();
            return bitmap;
        }

    }

}

