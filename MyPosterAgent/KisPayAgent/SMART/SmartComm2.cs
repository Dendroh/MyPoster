using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace SC2
{
    public class SmartComm2
    {
        public const int DEVICELIST_ALL         = 0x00;
        public const int DEVICELIST_USB         = 0x01;
        public const int DEVICELIST_NET         = 0x02;

        public const int OPENDEVICE_BYID        = 0;
        public const int OPENDEVICE_BYDESC      = 1;


        public const byte PAGE_FRONT            = 0;
        public const byte PAGE_BACK             = 1;
        public const byte PAGE_P1FRONT          = PAGE_FRONT;
        public const byte PAGE_P1BACK           = PAGE_BACK;
        public const byte PAGE_max              = PAGE_BACK + 1;
        public const byte PAGE_P2FRONT          = 2;
        public const byte PAGE_P2BACK           = 3;
        public const byte PAGE_COUNT            = PAGE_P2BACK + 1;
        public const byte PAGE_START            = PAGE_P1FRONT;
        public const byte PAGE_end              = PAGE_P2BACK;
        public const byte PANEL_COLOR           = 1;
        public const byte PANEL_BLACK           = 2;
        public const byte PANEL_OVERLAY         = 4;
        public const byte PANEL_UV              = 8;
        public const byte PANELIDX__COLOR       = 0;
        public const byte PANELIDX__BLACK       = 1;
        public const byte PANELIDX__OVERLAY     = 2;
        public const byte PANELIDX__UV          = 3;
        public const byte PANELIDX__max         = 4;




        public enum ObjType
        {
	        obj_rect,
	        obj_roundrect,
	        obj_oval,
	        obj_line,
	        obj_image,
	        obj_text,
	        obj_bar
        }



        public const byte   OBJ_ALIGN_LEFT		= 0x00;
        public const byte   OBJ_ALIGN_CENTER	= 0x01;
        public const byte   OBJ_ALIGN_RIGHT		= 0x02;
        public const byte   OBJ_ALIGN_JUSTIFY	= 0x03;     // text object only...
        public const byte   OBJ_ALIGN_HNOALIGN	= 0x04;
        public const byte   OBJ_ALIGN_TOP		= 0x00;
        public const byte   OBJ_ALIGN_MIDDLE	= 0x10;
        public const byte   OBJ_ALIGN_BOTTOM	= 0x20;
        public const byte   OBJ_ALIGN_VNOALIGN	= 0x30;
        public const byte   OBJ_ALIGN_HORZMASK	= 0x0F;
        public const byte   OBJ_ALIGN_VERTMASK	= 0xF0;
        public const byte   OBJ_ALIGN_NOALIGN	= OBJ_ALIGN_VNOALIGN | OBJ_ALIGN_HNOALIGN;
        public const byte   OBJ_ALIGN_HVCENTER  = OBJ_ALIGN_CENTER | OBJ_ALIGN_MIDDLE;
        public byte GET_HALIGN(byte a)	    	{ return (byte)(a & OBJ_ALIGN_HORZMASK); }
        public byte GET_VALIGN(byte a)	    	{ return (byte)(a & OBJ_ALIGN_VERTMASK); }
        public byte SET_HALIGN(byte a, byte h)  { return (byte)(GET_HALIGN(h) | GET_VALIGN(a)); }
        public byte SET_VALIGN(byte a, byte v)	{ return (byte)(GET_HALIGN(a) | GET_VALIGN(v)); }


        public const byte   IMGSCALE_FITHORZ	= 0;	// scale to fit to width of frame
        public const byte   IMGSCALE_FITVERT	= 1;	// scale to fit to height of frame
        public const byte   IMGSCALE_FITFRAME	= 2;	// scale to fit to frame.
        public const byte   IMGSCALE_USER		= 3;	// scale factor is user setted.
        public const byte   IMGSCALE_BEST		= IMGSCALE_FITHORZ;
        public const int    IMGSCALE_BASEAMP	= 10000;	// default amplyfiying value

        public const short  IMGEFFECT_CONTRASTMIN	= -100;
        public const short  IMGEFFECT_CONTRASTMAX	= 100;
        public const short  IMGEFFECT_BRIGHTMIN		= -256;
        public const short  IMGEFFECT_BRIGHTMAX		= 255;

        public const byte   FONT_NORMAL		    = 0x00;
        public const byte   FONT_BOLD		    = 0x01;
        public const byte   FONT_ITALIC		    = 0x02;
        public const byte   FONT_UNDERLINE	    = 0x04;
        public const byte   FONT_STRIKEOUT	    = 0x08;

        public const short  TEXT_NOFIT			= 0;       // use given font size...
        public const short  TEXT_FITHEIGHT		= 1;       // font size is determined by frame's height
        public const short  TEXT_FITWIDTH		= 2;       // font size is determined by frame's width
        public const short  TEXT_FITBOTH		= 3;       // font height and width is determined at each by frame
        public const short  TEXT_AUTOSIZE		= 4;       // font size is determined by frame's width and height
        public const short  TEXT_SIZEMASK		= 0x000F;

        public const int    OTB_NONE            = 0;
        public const int    OTB_RECT            = 0x01;
        public const int    OTB_ROUNDRECT		= 0x02;
        public const int    OTB_OVAL			= 0x04;
        public const int    OTB_LINE			= 0x08;
        public const int    OTB_IMAGE			= 0x10;
        public const int    OTB_TEXT			= 0x20;
        public const int    OTB_BAR			    = 0x40;

        public const int    UNITTYPE_TEXT		= OTB_TEXT;
        public const int    UNITTYPE_IMAGE	    = OTB_IMAGE;
        public const int    UNITTYPE_BARCODE	= OTB_BAR;

        public const int    UNITINFO_FIRST		= 0;
        public const int    UNITINFO_NEXT		= 1;


        public const int    MAX_FIELDNAMELEN	= 32;
        public const int    MAX_FIELDVALUELEN	= 1024;

        public const int    MAX_BARCODENAME		= 64;


        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct FontInfo
        {
	        public int  	size;
	        public Byte		style;
            public int      color;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = Win32.LF_FACESIZE)]
            public String   name;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct FontInfo2
        {
	        public int  	size;			// font size
	        public int  	size2;			// font width size  (for slim or large sized text...)
	        public byte 	style;
	        public int  	color;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = Win32.LF_FACESIZE)]
            public String   name;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct Border
        {
	        public Int16		type;
	        public Int16		width;
	        public int			color;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct BackGround
        {
	        public int  		fill;
	        public int			color;
            public Byte         transparency;		// % unit. (0:Opaque, 100:transparent)
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct Laser
        {
	        public int  		power;
	        public int			speed;
	        public int			frequency;
	        public int			angle;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
            public int[] reserved;
        }




        //--------------------------------------------------------------------------------------------

        public const int    UNITINFO_VER_1		= 1;
        public const int    UNITINFO_VER_2		= 2;
        public const int    UNITINFO_VER		= UNITINFO_VER_2;

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct UNITTEXT2
        {
	        public short	    leftMargin;		// spaceLeft
	        public short	    topMargin;		// spaceTop
	        public short	    rightMargin;	// spaceRight
	        public short	    bottomMargin;	// spaceBottom
	        public byte         align;
	        public FontInfo	    font;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_FIELDNAMELEN+1)]
	        public String	    field;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public byte[]       reserved;
        }


        public const short  UIMG2OPT_OFFSET_ORIGIN	= 0;        // UNITIMAGE2::offset value is original (before-rotating) offset point.
        public const short  UIMG2OPT_OFFSET_DRAW	= 1;        // UNITIMAGE2::offset value is drawing point (applied-rotation).
        public const short  UIMG2OPT_OFFSET_MASK	= 0x01;

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct UNITIMAGE2
        {
	        public int		    option;
	        public int		    widthZoom;
	        public int		    heightZoom;
	        public short	    contrast;
	        public short	    brightness;
	        public int          grayscale;
	        public short        align;
	        public Win32.POINT	offset;
	        public byte         scaleMethod;
	        public int		    round;
	        public int          autoportrait;
	        public int          autoeffect;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_FIELDNAMELEN+1)]
	        public String	    field;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public byte[]       reserved;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct UNITBAR2
        {
	        public int		    type;
	        public int		    size;
	        public int		    option;
	        public Win32.SIZE	size2D;			// bar2DWidth, bar2DHeight
	        public int          barColor;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_FIELDNAMELEN+1)]
	        public String	    field;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public byte[]       reserved;
        }


        //--------------------------------------------------------------------------------------------

        public const int        SIZEOF_UNITTEXT2    = 188;    //Marshal.SizeOf(typeof(UNITTEXT2));
        public const int        SIZEOF_UNITIMAGE2   = 148;    //Marshal.SizeOf(typeof(UNITIMAGE2));
        public const int        SIZEOF_UNITBAR2     = 124;    //Marshal.SizeOf(typeof(UNITBAR2));
        public const int        SIZEOF_UNITMAX2     = 188;    //Math.Max(Math.Max(SIZEOF_UNITTEXT2,SIZEOF_UNITBAR2),SIZEOF_UNITBAR2);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct UNITINFOBASE2
        {
	        public int		    ver;
	        public int		    index;			// zero-based index from ElementList...
	        public int		    type;			// this is not EQUAL with CGObj's one.
							                    // objType can have the value within UNITTYPE_TEXT, UNITTYPE_IMAGE and UNITTYPE_BARCODE.
	        public byte         page;
	        public byte         panel;
	        public int		    left;			// offsetLeft
	        public int		    top;			// offsetTop
	        public int		    width;
	        public int		    height;
	        public int		    rotate;
	        public Border   	border;
	        public BackGround	back;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct UNITINFOTEXT2 //: UNITINFOBASE2, UNITTEXT2
        {
            // UNITINFOBASE2
            public int          ver;
            public int          index;			// zero-based index from ElementList...
            public int          type;			// this is not EQUAL with CGObj's one.
                                                // objType can have the value within UNITTYPE_TEXT, UNITTYPE_IMAGE and UNITTYPE_BARCODE.
            public byte         page;
            public byte         panel;
            public int          left;			// offsetLeft
            public int          top;		    // offsetTop
            public int          width;
            public int          height;
            public int          rotate;
            public Border       border;
            public BackGround   back;
            // UNITTEXT2
	        public short	    leftMargin;		// spaceLeft
	        public short	    topMargin;		// spaceTop
	        public short	    rightMargin;	// spaceRight
	        public short	    bottomMargin;	// spaceBottom
	        public byte         align;
	        public FontInfo	    font;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_FIELDNAMELEN+1)]
	        public String	    field;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public Byte[]       reserved;
            // rest
            //[MarshalAs(UnmanagedType.ByValArray, SizeConst = SIZEOF_UNITMAX2-SIZEOF_UNITTEXT2)]
            //public Byte[]       reserved2;    // size --> 0
	        public Laser	    laser;          // UNITINFO2.ver >= UNITINFO_VER_2
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct UNITINFOIMAGE2 //: UNITINFOBASE2, UNITIMAGE2
        {
            // UNITINFOBASE2
            public int          ver;
            public int          index;			// zero-based index from ElementList...
            public int          type;			// this is not EQUAL with CGObj's one.
                                                // objType can have the value within UNITTYPE_TEXT, UNITTYPE_IMAGE and UNITTYPE_BARCODE.
            public byte         page;
            public byte         panel;
            public int          left;			// offsetLeft
            public int          top;		    // offsetTop
            public int          width;
            public int          height;
            public int          rotate;
            public Border       border;
            public BackGround   back;
            // UNITIMAGE2
	        public int		    option;
	        public int		    widthZoom;
	        public int		    heightZoom;
	        public short	    contrast;
	        public short	    brightness;
	        public int          grayscale;
	        public short        align;
	        public Win32.POINT	offset;
	        public byte         scaleMethod;
	        public int		    round;
	        public int          autoportrait;
	        public int          autoeffect;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_FIELDNAMELEN+1)]
	        public String	    field;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public byte[]       reserved;
            // rest
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = SIZEOF_UNITMAX2 - SIZEOF_UNITIMAGE2)]
            public byte[]       reserved2;
	        public  Laser	    laser;      //{{ UNITINFO2.ver >= UNITINFO_VER_2
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct UNITINFOBAR2 //: UNITINFOBASE2, UNITBAR2
        {
            // UNITINFOBASE2
            public int          ver;
            public int          index;			// zero-based index from ElementList...
            public int          type;			// this is not EQUAL with CGObj's one.
                                                // objType can have the value within UNITTYPE_TEXT, UNITTYPE_IMAGE and UNITTYPE_BARCODE.
            public byte         page;
            public byte         panel;
            public int          left;			// offsetLeft
            public int          top;		    // offsetTop
            public int          width;
            public int          height;
            public int          rotate;
            public Border       border;
            public BackGround   back;
            // UNITBAR2
	        public int		    bartype;
	        public int		    size;
	        public int		    option;
	        public Win32.SIZE	size2D;			// bar2DWidth, bar2DHeight
	        public int          barColor;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_FIELDNAMELEN+1)]
	        public String	    field;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public byte[]       reserved;
            // rest
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = SIZEOF_UNITMAX2 - SIZEOF_UNITBAR2)]
            public byte[]       reserved2;
	        public  Laser	    laser;      //{{ UNITINFO2.ver >= UNITINFO_VER_2
        }

        //--------------------------------------------------------------------------------------------



        public const int    OBJINFO2_VER_2	= 2;
        public const int    OBJINFO2_VER	= OBJINFO2_VER_2;

        public const int    LINE_FLIP_HORZ	= 0x01;
        public const int    LINE_FLIP_VERT	= 0x02;

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct OBJ_INFO2_BASE
        {
	        public int      oiVer;					// OBJINFO_VER
	        public int      oiSize;					// base structure size.  = sizeof(OBJ_INFO2)
	        public int      oiWholeSize;			// entire struct size. include datas.	= sizeof(OBJ_INFO2) + extend data size
	        public int      oiIndex;				// object index
	        public byte     oiPage;
	        public byte     oiPanel;
	        public int      oiLeft;
	        public int      oiTop;
	        public int      oiWidth;
	        public int      oiHeight;
	        public int      oiRotate;
	        public int      oiPrintable;			// print or not ...
	        public int      oiType;					// object type
	        public Border		oiBorder;
	        public BackGround	oiBack;
	        public Laser		oiLaser;
        }


        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct OBJ_INFO2__RRECT
        {
			public int	    oirrRound;
		}

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct OBJ_INFO2__TEXT
        {
	        public short		oitLeftSpace;
	        public short		oitTopSpace;
	        public short		oitRightSpace;
	        public short		oitBottomSpace;
	        public byte		    oitAlign;
	        public int			oitOption;
	        public FontInfo2	oitFont;
	        public int			oitField;
	        public int		    oitTextPos;			// offset position of wide-char string data..
	        public int		    oitTextLen;			// data length..

            public string       _oitText;           // 
		}

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct OBJ_INFO2__IMAGE
        {
	        public int			oiiWidth;
	        public int			oiiHeight;
	        public int  		oiiZoomWidth;		// unit is permil. IMGSCALE_BASEAMP: use origin size of image
	        public int  		oiiZoomHeight;		// unit is permil. IMGSCALE_BASEAMP: use origin size of image
	        public short		oiiContrast;		// -100 ~ . default value is 0.
	        public short		oiiBrightness;		// -256 ~ 255. default value is 0.
	        public int      	oiiGrayscaled;
	        public short		oiiAlign;
	        public Win32.POINT	oiiOffset;			// offset position in left-upper corner of frame
	        public byte 		oiiScale;			// image scaling method...
	        public int          oiiTransColor;
	        public int			oiiTransRange;
	        public int			oiiRound;
	        public int			oiiField;
	        public int  		oiiImgNamePos;		// offset position of wide-char string data..
	        public int  		oiiImgNameLen;		// data length..

            public string       _oiiImgName;        // 
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct OBJ_INFO2__BAR
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_BARCODENAME)]
	        public String	    oibName;
	        public int			oibSize;
	        public int			oibOpt;
	        public short		oib2DOpt1;
	        public short		oib2DOpt2;
	        public int      	oibColor;
	        public int			oibField;
	        public int		    oibDataPos;
	        public int		    oibDataLen;
	        public int		    oibExDataPos;		// zip-postal data. it is used only "MaxiCode" type.
	        public int		    oibExDataLen;		// zip-postal data. it is used only "MaxiCode" type.

            public string       _oibData;           // 
            public string       _oibExData;         // 
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct OBJ_INFO2__LINE
        {
            public int			oilFlip;			// LINE_FLIP_HORZ | LINE_FLIP_VERT
		}

        [StructLayout(LayoutKind.Sequential, Pack = 4, CharSet = CharSet.Unicode)]
        public struct OBJ_INFO2
        {
            public int oiVer;	            // OBJINFO_VER
            public int oiSize;	            // base structure size.  = sizeof(OBJ_INFO2)
            public int oiWholeSize;			// entire struct size. include datas.	= sizeof(OBJ_INFO2) + extend data size
            public int oiIndex;				// object index
            public int oiPage;              // byte
            public int oiPanel;             // byte
            public int oiLeft;
            public int oiTop;
            public int oiWidth;
            public int oiHeight;
            public int oiRotate;
            public int oiPrintable;			// print or not ...
            public int oiType;				// object type
            public Border oiBorder;        // 12 Byte
            public BackGround oiBack;      // 12 Byte
            public Laser oiLaser;          // 80 Byte

            //{{ union area
            public OBJ_INFO2__RRECT oiRRect;
            public OBJ_INFO2__TEXT oiText;
            public OBJ_INFO2__IMAGE oiImage;
            public OBJ_INFO2__BAR oiBar;
            public OBJ_INFO2__LINE oiLine;
            //}}
        }



        public const int        SIZEOF__OBJ_INFO2__RRECT    = 4;    //Marshal.SizeOf(typeof(OBJ_INFO2__RRECT));
        public const int        SIZEOF__OBJ_INFO2__TEXT     = 108;    //Marshal.SizeOf(typeof(OBJ_INFO2__TEXT));
        public const int        SIZEOF__OBJ_INFO2__IMAGE    = 64;    //Marshal.SizeOf(typeof(OBJ_INFO2__IMAGE));
        public const int        SIZEOF__OBJ_INFO2__BAR      = 164;    //Marshal.SizeOf(typeof(OBJ_INFO2__BAR));
        public const int        SIZEOF__OBJ_INFO2__LINE     = 4;    //Marshal.SizeOf(typeof(OBJ_INFO2__LINE));
        public const int        SIZEOF__OBJ_INFO2__MAX      = 164;    //Math.Max(Math.Max(Math.Max(Math.Max(SIZEOF__OBJ_INFO2__RRECT, SIZEOF__OBJ_INFO2__TEXT), SIZEOF__OBJ_INFO2__IMAGE), SIZEOF__OBJ_INFO2__BAR), SIZEOF__OBJ_INFO2__LINE);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct OBJ_INFO2_RRECT //: OBJ_INFO2_BASE, OBJ_INFO2__RRECT
        {
            // OBJ_INFO2_BASE
	        public int          oiVer;					// OBJINFO_VER
	        public int          oiSize;					// base structure size.  = sizeof(OBJ_INFO2)
	        public int          oiWholeSize;			// entire struct size. include datas.	= sizeof(OBJ_INFO2) + extend data size
	        public int          oiIndex;				// object index
	        public byte         oiPage;
	        public byte         oiPanel;
	        public int          oiLeft;
	        public int          oiTop;
	        public int          oiWidth;
	        public int          oiHeight;
	        public int          oiRotate;
	        public int          oiPrintable;			// print or not ...
	        public int          oiType;					// object type
	        public Border		oiBorder;
	        public BackGround	oiBack;
	        public Laser		oiLaser;
            // OBJ_INFO2__RRECT
			public int	    oirrRound;
            // rest
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = SIZEOF__OBJ_INFO2__MAX-SIZEOF__OBJ_INFO2__RRECT)]
            public byte[]       reserved;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct OBJ_INFO2_TEXT //: OBJ_INFO2_BASE, OBJ_INFO2__TEXT
        {
            // OBJ_INFO2_BASE
	        public int          oiVer;					// OBJINFO_VER
	        public int          oiSize;					// base structure size.  = sizeof(OBJ_INFO2)
	        public int          oiWholeSize;			// entire struct size. include datas.	= sizeof(OBJ_INFO2) + extend data size
	        public int          oiIndex;				// object index
	        public byte         oiPage;
	        public byte         oiPanel;
	        public int          oiLeft;
	        public int          oiTop;
	        public int          oiWidth;
	        public int          oiHeight;
	        public int          oiRotate;
	        public int          oiPrintable;			// print or not ...
	        public int          oiType;					// object type
	        public Border		oiBorder;
	        public BackGround	oiBack;
	        public Laser		oiLaser;
            // OBJ_INFO2__TEXT
	        public short		oitLeftSpace;
	        public short		oitTopSpace;
	        public short		oitRightSpace;
	        public short		oitBottomSpace;
	        public byte		    oitAlign;
	        public int			oitOption;
	        public FontInfo2	oitFont;
	        public int			oitField;
	        public int		    oitTextPos;			// offset position of wide-char string data..
	        public int		    oitTextLen;			// data length..
            // rest
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = SIZEOF__OBJ_INFO2__MAX - SIZEOF__OBJ_INFO2__TEXT)]
            public byte[]       reserved;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct OBJ_INFO2_IMAGE //: OBJ_INFO2_BASE, OBJ_INFO2__IMAGE
        {
            // OBJ_INFO2_BASE
	        public int          oiVer;					// OBJINFO_VER
	        public int          oiSize;					// base structure size.  = sizeof(OBJ_INFO2)
	        public int          oiWholeSize;			// entire struct size. include datas.	= sizeof(OBJ_INFO2) + extend data size
	        public int          oiIndex;				// object index
	        public byte         oiPage;
	        public byte         oiPanel;
	        public int          oiLeft;
	        public int          oiTop;
	        public int          oiWidth;
	        public int          oiHeight;
	        public int          oiRotate;
	        public int          oiPrintable;			// print or not ...
	        public int          oiType;					// object type
	        public Border		oiBorder;
	        public BackGround	oiBack;
	        public Laser		oiLaser;
            // OBJ_INFO2__IMAGE
	        public int			oiiWidth;
	        public int			oiiHeight;
	        public int  		oiiZoomWidth;		// unit is permil. IMGSCALE_BASEAMP: use origin size of image
	        public int  		oiiZoomHeight;		// unit is permil. IMGSCALE_BASEAMP: use origin size of image
	        public short		oiiContrast;		// -100 ~ . default value is 0.
	        public short		oiiBrightness;		// -256 ~ 255. default value is 0.
	        public int      	oiiGrayscaled;
	        public short		oiiAlign;
	        public Win32.POINT	oiiOffset;			// offset position in left-upper corner of frame
	        public byte 		oiiScale;			// image scaling method...
	        public int          oiiTransColor;
	        public int			oiiTransRange;
	        public int			oiiRound;
	        public int			oiiField;
	        public int  		oiiImgNamePos;		// offset position of wide-char string data..
	        public int  		oiiImgNameLen;		// data length..
            // rest
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = SIZEOF__OBJ_INFO2__MAX - SIZEOF__OBJ_INFO2__IMAGE)]
            public byte[]       reserved;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct OBJ_INFO2_BAR //: OBJ_INFO2_BASE, OBJ_INFO2__BAR
        {
            // OBJ_INFO2_BASE
	        public int          oiVer;					// OBJINFO_VER
	        public int          oiSize;					// base structure size.  = sizeof(OBJ_INFO2)
	        public int          oiWholeSize;			// entire struct size. include datas.	= sizeof(OBJ_INFO2) + extend data size
	        public int          oiIndex;				// object index
	        public byte         oiPage;
	        public byte         oiPanel;
	        public int          oiLeft;
	        public int          oiTop;
	        public int          oiWidth;
	        public int          oiHeight;
	        public int          oiRotate;
	        public int          oiPrintable;			// print or not ...
	        public int          oiType;					// object type
	        public Border		oiBorder;
	        public BackGround	oiBack;
	        public Laser		oiLaser;
            // OBJ_INFO2__BAR
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_BARCODENAME)]
	        public String	    oibName;
	        public int			oibSize;
	        public int			oibOpt;
	        public short		oib2DOpt1;
	        public short		oib2DOpt2;
	        public int      	oibColor;
	        public int			oibField;
	        public int		    oibDataPos;
	        public int		    oibDataLen;
	        public int		    oibExDataPos;		// zip-postal data. it is used only "MaxiCode" type.
	        public int		    oibExDataLen;		// zip-postal data. it is used only "MaxiCode" type.
            // rest
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = SIZEOF__OBJ_INFO2__MAX - SIZEOF__OBJ_INFO2__BAR)]
            public byte[]       reserved;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct OBJ_INFO2_LINE //: OBJ_INFO2_BASE, OBJ_INFO2__LINE
        {
            // OBJ_INFO2_BASE
	        public int          oiVer;					// OBJINFO_VER
	        public int          oiSize;					// base structure size.  = sizeof(OBJ_INFO2)
	        public int          oiWholeSize;			// entire struct size. include datas.	= sizeof(OBJ_INFO2) + extend data size
	        public int          oiIndex;				// object index
	        public byte         oiPage;
	        public byte         oiPanel;
	        public int          oiLeft;
	        public int          oiTop;
	        public int          oiWidth;
	        public int          oiHeight;
	        public int          oiRotate;
	        public int          oiPrintable;			// print or not ...
	        public int          oiType;					// object type
	        public Border		oiBorder;
	        public BackGround	oiBack;
	        public Laser		oiLaser;
            // OBJ_INFO2__LINE
            public int			oilFlip;			// LINE_FLIP_HORZ | LINE_FLIP_VERT
            // rest
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = SIZEOF__OBJ_INFO2__MAX - SIZEOF__OBJ_INFO2__LINE)]
            public byte[]       reserved;
        }





        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct DRAWTEXT2INFO
        {
	        public int			x;
	        public int			y;
	        public int			cx;
	        public int			cy;
	        public int			rotate;
	        public int			align;
	        public int			fontHeight;
	        public int			fontWidth;
	        public int			style;
	        public int  	    color;
	        public int			option;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = Win32.LF_FACESIZE)]
	        public String	    szFaceName;
        }


        public const int    MAX_FIELDDATALEN	= 1024;
        
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct FIELD_INFO
        {
	        public int          fiIndex;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_FIELDNAMELEN)]
	        public String	    fiName;
	        public int          fiDataLen;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_FIELDDATALEN)]
	        public String	    fiData;
        }







        public const int MAX_SMART_PRINTER = 32;

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct SMART_PRINTER_ITEM
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public String name;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
            public String id;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
            public String dev;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public String desc;
            public int pid;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct SMART_PRINTER_LIST
        {
            public int n;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MAX_SMART_PRINTER)]
            public SMART_PRINTER_ITEM[] item;
        }




        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct SMART_PRINTER_PORT_USB
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
            public String port;      // usb port
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public String link;     // symbolic link of usb port
            public int is_bridge;                                                           // Network module bridge
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct SMART_PRINTER_PORT_NET
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
            public String ver;       // version of network protocol
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
            public String ip;        // ip address
            public int port;                                                                // tcp port
            public int is_ssl;                                                              // ssl protocol
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct SMART_PRINTER_STANDARD
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public String name;     // printer name
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
            public String id;        // printer ID
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
            public String dev;       // device connection
            public int dev_type;                                                            // 1=USB, 2=NET
            public int pid;                                                                 // USB product ID
            public SMART_PRINTER_PORT_USB usb;
            public SMART_PRINTER_PORT_NET net;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct SMART_PRINTER_OPTIONS
        {
            public int is_dual;                                                             // dual printer
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
            public String ic1;       // internal contact encoder
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
            public String ic2;       // external contact SIM encoder
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
            public String rf1;       // internal contactless encoder
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
            public String rf2;       // external contactless encoder
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct SMART_PRINTER_INFO
        {
            public SMART_PRINTER_STANDARD std;
            public SMART_PRINTER_OPTIONS opt;
        }


        // @SmartComm_Move
        public const int    CARDPOS_PRINT		= 0;
        public const int    CARDPOS_MAGNETIC	= 1;
        public const int    CARDPOS_TOROT		= 4;			// move to rotator.
        public const int    CARDPOS_FROMROT		= 5;			// move from rotator. after move, must move to another position.
        public const int    CARDPOS_RF2			= 6;			// move to omnikey module position.
        public const int    CARDPOS_IC2			= 7;			// compatible with both SMART50 and SMART30.

        //@SmartCommEx_Rotate180
        public const int    SMART_FLIP_FACEDOWN	= 0;			// flip card to down faced.
        public const int    SMART_FLIP_FACEUP	= 1;			// flip card to up faced.


        // Magnetic
        public const byte   MAG_T1				= 0x01;
        public const byte   MAG_T2				= 0x02;
        public const byte   MAG_T3				= 0x04;
        public const byte   MAG_JIS				= 0x08;
        public const byte   MAG_T1MAX			= 76;
        public const byte   MAG_T2MAX			= 37;
        public const byte   MAG_T3MAX			= 104;
        public const byte   MAG_JISMAX			= 69;
        public const byte   MAG_NORMAL			= 0x00;
        public const byte   MAG_HIGHCO			= 0x10;
        public const byte   MAG_SUPERCO			= 0x20;
        public const byte   MAG_USERCO			= 0x30;
        public const byte   MAG_BITMODE			= 0x80;
        public const byte   MAG_LOCO			= MAG_NORMAL;
        public const byte   MAG_HICO			= MAG_HIGHCO;

        // @SMART51_SURFACE, @SMART51_DEVMODE
        public const uint  SMART51_MAG_LOCO		= 0;
        public const uint  SMART51_MAG_HICO		= 1;
        public const uint  SMART51_MAG_SPCO		= 2;
        public const uint  SMART51_MAG_AUTO		= 3;

        // @SMART51_SURFACE, @SMART51_DEVMODE
        public const uint  SMART51_MAG_NOREPEAT	= 0;
        public const uint  SMART51_MAG_REPEAT1	= 1;
        public const uint  SMART51_MAG_REPEAT2	= 2;
        public const uint  SMART51_MAG_REPEAT3	= 3;
        public const uint  SMART51_MAG_REPEAT4	= 4;

        // @SMART51_SURFACE, @SMART51_DEVMODE
        public const uint  SMART51_MAG_FORMAT_IATA	= 0;
        public const uint  SMART51_MAG_FORMAT_ABA	= 1;
        public const uint  SMART51_MAG_FORMAT_MINS	= 2;
        public const uint  SMART51_MAG_FORMAT_JIS2	= 3;
        public const uint  SMART51_MAG_FORMAT_BITS	= 4;


        // LCD Character
        public const int    LCD_LINE1			= 0;			// LCD Text Display Line Position. (Small Text Base)
        public const int    LCD_LINE2			= 1;
        public const int    LCD_LINE3			= 2;
        public const int    LCD_LINE4			= 3;
        public const int    LCD_LINE5			= 4;
        public const int    LCD_LINE6			= 5;
        public const int    LCD_LINE7			= 6;
        public const int    LCD_LINE8			= 7;

        public const int    LCD_TYPE_CH			= 0;			// character LCD : Big Sized Text
        public const int    LCD_TYPE_GSMALL		= 1;			// graphic LCD : Small Sized Text
        public const int    LCD_TYPE_GBIG		= 2;			// graphic LCD : Big Sized Text
        public const int    LCD_TYPE_144X32		= 3;			// graphic LCD : 144x32 Monochrome

        public const int    DISPLAYTYPE_144X32	= 2;			// device-information : display type value will be
                                                                //                      discarded, please, use below DISPLAY_##### instead this.

        public const int    LCD_LENSMALL		= 21;			// Length of Small Text
        public const int    LCD_LENBIG			= 16;			// Length of Big Text
        public const int    LCD_LEN_CH			= LCD_LENBIG;
        public const int    LCD_LEN_GSMALL		= LCD_LENSMALL;
        public const int    LCD_LEN_GBIG		= LCD_LENBIG;

        public const int    DISPLAY_T16X2		= 0;			// Text-LCD - 16 x 2 matrix
        public const int    DISPLAY_G128X64		= 1;			// Graphic-LCD - 128 x 64 matrix	// SMART-50/30.
        public const int    DISPLAY_G144X32		= 2;			// Graphic-LCD - 144 x 32 matrix	// SMART-50/30.
        public const int    DISPLAY51_G144X32	= 1;			// Graphic-LCD - 144 x 32 matrix	// SMART-51
        //#define	SMART51_DISPLAYTYPE(_d)		( ((_d)==DISPLAY_T16X2)?DISPLAY_T16X2:DISPLAY51_G144X32);

        public const int    DISPLAY_LANG_ENG	= 0;			// Display Language : English
        public const int    DISPLAY_LANG_ARB	= 1;			// Display Language : Arabic

        public const uint   DISPLAY_TYPE_MASK	= 0x000000FF;
        public const uint   DISPLAY_G_LANG_ENG	= 0x00000000;	// english
        public const uint   DISPLAY_G_LANG_ARB	= 0x00000100;	// arabic
        public const uint   DISPLAY_LANG_MASK	= 0xFFFFFF00;


        // Contactless PC/SC : Auth Side
        public const byte   MIFARE_AUTHENT1A	= 0x60;		// authentication with Mifare key A
        public const byte   MIFARE_AUTHENT1B	= 0x61;		// authentication with Mifare key B


        // Device
        public const int    INTERNALDEV			= 1;			// internal contact/contactless device
        public const int    EXTERNALDEV			= 2;			// external contact/contactless device




        //_________________________________________
        //
        // Status Code Definition.
        //_________________________________________
        //


        // SMART-50 PRINTER Status Code Definition.

        // MOTION-PART
        public const UInt64 SMSC_M_CARDIN				= 0x0000000000000001;	// Under card in
        public const UInt64 SMSC_M_CARDOUT				= 0x0000000000000002;	// Under card out
        public const UInt64 SMSC_M_MOVE_PRINT			= 0x0000000000000004;	// Move to print position
        public const UInt64 SMSC_M_MOVE_PRN2ROT			= 0x0000000000000008;	// Move to roator from printer
        public const UInt64 SMSC_M_MOVE_ROT2PRN			= 0x0000000000000010;	// Move to pritner from rotator
        public const UInt64 SMSC_M_MOVE_IC				= 0x0000000000000020;	// Move to IC position
        public const UInt64 SMSC_M_MOVE_RF				= 0x0000000000000040;	// Move to contactless IC position
        public const UInt64 SMSC_M_MOVE_MAG				= 0x0000000000000080;	// Move to the magnetic stripe position
        public const UInt64 SMSC_M_THUP					= 0x0000000000000100;	// Under Thermal head up
        public const UInt64 SMSC_M_THDOWN				= 0x0000000000000200;	// Under Thermal head down
        public const UInt64 SMSC_M_ICHUP				= 0x0000000000000400;	// Under IC head(Contactor) up
        public const UInt64 SMSC_M_ICHDOWN				= 0x0000000000000800;	// Under IC head(Contactor) down
        public const UInt64 SMSC_M_PRINT				= 0x0000000000001000;	// Under printing
        public const UInt64 SMSC_M_MAGRW				= 0x0000000000002000;	// Under Mag read/write
        public const UInt64 SMSC_M_SEEKRIBBON			= 0x0000000000004000;	// Under ribbon finding
        public const UInt64 SMSC_M_MOVERIBBON			= 0x0000000000008000;	// Under ribbon moving
        public const UInt64 SMSC_M_ROTATORTOP			= 0x0000000000010000;	// Card front side rotate to upper
        public const UInt64 SMSC_M_ROTATORBOTTOM		= 0x0000000000020000;	// Card front side rotate to down
        public const UInt64 SMSC_S_HOPPERHASCARD		= 0x0000000000040000;	// Card is in hopper
        public const UInt64 SMSC_S_THUP					= 0x0000000000080000;	// Thermal head up state   (f/w ver. 1.00.59 or lower)
        public const UInt64 SMSC_S_CLEANWARNING			= 0x0000000000080000;	// Warn need printer clean (f/w ver. 1.00.60 or higher)
        public const UInt64 SMSC_S_CARDIN				= 0x0000000000100000;	// Card checking by card in sensor
        public const UInt64 SMSC_S_CARDOUT				= 0x0000000000200000;	// Card checking by card out sensor
        public const UInt64 SMSC_S_ROTATORTOP			= 0x0000000000400000;	// Card front is upside in rotator  (f/w ver. 1.00.59 or lower)
        public const UInt64 SMSC_S_EQUIPLAMINATOR		= 0x0000000000400000;	// Laminator is installed           (f/w ver. 1.00.60 or higher)
        public const UInt64 SMSC_S_EQUIPROTATOR			= 0x0000000000800000;	// Rotator is installed
        public const UInt64 SMSC_M_RECVPRINTDATA		= 0x0000000001000000;	// Uner receiving the printer buffer
        public const UInt64 SMSC_S_HASPRINTBUFFER		= 0x0000000002000000;	// Unser saving the printing data buffer
        public const UInt64 SMSC_M_SBSRUNNING			= 0x0000000004000000;	// Under executing SBS command
        public const UInt64 SMSC_S_SBSMODE				= 0x0000000008000000;	// SBS mode state
        public const UInt64 SMSC_S_CASEOPEN				= 0x0000000010000000;	// Case(cover) open state
        public const UInt64 SMSC_M_INIT					= 0x0000000020000000;	// Under initialization
        public const UInt64 SMSC_S_TESTMODE				= 0x0000000080000000;	// Under test mode

        public const UInt64 SMSC_M_MASK					= 0x000000002503FFFF;
        public const UInt64 SMSC_S_MASK					= 0x000000009AFC0000;

        // ERROR-PART
        public const UInt64 SMSC_F_CARDIN				= 0x0000000100000000;	// Card in error
        public const UInt64 SMSC_F_MOVETOPRINT			= 0x0000000200000000;	// Card moving error to the printing position
        public const UInt64 SMSC_F_CARDOUT				= 0x0000000400000000;	// Card out error
        public const UInt64 SMSC_F_MOVETOMAG			= 0x0000000800000000;	// Card moving error to the magstripe position
        public const UInt64 SMSC_F_MOVETOIC				= 0x0000001000000000;	// Card moving error to IC position
        public const UInt64 SMSC_F_MOVETORF				= 0x0000002000000000;	// Card moving error to contactless position
        public const UInt64 SMSC_F_MOVETOROTATOR		= 0x0000004000000000;	// Fail to move to rotator
        public const UInt64 SMSC_F_MOVEFROMROTATOR		= 0x0000008000000000;	// Fail to move to printer from rotator
        public const UInt64 SMSC_F_THUP					= 0x0000010000000000;	// Thermal head up fail
        public const UInt64 SMSC_F_THDOWN				= 0x0000020000000000;	// Thermal head down fail
        public const UInt64 SMSC_F_ICHUP				= 0x0000040000000000;	// IC head(Contactor) up fail
        public const UInt64 SMSC_F_ICHDOWN				= 0x0000080000000000;	// IC head(Contactor) down fail
        public const UInt64 SMSC_F_ROTATORTOP			= 0x0000100000000000;	// Card front side rotating fail to upside
        public const UInt64 SMSC_F_ROTATORBOTTOM		= 0x0000200000000000;	// Card front side rotating fail to down side
        public const UInt64 SMSC_F_PRINT				= 0x0000400000000000;	// Fail in printing
        public const UInt64 SMSC_F_MAGRW				= 0x0000800000000000;	// Magnetic Read/Write fail
        public const UInt64 SMSC_E_SEEKRIBBON			= 0x0001000000000000;	// Ribbon search error
        public const UInt64 SMSC_E_MOVERIBBON			= 0x0002000000000000;	// Ribbon moving error
        public const UInt64 SMSC_E_NOTH					= 0x0004000000000000;	// Thermal head uninstalled
        public const UInt64 SMSC_E_THOVERHEAT			= 0x0008000000000000;	// Thermal head over heated
        public const UInt64 SMSC_E_EMPTYRIBBON			= 0x0010000000000000;	// No ribbon
        public const UInt64 SMSC_F_DATA					= 0x0020000000000000;	// Printing data error
        public const UInt64 SMSC_F_CARDBACKOUT			= 0x0040000000000000;	// card back out fail
        public const UInt64 SMSC_F_CARDERASE			= 0x0080000000000000;	// Card data removing fail
        public const UInt64 SMSC_F_INCORRECT_PW			= 0x0100000000000000;	// PassWord Incorrect
        public const UInt64 SMSC_F_MAGREADT1			= 0x0200000000000000;	// Magnetic Track 1 Read Failed
        public const UInt64 SMSC_F_MAGREADT2			= 0x0400000000000000;	// Magnetic Track 2 Read Failed
        public const UInt64 SMSC_F_MAGREADT3			= 0x0800000000000000;	// Magnetic Track 3 Read Failed
        public const UInt64 SMSC_F_LOCKED				= 0x1000000000000000;	// device is locked status
        public const UInt64 SMSC_F_SPOOLFULL			= 0x2000000000000000;	// printer's spool is fulled
        public const UInt64 SMSC_F_SET					= 0x4000000000000000;	// printer setting is failed (like, changing pw., ...)

        public const UInt64 SMSC_F_MASK					= 0x0FE0FFFF00000000;
        public const UInt64 SMSC_E_MASK					= 0x001F000000000000;



        // SMART-50 LAMINATOR Status Code Definition.
        //

        // MOTION-PART
        public const UInt64 LMSC_M_HEATHDRLIFTUP		= 0x0000000000000001;	// Under lifting up Heat header
        public const UInt64 LMSC_M_HEATHDRLIFTDOWN   	= 0x0000000000000004;	// Under lifting down Heat header
        public const UInt64 LMSC_M_CARDIN				= 0x0000000000000010;	// Under card in
        public const UInt64 LMSC_M_MOVE_LAMINATE		= 0x0000000000000040;	// Under Moving to laminate position
        public const UInt64 LMSC_M_FRONTCARDOUT			= 0x0000000000000100;	// Under card out to front (printer side)
        public const UInt64 LMSC_M_REARCARDOUT			= 0x0000000000000200;	// Under card out to rear (stacker side)
        public const UInt64 LMSC_M_ROTATE      			= 0x0000000000000400;	// Under rotating
        public const UInt64 LMSC_S_WAIT					= 0x0000000000000800;	// Under Wait
        public const UInt64 LMSC_S_CMDRUN				= 0x0000000000002000;	// Under executing command
        public const UInt64 LMSC_M_HEATING				= 0x0000000000004000;	// Under heating Heat header
        public const UInt64 LMSC_S_CASEOPEN				= 0x0000000000008000;	// top cover is opened
        public const UInt64 LMSC_M_LAMINATING			= 0x0000000000010000;	// Under Laminating
        public const UInt64 LMSC_S_CARDINSENSOR			= 0x0000000020000000;	// caught by card in sensor
        public const UInt64 LMSC_S_CARDOUTSENSOR		= 0x0000000040000000;	// caught by card out sensor
        public const UInt64 LMSC_S_OUTDOORSENSOR		= 0x0000000080000000;	// caught by stacker open sensor

        public const UInt64 LMSC_M_MASK					= 0x0000000000014755;
        public const UInt64 LMSC_S_MASK					= 0x00000000E000A800;

        // ERROR-PART
        public const UInt64 LMSC_E_HEATHDRLIFTUP		= 0x0000000100000000;	// Error while lift up Heat header
        public const UInt64 LMSC_E_HEATHDRLIFTDOWN		= 0x0000000400000000;	// Error while lift down Heat header
        public const UInt64 LMSC_E_CARDIN				= 0x0000001000000000;	// Error while card in
        public const UInt64 LMSC_E_MOVE_LAMINATE		= 0x0000004000000000;	// Error while move the card to Laminate position
        public const UInt64 LMSC_E_FRONTCARDOUT			= 0x0000010000000000;	// Error while card out to front (printer side)
        public const UInt64 LMSC_E_REARCARDOUT			= 0x0000020000000000;	// Error while card out to rear (stacker side)
        public const UInt64 LMSC_E_ROTATE				= 0x0000040000000000;	// Error while rotate
        public const UInt64 LMSC_E_INIT					= 0x0000800000000000;	// Error while initialize laminator
        public const UInt64 LMSC_E_EMPTYFILM			= 0x0001000000000000;	// Film remain count is 0.
        public const UInt64 LMSC_E_NOFILM				= 0x8000000000000000;	// No film is installed or cannot recognize the film.

        public const UInt64 LMSC_F_MASK					= 0x0000000000000000;
        public const UInt64 LMSC_E_MASK					= 0x8001875500000000;






        // SMART-51 Printer Status Code Definition.
        //

        // MOTION-PART
        public const UInt64 S51PS_M_CARDIN				= 0x0000000000000001;	// inserting card
        public const UInt64 S51PS_M_CARDMOVE			= 0x0000000000000002;	// moving card
        public const UInt64 S51PS_M_CARDMOVEEXT			= 0x0000000000000004;	// moving card between external
        public const UInt64 S51PS_M_CARDEJECT			= 0x0000000000000008;	// ejecting card
        public const UInt64 S51PS_M_THEADLIFT			= 0x0000000000000010;	// lifting up/down thermal head
        public const UInt64 S51PS_M_ICLIFT				= 0x0000000000000020;	// lifting up/down ic connector
        public const UInt64 S51PS_M_RIBBONSEARCH		= 0x0000000000000040;	// searching ribbon
        public const UInt64 S51PS_M_RIBBONWIND			= 0x0000000000000080;	// winding ribbon
        public const UInt64 S51PS_M_MAGNETIC			= 0x0000000000000100;	// processing magnetic
        public const UInt64 S51PS_M_PRINT				= 0x0000000000000200;	// printing
        public const UInt64 S51PS_M_INIT				= 0x0000000000000400;	// initializing
        public const UInt64 S51PS_S_CONNHOPPER			= 0x0000000000000800;	// hopper connected
        public const UInt64 S51PS_S_CONNICENCODEER		= 0x0000000000001000;	// ic encoder connected
        public const UInt64 S51PS_S_CONNMAGNETIC		= 0x0000000000002000;	// magnetic encoder connected
        public const UInt64 S51PS_S_CONNLAMINATOR		= 0x0000000000004000;	// laminator connected
        public const UInt64 S51PS_S_CONNFLIPPER			= 0x0000000000008000;	// flipper connected
        public const UInt64 S51PS_S_FLIPPERTOP			= 0x0000000000010000;	// flipper is top sided
        public const UInt64 S51PS_S_COVEROPENED			= 0x0000000000020000;	// cover is opened
        public const UInt64 S51PS_S_DETECTIN			= 0x0000000000040000;	// detect a card from in sensor
        public const UInt64 S51PS_S_DETECTOUT			= 0x0000000000080000;	// detect a card from out sensor
        public const UInt64 S51PS_S_CARDEMPTY			= 0x0000000000100000;	// card empty
        public const UInt64 S51PS_S_RECVPRINTDATA		= 0x0000000000200000;	// receiving print data
        public const UInt64 S51PS_S_HAVEPRINTDATA		= 0x0000000000400000;	// having print data
        public const UInt64 S51PS_S_NEEDCLEANING		= 0x0000000004000000;	// need cleaning
        public const UInt64 S51PS_S_SWLOCKED			= 0x0000000008000000;	// system locked (sw)
        public const UInt64 S51PS_S_HWLOCKED			= 0x0000000010000000;	// system locked (hw)
        public const UInt64 S51PS_M_SBSCOMMAND			= 0x0000000020000000;	// doing SBS command
        public const UInt64 S51PS_S_SBSMODE				= 0x0000000040000000;	// under SBS mode
        public const UInt64 S51PS_S_TESTMODE			= 0x0000000080000000;	// test mode

        public const UInt64 S51PS_M_MASK				= (S51PS_M_CARDIN|S51PS_M_CARDMOVE|S51PS_M_CARDMOVEEXT|S51PS_M_CARDEJECT|S51PS_M_THEADLIFT|S51PS_M_ICLIFT|S51PS_M_RIBBONSEARCH|S51PS_M_RIBBONWIND|S51PS_M_MAGNETIC|S51PS_M_PRINT|S51PS_M_INIT|S51PS_M_SBSCOMMAND);
        public const UInt64 S51PS_S_MASK				= (S51PS_S_CONNHOPPER|S51PS_S_CONNICENCODEER|S51PS_S_CONNMAGNETIC|S51PS_S_CONNLAMINATOR|S51PS_S_CONNFLIPPER|S51PS_S_FLIPPERTOP|S51PS_S_COVEROPENED|S51PS_S_DETECTIN|S51PS_S_DETECTOUT|S51PS_S_CARDEMPTY|S51PS_S_RECVPRINTDATA|S51PS_S_HAVEPRINTDATA|S51PS_S_NEEDCLEANING|S51PS_S_SWLOCKED|S51PS_S_HWLOCKED|S51PS_S_SBSMODE|S51PS_S_TESTMODE);


        // ERROR-PART
        public const UInt64 S51PS_F_CARDIN				= 0x0000000100000000;	// failed to insert card
        public const UInt64 S51PS_F_CARDMOVE			= 0x0000000200000000;	// failed to move card
        public const UInt64 S51PS_F_CARDMOVEEXT			= 0x0000000400000000;	// failed to move card between external
        public const UInt64 S51PS_F_CARDEJECT			= 0x0000000800000000;	// failed to eject card
        public const UInt64 S51PS_F_THEADLIFT			= 0x0000001000000000;	// failed to lift up/down thermal head
        public const UInt64 S51PS_F_ICLIFT				= 0x0000002000000000;	// failed to lift up/down ic connector
        public const UInt64 S51PS_F_RIBBONSEARCH		= 0x0000004000000000;	// failed to search ribbon
        public const UInt64 S51PS_F_RIBBONWIND			= 0x0000008000000000;	// failed to wind ribbon
        public const UInt64 S51PS_F_MAGNETIC			= 0x0000010000000000;	// failed to read/write magnetic
        public const UInt64 S51PS_F_READMAGT1			= 0x0000020000000000;	// failed to read magnetic track 1
        public const UInt64 S51PS_F_READMAGT2			= 0x0000040000000000;	// failed to read magnetic track 2
        public const UInt64 S51PS_F_READMAGT3			= 0x0000080000000000;	// failed to read magnetic track 3
        public const UInt64 S51PS_F_PRINT				= 0x0000100000000000;	// error from printing
        public const UInt64 S51PS_E_INIT				= 0x0000200000000000;	// error from initializing
        public const UInt64 S51PS_E_CONNEXT				= 0x0000400000000000;	// error from device connection -failed to connect
        public const UInt64 S51PS_E_CONNLAMINATOR		= 0x0000800000000000;	// error from device connection -laminator
        public const UInt64 S51PS_E_CONNFLIPPER			= 0x0001000000000000;	// error from device connection -flipper
        public const UInt64 S51PS_E_RIBBON0				= 0x0020000000000000;	// ribbon remain 0
        public const UInt64 S51PS_E_NORIBBON			= 0x0040000000000000;	// no ribbon
        public const UInt64 S51PS_E_NOTHEAD				= 0x0080000000000000;	// no thermal head
        public const UInt64 S51PS_E_OVERHEAT			= 0x0100000000000000;	// thermal head overheat
        public const UInt64 S51PS_F_INVALIDPRINTDATA	= 0x0200000000000000;	// invalid printing data format
        public const UInt64 S51PS_F_INVALIDPASSWORD		= 0x0400000000000000;	// invalid password
        public const UInt64 S51PS_F_SET					= 0x4000000000000000;	// failed to set
        public const UInt64 S51PS_F_SPOOLFULL			= 0x8000000000000000;	// fulled spool pool

        public const UInt64 S51PS_F_MASK				= (S51PS_F_CARDIN|S51PS_F_CARDMOVE|S51PS_F_CARDMOVEEXT|S51PS_F_CARDEJECT|S51PS_F_THEADLIFT|S51PS_F_ICLIFT|S51PS_F_RIBBONSEARCH|S51PS_F_RIBBONWIND|S51PS_F_MAGNETIC|S51PS_F_READMAGT1|S51PS_F_READMAGT2|S51PS_F_READMAGT3|S51PS_F_PRINT|S51PS_F_INVALIDPRINTDATA);
        public const UInt64 S51PS_E_MASK				= (S51PS_E_INIT|S51PS_E_CONNEXT|S51PS_E_CONNLAMINATOR|S51PS_E_CONNFLIPPER|S51PS_E_RIBBON0|S51PS_E_NORIBBON|S51PS_E_NOTHEAD|S51PS_E_OVERHEAT);



        // SMART-51 Flipper Status Code Definition.
        //

        // MOTION-PART
        public const UInt64 S51FS_S_READY				= 0x0000000000000001;	// ready
        public const UInt64 S51FS_S_BUSY				= 0x0000000000000002;	// busy (doing something)
        public const UInt64 S51FS_M_CARDMOVE			= 0x0000000000000004;	// moving card
        public const UInt64 S51FS_M_CARDIN				= 0x0000000000000008;	// inserting card
        public const UInt64 S51FS_M_CARDEJECT			= 0x0000000000000010;	// ejecting card
        public const UInt64 S51FS_M_FLIP				= 0x0000000000000020;	// flipping
        public const UInt64 S51FS_S_FLIPTRAYTOPSIDED	= 0x0000000000000100;	// flip tray top sided
        public const UInt64 S51FS_S_ACTIVATEDREARSENSOR	= 0x0000000002000000;	// activated rear sensor
        public const UInt64 S51FS_M_CARDMOVESTEPMOTOR	= 0x0000000004000000;	// working card move step motor
        public const UInt64 S51FS_M_FLIPSTEPMOTOR		= 0x0000000008000000;	// working flip step motor
        public const UInt64 S51FS_S_COVERCLOSED			= 0x0000000010000000;	// cover is closed
        public const UInt64 S51FS_S_CENTERSENSOR		= 0x0000000020000000;	// center sensor
        public const UInt64 S51FS_S_REARSENSOR			= 0x0000000040000000;	// rear sensor
        public const UInt64 S51FS_S_FLIPSENSOR			= 0x0000000080000000;	// flip sensor

        public const UInt64 S51FS_M_MASK				= (S51FS_S_READY|S51FS_S_BUSY|S51FS_S_FLIPTRAYTOPSIDED|S51FS_S_ACTIVATEDREARSENSOR|S51FS_S_COVERCLOSED|S51FS_S_CENTERSENSOR|S51FS_S_REARSENSOR|S51FS_S_FLIPSENSOR);
        public const UInt64 S51FS_S_MASK				= (S51FS_M_CARDMOVE|S51FS_M_CARDIN|S51FS_M_CARDEJECT|S51FS_M_FLIP|S51FS_M_CARDMOVESTEPMOTOR|S51FS_M_FLIPSTEPMOTOR);


        // ERROR-PART
        public const UInt64 S51FS_F_CARDIN				= 0x0000000100000000;	// failed to card insert
        public const UInt64 S51FS_F_CARDMOVE			= 0x0000000200000000;	// failed to card move
        public const UInt64 S51FS_F_CARDEJECT			= 0x0000000400000000;	// failed to card eject
        public const UInt64 S51FS_F_MOVEFLIPTRAY		= 0x0000000800000000;	// failed to move flip tray
        public const UInt64 S51FS_F_COMMAND				= 0x0000001000000000;	// failed while process command
        public const UInt64 S51FS_E_INIT				= 0x0001000000000000;	// error from initializing

        public const UInt64 S51FS_F_MASK				= (S51FS_F_CARDIN|S51FS_F_CARDMOVE|S51FS_F_CARDEJECT|S51FS_F_MOVEFLIPTRAY|S51FS_F_COMMAND);
        public const UInt64 S51FS_E_MASK				= (S51FS_E_INIT);



        // SMART-51 Laminator Status Code Definition.
        //

        // MOTION-PART
        public const UInt64 S51LS_S_READY				= 0x0000000000000001;	// ready
        public const UInt64 S51LS_S_BUSY				= 0x0000000000000002;	// busy (doing something)
        public const UInt64 S51LS_M_CARDMOVE			= 0x0000000000000004;	// moving card
        public const UInt64 S51LS_M_CARDIN				= 0x0000000000000008;	// inserting card
        public const UInt64 S51LS_M_CARDEJECT			= 0x0000000000000010;	// ejecting card
        public const UInt64 S51LS_M_THEADLIFT			= 0x0000000000000020;	// lifting up/down thermal head
        public const UInt64 S51LS_M_LAMINATE			= 0x0000000000000040;	// laminating
        public const UInt64 S51LS_M_FLIPTRAYMOVE		= 0x0000000000000080;	// moving flip tray
        public const UInt64 S51LS_S_FLIPTRAYTOPSIDED	= 0x0000000000000100;	// top sided flipper tray
        public const UInt64 S51LS_M_HEATHEADHEAT		= 0x0000000000010000;	// heating heat-head
        public const UInt64 S51LS_M_FILMMOTOR			= 0x0000000000020000;	// working film motor
        public const UInt64 S51LS_M_HEADMOTOR			= 0x0000000000040000;	// working head motor
        public const UInt64 S51LS_M_CARDMOVESTEPMOTOR	= 0x0000000000080000;	// working card move step motor
        public const UInt64 S51LS_M_FLIPSTEPMOTOR		= 0x0000000000100000;	// working flip step motor
        public const UInt64 S51LS_S_DETECTENCODERCNTINC	= 0x0000000000200000;	// detect increasing encoder count
        public const UInt64 S51LS_S_DETECTENCODERCNTDEC	= 0x0000000000400000;	// detect decreasing encoder count
        public const UInt64 S51LS_S_COVEROPENED			= 0x0000000000800000;	// cover is opened
        public const UInt64 S51LS_S_LOCKSENSOR			= 0x0000000001000000;	// lock sensor
        public const UInt64 S51LS_S_CARDCENTERSENSOR	= 0x0000000002000000;	// card center sensor
        public const UInt64 S51LS_S_CARDOUTSENSOR		= 0x0000000004000000;	// card out sensor
        public const UInt64 S51LS_S_FLIPPERCENTERSENSOR	= 0x0000000008000000;	// flipper center sensor
        public const UInt64 S51LS_S_FLIPPERFLIPSENSOR	= 0x0000000010000000;	// flipper flip sensor
        public const UInt64 S51LS_S_HEADSENSOR			= 0x0000000020000000;	// head up/down sensor
        public const UInt64 S51LS_S_FILMMARKMAINSENSOR	= 0x0000000040000000;	// film mark main sensor
        public const UInt64 S51LS_S_FILMMARKSUBSENSOR	= 0x0000000080000000;	// film mark sub sensor

        public const UInt64 S51LS_M_MASK				= (S51LS_S_READY|S51LS_S_BUSY|S51LS_S_FLIPTRAYTOPSIDED|S51LS_S_DETECTENCODERCNTINC|S51LS_S_DETECTENCODERCNTDEC|S51LS_S_COVEROPENED|S51LS_S_LOCKSENSOR|S51LS_S_CARDCENTERSENSOR|S51LS_S_CARDOUTSENSOR|S51LS_S_FLIPPERCENTERSENSOR|S51LS_S_FLIPPERFLIPSENSOR|S51LS_S_HEADSENSOR|S51LS_S_FILMMARKMAINSENSOR|S51LS_S_FILMMARKSUBSENSOR);
        public const UInt64 S51LS_S_MASK				= (S51LS_M_CARDMOVE|S51LS_M_CARDIN|S51LS_M_CARDEJECT|S51LS_M_THEADLIFT|S51LS_M_LAMINATE|S51LS_M_FLIPTRAYMOVE|S51LS_M_HEATHEADHEAT|S51LS_M_FILMMOTOR|S51LS_M_HEADMOTOR|S51LS_M_CARDMOVESTEPMOTOR|S51LS_M_FLIPSTEPMOTOR);


        // ERROR-PART
        public const UInt64 S51LS_F_CARDIN				= 0x0000000100000000;	// failed to insert card
        public const UInt64 S51LS_F_CARDMOVE			= 0x0000000200000000;	// failed to move card
        public const UInt64 S51LS_F_CARDEJECT			= 0x0000000400000000;	// failed to eject card
        public const UInt64 S51LS_F_HEADLIFT			= 0x0000000800000000;	// failed to lift head
        public const UInt64 S51LS_F_LAMINATE			= 0x0000001000000000;	// error from laminating
        public const UInt64 S51LS_F_COMMAND				= 0x0000002000000000;	// error while process command
        public const UInt64 S51LS_F_FLIPTRAYMOVE		= 0x0000004000000000;	// error from moving flip tray
        public const UInt64 S51LS_E_INIT				= 0x0001000000000000;	// error while initializing
        public const UInt64 S51LS_E_FILMSEARCH			= 0x0002000000000000;	// error from film search
        public const UInt64 S51LS_E_FILM0				= 0x0004000000000000;	// film remain 0
        public const UInt64 S51LS_E_NOFILM				= 0x0008000000000000;	// no film
        public const UInt64 S51LS_E_HEADOVERHEAT		= 0x0010000000000000;	// head overheat or failed to control heat

        public const UInt64 S51LS_F_MASK				= (S51LS_F_CARDIN|S51LS_F_CARDMOVE|S51LS_F_CARDEJECT|S51LS_F_HEADLIFT|S51LS_F_LAMINATE|S51LS_F_COMMAND|S51LS_F_FLIPTRAYMOVE);
        public const UInt64 S51LS_E_MASK				= (S51LS_E_INIT|S51LS_E_FILMSEARCH|S51LS_E_FILM0|S51LS_E_NOFILM|S51LS_E_HEADOVERHEAT);



        // Configuration Item Definition.
        public const int    SMEX_CFG_PRNSAMEASDRV		= 0x000F;		// smart-51 only. prn data will be generated same as drivers one.
        public const int    SMEX_CFG_STATUS51TO50		= 0x000E;		// change 51 status code to 50.	// caution : it *CAN LOOSE* some status bits.
        public const int    SMEX_CFG_RESOLUTION			= 0x0010;		// resolution. (300x300, 300x600, 300x1200, 600x600, 600x1200)	// 2018.03.13


        // Configuration Value Definition.

        // General Configuration
        public const int    SMEX_CFG_NOTUSE				= 0;
        public const int    SMEX_CFG_USE				= 1;

        // SMEX_CFG_RESOLUTION
        public const int    SMEX_CFG_RES300X300			= 1;			// resolution : 300x300 dpi
        public const int    SMEX_CFG_RES300X600			= 2;			// resolution : 300x600 dpi
        public const int    SMEX_CFG_RES300X1200		= 3;			// resolution : 300x1200 dpi
        public const int    SMEX_CFG_RES600X600			= 6;			// resolution : 600x600 dpi
        public const int    SMEX_CFG_RES600X1200		= 7;			// resolution : 600x1200 dpi







        // SMART-50/30 Magnetic Strip Encoding Mode
        public const byte   SMART_ENCODE_NORMAL			= 0x00;	// normal stripe order
        public const byte   SMART_ENCODE_REVERSE		= 0x01;	// reverse direction encoding...
        public const byte   SMART_ENCODE_BIT			= 0x02;	//
        public const byte   SMART_ENCODE_HICO_NORM		= 0x03;	//
        public const byte   SMART_ENCODE_HICO_REV		= 0x04;	//
        public const byte   SMART_ENCODE_HICO_BIT		= 0x05;	//

        // SMART-51/31 Magnetic Encoding Format
        public const int    SMART_MAG_FORMAT_IATA	    = 0;
        public const int    SMART_MAG_FORMAT_ABA	    = 1;
        public const int    SMART_MAG_FORMAT_MINS	    = 2;
        public const int    SMART_MAG_FORMAT_JIS2	    = 3;
        public const int    SMART_MAG_FORMAT_BITS	    = 4;


        
        // @SmartCommEx_GetDevParam2
        // @SmartCommEx_SetDevParam2
        public const int    SMART51_DEV_PRINTER		    = 0x00;
        public const int    SMART51_DEV_EXTERNAL	    = 0x80;


        // @SmartKiosk_Hopper
        // @SmartKiosk_CardIn2
        public const int    KIOSK_HOPPER_SCAN		    = 0;
        public const int    KIOSK_HOPPER_STATUS		    = 1;

        public const byte   KIOSK_HOPPER_AUTO		    = 0;
        public const byte   KIOSK_HOPPER_1			    = 0x01;
        public const byte   KIOSK_HOPPER_2			    = 0x02;
        public const byte   KIOSK_HOPPER_3			    = 0x04;
        public const byte   KIOSK_HOPPER_4			    = 0x08;
        public const byte   KIOSK_HOPPER_5			    = 0x10;
        public const byte   KIOSK_HOPPER_6			    = 0x20;

        public const uint   KIOSK_HOPPER_READY		    = 0;			// == SM_SUCCESS
        public const uint   KIOSK_HOPPER_ERROR		    = 0x8000000F;	// == SM_F_ERRORSTATUS
        public const uint   KIOSK_HOPPER_EMPTY		    = 0x8000000B;	// == SM_F_HOPPEREMPTY
        public const uint   KIOSK_HOPPER_NOTEXIST	    = 0x80000003;	// == SM_F_NOTEXISTDEV
        public const uint   KIOSK_HOPPER_NEAREMPTY	    = 0x80000382;	// == SM_W_NEAREMPTYHOPPER


        // @SmartComm_MoveSensor
        public const int    SMART51_SENSOR_NONE			= 0;
        public const int    SMART51_SENSOR_CENTER		= 1;
        public const int    SMART51_SENSOR_REAR			= 2;
        public const int    SMART51_SENSOR_FLIPMOTOR	= 3;
        public const int    SMART51_SENSOR_FLIPCENTER	= 4;
        public const int    SMART51_SENSOR_FLIPREAR		= 5;
        public const int    SMART51_SENSOR_FLIPANGLE	= 6;
        

        // @SmartComm_MagConfig
        public const int    MAGCFG_VER_1				= 1;

        public const int    MAGCFG_BALLYS_NOTUSE		= 0;
        public const int    MAGCFG_BALLYS_1				= 1;
        public const int    MAGCFG_BALLYS_2				= 2;

        public const int    MAGCFG_FORWARD				= 0;
        public const int    MAGCFG_BACKWARD				= 1;

        public const int    MAGCFG_FORMAT_IATA			= SMART_MAG_FORMAT_IATA;	// normally, track 1 use this
        public const int    MAGCFG_FORMAT_ABA			= SMART_MAG_FORMAT_ABA;		// normally, track 2 use this
        public const int    MAGCFG_FORMAT_MINS			= SMART_MAG_FORMAT_MINS;	// normally, track 3 use this. also known as TTS (Thrift Third Standard)
        public const int    MAGCFG_FORMAT_JIS2			= SMART_MAG_FORMAT_JIS2;	// normally, jis track use this.
        public const int    MAGCFG_FORMAT_BITS			= SMART_MAG_FORMAT_BITS;

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct MAGTRACKCONFIG
        {
	        public int		ballys_count;	// 0 ~ 2	: 0(not use ballys)
	        public int		backward;		// forward(normal) backward(reverse)
	        public int		format;			// data format
	        public int		bpi;			// write bpi (bits per inch)
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct MAGCFG
        {
	        public int		        ver;    // structure version (= MAGCFG_VER_1)
            public MAGTRACKCONFIG   t1;
            public MAGTRACKCONFIG   t2;
            public MAGTRACKCONFIG   t3;
            public MAGTRACKCONFIG   jis;
        }







        public const int    COUNTPARAM_VER_0		= 0;
        public const int    COUNTPARAM_VER_1		= 1;
        public const int    COUNTPARAM_VER_LATEST	= COUNTPARAM_VER_1;

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct COUNTPARAM
        {
	        public  int		ver;				// current version is 0.

	        // after ver:0
	        public  int		img;				// image issue count
	        public  int		mag;				// magnetic issue count
	        public  int		contact;			// contact smart card issue count
	        public  int		sim;				// SIM card issue count
	        public  int		contactless_ext;	// contactless smart card (external) issue count
	        public  int		contactless_int;	// contactless smart card (internal) issue count
	        public  int     cleanskip;			// cleaning skip count
	        public  int		cleaning;			// cleaning count

	        // after ver:1
	        public  int		fimg;				// factory image issue count
	        public  int		fmag;				// factory magnetic issue count
	        public  int		fcontact;			// factory contact smart card issue count
	        public  int		fsim;				// factory SIM card issue count
	        public  int		fcontactless_ext;	// factory contactless smart card (external) issue count
	        public  int		fcontactless_int;	// factory contactless smart card (internal) issue count
	        public  int		fpassedpanels;		// factory passed panels
	        public  int		fcleanskip;			// cleaning skip count
	        public  int		fcleaning;			// cleaning count
        }



        public const int    SYSINFO_VER_0			= 0;
        public const int    SYSINFO_VER_1			= 1;
        public const int    SYSINFO_VER_LATEST		= SYSINFO_VER_1;

        public const int    MAX_VER_LEN				= 32;
        public const int    MAX_SERIAL_LEN			= 24;

        public const int    SMART_CLEANWARN_OFF			= 0;
        public const int    SMART_CLEANWARN_NOTICE		= 1;
        public const int    SMART_CLEANWARN_PERMANENT	= 2;

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct SMART_SYSINFO
        {
	        public  int		ver;					// structure version (= SYSINFO_VER_1)

            // after ver:0
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_VER_LEN)]
            public String printer_ver;                  // printer f/w ver.
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_SERIAL_LEN)]
            public String printer_serial;               // printer serial number
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_SERIAL_LEN)]
            public String printer_hserial;               // thermal header serial number
	        // after ver:1
            public int		printer_cleaningWarning;    // cleaning-warning configuration value
		    public int		printer_cleaning;			// cleaning count
	        public int		printer_totalDensity;		// total density
	        public int		printer_colorDensity;		// YMC (color) density
	        public int		printer_blackDensity;		// K (black) density
	        public int		printer_overlayDensity;		// O (overlay) density
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public Byte[] printer_reserved;

	        public int      laminator_installed;        // flag for laminator installation
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_VER_LEN)]
            public String laminator_ver;                // laminator f/w ver.
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_SERIAL_LEN)]
            public String laminator_serial;             // laminator serial number
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public Byte[] laminator_reserved;
        }






        //
        // OEMDEV50, OEMDEV51
        //

        // Supply
        public const int   SMART_SUPPLY_AUTO			= 0;
        public const int   SMART_SUPPLY_HOPPER			= 1;


        // Tray
        public const int   SMART_TRAY_CR80				= 0;


        // Ribbon Type
        public const int   SMART_RIBBON_YMCKO			= 0;
        public const int   SMART_RIBBON_YMCKOK			= 1;
        public const int   SMART_RIBBON_hYMCKO			= 2;
        public const int   SMART_RIBBON_KO				= 3;
        public const int   SMART_RIBBON_K				= 4;
        public const int   SMART_RIBBON_BO				= 5;
        public const int   SMART_RIBBON_B				= 6;
        public const int   SMART_RIBBON_BYMCKO			= 7;
        public const int   SMART_RIBBON_YMCKFO			= 8;
        public const int   SMART_RIBBON_REWRITABLE		= 9;
        public const int   SMART_RIBBON_hYMCKOKO		= 11;
        public const int   SMART_RIBBON_YMCKOKR		= 12;


        // Ribbon - Resin
        public const int   SMART_RESIN_OBJECT			= 0;		// Extract Black Object to RESIN Panel
        public const int   SMART_RESIN_TEXT			= 1;		// Extract Black Text to RESIN Panel
        public const int   SMART_RESIN_DOT				= 2;		// Extract Black Dot(Pixel) to RESIN Panel
        public const int   SMART_RESIN_NOTUSE			= 3;		// No Extraction.
        public const int   SMART_RESIN_DYNAMIC			= 3;		// use DC's ExtEscape().
        public const int   SMART_RESIN_BLACKTEXT		= SMART_RESIN_TEXT;
        public const int   SMART_RESIN_BLACKDOT		= SMART_RESIN_DOT;


        // Ribbon Split
        public const int   SMART_RIBBONSPLIT_NORMAL	= 0;
        public const int   SMART_RIBBONSPLIT_FORBACK	= 1;


        // Flip Option
        public const int   SMART_FLIP_NORMAL			= 0;
        public const int   SMART_FLIP_VERTICAL			= 1;
        public const int   SMART_FLIP_HORIZONTAL		= 2;
        public const int   SMART_FLIP_ALL				= 3;


        // Quality : SMART-50/30
        public const int   SMART_QUALITY_STANDARD		= 0;
        public const int   SMART_QUALITY_FAST			= 1;
        public const int   SMART_QUALITY_PARTIAL		= 2;
        public const int   SMART_QUALITY_SEMIPARTIAL	= 3;
        public const int   SMART_QUALITY_PARTIAL2		= 3;
        public const int   SMART_QUALITY_HIGH			= 4;		// f/w ver 1.00.79 or higher
        public const int   SMART_QUALITY_HIGH_PARTIAL	= 5;		// f/w ver 1.00.79 or higher

        // Quality : SMART-51/31
        public const int   SMART51_SPEED_HIGHQUALITY	= 0;
        public const int   SMART51_SPEED_STANDARD		= 1;

        // Speed : SMART-51/31
        public const int   SMART51_QUALITY_STANDARD	= 0;
        public const int   SMART51_QUALITY_PARTIAL		= 1;


        // Color
        public const int   SMART_COLOR_COLOR			= 0;
        public const int   SMART_COLOR_BLACKNWHITE		= 1;


        // Dithering
        public const int   SMART_DITHER_THRESHOLD		= 0;
        public const int   SMART_DITHER_RANDOM			= 1;
        public const int   SMART_DITHER_DIFFUSION		= 2;
        public const int   SMART_DITHER_HALFTONE		= 3;		// SMART-51/31
        public const int   SMART_DITHER_HALFTONE2		= 4;		// SMART-51/31


        // Print Side
        public const int   SMART_PRINTSIDE_FRONT		= 0;	// Front Side Only
        public const int   SMART_PRINTSIDE_BOTH		= 1;	// Both of Front and Back Side
        public const int   SMART51_PRINTSIDE_FRONT		= SMART_PRINTSIDE_FRONT;    // SMART-51/31
        public const int   SMART51_PRINTSIDE_BOTH		= 2;    // SMART-51/31


        // Print - Media Mask
        public const int   SMART_MEDIA_STANDARD		= 0;
        public const int   SMART_MEDIA_SMART			= 1;	// Exclude IC Chip Area
        public const int   SMART_MEDIA_SMARTRIGHT		= 2;	// Exclude Left of IC Chip Area
        public const int   SMART_MEDIA_MSISO			= 3;	// Exclude MS ISO Track Area
        public const int   SMART_MEDIA_MSJIS			= 4;	// Exclude MS JIS Track Area
        public const int   SMART_MEDIA_SMARTMSJIS		= 5;	// Exclude MS JIS Track and IC Chip Area
        public const int   SMART_MEDIA_NOOVERLAY		= 6;	// No Overlay
        public const int   SMART_MEDIA_USERDEFINED		= 7;	// Use User Defined Bitmap
        public const int   SMART_MEDIA_DYNAMIC			= 8;	// use DC's ExtEscape().
        public const string SMART_MEDIA_BITMAP			= "cardprinter_mask_";	// $(SYSDIR)\cardprinter_mask_#.bmp (# : 0 ~ 6)


        // Laminate Side
        public const int   SMART_LAMINATESIDE_NONE		= 0;
        public const int   SMART_LAMINATESIDE_TOP		= 1;
        public const int   SMART_LAMINATESIDE_BOTTOM	= 2;
        public const int   SMART_LAMINATESIDE_BOTH		= 3;


        // Eject
        public const int   SMART_EJECT_EJECT_CARD		= 0;
        public const int   SMART_EJECT_HOLD_CARD		= 1;



        public const int    MAG_BUFFER					= 1024;

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct OEM_DMEXTRAHEADER
        {
            public int dwSize;
            public int dwSignature;
            public int dwVersion;
        }


        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct OEMDEV50
        {
            public OEM_DMEXTRAHEADER dmOEMExtra;

	        public int      dwCCMain;
            public int      dwCCYellow;
            public int      dwCCMagenta;
            public int      dwCCCyan;
            public int      dwCCBlack;
            public int      dwCCOverlay;
            public int      dwDocRibbonSplit;
            public int      dwDocFlip;
	        public int      dwBPText;
            public int      dwBPDot;
	        public int      dwBPThreshold;
	        public int      dwBPDitherDegree;
	        public int      dwEdgeSize;
	        public int      dwEjectCard;
	        public int      dwErase;
	        public int      dwDocHeatControl;
	        public int      dwMSTrack1;
	        public int      dwMSTrack2;
	        public int      dwMSTrack3;
	        public int      dwJISTrack;
	        public int      dwManualEncoding;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAG_BUFFER)]
            public String strMSTrack1;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAG_BUFFER)]
            public String strMSTrack2;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAG_BUFFER)]
            public String strMSTrack3;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAG_BUFFER)]
            public String strJISTrack;
	        public int      dwDocSupply;
	        public int      dwDocTray;
	        public int      dwDocRibbon;
	        public int      dwDocResin;
	        public int      dwDocQuality;
	        public int      dwDocColor;
	        public int      dwDocDither;
	        public int      dwDocPrintSide;
	        public int      dwDocMediaFront;
	        public int      dwDocMediaBack;
	        public int      dwDocLaminateSide;
	        public int      reserved;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 1024)]
            public String strDocMediaUserFront;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 1024)]
            public String strDocMediaUserBack;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct SMART50_DEVMODE
        {
            public Win32.DEVMODEW devmode;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 564)]
            public Byte[] reserved;
            public OEMDEV50 oemdev;
        }






        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct OEMDEV51
        {
            public OEM_DMEXTRAHEADER dmOEMExtra;

            public int      dwASMain;
            public int      dwASYellow;
            public int      dwASMagenta;
            public int      dwASCyan;
	        public int      dwASBlack;
            public int      dwASOverlay;
	        public int      dwASCorrectColor;           
	        public int      dwASCorrectMono;			
	        public int      dwASCorrectOverlay;			
	        public int      dwASBPText;
            public int      dwASBPDot;
	        public int      dwASBPThreshold;
	        public int      dwASBPDitherDegree;
	        public int      dwASBPResin;
	        public int      dwASBPResinOnly;
	        public int      dwASErase;
	        public int      dwASFastAlignment;			
	        public int      dwASWaitRFUse;
	        public int      dwASWaitRFSide;
	        public int      dwASWaitRFPos;
	        public int      dwASWaitRFTime;
	        public int      dwASWaitICUse;
	        public int      dwASWaitICSide;
	        public int      dwASWaitICPos;
	        public int      dwASWaitICTime;
	        public int      dwASResolution;     // 0:300x300(1012x636), 1:300x600(2024x636)
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 36)]
            public Byte[]   btAsReserved;
	        public int      dwIOSupply;
	        public int      dwIOTray;
	        public int      dwPrtUse;
	        public int      dwPrtSide;
	        public int      dwPrtColorFront;
	        public int      dwPrtColorBack;
	        public int      dwPrtFlipFront;				
	        public int      dwPrtFlipBack;				
	        public int      dwPrtMediaFront;
	        public int      dwPrtMediaBack;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 1024)]
            public String strPrtMediaUserFront;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 1024)]
            public String strPrtMediaUserBack;
	        public int      dwPrtRibbon;
	        public int      dwPrtSpeed;
	        public int      dwPrtQuality;
	        public int      dwPrtDither;
	        public int      dwPrtHeatControl;
	        public int      dwPrtRibbonSplit;
	        public int      dwPrtAntiAliasing;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 36)]
            public Byte[]   btPrtReserved;
	        public int      dwLamUse;
	        public int      dwLamSide;			
	        public int      dwLamOverlay;				
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 40)]
            public Byte[]   btLamReserved;
	        public int      dwEncUse;
	        public int      dwEncSide;
	        public int      dwEncCoer;
	        public int      dwEncMagRepeat;
	        public int      dwEncMagFlipBefore;
	        public int      dwEncMagFlipAfter;
	        public int      dwEncUseUserOption;
	        public int      dwEncUserHeadValue;
	        public int      dwEncFormat1;
	        public int      dwEncFormat2;
	        public int      dwEncFormat3;
	        public int      dwEncFormatJ;
	        public int      dwEncDensity1;
	        public int      dwEncDensity2;
	        public int      dwEncDensity3;
	        public int      dwEncDensityJ;
	        public int      dwEncOption1;
	        public int      dwEncOption2;
	        public int      dwEncOption3;
	        public int      dwEncOptionJ;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAG_BUFFER)]
            public String strEncTrack1;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAG_BUFFER)]
            public String strEncTrack2;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAG_BUFFER)]
            public String strEncTrack3;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAG_BUFFER)]
            public String strEncJISTrack;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 10)]
            public String strEncMSST1;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 10)]
            public String strEncMSST2;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 10)]
            public String strEncMSST3;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 10)]
            public String strEncMSSTJ;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 10)]
            public String strEncMSET1;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 10)]
            public String strEncMSET2;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 10)]
            public String strEncMSET3;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 10)]
            public String strEncMSETJ;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 40)]
            public Byte[]   btEncReserved;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 100)]
            public Byte[]   reserved;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct SMART51_DEVMODE
        {
            public Win32.DEVMODEW devmode;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 564)]
            public Byte[] reserved;
            public OEMDEV51 oemdev;
        }







        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct SMART_SURFACE_PROPERTIES
        {
	        public int      side;
	        public int      orientation;
	        public int      ribbon;
	        public int      ribbon_type;
	        public int      width;
	        public int      height;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct SMART_SURFACE_BITMAPS
        {
	        public IntPtr   hdcColor;       // HDC
            public IntPtr   hdcResin;       // HDC
            public IntPtr   hdcFluorescent;       // HDC
            public IntPtr   hdcOverlay;       // HDC
            public IntPtr   hBmpColor;       // HBITMAP
            public IntPtr   hBmpResin;       // HBITMAP
            public IntPtr   hBmpFluorescent;       // HBITMAP
            public IntPtr   hBmpOverlay;       // HBITMAP
            public IntPtr   lpPRN;              // BYTE *
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct SMART_SURFACE_OEMDEV
        {
	        public int      dwCCMain;
            public int      dwCCYellow;
            public int      dwCCMagenta;
            public int      dwCCCyan;
            public int      dwCCBlack;
            public int      dwCCOverlay;
            public int      dwDocRibbonSplit;
            public int      dwDocFlip;
	        public int      dwBPText;
            public int      dwBPDot;
	        public int      dwBPThreshold;
	        public int      dwBPDitherDegree;
	        public int      dwEdgeSize;
	        public int      dwEjectCard;
	        public int      dwErase;
	        public int      dwDocHeatControl;
	        public int      dwMSTrack1;
	        public int      dwMSTrack2;
	        public int      dwMSTrack3;
	        public int      dwJISTrack;
	        public int      dwManualEncoding;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAG_BUFFER)]
            public String strEncTrack1;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAG_BUFFER)]
            public String strEncTrack2;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAG_BUFFER)]
            public String strEncTrack3;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAG_BUFFER)]
            public String strJISTrack;
	        public int      dwDocSupply;
	        public int      dwDocTray;
	        public int      dwDocRibbon;
	        public int      dwDocResin;
	        public int      dwDocQuality;
	        public int      dwDocColor;
	        public int      dwDocDither;
	        public int      dwDocPrintSide;
	        public int      dwDocMediaFront;
	        public int      dwDocMediaBack;
	        public int      dwDocLaminateSide;
	        public int      reserved;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 1024)]
            public String strDocMediaUserFront;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 1024)]
            public String strDocMediaUserBack;
        }


        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct SMART_SURFACE
        {
	        public SMART_SURFACE_PROPERTIES	prop;
	        public SMART_SURFACE_BITMAPS	bmp;
	        public SMART_SURFACE_OEMDEV		oemdev;
        }





        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct SMART51_SURFACE_PROPERTIES
        {
	        public int      side;
	        public int      orientation;
	        public int      ribbon;
	        public int      ribbon_type;
	        public int      width;
	        public int      height;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct SMART51_SURFACE_BITMAPS
        {
            public IntPtr hdcColor;           // HDC
            public IntPtr hdcResin;           // HDC
            public IntPtr hdcFluorescent;     // HDC
            public IntPtr hdcOverlay;         // HDC
            public IntPtr hBmpColor;          // HBITMAP
            public IntPtr hBmpResin;          // HBITMAP
            public IntPtr hBmpFluorescent;    // HBITMAP
            public IntPtr hBmpOverlay;        // HBITMAP
            public IntPtr lpPRN;              // BYTE *
        }


        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct  SMART51_SURFACE_OEMDEV
        {
	        public int      dwASMain;
            public int      dwASYellow;
            public int      dwASMagenta;
            public int      dwASCyan;
	        public int      dwASBlack;
            public int      dwASOverlay;
	        public int      dwASCorrectColor;           
	        public int      dwASCorrectMono;			
	        public int      dwASCorrectOverlay;			
	        public int      dwASBPText;
            public int      dwASBPDot;
	        public int      dwASBPThreshold;
	        public int      dwASBPDitherDegree;
	        public int      dwASBPResin;
	        public int      dwASBPResinOnly;
	        public int      dwASErase;
	        public int      dwASFastAlignment;			
	        public int      dwASWaitRFUse;
	        public int      dwASWaitRFSide;
	        public int      dwASWaitRFPos;
	        public int      dwASWaitRFTime;
	        public int      dwASWaitICUse;
	        public int      dwASWaitICSide;
	        public int      dwASWaitICPos;
	        public int      dwASWaitICTime;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 40)]
            public Byte[]   btAsReserved;
	        public int      dwIOSupply;
	        public int      dwIOTray;
	        public int      dwPrtUse;
	        public int      dwPrtSide;
	        public int      dwPrtColorFront;
	        public int      dwPrtColorBack;
	        public int      dwPrtFlipFront;				
	        public int      dwPrtFlipBack;				
	        public int      dwPrtMediaFront;
	        public int      dwPrtMediaBack;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 1024)]
            public String strPrtMediaUserFront;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 1024)]
            public String strPrtMediaUserBack;
	        public int      dwPrtRibbon;
	        public int      dwPrtSpeed;
	        public int      dwPrtQuality;
	        public int      dwPrtDither;
	        public int      dwPrtHeatControl;
	        public int      dwPrtRibbonSplit;
	        public int      dwPrtAntiAliasing;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 36)]
            public Byte[]   btPrtReserved;
	        public int      dwLamUse;
	        public int      dwLamSide;			
	        public int      dwLamOverlay;				
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 40)]
            public Byte[]   btLamReserved;
	        public int      dwEncUse;
	        public int      dwEncSide;
	        public int      dwEncCoer;
	        public int      dwEncMagRepeat;
	        public int      dwEncMagFlipBefore;			
	        public int      dwEncMagFlipAfter;			
	        public int      dwEncUseUserOption;
	        public int      dwEncUserHeadValue;
	        public int      dwEncFormat1;
	        public int      dwEncFormat2;
	        public int      dwEncFormat3;
	        public int      dwEncFormatJ;
	        public int      dwEncDensity1;
	        public int      dwEncDensity2;
	        public int      dwEncDensity3;
	        public int      dwEncDensityJ;
	        public int      dwEncOption1;
	        public int      dwEncOption2;
	        public int      dwEncOption3;
	        public int      dwEncOptionJ;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAG_BUFFER)]
            public String strEncTrack1;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAG_BUFFER)]
            public String strEncTrack2;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAG_BUFFER)]
            public String strEncTrack3;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAG_BUFFER)]
            public String strEncJISTrack;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 10)]
            public String strEncMSST1;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 10)]
            public String strEncMSST2;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 10)]
            public String strEncMSST3;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 10)]
            public String strEncMSSTJ;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 10)]
            public String strEncMSET1;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 10)]
            public String strEncMSET2;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 10)]
            public String strEncMSET3;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 10)]
            public String strEncMSETJ;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 40)]
            public Byte[]   btEncReserved;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 100)]
            public Byte[]   reserved;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct  SMART51_SURFACE
        {
	        public SMART51_SURFACE_PROPERTIES	prop;
	        public SMART51_SURFACE_BITMAPS		bmp;
	        public SMART51_SURFACE_OEMDEV		oemdev;
        }



        // device information/list

        [DllImport("SmartComm2.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "SmartComm_GetDeviceList2")]
        public static extern uint GetDeviceList2(IntPtr pdevlist_ptr);    // SMART_PRINTER_LIST* pDevList

        [DllImport("SmartComm2.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "SmartCommEx_GetDeviceList2")]
        public static extern uint GetDeviceList2Ex(IntPtr pdevlist_ptr, int opt);    // SMART_PRINTER_LIST*, int

        [DllImport("SmartComm2.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "SmartComm_GetDeviceInfo2")]
        public static extern uint GetDeviceInfo2(IntPtr pdevinfo_ptr, IntPtr szdev_ptr, int ndevtype);    // SMART_PRINTER_INFO*, WCHAR*, int




        // device open/close

        [DllImport("SmartComm2.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "SmartComm_OpenDevice2")]
        public static extern uint OpenDevice2(ref IntPtr phsmart_ptr, IntPtr szdev_ptr, int ndevtype);    // HSMART*, WCHAR*, int

        [DllImport("SmartComm2.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "SmartCommEx_OpenDevice2")]
        public static extern uint OpenDevice2Ex(ref IntPtr phsmart_ptr, IntPtr szdev_ptr, int ndevtype);    // HSMART*, WCHAR*, int

        [DllImport("SmartComm2.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "SmartComm_CloseDevice")]
        public static extern uint CloseDevice(IntPtr hsmart_ptr);    // HSMART


        [DllImport("SmartComm2.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "SmartDCL_OpenDevice2")]
        public static extern uint DCLOpenDevice2(ref IntPtr phsmart_ptr, IntPtr szdev_ptr, int ndevtype, int norientation);    // HSMART*, WCHAR*, int, int

        [DllImport("SmartComm2.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "SmartDCLEx_OpenDevice2")]
        public static extern uint DCLOpenDevice2Ex(ref IntPtr phsmart_ptr, IntPtr szdev_ptr, int ndevtype, int norientation);    // HSMART*, WCHAR*, int, int

        [DllImport("SmartComm2.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "SmartDCL_OpenDevice3")]
        public static extern uint DCLOpenDevice3(ref IntPtr phsmart_ptr, IntPtr szdev_ptr, int port, int bSSL, int norientation);    // HSMART*, WCHAR*, int, BOOL, int

        [DllImport("SmartComm2.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "SmartDCL_CloseDevice")]
        public static extern uint DCLCloseDevice(IntPtr hsmart_ptr);    // HSMART




        // get information ...

        [DllImport("SmartComm2.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "SmartComm_GetStatus")]
        public static extern uint GetStatus(IntPtr hsmart_ptr, IntPtr pstatus_ptr);    // HSMART, __int64*

        [DllImport("SmartComm2.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "SmartLami_GetStatus")]
        public static extern uint GetLamiStatus(IntPtr hsmart_ptr, IntPtr pstatus_ptr, IntPtr pnbuflen_ptr);    // HSMART, BYTE*, int*

        [DllImport("SmartComm2.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "SmartFlip_GetStatus")]
        public static extern uint GetFlipStatus(IntPtr hsmart_ptr, IntPtr pstatus_ptr, IntPtr pnbuflen_ptr);    // HSMART, BYTE*, int*


        [DllImport("SmartComm2.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "SmartCommEx_GetVersion")]
        public static extern uint GetVersion(IntPtr hsmart_ptr, IntPtr szver_ptr, IntPtr pnbuflen_ptr);    // HSMART, WCHAR*, int*

        [DllImport("SmartComm2.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "SmartLami_GetVersion")]
        public static extern uint GetLamiVersion(IntPtr hsmart_ptr, IntPtr szbuf_ptr, IntPtr pnbuflen_ptr);    // HSMART, WCHAR*, int*


        [DllImport("SmartComm2.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "SmartCommEx_GetTemperature")]
        public static extern uint GetTemperature(IntPtr hsmart_ptr, IntPtr temperature_ptr, IntPtr ribcolor_ptr);    // HSMART, short*, short*


        [DllImport("SmartComm2.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "SmartComm_GetRibbonType")]
        public static extern uint GetRibbonType(IntPtr hsmart_ptr, IntPtr pntype_ptr);    // HSMART, int*

        [DllImport("SmartComm2.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "SmartComm_GetRibbonRemain")]
        public static extern uint GetRibbonRemain(IntPtr hsmart_ptr, IntPtr pnremain_ptr);    // HSMART, int*

        [DllImport("SmartComm2.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "SmartComm_GetRibbonInfo")]
        public static extern uint GetRibbonInfo(IntPtr hsmart_ptr, IntPtr pntype_ptr, IntPtr pnmax_ptr, IntPtr pnremain_ptr, IntPtr pngrade_ptr);    // HSMART, int*, int*, int*, int*

        [DllImport("SmartComm2.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "SmartComm_GetDisplayInfo")]
        public static extern uint GetDisplayInfo(IntPtr hsmart_ptr, IntPtr pntype_ptr, IntPtr pnlang_ptr);    // HSMART, int*, int*

        [DllImport("SmartComm2.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "SmartComm_GetSystemInfo")]
        public static extern uint GetSystemInfo(IntPtr hsmart_ptr, IntPtr psysinfo_ptr);    // HSMART, SMART_SYSINFO*

        [DllImport("SmartComm2.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "SmartComm_GetVRInfo")]
        public static extern uint GetVRInfo(IntPtr hsmart_ptr, int opt, IntPtr pnv_ptr, IntPtr pnr_ptr);    // HSMART, int, int*, int*

        [DllImport("SmartComm2.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "SmartCommEx_GetPanelDensity")]
        public static extern uint GetPanelDensity(IntPtr hsmart_ptr, byte panel, IntPtr pdensity_ptr);    // HSMART, BYTE, int*

        [DllImport("SmartComm2.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "SmartCommEx_SetPanelDensity")]
        public static extern uint SetPanelDensity(IntPtr hsmart_ptr, byte panel, int density, IntPtr pw_ptr);    // HSMART, BYTE, int, WCHAR*

        [DllImport("SmartComm2.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "SmartCommEx_GetSlowPrint")]
        public static extern uint GetSlowPrint(IntPtr hsmart_ptr, IntPtr pflag_ptr);    // HSMART, BOOL*

        [DllImport("SmartComm2.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "SmartCommEx_SetSlowPrint")]
        public static extern uint SetSlowPrint(IntPtr hsmart_ptr, int flag, IntPtr pw_ptr);    // HSMART, BOOL, WCHAR*

        [DllImport("SmartComm2.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "SmartComm_ReadUserMemory")]
        public static extern uint ReadUserMemory(IntPtr hsmart_ptr, IntPtr pw_ptr, int addr, IntPtr btdata_ptr);    // HSMART, WCHAR*, int, BYTE*

        [DllImport("SmartComm2.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "SmartComm_WriteUserMemory")]
        public static extern uint WriteUserMemory(IntPtr hsmart_ptr, IntPtr pw_ptr, int addr, IntPtr btdata_ptr);    // HSMART, WCHAR*, int, BYTE*

        
        
        


        // device control ...

        [DllImport("SmartComm2.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "SmartComm_SBSStart")]
        public static extern uint SBSStart(IntPtr hsmart_ptr);    // HSMART

        [DllImport("SmartComm2.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "SmartComm_SBSEnd")]
        public static extern uint SBSEnd(IntPtr hsmart_ptr);    // HSMART

        [DllImport("SmartComm2.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "SmartComm_CardIn")]
        public static extern uint CardIn(IntPtr hsmart_ptr);    // HSMART

        [DllImport("SmartComm2.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "SmartComm_CardInBack")]
        public static extern uint CardInBack(IntPtr hsmart_ptr);    // HSMART

        [DllImport("SmartComm2.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "SmartComm_CardOut")]
        public static extern uint CardOut(IntPtr hsmart_ptr);    // HSMART

        [DllImport("SmartComm2.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "SmartComm_CardOutBack")]
        public static extern uint CardOutBack(IntPtr hsmart_ptr);    // HSMART

        [DllImport("SmartComm2.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "SmartComm_CardOutBackAngle")]
        public static extern uint CardOutBackAngle(IntPtr hsmart_ptr, int angle);    // HSMART, int

        [DllImport("SmartComm2.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "SmartComm_Move")]
        public static extern uint Move(IntPtr hsmart_ptr, int pos);    // HSMART, int

        [DllImport("SmartComm2.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "SmartComm_MoveFromIn")]
        public static extern uint MoveFromIn(IntPtr hsmart_ptr, int dist);    // HSMART, int

        [DllImport("SmartComm2.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "SmartComm_MoveFromOut")]
        public static extern uint MoveFromOut(IntPtr hsmart_ptr, int dist);    // HSMART, int

        [DllImport("SmartComm2.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "SmartComm_MoveFromRotateIn")]
        public static extern uint MoveFromRotateIn(IntPtr hsmart_ptr, int dist);    // HSMART, int

        [DllImport("SmartComm2.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "SmartComm_MoveSensor")]
        public static extern uint MoveSensor(IntPtr hsmart_ptr, int sensor, int accel, int dist, int speed);    // HSMART, int, int, int, int

        [DllImport("SmartComm2.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "SmartComm_MovingScan")]
        public static extern uint MovingScan(IntPtr hsmart_ptr, short dist, short speed, short speed2);    // HSMART, WORD, WORD, WORD


        [DllImport("SmartComm2.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "SmartComm_DoPrint")]
        public static extern uint DoPrint(IntPtr hsmart_ptr);    // HSMART

        [DllImport("SmartComm2.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "SmartComm_ICHContact")]
        public static extern uint ICHContact(IntPtr hsmart_ptr);    // HSMART

        [DllImport("SmartComm2.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "SmartComm_ICHDiscontact")]
        public static extern uint ICHDiscontact(IntPtr hsmart_ptr);    // HSMART

        [DllImport("SmartComm2.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "SmartComm_Rotate")]
        public static extern uint Flip(IntPtr hsmart_ptr);    // HSMART

        [DllImport("SmartComm2.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "SmartComm_DoCleaning")]
        public static extern uint DoCleaning(IntPtr hsmart_ptr);    // HSMART

        [DllImport("SmartComm2.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "SmartComm_ClearStatus")]
        public static extern uint ClearStatus(IntPtr hsmart_ptr);    // HSMART

        [DllImport("SmartComm2.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "SmartComm_Reboot")]
        public static extern uint Reboot(IntPtr hsmart_ptr);    // HSMART

        [DllImport("SmartComm2.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "SmartComm_SetLCDText")]
        public static extern uint SetLCDText(IntPtr hsmart_ptr, int type, int line, IntPtr sztext_ptr);    // HSMART, int, int, WCHAR*


        [DllImport("SmartComm2.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "SmartCommEx_LockPrinter2")]
        public static extern uint LockPrinter2(IntPtr hsmart_ptr, IntPtr szpw_ptr);    // HSMART, WCHAR*

        [DllImport("SmartComm2.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "SmartCommEx_UnlockPrinter2")]
        public static extern uint UnlockPrinter2(IntPtr hsmart_ptr, IntPtr szpw_ptr);    // HSMART, WCHAR*

        [DllImport("SmartComm2.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "SmartCommEx_UnlockPrinter3")]
        public static extern uint UnlockPrinter3(IntPtr hsmart_ptr, IntPtr szpw_ptr, int unlocktime);    // HSMART, WCHAR*, int


        [DllImport("SmartComm2.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "SmartComm_MagConfig")]
        public static extern uint MagConfig(IntPtr hsmart_ptr, IntPtr pmagcfg_ptr);    // HSMART, MAGCFG*

        [DllImport("SmartComm2.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "SmartComm_MagReadAction")]
        public static extern uint MagReadAction(IntPtr hsmart_ptr, int track);    // HSMART, int

        [DllImport("SmartComm2.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "SmartComm_MagReadAction2")]
        public static extern uint MagReadAction2(IntPtr hsmart_ptr, int track, int opt);    // HSMART, int, int

        [DllImport("SmartComm2.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "SmartComm_MagGetBuffer")]
        public static extern uint MagGetBuffer(IntPtr hsmart_ptr, int track, IntPtr poutbuf_ptr, IntPtr pninoutlen_ptr);    // HSMART, int, BYTE*, int*

        [DllImport("SmartComm2.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "SmartComm_MagGetAllBufferEx")]
        public static extern uint MagGetAllBuffer(IntPtr hsmart_ptr, int bT1, IntPtr pbufT1_ptr, IntPtr pnlenT1_ptr, int bT2, IntPtr pbufT2_ptr, IntPtr pnlenT2_ptr, int bT3, IntPtr pbufT3_ptr, IntPtr pnlenT3_ptr, int bTJ, IntPtr pbufTJ_ptr, IntPtr pnlenTJ_ptr);    // HSMART, BOOL, BYTE*, int*, BOOL, BYTE*, int*, BOOL, BYTE*, int*, BOOL, BYTE*, int*

        [DllImport("SmartComm2.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "SmartComm_MagGetAllCryptoBufferEx")]
        public static extern uint MagGetAllCryptoBuffer(IntPtr hsmart_ptr, int bT1, IntPtr pbufT1_ptr, IntPtr pnlenT1_ptr, int bT2, IntPtr pbufT2_ptr, IntPtr pnlenT2_ptr, int bT3, IntPtr pbufT3_ptr, IntPtr pnlenT3_ptr, int bTJ, IntPtr pbufTJ_ptr, IntPtr pnlenTJ_ptr, IntPtr pkey_ptr);    // HSMART, BOOL, BYTE*, int*, BOOL, BYTE*, int*, BOOL, BYTE*, int*, BOOL, BYTE*, int*, BYTE*


        [DllImport("SmartComm2.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "SmartComm_MagWriteAction")]
        public static extern uint MagWriteAction(IntPtr hsmart_ptr, int track, int bhighco);    // HSMART, int, BOOL

        [DllImport("SmartComm2.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "SmartComm_MagWriteAction2")]
        public static extern uint MagWriteAction2(IntPtr hsmart_ptr, int track, int opt, int nBPIT1, int nBPIT2, int nBPIT3);    // HSMART, int, int nOpt, int, int, int

        [DllImport("SmartComm2.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "SmartComm_MagSetBuffer")]
        public static extern uint MagSetBuffer(IntPtr hsmart_ptr, int track, IntPtr pbuf_ptr, int nlen);    // HSMART, int, BYTE*, int

        [DllImport("SmartComm2.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "SmartComm_MagSetAllBufferEx")]
        public static extern uint MagSetAllBuffer(IntPtr hsmart_ptr, int bT1, IntPtr pbufT1, int nlenT1, int bT2, IntPtr pbufT2, int nlenT2, int bT3, IntPtr pbufT3, int nlenT3, int bTJ, IntPtr pbufTJ, int nlenTJ);    // HSMART, BOOL, BYTE*, int, BOOL, BYTE*, int, BOOL, BYTE*, int, BOOL, BYTE*, int

        [DllImport("SmartComm2.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "SmartComm_MagBitModeGetBPI")]
        public static extern uint MagBitModeGetBPI(int BPI, IntPtr pnewBPI_ptr);    // int, int*

        [DllImport("SmartComm2.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "SmartComm_MagBitModeGetBPI2")]
        public static extern uint MagBitModeGetBPI2(IntPtr hsmart_ptr, int BPI, IntPtr pnewBPI_ptr);    // HSMART, int, int*

        [DllImport("SmartComm2.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "SmartComm_MagBitModeGetMaxSize")]
        public static extern uint MagBitModeGetMaxSize(int BPI, IntPtr pnmaxsize_ptr);    // int, int*

        [DllImport("SmartComm2.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "SmartComm_MagBitModeGetMaxSize2")]
        public static extern uint MagBitModeGetMaxSize2(IntPtr hsmart_ptr, int BPI, IntPtr pnmaxsize_ptr);    // HSMART, int, int*

        [DllImport("SmartComm2.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "SmartComm_MagGetCryptoBuffer")]
        public static extern uint MagGetCryptoBuffer(IntPtr hsmart_ptr, int track, IntPtr pbuf_ptr, IntPtr pninoutlen_ptr, IntPtr pkey_ptr);    // HSMART, int, BYTE*, int*, BYTE*

        [DllImport("SmartComm2.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "SmartComm_MagGetAllCryptoBuffer")]
        public static extern uint MagGetAllCryptoBuffer(IntPtr hsmart_ptr, int bT1, IntPtr pbufT1_ptr, IntPtr pnlenT1_ptr, int bT2, IntPtr pbufT2_ptr, IntPtr pnlenT2_ptr, int bT3, IntPtr pbufT3_ptr, IntPtr pnlenT3_ptr, IntPtr pkey_ptr);    // HSMART, BOOL, BYTE*, int*, BOOL, BYTE*, int*, BOOL, BYTE*, int*, BYTE*

        [DllImport("SmartComm2.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "SmartComm_MagSetCryptoBuffer")]
        public static extern uint MagSetCryptoBuffer(IntPtr hsmart_ptr, int track, IntPtr pbuf_ptr, int nlen, IntPtr pkey_ptr);    // HSMART, int, BYTE*, int, BYTE*

        [DllImport("SmartComm2.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "SmartComm_MagSetAllCryptoBufferEx")]
        public static extern uint MagSetAllCryptoBuffer(IntPtr hsmart_ptr, int bT1, IntPtr pbufT1_ptr, int nlenT1, int bT2, IntPtr pbufT2_ptr, int nlenT2, int bT3, IntPtr pbufT3_ptr, int nlenT3, int bTJ, IntPtr pbufTJ_ptr, int nlenTJ, IntPtr pkey_ptr);    // HSMART, BOOL, BYTE*, int, BOOL, BYTE*, int, BOOL, BYTE*, int, BOOL, BYTE*, int, BYTE*


        [DllImport("SmartComm2.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "SmartCommEx_CmdRecv")]
        public static extern uint CmdRecv(IntPtr hsmart_ptr, IntPtr pcmd_ptr, int ncmdlen, IntPtr prcvbuf_ptr, IntPtr pninoutrcvlen_ptr);    // HSMART, BYTE*, int, BYTE*, int*

        [DllImport("SmartComm2.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "SmartCommEx_CmdSend")]
        public static extern uint CmdSend(IntPtr hsmart_ptr, IntPtr pcmd_ptr, int ncmdlen);    // HSMART, BYTE*, int





        // document/drawing ...

        [DllImport("SmartComm2.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "SmartComm_OpenDocument")]
        public static extern uint OpenDocument(IntPtr hsmart_ptr, IntPtr szcsd_ptr);    // HSMART, WCHAR*

        [DllImport("SmartComm2.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "SmartComm_CloseDocument")]
        public static extern uint CloseDocument(IntPtr hsmart_ptr);    // HSMART

        [DllImport("SmartComm2.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "SmartComm_GetFieldCount")]
        public static extern uint GetFieldCount(IntPtr hsmart_ptr, IntPtr pncount_ptr);    // HSMART, int*

        [DllImport("SmartComm2.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "SmartComm_GetFieldName")]
        public static extern uint GetFieldName(IntPtr hsmart_ptr, int idx, IntPtr szname_ptr, int nbuflen);    // HSMART, int, WCHAR*, DWORD

        [DllImport("SmartComm2.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "SmartComm_GetFieldValue")]
        public static extern uint GetFieldValue(IntPtr hsmart_ptr, IntPtr szname_ptr, IntPtr szvalue_ptr);    // HSMART, WCHAR*, WCHAR*

        [DllImport("SmartComm2.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "SmartComm_SetFieldValue")]
        public static extern uint SetFieldValue(IntPtr hsmart_ptr, IntPtr szname_ptr, IntPtr szvalue_ptr);    // HSMART, WCHAR*, WCHAR*

        [DllImport("SmartComm2.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "SmartComm_GetPrinterSettings")]
        public static extern uint GetPrinterSettings(IntPtr hsmart_ptr, IntPtr pdm_ptr);    // HSMART, SMART50_DEVMODE*

        [DllImport("SmartComm2.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "SmartComm_SetPrinterSettings")]
        public static extern uint SetPrinterSettings(IntPtr hsmart_ptr, IntPtr pdm_ptr);    // HSMART, SMART50_DEVMODE*

        [DllImport("SmartComm2.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "SmartComm_GetPrinterSettings2")]
        public static extern uint GetPrinterSettings2(IntPtr hsmart_ptr, IntPtr pdm_ptr, IntPtr pdmlen_ptr);    // HSMART, void*, int*

        [DllImport("SmartComm2.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "SmartComm_SetPrinterSettings2")]
        public static extern uint SetPrinterSettings2(IntPtr hsmart_ptr, IntPtr pdm_ptr, int dmlen);    // HSMART, void*, int

        [DllImport("SmartComm2.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "SmartComm_DrawLine")]
        public static extern uint DrawLine(IntPtr hsmart_ptr, byte page, byte panel, int x1, int y1, int x2, int y2, int thick, int style, int color, IntPtr prcdraw_ptr);    // HSMART, BYTE, BYTE, int, int, int, int, int, int, COLORREF, RECT*

        [DllImport("SmartComm2.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "SmartComm_DrawRect")]
        public static extern uint DrawRect(IntPtr hsmart_ptr, byte page, byte panel, int x, int y, int cx, int cy, int color, IntPtr prcdraw_ptr);    // HSMART, BYTE, BYTE, int, int, int, int, COLORREF, RECT*

        [DllImport("SmartComm2.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "SmartComm_DrawText")]
        public static extern uint DrawText(IntPtr hsmart_ptr, byte page, byte panel, int x, int y, IntPtr szfontname_ptr, int fontsize, byte fontstyle, IntPtr sztext_ptr, IntPtr prcdraw_ptr);    // HSMART, BYTE, BYTE, int, int, WCHAR*, int, BYTE, WCHAR*, RECT*

        [DllImport("SmartComm2.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "SmartComm_DrawText2")]
        public static extern uint DrawText2(IntPtr hsmart_ptr, byte page, byte panel, IntPtr pdt2info_ptr, IntPtr sztext_ptr);    // HSMART, BYTE, BYTE, DRAWTEXT2INFO*, WCHAR*

        [DllImport("SmartComm2.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "SmartComm_DrawImage")]
        public static extern uint DrawImage(IntPtr hsmart_ptr, byte page, byte panel, int x, int y, int cx, int cy, IntPtr szimgpath_ptr, IntPtr prcdraw_ptr);    // HSMART, BYTE, BYTE, int, int, int, int, WCHAR*, RECT*

        [DllImport("SmartComm2.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "SmartCommEx_DrawImage")]
        public static extern uint DrawImage2(IntPtr hsmart_ptr, byte page, byte panel, int x, int y, int cx, int cy, int scale, byte align, IntPtr szimgpath_ptr, IntPtr prcdraw_ptr);    // HSMART, BYTE, BYTE, int, int, int, int, int, BYTE, WCHAR*, RECT*

        [DllImport("SmartComm2.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "SmartComm_DrawBitmap")]
        public static extern uint DrawBitmap(IntPtr hsmart_ptr, byte page, byte panel, int x, int y, int cx, int cy, int scale, byte align, IntPtr hbmp_ptr, IntPtr prcdraw_ptr);    // HSMART, BYTE, BYTE, int, int, int, int, int, BYTE, HBITMAP, RECT*

        [DllImport("SmartComm2.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "SmartCommEx_DrawBitmap")]
        public static extern uint DrawBitmap2(IntPtr hsmart_ptr, byte page, byte panel, int x, int y, int cx, int cy, int scale, byte align, IntPtr hbmp_ptr, IntPtr prcdraw_ptr);    // HSMART, BYTE, BYTE, int, int, int, int, int, BYTE, HBITMAP, RECT*

        [DllImport("SmartComm2.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "SmartComm_DrawBarcode")]
        public static extern uint DrawBarcode(IntPtr hsmart_ptr, byte page, byte panel, int x, int y, int cx, int cy, int col, IntPtr prcdraw_ptr, IntPtr szname_ptr, int size, IntPtr szdata_ptr, IntPtr szpost_ptr);    // HSMART, BYTE, BYTE, int, int, int, int, COLORREF, RECT*, WCHAR*, int, WCHAR*, WCHAR*

        [DllImport("SmartComm2.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "SmartComm_GetBarcodeTypeCount")]
        public static extern uint GetBarcodeTypeCount(IntPtr hsmart_ptr, IntPtr pncount_ptr);    // HSMART, int*

        [DllImport("SmartComm2.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "SmartComm_GetBarcodeTypeName")]
        public static extern uint GetBarcodeTypeName(IntPtr hsmart_ptr, int idx, IntPtr szname_ptr, int nbuflen);    // HSMART, int, WCHAR*, int

        [DllImport("SmartComm2.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "SmartComm_GetPreviewBitmap")]
        public static extern uint GetPreviewBitmap(IntPtr hsmart_ptr, byte page, ref IntPtr ppbi_ptr);    // HSMART, BYTE, BITMAPINFO**

        [DllImport("SmartComm2.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "SmartComm_Print")]
        public static extern uint Print(IntPtr hsmart_ptr);    // HSMART

        [DllImport("SmartComm2.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "SmartDCL_Print")]
        public static extern uint DCLPrint(IntPtr hsmart_ptr, int printside);    // HSMART, int

        [DllImport("SmartComm2.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "SmartDCL_GetSurface")]
        public static extern uint GetSurface(IntPtr hsmart_ptr, ref IntPtr ppfrontsurf_ptr, ref IntPtr ppbacksurf_ptr);    // HSMART, SMART_SURFACE **, SMART_SURFACE **

        [DllImport("SmartComm2.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "SmartDCL_GetSurface2")]
        public static extern uint GetSurface2(IntPtr hsmart_ptr, int page, ref IntPtr ppsurf_ptr, IntPtr plen_ptr);    // HSMART, int, void**, int*


        [DllImport("SmartComm2.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "SmartComm_GetUnitInfo")]
        public static extern uint GetUnitInfo(IntPtr hsmart_ptr, IntPtr punit_ptr, int dir);    // HSMART, UNITINFO*, int

        [DllImport("SmartComm2.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "SmartComm_GetFieldLinkedUnitInfo")]
        public static extern uint GetFieldLinkedUnitInfo(IntPtr hsmart_ptr, IntPtr szfield_ptr, IntPtr punit_ptr, int dir);    // HSMART, WCHAR*, UNITINFO*, int

        [DllImport("SmartComm2.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "SmartComm_SetUnitInfo")]
        public static extern uint SetUnitInfo(IntPtr hsmart_ptr, IntPtr punit_ptr);    // HSMART, UNITINFO*

        [DllImport("SmartComm2.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "SmartComm_GetUnitInfo2")]
        public static extern uint GetUnitInfo2(IntPtr hsmart_ptr, IntPtr punit_ptr, int dir);    // HSMART, UNITINFO2*, int

        [DllImport("SmartComm2.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "SmartComm_GetFieldLinkedUnitInfo2")]
        public static extern uint GetFieldLinkedUnitInfo2(IntPtr hsmart_ptr, IntPtr szfield_ptr, IntPtr punit_ptr, int dir);    // HSMART, WCHAR*, UNITINFO2*, int

        [DllImport("SmartComm2.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "SmartComm_SetUnitInfo2")]
        public static extern uint SetUnitInfo2(IntPtr hsmart_ptr, IntPtr punit_ptr);    // HSMART, UNITINFO2*

        [DllImport("SmartComm2.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "SmartComm_GetUnitInfo2Direct")]
        public static extern uint GetUnitInfo2Direct(IntPtr hsmart_ptr, IntPtr punit_ptr);    // HSMART, UNITINFO2*


        [DllImport("SmartComm2.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "SmartCommEx_GetObjectInfo2")]
        public static extern uint GetObjectInfo2(int idx, ref IntPtr pinfo2_ptr, IntPtr ver_ptr);   // HSMART, DWORD, OBJ_INFO2**, int*

        [DllImport("SmartComm2.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "SmartCommEx_ReleaseObjectInfo2")]
        public static extern uint ReleaseObjectInfo2(IntPtr pinfo2_ptr);   // HSMART, OBJ_INFO2*

        [DllImport("SmartComm2.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "SmartCommEx_UpdateObject2")]
        public static extern uint UpdateObject2(IntPtr pinfo2_ptr);        // HSMART, OBJ_INFO2*

        [DllImport("SmartComm2.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "SmartCommEx_GetFirstObjectInfo2")]
        public static extern uint GetFirstObjectInfo2(byte page, byte panel, ref IntPtr pinfo2_ptr);      // HSMART, BYTE, BYTE, OBJ_INFO2**

        [DllImport("SmartComm2.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "SmartCommEx_GetNextObjectInfo2")]
        public static extern uint GetNextObjectInfo2(byte page, byte panel, ref IntPtr pinfo2_ptr);       // HSMART, BYTE, BYTE, OBJ_INFO2**



        [DllImport("SmartComm2.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "SmartComm_GetICG")]
        public static extern uint GetICG(IntPtr hsmart_ptr, int page, IntPtr pnicg_ptr);    // HSMART, int, int*

        [DllImport("SmartComm2.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "SmartComm_SetICG")]
        public static extern uint SetICG(IntPtr hsmart_ptr, int page, int icg);    // HSMART, int, int

        [DllImport("SmartComm2.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "SmartComm_IsBackEnable")]
        public static extern uint IsBackEnabled(IntPtr hsmart_ptr, IntPtr pbenabled_ptr);    // HSMART, BOOL*







        // Contact & Contactless

        [DllImport("SmartComm2.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "SmartComm_RFPowerOn")]
        public static extern uint RFPowerOn(IntPtr hsmart_ptr, int dev, IntPtr cardtype_ptr, IntPtr poutlen_ptr, IntPtr poutbuf_ptr);    // HSMART, int, int*, DWORD*, BYTE*

        [DllImport("SmartComm2.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "SmartComm_RFPowerOff")]
        public static extern uint RFPowerOff(IntPtr hsmart_ptr, int dev);    // HSMART, int

        [DllImport("SmartComm2.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "SmartComm_RFTransmit")]
        public static extern uint RFTransmit(IntPtr hsmart_ptr, int dev, int inlen, IntPtr inbuf_ptr, IntPtr outlen_ptr, IntPtr outbuf_ptr);    // HSMART, int, DWORD, BYTE*, DWORD*, BYTE*

        [DllImport("SmartComm2.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "SmartCommEx_RFPCSC_GetReaderName")]
        public static extern uint GetRFReaderName(IntPtr hsmart_ptr, int which, IntPtr szname_ptr);    // HSMART, int, WCHAR*


        [DllImport("SmartComm2.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "SmartComm_ICPowerOn")]
        public static extern uint ICPowerOn(IntPtr hsmart_ptr, int dev, IntPtr pnoutlen_ptr, IntPtr poutbuf_ptr);    // HSMART, int, DWORD*, BYTE*

        [DllImport("SmartComm2.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "SmartComm_ICPowerOff")]
        public static extern uint ICPowerOff(IntPtr hsmart_ptr, int dev);    // HSMART, int

        [DllImport("SmartComm2.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "SmartComm_ICTransmit")]
        public static extern uint ICTransmit(IntPtr hsmart_ptr, int dev, int inlen, IntPtr inbuf_ptr, IntPtr outlen_ptr, IntPtr outbuf_ptr);    // HSMART, int, DWORD, BYTE*, DWORD*, BYTE*

        [DllImport("SmartComm2.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "SmartCommEx_IC_GetICReaderInfo")]
        public static extern uint GetICReaderInfo(IntPtr hsmart_ptr, IntPtr pnidx_ptr, IntPtr szname_ptr);    // HSMART, int*, WCHAR*

        [DllImport("SmartComm2.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "SmartCommEx_IC_GetSIMReaderInfo")]
        public static extern uint GetSIMReaderInfo(IntPtr hsmart_ptr, IntPtr pnidx_ptr, IntPtr szname_ptr);    // HSMART, int*, WCHAR*







        // dll management

        [DllImport("SmartComm2.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "SmartCommEx_GetConfig")]
        public static extern uint GetConfig(IntPtr hsmart_ptr, int id, IntPtr pnvalue_ptr);    // HSMART, int, int*

        [DllImport("SmartComm2.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "SmartCommEx_SetConfig")]
        public static extern uint SetConfig(IntPtr hsmart_ptr, int id, int value);    // HSMART, int, int







        // serial & kiosk

        [DllImport("SmartComm2.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "SmartCommEx_SerialCmdRecv")]
        public static extern uint SerialCmdRecv(IntPtr hsmart_ptr, IntPtr prcvbuf_ptr, IntPtr pninoutrcvlen_ptr);    // HSMART, BYTE*, int*

        [DllImport("SmartComm2.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "SmartCommEx_SerialCmdSend")]
        public static extern uint SerialCmdSend(IntPtr hsmart_ptr, IntPtr pcmd_ptr, int ncmdlen);    // HSMART, BYTE*, int


        [DllImport("SmartComm2.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "SmartKiosk_CardIn")]
        public static extern uint KioskCardIn(IntPtr hsmart_ptr, IntPtr pcmd_ptr, int ncmdlen, IntPtr btrcvbuf_len, IntPtr pninoutrcvlen_ptr);    // HSMART, BYTE*, int, BYTE*, int*

        [DllImport("SmartComm2.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "SmartKiosk_CardIn2")]
        public static extern uint KioskCardIn2(IntPtr hsmart_ptr, int hopper, int opt);    // HSMART, int, int

        [DllImport("SmartComm2.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "SmartKiosk_Hopper")]
        public static extern uint KioskHopper(IntPtr hsmart_ptr, int hopper, int opt, IntPtr pvalue_ptr);    // HSMART, int, int, int*







        // ethernet module
        public const int    SMART_CONFIG_ON		        = 1;
        public const int    SMART_CONFIG_OFF	        = 0;

        public const int    SMART_SERVICE_LOOKUP_PORT	= 11119;
        public const int    SMART_SERVICE_ADMIN_PORT	= 11111;
        public const int    SMART_SERVICE_ADMIN_TIMEOUT	= 120000;

        public const int    SMART_LOOKUP_TYPE_SOFTWARE	= 0;
        public const int    SMART_LOOKUP_TYPE_HARDWARE	= 1;

        public const int    SMART_LOOKUP_CMD_GETCFG		= 0;
        public const int    SMART_LOOKUP_CMD_GETSDK		= 1;

        // smart system configuration for ver 1.
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct  SMART_SYSTEM_CONFIG_VER1
        {
	        public int		b_on_dhcp;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public Byte[]   ip;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public Byte[]   mask;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public Byte[]   gw;
        }

        // smart system configuration for ver 2.
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct  SMART_SYSTEM_CONFIG_VER2
        {
	        public  int		b_on_dhcp;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public Byte[]   ip;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public Byte[]   mask;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public Byte[]   gw;
	        //wifi
	        public  int		b_wifi;
	        public  int		b_wifi_dhcp;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public Byte[]   wifi_essid;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public Byte[]   wifi_key;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public Byte[]   wifi_ip;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public Byte[]   wifi_mask;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public Byte[]   wifi_gw;
	        // reserved
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public int[]    reserved;
        }


        // smart service configuration for ver 1
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct  SMART_SERVICE_CONFIG_VER1
        {
	        // Enable/Disable service
	        public int      b_on_usb_spool;
	        public int      b_on_net_spool;
	        public int      b_on_net_sdk;
	        // Network Spool Configuration
	        public int      net_spool_port;
	        public int      net_spool_timeout;	// mSec
	        // Network SDK Configuration
	        public int      net_sdk_b_ssl;
	        public int      net_sdk_b_user_auth;
	        public int      net_sdk_b_cardout;
	        public int      net_sdk_port;
	        public int      net_sdk_timeout;	// mSec
	        // etc
	        public int      log_level;
        }

        // smart service configuration for ver 2
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct  SMART_SERVICE_CONFIG_VER2
        {
	        // Enable/Disable service
	        public int      b_on_usb_spool;
	        public int      b_on_net_spool;
	        public int      b_on_net_sdk;
	        // Network Spool Configuration
	        public int      net_spool_port;
	        public int      net_spool_timeout;	// mSec
	        // Network SDK Configuration
	        public int      net_sdk_b_ssl;
	        public int      net_sdk_b_user_auth;
	        public int      net_sdk_b_cardout;
	        public int      net_sdk_port;
	        public int      net_sdk_timeout;	// mSec
	        // etc
	        public int      log_level;
	        public int      b_on_flashusb_monitor;
	        // reserved
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public int[]    reserved;
        }



        // ocp config for ver 1
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct  SMART_OCP_CONFIG_VER1
        {
	        // Enable/Disable service
	        public int	    b_on_net_ocp;
	        // Network SDK Configuration
	        public int	    net_ocp_b_terminal;
	        public int	    net_ocp_b_ssl;
	        public int	    net_ocp_b_user_auth;
	        public int	    net_ocp_b_cardout;
	        public int	    net_ocp_port;
	        public int	    net_ocp_timeout;	// mSec
        }

        // ocp config for ver 2
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct  SMART_OCP_CONFIG_VER2
        {
	        // Enable/Disable service
	        public int	    b_on_net_ocp;
	        // Network SDK Configuration
	        public int	    net_ocp_b_terminal;
	        public int	    net_ocp_b_ssl;
	        public int	    net_ocp_b_user_auth;
	        public int	    net_ocp_b_cardout;
	        public int	    net_ocp_port;
	        public int	    net_ocp_timeout;	// mSec
	        public int	    b_on_net_lpd;
	        public int	    net_lpd_port;
	        public int	    net_lpd_timeout;
        }

        // ocp config for ver 3
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct  SMART_OCP_CONFIG_VER3
        {
	        // Enable/Disable service
	        public int	    b_on_net_ocp;
	        // Network SDK Configuration
	        public int	    net_ocp_b_terminal;
	        public int	    net_ocp_b_ssl;
	        public int	    net_ocp_b_user_auth;
	        public int	    net_ocp_b_cardout;
	        public int	    net_ocp_port;
	        public int	    net_ocp_timeout;	// mSec
	        public int	    b_on_net_lpd;
	        public int	    net_lpd_port;
	        public int	    net_lpd_timeout;
	        // reserved
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public int[]    reserved;
        }



        // smart user configuration
        public const int    MAX_SMART_USER		    = 256;

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct  SMART_USER
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
            public Byte[]    id;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 40)]
            public Byte[]    pw;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct  SMART_USER_LIST
        {
	        public int	    n;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MAX_SMART_USER)]
	        public SMART_USER[]   user;
        }



        [DllImport("SmartComm2.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "SmartNet_Reboot")]
        public static extern uint NetReboot(IntPtr hsmart_ptr, int devtype, IntPtr szadminid_ptr, IntPtr szadminpw_ptr);    // HSMART, int, WCHAR*, WCHAR*

        [DllImport("SmartComm2.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "SmartNet_Reload")]
        public static extern uint NetReload(IntPtr hsmart_ptr, int devtype, IntPtr szadminid_ptr, IntPtr szadminpw_ptr);    // HSMART, int, WCHAR*, WCHAR*

        [DllImport("SmartComm2.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "SmartNet_Reset")]
        public static extern uint NetReset(IntPtr hsmart_ptr, int devtype, IntPtr szadminid_ptr, IntPtr szadminpw_ptr);    // HSMART, int, WCHAR*, WCHAR*

        [DllImport("SmartComm2.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "SmartNet_Upgrade")]
        public static extern uint NetUpgrade(IntPtr hsmart_ptr, int devtype, IntPtr szadminid_ptr, IntPtr szadminpw_ptr, IntPtr szfile_ptr);    // HSMART, int, WCHAR*, WCHAR*, WCHAR*

        [DllImport("SmartComm2.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "SmartNet_GetSysConfig")]
        public static extern uint NetGetSysConfig(IntPtr hsmart_ptr, int devtype, IntPtr szadminid_ptr, IntPtr szadminpw_ptr, IntPtr psystem2_ptr);    // HSMART, int, WCHAR*, WCHAR*, SMART_SYSTEM_CONFIG_VER2*

        [DllImport("SmartComm2.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "SmartNet_SetSysConfig")]
        public static extern uint NetSetSysConfig(IntPtr hsmart_ptr, int devtype, IntPtr szadminid_ptr, IntPtr szadminpw_ptr, IntPtr psystem2_ptr);    // HSMART, int, WCHAR*, WCHAR*, SMART_SYSTEM_CONFIG_VER2*

        [DllImport("SmartComm2.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "SmartNet_GetSvcConfig")]
        public static extern uint NetGetSvcConfig(IntPtr hsmart_ptr, int devtype, IntPtr szadminid_ptr, IntPtr szadminpw_ptr, IntPtr pservice2_ptr);    // HSMART, int, WCHAR*, WCHAR*, SMART_SERVICE_CONFIG_VER2*

        [DllImport("SmartComm2.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "SmartNet_SetSvcConfig")]
        public static extern uint NetSetSvcConfig(IntPtr hsmart_ptr, int devtype, IntPtr szadminid_ptr, IntPtr szadminpw_ptr, IntPtr pservice2_ptr);    // HSMART, int, WCHAR*, WCHAR*, SMART_SERVICE_CONFIG_VER2*

        [DllImport("SmartComm2.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "SmartNet_GetUserList")]
        public static extern uint NetGetUserList(IntPtr hsmart_ptr, int devtype, IntPtr szadminid_ptr, IntPtr szadminpw_ptr, IntPtr mszuserlist_ptr);    // HSMART, int, WCHAR*, WCHAR*, WCHAR*

        [DllImport("SmartComm2.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "SmartNet_AddUser")]
        public static extern uint NetAddUser(IntPtr hsmart_ptr, int devtype, IntPtr szadminid_ptr, IntPtr szadminpw_ptr, IntPtr szid_ptr, IntPtr szpw_ptr);    // HSMART, int, WCHAR*, WCHAR*, WCHAR*, WCHAR*

        [DllImport("SmartComm2.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "SmartNet_DelUser")]
        public static extern uint NetDelUser(IntPtr hsmart_ptr, int devtype, IntPtr szadminid_ptr, IntPtr szadminpw_ptr, IntPtr szid_ptr);    // HSMART, int, WCHAR*, WCHAR*, WCHAR*

        [DllImport("SmartComm2.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "SmartNet_PasswordUser")]
        public static extern uint NetPasswordUser(IntPtr hsmart_ptr, int devtype, IntPtr szadminid_ptr, IntPtr szadminpw_ptr, IntPtr szid_ptr, IntPtr szpw_ptr);    // HSMART, int, WCHAR*, WCHAR*, WCHAR*, WCHAR*

        [DllImport("SmartComm2.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "SmartNet_GetOcpConfig")]
        public static extern uint NetGetOcpConfig(IntPtr hsmart_ptr, int devtype, IntPtr szadminid_ptr, IntPtr szadminpw_ptr, IntPtr pocp3_ptr);    // HSMART, int, WCHAR*, WCHAR*, SMART_OCP_CONFIG_VER3*

        [DllImport("SmartComm2.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "SmartNet_SetOcpConfig")]
        public static extern uint NetSetOcpConfig(IntPtr hsmart_ptr, int devtype, IntPtr szadminid_ptr, IntPtr szadminpw_ptr, IntPtr pocp3_ptr);    // HSMART, int, WCHAR*, WCHAR*, SMART_OCP_CONFIG_VER3*

        [DllImport("SmartComm2.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "SmartNet_GetVersion")]
        public static extern uint NetGetVersion(IntPtr hsmart_ptr, int devtype, IntPtr szadminid_ptr, IntPtr szadminpw_ptr, IntPtr szos_ptr, IntPtr szfs_ptr);    // HSMART, int, WCHAR*, WCHAR*, WCHAR*, WCHAR*

        [DllImport("SmartComm2.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "SmartNet_GetUsbState")]
        public static extern uint NetGetUsbState(IntPtr hsmart_ptr, int devtype, IntPtr szadminid_ptr, IntPtr szadminpw_ptr, IntPtr szstate_ptr_ptr, int nlen);    // HSMART, int, WCHAR*, WCHAR*, WCHAR*, int 






        // Return Codes

        public const uint SM_SUCCESS = 0;
        public const uint SM_F_FOUNDNODEV = 0x80000000;	// there is no device to use.
        public const uint SM_F_INVALIDDEVIDX = 0x80000001;	// index of device is out of bound.
        public const uint SM_F_INVALIDBUFPOINTER = 0x80000002;	// invalid buffer pointer. (may be null)
        public const uint SM_F_NOTEXISTDEV = 0x80000003;	// not exist device. (not connected device)
        public const uint SM_F_INVALIDPARAM = 0x80000004;	// invalid parameter value.
        public const uint SM_F_DEVOPENFAILED = 0x80000005;	// device open failed. for more information, use GetLastError API.
        public const uint SM_F_DEVIO = 0x80000006;	// device io operation is failed. for more information, use GetLastError API.
        public const uint SM_F_FOUNDNODRV = 0x80000007;	// not found driver or cannot acquire DEVMODE from driver.
        public const uint SM_F_INVALIDHANDLE = 0x80000008;	// invalid handle value.
        public const uint SM_F_CARDISINSIDE = 0x80000009;	// card is already inside of device.
        public const uint SM_F_NOCARDISINSIDE = 0x8000000A;	// no card is inside of device.
        public const uint SM_F_HOPPEREMPTY = 0x8000000B;	// no cards are in hopper.
        public const uint SM_F_NOCARDONBOTH = 0x8000000C;	// no card both hopper and inside of printer.
        public const uint SM_F_WAITTIMEOUT = 0x8000000D;	// timeout occured while wait...
        public const uint SM_F_CASEOPEN = 0x8000000E;	// case is opened.
        public const uint SM_F_ERRORSTATUS = 0x8000000F;	// current status has error flag.
        public const uint SM_F_CARDIN = 0x80000010;	// card-in action is failed.
        public const uint SM_F_CARDOUT = 0x80000011;	// card-out action is failed.
        public const uint SM_F_CARDOUTBACK = 0x80000012;	// card-back-out action is failed.
        public const uint SM_F_MOVE2MAG = 0x80000013;	// card move (to magnetic) is failed.
        public const uint SM_F_MOVE2IC = 0x80000014;	// card move (to IC) is failed.
        public const uint SM_F_MOVE2RF = 0x80000015;	// card move (to RF) is failed.
        public const uint SM_F_MOVE2ROT = 0x80000016;	// card move (to Rotator) is failed.
        public const uint SM_F_MOVE2DEV = 0x80000017;	// card move (from Rotator) is failed.
        public const uint SM_F_MAGRW = 0x80000018;	// magnetic read/write is failed,
        public const uint SM_F_NOPRINTDATA = 0x80000019;	// printer failed to receive print data.
        public const uint SM_F_PRINT = 0x8000001A;	// print failed.
        public const uint SM_F_SEEKRIBBON = 0x8000001B;	// seek-ribbon is failed.
        public const uint SM_F_MOVERIBBON = 0x8000001C;	// move-ribbon is failed.
        public const uint SM_F_EMPTYRIBBON = 0x8000001D;	// ribbon is empty.
        public const uint SM_F_ICHUP = 0x8000001E;	// ic-head up failed.
        public const uint SM_F_ICHDN = 0x8000001F;	// ic-head down failed.
        public const uint SM_F_ROTTOP = 0x80000020;	// rotate to top is failed.
        public const uint SM_F_ROTBOTTOM = 0x80000021;	// rotate to bottom is failed.
        public const uint SM_F_REQNOMAGTRACK = 0x80000022;	// requested no magnetic track.
        public const uint SM_F_REQMULTIMAGTRACK = 0x80000023;	// requested two or more magnetic tracks in XXXGetBuffer function.
        public const uint SM_F_FILENOTFOUND = 0x80000024;	// file not found.
        public const uint SM_F_FIELDNOTFOUND = 0x80000025;	// field is not exist.
        public const uint SM_F_IMAGELOAD = 0x80000026;	// failed to load image.
        public const uint SM_F_CREATEDC = 0x80000027;	// dc creation is failed.
        public const uint SM_F_VERIFYDRV = 0x80000028;	// driver verification is failed. may the driver is not ours.
        public const uint SM_F_SPOOLING = 0x80000029;	// failed to make spool data. (includes StartDoc, StartPage, EndPage and EndDoc)
        public const uint SM_F_DEVNOTOPENED = 0x8000002A;	// device(printer or ic/rf module which installed printer) is not opened.
        public const uint SM_F_USEDBYOTHER = 0x8000002B;	// usb is temporarily blocked by other. 
        public const uint SM_F_SOCKETCREATE = 0x8000002C;	// socket creation failed.
        public const uint SM_F_SOCKETCONNECT = 0x8000002D;	// socket connection failed.
        public const uint SM_F_SSLINIT = 0x8000002E;	// SSL initialization failed.
        public const uint SM_F_SSLCREATE = 0x8000002F;	// SSL creation failed.
        public const uint SM_F_SSLCONNECT = 0x80000030;	// SSL connection is failed.
        public const uint SM_F_RESERVED = 0x80000031;	// host is already reserved status.
        public const uint SM_F_INVALIDSOCKET = 0x80000032;	// socket fd is invalid.
        public const uint SM_F_LESSSENDED = 0x80000033;	// packet is sended less than requested.
        public const uint SM_F_LESSRECVED = 0x80000034;	// packet is received less than requested.
        public const uint SM_F_SOCKETERROR = 0x80000035;	// socket error occured. for more information, use WSAGetLastError API.
        public const uint SM_F_INVALIDPACKET = 0x80000036;	// packet is not valid. (include signature is not matched or else ...)
        public const uint SM_F_PACKETSEQDIFFER = 0x80000037;	// packet sequence/id is not equaled when receive.
        public const uint SM_F_PACKETFLAGNOREPLY = 0x80000038;	// reply flag is not setted on received packet.
        public const uint SM_F_PACKETFLAGHEADER = 0x80000039;	// sent packet header is incorrect.
        public const uint SM_F_PACKETFLAGARGUMENT = 0x8000003A;	// argument is not valid on sent packet.
        public const uint SM_F_PACKETFLAGEXE = 0x8000003B;	// execution error flag is setted.
        public const uint SM_F_PACKETFLAGBADCMD = 0x8000003C;	// bad command flag is setted.
        public const uint SM_F_PACKETFLAGINIT = 0x8000003D;	// ...
        public const uint SM_F_PACKETFLAGHANDLE = 0x8000003E;	// invalid handle is given.
        public const uint SM_F_FILEOPEN = 0x8000003F;	// file open failed...
        public const uint SM_F_FILEREAD = 0x80000040;	// read from fale is failed.
        public const uint SM_F_NOTSUPPORTYET = 0x80000041;	// not support yet...
        public const uint SM_F_INSUFFICIENTBUF = 0x80000042;	// insufficient buffer.

        // IC error area...
        public const uint SM_F_ICESTABLISH = 0x80000043;	// SCardEstablish failed. for more information, use GetLastError.
        public const uint SM_F_ICLISTREADER = 0x80000044;	// SCardListReaders failed. for more information, use GetLastError.
        public const uint SM_F_ICCONNECT = 0x80000045;	// SCardConnect failed. for more information, use GetLastError.
        public const uint SM_F_ICGETSTATUS = 0x80000046;	// SCardStatus failed. for more information, use GetLastError.
        public const uint SM_F_ICDISCONNECT = 0x80000047;	// SCardDisconenct failed. for more information, use GetLastError.
        public const uint SM_F_ICRELEASE = 0x80000048;	// SCardReleaseContext failed. for more information, use GetLastError.

        // RF error area...
        public const uint SM_F_RFINVALIDSETTING = 0x80000049;	// setting value is incorrect. (i.e., RF Port, ...)
        public const uint SM_F_RFOPEN = 0x8000004A;	// port open failed.
        public const uint SM_F_RFCONNECT = 0x8000004B;	// connect failed.
        public const uint SM_F_RFALREADYOPENED = 0x80000051;	// open the port that already opened.
        public const uint SM_F_RFOPENFAIL = 0x80000052;	// port open failed.
        public const uint SM_F_RFINVALIDCOMMPORT = 0x80000053;	// invalid COM port number.
        public const uint SM_F_RFFAILGETCOMMSTATE = 0x80000054;	// error of communication setting.
        public const uint SM_F_RFFAILSETCOMMSTATE = 0x80000055;	// error of communication setting.
        public const uint SM_F_RFFAILCOMMIOTHREAD = 0x80000056;	// error of communication setting.
        public const uint SM_F_RFPARAMETER = 0x80000057;	// incorrect function parameter.
        public const uint SM_F_RFCOMMSTATE = 0x80000150;	// .
        public const uint SM_F_RFCOMMSTATEPROCESS = 0x80000151;	// running non-blocking mode works.
        public const uint SM_F_RFCOMMSTATEFINISH = 0x8000015F;	// finished non-blocking mode works.
        public const uint SM_F_RFCOMMSTATEERROR = 0x800001D0;	// .
        public const uint SM_F_RFCOMMSTATEERRORWRITE = 0x800001D1;	// data transmition error.
        public const uint SM_F_RFCOMMSTATEERRORFORMAT = 0x800001D2;	// invalid received data format.
        public const uint SM_F_RFCOMMSTATEERRORLENGTH = 0x800001D3;	// incorrect data length of received data length.
        public const uint SM_F_RFCOMMSTATEERRORLRC = 0x800001D4;	// error of received data's LRC.
        public const uint SM_F_RFCOMMSTATEERRORTIMEOUT = 0x800001D5;	// receiving timeout.

        // additional error codes...
        public const uint SM_F_INVALIDNAME = 0x80000200;	// invalid or not supporting barcode name is given.
        public const uint SM_F_ICDLLLOADFAILED = 0x80000201;	// ic(contact smartcard) dll load failed.	;	// DEPRECATED.
        public const uint SM_F_RFDLLLOADFAILED = 0x80000202;	// rf(contactless smartcard) dll load failed.;	// DEPRECATED.
        public const uint SM_F_OBJECTNOTFOUND = 0x80000203;	// object is not exist.
        public const uint SM_F_SUBDLLLOADFAILED = 0x80000204;	// sub-dll load failed... (ie., devconlib.dll, PIR_D02B.dll, ...)
        public const uint SM_F_CONFIGFILEFAILED = 0x80000205;	// configuration file load/save failed...

        public const uint SM_F_OVERHEATED = 0x80000206;	// thermal-head is overheated...
        public const uint SM_F_NOPRINTHREAD = 0x80000207;	// no thermal-head

        public const uint SM_F_LOWERVERSION = 0x80000208;	// SmartCommCl is lower than SmartCommon
        public const uint SM_F_HIGHERVERSION = 0x80000209;	// SmartCommCl is higher than SmartCommon
        public const uint SM_F_OLDSERVER = 0x8000020A;	// SmartCommonService is old.
        public const uint SM_F_VERSIONNOTMATCH = 0x8000020B;	// version is not matched.

        public const uint SM_F_CHANGEPASSWORD = 0x8000020C;	// changing root/user password is failed.
        public const uint SM_F_UNLOCK = 0x8000020D;	// unlock is failed.
        public const uint SM_F_LOCK = 0x8000020E;	// lock is failed.
        public const uint SM_F_FAILEDTOSET = 0x8000020F;	// failed to printer set....				;	// 2014.03.31

        public const uint SM_F_FILESIZEZERO = 0x80000220;	// required file size is 0.
        public const uint SM_F_DEPRECATED = 0x80000221;	// that function is deprecated.
        public const uint SM_F_SERIALNORESPONSE = 0x80000222;	// not responding for serial command...		;	// 2012.11.07


        public const uint SM_F_NETAUTHFAIL = 0x80000300;	// Network Authentification failed.
        public const uint SM_F_NETREBOOTERROR = 0x80000301;	// Network Reboot command failed.
        public const uint SM_F_NETRELOADERROR = 0x80000302;	// Network Reload command failed.
        public const uint SM_F_NETRESETERROR = 0x80000303;	// Network Reset command failed.
        public const uint SM_F_NETUPGRADEERROR = 0x80000304;	// Network upgrade command failed.
        public const uint SM_F_NETGETSYSCFGERROR = 0x80000305;	// Network Get System Config command failed.
        public const uint SM_F_NETSETSYSCFGERROR = 0x80000306;	// Network Set System Config command failed.
        public const uint SM_F_NETGETSVCCFGERROR = 0x80000307;	// Network Get Service Config command failed.
        public const uint SM_F_NETSETSVCCFGERROR = 0x80000308;	// Network Set Service Config command failed.
        public const uint SM_F_NETLISTUSERERROR = 0x80000309;	// Network List User command failed.
        public const uint SM_F_NETADDUSERERROR = 0x8000030A;	// Network Add User command failed.
        public const uint SM_F_NETDELUSERERROR = 0x8000030B;	// Network Delete User command failed.
        public const uint SM_F_NETPASSWDUSERERROR = 0x8000030C;	// Network Change User Password command failed.

        public const uint SM_F_PREEMPTION = 0x80000350;	// Preemption Failed. Other Program Already have the Preemption.
        public const uint SM_F_CARDMOVE = 0x80000351;	// Card Move (among with Modules) is failed.
        public const uint SM_F_MOVEPOSITION = 0x80000352;	// Card Positining Move is failed.
        public const uint SM_F_SETENCCONFIG = 0x80000353;	// Change Encoder Config is failed.
        public const uint SM_F_ENCODING = 0x80000354;	// Encoding failed.
        public const uint SM_F_NOTEXISTMODULE = 0x80000355;	// Module does not exist.
        public const uint SM_F_MODULERESERVED = 0x80000356;	// Module is reserved by other job.
        public const uint SM_F_MODULEOCUPIED = 0x80000357;	// Module is ocupied by other job.
        public const uint SM_F_NOTEXISTJOB = 0x80000358;	// Specified Job Id does not exit. Maybe Wrong Job id or finished already.
        public const uint SM_F_MODULEBUSY = 0x80000359;	// Module is busy
        public const uint SM_F_MODULEHASERROR = 0x8000035A;	// Module has error
        public const uint SM_F_CANCELBYUSER = 0x8000035B;	// Printing Job is Canceled by User.
        public const uint SM_F_EMPTYDATA = 0x8000035C;	// There is No Encoded Data.
        public const uint SM_F_LAMINATE = 0x8000035D;	// error while laminating
        public const uint SM_W_HOLDINGPRINTDATA = 0x8000035E;	// not an error. have print data but not printing.
        public const uint SM_F_INVALIDMODULEID = 0x8000035F;	// invalid module id.						// 2016.09.21
        public const uint SM_F_STACKERFULL = 0x80000360;	// stacker is full.							// 2019.05.27
        public const uint SM_F_MODULEWEAKRESERVED = 0x80000361;	// Module is weak-reserved by other job.	// 2019.07.12
        public const uint SM_W_BLANKIMAGE = 0x80000380;	// Blank (All White) Image is Drawed...		// 2017.06.29
        public const uint SM_F_UNKNOWNCMD = 0x80000381;	// given command is unknown.				// 2018.01.10
        public const uint SM_W_NEAREMPTYHOPPER = 0x80000382;	// hopper became near empty. please prepare it.		// 2018.08.09

        // Packet Error Code
        public const uint SM7_F_PACKETINVALID = 0x80000400;	// packet is invalid (not our format).
        public const uint SM7_F_PACKETSHORT = 0x80000401;	// data
        public const uint SM7_F_PACKETCRC = 0x80000402;	// different crc
        public const uint SM7_F_EMPTYHOPPER = 0x80000403;	// hopper is empty
        public const uint SM7_F_ERRORLASTCMD = 0x80000404;	// error came from last command...


        // PC/SC SmartCard Service Error Code...
        public const uint SM_F_SCARD_F_INTERNAL_ERROR = 0x80100001;	//SCARD_F_INTERNAL_ERROR			//  An internal consistency check failed.
        public const uint SM_F_SCARD_E_CANCELLED = 0x80100002;	//SCARD_E_CANCELLED				 	//	The	action was cancelled by	an SCardCancel request.
        public const uint SM_F_SCARD_E_INVALID_HANDLE = 0x80100003;	//SCARD_E_INVALID_HANDLE			//	The	supplied handle	was	invalid.
        public const uint SM_F_SCARD_E_INVALID_PARAMETER = 0x80100004;	//SCARD_E_INVALID_PARAMETER		 	//	One	or more	of the supplied	parameters could not be	properly interpreted.
        public const uint SM_F_SCARD_E_INVALID_TARGET = 0x80100005;	//SCARD_E_INVALID_TARGET			//	Registry startup information is	missing	or invalid.
        public const uint SM_F_SCARD_E_NO_MEMORY = 0x80100006;	//SCARD_E_NO_MEMORY				 	//	Not	enough memory available	to complete	this command.
        public const uint SM_F_SCARD_F_WAITED_TOO_LONG = 0x80100007;	//SCARD_F_WAITED_TOO_LONG			//	An internal	consistency	timer has expired.
        public const uint SM_F_SCARD_E_INSUFFICIENT_BUFFER = 0x80100008;	//SCARD_E_INSUFFICIENT_BUFFER		//	The	data buffer	to receive returned	data is	too	small for the returned data.
        public const uint SM_F_SCARD_E_UNKNOWN_READER = 0x80100009;	//SCARD_E_UNKNOWN_READER			//	The	specified reader name is not recognized.
        public const uint SM_F_SCARD_E_TIMEOUT = 0x8010000A;	//SCARD_E_TIMEOUT					//	The	user-specified timeout value has expired.
        public const uint SM_F_SCARD_E_SHARING_VIOLATION = 0x8010000B;	//SCARD_E_SHARING_VIOLATION		 	//	The	smart card cannot be accessed because of other connections outstanding.
        public const uint SM_F_SCARD_E_NO_SMARTCARD = 0x8010000C;	//SCARD_E_NO_SMARTCARD			 	//	The	operation requires a Smart Card, but no	Smart Card is currently	in the device.
        public const uint SM_F_SCARD_E_UNKNOWN_CARD = 0x8010000D;	//SCARD_E_UNKNOWN_CARD			 	//	The	specified smart	card name is not recognized.
        public const uint SM_F_SCARD_E_CANT_DISPOSE = 0x8010000E;	//SCARD_E_CANT_DISPOSE			 	//	The	system could not dispose of	the	media in the requested manner.
        public const uint SM_F_SCARD_E_PROTO_MISMATCH = 0x8010000F;	//SCARD_E_PROTO_MISMATCH			//	The	requested protocols	are	incompatible with the protocol currently in	use	with the smart card.
        public const uint SM_F_SCARD_E_NOT_READY = 0x80100010;	//SCARD_E_NOT_READY				 	//	The	reader or smart	card is	not	ready to accept	commands.
        public const uint SM_F_SCARD_E_INVALID_VALUE = 0x80100011;	//SCARD_E_INVALID_VALUE			 	//	One	or more	of the supplied	parameters values could	not	be properly	interpreted.
        public const uint SM_F_SCARD_E_SYSTEM_CANCELLED = 0x80100012;	//SCARD_E_SYSTEM_CANCELLED		 	//	The	action was cancelled by	the	system,	presumably to log off or shut down.
        public const uint SM_F_SCARD_F_COMM_ERROR = 0x80100013;	//SCARD_F_COMM_ERROR				//	An internal	communications error has been detected.
        public const uint SM_F_SCARD_F_UNKNOWN_ERROR = 0x80100014;	//SCARD_F_UNKNOWN_ERROR			 	//	An internal	error has been detected, but the source	is unknown.
        public const uint SM_F_SCARD_E_INVALID_ATR = 0x80100015;	//SCARD_E_INVALID_ATR				//	An ATR obtained	from the registry is not a valid ATR string.
        public const uint SM_F_SCARD_E_NOT_TRANSACTED = 0x80100016;	//SCARD_E_NOT_TRANSACTED			//	An attempt was made	to end a non-existent transaction.
        public const uint SM_F_SCARD_E_READER_UNAVAILABLE = 0x80100017;	//SCARD_E_READER_UNAVAILABLE		//	The	specified reader is	not	currently available	for	use.
        public const uint SM_F_SCARD_P_SHUTDOWN = 0x80100018;	//SCARD_P_SHUTDOWN				 	//	The	operation has been aborted to allow	the	server application to exit.
        public const uint SM_F_SCARD_E_PCI_TOO_SMALL = 0x80100019;	//SCARD_E_PCI_TOO_SMALL			 	//	The	PCI	Receive	buffer was too small.
        public const uint SM_F_SCARD_E_READER_UNSUPPORTED = 0x8010001A;	//SCARD_E_READER_UNSUPPORTED		//	The	reader driver does not meet	minimal	requirements for support.
        public const uint SM_F_SCARD_E_DUPLICATE_READER = 0x8010001B;	//SCARD_E_DUPLICATE_READER		 	//	The	reader driver did not produce a	unique reader name.
        public const uint SM_F_SCARD_E_CARD_UNSUPPORTED = 0x8010001C;	//SCARD_E_CARD_UNSUPPORTED		 	//	The	smart card does	not	meet minimal requirements for support.
        public const uint SM_F_SCARD_E_NO_SERVICE = 0x8010001D;	//SCARD_E_NO_SERVICE				//	The	Smart card resource	manager	is not running.
        public const uint SM_F_SCARD_E_SERVICE_STOPPED = 0x8010001E;	//SCARD_E_SERVICE_STOPPED			//	The	Smart card resource	manager	has	shut down.
        public const uint SM_F_SCARD_E_UNEXPECTED = 0x8010001F;	//SCARD_E_UNEXPECTED				//	An unexpected card error has occurred.
        public const uint SM_F_SCARD_E_ICC_INSTALLATION = 0x80100020;	//SCARD_E_ICC_INSTALLATION		 	//	No Primary Provider	can	be found for the smart card.
        public const uint SM_F_SCARD_E_ICC_CREATEORDER = 0x80100021;	//SCARD_E_ICC_CREATEORDER			//	The	requested order	of object creation is not supported.
        public const uint SM_F_SCARD_E_UNSUPPORTED_FEATURE = 0x80100022;	//SCARD_E_UNSUPPORTED_FEATURE		//	This smart card	does not support the requested feature.
        public const uint SM_F_SCARD_E_DIR_NOT_FOUND = 0x80100023;	//SCARD_E_DIR_NOT_FOUND			 	//	The	identified directory does not exist	in the smart card.
        public const uint SM_F_SCARD_E_FILE_NOT_FOUND = 0x80100024;	//SCARD_E_FILE_NOT_FOUND			//	The	identified file	does not exist in the smart	card.
        public const uint SM_F_SCARD_E_NO_DIR = 0x80100025;	//SCARD_E_NO_DIR					//	The	supplied path does not represent a smart card directory.
        public const uint SM_F_SCARD_E_NO_FILE = 0x80100026;	//SCARD_E_NO_FILE					//	The	supplied path does not represent a smart card file.
        public const uint SM_F_SCARD_E_NO_ACCESS = 0x80100027;	//SCARD_E_NO_ACCESS				 	//	Access is denied to	this file.
        public const uint SM_F_SCARD_E_WRITE_TOO_MANY = 0x80100028;	//SCARD_E_WRITE_TOO_MANY			//	The	smartcard does not have	enough memory to store the information.
        public const uint SM_F_SCARD_E_BAD_SEEK = 0x80100029;	//SCARD_E_BAD_SEEK				 	//	There was an error trying to set the smart card	file object	pointer.
        public const uint SM_F_SCARD_E_INVALID_CHV = 0x8010002A;	//SCARD_E_INVALID_CHV				//	The	supplied PIN is	incorrect.
        public const uint SM_F_SCARD_E_UNKNOWN_RES_MNG = 0x8010002B;	//SCARD_E_UNKNOWN_RES_MNG			//	An unrecognized	error code was returned	from a layered component.
        public const uint SM_F_SCARD_E_NO_SUCH_CERTIFICATE = 0x8010002C;	//SCARD_E_NO_SUCH_CERTIFICATE		//	The	requested certificate does not exist.
        public const uint SM_F_SCARD_E_CERTIFICATE_UNAVAILABLE = 0x8010002D;	//SCARD_E_CERTIFICATE_UNAVAILABLE	//	The	requested certificate could	not	be obtained.
        public const uint SM_F_SCARD_E_NO_READERS_AVAILABLE = 0x8010002E;	//SCARD_E_NO_READERS_AVAILABLE	 	//	Cannot find	a smart	card reader.
        public const uint SM_F_SCARD_E_COMM_DATA_LOST = 0x8010002F;	//SCARD_E_COMM_DATA_LOST			//	A communications error with	the	smart card has been	detected.  Retry the operation.
        public const uint SM_F_SCARD_E_NO_KEY_CONTAINER = 0x80100030;	//SCARD_E_NO_KEY_CONTAINER		 	//	The	requested key container	does not exist on the smart	card.
        public const uint SM_F_SCARD_E_SERVER_TOO_BUSY = 0x80100031;	//SCARD_E_SERVER_TOO_BUSY			//  The Smart card resource manager is too busy to complete this operation.

        // PC/SC SmartCard Service Warning Code...
        public const uint SM_F_SCARD_W_UNSUPPORTED_CARD = 0x80100065;	//SCARD_W_UNSUPPORTED_CARD		 
        public const uint SM_F_SCARD_W_UNRESPONSIVE_CARD = 0x80100066;	//SCARD_W_UNRESPONSIVE_CARD		 	//	The	smart card is not responding to	a reset.		
        public const uint SM_F_SCARD_W_UNPOWERED_CARD = 0x80100067;	//SCARD_W_UNPOWERED_CARD			//	Power has been removed from	the	smart card,	so that	further	communication is not possible.
        public const uint SM_F_SCARD_W_RESET_CARD = 0x80100068;	//SCARD_W_RESET_CARD				//	The	smart card has been	reset, so any shared state information is invalid.
        public const uint SM_F_SCARD_W_REMOVED_CARD = 0x80100069;	//SCARD_W_REMOVED_CARD			 	//	The	smart card has been	removed, so	that further communication is not possible.
        public const uint SM_F_SCARD_W_SECURITY_VIOLATION = 0x8010006A;	//SCARD_W_SECURITY_VIOLATION		//	Access was denied because of a security	violation.
        public const uint SM_F_SCARD_W_WRONG_CHV = 0x8010006B;	//SCARD_W_WRONG_CHV				 	//	The	card cannot	be accessed	because	the	wrong PIN was presented.
        public const uint SM_F_SCARD_W_CHV_BLOCKED = 0x8010006C;	//SCARD_W_CHV_BLOCKED				//	The	card cannot	be accessed	because	the	maximum	number of PIN entry	attempts has been reached.
        public const uint SM_F_SCARD_W_EOF = 0x8010006D;	//SCARD_W_EOF						//	The	end	of the smart card file has been	reached.
        public const uint SM_F_SCARD_W_CANCELLED_BY_USER = 0x8010006E;	//SCARD_W_CANCELLED_BY_USER		 	//	The	action was cancelled by	the	user.
        public const uint SM_F_SCARD_W_CARD_NOT_AUTHENTICATED = 0x8010006F;	//SCARD_W_CARD_NOT_AUTHENTICATED	//  No PIN was presented to the smart card.

    }
}


