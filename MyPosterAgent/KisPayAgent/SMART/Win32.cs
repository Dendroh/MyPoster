using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace SC2
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
            //public Int32 bmBits;           // void*
            public IntPtr bmBits;           // void*
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
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
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



        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct DOCINFO
        {
            public int      cbSize;
            public IntPtr   lpszDocName;
            public IntPtr   lpszOutput;
            public IntPtr   lpszDataType;
            public  uint    fwType;
        }

        [DllImport("gdi32.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "StartDocW")]
        public static extern int StartDoc(IntPtr hdc_ptr, IntPtr docinfo_ptr);

        [DllImport("gdi32.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "EndDoc")]
        public static extern int EndDoc(IntPtr hdc_ptr);

        [DllImport("gdi32.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "AbortDoc")]
        public static extern int AbortDoc(IntPtr hdc_ptr);

        [DllImport("gdi32.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "StartPage")]
        public static extern int StartPage(IntPtr hdc_ptr);

        [DllImport("gdi32.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "EndPage")]
        public static extern int EndPage(IntPtr hdc_ptr);


        [DllImport("gdi32.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "CreateDC")]
        //public static extern IntPtr CreateDC(IntPtr strdrv_ptr, IntPtr strdev_ptr, IntPtr strport_ptr, IntPtr dm);
        public static extern IntPtr CreateDC(int strdrv_ptr, string strdev_ptr, int strport_ptr, int dm);

        [DllImport("gdi32.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "DeleteDC")]
        public static extern int DeleteDC(IntPtr hdc);

        [DllImport("gdi32.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "ResetDC")]
        public static extern IntPtr ResetDC(IntPtr hdc, IntPtr dm);



        public const int FW_DONTCARE    = 0;
        public const int FW_THIN        = 100;
        public const int FW_EXTRALIGHT  = 200;
        public const int FW_LIGHT       = 300;
        public const int FW_NORMAL      = 400;
        public const int FW_MEDIUM      = 500;
        public const int FW_SEMIBOLD    = 600;
        public const int FW_BOLD        = 700;
        public const int FW_EXTRABOLD   = 800;
        public const int FW_HEAVY       = 900;
        public const int FW_ULTRALIGHT  = FW_EXTRALIGHT;
        public const int FW_REGULAR     = FW_NORMAL;
        public const int FW_DEMIBOLD    = FW_SEMIBOLD;
        public const int FW_ULTRABOLD   = FW_EXTRABOLD;
        public const int FW_BLACK       = FW_HEAVY;

        public const int ANSI_CHARSET            = 0;
        public const int DEFAULT_CHARSET         = 1;
        public const int SYMBOL_CHARSET          = 2;
        public const int SHIFTJIS_CHARSET        = 128;
        public const int HANGEUL_CHARSET         = 129;
        public const int HANGUL_CHARSET          = 129;
        public const int GB2312_CHARSET          = 134;
        public const int CHINESEBIG5_CHARSET     = 136;
        public const int OEM_CHARSET             = 255;
        public const int JOHAB_CHARSET           = 130;
        public const int HEBREW_CHARSET          = 177;
        public const int ARABIC_CHARSET          = 178;
        public const int GREEK_CHARSET           = 161;
        public const int TURKISH_CHARSET         = 162;
        public const int VIETNAMESE_CHARSET      = 163;
        public const int THAI_CHARSET            = 222;
        public const int EASTEUROPE_CHARSET      = 238;
        public const int RUSSIAN_CHARSET         = 204;
        public const int MAC_CHARSET             = 77;
        public const int BALTIC_CHARSET          = 186;

        public const int OUT_DEFAULT_PRECIS          = 0;
        public const int OUT_STRING_PRECIS           = 1;
        public const int OUT_CHARACTER_PRECIS        = 2;
        public const int OUT_STROKE_PRECIS           = 3;
        public const int OUT_TT_PRECIS               = 4;
        public const int OUT_DEVICE_PRECIS           = 5;
        public const int OUT_RASTER_PRECIS           = 6;
        public const int OUT_TT_ONLY_PRECIS          = 7;
        public const int OUT_OUTLINE_PRECIS          = 8;
        public const int OUT_SCREEN_OUTLINE_PRECIS   = 9;
        public const int OUT_PS_ONLY_PRECIS          = 10;

        public const int CLIP_DEFAULT_PRECIS     = 0;
        public const int CLIP_CHARACTER_PRECIS   = 1;
        public const int CLIP_STROKE_PRECIS      = 2;
        public const int CLIP_MASK               = 0xF;
        public const int CLIP_LH_ANGLES          = (1<<4);
        public const int CLIP_TT_ALWAYS          = (2<<4);
        public const int CLIP_DFA_DISABLE        = (4<<4);
        public const int CLIP_EMBEDDED           = (8<<4);

        public const int DEFAULT_QUALITY            = 0;
        public const int DRAFT_QUALITY              = 1;
        public const int PROOF_QUALITY              = 2;
        public const int NONANTIALIASED_QUALITY     = 3;
        public const int ANTIALIASED_QUALITY        = 4;
        public const int CLEARTYPE_QUALITY          = 5;
        public const int CLEARTYPE_NATURAL_QUALITY  = 6;

        public const int DEFAULT_PITCH           = 0;
        public const int FIXED_PITCH             = 1;
        public const int VARIABLE_PITCH          = 2;
        public const int MONO_FONT               = 8;

        public const int FF_DONTCARE         = (0<<4);  // Don't care or don't know.
        public const int FF_ROMAN            = (1<<4);  // Variable stroke width, serifed. Times Roman, Century Schoolbook, etc.
        public const int FF_SWISS            = (2<<4);  // Variable stroke width, sans-serifed. Helvetica, Swiss, etc.
        public const int FF_MODERN           = (3<<4);  // Constant stroke width, serifed or sans-serifed.  Pica, Elite, Courier, etc.
        public const int FF_SCRIPT           = (4<<4);  // Cursive, etc.
        public const int FF_DECORATIVE       = (5<<4);  // Old English, etc.

        [DllImport("gdi32.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "CreateFontW")]
        public static extern IntPtr CreateFont(int height, int width, int escape, int orientation, int weight, uint italic, uint underline, uint strikeout, uint charset, uint outprecision, uint clipprecision, uint quality, uint pitchandfamily, IntPtr facename_ptr);

        [DllImport("gdi32.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "SelectObject")]
        public static extern IntPtr SelectObject(IntPtr hdc_ptr, IntPtr hobj_ptr);

        [DllImport("gdi32.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "DeleteObject")]
        public static extern int DeleteObject(IntPtr hobj_ptr);

        [DllImport("gdi32.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "SetTextColor")]
        public static extern int SetTextColor(IntPtr hdc_ptr, int rgb);

        public static int RGB(byte r, byte g, byte b)
        {
            return ( r | (g<<8) | (b<<16) );
        }



        public const int DT_TOP                      = 0x00000000;
        public const int DT_LEFT                     = 0x00000000;
        public const int DT_CENTER                   = 0x00000001;
        public const int DT_RIGHT                    = 0x00000002;
        public const int DT_VCENTER                  = 0x00000004;
        public const int DT_BOTTOM                   = 0x00000008;
        public const int DT_WORDBREAK                = 0x00000010;
        public const int DT_SINGLELINE               = 0x00000020;
        public const int DT_EXPANDTABS               = 0x00000040;
        public const int DT_TABSTOP                  = 0x00000080;
        public const int DT_NOCLIP                   = 0x00000100;
        public const int DT_EXTERNALLEADING          = 0x00000200;
        public const int DT_CALCRECT                 = 0x00000400;
        public const int DT_NOPREFIX                 = 0x00000800;
        public const int DT_INTERNAL                 = 0x00001000;

        public const int DT_EDITCONTROL              = 0x00002000;
        public const int DT_PATH_ELLIPSIS            = 0x00004000;
        public const int DT_END_ELLIPSIS             = 0x00008000;
        public const int DT_MODIFYSTRING             = 0x00010000;
        public const int DT_RTLREADING               = 0x00020000;
        public const int DT_WORD_ELLIPSIS            = 0x00040000;
        public const int DT_NOFULLWIDTHCHARBREAK     = 0x00080000;
        public const int DT_HIDEPREFIX               = 0x00100000;
        public const int DT_PREFIXONLY               = 0x00200000;

        [DllImport("user32.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "DrawTextW")]
        public static extern int DrawText(IntPtr hdc, IntPtr text_ptr, int textlen, IntPtr rect_ptr, uint format);




        public const int IMAGE_BITMAP        = 0;
        public const int IMAGE_ICON          = 1;
        public const int IMAGE_CURSOR        = 2;
        public const int IMAGE_ENHMETAFILE   = 3;

        public const int LR_DEFAULTCOLOR     = 0x0000;
        public const int LR_MONOCHROME       = 0x0001;
        public const int LR_COLOR            = 0x0002;
        public const int LR_COPYRETURNORG    = 0x0004;
        public const int LR_COPYDELETEORG    = 0x0008;
        public const int LR_LOADFROMFILE     = 0x0010;
        public const int LR_LOADTRANSPARENT  = 0x0020;
        public const int LR_DEFAULTSIZE      = 0x0040;
        public const int LR_VGACOLOR         = 0x0080;
        public const int LR_LOADMAP3DCOLORS  = 0x1000;
        public const int LR_CREATEDIBSECTION = 0x2000;
        public const int LR_COPYFROMRESOURCE = 0x4000;
        public const int LR_SHARED           = 0x8000;

        [DllImport("user32.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "LoadImageW")]
        public static extern IntPtr LoadImage(IntPtr hinstance, IntPtr img_ptr, uint type, int cx, int cy, uint load);


        public const int DIB_RGB_COLORS      = 0; /* color table in RGBs */
        public const int DIB_PAL_COLORS      = 1; /* color table in palette indices */

        [DllImport("gdi32.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "SetDIBitsToDevice")]
        public static extern int SetDIBitsToDevice(IntPtr hdc, int dstx, int dsty, uint w, uint h, int srcx, int srcy, uint startscan, int lines, IntPtr bits_ptr, IntPtr bmi_ptr, uint coloruse);


        [DllImport("gdi32.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "GetObjectW")]
        public static extern int GetObject(IntPtr hobj, int size, IntPtr v_ptr);



        public const int DRIVERVERSION  = 0;     /* Device driver version                    */
        public const int TECHNOLOGY     = 2;     /* Device classification                    */
        public const int HORZSIZE       = 4;     /* Horizontal size in millimeters           */
        public const int VERTSIZE       = 6;     /* Vertical size in millimeters             */
        public const int HORZRES        = 8;     /* Horizontal width in pixels               */
        public const int VERTRES        = 10;    /* Vertical height in pixels                */
        public const int BITSPIXEL      = 12;    /* Number of bits per pixel                 */
        public const int PLANES         = 14;    /* Number of planes                         */
        public const int NUMBRUSHES     = 16;    /* Number of brushes the device has         */
        public const int NUMPENS        = 18;    /* Number of pens the device has            */
        public const int NUMMARKERS     = 20;    /* Number of markers the device has         */
        public const int NUMFONTS       = 22;    /* Number of fonts the device has           */
        public const int NUMCOLORS      = 24;    /* Number of colors the device supports     */
        public const int PDEVICESIZE    = 26;    /* Size required for device descriptor      */
        public const int CURVECAPS      = 28;    /* Curve capabilities                       */
        public const int LINECAPS       = 30;    /* Line capabilities                        */
        public const int POLYGONALCAPS  = 32;    /* Polygonal capabilities                   */
        public const int TEXTCAPS       = 34;    /* Text capabilities                        */
        public const int CLIPCAPS       = 36;    /* Clipping capabilities                    */
        public const int RASTERCAPS     = 38;    /* Bitblt capabilities                      */
        public const int ASPECTX        = 40;    /* Length of the X leg                      */
        public const int ASPECTY        = 42;    /* Length of the Y leg                      */
        public const int ASPECTXY       = 44;    /* Length of the hypotenuse                 */
        public const int LOGPIXELSX     = 88;    /* Logical pixels/inch in X                 */
        public const int LOGPIXELSY     = 90;    /* Logical pixels/inch in Y                 */
        public const int SIZEPALETTE    = 104;    /* Number of entries in physical palette    */
        public const int NUMRESERVED    = 106;    /* Number of reserved entries in palette    */
        public const int COLORRES       = 108;    /* Actual color resolution                  */
        public const int PHYSICALWIDTH   = 110; /* Physical Width in device units           */
        public const int PHYSICALHEIGHT  = 111; /* Physical Height in device units          */
        public const int PHYSICALOFFSETX = 112; /* Physical Printable Area x margin         */
        public const int PHYSICALOFFSETY = 113; /* Physical Printable Area y margin         */
        public const int SCALINGFACTORX  = 114; /* Scaling factor x                         */
        public const int SCALINGFACTORY  = 115; /* Scaling factor y                         */
        public const int VREFRESH        = 116;  /* Current vertical refresh rate of the display device (for displays only) in Hz */
        public const int DESKTOPVERTRES  = 117;  /* Horizontal width of entire desktop in pixels */
        public const int DESKTOPHORZRES  = 118;  /* Vertical height of entire desktop in pixels */
        public const int BLTALIGNMENT    = 119;  /* Preferred blt alignment                 */
        public const int SHADEBLENDCAPS  = 120;  /* Shading and blending caps               */
        public const int COLORMGMTCAPS   = 121;  /* Color Management caps                   */

        [DllImport("gdi32.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "GetDeviceCaps")]
        public static extern uint GetDeviceCaps(IntPtr hdc, int index);



        public const int DM_OUT_BUFFER  = 2;
        public const int DM_IN_BUFFER   = 8;

        [DllImport("winspool.drv", CallingConvention = CallingConvention.Winapi, EntryPoint = "DocumentPropertiesW")]
        public static extern int DocumentProperties(IntPtr hwnd, IntPtr hprinter, IntPtr strdevname, IntPtr dmOutput, IntPtr dmInput, uint fMode);



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
