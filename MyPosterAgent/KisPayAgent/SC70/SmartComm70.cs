using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;


namespace SC70
{
    public class SmartComm70
    {
        public const int SMART70_DEVICELIST_ALL         = 0x00;
        public const int SMART70_DEVICELIST_USB			= 0x01;
        public const int SMART70_DEVICELIST_NET			= 0x02;
        public const int SMART70_DEVICELIST_485			= 0x04;

        public const int SMART70_OPENDEVICE_BYID         = 0;
        public const int SMART70_OPENDEVICE_BYDESC       = 1;


        public const byte	PAGE_FRONT			= 0;
        public const byte	PAGE_BACK			= 1;
        public const byte	PAGE_P1FRONT		= PAGE_FRONT;
        public const byte	PAGE_P1BACK			= PAGE_BACK;
        public const byte	PAGE_P2FRONT		= 2;
        public const byte	PAGE_P2BACK			= 3;
        public const byte   PAGE_COUNT          = PAGE_P2BACK + 1;
        public const byte	PAGE_START			= PAGE_P1FRONT;
        public const byte	PAGE_END			= PAGE_P2BACK;
        public const byte	PANEL_COLOR			= 1;
        public const byte	PANEL_BLACK			= 2;
        public const byte	PANEL_OVERLAY		= 4;
        public const byte	PANEL_UV			= 8;



        
        public const int MAX_SMART_PRINTER = 32;

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct SMART_PRINTER_ITEM
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]   public String name;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]    public String id;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]    public String dev;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]   public String desc;
            public int pid;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct SMART_PRINTER_LIST
        {
            public int n;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MAX_SMART_PRINTER)]  public SMART_PRINTER_ITEM[] item;
        }




        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct SMART70_PRINTER_PORT_USB
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]  public String port;      // usb port
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]  public String link;     // symbolic link of usb port
            public int is_bridge;                                                           // Network module bridge
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct SMART70_PRINTER_PORT_NET
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]  public String ver;       // version of network protocol
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]  public String ip;        // ip address
            public int port;                                                                // tcp port
            public int is_ssl;                                                              // ssl protocol
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct SMART70_PRINTER_STANDARD
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]  public String name;     // printer name
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]  public String id;        // printer ID
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]  public String dev;       // device connection
            public int dev_type;                                                            // 1=USB, 2=NET
            public int pid;                                                                 // USB product ID
            public SMART70_PRINTER_PORT_USB usb;
            public SMART70_PRINTER_PORT_NET net;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct SMART70_PRINTER_OPTIONS
        {
            public int is_dual;                                                             // dual printer
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]  public String ic1;       // internal contact encoder
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]  public String ic2;       // external contact SIM encoder
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]  public String rf1;       // internal contactless encoder
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]  public String rf2;       // external contactless encoder
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct SMART70_PRINTER_INFO
        {
            public SMART70_PRINTER_STANDARD std;
            public SMART70_PRINTER_OPTIONS opt;
        }




        public const int    SMART70_MAX_DEVICE			= 32;

        public const int    SMART70_DEV_INHOPPER		= 0x10;
        public const int    SMART70_DEV_MULINHOPPER		= 0x20;
        public const int    SMART70_DEV_ENCODER			= 0x30;
        public const int    SMART70_DEV_PRINTER			= 0x40;
        public const int    SMART70_DEV_LAMINATOR		= 0x50;
        public const int    SMART70_DEV_FLIPPER			= 0x60;
        public const int    SMART70_DEV_OUTHOPPER		= 0x70;
        public const int    SMART70_DEV_MULOUTHOPPER	= 0x80;
        public const int    SMART70_DEV_LASER			= 0x60;
        public const int    SMART70_DEV_INTENCODER		= 0x90;			// internal encoder.
        public const int    SMART70_DEV_OTHER			= 0xD0;			// other devices
        public const int    SMART70_DEV_ALL				= 0xF0;
        public const int    SMART70_DEV_IDX				= 0x0F;
        public const int    SMART70_DEV_MASK			= SMART70_DEV_ALL;
        public const int    SMART70_MOD_LASER			= (SMART70_DEV_LASER|2);    // laser device module-id


        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct SMART70_DEVICE_STATUS
        {
            public Byte id;
            public Byte status;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct SMART70_DEVCONNLIST
        {
            public int  n;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = SMART70_MAX_DEVICE)]  public SMART70_DEVICE_STATUS[] dev;
        }





       public const int    COUNTPARAM_VER_0        	= 0;
       public const int    COUNTPARAM_VER_LATEST	= COUNTPARAM_VER_0;

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct COUNTPARAM
        {
            public  int		ver;				// current version is 0.
	        public  int		img;				// image issue count
	        public  int		mag;				// magnetic issue count
	        public  int		contact;			// contact smart card issue count
	        public  int		sim;				// SIM card issue count
	        public  int		contactless_ext;	// contactless smart card (external) issue count
	        public  int		contactless_int;	// contactless smart card (internal) issue count
	        public  int     cleanskip;			// cleaning skip count
	        public  int		cleaning;			// cleaning count
        }



        public const int	SYSINFO_VER_0			= 0;
        public const int    SYSINFO_VER_LASTEST		= SYSINFO_VER_0;

        public const int    MAX_VER_LEN				= 32;
        public const int    MAX_SERIAL_LEN			= 24;


        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct SMART_SYSINFO_PRINTER
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_VER_LEN)]      public String ver;      // printer f/w ver
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_SERIAL_LEN)]   public String serial;   // printer serial number
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_SERIAL_LEN)]   public String hserial;  // thermal header serial number
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]               public Byte[] reserved;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct SMART_SYSINFO_LAMINATOR
        {
            public int  installed;                                                                      // flag for laminator installation
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_VER_LEN)]      public String ver;      // laminator f/w ver
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_SERIAL_LEN)]   public String serial;   // laminator serial number
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]               public Byte[] reserved;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct SMART_SYSINFO
        {
	        public int		ver;				        // structure version (= SYSINFO_VER_0)
	        public SMART_SYSINFO_PRINTER    printer;
	        public SMART_SYSINFO_LAMINATOR  laminator;
        }





        public const int	SMART70_SUPPLY_AUTO				= 0;
        public const int	SMART70_SUPPLY_HOPPER			= 1;

        public const int	SMART70_TRAY_CR80				= 0;

        public const int	SMART70_RIBBON_YMCKO			= 0;
        public const int	SMART70_RIBBON_YMCKOK			= 1;
        public const int	SMART70_RIBBON_hYMCKO			= 2;
        public const int	SMART70_RIBBON_KO				= 3;
        public const int	SMART70_RIBBON_K				= 4;
        public const int	SMART70_RIBBON_BO				= 5;
        public const int	SMART70_RIBBON_B				= 6;
        public const int	SMART70_RIBBON_YMC				= 7;	// BYMCKO deprecated. YMC replace it.
        public const int	SMART70_RIBBON_YMCKFO			= 8;
        public const int	SMART70_RIBBON_REWRITABLE		= 9;
        public const int	SMART70_RIBBON_hYMCKOKO			= 11;
        public const int	SMART70_RIBBON_YMCKOKR			= 12;

        public const int	SMART70_RESIN_BLACKTEXT			= 0;	// Extract Black Text to RESIN Panel
        public const int	SMART70_RESIN_BLACKDOT			= 1;	// Extract Black Dot(Pixel) to RESIN Panel
        public const int	SMART70_RESIN_NOTUSE			= 2;	// No Extraction.
        public const int	SMART70_RESIN_DYNAMIC			= 3;	// use DC's ExtEscape().

        public const int	SMART70_RIBBONSPLIT_NORMAL		= 0;
        public const int	SMART70_RIBBONSPLIT_FORBACK		= 1;

        public const int	SMART70_FLIP_NORMAL				= 0;
        public const int	SMART70_FLIP_VERTICAL			= 1;
        public const int	SMART70_FLIP_HORIZONTAL			= 2;
        public const int	SMART70_FLIP_ALL				= 3;

        public const int	SMART_COLOR_COLOR				= 0;
        public const int	SMART_COLOR_BLACKNWHITE			= 1;

        public const int	SMART_DITHER_THRESHOLD			= 0;
        public const int	SMART_DITHER_RANDOM				= 1;
        public const int	SMART_DITHER_DIFFUSION			= 2;
        public const int	SMART_DITHER_HALFTONE			= 3;	
        public const int	SMART_DITHER_HALFTONE2			= 4;

        public const int	SMART_MEDIA_STANDARD			= 0;
        public const int	SMART_MEDIA_SMART				= 1;	// Exclude IC Chip Area
        public const int	SMART_MEDIA_SMARTRIGHT			= 2;	// Exclude Left of IC Chip Area
        public const int	SMART_MEDIA_MSISO				= 3;	// Exclude MS ISO Track Area
        public const int	SMART_MEDIA_MSJIS				= 4;	// Exclude MS JIS Track Area
        public const int	SMART_MEDIA_SMARTMSJIS			= 5;	// Exclude MS JIS Track and IC Chip Area
        public const int	SMART_MEDIA_NOOVERLAY			= 6;	// No Overlay
        public const int	SMART_MEDIA_USERDEFINED			= 7;	// Use User Defined Bitmap
        public const int	SMART_MEDIA_DYNAMIC				= 8;	// use DC's ExtEscape().
        public const string	SMART_MEDIA_BITMAP				= "cardprinter_mask_";	// $SYSTEM32\cardprinter_mask_#.bmp

        public const int	SMART_LAMINATESIDE_NONE			= 0;
        public const int	SMART_LAMINATESIDE_TOP			= 1;
        public const int	SMART_LAMINATESIDE_BOTTOM		= 2;
        public const int	SMART_LAMINATESIDE_BOTH			= 3;

        public const int	SMART_EJECT_EJECT_CARD			= 0;
        public const int	SMART_EJECT_HOLD_CARD			= 1;

        public const int	SMART70_PRINT_NOTUSE			= 0;
        public const int	SMART70_PRINT_USE				= 1;

        public const int	SMART70_PRINTSIDE_FRONT			= 0;	// Front Side Only
        public const int	SMART70_PRINTSIDE_BACK			= 1;	// Back Side Only
        public const int	SMART70_PRINTSIDE_BOTH			= 2;	// Both of Front and Back Side

        public const int	SMART70_QUALITY_STANDARD		= 0;
        public const int	SMART70_QUALITY_PARTIAL			= 1;

        public const int	SMART70_SPEED_NORMAL			= 1;		// --> Normal (on 70 Driver UI)
        public const int	SMART70_SPEED_HIGH				= 2;		// --> High   (on 70 Driver UI)

        public const int	SMART70_LAMIN_NOTUSE			= 0;
        public const int	SMART70_LAMIN_USE				= 1;

        public const int	SMART70_LAMIN_TOP				= 0;		
        public const int	SMART70_LAMIN_BOTTOM			= 1;		
        public const int	SMART70_LAMIN_BOTH				= 2;		

        public const int	SMART70_LAMIN_OVERLAY_NOTUSE	= 0;		
        public const int	SMART70_LAMIN_OVERLAY_USE		= 1;

        public const int	SMART70_MODULE_INTERNAL			= 0;
        public const int	SMART70_MODULE_EXTERNAL			= 1;
        public const int	SMART70_MODULE_BOTH				= 2;

        public const int	SMART70_MAG_COER_AUTO			= 0;
        public const int	SMART70_MAG_COER_LO				= 1;
        public const int	SMART70_MAG_COER_HI				= 2;

        public const int	SMART70_MAG_OPTION_FORWARD		= 0;
        public const int	SMART70_MAG_OPTION_BACKWARD		= 1;
        public const int	SMART70_MAG_OPTION_BITMODE		= 2;

        public const int	SMART70_MAG_FLIP_NOTUSE			= 0;
        public const int	SMART70_MAG_FLIP_USE			= 1;

        public const int	SMART70_MAG_NOTUSE				= 0;
        public const int	SMART70_MAG_USE					= 1;

        public const int	SMART70_MAG_ENCODING_ISO		= 0;
        public const int	SMART70_MAG_ENCODING_BALLYS		= 1;




        public const int	SMART_MAG_BUFFER				= 1024;

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct OEM_DMEXTRAHEADER
        {
            public int  dwSize;
            public int  dwSignature;
            public int  dwVersion;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct OEMDEV
        {
            public OEM_DMEXTRAHEADER    dmOEMExtra;
	        public int                  dwASMain;
	        public int                  dwASYellow;
	        public int                  dwASMagenta;
	        public int                  dwASCyan;
	        public int                  dwASBlack;
	        public int                  dwASOverlay;
	        public int                  dwASBPText;
	        public int                  dwASBPDot;
	        public int			        dwASBPThreshold;
	        public int			        dwASBPDitherDegree;
	        public int			        dwASBPResin;
	        public int			        dwASErase;
	        public int                  dwASWaitInRFUse;
	        public int                  dwASWaitInRFSide;
	        public int                  dwASWaitInRFPos;
	        public int                  dwASWaitInRFTime;
	        public int                  dwASWaitExRFUse;
	        public int                  dwASWaitExRFSide;
	        public int                  dwASWaitExRFPos;
	        public int                  dwASWaitExRFTime;
	        public int                  dwASWaitExICUse;
	        public int                  dwASWaitExICSide;
	        public int                  dwASWaitExICPos;
	        public int                  dwASWaitExICTime;
	        public int			        dwIOSupply;
	        public int			        dwIOTray;
	        public int			        dwIOOutput;
	        public int			        dwPrtUse;
	        public int			        dwPrtSide;
	        public int			        dwPrtColorFront;
	        public int			        dwPrtColorBack;
	        public int			        dwPrtMediaFront;
	        public int			        dwPrtMediaBack;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 1024)]   public String strPrtMediaUserFront;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 1024)]   public String strPrtMediaUserBack;
	        public int			        dwPrtRibbon;
	        public int			        dwPrtQuality;
	        public int			        dwPrtSpeed;
	        public int			        dwPrtHeatControl;
	        public int			        dwPrtFlipFront;
	        public int			        dwPrtFlipBack;
	        public int			        dwPrtDither;
	        public int			        dwPrtRibbonSplit;
	        public int			        dwPrtUse2;
	        public int			        dwPrtSide2;
	        public int			        dwPrtColorFront2;
	        public int			        dwPrtColorBack2;
	        public int			        dwPrtMediaFront2;
	        public int			        dwPrtMediaBack2;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 1024)]   public String strPrtMediaUserFront2;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 1024)]   public String strPrtMediaUserBack2;
	        public int			        dwPrtRibbon2;
	        public int			        dwPrtQuality2;
	        public int			        dwPrtSpeed2;
	        public int			        dwPrtHeatControl2;
	        public int			        dwPrtFlipFront2;
	        public int			        dwPrtFlipBack2;
	        public int			        dwPrtDither2;
	        public int			        dwPrtRibbonSplit2;
	        public int			        dwLamUse;
	        public int			        dwLamSide;			
	        public int			        dwLamOverlay;
	        public int			        dwLamUse2;
	        public int			        dwLamSide2;
	        public int			        dwLamReserved2;
	        public int			        dwEncUse;
	        public int			        dwEncModule;	
	        public int			        dwEncInCoer;	
	        public int			        dwEncInOption;	
	        public int			        dwEncInMagFlip;	
	        public int			        dwEncInEncoding;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 1024)]   public String strEncInTrack1;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 1024)]   public String strEncInTrack2;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 1024)]   public String strEncInTrack3;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 1024)]   public String strEncInJISTrack;
	        public int			        dwEncExCoer;		
	        public int			        dwEncExOption;		
	        public int			        dwEncExMagFlip;		
	        public int			        dwEncExEncoding;	
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 1024)]   public String strEncExTrack1;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 1024)]   public String strEncExTrack2;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 1024)]   public String strEncExTrack3;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 1024)]   public String strEncExJISTrack;
	        public int			        dwASCorrectColor;	
	        public int			        dwASCorrectMono;	
	        public int			        dwASCorrectOverlay;	
	        public int			        dwEncInMagRepeat;	
	        public int			        dwEncExMagRepeat;	
	        public int			        dwOptSeparate;		
	        public int			        dwASFastAlignment;	
	        public int			        dwPrtAntiAliasing;	
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]   public Byte[] reserved;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct SMART70_DEVMODE
        {
	        public Win32.DEVMODEW     devmode;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 564)]   public Byte[] reserved;
	        public OEMDEV       oemdev;
        }



        public const int    MAX_FIELDNAMELEN	= 32;
        public const int    MAX_FIELDVALUELEN = 1024;



        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct DT2INFO
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
	        public int      	color;
	        public int			option;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = Win32.LF_FACESIZE)]   public String szFaceName;
        }





        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct S70Border
        {
	        public Int16		type;
	        public Int16		width;
	        public int			color;

            public S70Border(int _type, int _width, int _color)
            {
                type = (Int16)_type;
                width = (Int16)_width;
                color = _color;
            }
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct S70BackGround
        {
	        public int  		fill;
	        public int			color;
            public Byte         transparency;		// % unit. (0:Opaque, 100:transparent)

            public S70BackGround(int _fill, int _color)
            {
                fill = _fill;
                color = _color;
                transparency = 0;
            }
        }


        public const Byte BS_NONE             = 5;
        public const Byte BS_SOLID            = 0;
        public const Byte BS_DASH             = 1;
        public const Byte BS_DOT              = 2;
        public const Byte BS_DASHDOT          = 3;
        public const Byte BS_DASHDOTDOT       = 4;


        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct S70FontInfo
        {
	        public int  		size;
	        public Byte			style;
            public int          color;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = Win32.LF_FACESIZE)]   public String name;

            public S70FontInfo(int _size, int _style, int _color, string _name)
            {
                size = _size;
                style = (Byte)_style;
                color = _color;
                name = _name;
            }
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct S70Laser
        {
	        public int  		power;
	        public int			speed;
	        public int			frequency;
	        public int			angle;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]   public int[] reserved;

            public S70Laser(int _p, int _s, int _f, int _a)
            {
                power = _p;
                speed = _s;
                frequency = _f;
                angle = _a;
                reserved = new int[16];
                for (int i = 0; i < 16; i++)
                    reserved[i] = 0;
            }
        }

        
        


        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct LLINE
        {
	        public Byte page;
	        public Byte panel;
	        public int  x;
	        public int  y;
	        public int  cx;
	        public int  cy;
	        public S70Border        border;
	        public S70BackGround    back;

            public LLINE(byte _page, byte _panel, int _x, int _y, int _cx, int _cy, S70Border _bd, S70BackGround _bk)
            {
                page = _page;
                panel = _panel;
                x = _x;
                y = _y;
                cx = _cx;
                cy = _cy;
                border = _bd;
                back = _bk;
            }
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct LRECT
        {
	        public Byte page;
	        public Byte panel;
	        public int  x;
	        public int  y;
	        public int  cx;
	        public int  cy;
	        public S70Border        border;
	        public S70BackGround    back;

            public LRECT(byte _page, byte _panel, int _x, int _y, int _cx, int _cy, S70Border _bd, S70BackGround _bk)
            {
                page = _page;
                panel = _panel;
                x = _x;
                y = _y;
                cx = _cx;
                cy = _cy;
                border = _bd;
                back = _bk;
            }
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct LOVAL
        {
	        public Byte page;
	        public Byte panel;
	        public int  x;
	        public int  y;
	        public int  cx;
	        public int  cy;
	        public S70Border        border;
	        public S70BackGround    back;

            public LOVAL(byte _page, byte _panel, int _x, int _y, int _cx, int _cy, S70Border _bd, S70BackGround _bk)
            {
                page = _page;
                panel = _panel;
                x = _x;
                y = _y;
                cx = _cx;
                cy = _cy;
                border = _bd;
                back = _bk;
            }
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct LRRECT
        {
	        public Byte page;
	        public Byte panel;
	        public int  x;
	        public int  y;
	        public int  cx;
	        public int  cy;
	        public S70Border        border;
	        public S70BackGround    back;
            public int  round;

            public LRRECT(byte _page, byte _panel, int _x, int _y, int _cx, int _cy, S70Border _bd, S70BackGround _bk, int _r)
            {
                page = _page;
                panel = _panel;
                x = _x;
                y = _y;
                cx = _cx;
                cy = _cy;
                border = _bd;
                back = _bk;
                round = _r;
            }
        }


        public const int	OBJ_ALIGN_LEFT		= 0x00;
        public const int	OBJ_ALIGN_CENTER	= 0x01;
        public const int	OBJ_ALIGN_RIGHT		= 0x02;
        public const int	OBJ_ALIGN_JUSTIFY	= 0x03;	// text object only...
        public const int	OBJ_ALIGN_HNOALIGN	= 0x04;
        public const int	OBJ_ALIGN_TOP		= 0x00;
        public const int	OBJ_ALIGN_MIDDLE	= 0x10;
        public const int	OBJ_ALIGN_BOTTOM	= 0x20;
        public const int	OBJ_ALIGN_VNOALIGN	= 0x30;

        public const byte	FONT_NORMAL	    	= 0x00;
        public const byte	FONT_BOLD	    	= 0x01;
        public const byte	FONT_ITALIC	    	= 0x02;
        public const byte	FONT_UNDERLINE  	= 0x04;


        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct LTEXT
        {
	        public Byte     page;
	        public Byte     panel;
	        public int      x;
	        public int      y;
	        public int      cx;
	        public int      cy;
	        public S70Border        border;
	        public S70BackGround    back;
	        public S70FontInfo      font;
	        public int      align;
	        public IntPtr   szText;

            public LTEXT(byte _page, byte _panel, int _x, int _y, int _cx, int _cy, S70Border _bd, S70BackGround _bk, S70FontInfo _font, int _align, IntPtr _strdata)
            {
                page = _page;
                panel = _panel;
                x = _x;
                y = _y;
                cx = _cx;
                cy = _cy;
                border = _bd;
                back = _bk;
                font = _font;
                align = _align;
                szText = _strdata;
            }
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct LBAR
        {
	        public Byte     page;
	        public Byte     panel;
	        public int      x;
	        public int      y;
	        public int      cx;
	        public int      cy;
	        public S70Border        border;
	        public S70BackGround    back;
	        public IntPtr   szBarName;
	        public int      nSize;
	        public int      rgbBar;
	        public IntPtr   szData;
	        public IntPtr   szPost;
	        public bool     showString;
	        public int      opt1;
	        public int      opt2;

            public LBAR(byte _page, byte _panel, int _x, int _y, int _cx, int _cy, S70Border _bd, S70BackGround _bk,
                      IntPtr _barname, int _size, int _color, IntPtr _strdata, IntPtr _strpost, bool _show, int _opt1, int _opt2)
            {
                page = _page;
                panel = _panel;
                x = _x;
                y = _y;
                cx = _cx;
                cy = _cy;
                border = _bd;
                back = _bk;
                szBarName = _barname;
                nSize = _size;
                rgbBar = _color;
                szData = _strdata;
                szPost = _strpost;
                showString = _show;
                opt1 = _opt1;
                opt2 = _opt2;
            }
        }


        public const Byte	IMGSCALE_FITHORZ	= 0x00;	// scale to fit to width of frame
        public const Byte	IMGSCALE_FITVERT	= 0x01;	// scale to fit to height of frame
        public const Byte	IMGSCALE_FITFRAME	= 0x02;	// scale to fit to frame.
        //public const Byte	IMGSCALE_USER		= 0x03;	// scale factor is user setted.

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct LIMG
        {
	        public Byte     page;
	        public Byte     panel;
	        public int      x;
	        public int      y;
	        public int      cx;
	        public int      cy;
	        public S70Border        border;
	        public S70BackGround    back;
	        public IntPtr   szFile;
	        public Byte     scale;
	        public Byte     align;

            public LIMG(byte _page, byte _panel, int _x, int _y, int _cx, int _cy, S70Border _bd, S70BackGround _bk,
                        IntPtr _file, byte _scale, byte _align)
            {
                page = _page;
                panel = _panel;
                x = _x;
                y = _y;
                cx = _cx;
                cy = _cy;
                border = _bd;
                back = _bk;
                szFile = _file;
                scale = _scale;
                align = _align;
            }
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct LBMP
        {
	        public Byte     page;
	        public Byte     panel;
	        public int      x;
	        public int      y;
	        public int      cx;
	        public int      cy;
	        public S70Border        border;
	        public S70BackGround    back;
	        public IntPtr   hbmp;
	        public Byte     scale;
	        public Byte     align;

            public LBMP(byte _page, byte _panel, int _x, int _y, int _cx, int _cy, S70Border _bd, S70BackGround _bk,
                        IntPtr _hbmp, byte _scale, byte _align)
            {
                page = _page;
                panel = _panel;
                x = _x;
                y = _y;
                cx = _cx;
                cy = _cy;
                border = _bd;
                back = _bk;
                hbmp = _hbmp;
                scale = _scale;
                align = _align;
            }
        }





        public const int UNITINFO2_VER_1		= 1;
        public const int UNITINFO2_VER_2		= 2;
        public const int UNITINFO2_VER		    = UNITINFO2_VER_2;

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct UNITTEXT2
        {
	        public Int16    leftMargin;		// spaceLeft
	        public Int16    topMargin;		// spaceTop
	        public Int16    rightMargin;	// spaceRight
	        public Int16    bottomMargin;	// spaceBottom
	        public Byte     align;
	        public S70FontInfo  font;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_FIELDNAMELEN+1)]   public String field;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]   public Byte[] reserved;
        }


        public const int UIMG2OPT_OFFSET_ORIGIN = 0x0000;
        public const int UIMG2OPT_OFFSET_DRAW	= 0x0001;
        public const int UIMG2OPT_OFFSET_MASK	= 0x0001;

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct UNITIMAGE2
        {
	        public int		option;
	        public int		widthZoom;
	        public int		heightZoom;
	        public Int16    contrast;
	        public Int16    brightness;
	        public int      grayscale;          // BOOL
	        public UInt16   align;
	        public Win32.POINT	offset;
	        public Byte     scaleMethod;
	        public int		round;
	        public int      autoportrait;       // BOOL
	        public int      autoeffect;         // BOOL
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_FIELDNAMELEN+1)]   public String field;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]   public Byte[] reserved;
        }

 
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct UNITBAR2
        {
	        public int		type;
	        public int		size;
	        public int		option;
	        public Win32.SIZE	size2D;			// bar2DWidth, bar2DHeight
	        public int      barColor;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_FIELDNAMELEN+1)]   public String field;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]   public Byte[] reserved;
        }



        [StructLayout(LayoutKind.Explicit, CharSet = CharSet.Unicode)]
        public struct UNITINFO2_UNION
        {
            [FieldOffset(0)]    public UNITTEXT2    txt;
            [FieldOffset(0)]    public UNITIMAGE2   img;
            [FieldOffset(0)]    public UNITBAR2     bar;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct UNITINFO2
        {
	        public int		ver;    // = UNITINFO2_VER_2
	        public int		index;	// zero-based index from ElementList...
	        public int		type;	// this is not EQUAL with CGObj's one.
							        // objType can have the value within UNITTYPE_TEXT, UNITTYPE_IMAGE and UNITTYPE_BARCODE.
	        public Byte		page;
	        public Byte		panel;
	        public int		left;			// offsetLeft
	        public int		top;			// offsetTop
	        public int		width;
	        public int		height;
	        public int		rotate;
	        public S70Border		border;
	        public S70BackGround	back;
            public UNITINFO2_UNION  union;      // union.txt, union.img, union.bar
	        public S70Laser			laser;
        }


        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct S70CBParam
        {
            public int jobid;
            public int res;
            public Byte mod;
        }

        // job finish notify callback
        public delegate uint S70CBPROC(IntPtr pcbres_ptr);  //S70CBParam* pcbres

        // error callback
        public delegate uint SM70CB_ERROR(IntPtr hsmart, int jobid, Byte mod, int nres); // HSMART70, int, BYTE, int

        // magnetic encoding callback 
        public delegate uint SM70CB_MAGNETIC(IntPtr hsmart, int jobid, Byte mod);    //HSMART70 handle, int jobid, BYTE mid
        
        // contact smart card encoding callback 
        public delegate uint SM70CB_CONTACT(IntPtr hsmart, int jobid, Byte mod, IntPtr pATR_ptr, int nATRLen);  //HSMART70 handle, int jobid, BYTE mid, BYTE *pATR, int nATRLen;

        // contactless smart card encoding callback 
        public delegate uint SM70CB_CONTACTLESS(IntPtr hsmart, int jobid, Byte mod, IntPtr pATR_ptr, int nATRLen);  //HSMART70 handle, int jobid, BYTE mid, BYTE *pATR, int nATRLen



        // scan image callback 
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct IMAGE_INFO
        {
	        public int bpp;
	        public int width;
	        public int height;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct SCANIMAGE_RESULT
        {
	        public IntPtr       ptop_ptr;           //BYTE *	
	        public int		    ntoplen;
	        public IntPtr       pbottom_ptr;        //BYTE *	
	        public int		    nbottomlen;
	        public IMAGE_INFO	topinf;
	        public IMAGE_INFO	bottominf;
        }

        public delegate uint SM70CB_SCANIMAGE(IntPtr hsmart, int jobid, Byte mod, IntPtr presult_ptr); //HSMART70 handle, int jobid, BYTE mid, SCANIMAGE_RESULT * presult


        // scan barcode callback 
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct SCANBARCODE_RESULT
        {
	        public IntPtr   msztop_ptr;			// WCHAR *. multi null-terminated string of barcode text in top side.
	        public IntPtr   mszbottom_ptr;		// WCHAR *. multi null-terminated string of barcode text in bottom side.
        }

        public delegate uint SM70CB_SCANBARCODE(IntPtr hsmart, int jobid, Byte mod, IntPtr presult_ptr); //HSMART70 handle, int jobid, BYTE mid, SCANBARCODE_RESULT * presult

        // print a page callback 
        public delegate uint SM70CB_PRINT(IntPtr hsmart, int jobid, Byte mod, int page);   //HSMART70 handle, int jobid, BYTE mid, int page

        // laminate a page callback 
        public delegate uint SM70CB_LAMINATE(IntPtr hsmart, int jobid, Byte mod, int page);    //HSMART70 handle, int jobid, BYTE mid, int page

        // user callback 
        public delegate uint SM70CB_USER(IntPtr hsmart, int jobid, Byte mod, int opt); //HSMART70 handle, int jobid, BYTE mid, int opt




        public const int JOB_WAIT		= 0;
        public const int JOB_MOVE		= 1;
        public const int JOB_PRINT		= 2;
        public const int JOB_FLIP		= 3;
        public const int JOB_LAMINATE	= 4;
        public const int JOB_ERROR		= 5;
        public const int JOB_LASER		= 6;

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode, Pack=1)]
        public struct JOBINFO
        {
	        public int	jobid;		// job-id for request information, and you must fill this before request.
	        public int  work;		// job's current working info.	JOB_WAIT, ...
	        public Byte module;	    // module-id where the card is.
	        public int	remain;		// the number of remain jobs in job pool, EXCEPT jobid.
				        	        // when the 'jobid' value is 0 'work' and 'module' does not have any information and
				        	        // 'remain' is ENTIRE jobs count.
        }


        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode, Pack=1)]
        public struct JOBINFO_EX
        {
	        public int	jobid;		// job-id for request information, and you must fill this before request.
            public int  work;		// job's current working info.	JOB_WAIT, ...
            public int  opt;		// print option
            public Byte module;	    // module-id where the card is.
            public IntPtr hsmart;
            public int  cancel;	    // user requests cancel.
	        public int	remain;		// the number of remain jobs in job pool, EXCEPT jobid.
					                // when the 'jobid' value is 0 'work' and 'module' does not have any information and
					                // 'remain' is ENTIRE jobs count.
        }



        // card insert, move, eject.
        public const int S7JD_MOVE			= 1;

        public const Byte S7JD_MOVE_OPT_MIHAUTO	= 0x0F;	// Multi-Input-Hopper Auto
        public const Byte S7JD_MOVE_OPT_MOHAUTO	= 0x0F;	// Multi-Output-Hopper Auto
        public const Byte S7JD_MOVE_OPT_EJECT	= 0xFF;

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct S7JD_PARAM_MOVE
        {
	        public Byte	from_mid;			// source module-id to move
	        public Byte	from_opt;			// option for from_mid
	        public Byte	to_mid;				// target module-id to move
	        public Byte	to_opt;				// optio nfor to_mid
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]   public Byte[] reserved;
        }


        // card flip
        public const int S7JD_FLIP			= 2;

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct S7JD_PARAM_FLIP
        {
	        public Byte	from_mid;			// source module-id to move for flip
	        public Byte	flip_mid;			// module-id of flipper
	        public Byte	to_mid;				// target module-id to move after flip
	        public Byte	opt;				// flip option
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]   public Byte[] reserved;
        }


        // magnetic encoding
        public const int S7JD_MAGNETIC		= 3;

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct S7JD_PARAM_MAGNETIC
        {
	        public Byte	mod;				// module-id of magnetic encoder
	        public int	coer;				// coercivity value
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]   public Byte[] reserved;
            public IntPtr	pfncb;		    // SM70CB_MAGNETIC. callback function for magnetic encoding.
								            // SDK move the card to magnetic position, configure coercivity and
								            // calls this callback function.
        }


        // contact smart card encoding
        public const int S7JD_CONTACT		= 4;

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct S7JD_PARAM_CONTACT
        {
	        public Byte	mod;			// module-id of contact smart card encoder
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]   public Byte[] reserved;
	        public IntPtr	pfncb;		// SM70CB_CONTACT. callback function for contact smart card encoding.
								        // SDK move the card to contact position, contact header down, power on and
								        // calls this callback function.
								        // after callback function SDK power off and detach contact header.
        }


        // contactless smart card encoding
        public const int S7JD_CONTACTLESS	= 5;

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct S7JD_PARAM_CONTACTLESS
        {
	        public Byte	mod;			// module-id of contactless smart card encoder
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]   public Byte[] reserved;
	        public IntPtr   pfncb;	    // SM70CB_CONTACTLESS. callback function for contactless smart card encoding.
								        // SDK move the card to contactless position, power on and
								        // calls this callback function.
								        // after callback function SDK power off.
        }


        // scan the image
        public const int S7JD_SCANIMAGE		= 6;

        public const int SCANIMAGE_OPT_TOP		= 0;		// acquire top side result only
        public const int SCANIMAGE_OPT_BOTTOM	= 1;		// acquire bottom side result only
        public const int SCANIMAGE_OPT_BOTH		= 2;		// acquire both side results

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct S7JD_PARAM_SCANIMAGE
        {
	        public Byte	mod;			// module-id of scanner
	        public int	dpi;			// scanning dpi
	        public int	mode;			// scanning color mode
	        public int	side;			// scanning side
	        public int	opt;			// option for scan
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]   public Byte[] reserved;
	        public IntPtr	pfncb;	    // SM70CB_SCANIMAGE. callback function for scan the image.
								        // SDK configure scanner, scan image and calls this callback function.
        }




        // scan the barcode
        public const int S7JD_SCANBARCODE	= 7;

        public const int SCANBARCODE_OPT_TOP	= 0;		// acquire top side result only
        public const int SCANBARCODE_OPT_BOTTOM	= 1;		// acquire bottom side result only
        public const int SCANBARCODE_OPT_BOTH	= 2;		// acquire both side results

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct S7JD_PARAM_SCANBARCODE
        {
	        public Byte	mod;			// module-id of scanner
	        public int	dpi;			// scanning dpi
	        public int	mode;			// scanning color mode
	        public int	side;			// scanning side
	        public int	opt;			// option for scan
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]   public Byte[] reserved;
	        public IntPtr	pfncb;	    // SM70CB_SCANBARCODE. callback function for scan the barcode.
								        // SDK configure scanner, scan barcodes and calls this callback function.
        }


        // print
        public const int S7JD_PRINT			= 8;

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct S7JD_PARAM_PRINT
        {
	        public Byte	mod;			// module-id of printer
	        public int	page;			// printing page
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]   public Byte[] reserved;
	        public IntPtr	pfncb;	    // SM70CB_PRINT. callback function for print a page.
								        // SDK move card to print position if necessary and calls this callback function.
								        // In print callback function, draw, make print data and send print data to printer.
								        // After callback function, SDK starts printing.
        }


        // laminate
        public const int S7JD_LAMINATE		= 9;

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct S7JD_PARAM_LAMINATE
        {
	        public Byte	mod;			// module-id of laminator
	        public int	page;			// laminating page
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]   public Byte[] reserved;
	        public IntPtr   pfncb;	    // SM70CB_LAMINATE. callback function for laminate a page.
								        // SDK laminates page and calls this callback function.
        }


        // laminate
        public const int S7JD_USER			= 10;

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct S7JD_PARAM_USER
        {
	        public Byte	mod;			// module-id of user-defined work
	        public int	opt;			// option
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]    public Byte[] reserved;
            public IntPtr   pfncb;		// SM70CB_USER. callback function for user-defined work.
        }







        // @Smart70_GetModuleState

        // Action Result
        public const byte	SS_ACT_SUCCESS		= 0x00;
        public const byte	SS_ACT_FAIL			= 0x01;
        public const byte	SS_ACT_RESULT		= 0x01;	// mask

        // Action State
        public const byte	SS_ACT_IDLE			= 0x00;
        public const byte	SS_ACT_RUN			= 0x02;
        public const byte	SS_ACT_STATE		= 0x02;	// mask

        // Card State
        public const byte	SS_CARD_ABSENT		= 0x00;
        public const byte	SS_CARD_EXIST		= 0x04;
        public const byte	SS_CARD_STATE		= 0x04;	// mask

        public const byte	SS_INIT				= 0x08;

        // Command State
        public const byte	SS_CMD_IDLE			= 0x00;
        public const byte	SS_CMD_RUN			= 0x10;
        public const byte	SS_CMD_STATE		= 0x10;	// mask

        // Command Result
        public const byte	SS_CMD_SUCCESS		= 0x00;
        public const byte	SS_CMD_FAIL			= 0x20;
        public const byte	SS_CMD_RESULT		= 0x20;	// mask



        // @Smart70_GetStatus
        //
        //  S7#S_M_... : Action flag
        //  S7#S_S_... : Status flag
        //  S7#S_F_... : Fail flag
        //  S7#S_E_... : Error flag

        // SMART-70 PRINTER STATUS
        public const Int64	S7PS_M_SBSRUNNING			= 0x0000000000000001;	// doing SBS Command
        public const Int64	S7PS_M_CARDMOVE				= 0x0000000000000002;	// card is moving
        public const Int64	S7PS_M_CARDIN				= 0x0000000000000004;	// card is inserting
        public const Int64	S7PS_M_CARDOUT				= 0x0000000000000008;	// card is ejecting
        public const Int64	S7PS_M_THEAD				= 0x0000000000000010;	// thermal head lifting
        public const Int64	S7PS_M_SEEKRIBBON			= 0x0000000000000020;	// seek ribbon
        public const Int64	S7PS_M_MOVERIBBON			= 0x0000000000000040;	// moving ribbon
        public const Int64	S7PS_M_PRINT				= 0x0000000000000080;	// printing
        public const Int64	S7PS_M_MAGRW				= 0x0000000000000100;	// magnetic encoding
        public const Int64	S7PS_M_RECVPRINTDATA		= 0x0000000000000200;	// receiving print data
        public const Int64	S7PS_M_INIT					= 0x0000000000000400;	// device initializing
        public const Int64	S7PS_S_READY				= 0x0000000000000800;	// device is ready
        public const Int64	S7PS_S_INSTALLINTENCODER	= 0x0000000000008000;	// internal encoder is installed
        public const Int64	S7PS_S_INSTALLEXTHOPPER		= 0x0000000000010000;	// external input-hopper is installed
        public const Int64	S7PS_S_INSTALLEXTSTACKER	= 0x0000000000020000;	// external output-hopper is installed
        public const Int64	S7PS_S_INSTALLEXTENCODER	= 0x0000000000040000;	// external encoder is installed
        public const Int64	S7PS_S_INSTALLEXTLAMINATOR	= 0x0000000000080000;	// external laminator is installed
        public const Int64	S7PS_S_INSTALLEXTFLIPPER	= 0x0000000000100000;	// external flipper is installed
        public const Int64	S7PS_S_INSTALLEXTETC		= 0x0000000000200000;	// external etc. module is installed
        public const Int64	S7PS_S_CASEOPEN				= 0x0000000000400000;	// case is opened
        public const Int64	S7PS_S_SOFTLOCKED			= 0x0000000000800000;	// soft locked state.
        public const Int64	S7PS_S_KEYLOCKED			= 0x0000000001000000;	// key locked state.
        public const Int64	S7PS_S_DETECTCARD			= 0x0000000002000000;	// detect a card
        public const Int64	S7PS_S_DETECTFRONTDEVICE	= 0x0000000004000000;	// module is connected to left
        public const Int64	S7PS_S_DETECTREARDEVICE		= 0x0000000008000000;	// module is connected to right
        public const Int64	S7PS_S_CLEANWARNING			= 0x0000000010000000;	// need cleaning
        public const Int64	S7PS_S_HAVEPRINTDATA		= 0x0000000020000000;	// have some printing data
        public const Int64	S7PS_S_SBSMODE				= 0x0000000040000000;	// Manual Mode
        public const Int64	S7PS_S_TESTMODE				= 0x0000000080000000;	// Test mode.
        public const Int64	S7PS_M_MASK					= (S7PS_M_SBSRUNNING|S7PS_M_CARDMOVE|S7PS_M_CARDIN|S7PS_M_CARDOUT|S7PS_M_THEAD|S7PS_M_SEEKRIBBON|S7PS_M_MOVERIBBON|S7PS_M_PRINT|S7PS_M_MAGRW|S7PS_M_INIT);
        public const Int64	S7PS_S_MASK					= (S7PS_S_INSTALLINTENCODER|S7PS_S_INSTALLEXTHOPPER|S7PS_S_INSTALLEXTSTACKER|S7PS_S_INSTALLEXTENCODER|S7PS_S_INSTALLEXTLAMINATOR|S7PS_S_INSTALLEXTFLIPPER|S7PS_S_INSTALLEXTETC|S7PS_S_CASEOPEN|S7PS_S_SOFTLOCKED|S7PS_S_KEYLOCKED|S7PS_S_DETECTCARD|S7PS_S_DETECTFRONTDEVICE|S7PS_S_DETECTREARDEVICE|S7PS_S_CLEANWARNING|S7PS_S_HAVEPRINTDATA|S7PS_S_SBSMODE|S7PS_S_TESTMODE);

        public const Int64  S7PS_E_CARDIN               = 0x0000000100000000;	// failed to insert card
        public const Int64  S7PS_E_CARDMOVE             = 0x0000000200000000;	// failed to move card
        public const Int64  S7PS_E_CARDOUT              = 0x0000000400000000;	// failed to eject card
        public const Int64  S7PS_E_THEADLIFT            = 0x0000000800000000;	// failed to lift thermal head
        public const Int64  S7PS_E_PRINT                = 0x0000004000000000;	// error while printing
        public const Int64  S7PS_E_MAGRW                = 0x0000008000000000;	// magnetic encoding error
        public const Int64  S7PS_E_MAGREADT1            = 0x0000010000000000;	// magnetic ISO track 1 read error
        public const Int64  S7PS_E_MAGREADT2            = 0x0000020000000000;	// magnetic ISO track 2 read error
        public const Int64  S7PS_E_MAGREADT3            = 0x0000040000000000;	// magnetic ISO track 3 read error
        public const Int64  S7PS_E_CONNECTEXTHOPPER     = 0x0000080000000000;	// ext. input-hopper connection failure
        public const Int64  S7PS_E_CONNECTEXTSTACKER    = 0x0000100000000000;	// ext. output-hopper connection failure
        public const Int64  S7PS_E_CONNECTEXTENCODER    = 0x0000200000000000;	// external encoder connection failure
        public const Int64  S7PS_E_CONNECTEXTLAMINATOR  = 0x0000400000000000;	// external laminator connection failure
        public const Int64  S7PS_E_CONNECTEXTFLIPPER    = 0x0000800000000000;	// external flipper connection failure
        public const Int64  S7PS_E_CONNECTEXTETC        = 0x0001000000000000;	// ext. etc. module connection failure
        public const Int64  S7PS_E_EXTPRESETMATCH       = 0x0002000000000000;	// ext. preset module connection failure
        public const Int64  S7PS_E_SCHEDULER            = 0x0020000000000000;	// scheduler run error
        public const Int64  S7PS_E_RIBBONEMPTY          = 0x0040000000000000;	// ribbon is empty
        public const Int64  S7PS_E_RIBBONSEEK           = 0x0080000000000000;	// ribbon search failure
        public const Int64  S7PS_E_RIBBONMOVE           = 0x0100000000000000;	// ribbon move failure
        public const Int64  S7PS_F_THEADABSENT          = 0x0200000000000000;	// thermal head is absent
        public const Int64  S7PS_F_THEADOVERHEAT        = 0x0400000000000000;	// thermal head is overheated
        public const Int64  S7PS_F_RIBBONABSENT         = 0x0800000000000000;	// ribbon is absent
        public const Int64  S7PS_F_PRINTDATA            = 0x1000000000000000;	// error on printing data
        public const Int64  S7PS_F_INCORRECTPASSWORRD   = 0x2000000000000000;	// wrong password
        public const Int64  S7PS_F_CONFIG               = 0x4000000000000000;	// failed to change printer config.
        public const Int64  S7PS_E_MASK                 = (S7PS_E_CARDIN | S7PS_E_CARDMOVE | S7PS_E_CARDOUT | S7PS_E_THEADLIFT | S7PS_E_PRINT | S7PS_E_MAGRW | S7PS_E_MAGREADT1 | S7PS_E_MAGREADT2 | S7PS_E_MAGREADT3 | S7PS_E_CONNECTEXTHOPPER | S7PS_E_CONNECTEXTSTACKER | S7PS_E_CONNECTEXTENCODER | S7PS_E_CONNECTEXTLAMINATOR | S7PS_E_CONNECTEXTFLIPPER | S7PS_E_CONNECTEXTETC | S7PS_E_EXTPRESETMATCH | S7PS_E_RIBBONSEEK | S7PS_E_RIBBONMOVE);
        public const Int64  S7PS_F_MASK					= (S7PS_F_THEADABSENT|S7PS_F_THEADOVERHEAT|S7PS_F_RIBBONABSENT|S7PS_F_PRINTDATA|S7PS_F_INCORRECTPASSWORRD|S7PS_F_CONFIG);

        
        // SMART-70 INPUT-HOPPER (HOPPER) STATUS

        public const Int64	S7IS_M_CARDOUT				= 0x0000000000000002;	// card ejecting
        public const Int64	S7IS_S_READY				= 0x0000000000010000;	// ready to use
        public const Int64	S7IS_M_PROCESSING			= 0x0000000000020000;	// doing command
        public const Int64	S7IS_S_CATRIDGENEAREMPTY	= 0x0000000002000000;	// almost empty cards
        public const Int64	S7IS_S_CATRIDGEEMPTY		= 0x0000000004000000;	// empty card
        public const Int64	S7IS_S_CATRIDGELOCK			= 0x0000000008000000;	// detect cartridge lock sensor
        public const Int64	S7IS_S_CATRIDGECONTACT		= 0x0000000010000000;	// detect cartridge contact sensor
        public const Int64	S7IS_S_CARDOUT				= 0x0000000020000000;	// detect card-out sensor
        public const Int64	S7IS_S_REARHOOK				= 0x0000000040000000;	// detect right hook
        public const Int64	S7IS_S_FRONTHOOK			= 0x0000000080000000;	// detect left hook
        public const Int64	S7IS_M_MASK					= (S7IS_M_CARDOUT|S7IS_M_PROCESSING);
        public const Int64	S7IS_S_MASK					= (S7IS_S_READY|S7IS_S_CATRIDGENEAREMPTY|S7IS_S_CATRIDGEEMPTY|S7IS_S_CATRIDGELOCK|S7IS_S_CATRIDGECONTACT|S7IS_S_CARDOUT|S7IS_S_REARHOOK|S7IS_S_FRONTHOOK);

        public const Int64	S7IS_E_CARDOUT				= 0x0000000200000000;	// error while eject card
        public const Int64	S7IS_F_INIT					= 0x0001000000000000;	// initializing failure
        public const Int64	S7IS_E_MASK					= (S7IS_E_CARDOUT);
        public const Int64	S7IS_F_MASK					= (S7IS_F_INIT);

        
        // SMART-70 MULTI-HOPPER STATUS

        public const Int64	S7MS_M_INIT					= 0x0000000000000001;	// initializing
        public const Int64	S7MS_M_CARDOUT				= 0x0000000000000002;	// card ejecting
        public const Int64	S7MS_S_READY				= 0x0000000000000004;	// ready to use
        public const Int64	S7MS_M_PROCESSING			= 0x0000000000000008;	// doing command
        public const Int64	S7MS_S_HOPPER1CONNECT		= 0x0000000000000010;	// detect hopper 1
        public const Int64	S7MS_S_HOPPER2CONNECT		= 0x0000000000000020;	// detect hopper 2
        public const Int64	S7MS_S_HOPPER3CONNECT		= 0x0000000000000040;	// detect hopper 3
        public const Int64	S7MS_S_HOPPER4CONNECT		= 0x0000000000000080;	// detect hopper 4
        public const Int64	S7MS_S_HOPPER5CONNECT		= 0x0000000000000100;	// detect hopper 5
        public const Int64	S7MS_S_HOPPER6CONNECT		= 0x0000000000000200;	// detect hopper 6
        public const Int64	S7MS_M_HOPPER1RUN			= 0x0000000000000400;	// hopper 1 is running
        public const Int64	S7MS_M_HOPPER2RUN			= 0x0000000000000800;	// hopper 2 is running
        public const Int64	S7MS_M_HOPPER3RUN			= 0x0000000000001000;	// hopper 3 is running
        public const Int64	S7MS_M_HOPPER4RUN			= 0x0000000000002000;	// hopper 4 is running
        public const Int64	S7MS_M_HOPPER5RUN			= 0x0000000000004000;	// hopper 5 is running
        public const Int64	S7MS_M_HOPPER6RUN			= 0x0000000000008000;	// hopper 6 is running
        public const Int64	S7MS_S_LEFTLSENSOR			= 0x0000000000040000;	// left limit sensor
        public const Int64	S7MS_S_RIGHTLSENSOR			= 0x0000000000080000;	// right limit sensor
        public const Int64	S7MS_S_SPSENSORA			= 0x0000000000100000;	// shiter position sensor A
        public const Int64	S7MS_S_SPSENSORB			= 0x0000000000200000;	// shiter position sensor B
        public const Int64	S7MS_S_SPSENSORC			= 0x0000000000400000;	// shiter position sensor C
        public const Int64	S7MS_S_SPSENSORD			= 0x0000000000800000;	// shiter position sensor D
        public const Int64	S7MS_S_CISENSORA0			= 0x0000000001000000;	// card in sensor A0
        public const Int64	S7MS_S_CISENSORA1			= 0x0000000002000000;	// card in sensor A1
        public const Int64	S7MS_S_CISENSORB0			= 0x0000000004000000;	// card in sensor B0
        public const Int64	S7MS_S_CISENSORB1			= 0x0000000008000000;	// card in sensor B1
        public const Int64	S7MS_S_CISENSORC0			= 0x0000000010000000;	// card in sensor C0
        public const Int64	S7MS_S_CISENSORC1			= 0x0000000020000000;	// card in sensor C1
        public const Int64	S7MS_S_CISENSORD0			= 0x0000000040000000;	// card in sensor D0
        public const Int64	S7MS_S_CISENSORD1			= 0x0000000080000000;	// card in sensor D1
        public const Int64	S7MS_M_MASK					= (S7MS_M_INIT|S7MS_M_CARDOUT|S7MS_M_PROCESSING|S7MS_M_HOPPER1RUN|S7MS_M_HOPPER2RUN|S7MS_M_HOPPER3RUN|S7MS_M_HOPPER4RUN|S7MS_M_HOPPER5RUN|S7MS_M_HOPPER6RUN);
        public const Int64	S7MS_S_MASK					= (S7MS_S_READY|S7MS_S_HOPPER1CONNECT|S7MS_S_HOPPER2CONNECT|S7MS_S_HOPPER3CONNECT|S7MS_S_HOPPER4CONNECT|S7MS_S_HOPPER5CONNECT|S7MS_S_HOPPER6CONNECT|S7MS_S_LEFTLSENSOR|S7MS_S_RIGHTLSENSOR|S7MS_S_SPSENSORA|S7MS_S_SPSENSORB|S7MS_S_SPSENSORC|S7MS_S_SPSENSORD|S7MS_S_CISENSORA0|S7MS_S_CISENSORA1|S7MS_S_CISENSORB0|S7MS_S_CISENSORB1|S7MS_S_CISENSORC0|S7MS_S_CISENSORC1|S7MS_S_CISENSORD0|S7MS_S_CISENSORD1);

        public const Int64	S7MS_F_INIT					= 0x0000000100000000;	// initializing fail
        public const Int64	S7MS_E_CARDOUT				= 0x0000000200000000;	// card out error
        public const Int64	S7MS_E_RUN					= 0x0000000800000000;	// failed to run command
        public const Int64	S7MS_E_HOPPER1CONTROL		= 0x0000001000000000;	// hopper 1 control error
        public const Int64	S7MS_E_HOPPER2CONTROL		= 0x0000002000000000;	// hopper 2 control error
        public const Int64	S7MS_E_HOPPER3CONTROL		= 0x0000004000000000;	// hopper 3 control error
        public const Int64	S7MS_E_HOPPER4CONTROL		= 0x0000008000000000;	// hopper 4 control error
        public const Int64	S7MS_E_HOPPER5CONTROL		= 0x0000010000000000;	// hopper 5 control error
        public const Int64	S7MS_E_HOPPER6CONTROL		= 0x0000020000000000;	// hopper 6 control error
        public const Int64	S7MS_E_HOPPER1EMPTY			= 0x0000040000000000;	// hopper 1 is empty
        public const Int64	S7MS_E_HOPPER2EMPTY			= 0x0000080000000000;	// hopper 2 is empty
        public const Int64	S7MS_E_HOPPER3EMPTY			= 0x0000100000000000;	// hopper 3 is empty
        public const Int64	S7MS_E_HOPPER4EMPTY			= 0x0000200000000000;	// hopper 4 is empty
        public const Int64	S7MS_E_HOPPER5EMPTY			= 0x0000400000000000;	// hopper 5 is empty
        public const Int64	S7MS_E_HOPPER6EMPTY			= 0x0000800000000000;	// hopper 6 is empty
        public const Int64	S7MS_E_HOPPER1				= 0x0001000000000000;	// hopper 1 has error
        public const Int64	S7MS_E_HOPPER2				= 0x0002000000000000;	// hopper 2 has error
        public const Int64	S7MS_E_HOPPER3				= 0x0004000000000000;	// hopper 3 has error
        public const Int64	S7MS_E_HOPPER4				= 0x0008000000000000;	// hopper 4 has error
        public const Int64	S7MS_E_HOPPER5				= 0x0010000000000000;	// hopper 5 has error
        public const Int64	S7MS_E_HOPPER6				= 0x0020000000000000;	// hopper 6 has error
        public const Int64	S7MS_E_MASK					= (S7MS_E_CARDOUT|S7MS_E_RUN|S7MS_E_HOPPER1CONTROL|S7MS_E_HOPPER2CONTROL|S7MS_E_HOPPER3CONTROL|S7MS_E_HOPPER4CONTROL|S7MS_E_HOPPER5CONTROL|S7MS_E_HOPPER6CONTROL|/*S7MS_E_HOPPER1EMPTY|S7MS_E_HOPPER2EMPTY|S7MS_E_HOPPER3EMPTY|S7MS_E_HOPPER4EMPTY|S7MS_E_HOPPER5EMPTY|S7MS_E_HOPPER6EMPTY|*/S7MS_E_HOPPER1|S7MS_E_HOPPER2|S7MS_E_HOPPER3|S7MS_E_HOPPER4|S7MS_E_HOPPER5|S7MS_E_HOPPER6);
        public const Int64	S7MS_F_MASK					= (S7MS_F_INIT);

        
        // SMART-70 OUTPUT-HOPPER (STACKER) STATUS

        public const Int64	S7OS_M_CARDIN				= 0x0000000000000001;	// card inserting
        public const Int64	S7OS_M_LIFTUP				= 0x0000000000000004;	// lifting up
        public const Int64	S7OS_M_LIFTDOWN				= 0x0000000000000008;	// lifting down
        public const Int64	S7OS_S_READY				= 0x0000000000010000;	// ready state
        public const Int64	S7OS_M_PROCESSING			= 0x0000000000020000;	// doing command
        public const Int64	S7OS_S_CARDSTANDBY			= 0x0000000002000000;	// detect card standby sensor
        public const Int64	S7OS_S_CATRIDGEHAVESPACE	= 0x0000000004000000;	// output hopper is full
        public const Int64	S7OS_S_CATRIDGELOCK			= 0x0000000008000000;	// detect cartridge lock sensor
        public const Int64	S7OS_S_CATRIDGECONTACT		= 0x0000000010000000;	// detect cartridge contact sensor
        public const Int64	S7OS_S_CARDINENABLER		= 0x0000000020000000;	// detect able to insert card sensor
        public const Int64	S7OS_S_LIFTUP				= 0x0000000040000000;	// detect a card from in sensor
        public const Int64	S7OS_S_DETECTCARDIN			= 0x0000000080000000;	// lifted up
        public const Int64	S7OS_M_MASK					= (S7OS_M_CARDIN|S7OS_M_LIFTUP|S7OS_M_LIFTDOWN|S7OS_M_PROCESSING);
        public const Int64	S7OS_S_MASK					= (S7OS_S_READY|S7OS_M_PROCESSING|S7OS_S_CARDSTANDBY|S7OS_S_CATRIDGEHAVESPACE|S7OS_S_CATRIDGELOCK|S7OS_S_CATRIDGECONTACT|S7OS_S_CARDINENABLER|S7OS_S_LIFTUP|S7OS_S_DETECTCARDIN);

        public const Int64	S7OS_E_CARDIN				= 0x0000000100000000;	// error while insert card
        public const Int64	S7OS_E_LIFTUP				= 0x0000000400000000;	// error while lift up
        public const Int64	S7OS_E_LIFTDOWN				= 0x0000000800000000;	// error while lift down
        public const Int64	S7OS_F_INIT					= 0x0001000000000000;	// initializing failure
        public const Int64	S7OS_E_MASK					= (S7OS_E_CARDIN|S7OS_E_LIFTUP|S7OS_E_LIFTDOWN);
        public const Int64	S7OS_F_MASK					= (S7OS_F_INIT);

        
        // SMART-70 FLIPPER STATUS

        public const Int64	S7FS_M_CARDIN				= 0x0000000000000001;	// card inserting
        public const Int64	S7FS_M_CARDOUT				= 0x0000000000000002;	// card ejecting
        public const Int64	S7FS_M_FLIPPING				= 0x0000000000000004;	// card flipping
        public const Int64	S7FS_M_CARDMOVE				= 0x0000000000000008;	// card moving
        public const Int64	S7FS_S_SMARTREADERDOWN		= 0x0000000000000010;	// ic contactor contacted (board Ver.2)
        public const Int64	S7FS_S_SMARTREADERUP		= 0x0000000000000020;	// ic contactor released (board Ver.2)
        public const Int64	S7FS_S_EXTBOARDABSENT		= 0x0000000000000100;	// detect extended board connect sensor (board Ver.2)
        public const Int64	S7FS_S_DETECTCARDSCANIN		= 0x0000000000000200;	// detect a card at scanner in sensor (board Ver.2)
        public const Int64	S7FS_S_DETECTCARDSCANOUT	= 0x0000000000000400;	// detect a card at scanner out sensor (board Ver.2)
        public const Int64	S7FS_S_SMARTREADERCAMSENSOR	= 0x0000000000000800;	// detect ic contactor (board Ver.2)
        public const Int64	S7FS_S_NOTINSTALLSCANMOD	= 0x0000000000001000;	// not installed scanner module (board Ver.2)
        public const Int64	S7FS_S_READY				= 0x0000000000010000;	// ready state
        public const Int64	S7FS_M_PROCESSING			= 0x0000000000020000;	// doing command
        public const Int64	S7FS_S_V2BOARD				= 0x0000000000200000;	// board ver. 2
        public const Int64	S7FS_S_FLIPPERBOTTOM		= 0x0000000000400000;	// faced to bottom
        public const Int64	S7FS_S_FLIPPERTOP			= 0x0000000000800000;	// faced to top
        public const Int64	S7FS_S_CATRIDGELOCK			= 0x0000000001000000;	// detect cartridge lock sensor
        public const Int64	S7FS_S_CASEOPENSENSOR		= 0x0000000002000000;	// detect case open sensor. (board Ver.2)
        public const Int64	S7FS_S_CATRIDGEFULLSENSOR	= 0x0000000004000000;	// cartridge is full
        public const Int64	S7FS_S_PCSVERTICALSENSOR	= 0x0000000008000000;	// Vertical Position Control Sensor
        public const Int64	S7FS_S_PCSHORIZONTALSENSOR	= 0x0000000010000000;	// Horizontal Position Control Sensor
        public const Int64	S7FS_S_DETECTCARDRIGHT		= 0x0000000020000000;	// Detect Card on Right Sensor
        public const Int64	S7FS_S_DETECTCARDCENTER		= 0x0000000040000000;	// Detect Card on Center Sensor
        public const Int64	S7FS_S_DETECTCARDLEFT		= 0x0000000080000000;	// Detect Card on Left Sensor
        public const Int64	S7FS_M_MASK					= (S7FS_M_CARDIN|S7FS_M_CARDOUT|S7FS_M_FLIPPING|S7FS_M_PROCESSING);

        public const Int64	S7FS_E_CARDIN				= 0x0000000100000000;	// card insert error
        public const Int64	S7FS_E_CARDOUT				= 0x0000000200000000;	// card eject error
        public const Int64	S7FS_E_FLIP					= 0x0000000400000000;	// flipping error
        public const Int64	S7FS_E_CARDMOVE				= 0x0000000800000000;	// card moving error
        public const Int64	S7FS_E_SMARTREADERDOWN		= 0x0000001000000000;	// ic contactor contact error (board Ver.2)
        public const Int64	S7FS_E_SMARTREADERUP		= 0x0000002000000000;	// ic contactor release error (board Ver.2)
        public const Int64	S7FS_F_INIT					= 0x0001000000000000;	// initialize failure
        public const Int64	S7FS_E_MASK					= (S7FS_E_CARDIN|S7FS_E_CARDOUT|S7FS_E_FLIP);
        public const Int64	S7FS_F_MASK					= (S7FS_F_INIT);

        
        // SMART-70 LAMINATOR STATUS

        public const Int64	S7LS_M_CARDIN				= 0x0000000000000001;	// card inserting
        public const Int64	S7LS_M_CARDMOVE				= 0x0000000000000002;	// card moving
        public const Int64	S7LS_M_CARDOUT				= 0x0000000000000004;	// card ejecting
        public const Int64	S7LS_M_THEADUP				= 0x0000000000000008;	// thermal head lifting up
        public const Int64	S7LS_M_THEADDOWN			= 0x0000000000000010;	// thermal head lifting down
        public const Int64	S7LS_M_THEADHEAT			= 0x0000000000000020;	// thermal head heating
        public const Int64	S7LS_M_LAMINATING			= 0x0000000000000040;	// laminating
        public const Int64	S7LS_S_INSTALLEXTHOPPER		= 0x0000000000000080;	// install ext. hopper
        public const Int64	S7LS_S_INSTALLEXTSTACKER	= 0x0000000000000100;	// install ext. stacker
        public const Int64	S7LS_S_INSTALLEXTFLIPPER	= 0x0000000000000200;	// install ext. flipper
        public const Int64	S7LS_S_READY				= 0x0000000000010000;	// ready state
        public const Int64	S7LS_M_PROCESSING			= 0x0000000000020000;	// doing command
        public const Int64	S7LS_M_PROCESSUSBCMD		= 0x0000000000040000;	// doing USB command
        public const Int64	S7LS_S_CASEOPEN				= 0x0000000000100000;	// Case is opened
        public const Int64	S7LS_S_CATRIDGELOCK			= 0x0000000000200000;	// Detect Cartridge Lock Sensor
        public const Int64	S7LS_S_CARDOUT				= 0x0000000000400000;	// Detect a card from out sensor
        public const Int64	S7LS_S_CARDCENTERB			= 0x0000000000800000;	// Detect a card from center sensor B
        public const Int64	S7LS_S_CARDCENTERA			= 0x0000000001000000;	// Detect a card from center sensor A
        public const Int64	S7LS_S_CARDIN				= 0x0000000002000000;	// Detect a card from in sensor
        public const Int64	S7LS_S_THEAD				= 0x0000000004000000;	// Detect head lift sensor
        public const Int64	S7LS_S_CASEOPENSENSOR		= 0x0000000008000000;	// Detect Case Open Check Sensor
        public const Int64	S7LS_S_FILMMARKCHECKB		= 0x0000000010000000;	// Film Mark Check Sensor B
        public const Int64	S7LS_S_FILMMARKCHECKA		= 0x0000000020000000;	// Film Mark Check Sensor A
        public const Int64	S7LS_S_ENCODERB				= 0x0000000040000000;	// Encoder Sensor B
        public const Int64	S7LS_S_ENCODERA				= 0x0000000080000000;	// Encoder Sensor A
        public const Int64	S7LS_M_MASK					= (S7LS_M_CARDIN|S7LS_M_CARDMOVE|S7LS_M_CARDOUT|S7LS_M_THEADUP|S7LS_M_THEADDOWN|S7LS_M_THEADHEAT|S7LS_M_LAMINATING|S7LS_M_PROCESSING);
        public const Int64	S7LS_S_MASK					= (S7LS_S_READY|S7LS_S_CASEOPEN|S7LS_S_CATRIDGELOCK|S7LS_S_CARDOUT|S7LS_S_CARDCENTERB|S7LS_S_CARDCENTERA|S7LS_S_CARDIN|S7LS_S_THEAD|/*S7LS_S_CASEOPENSENSOR|*/S7LS_S_FILMMARKCHECKB|S7LS_S_FILMMARKCHECKA|S7LS_S_ENCODERB|S7LS_S_ENCODERA);

        public const Int64	S7LS_E_CARDIN				= 0x0000000100000000;	// error while insert card
        public const Int64	S7LS_E_CARDMOVE				= 0x0000000200000000;	// error while move card
        public const Int64	S7LS_E_CARDOUT				= 0x0000000400000000;	// error while eject card
        public const Int64	S7LS_E_THEADUP				= 0x0000000800000000;	// error while lift up header
        public const Int64	S7LS_E_THEADDOWN			= 0x0000001000000000;	// error while lift down header
        public const Int64	S7LS_E_LAMINATING			= 0x0000002000000000;	// error while laminating
        public const Int64	S7LS_E_INSTALLEXTHOPPER		= 0x0000008000000000;	// ext. flipper connection failure
        public const Int64	S7LS_E_INSTALLEXTSTACKER	= 0x0000010000000000;	// ext. stacker connection failure
        public const Int64	S7LS_E_INSTALLEXTFLIPPER	= 0x0000020000000000;	// ext. flipper connection failure
        public const Int64	S7LS_F_INIT					= 0x0001000000000000;	// initialize failure
        public const Int64	S7LS_E_PROCESSING			= 0x0002000000000000;	// error while process command
        public const Int64	S7LS_E_PROCESSUSBCMD		= 0x0004000000000000;	// error while process USB command
        public const Int64	S7LS_F_FILMEMPTY			= 0x0010000000000000;	// laminate film empty
        public const Int64	S7LS_F_FILMSEEK				= 0x0020000000000000;	// film seek failure
        public const Int64	S7LS_F_THEAD				= 0x0040000000000000;	// thermal head error
        public const Int64	S7LS_F_FILMABSENT			= 0x0080000000000000;	// laminate film is absent
        public const Int64	S7LS_E_MASK					= (S7LS_E_CARDIN|S7LS_E_CARDMOVE|S7LS_E_CARDOUT|S7LS_E_THEADUP|S7LS_E_THEADDOWN|S7LS_E_PROCESSING|S7LS_E_LAMINATING);
        public const Int64	S7LS_F_MASK					= (S7LS_F_INIT|S7LS_F_FILMEMPTY|S7LS_F_FILMSEEK|S7LS_F_THEAD|S7LS_F_FILMABSENT);

        
        // SMART-70 LASER STATUS

        public const Int64	S7XS_M_CARDIN				= 0x0000000000000001;	// card is inserting
        public const Int64	S7XS_M_CARDOUT				= 0x0000000000000002;	// card is ejecting
        public const Int64	S7XS_M_FLIPPING				= 0x0000000000000004;	// flipping
        public const Int64	S7XS_M_CARDMOVE				= 0x0000000000000008;	// card is moving
        public const Int64	S7XS_M_ICHLIFT				= 0x0000000000000010;	// contact smartcard header is lifting
        public const Int64	S7XS_M_MAGRW				= 0x0000000000000020;	// magnetic working
        public const Int64	S7XS_S_NOICCAM				= 0x0000000000000800;	// there is no contact smart reader CAM Sensor
        public const Int64	S7XS_S_LE					= 0x0000000000008000;	// device is SMART-70LE
        public const Int64	S7XS_S_READY				= 0x0000000000010000;	// ready for use
        public const Int64	S7XS_M_PROCESSING			= 0x0000000000020000;	// working
        public const Int64	S7XS_S_AUTOCARDIN			= 0x0000000000040000;	// automatic card insertion is set
        public const Int64	S7XS_S_RECOVERING			= 0x0000000000080000;	// booting or recovering after door close
        public const Int64	S7XS_S_FLPBOTTOM			= 0x0000000000100000;	// flipper is bottom faced
        public const Int64	S7XS_S_FLPTOP				= 0x0000000000200000;	// flipper is top faced
        public const Int64	S7XS_S_CASEOPEN				= 0x0000000001000000;	// door is opened
        public const Int64	S7XS_S_DETECTOPTOUT			= 0x0000000004000000;	// detect card from option-out-sensor
        public const Int64	S7XS_S_DETECTFLPREAR		= 0x0000000008000000;	// detect card from flipper-rear-sensor
        public const Int64	S7XS_S_DETECTOPTIN			= 0x0000000010000000;	// detect card from option-in-sensor
        public const Int64	S7XS_S_DETECTCARDOUT		= 0x0000000020000000;	// detect card from card-out-sensor
        public const Int64	S7XS_S_DETECTFLPCENTER		= 0x0000000040000000;	// detect card from flipper-center-sensor
        public const Int64	S7XS_S_DETECTCARDIN			= 0x0000000080000000;	// detect card from card-in-sensor
        public const Int64	S7XS_M_MASK					= (S7XS_M_CARDIN|S7XS_M_CARDOUT|S7XS_M_FLIPPING|S7XS_M_CARDMOVE|S7XS_M_ICHLIFT|S7XS_M_MAGRW|S7XS_M_PROCESSING);
        public const Int64	S7XS_S_MASK					= (S7XS_S_READY|S7XS_S_AUTOCARDIN|S7XS_S_FLPBOTTOM|S7XS_S_FLPTOP|S7XS_S_CASEOPEN|S7XS_S_DETECTOPTOUT|S7XS_S_DETECTFLPREAR|S7XS_S_DETECTOPTIN|S7XS_S_DETECTCARDOUT|S7XS_S_DETECTFLPCENTER|S7XS_S_DETECTCARDIN);

        public const Int64	S7XS_E_CARDIN				= 0x0000000100000000;	// failed to insert card
        public const Int64	S7XS_E_CARDOUT				= 0x0000000200000000;	// failed to eject card
        public const Int64	S7XS_E_CARDFLIP				= 0x0000000400000000;	// failed to flip
        public const Int64	S7XS_E_CARDMOVE				= 0x0000000800000000;	// failed to move card
        public const Int64	S7XS_E_ICHLIFT				= 0x0000001000000000;	// failed to contact smartcard header lift
        public const Int64	S7XS_E_MAGRW				= 0x0000002000000000;	// failed to magnetic write/read
        public const Int64	S7XS_F_INIT					= 0x0001000000000000;	// initialization error
        public const Int64	S7XS_E_MASK					= (S7XS_E_CARDIN|S7XS_E_CARDOUT|S7XS_E_CARDFLIP|S7XS_E_CARDMOVE|S7XS_E_ICHLIFT|S7XS_E_MAGRW);
        public const Int64	S7XS_F_MASK					= (S7XS_F_INIT);






        public const int	SMART70_COMMAND_WAITFINISH	= 0x00000000;
        public const int	SMART70_COMMAND_ONLY		= 0x00000001;
        public const int	SMART70_PRINT_EJECT			= 0x00000000;
        public const int	SMART70_PRINT_HOLD			= 0x00000010;
        public const int	SMART70_PRINT_PRNONLY		= 0x00000020;		// for internal use only. you cannot use SMART70_PRINT_PRNONLY and SMART70_PRINT_HOLD both.
        public const int	SMART70_WAIT_PRINTING		= 0x00000100;		// @Smart70_SpoolPrint.  Wait while printing. Using with SMART70_COMMAND_ONLY, it will be ignored.
        public const int	SMART70_WAIT_LAMINATING		= 0x00000200;		// @Smart70_SpoolPrint.  Wait while laminating. Using with SMART70_COMMAND_ONLY, it will be ignored.

        public const int	SMART70_LEFTSIDE			= 0;	// left : front 
        public const int	SMART70_RIGHTSIDE			= 1;	// right : rear


        // @Smart70_CardMove2
        public const byte	SMART70_MHOP_AUTO			= 0;
        public const byte	SMART70_MHOP_HOP1			= 0x10;
        public const byte	SMART70_MHOP_HOP2			= 0x20;
        public const byte	SMART70_MHOP_HOP3			= 0x30;
        public const byte	SMART70_MHOP_HOP4			= 0x40;
        public const byte	SMART70_MHOP_HOP5			= 0x50;
        public const byte	SMART70_MHOP_HOP6			= 0x60;

        // @Smart70_CardEject2
        public const int	SMART70_EJECT_MHOP_LEFT		= 0x00000000;		// eject option : multi-hopper eject to left side
        public const int	SMART70_EJECT_MHOP_RIGHT	= 0x00000001;		// eject option : multi-hopper eject to right side
        public const int	SMART70_EJECT_MHOP_SIDE		= 0x00000001;		// eject option : multi-hopper side mask
        public const int	SMART70_EJECT_MHOP_HOP1		= 0x00010000;		// eject option : multi-hopper hopper #1
        public const int	SMART70_EJECT_MHOP_HOP2		= 0x00020000;		// eject option : multi-hopper hopper #2
        public const int	SMART70_EJECT_MHOP_HOP3		= 0x00030000;		// eject option : multi-hopper hopper #3
        public const int	SMART70_EJECT_MHOP_HOP4		= 0x00040000;		// eject option : multi-hopper hopper #4
        public const int	SMART70_EJECT_MHOP_HOPPER	= 0x00070000;		// eject option : multi-hopper hopper mask

        // @Smart70_CardMovePosition2
        public const int	SMART70_OPT_MOVEHOLD		= 0x00010000;		// hold the moter after move




        // Encoder Config
        public const int	SMART70_ENCCFG_MAG_COER		= 0;		// [set] set magnetic coercivity.
        public const int	SMART70_ENCCFG_MIRU_IMAGE	= 10;		// [get/set] miru image config (set:volatile)
        public const int	SMART70_ENCCFG_MIRU			= 11;		// [get] get MIRU config.
        public const int	SMART70_ENCCFG_MIRU_GAIN	= 12;		// [set]
        public const int	SMART70_ENCCFG_MIRU_OFFSET	= 13;		// [set]
        public const int	SMART70_ENCCFG_MIRU_SPEED	= 14;		// [set]
        public const int	SMART70_ENCCFG_MIRU_SHADING	= 15;		// [set]

        // Encoder Specific Command - MIRU
        public const int	SMART70_ENC_MIRU_VERSION	= 10;		// [get] get version of miru
        public const int	SMART70_ENC_MIRU_IMAGESAVE	= 11;		// [set] miru image config save
        public const int	SMART70_ENC_MIRU_IMAGEREAD	= 12;		// [get +bulk] read and get image
        public const int	SMART70_ENC_MIRU_READTOPBAR	= 13;		// [get] get top side bar-codes
        public const int	SMART70_ENC_MIRU_READBOTBAR	= 14;		// [get] get bottom side bar-codes
        public const int	SMART70_ENC_MIRU_SCANAREA	= 15;		// [set] set the scanning area.
        public const int	SMART70_ENC_MIRU_RESET		= 16;		// [set] reset the scanner.
        public const int	SMART70_ENC_MIRU_DOSHADING	= 17;		// [get] do shading and get the result.
        public const int	SMART70_ENC_MIRU_RESTORE	= 18;		// [get] restore defaults and get the result.



        public const int MIRU_STATUS_DPI_300     = 300;
        public const int MIRU_STATUS_DPI_600     = 600;

        public const int MIRU_STATUS_MODE_GRAY   = 1;
        public const int MIRU_STATUS_MODE_COLOR  = 3 ;      

        public const int MIRU_STATUS_SIDE_SINGLE = 1;     
        public const int MIRU_STATUS_SIDE_DUAL   = 2;

        public const int MIRU_STATUS_FACE_TOP    = 0;
        public const int MIRU_STATUS_FACE_BOTTOM = 1;

        public const int MIRU_STATUS_DROP_NONE   = 0;
        public const int MIRU_STATUS_DROP_BLUE   = 1;
        public const int MIRU_STATUS_DROP_GREEN  = 2;
        public const int MIRU_STATUS_DROP_RED    = 3;
        public const int MIRU_STATUS_DROP_IR     = 4;

        public const int MIRU_STATUS_GAIN_MIN    = 0;
        public const int MIRU_STATUS_GAIN_MAX    = 63;

        public const int MIRU_STATUS_OFFSET_MIN  = 0;
        public const int MIRU_STATUS_OFFSET_MAX  = 511;

        public const int MIRU_STATUS_SPEED_MIN   = 7400;
        public const int MIRU_STATUS_SPEED_MAX   = 59200;

        public const int MIRU_SHADING_OFF        = 0;
        public const int MIRU_SHADING_ON         = 1;


        
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct MIRU_IMAGE_CONFIG
        {
            public int     dpi;
            public int     mode;
            public int     side;
            public int     face;
            public int     drop;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct MIRU_CONFIG
        {
	        public int	dpi;
	        public int	mode;
	        public int	side;
	        public int	face;
	        public int	drop;
	        public int	width;
	        public int	height;
	        public int	gain;
	        public int	offset;
	        public int	speed;
	        public int	state;
	        public int	cline;
	        public int	sline;
	        public int	b_trigger;
	        public int	b_shading;
	        public int	b_monitoring;
        }



        // Encoder Track
        public const byte	SMART70_ENC_TRACK_MS1		= 0x01;
        public const byte	SMART70_ENC_TRACK_MS2		= 0x02;
        public const byte	SMART70_ENC_TRACK_MS3		= 0x04;
        public const byte	SMART70_ENC_TRACK_JIS		= 0x08;
        public const byte	SMART70_ENC_TRACK_BAR		= 0x10;

        // Encoder Option
        public const int	SMART70_ENC_OPTION_NORMAL	= 0x00;
        public const int	SMART70_ENC_OPTION_BACKWARD	= 0x40;
        public const int	SMART70_ENC_OPTION_BITMODE	= 0x80;

        // Magnetic Coercivity : @SMART70_ENCCFG_MAG_COER
        public const int	SMART70_ENC_MAGCOERCIVITY_LOCO	= 200;
        public const int	SMART70_ENC_MAGCOERCIVITY_HICO	= 600;
        public const int	SMART70_ENC_MAGCOERCIVITY_SPCO	= 800;

        public const int	SMART70_ENC_TRACK_MS1_MAX	= 76;
        public const int	SMART70_ENC_TRACK_MS2_MAX	= 37;
        public const int	SMART70_ENC_TRACK_MS3_MAX	= 104;
        public const int	SMART70_ENC_TRACK_JIS_MAX	= 69;







        // DEVICE LIST/INFO

        [DllImport("SmartComm70.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "Smart70_GetDeviceList")]
        public static extern uint GetDeviceList(IntPtr list_ptr);     // SMART70_PRINTER_LIST*

        [DllImport("SmartComm70.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "Smart70_ExGetDeviceList")]
        public static extern uint ExGetDeviceList(IntPtr list_ptr, int opt);    // SMART70_PRINTER_LIST*, int

        [DllImport("SmartComm70.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "Smart70_GetDeviceInfo")]
        public static extern uint GetDeviceInfo(IntPtr info_ptr, IntPtr szdev_ptr, int ndevtype);    // SMART70_PRINTER_INFO*, WCHAR*, int




        // OPEN/CLOSE

        [DllImport("SmartComm70.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "Smart70_OpenDevice")]
        public static extern uint OpenDevice(ref IntPtr phsmart_ptr, IntPtr szdev_ptr, int ndevtype);    // HSMART70* pHsmart, WCHAR* szdev, int ndevtype

        [DllImport("SmartComm70.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "Smart70_ExOpenDevice")]
        public static extern uint OpenDevice2(ref IntPtr phsmart_ptr, IntPtr szdev_ptr, int ndevtype);    // HSMART70* pHsmart, WCHAR* szdev, int ndevtype

        [DllImport("SmartComm70.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "Smart70_CloseDevice")]
        public static extern uint CloseDevice(IntPtr hsmart_ptr);                                       // HSMART70 hsmart


        [DllImport("SmartComm70.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "Smart70DCL_OpenDevice")]
        public static extern uint DCLOpenDevice(ref IntPtr phsmart_ptr, IntPtr szdev_ptr, int ndevtype, int orientation);    // HSMART70* pHsmart, WCHAR* szdev, int ndevtype, int orientation

        [DllImport("SmartComm70.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "Smart70DCL_ExOpenDevice")]
        public static extern uint DCLOpenDevice2(ref IntPtr phsmart_ptr, IntPtr szdev_ptr, int ndevtype, int orientation);    // HSMART70* pHsmart, WCHAR* szdev, int ndevtype, int orientation

        [DllImport("SmartComm70.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "Smart70DCL_OpenDevice3")]
        public static extern uint DCLOpenDevice3(ref IntPtr phsmart_ptr, IntPtr szdev_ptr, int port, int bSSL, int orientation);    // HSMART70* pHsmart, WCHAR* szdev, int port, BOOL bSSL, int nOrientation

        [DllImport("SmartComm70.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "Smart70DCL_CloseDevice")]
        public static extern uint DCLCloseDevice(IntPtr phsmart_ptr);                                                       // HSMART70 hsmart

        
        
        
        // COMMON INFO.

        [DllImport("SmartComm70.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "Smart70_GetConnectedModules")]
        public static extern uint GetConnectedModules(IntPtr hsmart_ptr, IntPtr connlist_ptr);    // HSMART70, SMART70_DEVCONNLIST*
    
        [DllImport("SmartComm70.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "Smart70_GetRibbonInfo")]
        public static extern uint GetRibbonInfo(IntPtr hsmart_ptr, Byte mod, IntPtr type_ptr, IntPtr remain_ptr, IntPtr max_ptr);    // HSMART70, BYTE, int*, int*, int*
    
        [DllImport("SmartComm70.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "Smart70_GetDisplayInfo")]
        public static extern uint GetDisplayInfo(IntPtr hsmart_ptr, IntPtr pntype_ptr, IntPtr pnlang_ptr);    // HSMART70, int* pnType, int* pnLang
    
        [DllImport("SmartComm70.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "Smart70_GetSystemInfo")]
        public static extern uint GetSystemInfo(IntPtr hsmart_ptr, IntPtr psysinfo_ptr);    // HSMART70, SMART_SYSINFO* pSysInfo

    
        [DllImport("SmartComm70.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "Smart70_GetModuleState")]
        public static extern uint GetModuleState(IntPtr hsmart, Byte mod, IntPtr pstate_ptr);    // HSMART70, BYTE mod, BYTE * pstate (=BYTE pstate[1])
    
        [DllImport("SmartComm70.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "Smart70_GetStatus")]
        public static extern uint GetStatus(IntPtr hsmart, Byte mod, IntPtr pstatus_ptr);    // HSMART70, BYTE mod, __int64 *pstatus
    
        [DllImport("SmartComm70.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "Smart70_ExGetTemperature")]
        public static extern uint GetTemperature(IntPtr hsmart, IntPtr ntemperature_ptr, IntPtr prcolorvalue);    // HSMART70, short* nTmp, short* nRibColVal
    

        [DllImport("SmartComm70.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "Smart70_ExGetUserIssueCount")]
        public static extern uint GetUserIssueCount(IntPtr hsmart, IntPtr pbuf_ptr, IntPtr nlen_ptr);    // HSMART70, BYTE* pBuf, int* nLen

        [DllImport("SmartComm70.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "Smart70_ExGetFactIssueCount")]
        public static extern uint GetFactIssueCount(IntPtr hsmart, IntPtr pbuf_ptr, IntPtr nlen_ptr);    // HSMART70, BYTE* pBuf, int* nLen

        
        

        // DOCUMENT
    
        [DllImport("SmartComm70.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "Smart70_OpenDocument")]
        public static extern uint OpenDocument(IntPtr hsmart_ptr, IntPtr csd_ptr);    // HSMART70, WCHAR*

        [DllImport("SmartComm70.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "Smart70_CloseDocument")]
        public static extern uint CloseDocument(IntPtr hsmart_ptr);    // HSMART70

        [DllImport("SmartComm70.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "Smart70_ClearDocument")]
        public static extern uint ClearDocument(IntPtr hsmart_ptr, int page);    // HSMART70, page

        [DllImport("SmartComm70.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "Smart70_GetFieldCount")]
        public static extern uint GetFieldCount(IntPtr hsmart_ptr, IntPtr pcount_ptr);    // HSMART70, int*

        [DllImport("SmartComm70.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "Smart70_GetFieldName")]
        public static extern uint GetFieldName(IntPtr hsmart_ptr, int idx, IntPtr szname_ptr, int buflen);    // HSMART70, int, WCHAR* DWORD

        [DllImport("SmartComm70.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "Smart70_GetFieldValue")]
        public static extern uint GetFieldValue(IntPtr hsmart_ptr, IntPtr szname_ptr, IntPtr szvalue_ptr);    // HSMART70, WCHAR*, WCHAR*

        [DllImport("SmartComm70.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "Smart70_SetFieldValue")]
        public static extern uint SetFieldValue(IntPtr hsmart_ptr, IntPtr szname_ptr, IntPtr szvalue_ptr);    // HSMART70, WCHAR*, WCHAR*

        [DllImport("SmartComm70.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "Smart70_IsBackEnable")]
        public static extern uint IsBackEnable(IntPtr hsmart_ptr, IntPtr benabled_ptr);    // HSMART70, BOOL*


        [DllImport("SmartComm70.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "Smart70_GetPrinterSettings")]
        public static extern uint GetPrinterSettings(IntPtr hsmart_ptr, Byte mod, IntPtr pdm_ptr);    // HSMART70, BYTE module, SMART70_DEVMODE* pdm

        [DllImport("SmartComm70.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "Smart70_SetPrinterSettings")]
        public static extern uint SetPrinterSettings(IntPtr hsmart_ptr, Byte mod, IntPtr pdm_ptr);    // HSMART70, BYTE module, SMART70_DEVMODE* pdm

        

        
        // DRAWING

        [DllImport("SmartComm70.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "Smart70_DrawRect")]
        public static extern uint DrawRect(IntPtr hsmart_ptr, Byte page, Byte panel, int x, int y, int cx, int cy, int col, IntPtr prc_ptr);    // HSMART70, BYTE, BYTE, int, int, int, int, COLORREF, RECT*

        [DllImport("SmartComm70.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "Smart70_DrawText")]
        public static extern uint DrawText(IntPtr hsmart_ptr, Byte page, Byte panel, int x, int y, IntPtr szfont_ptr, int size, Byte style, IntPtr sztext_ptr, IntPtr prc_ptr);    // HSMART70, BYTE, BYTE, int, int, WCHAR*, int, BYTE, WCHAR*, RECT*

        [DllImport("SmartComm70.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "Smart70_DrawText2")]
        public static extern uint DrawText2(IntPtr hsmart_ptr, Byte page, Byte panel, IntPtr pdt2info_ptr, IntPtr sztext_ptr);    // HSMART70, BYTE, BYTE, DT2INFO*, WCHAR*

        [DllImport("SmartComm70.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "Smart70_DrawImage")]
        public static extern uint DrawImage(IntPtr hsmart_ptr, Byte page, Byte panel, int x, int y, int cx, int cy, IntPtr szpath_ptr, IntPtr prc_ptr);    // HSMART70, BYTE, BYTE, int, int, int, int, WCHAR*, RECT*

        [DllImport("SmartComm70.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "Smart70_ExDrawImage")]
        public static extern uint DrawImage2(IntPtr hsmart_ptr, Byte page, Byte panel, int x, int y, int cx, int cy, int scale, Byte align, IntPtr szpath_ptr, IntPtr prc_ptr);    // HSMART70, BYTE, BYTE, int, int y, int cx, int cy, int nScaleMethod, BYTE nAlign, WCHAR* szImgPath, RECT* prcArea

        [DllImport("SmartComm70.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "Smart70_DrawBitmap")]
        public static extern uint DrawBitmap(IntPtr hsmart_ptr, Byte page, Byte panel, int x, int y, int cx, int cy, IntPtr hbmp_ptr, IntPtr prc_ptr);    // HSMART70, BYTE, BYTE, int, int, int, int, HBITMAP hbmp, RECT*

        [DllImport("SmartComm70.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "Smart70_ExDrawBitmap")]
        public static extern uint DrawBitmap2(IntPtr hsmart_ptr, Byte page, Byte panel, int x, int y, int cx, int cy, int scale, Byte align, IntPtr hbmp_ptr, IntPtr prc_ptr);    // HSMART70, BYTE, BYTE, int, int y, int cx, int cy, int nScaleMethod, BYTE nAlign, HBITMAP hbmp, RECT* prcArea

        [DllImport("SmartComm70.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "Smart70_GetBarcodeTypeCount")]
        public static extern uint GetBarcodeTypeCount(IntPtr hsmart_ptr, IntPtr pncount_ptr);    // HSMART70, int*

        [DllImport("SmartComm70.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "Smart70_GetBarcodeTypeName")]
        public static extern uint GetBarcodeTypeName(IntPtr hsmart_ptr, int idx, IntPtr szname_ptr, int buflen);    // HSMART70, int idx, WCHAR* szName, int nBufLen

        [DllImport("SmartComm70.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "Smart70_DrawBarcode")]
        public static extern uint DrawBarcode(IntPtr hsmart_ptr, Byte page, Byte panel, int x, int y, int cx, int cy, int col, IntPtr prc_ptr, IntPtr szname_ptr, int size, IntPtr szdata_ptr, IntPtr szpost_ptr);    // HSMART70, BYTE page, BYTE panel, int x, int y, int cx, int cy, COLORREF col, RECT* prcArea, const WCHAR* szName, int nSize, const WCHAR* szData, const WCHAR* szPost


        [DllImport("SmartComm70.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "Smart70_LDrawLine")]
        public static extern uint LDrawLine(IntPtr hsmart_ptr, IntPtr pline_ptr, IntPtr plaser_ptr, IntPtr prc_ptr);    // HSMART70, LLINE *pline, S70Laser *plaser, RECT* prc

        [DllImport("SmartComm70.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "Smart70_LDrawRect")]
        public static extern uint LDrawRect(IntPtr hsmart_ptr, IntPtr prect_ptr, IntPtr plaser_ptr, IntPtr prc_ptr);    // HSMART70, LRECT *pline, S70Laser *plaser, RECT* prc

        [DllImport("SmartComm70.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "Smart70_LDrawOval")]
        public static extern uint LDrawOval(IntPtr hsmart_ptr, IntPtr poval_ptr, IntPtr plaser_ptr, IntPtr prc_ptr);    // HSMART70, LOVAL *pline, S70Laser *plaser, RECT* prc

        [DllImport("SmartComm70.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "Smart70_LDrawRRect")]
        public static extern uint LDrawRRect(IntPtr hsmart_ptr, IntPtr prrect_ptr, IntPtr plaser_ptr, IntPtr prc_ptr);    // HSMART70, LRRECT *pline, S70Laser *plaser, RECT* prc

        [DllImport("SmartComm70.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "Smart70_LDrawText")]
        public static extern uint LDrawText(IntPtr hsmart_ptr, IntPtr ptext_ptr, IntPtr plaser_ptr, IntPtr prc_ptr);    // HSMART70, LTEXT *pline, S70Laser *plaser, RECT* prc

        [DllImport("SmartComm70.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "Smart70_LDrawBarcode")]
        public static extern uint LDrawBarcode(IntPtr hsmart_ptr, IntPtr pbar_ptr, IntPtr plaser_ptr, IntPtr prc_ptr);    // HSMART70, LBAR *pline, S70Laser *plaser, RECT* prc

        [DllImport("SmartComm70.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "Smart70_LDrawImage")]
        public static extern uint LDrawImage(IntPtr hsmart_ptr, IntPtr pimg_ptr, IntPtr plaser_ptr, IntPtr prc_ptr);    // HSMART70, LIMG *pline, S70Laser *plaser, RECT* prc

        [DllImport("SmartComm70.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "Smart70_LDrawBitmap")]
        public static extern uint LDrawBitmap(IntPtr hsmart_ptr, IntPtr pbmp_ptr, IntPtr plaser_ptr, IntPtr prc_ptr);    // HSMART70, LBMP *pline, S70Laser *plaser, RECT* prc

        

        
        // DOCUMENT OBJECT MANIPULATION

        [DllImport("SmartComm70.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "Smart70_GetUnitInfo2")]
        public static extern uint GetUnitInfo2(IntPtr hsmart_ptr, IntPtr punit_ptr, int dir);    // HSMART70, S70UNITINFO2* pUnit, int dir

        [DllImport("SmartComm70.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "Smart70_GetFieldLinkedUnitInfo2")]
        public static extern uint GetFieldLinkedUnitInfo2(IntPtr hsmart_ptr, IntPtr szfield_ptr, IntPtr punit_ptr, int dir);    // HSMART70, WCHAR* szField, S70UNITINFO2* pUnit, int dir

        [DllImport("SmartComm70.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "Smart70_SetUnitInfo2")]
        public static extern uint SetUnitInfo2(IntPtr hsmart_ptr, IntPtr punit_ptr);    // HSMART70, WCHAR* szField, S70UNITINFO2* pUnit

        [DllImport("SmartComm70.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "Smart70_GetUnitInfo2Direct")]
        public static extern uint GetUnitInfo2Direct(IntPtr hsmart_ptr, IntPtr punit_ptr);    // HSMART70, S70UNITINFO2* pUnit

        


        
        // PREVIEW

        [DllImport("SmartComm70.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "Smart70_GetPreviewBitmap")]
        public static extern uint GetPreviewBitmap(IntPtr hsmart_ptr, Byte page, ref IntPtr ppbi_ptr);    // HSMART70, BYTE page, BITMAPINFO** const ppbi

        [DllImport("SmartComm70.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "Smart70_GetDocumentBitmap")]
        public static extern uint GetDocumentBitmap(IntPtr hsmart_ptr, Byte page, Byte panel, IntPtr phbmp_ptr);    // HSMART70, BYTE page, BYTE panel, HBITMAP* phbmp

        
        
        
        // PRINT

        [DllImport("SmartComm70.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "Smart70_Print")]
        public static extern uint Print(IntPtr hsmart_ptr);    // HSMART70

        [DllImport("SmartComm70.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "Smart70DCL_Print")]
        public static extern uint DCLPrint(IntPtr hsmart_ptr, int nPrintSide);    // HSMART70, int nPrintSide

        
        
        
        // PRINT - JOB-SCHEDULER
 
        [DllImport("SmartComm70.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "Smart70_SetJobCallback")]
        public static extern uint SetJobCallback(IntPtr hsmart_ptr, IntPtr pfncb_ptr);    // HSMART70, S70CBPROC pfnCB

        [DllImport("SmartComm70.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "Smart70_PrintDocument")]
        public static extern uint PrintDocument(IntPtr hsmart_ptr, int opt, IntPtr pjobid_ptr);    // HSMART70, int opt, int * pjobid

        [DllImport("SmartComm70.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "Smart70_GetJobInfo")]
        public static extern uint GetJobInfo(IntPtr hsmart_ptr, IntPtr pjobinfo_ptr);    // HSMART70, JOBINFO * pjobinf
        
        [DllImport("SmartComm70.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "Smart70_GetJobInfoEx")]
        public static extern uint GetJobInfo2(IntPtr hsmart_ptr, IntPtr pjobinfo2_ptr);    // HSMART70, JOBINFO_EX * pjobinf

        
        
        
        // PRINT - JOB_DESCRIPTOR
        
        [DllImport("SmartComm70.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "Smart70_NewJobDescriptor")]
        public static extern uint NewJobDescriptor(IntPtr hsmart_ptr, ref IntPtr phjd_ptr, IntPtr pfncbErr_ptr);    // HSMART70, HJOBD * phjd, SM70CB_ERROR pfncbErr
        
        [DllImport("SmartComm70.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "Smart70_FreeJobDescriptor")]
        public static extern uint FreeJobDescriptor(IntPtr hsmart_ptr, IntPtr hjd_ptr, IntPtr pfncbErr_ptr);    // HSMART70, HJOBD hjd
        
        [DllImport("SmartComm70.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "Smart70_AddJobItem")]
        public static extern uint AddJobItem(IntPtr hsmart_ptr, IntPtr hjd_ptr, int ji_type, IntPtr pjiparam_ptr);    // HSMART70, HJOBD hjd, int ji_type, void * ji_param
        
        [DllImport("SmartComm70.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "Smart70_AddJobItemMove")]
        public static extern uint AddJobItemMove(IntPtr hsmart_ptr, IntPtr hjd_ptr, Byte modfrom, Byte optfrom, Byte modto, Byte optto);    // HSMART70, HJOBD hjd, BYTE from_mid, BYTE from_opt, BYTE to_mid, BYTE to_opt
        
        [DllImport("SmartComm70.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "Smart70_AddJobItemFlip")]
        public static extern uint AddJobItemFlip(IntPtr hsmart_ptr, IntPtr hjd_ptr, Byte modfrom, Byte modflip, Byte modto, Byte opt);    // HSMART70, HJOBD hjd, BYTE from_mid, BYTE flip_mid, BYTE to_mid, BYTE opt
        
        [DllImport("SmartComm70.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "Smart70_AddJobItemMagnetic")]
        public static extern uint AddJobItemMagnetic(IntPtr hsmart_ptr, IntPtr hjd_ptr, Byte mod, int coer, IntPtr pfncbMag_ptr);    // HSMART70, HJOBD hjd, BYTE mid, int coer, SM70CB_MAGNETIC pfncb
        
        [DllImport("SmartComm70.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "Smart70_AddJobItemContact")]
        public static extern uint AddJobItemContact(IntPtr hsmart_ptr, IntPtr hjd_ptr, Byte mod, IntPtr pfncbContact_ptr);    // HSMART70, HJOBD hjd, BYTE mid, SM70CB_CONTACT pfncb
        
        [DllImport("SmartComm70.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "Smart70_AddJobItemContactless")]
        public static extern uint AddJobItemContactless(IntPtr hsmart_ptr, IntPtr hjd_ptr, Byte mod, IntPtr pfncbCL_ptr);    // HSMART70, HJOBD hjd, BYTE mid, SM70CB_CONTACTLESS pfncb
        
        [DllImport("SmartComm70.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "Smart70_AddJobItemScanImage")]
        public static extern uint AddJobItemScanImage(IntPtr hsmart_ptr, IntPtr hjd_ptr, Byte mod, int dpi, int mode, int side, int opt, IntPtr pfncbScanImage_ptr);    // HSMART70, HJOBD hjd, BYTE mid, int dpi, int mode, int side, int opt, SM70CB_SCANIMAGE pfncb
        
        [DllImport("SmartComm70.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "Smart70_AddJobItemScanBarcode")]
        public static extern uint AddJobItemScanBarcode(IntPtr hsmart_ptr, IntPtr hjd_ptr, Byte mod, int dpi, int mode, int side, int opt, IntPtr pfncbScanBar_ptr);    // HSMART70, HJOBD hjd, BYTE mid, int dpi, int mode, int side, int opt, SM70CB_SCANBARCODE pfncb
        
        [DllImport("SmartComm70.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "Smart70_AddJobItemPrint")]
        public static extern uint AddJobItemPrint(IntPtr hsmart_ptr, IntPtr hjd_ptr, Byte mod, int page, IntPtr pfncbPrint_ptr);    // HSMART70, HJOBD hjd, BYTE mid, int page, SM70CB_PRINT pfncb
        
        [DllImport("SmartComm70.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "Smart70_AddJobItemLaminate")]
        public static extern uint AddJobItemLaminate(IntPtr hsmart_ptr, IntPtr hjd_ptr, Byte mod, int page, IntPtr pfncbLami_ptr);    // HSMART70, HJOBD hjd, BYTE mid, int page, SM70CB_LAMINATE pfncb
        
        [DllImport("SmartComm70.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "Smart70_AddJobItemUser")]
        public static extern uint AddJobItemUser(IntPtr hsmart_ptr, IntPtr hjd_ptr, Byte mod, int opt, IntPtr pfncbUser_ptr);    // HSMART70, HJOBD hjd, BYTE mid, int opt, SM70CB_USER pfncb
        
        [DllImport("SmartComm70.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "Smart70_InsertJob")]
        public static extern uint InsertJob(IntPtr hsmart_ptr, IntPtr hjd_ptr, IntPtr pjobid_ptr);    // HSMART70, HJOBD hjd, int * pjobid

        
        
        
        // DEVICE ACTIONS
        
        [DllImport("SmartComm70.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "Smart70_RequestPreemption")]
        public static extern uint RequestPreemption(IntPtr hsmart_ptr, IntPtr pprmt_ptr);    // HSMART70, WORD * pwPrmt
        
        [DllImport("SmartComm70.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "Smart70_ReleasePreemption")]
        public static extern uint ReleasePreemption(IntPtr hsmart_ptr, Int16 prmt);    // HSMART70, WORD wPrmt
        
        [DllImport("SmartComm70.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "Smart70_IsModuleReady")]
        public static extern uint IsModuleReady(IntPtr hsmart_ptr, Byte mod, IntPtr pnready_ptr);    // HSMART70, BYTE mod, int *pnReady
        
        [DllImport("SmartComm70.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "Smart70_IsReadyInsertJob")]
        public static extern uint IsReadyInsertJob(IntPtr hsmart_ptr, IntPtr pnready_ptr);    // HSMART70, int *pnReady
        
        [DllImport("SmartComm70.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "Smart70_ToggleManualMode")]
        public static extern uint ToggleManualMode(IntPtr hsmart_ptr);    // HSMART70


        [DllImport("SmartComm70.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "Smart70_CardIn")]
        public static extern uint CardIn(IntPtr hsmart_ptr, Byte mod, int dir, int opt);    // HSMART70, BYTE module, int dir, int opt

        [DllImport("SmartComm70.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "Smart70_CardOut")]
        public static extern uint CardOut(IntPtr hsmart_ptr, Byte mod, int dir, int opt);    // HSMART70, BYTE module, int dir, int opt

        [DllImport("SmartComm70.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "Smart70_CardMove2")]
        public static extern uint CardMove(IntPtr hsmart_ptr, Byte modfrom, Byte optfrom, Byte modto, Byte optto, int cmdopt);    // HSMART70, BYTE fromModule, BYTE fromOpt, BYTE toModule, BYTE toOpt, int cmdopt

        //[DllImport("SmartComm70.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "Smart70_CardListMove")]
        //public static extern uint CardListMove(IntPtr hsmart_ptr, IntPtr plistmove_ptr);    // HSMART70, CARDMOVELIST * plist

        [DllImport("SmartComm70.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "Smart70_CardMoveQuick")]
        public static extern uint CardMoveQuick(IntPtr hsmart_ptr, Byte mod, Byte opt, int cmdopt);    // HSMART70, BYTE mod, BYTE opt, int cmdopt

        [DllImport("SmartComm70.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "Smart70_CardPass")]
        public static extern uint CardPass(IntPtr hsmart_ptr, Byte mod, Byte opt, int cmdopt);    // HSMART70, BYTE mod, BYTE opt, int cmdopt

        [DllImport("SmartComm70.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "Smart70_CardFlipPass")]
        public static extern uint CardFlipPass(IntPtr hsmart_ptr, Byte mod, Byte opt, int cmdopt);    // HSMART70, BYTE mod, BYTE opt, int cmdopt

        [DllImport("SmartComm70.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "Smart70_CardFlip2")]
        public static extern uint CardFlip(IntPtr hsmart_ptr, Byte modfrom, Byte modflip, Byte modto);    // HSMART70, BYTE modfrom, BYTE modflip, BYTE modto

        [DllImport("SmartComm70.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "Smart70_CardFlipStay")]
        public static extern uint CardFlipStay(IntPtr hsmart_ptr, Byte modfrom, Byte modflip);    // HSMART70, BYTE modfrom, BYTE modflip

        [DllImport("SmartComm70.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "Smart70_Flip")]
        public static extern uint Flip(IntPtr hsmart_ptr, Byte mod, int opt, int value);    // HSMART70, BYTE mod, int opt, int value

        [DllImport("SmartComm70.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "Smart70_CardMoveSensor")]
        public static extern uint CardMoveSensor(IntPtr hsmart_ptr, Byte mod, int opt1, int valule, int opt2);    // HSMART70, BYTE module, int opt1, int valule, int opt2

        [DllImport("SmartComm70.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "Smart70_CardMovePosition")]
        public static extern uint CardMovePosition(IntPtr hsmart_ptr, Byte mod, int pos);    // HSMART70, BYTE module, int pos

        [DllImport("SmartComm70.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "Smart70_CardEject")]
        public static extern uint CardEject(IntPtr hsmart_ptr, Byte mod);    // HSMART70, BYTE mod

        [DllImport("SmartComm70.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "Smart70_CardEject2")]
        public static extern uint CardEject2(IntPtr hsmart_ptr, Byte mod, int opt, Byte modout, int optout);    // HSMART70, BYTE mod, int opt, BYTE modout, int optout

        [DllImport("SmartComm70.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "Smart70_CardEjectBin")]
        public static extern uint CardEjectBin(IntPtr hsmart_ptr, Byte mod, Byte modbin);    // HSMART70, BYTE mod, BYTE modbin

        [DllImport("SmartComm70.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "Smart70_SetEncConfig")]
        public static extern uint SetEncConfig(IntPtr hsmart_ptr, Byte mod, int config, int value);    // HSMART70, BYTE mod, int config, int value

        [DllImport("SmartComm70.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "Smart70_GetEncConfig2")]
        public static extern uint GetEncConfig2(IntPtr hsmart_ptr, Byte mod, int config, IntPtr pvalue_ptr, IntPtr plen_ptr);    // HSMART70, BYTE mod, int config, void* pvalue, int* plen

        [DllImport("SmartComm70.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "Smart70_SetEncConfig2")]
        public static extern uint SetEncConfig2(IntPtr hsmart_ptr, Byte mod, int config, IntPtr pvalue_ptr, int len);    // HSMART70, BYTE mod, int config, void* pvalue, int len

        [DllImport("SmartComm70.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "Smart70_SetEncBuffer")]
        public static extern uint SetEncBuffer(IntPtr hsmart_ptr, Byte mod, Byte track, int nbuflen, IntPtr szbuf_ptr);    // HSMART70, BYTE mod, BYTE track, int nbuflen, WCHAR* szbuf

        [DllImport("SmartComm70.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "Smart70_WriteEncoder")]
        public static extern uint WriteEncoder(IntPtr hsmart_ptr, Byte mod, Byte track, int opt);    // HSMART70, BYTE mod, BYTE track, int opt

        [DllImport("SmartComm70.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "Smart70_ReadEncoder")]
        public static extern uint ReadEncoder(IntPtr hsmart_ptr, Byte mod, Byte track, int opt);    // HSMART70, BYTE mod, BYTE track, int opt

        [DllImport("SmartComm70.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "Smart70_FetchEncoder")]
        public static extern uint FetchEncoder(IntPtr hsmart_ptr, Byte mod, Byte track);    // HSMART70, BYTE mod, BYTE track

        [DllImport("SmartComm70.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "Smart70_GetEncBuffer")]
        public static extern uint GetEncBuffer(IntPtr hsmart_ptr, Byte mod, Byte track, IntPtr pbufsize_ptr, IntPtr szbuf_ptr);    // HSMART70, BYTE mod, BYTE track, int * pnbufsize, WCHAR* szbuf

        [DllImport("SmartComm70.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "Smart70_RunEncoder")]
        public static extern uint RunEncoder(IntPtr hsmart_ptr, Byte mod, Byte track, int run, IntPtr pbuf_ptr, IntPtr pbuflen_ptr);    // HSMART70, BYTE mod, BYTE track, int run, void* pbuf, int* pbuflen

        [DllImport("SmartComm70.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "Smart70_MagGetAllBufferEx")]
        public static extern uint MagGetAllBuffer(IntPtr hsmart_ptr, int bT1, IntPtr pbuf1_ptr, IntPtr plen1_ptr, int bT2, IntPtr pbuf2_ptr, IntPtr plen2_ptr, int bT3, IntPtr pbuf3_ptr, IntPtr plen3_ptr, int bJIS, IntPtr pbufJ_ptr, IntPtr plenJ_ptr);    // HSMART70, BOOL, BYTE*, int*, BOOL, BYTE*, int*, BOOL, BYTE*, int*, BOOL, BYTE*, int*

        [DllImport("SmartComm70.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "Smart70_MagGetAllCryptoBufferEx")]
        public static extern uint MagGetAllCryptoBuffer(IntPtr hsmart_ptr, int bT1, IntPtr pbuf1_ptr, IntPtr plen1_ptr, int bT2, IntPtr pbuf2_ptr, IntPtr plen2_ptr, int bT3, IntPtr pbuf3_ptr, IntPtr plen3_ptr, int bJIS, IntPtr pbufJ_ptr, IntPtr plenJ_ptr, IntPtr pbtkey_ptr);    // HSMART70, BOOL, BYTE*, int*, BOOL, BYTE*, int*, BOOL, BYTE*, int*, BOOL, BYTE*, int*, BYTE* pKey

        [DllImport("SmartComm70.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "Smart70_MagSetAllCryptoBufferEx")]
        public static extern uint MagSetAllCryptoBuffer(IntPtr hsmart_ptr, int bT1, IntPtr pbuf1_ptr, int len1, int bT2, IntPtr pbuf2_ptr, int len2, int bT3, IntPtr pbuf3_ptr, int len3, int bJIS, IntPtr pbufJ_ptr, int lenJ, IntPtr pbtkey_ptr);    // HSMART70, BOOL, BYTE*, int, BOOL, BYTE*, int, BOOL, BYTE*, int, BOOL, BYTE*, int, BYTE* pKey


        [DllImport("SmartComm70.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "Smart70_MagWrite")]
        public static extern uint MagWrite(IntPtr hsmart_ptr, Byte mod, int bT1, IntPtr szT1_ptr, int bT2, IntPtr szT2_ptr, int bT3, IntPtr szT3_ptr, int bJIS, IntPtr szJIS_ptr);    // HSMART70, BYTE mod, BOOL bT1, WCHAR* szT1, BOOL bT2, WCHAR* szT2, BOOL bT3, WCHAR* szT3, BOOL bJIS, WCHAR* szJIS

        [DllImport("SmartComm70.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "Smart70_MagRead")]
        public static extern uint MagRead(IntPtr hsmart_ptr, Byte mod, int bT1, IntPtr szT1_ptr, int nT1, int bT2, IntPtr szT2_ptr, int nT2, int bT3, IntPtr szT3_ptr, int nT3, int bTJ, IntPtr szTJ_ptr, int nTJ);    // HSMART70, BYTE mod, BOOL bT1, WCHAR* szT1, int nT1, BOOL bT2, WCHAR* szT2, int nT2, BOOL bT3, WCHAR* szT3, int nT3, BOOL bJIS, WCHAR* szJIS, int nJIS


        [DllImport("SmartComm70.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "Smart70_ExUnlockPrinter2")]
        public static extern uint UnlockPrinter(IntPtr hsmart_ptr, IntPtr szpw_ptr);    // HSMART70, WCHAR* szPW

        [DllImport("SmartComm70.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "Smart70_ExLockPrinter2")]
        public static extern uint LockPrinter(IntPtr hsmart_ptr, IntPtr szpw_ptr);    // HSMART70, WCHAR* szPW

        
        
        
        // DEVICE SETTINGS

        [DllImport("SmartComm70.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "Smart70_GetCountParam")]
        public static extern uint GetCountParam(IntPtr hsmart_ptr, IntPtr pcntparam_ptr);    // HSMART70, COUNTPARAM* pParam

        [DllImport("SmartComm70.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "Smart70_GetCountParamEx")]
        public static extern uint GetCountParam2(IntPtr hsmart_ptr, IntPtr pcntparam_ptr);    // HSMART70, COUNTPARAM* pParam

        [DllImport("SmartComm70.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "Smart70_ReadUserMemory")]
        public static extern uint ReadUserMemory(IntPtr hsmart_ptr, IntPtr szpw_ptr, int addr, IntPtr pdata_ptr);    // HSMART70, WCHAR* szPW, int addr, BYTE* btdata

        [DllImport("SmartComm70.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "Smart70_WriteUserMemory")]
        public static extern uint WriteUserMemory(IntPtr hsmart_ptr, IntPtr szpw_ptr, int addr, IntPtr pdata_ptr);    // HSMART70, WCHAR* szPW, int addr, BYTE* btdata

        
        
        
        // CONTACT/CONTACTLESS SMART-CARD

        [DllImport("SmartComm70.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "Smart70_ICHContact")]
        public static extern uint ICHContact(IntPtr hsmart_ptr, Byte mod);    // HSMART70, BYTE mod

        [DllImport("SmartComm70.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "Smart70_ICHDiscontact")]
        public static extern uint ICHDiscontact(IntPtr hsmart_ptr, Byte mod);    // HSMART70, BYTE mod


        [DllImport("SmartComm70.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "Smart70_ICPowerOn2")]
        public static extern uint ICPowerOn(IntPtr hsmart_ptr, Byte mod, IntPtr poutlen_ptr, IntPtr poutbuf_ptr);    // HSMART70, BYTE mod, DWORD* pdwOutLen, BYTE* pOutBuf

        [DllImport("SmartComm70.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "Smart70_ICPowerOff2")]
        public static extern uint ICPowerOff(IntPtr hsmart_ptr, Byte mod);    // HSMART70, BYTE mod

        [DllImport("SmartComm70.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "Smart70_ICTransmit2")]
        public static extern uint ICTransmit(IntPtr hsmart_ptr, Byte mod, int dwInLen, IntPtr pinbuf_ptr, IntPtr poutlen_ptr, IntPtr poutbuf_ptr);    // HSMART70, BYTE mod, DWORD dwInLen, BYTE* pInBuf, DWORD* pdwOutLen, BYTE* pOutBuf

        
        [DllImport("SmartComm70.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "Smart70_RFPowerOn2")]
        public static extern uint RFPowerOn(IntPtr hsmart_ptr, Byte mod, IntPtr poutlen_ptr, IntPtr poutbuf_ptr);    // HSMART70, BYTE mod, DWORD* pdwOutLen, BYTE* pOutBuf

        [DllImport("SmartComm70.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "Smart70_RFPowerOff2")]
        public static extern uint RFPowerOff(IntPtr hsmart_ptr, Byte mod);    // HSMART70, BYTE mod

        [DllImport("SmartComm70.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "Smart70_RFTransmit2")]
        public static extern uint RFTransmit(IntPtr hsmart_ptr, Byte mod, int dwInLen, IntPtr pinbuf_ptr, IntPtr poutlen_ptr, IntPtr poutbuf_ptr);    // HSMART70, BYTE mod, DWORD dwInLen, BYTE* pInBuf, DWORD* pdwOutLen, BYTE* pOutBuf

        
        
        
        // LASER DEVICE / DRAWING

        [DllImport("SmartComm70.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "Smart70_LaserConnect")]
        public static extern uint LaserConnect();

        [DllImport("SmartComm70.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "Smart70_LaserDisconnect")]
        public static extern uint LaserDisconnect();

        
        
        
        // ETC

        [DllImport("SmartComm70.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "Smart70_ExSetConfig")]
        public static extern uint SetConfig(IntPtr hsmart_ptr, int nID, IntPtr nvalue_ptr);      // HSMART70 hHandle, int nID, int nValue

        [DllImport("SmartComm70.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "Smart70_ExGetConfig")]
        public static extern uint GetConfig(IntPtr hsmart_ptr, int nID, IntPtr pnvalue_ptr);      // HSMART70 hHandle, int nID, int * pnValue




        // Return Codes
        public const uint	SM_SUCCESS						= 0;
        public const uint	SM_F_FOUNDNODEV                 = 0x80000000;	// there is no device to use.
        public const uint	SM_F_INVALIDDEVIDX              = 0x80000001;	// index of device is out of bound.
        public const uint	SM_F_INVALIDBUFPOINTER          = 0x80000002;	// invalid buffer pointer. (may be null)
        public const uint	SM_F_NOTEXISTDEV                = 0x80000003;	// not exist device. (not connected device)
        public const uint	SM_F_INVALIDPARAM               = 0x80000004;	// invalid parameter value.
        public const uint	SM_F_DEVOPENFAILED              = 0x80000005;	// device open failed. for more information, use GetLastError API.
        public const uint	SM_F_DEVIO                      = 0x80000006;	// device io operation is failed. for more information, use GetLastError API.
        public const uint	SM_F_FOUNDNODRV                 = 0x80000007;	// not found driver or cannot acquire DEVMODE from driver.
        public const uint	SM_F_INVALIDHANDLE              = 0x80000008;	// invalid handle value.
        public const uint	SM_F_CARDISINSIDE               = 0x80000009;	// card is already inside of device.
        public const uint	SM_F_NOCARDISINSIDE             = 0x8000000A;	// no card is inside of device.
        public const uint	SM_F_HOPPEREMPTY                = 0x8000000B;	// no cards are in hopper.
        public const uint	SM_F_NOCARDONBOTH               = 0x8000000C;	// no card both hopper and inside of printer.
        public const uint	SM_F_WAITTIMEOUT                = 0x8000000D;	// timeout occured while wait...
        public const uint	SM_F_CASEOPEN                   = 0x8000000E;	// case is opened.
        public const uint	SM_F_ERRORSTATUS                = 0x8000000F;	// current status has error flag.
        public const uint	SM_F_CARDIN                     = 0x80000010;	// card-in action is failed.
        public const uint	SM_F_CARDOUT                    = 0x80000011;	// card-out action is failed.
        public const uint	SM_F_CARDOUTBACK                = 0x80000012;	// card-back-out action is failed.
        public const uint	SM_F_MOVE2MAG                   = 0x80000013;	// card move (to magnetic) is failed.
        public const uint	SM_F_MOVE2IC                    = 0x80000014;	// card move (to IC) is failed.
        public const uint	SM_F_MOVE2RF                    = 0x80000015;	// card move (to RF) is failed.
        public const uint	SM_F_MOVE2ROT                   = 0x80000016;	// card move (to Rotator) is failed.
        public const uint	SM_F_MOVE2DEV                   = 0x80000017;	// card move (from Rotator) is failed.
        public const uint	SM_F_MAGRW                      = 0x80000018;	// magnetic read/write is failed,
        public const uint	SM_F_NOPRINTDATA                = 0x80000019;	// printer failed to receive print data.
        public const uint	SM_F_PRINT                      = 0x8000001A;	// print failed.
        public const uint	SM_F_SEEKRIBBON                 = 0x8000001B;	// seek-ribbon is failed.
        public const uint	SM_F_MOVERIBBON                 = 0x8000001C;	// move-ribbon is failed.
        public const uint	SM_F_EMPTYRIBBON                = 0x8000001D;	// ribbon is empty.
        public const uint	SM_F_ICHUP                      = 0x8000001E;	// ic-head up failed.
        public const uint	SM_F_ICHDN                      = 0x8000001F;	// ic-head down failed.
        public const uint	SM_F_ROTTOP                     = 0x80000020;	// rotate to top is failed.
        public const uint	SM_F_ROTBOTTOM                  = 0x80000021;	// rotate to bottom is failed.
        public const uint	SM_F_REQNOMAGTRACK              = 0x80000022;	// requested no magnetic track.
        public const uint	SM_F_REQMULTIMAGTRACK           = 0x80000023;	// requested two or more magnetic tracks in XXXGetBuffer function.
        public const uint	SM_F_FILENOTFOUND               = 0x80000024;	// file not found.
        public const uint	SM_F_FIELDNOTFOUND              = 0x80000025;	// field is not exist.
        public const uint	SM_F_IMAGELOAD                  = 0x80000026;	// failed to load image.
        public const uint	SM_F_CREATEDC                   = 0x80000027;	// dc creation is failed.
        public const uint	SM_F_VERIFYDRV                  = 0x80000028;	// driver verification is failed. may the driver is not ours.
        public const uint	SM_F_SPOOLING                   = 0x80000029;	// failed to make spool data. (includes StartDoc, StartPage, EndPage and EndDoc)
        public const uint	SM_F_DEVNOTOPENED               = 0x8000002A;	// request access to ic/rf module without opening the printer device.
        public const uint	SM_F_USEDBYOTHER                = 0x8000002B;	// usb is temporarily blocked by other. 
        public const uint	SM_F_SOCKETCREATE               = 0x8000002C;	// socket creation failed.
        public const uint	SM_F_SOCKETCONNECT              = 0x8000002D;	// socket connection failed.
        public const uint	SM_F_SSLINIT                    = 0x8000002E;	// SSL initialization failed.
        public const uint	SM_F_SSLCREATE                  = 0x8000002F;	// SSL creation failed.
        public const uint	SM_F_SSLCONNECT                 = 0x80000030;	// SSL connection is failed.
        public const uint	SM_F_RESERVED                   = 0x80000031;	// host is already reserved status.
        public const uint	SM_F_INVALIDSOCKET              = 0x80000032;	// socket fd is invalid.
        public const uint	SM_F_LESSSENDED                 = 0x80000033;	// packet is sended less than requested.
        public const uint	SM_F_LESSRECVED                 = 0x80000034;	// packet is received less than requested.
        public const uint	SM_F_SOCKETERROR                = 0x80000035;	// socket error occured. for more information, use WSAGetLastError API.
        public const uint	SM_F_INVALIDPACKET              = 0x80000036;	// packet is not valid. (include signature is not matched or else ...)
        public const uint	SM_F_PACKETSEQDIFFER            = 0x80000037;	// packet sequence/id is not equaled when receive.
        public const uint	SM_F_PACKETFLAGNOREPLY          = 0x80000038;	// reply flag is not setted on received packet.
        public const uint	SM_F_PACKETFLAGHEADER           = 0x80000039;	// sent packet header is incorrect.
        public const uint	SM_F_PACKETFLAGARGUMENT         = 0x8000003A;	// argument is not valid on sent packet.
        public const uint	SM_F_PACKETFLAGEXE              = 0x8000003B;	// execution error flag is setted.
        public const uint	SM_F_PACKETFLAGBADCMD           = 0x8000003C;	// bad command flag is setted.
        public const uint	SM_F_PACKETFLAGINIT             = 0x8000003D;	// ...
        public const uint	SM_F_PACKETFLAGHANDLE           = 0x8000003E;	// invalid handle is given.
        public const uint	SM_F_FILEOPEN                   = 0x8000003F;	// file open failed...
        public const uint	SM_F_FILEREAD                   = 0x80000040;	// read from fale is failed.
        public const uint	SM_F_NOTSUPPORTYET              = 0x80000041;	// not support yet...
        public const uint	SM_F_INSUFFICIENTBUF            = 0x80000042;	// insufficient buffer.

        // IC error area...
        public const uint	SM_F_ICESTABLISH                = 0x80000043;	// SCardEstablish failed. for more information, use GetLastError.
        public const uint	SM_F_ICLISTREADER               = 0x80000044;	// SCardListReaders failed. for more information, use GetLastError.
        public const uint	SM_F_ICCONNECT                  = 0x80000045;	// SCardConnect failed. for more information, use GetLastError.
        public const uint	SM_F_ICGETSTATUS                = 0x80000046;	// SCardStatus failed. for more information, use GetLastError.
        public const uint	SM_F_ICDISCONNECT               = 0x80000047;	// SCardDisconenct failed. for more information, use GetLastError.
        public const uint	SM_F_ICRELEASE                  = 0x80000048;	// SCardReleaseContext failed. for more information, use GetLastError.

        // RF error area...
        public const uint	SM_F_RFINVALIDSETTING           = 0x80000049;	// setting value is incorrect. (i.e., RF Port, ...)
        public const uint	SM_F_RFOPEN                     = 0x8000004A;	// port open failed.
        public const uint	SM_F_RFCONNECT                  = 0x8000004B;	// connect failed.
        public const uint	SM_F_RFALREADYOPENED            = 0x80000051;	// open the port that already opened.
        public const uint	SM_F_RFOPENFAIL                 = 0x80000052;	// port open failed.
        public const uint	SM_F_RFINVALIDCOMMPORT          = 0x80000053;	// invalid COM port number.
        public const uint	SM_F_RFFAILGETCOMMSTATE         = 0x80000054;	// error of communication setting.
        public const uint	SM_F_RFFAILSETCOMMSTATE         = 0x80000055;	// error of communication setting.
        public const uint	SM_F_RFFAILCOMMIOTHREAD         = 0x80000056;	// error of communication setting.
        public const uint	SM_F_RFPARAMETER                = 0x80000057;	// incorrect function parameter.
        public const uint	SM_F_RFCOMMSTATE                = 0x80000150;	// .
        public const uint	SM_F_RFCOMMSTATEPROCESS         = 0x80000151;	// running non-blocking mode works.
        public const uint	SM_F_RFCOMMSTATEFINISH          = 0x8000015F;	// finished non-blocking mode works.
        public const uint	SM_F_RFCOMMSTATEERROR           = 0x800001D0;	// .
        public const uint	SM_F_RFCOMMSTATEERRORWRITE      = 0x800001D1;	// data transmition error.
        public const uint	SM_F_RFCOMMSTATEERRORFORMAT     = 0x800001D2;	// invalid received data format.
        public const uint	SM_F_RFCOMMSTATEERRORLENGTH     = 0x800001D3;	// incorrect data length of received data length.
        public const uint	SM_F_RFCOMMSTATEERRORLRC        = 0x800001D4;	// error of received data's LRC.
        public const uint	SM_F_RFCOMMSTATEERRORTIMEOUT    = 0x800001D5;	// receiving timeout.

        // additional error codes...
        public const uint	SM_F_INVALIDNAME                = 0x80000200;	// invalid or not supporting barcode name is given.
        public const uint	SM_F_ICDLLLOADFAILED            = 0x80000201;	// ic(contact smartcard) dll load failed.	;	// DEPRECATED.
        public const uint	SM_F_RFDLLLOADFAILED            = 0x80000202;	// rf(contactless smartcard) dll load failed.;	// DEPRECATED.
        public const uint	SM_F_OBJECTNOTFOUND             = 0x80000203;	// object is not exist.
        public const uint	SM_F_SUBDLLLOADFAILED           = 0x80000204;	// sub-dll load failed... (ie., devconlib.dll, PIR_D02B.dll, ...)
        public const uint	SM_F_CONFIGFILEFAILED           = 0x80000205;	// configuration file load/save failed...

        public const uint	SM_F_OVERHEATED                 = 0x80000206;	// thermal-head is overheated...
        public const uint	SM_F_NOPRINTHREAD               = 0x80000207;	// no thermal-head

        public const uint	SM_F_LOWERVERSION               = 0x80000208;	// SmartCommCl is lower than SmartCommon
        public const uint	SM_F_HIGHERVERSION              = 0x80000209;	// SmartCommCl is higher than SmartCommon
        public const uint	SM_F_OLDSERVER                  = 0x8000020A;	// SmartCommonService is old.
        public const uint	SM_F_VERSIONNOTMATCH            = 0x8000020B;	// version is not matched.

        public const uint	SM_F_CHANGEPASSWORD             = 0x8000020C;	// changing root/user password is failed.
        public const uint	SM_F_UNLOCK                     = 0x8000020D;	// unlock is failed.
        public const uint	SM_F_LOCK                       = 0x8000020E;	// lock is failed.
        public const uint	SM_F_FAILEDTOSET                = 0x8000020F;	// failed to printer set....

        public const uint	SM_F_FILESIZEZERO               = 0x80000220;	// required file size is 0.
        public const uint	SM_F_DEPRECATED                 = 0x80000221;	// that function is deprecated.
        public const uint	SM_F_SERIALNORESPONSE           = 0x80000222;	// not responding for serial command...


        public const uint	SM_F_NETAUTHFAIL                = 0x80000300;	// Network Authentification failed.
        public const uint	SM_F_NETREBOOTERROR             = 0x80000301;	// Network Reboot command failed.
        public const uint	SM_F_NETRELOADERROR             = 0x80000302;	// Network Reload command failed.
        public const uint	SM_F_NETRESETERROR              = 0x80000303;	// Network Reset command failed.
        public const uint	SM_F_NETUPGRADEERROR            = 0x80000304;	// Network upgrade command failed.
        public const uint	SM_F_NETGETSYSCFGERROR          = 0x80000305;	// Network Get System Config command failed.
        public const uint	SM_F_NETSETSYSCFGERROR          = 0x80000306;	// Network Set System Config command failed.
        public const uint	SM_F_NETGETSVCCFGERROR          = 0x80000307;	// Network Get Service Config command failed.
        public const uint	SM_F_NETSETSVCCFGERROR          = 0x80000308;	// Network Set Service Config command failed.
        public const uint	SM_F_NETLISTUSERERROR           = 0x80000309;	// Network List User command failed.
        public const uint	SM_F_NETADDUSERERROR            = 0x8000030A;	// Network Add User command failed.
        public const uint	SM_F_NETDELUSERERROR            = 0x8000030B;	// Network Delete User command failed.
        public const uint	SM_F_NETPASSWDUSERERROR         = 0x8000030C;	// Network Change User Password command failed.

        public const uint	SM_F_PREEMPTION					= 0x80000350;	// Preemption Failed. Other Program Already have the Preemption.
        public const uint	SM_F_CARDMOVE					= 0x80000351;	// Card Move (among with Modules) is failed.
        public const uint	SM_F_MOVEPOSITION				= 0x80000352;	// Card Positining Move is failed.
        public const uint	SM_F_SETENCCONFIG				= 0x80000353;	// Change Encoder Config is failed.
        public const uint	SM_F_ENCODING					= 0x80000354;	// Encoding failed.
        public const uint	SM_F_NOTEXISTMODULE				= 0x80000355;	// Module does not exist.
        public const uint	SM_F_MODULERESERVED				= 0x80000356;	// Module is reserved by other job.
        public const uint	SM_F_MODULEOCUPIED				= 0x80000357;	// Module is ocupied by other job.
        public const uint	SM_F_NOTEXISTJOB				= 0x80000358;	// Specified Job Id does not exit. Maybe Wrong Job id or finished already.
        public const uint	SM_F_MODULEBUSY					= 0x80000359;	// Module is busy
        public const uint	SM_F_MODULEHASERROR				= 0x8000035A;	// Module has error
        public const uint	SM_F_CANCELBYUSER				= 0x8000035B;	// Printing Job is Canceled by User.
        public const uint	SM_F_EMPTYDATA					= 0x8000035C;	// There is No Encoded Data.
        public const uint	SM_F_LAMINATE					= 0x8000035D;	// error while laminating
        public const uint	SM_W_HOLDINGPRINTDATA			= 0x8000035E;	// not an error. have print data but not printing.
        public const uint	SM_F_INVALIDMODULEID			= 0x8000035F;	// invalid module id.
        public const uint	SM_F_STACKERFULL				= 0x80000360;	// stacker is full.
        public const uint	SM_F_MODULEWEAKRESERVED			= 0x80000361;	// Module is weak-reserved by other job.
        public const uint	SM_W_BLANKIMAGE					= 0x80000380;	// Blank (All White) Image is Drawed...
        public const uint	SM_F_UNKNOWNCMD					= 0x80000381;	// given command is unknown.
        public const uint	SM_W_NEAREMPTYHOPPER			= 0x80000382;	// hopper became near empty. please prepare it.

        // Packet Error Code
        public const uint	SM7_F_PACKETINVALID				= 0x80000400;	// packet is invalid (not our format).
        public const uint	SM7_F_PACKETSHORT				= 0x80000401;	// data
        public const uint	SM7_F_PACKETCRC					= 0x80000402;	// different crc
        public const uint	SM7_F_EMPTYHOPPER				= 0x80000403;	// hopper is empty
        public const uint	SM7_F_ERRORLASTCMD				= 0x80000404;	// error came from last command...

        // PC/SC SmartCard Service Error Code...
        public const uint	SM_F_SCARD_F_INTERNAL_ERROR             = 0x80100001;	//SCARD_F_INTERNAL_ERROR			//  An internal consistency check failed.
        public const uint	SM_F_SCARD_E_CANCELLED                  = 0x80100002;	//SCARD_E_CANCELLED				 	//	The	action was cancelled by	an SCardCancel request.
        public const uint	SM_F_SCARD_E_INVALID_HANDLE             = 0x80100003;	//SCARD_E_INVALID_HANDLE			//	The	supplied handle	was	invalid.
        public const uint	SM_F_SCARD_E_INVALID_PARAMETER          = 0x80100004;	//SCARD_E_INVALID_PARAMETER		 	//	One	or more	of the supplied	parameters could not be	properly interpreted.
        public const uint	SM_F_SCARD_E_INVALID_TARGET             = 0x80100005;	//SCARD_E_INVALID_TARGET			//	Registry startup information is	missing	or invalid.
        public const uint	SM_F_SCARD_E_NO_MEMORY                  = 0x80100006;	//SCARD_E_NO_MEMORY				 	//	Not	enough memory available	to complete	this command.
        public const uint	SM_F_SCARD_F_WAITED_TOO_LONG            = 0x80100007;	//SCARD_F_WAITED_TOO_LONG			//	An internal	consistency	timer has expired.
        public const uint	SM_F_SCARD_E_INSUFFICIENT_BUFFER        = 0x80100008;	//SCARD_E_INSUFFICIENT_BUFFER		//	The	data buffer	to receive returned	data is	too	small for the returned data.
        public const uint	SM_F_SCARD_E_UNKNOWN_READER             = 0x80100009;	//SCARD_E_UNKNOWN_READER			//	The	specified reader name is not recognized.
        public const uint	SM_F_SCARD_E_TIMEOUT                    = 0x8010000A;	//SCARD_E_TIMEOUT					//	The	user-specified timeout value has expired.
        public const uint	SM_F_SCARD_E_SHARING_VIOLATION          = 0x8010000B;	//SCARD_E_SHARING_VIOLATION		 	//	The	smart card cannot be accessed because of other connections outstanding.
        public const uint	SM_F_SCARD_E_NO_SMARTCARD               = 0x8010000C;	//SCARD_E_NO_SMARTCARD			 	//	The	operation requires a Smart Card, but no	Smart Card is currently	in the device.
        public const uint	SM_F_SCARD_E_UNKNOWN_CARD               = 0x8010000D;	//SCARD_E_UNKNOWN_CARD			 	//	The	specified smart	card name is not recognized.
        public const uint	SM_F_SCARD_E_CANT_DISPOSE               = 0x8010000E;	//SCARD_E_CANT_DISPOSE			 	//	The	system could not dispose of	the	media in the requested manner.
        public const uint	SM_F_SCARD_E_PROTO_MISMATCH             = 0x8010000F;	//SCARD_E_PROTO_MISMATCH			//	The	requested protocols	are	incompatible with the protocol currently in	use	with the smart card.
        public const uint	SM_F_SCARD_E_NOT_READY                  = 0x80100010;	//SCARD_E_NOT_READY				 	//	The	reader or smart	card is	not	ready to accept	commands.
        public const uint	SM_F_SCARD_E_INVALID_VALUE              = 0x80100011;	//SCARD_E_INVALID_VALUE			 	//	One	or more	of the supplied	parameters values could	not	be properly	interpreted.
        public const uint	SM_F_SCARD_E_SYSTEM_CANCELLED           = 0x80100012;	//SCARD_E_SYSTEM_CANCELLED		 	//	The	action was cancelled by	the	system,	presumably to log off or shut down.
        public const uint	SM_F_SCARD_F_COMM_ERROR                 = 0x80100013;	//SCARD_F_COMM_ERROR				//	An internal	communications error has been detected.
        public const uint	SM_F_SCARD_F_UNKNOWN_ERROR              = 0x80100014;	//SCARD_F_UNKNOWN_ERROR			 	//	An internal	error has been detected, but the source	is unknown.
        public const uint	SM_F_SCARD_E_INVALID_ATR                = 0x80100015;	//SCARD_E_INVALID_ATR				//	An ATR obtained	from the registry is not a valid ATR string.
        public const uint	SM_F_SCARD_E_NOT_TRANSACTED             = 0x80100016;	//SCARD_E_NOT_TRANSACTED			//	An attempt was made	to end a non-existent transaction.
        public const uint	SM_F_SCARD_E_READER_UNAVAILABLE         = 0x80100017;	//SCARD_E_READER_UNAVAILABLE		//	The	specified reader is	not	currently available	for	use.
        public const uint	SM_F_SCARD_P_SHUTDOWN                   = 0x80100018;	//SCARD_P_SHUTDOWN				 	//	The	operation has been aborted to allow	the	server application to exit.
        public const uint	SM_F_SCARD_E_PCI_TOO_SMALL              = 0x80100019;	//SCARD_E_PCI_TOO_SMALL			 	//	The	PCI	Receive	buffer was too small.
        public const uint	SM_F_SCARD_E_READER_UNSUPPORTED         = 0x8010001A;	//SCARD_E_READER_UNSUPPORTED		//	The	reader driver does not meet	minimal	requirements for support.
        public const uint	SM_F_SCARD_E_DUPLICATE_READER           = 0x8010001B;	//SCARD_E_DUPLICATE_READER		 	//	The	reader driver did not produce a	unique reader name.
        public const uint	SM_F_SCARD_E_CARD_UNSUPPORTED           = 0x8010001C;	//SCARD_E_CARD_UNSUPPORTED		 	//	The	smart card does	not	meet minimal requirements for support.
        public const uint	SM_F_SCARD_E_NO_SERVICE                 = 0x8010001D;	//SCARD_E_NO_SERVICE				//	The	Smart card resource	manager	is not running.
        public const uint	SM_F_SCARD_E_SERVICE_STOPPED            = 0x8010001E;	//SCARD_E_SERVICE_STOPPED			//	The	Smart card resource	manager	has	shut down.
        public const uint	SM_F_SCARD_E_UNEXPECTED                 = 0x8010001F;	//SCARD_E_UNEXPECTED				//	An unexpected card error has occurred.
        public const uint	SM_F_SCARD_E_ICC_INSTALLATION           = 0x80100020;	//SCARD_E_ICC_INSTALLATION		 	//	No Primary Provider	can	be found for the smart card.
        public const uint	SM_F_SCARD_E_ICC_CREATEORDER            = 0x80100021;	//SCARD_E_ICC_CREATEORDER			//	The	requested order	of object creation is not supported.
        public const uint	SM_F_SCARD_E_UNSUPPORTED_FEATURE        = 0x80100022;	//SCARD_E_UNSUPPORTED_FEATURE		//	This smart card	does not support the requested feature.
        public const uint	SM_F_SCARD_E_DIR_NOT_FOUND              = 0x80100023;	//SCARD_E_DIR_NOT_FOUND			 	//	The	identified directory does not exist	in the smart card.
        public const uint	SM_F_SCARD_E_FILE_NOT_FOUND             = 0x80100024;	//SCARD_E_FILE_NOT_FOUND			//	The	identified file	does not exist in the smart	card.
        public const uint	SM_F_SCARD_E_NO_DIR                     = 0x80100025;	//SCARD_E_NO_DIR					//	The	supplied path does not represent a smart card directory.
        public const uint	SM_F_SCARD_E_NO_FILE                    = 0x80100026;	//SCARD_E_NO_FILE					//	The	supplied path does not represent a smart card file.
        public const uint	SM_F_SCARD_E_NO_ACCESS                  = 0x80100027;	//SCARD_E_NO_ACCESS				 	//	Access is denied to	this file.
        public const uint	SM_F_SCARD_E_WRITE_TOO_MANY             = 0x80100028;	//SCARD_E_WRITE_TOO_MANY			//	The	smartcard does not have	enough memory to store the information.
        public const uint	SM_F_SCARD_E_BAD_SEEK                   = 0x80100029;	//SCARD_E_BAD_SEEK				 	//	There was an error trying to set the smart card	file object	pointer.
        public const uint	SM_F_SCARD_E_INVALID_CHV                = 0x8010002A;	//SCARD_E_INVALID_CHV				//	The	supplied PIN is	incorrect.
        public const uint	SM_F_SCARD_E_UNKNOWN_RES_MNG            = 0x8010002B;	//SCARD_E_UNKNOWN_RES_MNG			//	An unrecognized	error code was returned	from a layered component.
        public const uint	SM_F_SCARD_E_NO_SUCH_CERTIFICATE        = 0x8010002C;	//SCARD_E_NO_SUCH_CERTIFICATE		//	The	requested certificate does not exist.
        public const uint	SM_F_SCARD_E_CERTIFICATE_UNAVAILABLE	= 0x8010002D;	//SCARD_E_CERTIFICATE_UNAVAILABLE	//	The	requested certificate could	not	be obtained.
        public const uint	SM_F_SCARD_E_NO_READERS_AVAILABLE       = 0x8010002E;	//SCARD_E_NO_READERS_AVAILABLE	 	//	Cannot find	a smart	card reader.
        public const uint	SM_F_SCARD_E_COMM_DATA_LOST             = 0x8010002F;	//SCARD_E_COMM_DATA_LOST			//	A communications error with	the	smart card has been	detected.  Retry the operation.
        public const uint	SM_F_SCARD_E_NO_KEY_CONTAINER           = 0x80100030;	//SCARD_E_NO_KEY_CONTAINER		 	//	The	requested key container	does not exist on the smart	card.
        public const uint	SM_F_SCARD_E_SERVER_TOO_BUSY            = 0x80100031;	//SCARD_E_SERVER_TOO_BUSY			//  The Smart card resource manager is too busy to complete this operation.

        // PC/SC SmartCard Service Warning Code...
        public const uint	SM_F_SCARD_W_UNSUPPORTED_CARD           = 0x80100065;	//SCARD_W_UNSUPPORTED_CARD		 
        public const uint	SM_F_SCARD_W_UNRESPONSIVE_CARD          = 0x80100066;	//SCARD_W_UNRESPONSIVE_CARD		 	//	The	smart card is not responding to	a reset.		
        public const uint	SM_F_SCARD_W_UNPOWERED_CARD             = 0x80100067;	//SCARD_W_UNPOWERED_CARD			//	Power has been removed from	the	smart card,	so that	further	communication is not possible.
        public const uint	SM_F_SCARD_W_RESET_CARD                 = 0x80100068;	//SCARD_W_RESET_CARD				//	The	smart card has been	reset, so any shared state information is invalid.
        public const uint	SM_F_SCARD_W_REMOVED_CARD               = 0x80100069;	//SCARD_W_REMOVED_CARD			 	//	The	smart card has been	removed, so	that further communication is not possible.
        public const uint	SM_F_SCARD_W_SECURITY_VIOLATION         = 0x8010006A;	//SCARD_W_SECURITY_VIOLATION		//	Access was denied because of a security	violation.
        public const uint	SM_F_SCARD_W_WRONG_CHV                  = 0x8010006B;	//SCARD_W_WRONG_CHV				 	//	The	card cannot	be accessed	because	the	wrong PIN was presented.
        public const uint	SM_F_SCARD_W_CHV_BLOCKED                = 0x8010006C;	//SCARD_W_CHV_BLOCKED				//	The	card cannot	be accessed	because	the	maximum	number of PIN entry	attempts has been reached.
        public const uint	SM_F_SCARD_W_EOF                        = 0x8010006D;	//SCARD_W_EOF						//	The	end	of the smart card file has been	reached.
        public const uint	SM_F_SCARD_W_CANCELLED_BY_USER          = 0x8010006E;	//SCARD_W_CANCELLED_BY_USER		 	//	The	action was cancelled by	the	user.
        public const uint	SM_F_SCARD_W_CARD_NOT_AUTHENTICATED     = 0x8010006F;	//SCARD_W_CARD_NOT_AUTHENTICATED	//  No PIN was presented to the smart card.
    }
}
