using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace SC70
{
    public class Win32
    {
        public const int LF_FACESIZE = 32;


        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int x;
            public int y;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SIZE
        {
            public int cx;
            public int cy;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;

            public RECT(int _l, int _t, int _r, int _b)
            {
                left = _l;
                top = _t;
                right = _r;
                bottom = _b;
            }
        }


        public const int CCHDEVICENAME          = 32;
        public const int CCHFORMNAME            = 32;
        public const int DMORIENT_PORTRAIT      = 1;
        public const int DMORIENT_LANDSCAPE     = 2;


        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct DEVMODEW
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst=CCHDEVICENAME)]   public String dmDeviceName;
            public Int16 dmSpecVersion;
            public Int16 dmDriverVersion;
            public Int16 dmSize;
            public Int16 dmDriverExtra;
            public Int32 dmFields;
            public Int16 dmOrientation;
            public Int16 dmPaperSize;
            public Int16 dmPaperLength;
            public Int16 dmPaperWidth;
            public Int16 dmScale;
            public Int16 dmCopies;
            public Int16 dmDefaultSource;
            public Int16 dmPrintQuality;
            public Int16 dmColor;
            public Int16 dmDuplex;
            public Int16 dmYResolution;
            public Int16 dmTTOption;
            public Int16 dmCollate;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst=CCHFORMNAME)] public String dmFormName;
            public Int16 dmLogPixels;
            public Int32 dmBitsPerPel;
            public Int32 dmPelsWidth;
            public Int32 dmPelsHeight;
            public Int32 dmDisplayFlags;
            public Int32 dmDisplayFrequency;
            public Int32 dmICMMethod;
            public Int32 dmICMIntent;
            public Int32 dmMediaType;
            public Int32 dmDitherType;
            public Int32 dmReserved1;
            public Int32 dmReserved2;
            public Int32 dmPanningWidth;
            public Int32 dmPanningHeight;
        }





        public const int SRCCOPY = 0xCC0020;        // dest = source

        public const int DIBRGBCOLORS = 0;          // color table in RGBs
        public const int DIBPALCOLORS = 1;          // color table in palette indices

        public const int BLACKONWHITE = 1;
        public const int WHITEONBLACK = 2;
        public const int COLORONCOLOR = 3;
        public const int HALFTONE = 4;

        [StructLayout(LayoutKind.Sequential)]
        public struct BITMAP
        {
            public Int32 bmType;
            public Int32 bmWidth;
            public Int32 bmHeight;
            public Int32 bmWidthBytes;
            public Int16 bmPlanes;
            public Int16 bmBitsPixel;
            public Int32 bmBits;           // void*
        }

        //[StructLayout(LayoutKind.Sequential, Pack=2)]
        [StructLayout(LayoutKind.Sequential)]
        public struct BITMAPINFOHEADER
        {
            public Int32 biSize;
            public Int32 biWidth;
            public Int32 biHeight;
            public Int16 biPlanes;
            public Int16 biBitCount;
            public Int32 biCompression;
            public Int32 biSizeImage;
            public Int32 biXPelsPerMeter;
            public Int32 biYPelsPerMeter;
            public Int32 biClrUsed;
            public Int32 biClrImportant;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct BITMAPINFO
        {
            public BITMAPINFOHEADER bmiHeader;
            // datas are followed...
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public byte[] bmiColors;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct DIBSECTION
        {
            public BITMAP dsBm;
            public BITMAPINFOHEADER dsBmih;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public Int32[] dsBitfields;
            public Int32 dshSection;       // HANDLE
            public Int32 dsOffset;
        }


        [DllImport("kernel32.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "RtlMoveMemory")]
        public static extern int CopyMemory(int pdest, int psrc, int size);


        [DllImport("gdi32.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "CreateCompatibleDC")]
        public static extern IntPtr CreateCompatibleDC(IntPtr hdc_ptr);

        [DllImport("gdi32.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "SetStretchBltMode")]
        public static extern int SetStretchBltMode(IntPtr hdc, int mode);

        [DllImport("gdi32.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "StretchDIBits")]
        public static extern int StretchDIBits(IntPtr hdc, int dstx, int dsty, int dstcx, int dstcy,
                                            int srcx, int srcy, int srccx, int srccy, int bits, int bmi, int usage, int rop);


        [DllImport("gdi32.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "CreateDIBSection")]
        public static extern IntPtr CreateDIBSection(IntPtr hdc, int bmpinf, int usage, ref int pbits, IntPtr hsection, int offset);



        [DllImport("kernel32.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "lstrlenW")]
        public static extern int lstrlen(IntPtr str_ptr);





        public const int WM_USER = 0x0400;


        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern IntPtr FindWindow(string lpClassName, String lpWindowName);

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int wMsg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        static extern uint RegisterWindowMessage(string lpString);    
    }
}
