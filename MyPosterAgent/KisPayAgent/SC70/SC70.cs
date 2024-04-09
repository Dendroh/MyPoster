using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Drawing;


namespace SC70
{
    public struct SM7PRINTERITEM
    {
        public string name;
        public string id;
        public string dev;
        public string desc;
        public int pid;
    }

    /// <summary>
    /// C# wrapper structure of SMART70_PRINTER_LIST.
    /// @GetDeviceList
    /// </summary>
    public struct SM7PRINTERLIST
    {
        public int n;
        public SM7PRINTERITEM[] item;
    }




    public struct SM7_USB
    {
        public string port;     // usb port
        public string link;     // symbolic link of usb port
        public bool is_bridge;  // Network module bridge
    }

    public struct SM7_NET
    {
        public string ver;      // version of network protocol
        public string ip;       // ip address
        public int port;        // tcp port
        public bool is_ssl;     // ssl protocol
    }

    public struct SM7_STD
    {
        public string name;     // printer name
        public string id;       // printer ID
        public string dev;      // device connection
        public int dev_type;    // 1=USB, 2=NET
        public int pid;         // USB product ID
        public SM7_USB usb;
        public SM7_NET net;
    }

    public struct SM7_OPT
    {
        public bool is_dual;    // dual printer
        public string ic1;      // internal contact encoder
        public string ic2;      // external contact SIM encoder
        public string rf1;      // internal contactless encoder
        public string rf2;      // external contactless encoder
    }

    /// <summary>
    /// @GetDeviceInfo
    /// </summary>
    public struct SM7PRINTERINFO
    {
        public SM7_STD std;
        public SM7_OPT opt;
    }




    public struct SM7_INFPRN
    {
        public string ver;          // printer f/w ver
        public string serial;       // printer serial number
        public string hserial;      // thermal header serial number
    }

    public struct SM7_INFLAMI
    {
        public bool   installed;    // flag for laminator installation
        public string ver;          // laminator f/w ver
        public string serial;       // laminator serial number
    }

    /// <summary>
    /// @GetSystemInfo
    /// </summary>
    public struct SM7SYSINFO
    {
        public SM7_INFPRN    printer;
        public SM7_INFLAMI  laminator;
    }




    /// <summary>
    /// C# Wrapper Class for SmartComm70.dll
    /// </summary>
    public class CSM70
    {
        //private SmartComm70 m_smart;
        private IntPtr m_hsmart;
        private Int16  m_wprmt;
        private IntPtr[] m_hprevbmp_ptr;


        // class constructor
        public CSM70()
        {
            //m_smart = new SmartComm70();
            //string strpath = System.Reflection.Assembly.GetExecutingAssembly().Location;  // X:\\...\xxx.exe
            //  System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName; // X:\\...\xxx.vhost.exe
            //  System.Reflection.Assembly.GetExecutingAssembly().Location;         // X:\\...\xxx.exe
            //  System.Reflection.Assembly.GetCallingAssembly().Location;           // C:\\Windows\\Microsoft.NET\\Framework\\v2.0.50727\mscorlib.dll   // version may different
            //  System.Diagnostics.Process.GetCurrentProcess().ProcessName;         // X:\\...\xxx
            //  System.IO.Path.GetDirectoryName( ... );

            m_hsmart = IntPtr.Zero;
            m_wprmt = -1;
        }

        // class destructor
        ~CSM70()
        {
            if(m_hsmart != IntPtr.Zero)
            {
                if( IsOpened() )
                {
                    if( IsPreempted() )
                        ReleasePreemption();
                    DCLCloseDevice();
                    ReleasePreviewBitmapPtr();
                }
            }
        }




        // DEVICE LIST/INFO

        public void GetDeviceList(ref SM7PRINTERLIST list)
        {
            GetDeviceList(ref list, SmartComm70.SMART70_DEVICELIST_ALL);
        }

        public void GetDeviceList(ref SM7PRINTERLIST list, int opt)
        {
            SmartComm70.SMART_PRINTER_LIST smlist = new SmartComm70.SMART_PRINTER_LIST();
            IntPtr list_ptr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(SmartComm70.SMART_PRINTER_LIST)));
            SmartComm70.ExGetDeviceList(list_ptr, opt);
            smlist = (SmartComm70.SMART_PRINTER_LIST) Marshal.PtrToStructure(list_ptr, typeof(SmartComm70.SMART_PRINTER_LIST));
            list.n = smlist.n;
            list.item = new SM7PRINTERITEM[Math.Max(1, list.n)];
            for( int i=0; i<list.n; i++ ) {
                list.item[i].name = smlist.item[i].name;
                list.item[i].id = smlist.item[i].id;
                list.item[i].dev = smlist.item[i].dev;
                list.item[i].desc = smlist.item[i].desc;
                list.item[i].pid = smlist.item[i].pid;
            }
            Marshal.FreeHGlobal(list_ptr);
        }

        public void GetDeviceInfo(ref SM7PRINTERINFO info, string devdesc)
        {
            SmartComm70.SMART70_PRINTER_INFO sminfo = new SmartComm70.SMART70_PRINTER_INFO();
            IntPtr info_ptr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(SmartComm70.SMART70_PRINTER_INFO)));
            IntPtr dev_ptr = Marshal.StringToHGlobalUni(devdesc);
            SmartComm70.GetDeviceInfo(info_ptr, dev_ptr, SmartComm70.SMART70_OPENDEVICE_BYDESC);
            sminfo = (SmartComm70.SMART70_PRINTER_INFO) Marshal.PtrToStructure(info_ptr, typeof(SmartComm70.SMART70_PRINTER_INFO));
            info.std.name = sminfo.std.name;
            info.std.id = sminfo.std.id;
            info.std.dev = sminfo.std.dev;
            info.std.dev_type = sminfo.std.dev_type;
            info.std.pid = sminfo.std.pid;
            info.std.usb.port = sminfo.std.usb.port;
            info.std.usb.link = sminfo.std.usb.link;
            info.std.usb.is_bridge = (sminfo.std.usb.is_bridge != 0);
            info.std.net.ver = sminfo.std.net.ver;
            info.std.net.ip = sminfo.std.net.ip;
            info.std.net.port = sminfo.std.net.port;
            info.std.net.is_ssl = (sminfo.std.net.is_ssl != 0);
            info.opt.is_dual = (sminfo.opt.is_dual != 0);
            info.opt.ic1 = sminfo.opt.ic1;
            info.opt.ic2 = sminfo.opt.ic2;
            info.opt.rf1 = sminfo.opt.rf1;
            info.opt.rf2 = sminfo.opt.rf2;
            Marshal.FreeHGlobal(dev_ptr);
            Marshal.FreeHGlobal(info_ptr);
        }

        
        
        
        // OPEN/CLOSE

        public bool IsOpened()
        {
            return (m_hsmart != IntPtr.Zero);
        }

        public uint OpenDevice(string devdesc)
        {
            if (m_hsmart != IntPtr.Zero)
                Marshal.FreeHGlobal( m_hsmart );
            m_hsmart = IntPtr.Zero;
            IntPtr dev_ptr = Marshal.StringToHGlobalUni(devdesc);

            uint nres = SmartComm70.OpenDevice2(ref m_hsmart, dev_ptr, SmartComm70.SMART70_OPENDEVICE_BYDESC);
            if (nres == SmartComm70.SM_SUCCESS)
                PareparePreviewBitmapPtr();
            Marshal.FreeHGlobal(dev_ptr);
            return nres;
        }

        public uint CloseDevice()
        {
            if (!IsOpened())
                return SmartComm70.SM_SUCCESS;
            uint nres = SmartComm70.CloseDevice(m_hsmart);
            ReleasePreviewBitmapPtr();
            m_hsmart = IntPtr.Zero;
            return nres;
        }


        public uint DCLOpenDevice(string devdesc, int orient)
        {
            if (m_hsmart != IntPtr.Zero)
                Marshal.FreeHGlobal(m_hsmart);
            m_hsmart = IntPtr.Zero;
            IntPtr dev_ptr = Marshal.StringToHGlobalUni(devdesc);

            uint nres = SmartComm70.DCLOpenDevice2(ref m_hsmart, dev_ptr, SmartComm70.SMART70_OPENDEVICE_BYDESC, orient);
            if (nres == SmartComm70.SM_SUCCESS)
                PareparePreviewBitmapPtr();
            Marshal.FreeHGlobal(dev_ptr);
            return nres;
        }

        public uint DCLOpenDevice3(string devdesc, int port, bool bSSL, int orient)
        {
            if (m_hsmart != IntPtr.Zero)
                Marshal.FreeHGlobal(m_hsmart);
            m_hsmart = IntPtr.Zero;
            IntPtr dev_ptr = Marshal.StringToHGlobalUni(devdesc);

            uint nres = SmartComm70.DCLOpenDevice3(ref m_hsmart, dev_ptr, port, bSSL ? 1 : 0, orient);
            if (nres == SmartComm70.SM_SUCCESS)
                PareparePreviewBitmapPtr();
            Marshal.FreeHGlobal(dev_ptr);
            return nres;
        }

        public uint DCLCloseDevice()
        {
            if (!IsOpened())
                return SmartComm70.SM_SUCCESS;
            uint nres = SmartComm70.DCLCloseDevice(m_hsmart);
            m_hsmart = IntPtr.Zero;
            ReleasePreviewBitmapPtr();
            return nres;
        }

        
        
        
        // COMMON INFO.

        public uint GetModules(ref byte[] list)
        {
            IntPtr modlist_ptr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(SmartComm70.SMART70_DEVCONNLIST)));
            uint nres = SmartComm70.GetConnectedModules(m_hsmart, modlist_ptr);
            SmartComm70.SMART70_DEVCONNLIST modlist = new SmartComm70.SMART70_DEVCONNLIST();
            modlist = (SmartComm70.SMART70_DEVCONNLIST) Marshal.PtrToStructure(modlist_ptr, typeof(SmartComm70.SMART70_DEVCONNLIST));
            list = new byte[modlist.n];
            for (int i = 0; i < modlist.n; i++)
                list[i] = modlist.dev[i].id;
            Marshal.FreeHGlobal(modlist_ptr);
            return nres;
        }

        public uint GetRibbonType(byte mod, ref int type)
        {
            IntPtr int_ptr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(int)));
            uint nres = SmartComm70.GetRibbonInfo(m_hsmart, mod, int_ptr, IntPtr.Zero, IntPtr.Zero);
            type = Marshal.ReadInt32(int_ptr);
            Marshal.FreeHGlobal(int_ptr);
            return nres;
        }

        public uint GetRibbonRemain(byte mod, ref int remain)
        {
            IntPtr int_ptr = Marshal.AllocHGlobal(Marshal.SizeOf(remain));
            uint nres = SmartComm70.GetRibbonInfo(m_hsmart, mod, IntPtr.Zero, int_ptr, IntPtr.Zero);
            if(nres == SmartComm70.SM_SUCCESS)
                remain = Marshal.ReadInt32(int_ptr);

            Marshal.FreeHGlobal(int_ptr);
            return nres;

        }

        public uint GetRibbonMax(byte mod, ref int max)
        {
            IntPtr int_ptr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(int)));
            uint nres = SmartComm70.GetRibbonInfo(m_hsmart, mod, IntPtr.Zero, IntPtr.Zero, int_ptr);
            max = Marshal.ReadInt32(int_ptr);
            Marshal.FreeHGlobal(int_ptr);
            return nres;
        }

        public uint GetDisplayInfo(ref int type, ref int lang)
        {
            IntPtr type_ptr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(int)));
            IntPtr lang_ptr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(int)));
            uint nres = SmartComm70.GetDisplayInfo(m_hsmart, type_ptr, lang_ptr);
            type = Marshal.ReadInt32(type_ptr);
            lang = Marshal.ReadInt32(type_ptr);
            Marshal.FreeHGlobal(type_ptr);
            Marshal.FreeHGlobal(lang_ptr);
            return nres;
        }


        public uint GetSystemInfo(ref SM7SYSINFO sysinf)
        {
            IntPtr sminf_ptr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(SmartComm70.SMART_SYSINFO)));
            uint nres = SmartComm70.GetSystemInfo(m_hsmart, sminf_ptr);
            if (nres == SmartComm70.SM_SUCCESS)
            {
                SmartComm70.SMART_SYSINFO sminf = new SmartComm70.SMART_SYSINFO();
                sminf = (SmartComm70.SMART_SYSINFO) Marshal.PtrToStructure(sminf_ptr, typeof(SmartComm70.SMART_SYSINFO));
                sysinf.printer.ver = sminf.printer.ver;
                sysinf.printer.serial = sminf.printer.serial;
                sysinf.printer.hserial = sminf.printer.hserial;
                sysinf.laminator.installed = (sminf.laminator.installed != 0);
                sysinf.laminator.serial = sminf.laminator.serial;
            }
            Marshal.FreeHGlobal(sminf_ptr);
            return nres;
        }


        public uint GetState(byte mod, ref byte state)
        {
            IntPtr st_ptr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(Byte)));
            uint nres = SmartComm70.GetModuleState(m_hsmart, mod, st_ptr);
            state = Marshal.ReadByte(st_ptr);
            Marshal.FreeHGlobal(st_ptr);
            return nres;
        }

        public uint GetStatus(byte mod, ref Int64 status)
        {
            IntPtr st_ptr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(Int64)));
            uint nres = SmartComm70.GetStatus(m_hsmart, mod, st_ptr);
            status = Marshal.ReadInt64(st_ptr);
            Marshal.FreeHGlobal(st_ptr);
            return nres;
        }

        
        
        
        // DOCUMENT

        public uint OpenDocument(string szdoc)
        {
            IntPtr doc_ptr = Marshal.StringToHGlobalUni(szdoc);
            uint nres = SmartComm70.OpenDocument(m_hsmart, doc_ptr);
            Marshal.FreeHGlobal(doc_ptr);
            return nres;
        }

        public uint CloseDocument()
        {
            return SmartComm70.CloseDocument(m_hsmart);
        }

        public uint ClearDocument(byte page)
        {
            return SmartComm70.ClearDocument(m_hsmart, page);
        }


        public uint GetFieldCount(ref int count)
        {
            IntPtr cnt_ptr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(int)));
            uint nres = SmartComm70.GetFieldCount(m_hsmart, cnt_ptr);
            if (nres == SmartComm70.SM_SUCCESS)
                count = Marshal.ReadInt32(cnt_ptr);
            Marshal.FreeHGlobal(cnt_ptr);
            return nres;
        }

        public uint GetFieldName(int idx, ref string name)
        {
            int buflen = 32;
            char[] szbuf = new char[buflen];
            IntPtr name_ptr = Marshal.AllocHGlobal(buflen*Marshal.SizeOf(typeof(char)));
            uint nres = SmartComm70.GetFieldName(m_hsmart, idx, name_ptr, buflen);
            if (nres == SmartComm70.SM_SUCCESS)
                name = name_ptr.ToString();
            Marshal.FreeHGlobal(name_ptr);
            return nres;
        }

        public uint GetFieldValue(string name, ref string value)
        {
            int buflen = 1024;
            char[] szbuf = new char[buflen];
            IntPtr value_ptr = Marshal.AllocHGlobal(buflen*Marshal.SizeOf(typeof(char)));
            IntPtr name_ptr = Marshal.StringToHGlobalUni(name);
            uint nres = SmartComm70.GetFieldValue(m_hsmart, name_ptr, value_ptr);
            if (nres == SmartComm70.SM_SUCCESS)
                value = value_ptr.ToString();
            Marshal.FreeHGlobal(name_ptr);
            Marshal.FreeHGlobal(value_ptr);
            return nres;
        }

        public uint SetFieldValue(string name, string value)
        {
            IntPtr name_ptr = Marshal.StringToHGlobalUni(name);
            IntPtr value_ptr = Marshal.StringToHGlobalUni(value);
            uint nres = SmartComm70.SetFieldValue(m_hsmart, name_ptr, value_ptr);
            Marshal.FreeHGlobal(name_ptr);
            Marshal.FreeHGlobal(value_ptr);
            return nres;
        }

        public uint GetPrinterSettings(byte mod, ref SmartComm70.SMART70_DEVMODE dm)
        {
            IntPtr dm_ptr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(SmartComm70.SMART70_DEVMODE)));
            uint nres = SmartComm70.GetPrinterSettings(m_hsmart, mod, dm_ptr);
            if (nres == SmartComm70.SM_SUCCESS)
                dm = (SmartComm70.SMART70_DEVMODE) Marshal.PtrToStructure(dm_ptr, typeof(SmartComm70.SMART70_DEVMODE));
            Marshal.FreeHGlobal(dm_ptr);
            return nres;
        }

        public uint SetPrinterSettings(byte mod, SmartComm70.SMART70_DEVMODE dm)
        {
            IntPtr dm_ptr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(SmartComm70.SMART70_DEVMODE)));
            Marshal.StructureToPtr(dm, dm_ptr, true);
            uint nres = SmartComm70.SetPrinterSettings(m_hsmart, mod, dm_ptr);
            Marshal.FreeHGlobal(dm_ptr);
            return nres;
        }

        
        
        
        // DRAWING

        public uint DrawLine(byte page, byte panel, int x, int y, int cx, int cy, int color, int width, byte style)
        {
            Win32.RECT rc = new Win32.RECT(0, 0, 0, 0);
            rc.left = rc.top = rc.right = rc.bottom = 0;
            return DrawLine(page, panel, x, y, cx, cy, color, width, style, ref rc);
        }

        public uint DrawLine(byte page, byte panel, int x, int y, int cx, int cy, int color, int width, byte style, ref Win32.RECT rc)
        {
            SmartComm70.LLINE line = new SmartComm70.LLINE(page, panel, x, y, cx, cy, new SmartComm70.S70Border(style, width, color), new SmartComm70.S70BackGround(0,0) );
            SmartComm70.S70Laser laser = new SmartComm70.S70Laser(0, 0, 0, 0);
            return DrawLine(line, laser, ref rc);
        }

        public uint DrawLine(SmartComm70.LLINE line, SmartComm70.S70Laser laser, ref Win32.RECT rc)
        {
            IntPtr line_ptr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(SmartComm70.LLINE)));
            IntPtr laser_ptr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(SmartComm70.S70Laser)));
            IntPtr rc_ptr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(Win32.RECT)));
            Marshal.StructureToPtr(line, line_ptr, true);
            Marshal.StructureToPtr(laser, laser_ptr, true);
            uint nres = SmartComm70.LDrawLine(m_hsmart, line_ptr, laser_ptr, rc_ptr);
            if (nres == SmartComm70.SM_SUCCESS)
                rc = (Win32.RECT) Marshal.PtrToStructure(rc_ptr, typeof(Win32.RECT));
            Marshal.FreeHGlobal(rc_ptr);
            Marshal.FreeHGlobal(laser_ptr);
            Marshal.FreeHGlobal(line_ptr);
            return nres;
        }


        public uint DrawRect(byte page, byte panel, int x, int y, int cx, int cy, int bdcolor, int bdwidth, byte bdstyle, bool bkfill, int bkcolor)
        {
            Win32.RECT rc = new Win32.RECT(0, 0, 0, 0);
            return DrawRect(page, panel, x, y, cx, cy, bdcolor, bdwidth, bdstyle, bkfill, bkcolor, ref rc);
        }

        public uint DrawRect(byte page, byte panel, int x, int y, int cx, int cy, int bdcolor, int bdwidth, byte bdstyle, bool bkfill, int bkcolor, ref Win32.RECT rc)
        {
            SmartComm70.LRECT rect = new SmartComm70.LRECT(page, panel, x, y, cx, cy, new SmartComm70.S70Border(bdstyle, bdwidth, bdcolor), new SmartComm70.S70BackGround(0, 0));
            SmartComm70.S70Laser laser = new SmartComm70.S70Laser(0, 0, 0, 0);
            return DrawRect(rect, laser, ref rc);
        }

        public uint DrawRect(SmartComm70.LRECT rect, SmartComm70.S70Laser laser, ref Win32.RECT rc)
        {
            IntPtr rect_ptr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(SmartComm70.LRECT)));
            IntPtr laser_ptr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(SmartComm70.S70Laser)));
            IntPtr rc_ptr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(Win32.RECT)));
            Marshal.StructureToPtr(rect, rect_ptr, true);
            Marshal.StructureToPtr(laser, laser_ptr, true);
            uint nres = SmartComm70.LDrawRect(m_hsmart, rect_ptr, laser_ptr, rc_ptr);
            if (nres == SmartComm70.SM_SUCCESS)
                rc = (Win32.RECT)Marshal.PtrToStructure(rc_ptr, typeof(Win32.RECT));
            Marshal.FreeHGlobal(rc_ptr);
            Marshal.FreeHGlobal(laser_ptr);
            Marshal.FreeHGlobal(rect_ptr);
            return nres;
        }


        public uint DrawOval(byte page, byte panel, int x, int y, int cx, int cy, int bdcolor, int bdwidth, byte bdstyle, bool bkfill, int bkcolor)
        {
            Win32.RECT rc = new Win32.RECT(0, 0, 0, 0);
            return DrawOval(page, panel, x, y, cx, cy, bdcolor, bdwidth, bdstyle, bkfill, bkcolor, ref rc);
        }

        public uint DrawOval(byte page, byte panel, int x, int y, int cx, int cy, int bdcolor, int bdwidth, byte bdstyle, bool bkfill, int bkcolor, ref Win32.RECT rc)
        {
            SmartComm70.LOVAL oval = new SmartComm70.LOVAL(page, panel, x, y, cx, cy, new SmartComm70.S70Border(bdstyle, bdwidth, bdcolor), new SmartComm70.S70BackGround(0, 0));
            SmartComm70.S70Laser laser = new SmartComm70.S70Laser(0, 0, 0, 0);
            return DrawOval(oval, laser, ref rc);
        }

        public uint DrawOval(SmartComm70.LOVAL oval, SmartComm70.S70Laser laser, ref Win32.RECT rc)
        {
            IntPtr oval_ptr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(SmartComm70.LOVAL)));
            IntPtr laser_ptr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(SmartComm70.S70Laser)));
            IntPtr rc_ptr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(Win32.RECT)));
            Marshal.StructureToPtr(oval, oval_ptr, true);
            Marshal.StructureToPtr(laser, laser_ptr, true);
            uint nres = SmartComm70.LDrawOval(m_hsmart, oval_ptr, laser_ptr, rc_ptr);
            if (nres == SmartComm70.SM_SUCCESS)
                rc = (Win32.RECT)Marshal.PtrToStructure(rc_ptr, typeof(Win32.RECT));
            Marshal.FreeHGlobal(rc_ptr);
            Marshal.FreeHGlobal(laser_ptr);
            Marshal.FreeHGlobal(oval_ptr);
            return nres;
        }


        public uint DrawRRect(byte page, byte panel, int x, int y, int cx, int cy, int bdcolor, int bdwidth, byte bdstyle, bool bkfill, int bkcolor, int round)
        {
            Win32.RECT rc = new Win32.RECT(0, 0, 0, 0);
            return DrawRRect(page, panel, x, y, cx, cy, bdcolor, bdwidth, bdstyle, bkfill, bkcolor, round, ref rc);
        }

        public uint DrawRRect(byte page, byte panel, int x, int y, int cx, int cy, int bdcolor, int bdwidth, byte bdstyle, bool bkfill, int bkcolor, int round, ref Win32.RECT rc)
        {
            SmartComm70.LRRECT rrect = new SmartComm70.LRRECT(page, panel, x, y, cx, cy, new SmartComm70.S70Border(bdstyle, bdwidth, bdcolor), new SmartComm70.S70BackGround(0, 0),
                                                            round );
            SmartComm70.S70Laser laser = new SmartComm70.S70Laser(0, 0, 0, 0);
            return DrawRRect(rrect, laser, ref rc);
        }

        public uint DrawRRect(SmartComm70.LRRECT rrect, SmartComm70.S70Laser laser, ref Win32.RECT rc)
        {
            IntPtr rrect_ptr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(SmartComm70.LRRECT)));
            IntPtr laser_ptr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(SmartComm70.S70Laser)));
            IntPtr rc_ptr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(Win32.RECT)));
            Marshal.StructureToPtr(rrect, rrect_ptr, true);
            Marshal.StructureToPtr(laser, laser_ptr, true);
            uint nres = SmartComm70.LDrawRRect(m_hsmart, rrect_ptr, laser_ptr, rc_ptr);
            if (nres == SmartComm70.SM_SUCCESS)
                rc = (Win32.RECT)Marshal.PtrToStructure(rc_ptr, typeof(Win32.RECT));
            Marshal.FreeHGlobal(rc_ptr);
            Marshal.FreeHGlobal(laser_ptr);
            Marshal.FreeHGlobal(rrect_ptr);
            return nres;
        }


        public uint DrawText(byte page, byte panel, int x, int y, string facename, int size, int style, int color, string strdata)
        {
            Win32.RECT rc = new Win32.RECT(0, 0, 0, 0);
            return DrawText(page, panel, x, y, facename, size, style, color, strdata, ref rc);
        }

        public uint DrawText(byte page, byte panel, int x, int y, string facename, int size, int style, int color, string strdata, ref Win32.RECT rc)
        {
            SmartComm70.LTEXT text = new SmartComm70.LTEXT(page, panel, x, y, 1024, 1024, new SmartComm70.S70Border(SmartComm70.BS_NONE, 1, 0), new SmartComm70.S70BackGround(0, 0),
                                                            new SmartComm70.S70FontInfo(size, style, color, facename), (SmartComm70.OBJ_ALIGN_LEFT|SmartComm70.OBJ_ALIGN_TOP), IntPtr.Zero );
            IntPtr text_ptr = Marshal.StringToHGlobalUni(strdata);
            text.szText = text_ptr;
            SmartComm70.S70Laser laser = new SmartComm70.S70Laser(0, 0, 0, 0);
            uint nres = DrawText(text, laser, ref rc);
            Marshal.FreeHGlobal(text_ptr);
            return nres;
        }

        public uint DrawText(SmartComm70.LTEXT text, SmartComm70.S70Laser laser, ref Win32.RECT rc)
        {
            IntPtr text_ptr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(SmartComm70.LTEXT)));
            IntPtr laser_ptr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(SmartComm70.S70Laser)));
            IntPtr rc_ptr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(Win32.RECT)));
            Marshal.StructureToPtr(text, text_ptr, true);
            Marshal.StructureToPtr(laser, laser_ptr, true);
            uint nres = SmartComm70.LDrawText(m_hsmart, text_ptr, laser_ptr, rc_ptr);
            if (nres == SmartComm70.SM_SUCCESS)
                rc = (Win32.RECT)Marshal.PtrToStructure(rc_ptr, typeof(Win32.RECT));
            Marshal.FreeHGlobal(rc_ptr);
            Marshal.FreeHGlobal(laser_ptr);
            Marshal.FreeHGlobal(text_ptr);
            return nres;
        }


        public uint DrawBarcode(byte page, byte panel, int x, int y, int cx, int cy, string barname, int size, int color, string bardata)
        {
            Win32.RECT rc = new Win32.RECT(0, 0, 0, 0);
            return DrawBarcode(page, panel, x, y, cx, cy, barname, size, color, bardata, ref rc);
        }

        public uint DrawBarcode(byte page, byte panel, int x, int y, int cx, int cy, string barname, int size, int color, string bardata, ref Win32.RECT rc)
        {
            SmartComm70.LBAR bar = new SmartComm70.LBAR(page, panel, x, y, cx, cy, new SmartComm70.S70Border(SmartComm70.BS_NONE, 1, 0), new SmartComm70.S70BackGround(0, 0),
                                                        IntPtr.Zero, size, color, IntPtr.Zero, IntPtr.Zero, false, -1, 0);
            IntPtr name_ptr = Marshal.StringToHGlobalUni(barname);
            bar.szBarName = name_ptr;
            IntPtr bardata_ptr = Marshal.StringToHGlobalUni(bardata);
            bar.szData = bardata_ptr;
            bar.szPost = IntPtr.Zero;
            SmartComm70.S70Laser laser = new SmartComm70.S70Laser(0, 0, 0, 0);
            uint nres = DrawBarcode(bar, laser, ref rc);
            Marshal.FreeHGlobal(bardata_ptr);
            Marshal.FreeHGlobal(name_ptr);
            return nres;
        }

        public uint DrawBarcode(SmartComm70.LBAR bar, SmartComm70.S70Laser laser, ref Win32.RECT rc)
        {
            IntPtr bar_ptr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(SmartComm70.LBAR)));
            IntPtr laser_ptr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(SmartComm70.S70Laser)));
            IntPtr rc_ptr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(Win32.RECT)));
            Marshal.StructureToPtr(bar, bar_ptr, true);
            Marshal.StructureToPtr(laser, laser_ptr, true);
            uint nres = SmartComm70.LDrawBarcode(m_hsmart, bar_ptr, laser_ptr, rc_ptr);
            if (nres == SmartComm70.SM_SUCCESS)
                rc = (Win32.RECT)Marshal.PtrToStructure(rc_ptr, typeof(Win32.RECT));
            Marshal.FreeHGlobal(rc_ptr);
            Marshal.FreeHGlobal(laser_ptr);
            Marshal.FreeHGlobal(bar_ptr);
            return nres;
        }


        public uint DrawImage(byte page, byte panel, int x, int y, int cx, int cy, string imgpath)
        {
            Win32.RECT rc = new Win32.RECT(0, 0, 0, 0);
            return DrawImage(page, panel, x, y, cx, cy, imgpath, ref rc);
        }

        public uint DrawImage(byte page, byte panel, int x, int y, int cx, int cy, string imgpath, ref Win32.RECT rc)
        {
            SmartComm70.LIMG img = new SmartComm70.LIMG(page, panel, x, y, cx, cy, new SmartComm70.S70Border(SmartComm70.BS_NONE, 1, 0), new SmartComm70.S70BackGround(0, 0),
                                                        IntPtr.Zero, SmartComm70.IMGSCALE_FITHORZ, (SmartComm70.OBJ_ALIGN_CENTER | SmartComm70.OBJ_ALIGN_MIDDLE));
            IntPtr path_ptr = Marshal.StringToHGlobalUni(imgpath);
            img.szFile = path_ptr;
            SmartComm70.S70Laser laser = new SmartComm70.S70Laser(0, 0, 0, 0);
            uint nres = DrawImage(img, laser, ref rc);
            Marshal.FreeHGlobal(path_ptr);
            return nres;
        }

        public uint DrawImage(SmartComm70.LIMG img, SmartComm70.S70Laser laser, ref Win32.RECT rc)
        {
            IntPtr img_ptr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(SmartComm70.LIMG)));
            IntPtr laser_ptr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(SmartComm70.S70Laser)));
            IntPtr rc_ptr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(Win32.RECT)));
            Marshal.StructureToPtr(img, img_ptr, true);
            Marshal.StructureToPtr(laser, laser_ptr, true);
            uint nres = SmartComm70.LDrawImage(m_hsmart, img_ptr, laser_ptr, rc_ptr);
            if (nres == SmartComm70.SM_SUCCESS)
                rc = (Win32.RECT)Marshal.PtrToStructure(rc_ptr, typeof(Win32.RECT));
            Marshal.FreeHGlobal(rc_ptr);
            Marshal.FreeHGlobal(laser_ptr);
            Marshal.FreeHGlobal(img_ptr);
            return nres;
        }


        public uint DrawBitmap(byte page, byte panel, int x, int y, int cx, int cy, IntPtr hbmp_ptr)
        {
            Win32.RECT rc = new Win32.RECT(0, 0, 0, 0);
            return DrawBitmap(page, panel, x, y, cx, cy, hbmp_ptr, ref rc);
        }

        public uint DrawBitmap(byte page, byte panel, int x, int y, int cx, int cy, IntPtr hbmp_ptr, ref Win32.RECT rc)
        {
            SmartComm70.LBMP img = new SmartComm70.LBMP(page, panel, x, y, cx, cy, new SmartComm70.S70Border(SmartComm70.BS_NONE, 1, 0), new SmartComm70.S70BackGround(0, 0),
                                                        hbmp_ptr, SmartComm70.IMGSCALE_FITHORZ, (SmartComm70.OBJ_ALIGN_CENTER | SmartComm70.OBJ_ALIGN_MIDDLE));
            SmartComm70.S70Laser laser = new SmartComm70.S70Laser(0, 0, 0, 0);
            return DrawBitmap(img, laser, ref rc);
        }

        public uint DrawBitmap(SmartComm70.LBMP bmp, SmartComm70.S70Laser laser, ref Win32.RECT rc)
        {
            IntPtr bmp_ptr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(SmartComm70.LBMP)));
            IntPtr laser_ptr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(SmartComm70.S70Laser)));
            IntPtr rc_ptr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(Win32.RECT)));
            Marshal.StructureToPtr(bmp, bmp_ptr, true);
            Marshal.StructureToPtr(laser, laser_ptr, true);
            uint nres = SmartComm70.LDrawBitmap(m_hsmart, bmp_ptr, laser_ptr, rc_ptr);
            if (nres == SmartComm70.SM_SUCCESS)
                rc = (Win32.RECT)Marshal.PtrToStructure(rc_ptr, typeof(Win32.RECT));
            Marshal.FreeHGlobal(rc_ptr);
            Marshal.FreeHGlobal(laser_ptr);
            Marshal.FreeHGlobal(bmp_ptr);
            return nres;
        }


        public uint GetBarcodeCount(ref int count)
        {
            IntPtr cnt_ptr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(int)));
            uint nres = SmartComm70.GetBarcodeTypeCount(m_hsmart, cnt_ptr);
            if (nres == SmartComm70.SM_SUCCESS)
                count = Marshal.ReadInt32(cnt_ptr);
            Marshal.FreeHGlobal(cnt_ptr);
            return nres;
        }

        public uint GetBarcodeName(int idx, ref string barname)
        {
            int buflen = 32;
            char[] chbar = new char[buflen];
            IntPtr barname_ptr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(char))*buflen);
            uint nres = SmartComm70.GetBarcodeTypeName(m_hsmart, idx, barname_ptr, buflen);
            if (nres == SmartComm70.SM_SUCCESS)
                barname = barname_ptr.ToString();
            Marshal.FreeHGlobal(barname_ptr);
            return nres;
        }

        
        
        
        // DOCUMENT OBJECT MANIPULATION

        //[DllImport("SmartComm70.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "Smart70_GetUnitInfo2")]
        //public static extern uint GetUnitInfo2(IntPtr hsmart_ptr, IntPtr punit_ptr, int dir);    // HSMART70, S70UNITINFO2* pUnit, int dir

        //[DllImport("SmartComm70.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "Smart70_GetFieldLinkedUnitInfo2")]
        //public static extern uint GetFieldLinkedUnitInfo2(IntPtr hsmart_ptr, IntPtr szfield_ptr, IntPtr punit_ptr, int dir);    // HSMART70, WCHAR* szField, S70UNITINFO2* pUnit, int dir

        //[DllImport("SmartComm70.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "Smart70_SetUnitInfo2")]
        //public static extern uint SetUnitInfo2(IntPtr hsmart_ptr, IntPtr punit_ptr);    // HSMART70, WCHAR* szField, S70UNITINFO2* pUnit

        //[DllImport("SmartComm70.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "Smart70_GetUnitInfo2Direct")]
        //public static extern uint GetUnitInfo2Direct(IntPtr hsmart_ptr, IntPtr punit_ptr);    // HSMART70, S70UNITINFO2* pUnit





        // PREVIEW

        public uint GetPreviewBitmap(byte page, ref Bitmap bitmap)
        {
            //IntPtr bmi_ptr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(Int32)));
            IntPtr bmi_ptr = m_hprevbmp_ptr[page];
            Marshal.WriteInt32(bmi_ptr, 0);
            uint nres = SmartComm70.GetPreviewBitmap(m_hsmart, page, ref bmi_ptr);
            if (nres == SmartComm70.SM_SUCCESS && bmi_ptr != IntPtr.Zero)
            {
                // get BITMAPINFOHEADER only...
                Win32.BITMAPINFOHEADER bmihdr;
                IntPtr dib_ptr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(Win32.BITMAPINFOHEADER)));
                Win32.CopyMemory(dib_ptr.ToInt32(), bmi_ptr.ToInt32(), Marshal.SizeOf(typeof(Win32.BITMAPINFOHEADER)));
                bmihdr = (Win32.BITMAPINFOHEADER)Marshal.PtrToStructure(dib_ptr, typeof(Win32.BITMAPINFOHEADER));
                Marshal.FreeHGlobal(dib_ptr);

                // copy entire DIB datas from...
                dib_ptr = Marshal.AllocHGlobal(bmihdr.biSizeImage);
                Win32.CopyMemory(dib_ptr.ToInt32(), bmi_ptr.ToInt32(), bmihdr.biSizeImage);

                int pbits = 0;
                IntPtr hbmp_ptr = Win32.CreateDIBSection(
                                        Win32.CreateCompatibleDC(IntPtr.Zero),
                                        dib_ptr.ToInt32(),
                                        Win32.DIBRGBCOLORS,
                                        ref pbits,
                                        IntPtr.Zero,
                                        0);
                // copy pixels...
                Win32.CopyMemory(pbits, dib_ptr.ToInt32() + Marshal.SizeOf(typeof(Win32.BITMAPINFOHEADER)), bmihdr.biSizeImage);

                // create bitmap from hbitmap
                bitmap = Image.FromHbitmap(hbmp_ptr);

            }
            //Marshal.FreeHGlobal(bmi_ptr);
            return nres;
        }



        //[DllImport("SmartComm70.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "Smart70_GetDocumentBitmap")]
        //public static extern uint GetDocumentBitmap(IntPtr hsmart_ptr, Byte page, Byte panel, IntPtr phbmp_ptr);    // HSMART70, BYTE page, BYTE panel, HBITMAP* phbmp




        // PRINT

        public uint Print()
        {
            return SmartComm70.Print(m_hsmart);
        }

        public uint DCLPrint(bool dual)
        {
            return SmartComm70.DCLPrint(m_hsmart, dual?SmartComm70.SMART70_PRINTSIDE_BOTH:SmartComm70.SMART70_PRINTSIDE_FRONT);
        }

        
        
        
        // PRINT - JOB-SCHEDULER

        public uint SetFinCallback(IntPtr pfncb_ptr)
        {
            return SmartComm70.SetJobCallback(m_hsmart, pfncb_ptr);
        }

        public uint PrintJob(int opt, ref int jobid)
        {
            IntPtr jobid_ptr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(int)));
            uint nres = SmartComm70.PrintDocument(m_hsmart, opt, jobid_ptr);
            if (nres == SmartComm70.SM_SUCCESS)
                jobid = Marshal.ReadInt32(jobid_ptr);
            Marshal.FreeHGlobal(jobid_ptr);
            return nres;
        }

        public uint GetJobInfo(ref SmartComm70.JOBINFO jinfo)
        {
            IntPtr jinfo_ptr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(SmartComm70.JOBINFO)));
            Marshal.StructureToPtr(jinfo, jinfo_ptr, false );
            uint nres = SmartComm70.GetJobInfo(m_hsmart, jinfo_ptr);
            if (nres == SmartComm70.SM_SUCCESS)
                jinfo = (SmartComm70.JOBINFO) Marshal.PtrToStructure(jinfo_ptr, typeof(SmartComm70.JOBINFO));
            Marshal.FreeHGlobal(jinfo_ptr);
            return nres;
        }

        //[DllImport("SmartComm70.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "Smart70_GetJobInfoEx")]
        //public static extern uint GetJobInfo2(IntPtr hsmart_ptr, IntPtr pjobinfo2_ptr);    // HSMART70, JOBINFO_EX * pjobinf




        // PRINT - JOB_DESCRIPTOR

        public uint NewJobDescriptor(ref IntPtr hjobd_ptr, IntPtr pfncbErr_ptr)
        {
            return SmartComm70.NewJobDescriptor(m_hsmart, ref hjobd_ptr, pfncbErr_ptr);
        }

        public uint FreeJobDescriptor(ref IntPtr hjobd_ptr, IntPtr pfncbErr_ptr)
        {
            uint nres = SmartComm70.FreeJobDescriptor(m_hsmart, hjobd_ptr, pfncbErr_ptr);
            if (nres == SmartComm70.SM_SUCCESS)
                hjobd_ptr = IntPtr.Zero;
            return nres;
        }

        public uint AddJobItem(IntPtr hjobd_ptr, int jobtype, IntPtr jobparam_ptr)
        {
            return SmartComm70.AddJobItem(m_hsmart, hjobd_ptr, jobtype, jobparam_ptr);
        }

        public uint AddJobItemMove(IntPtr hjobd_ptr, byte modfrom, byte optfrom, byte modto, byte optto)
        {
            return SmartComm70.AddJobItemMove(m_hsmart, hjobd_ptr, modfrom, optfrom, modto, optto);
        }

        public uint AddJobItemFlip(IntPtr hjobd_ptr, byte modfrom, byte modflip, byte modto, byte opt)
        {
            return SmartComm70.AddJobItemFlip(m_hsmart, hjobd_ptr, modfrom, modflip, modto, opt);
        }

        public uint AddJobItemMagnetic(IntPtr hjobd_ptr, byte mod, int coer, IntPtr pfncbMag_ptr)
        {
            return SmartComm70.AddJobItemMagnetic(m_hsmart, hjobd_ptr, mod, coer, pfncbMag_ptr);
        }

        public uint AddJobItemContact(IntPtr hjobd_ptr, byte mod, IntPtr pfncbContact_ptr)
        {
            return SmartComm70.AddJobItemContact(m_hsmart, hjobd_ptr, mod, pfncbContact_ptr);
        }

        public uint AddJobItemContactless(IntPtr hjobd_ptr, byte mod, IntPtr pfncbCL_ptr)
        {
            return SmartComm70.AddJobItemContactless(m_hsmart, hjobd_ptr, mod, pfncbCL_ptr);
        }

        public uint AddJobItemScanImage(IntPtr hjobd_ptr, byte mod, int dpi, int mode, int side, int opt, IntPtr pfncbScanImg_ptr)
        {
            return SmartComm70.AddJobItemScanImage(m_hsmart, hjobd_ptr, mod, dpi, mode, side, opt, pfncbScanImg_ptr);
        }

        public uint AddJobItemScanBarcode(IntPtr hjobd_ptr, byte mod, int dpi, int mode, int side, int opt, IntPtr pfncbScanBar_ptr)
        {
            return SmartComm70.AddJobItemScanBarcode(m_hsmart, hjobd_ptr, mod, dpi, mode, side, opt, pfncbScanBar_ptr);
        }

        public uint AddJobItemPrint(IntPtr hjobd_ptr, byte mod, byte page, IntPtr pfncbPrint_ptr)
        {
            return SmartComm70.AddJobItemPrint(m_hsmart, hjobd_ptr, mod, page, pfncbPrint_ptr);
        }

        public uint AddJobItemLaminate(IntPtr hjobd_ptr, byte mod, byte page, IntPtr pfncbLami_ptr)
        {
            return SmartComm70.AddJobItemLaminate(m_hsmart, hjobd_ptr, mod, page, pfncbLami_ptr);
        }

        public uint AddJobItemUser(IntPtr hjobd_ptr, byte mod, int opt, IntPtr pfncbUser_ptr)
        {
            return SmartComm70.AddJobItemUser(m_hsmart, hjobd_ptr, mod, opt, pfncbUser_ptr);
        }

        public uint PrintJobD(IntPtr hjobd_ptr, ref int jobid)
        {
            IntPtr jobid_ptr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(int)));
            uint nres = SmartComm70.InsertJob(m_hsmart, hjobd_ptr, jobid_ptr);
            if (nres == SmartComm70.SM_SUCCESS)
                jobid = Marshal.ReadInt32(jobid_ptr);
            Marshal.FreeHGlobal(jobid_ptr);
            return nres;
        }

        
        
        
        // DEVICE ACTIONS

        public bool IsPreempted()
        {
            return (m_wprmt != -1);
        }

        public uint RequestPreemption()
        {
            IntPtr prmt_ptr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(Int16)));
            uint nres = SmartComm70.RequestPreemption(m_hsmart, prmt_ptr);
            if (nres == SmartComm70.SM_SUCCESS)
                m_wprmt = Marshal.ReadInt16(prmt_ptr);
            return nres;
        }

        public uint ReleasePreemption()
        {
            uint nres = SmartComm70.SM_SUCCESS;
            if( IsPreempted() )
                nres = SmartComm70.ReleasePreemption(m_hsmart, m_wprmt);
            if (nres == SmartComm70.SM_SUCCESS)
                m_wprmt = -1;
            return nres;
        }

        public uint ManualMode(byte mod, bool flag)
        {
            Int64 status = 0;
            IntPtr status_ptr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(Int64)));
            uint nres = SmartComm70.GetStatus(m_hsmart, mod, status_ptr);
            if (nres == SmartComm70.SM_SUCCESS)
                status = Marshal.ReadInt64(status_ptr);
            if( (flag && ((status & SmartComm70.S7PS_S_SBSMODE) == 0)) ||
                (!flag && ((status & SmartComm70.S7PS_S_SBSMODE) != 0)) )
            {
                nres = SmartComm70.ToggleManualMode(m_hsmart);
                System.Threading.Thread.Sleep( 100 );
            }
            Marshal.FreeHGlobal(status_ptr);
            return nres;
        }

        public uint IsModuleReady(byte mod, ref uint ready)
        {
            IntPtr ready_ptr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(int)));
            uint nres = SmartComm70.IsModuleReady(m_hsmart, mod, ready_ptr);
            if (nres == SmartComm70.SM_SUCCESS)
                ready = (uint)Marshal.ReadInt32(ready_ptr);
            Marshal.FreeHGlobal(ready_ptr);
            return nres;
        }

        public uint IsReadyInsertJob(ref uint ready)
        {
            IntPtr ready_ptr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(int)));
            uint nres = SmartComm70.IsReadyInsertJob(m_hsmart, ready_ptr);
            if (nres == SmartComm70.SM_SUCCESS)
                ready = (uint)Marshal.ReadInt32(ready_ptr);
            Marshal.FreeHGlobal(ready_ptr);
            return nres;
        }


        public uint CardIn(byte mod, int dir, int opt)
        {
            return SmartComm70.CardIn(m_hsmart, mod, dir, opt);
        }

        public uint CardOut(byte mod, int dir, int opt)
        {
            return SmartComm70.CardOut(m_hsmart, mod, dir, opt);
        }

        public uint CardMove(byte modfrom, byte optfrom, byte modto, byte optto, int cmdopt)
        {
            return SmartComm70.CardMove(m_hsmart, modfrom, optfrom, modto, optto, cmdopt);
        }

        public uint CardFlip(byte modfrom, byte modflip, byte modto)
        {
            return SmartComm70.CardFlip(m_hsmart, modfrom, modflip, modto);
        }

        //[DllImport("SmartComm70.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "Smart70_CardFlipStay")]
        //public static extern uint CardFlipStay(IntPtr hsmart_ptr, Byte modfrom, Byte modflip);    // HSMART70, BYTE modfrom, BYTE modflip

        public uint Flip(byte mod, int opt, int value)
        {
            return SmartComm70.Flip(m_hsmart, mod, opt, value);
        }

        public uint CardMovePosition(byte mod, int pos)
        {
            return SmartComm70.CardMovePosition(m_hsmart, mod, pos);
        }

        public uint CardMoveSensor(byte mod, int opt1, int value, int opt2)
        {
            return SmartComm70.CardMoveSensor(m_hsmart, mod, opt1, value, opt2);
        }

        public uint CardEject(byte modfrom)
        {
            return SmartComm70.CardEject(m_hsmart, modfrom);
        }

        public uint CardEject2(byte mod, int opt, byte modout, int optout)
        {
            return SmartComm70.CardEject2(m_hsmart, mod, opt, modout, optout);
        }

        public uint CardEjectBin(byte mod, byte modbin)
        {
            return SmartComm70.CardEjectBin(m_hsmart, mod, modbin);
        }



        //[DllImport("SmartComm70.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "Smart70_SetEncConfig")]
        //public static extern uint SetEncConfig(IntPtr hsmart_ptr, Byte mod, int config, int value);    // HSMART70, BYTE mod, int config, int value

        //[DllImport("SmartComm70.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "Smart70_GetEncConfig2")]
        //public static extern uint GetEncConfig2(IntPtr hsmart_ptr, Byte mod, int config, IntPtr pvalue_ptr, IntPtr plen_ptr);    // HSMART70, BYTE mod, int config, void* pvalue, int* plen

        //[DllImport("SmartComm70.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "Smart70_SetEncConfig2")]
        //public static extern uint SetEncConfig2(IntPtr hsmart_ptr, Byte mod, int config, IntPtr pvalue_ptr, int len);    // HSMART70, BYTE mod, int config, void* pvalue, int len

        //[DllImport("SmartComm70.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "Smart70_SetEncBuffer")]
        //public static extern uint SetEncBuffer(IntPtr hsmart_ptr, Byte mod, Byte track, int nbuflen, IntPtr szbuf_ptr);    // HSMART70, BYTE mod, BYTE track, int nbuflen, WCHAR* szbuf

        //[DllImport("SmartComm70.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "Smart70_WriteEncoder")]
        //public static extern uint WriteEncoder(IntPtr hsmart_ptr, Byte mod, Byte track, int opt);    // HSMART70, BYTE mod, BYTE track, int opt

        //[DllImport("SmartComm70.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "Smart70_ReadEncoder")]
        //public static extern uint ReadEncoder(IntPtr hsmart_ptr, Byte mod, Byte track, int opt);    // HSMART70, BYTE mod, BYTE track, int opt

        //[DllImport("SmartComm70.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "Smart70_FetchEncoder")]
        //public static extern uint FetchEncoder(IntPtr hsmart_ptr, Byte mod, Byte track);    // HSMART70, BYTE mod, BYTE track

        //[DllImport("SmartComm70.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "Smart70_GetEncBuffer")]
        //public static extern uint GetEncBuffer(IntPtr hsmart_ptr, Byte mod, Byte track, IntPtr pbufsize_ptr, IntPtr szbuf_ptr);    // HSMART70, BYTE mod, BYTE track, int * pnbufsize, WCHAR* szbuf

        //[DllImport("SmartComm70.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "Smart70_RunEncoder")]
        //public static extern uint RunEncoder(IntPtr hsmart_ptr, Byte mod, Byte track, int run, IntPtr pbuf_ptr, IntPtr pbuflen_ptr);    // HSMART70, BYTE mod, BYTE track, int run, void* pbuf, int* pbuflen

        //[DllImport("SmartComm70.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "Smart70_MagGetAllBufferEx")]
        //public static extern uint MagGetAllBuffer(IntPtr hsmart_ptr, int bT1, IntPtr pbuf1_ptr, IntPtr plen1_ptr, int bT2, IntPtr pbuf2_ptr, IntPtr plen2_ptr, int bT3, IntPtr pbuf3_ptr, IntPtr plen3_ptr, int bJIS, IntPtr pbufJ_ptr, IntPtr plenJ_ptr);    // HSMART70, BOOL, BYTE*, int*, BOOL, BYTE*, int*, BOOL, BYTE*, int*, BOOL, BYTE*, int*

        //[DllImport("SmartComm70.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "Smart70_MagGetAllCryptoBufferEx")]
        //public static extern uint MagGetAllCryptoBuffer(IntPtr hsmart_ptr, int bT1, IntPtr pbuf1_ptr, IntPtr plen1_ptr, int bT2, IntPtr pbuf2_ptr, IntPtr plen2_ptr, int bT3, IntPtr pbuf3_ptr, IntPtr plen3_ptr, int bJIS, IntPtr pbufJ_ptr, IntPtr plenJ_ptr, IntPtr pbtkey_ptr);    // HSMART70, BOOL, BYTE*, int*, BOOL, BYTE*, int*, BOOL, BYTE*, int*, BOOL, BYTE*, int*, BYTE* pKey

        //[DllImport("SmartComm70.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "Smart70_MagSetAllCryptoBufferEx")]
        //public static extern uint MagSetAllCryptoBuffer(IntPtr hsmart_ptr, int bT1, IntPtr pbuf1_ptr, int len1, int bT2, IntPtr pbuf2_ptr, int len2, int bT3, IntPtr pbuf3_ptr, int len3, int bJIS, IntPtr pbufJ_ptr, int lenJ, IntPtr pbtkey_ptr);    // HSMART70, BOOL, BYTE*, int, BOOL, BYTE*, int, BOOL, BYTE*, int, BOOL, BYTE*, int, BYTE* pKey

        // coer : SMART70_ENC_MAGCOERCIVITY_LOCO/..._HICO/..._SPCO
        public uint MagConfig(byte mod, int coer)
        {
            return SmartComm70.SetEncConfig(m_hsmart, mod, SmartComm70.SMART70_ENCCFG_MAG_COER, coer);
        }

        public uint MagWrite(byte mod, bool bT1, string szT1, bool bT2, string szT2, bool bT3, string szT3, bool bJIS, string szJIS)
        {
            IntPtr szT1_ptr = IntPtr.Zero;
            IntPtr szT2_ptr = IntPtr.Zero;
            IntPtr szT3_ptr = IntPtr.Zero;
            IntPtr szJIS_ptr = IntPtr.Zero;
            if(bT1) szT1_ptr = Marshal.StringToHGlobalUni(szT1);
            if(bT2) szT2_ptr = Marshal.StringToHGlobalUni(szT2);
            if(bT3) szT3_ptr = Marshal.StringToHGlobalUni(szT3);
            if(bJIS) szJIS_ptr = Marshal.StringToHGlobalUni(szJIS);
            uint nres = SmartComm70.MagWrite(m_hsmart, mod, bT1 ? 1 : 0, szT1_ptr, bT2 ? 1 : 0, szT2_ptr, bT3 ? 1 : 0, szT3_ptr, bJIS ? 1 : 0, szJIS_ptr);
            if(bT1) Marshal.FreeHGlobal(szT1_ptr);
            if(bT2) Marshal.FreeHGlobal(szT2_ptr);
            if(bT3) Marshal.FreeHGlobal(szT3_ptr);
            if(bJIS) Marshal.FreeHGlobal(szJIS_ptr);
            return nres;
        }

        public uint MagRead(byte mod, bool bT1, ref string szT1, int buflenT1, bool bT2, ref string szT2, int buflenT2, bool bT3, ref string szT3, int buflenT3, bool bJIS, ref string szJIS, int buflenJIS)
        {
            char[] chT1 = new char[Math.Max(1,buflenT1)];
            char[] chT2 = new char[Math.Max(1,buflenT2)];
            char[] chT3 = new char[Math.Max(1,buflenT3)];
            char[] chJIS = new char[Math.Max(1,buflenJIS)];
            IntPtr szT1_ptr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(char))*Math.Max(1,buflenT1));
            IntPtr szT2_ptr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(char))*Math.Max(1,buflenT2));
            IntPtr szT3_ptr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(char))*Math.Max(1,buflenT3));
            IntPtr szJIS_ptr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(char))*Math.Max(1,buflenJIS));
            uint nres = SmartComm70.MagRead(m_hsmart, mod, bT1 ? 1 : 0, szT1_ptr, buflenT1, bT2 ? 1 : 0, szT2_ptr, buflenT2, bT3 ? 1 : 0, szT3_ptr, buflenT3, bJIS ? 1 : 0, szJIS_ptr, buflenJIS);
            if (nres == SmartComm70.SM_SUCCESS)
            {
                if(bT1) szT1 = szT1_ptr.ToString();
                if(bT2) szT2 = szT2_ptr.ToString();
                if(bT3) szT3 = szT3_ptr.ToString();
                if(bJIS) szJIS = szJIS_ptr.ToString();
            }
            Marshal.FreeHGlobal(szT1_ptr);
            Marshal.FreeHGlobal(szT2_ptr);
            Marshal.FreeHGlobal(szT3_ptr);
            Marshal.FreeHGlobal(szJIS_ptr);
            return nres;
        }



        //[DllImport("SmartComm70.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "Smart70_ExUnlockPrinter2")]
        //public static extern uint UnlockPrinter(IntPtr hsmart_ptr, IntPtr szpw_ptr);    // HSMART70, WCHAR* szPW

        //[DllImport("SmartComm70.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "Smart70_ExLockPrinter2")]
        //public static extern uint LockPrinter(IntPtr hsmart_ptr, IntPtr szpw_ptr);    // HSMART70, WCHAR* szPW

        
        
        
        // DEVICE SETTINGS

        //[DllImport("SmartComm70.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "Smart70_GetCountParam")]
        //public static extern uint GetCountParam(IntPtr hsmart_ptr, IntPtr pcntparam_ptr);    // HSMART70, COUNTPARAM* pParam

        //[DllImport("SmartComm70.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "Smart70_GetCountParamEx")]
        //public static extern uint GetCountParam2(IntPtr hsmart_ptr, IntPtr pcntparam_ptr);    // HSMART70, COUNTPARAM* pParam

        //[DllImport("SmartComm70.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "Smart70_ReadUserMemory")]
        //public static extern uint ReadUserMemory(IntPtr hsmart_ptr, IntPtr szpw_ptr, int addr, IntPtr pdata_ptr);    // HSMART70, WCHAR* szPW, int addr, BYTE* btdata

        //[DllImport("SmartComm70.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "Smart70_WriteUserMemory")]
        //public static extern uint WriteUserMemory(IntPtr hsmart_ptr, IntPtr szpw_ptr, int addr, IntPtr pdata_ptr);    // HSMART70, WCHAR* szPW, int addr, BYTE* btdata




        // CONTACT/CONTACTLESS SMART-CARD

        public uint ICHContact(byte mod, bool flag)
        {
            if(flag)    return SmartComm70.ICHContact(m_hsmart, mod);
            return SmartComm70.ICHDiscontact(m_hsmart, mod);
        }

        public uint ICPowerOn(byte mod, byte[] btatr, ref int atrbuflen)
        {
            IntPtr btatr_ptr = Marshal.AllocHGlobal(atrbuflen);
            IntPtr atrlen_ptr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(int)));
            Marshal.WriteInt32(atrlen_ptr, atrbuflen);
            uint nres = SmartComm70.ICPowerOn(m_hsmart, mod, btatr_ptr, atrlen_ptr);
            if (nres == SmartComm70.SM_SUCCESS)
            {
                atrbuflen = Marshal.ReadInt32(atrlen_ptr);
                Marshal.Copy(btatr_ptr, btatr, 0, atrbuflen);
            }
            Marshal.FreeHGlobal(atrlen_ptr);
            Marshal.FreeHGlobal(btatr_ptr);
            return nres;
        }

        public uint ICPowerOff(byte mod)
        {
            return SmartComm70.ICPowerOff(m_hsmart, mod);
        }

        public uint ICTransmit(byte mod, byte[] apdu, int apdulen, byte[] recv, ref int recvbuflen)
        {
            IntPtr apdu_ptr = Marshal.AllocHGlobal(apdulen);
            Marshal.Copy(apdu, 0, apdu_ptr, apdulen);
            IntPtr recv_ptr = Marshal.AllocHGlobal(recvbuflen);
            IntPtr recvbuflen_ptr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(int)));
            Marshal.WriteInt32(recvbuflen_ptr, recvbuflen);
            uint nres = SmartComm70.ICTransmit(m_hsmart, mod, apdulen, apdu_ptr, recvbuflen_ptr, recv_ptr);
            if (nres == SmartComm70.SM_SUCCESS)
            {
                recvbuflen = Marshal.ReadInt32(recvbuflen_ptr);
                Marshal.Copy(recv_ptr, recv, 0, recvbuflen);
            }
            Marshal.FreeHGlobal(recvbuflen_ptr);
            Marshal.FreeHGlobal(recv_ptr);
            Marshal.FreeHGlobal(apdu_ptr);
            return nres;
        }



        public uint RFPowerOn(byte mod, byte[] btatr, ref int atrbuflen)
        {
            IntPtr btatr_ptr = Marshal.AllocHGlobal(atrbuflen);
            IntPtr atrlen_ptr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(int)));
            Marshal.WriteInt32(atrlen_ptr, atrbuflen);
            uint nres = SmartComm70.RFPowerOn(m_hsmart, mod, btatr_ptr, atrlen_ptr);
            if (nres == SmartComm70.SM_SUCCESS)
            {
                atrbuflen = Marshal.ReadInt32(atrlen_ptr);
                Marshal.Copy(btatr_ptr, btatr, 0, atrbuflen);
            }
            Marshal.FreeHGlobal(atrlen_ptr);
            Marshal.FreeHGlobal(btatr_ptr);
            return nres;
        }

        public uint RFPowerOff(byte mod)
        {
            return SmartComm70.RFPowerOff(m_hsmart, mod);
        }

        public uint RFTransmit(byte mod, byte[] apdu, int apdulen, byte[] recv, ref int recvbuflen)
        {
            IntPtr apdu_ptr = Marshal.AllocHGlobal(apdulen);
            Marshal.Copy(apdu, 0, apdu_ptr, apdulen);
            IntPtr recv_ptr = Marshal.AllocHGlobal(recvbuflen);
            IntPtr recvbuflen_ptr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(int)));
            Marshal.WriteInt32(recvbuflen_ptr, recvbuflen);
            uint nres = SmartComm70.RFTransmit(m_hsmart, mod, apdulen, apdu_ptr, recvbuflen_ptr, recv_ptr);
            if (nres == SmartComm70.SM_SUCCESS)
            {
                recvbuflen = Marshal.ReadInt32(recvbuflen_ptr);
                Marshal.Copy(recv_ptr, recv, 0, recvbuflen);
            }
            Marshal.FreeHGlobal(recvbuflen_ptr);
            Marshal.FreeHGlobal(recv_ptr);
            Marshal.FreeHGlobal(apdu_ptr);
            return nres;
        }

        
        
        
        // LASER DEVICE / DRAWING

        public uint LaserConnect()
        {
            return SmartComm70.LaserConnect();
        }

        public uint LaserDisconnect()
        {
            return SmartComm70.LaserDisconnect();
        }

        
        
        
        // ETC

        public uint SetConfig(int id, int value)
        {
            IntPtr value_ptr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(int)));
            Marshal.WriteInt32(value_ptr, value);
            uint nres = SmartComm70.SetConfig(m_hsmart, id, value_ptr);
            Marshal.FreeHGlobal(value_ptr);
            return nres;
        }

        public uint SetConfig(int id, string value)
        {
            IntPtr value_ptr = Marshal.StringToHGlobalUni(value);
            uint nres = SmartComm70.SetConfig(m_hsmart, id, value_ptr);
            Marshal.FreeHGlobal(value_ptr);
            return nres;
        }

        public uint GetConfig(int id, ref int value)
        {
            IntPtr value_ptr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(int)));
            uint nres = SmartComm70.GetConfig(m_hsmart, id, value_ptr);
            if (nres == SmartComm70.SM_SUCCESS)
                value = Marshal.ReadInt32(value_ptr);
            Marshal.FreeHGlobal(value_ptr);
            return nres;
        }

        public uint GetConfig(int id, ref string value)
        {
            char[] chvalue = new char[1024];
            IntPtr value_ptr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(char))*1024);
            uint nres = SmartComm70.GetConfig(m_hsmart, id, value_ptr);
            if (nres == SmartComm70.SM_SUCCESS)
            {
                int unilen = Win32.lstrlen(value_ptr);
                Marshal.Copy(value_ptr, chvalue, 0, unilen);
                value = new string(chvalue);
            }
            Marshal.FreeHGlobal(value_ptr);
            return nres;
        }



        private void PareparePreviewBitmapPtr()
        {
            m_hprevbmp_ptr = new IntPtr[SmartComm70.PAGE_COUNT];
            for (int page = 0; page < SmartComm70.PAGE_COUNT; page++)
                m_hprevbmp_ptr[page] = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(Int32)));
        }

        private void ReleasePreviewBitmapPtr()
        {
            if (m_hprevbmp_ptr.Length == 0)
                return;
            for (int page = 0; page < SmartComm70.PAGE_COUNT; page++)
                 Marshal.FreeHGlobal(m_hprevbmp_ptr[page]);
        }
    }




}


