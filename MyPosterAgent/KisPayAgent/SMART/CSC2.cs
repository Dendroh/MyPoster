using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Drawing;

namespace SC2
{
    public class CSC2
    {
        private IntPtr m_hsmart;
        private IntPtr[] m_hprevbmp_ptr;
        private string m_strDevice;

        public enum SMART_DEVGROUP : int {
            GROUP_SMART50,
            GROUP_SMART51,
            GROUP_SMART70
        };



        //____________________________________________________________________
        //

        public CSC2()
        {
            m_hsmart = IntPtr.Zero;
            m_hprevbmp_ptr = new IntPtr[0];
        }

        ~CSC2()
        {
            if (m_hsmart != IntPtr.Zero)
            {
                if (IsOpened())
                {
                    DCLCloseDevice();
                    ReleasePreviewBitmapPtr();
                }
            }
        }



        //____________________________________________________________________
        //
        // DEVICE LIST/INFO

        public void GetDeviceList(ref PRINTERLIST list)
        {
            GetDeviceList(ref list, SmartComm2.DEVICELIST_ALL);
        }

        public void GetDeviceList(ref PRINTERLIST list, int opt)
        {
            SmartComm2.SMART_PRINTER_LIST smlist = new SmartComm2.SMART_PRINTER_LIST();
            IntPtr list_ptr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(SmartComm2.SMART_PRINTER_LIST)));
            SmartComm2.GetDeviceList2Ex(list_ptr, opt);
            smlist = (SmartComm2.SMART_PRINTER_LIST)Marshal.PtrToStructure(list_ptr, typeof(SmartComm2.SMART_PRINTER_LIST));
            list.n = smlist.n;
            list.item = new PRINTERITEM[Math.Max(1, list.n)];
            for (int i = 0; i < list.n; i++)
            {
                list.item[i].name = smlist.item[i].name;
                list.item[i].id = smlist.item[i].id;
                list.item[i].dev = smlist.item[i].dev;
                list.item[i].desc = smlist.item[i].desc;
                list.item[i].pid = smlist.item[i].pid;
            }
            Marshal.FreeHGlobal(list_ptr);
        }

        public void GetDeviceInfo(ref PRINTERINFO info, string devdesc)
        {
            SmartComm2.SMART_PRINTER_INFO sminfo = new SmartComm2.SMART_PRINTER_INFO();
            IntPtr info_ptr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(SmartComm2.SMART_PRINTER_INFO)));
            IntPtr dev_ptr = Marshal.StringToHGlobalUni(devdesc);
            SmartComm2.GetDeviceInfo2(info_ptr, dev_ptr, SmartComm2.OPENDEVICE_BYDESC);
            sminfo = (SmartComm2.SMART_PRINTER_INFO)Marshal.PtrToStructure(info_ptr, typeof(SmartComm2.SMART_PRINTER_INFO));
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

        public SMART_DEVGROUP GetDeviceGroup(int pid)
        {
            CSC2.SMART_DEVGROUP group = CSC2.SMART_DEVGROUP.GROUP_SMART50;
            switch (pid >> 4)
            {
                case 0x385:
                case 0x386:
                case 0x387:
                case 0x388: group = CSC2.SMART_DEVGROUP.GROUP_SMART50; break;
                case 0x370: group = CSC2.SMART_DEVGROUP.GROUP_SMART70; break;
                default:    group = CSC2.SMART_DEVGROUP.GROUP_SMART51; break;
            }
            return group;
        }



        //____________________________________________________________________
        //
        // OPEN/CLOSE

        public bool IsOpened()
        {
            return (m_hsmart != IntPtr.Zero);
        }

        public uint OpenDevice(string devdesc)
        {
            if (m_hsmart != IntPtr.Zero)
                Marshal.FreeHGlobal(m_hsmart);
            m_hsmart = IntPtr.Zero;
            IntPtr dev_ptr = IntPtr.Zero;
            if( devdesc.Length > 0 )
                dev_ptr = Marshal.StringToHGlobalUni(devdesc);

            uint nres = SmartComm2.OpenDevice2Ex(ref m_hsmart, dev_ptr, SmartComm2.OPENDEVICE_BYDESC);
            if (nres == SmartComm2.SM_SUCCESS)
            {
                m_strDevice = devdesc;
                PareparePreviewBitmapPtr();
            }
            if( dev_ptr != IntPtr.Zero )
                Marshal.FreeHGlobal(dev_ptr);
            return nres;
        }

        public uint CloseDevice()
        {
            if (!IsOpened())
                return SmartComm2.SM_SUCCESS;
            uint nres = SmartComm2.CloseDevice(m_hsmart);
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

            uint nres = SmartComm2.DCLOpenDevice2(ref m_hsmart, dev_ptr, SmartComm2.OPENDEVICE_BYDESC, orient);
            if (nres == SmartComm2.SM_SUCCESS)
            {
                m_strDevice = devdesc;
                PareparePreviewBitmapPtr();
            }
            Marshal.FreeHGlobal(dev_ptr);
            return nres;
        }

        public uint DCLOpenDevice3(string devdesc, int port, bool bSSL, int orient)
        {
            if (m_hsmart != IntPtr.Zero)
                Marshal.FreeHGlobal(m_hsmart);
            m_hsmart = IntPtr.Zero;
            IntPtr dev_ptr = Marshal.StringToHGlobalUni(devdesc);

            //uint nres = SmartComm2.DCLOpenDevice3(ref m_hsmart, dev_ptr, port, bSSL ? 1 : 0, orient);
            uint nres = SmartComm2.DCLOpenDevice3(ref m_hsmart, dev_ptr, port, bSSL?1:0, orient);
            if (nres == SmartComm2.SM_SUCCESS)
            {
                m_strDevice = devdesc;
                PareparePreviewBitmapPtr();
            }
            Marshal.FreeHGlobal(dev_ptr);
            return nres;
        }

        public uint DCLCloseDevice()
        {
            if (!IsOpened())
                return SmartComm2.SM_SUCCESS;
            uint nres = SmartComm2.DCLCloseDevice(m_hsmart);
            m_hsmart = IntPtr.Zero;
            ReleasePreviewBitmapPtr();
            return nres;
        }




        // get information ...

        public uint GetStatus(ref ulong status)
        {
            IntPtr status_ptr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(Int64)));
            uint nres = SmartComm2.GetStatus(m_hsmart, status_ptr);
            if (nres == SmartComm2.SM_SUCCESS) {
                //status = (ulong)status_ptr.ToInt64();
                status = (ulong)Marshal.ReadInt64(status_ptr);
            }
            Marshal.FreeHGlobal(status_ptr);
            return nres;
        }

        public uint GetLamiStatus(ref ulong status)
        {
            int buflen = 32;
            IntPtr buf_ptr = Marshal.AllocHGlobal(buflen);
            IntPtr buflen_ptr = Marshal.AllocHGlobal(Marshal.SizeOf(buflen));
            Marshal.WriteInt32(buflen_ptr, buflen);
            uint nres = SmartComm2.GetLamiStatus(m_hsmart, buf_ptr, buflen_ptr);
            if (nres == SmartComm2.SM_SUCCESS)
            {
                buflen = Marshal.ReadInt32(buflen_ptr);//.ToInt32();
                if (buflen >= Marshal.SizeOf(status))
                    status = (ulong)Marshal.ReadInt64(buf_ptr);//.ToInt64();
                else
                    nres = SmartComm2.SM_F_INSUFFICIENTBUF;
            }
            Marshal.FreeHGlobal(buf_ptr);
            Marshal.FreeHGlobal(buflen_ptr);
            return nres;
        }

        public uint GetFlipStatus(ref ulong status)
        {
            int buflen = 32;
            IntPtr buf_ptr = Marshal.AllocHGlobal(buflen);
            IntPtr buflen_ptr = Marshal.AllocHGlobal(Marshal.SizeOf(buflen));
            Marshal.WriteInt32(buflen_ptr, buflen);
            uint nres = SmartComm2.GetFlipStatus(m_hsmart, buf_ptr, buflen_ptr);
            if (nres == SmartComm2.SM_SUCCESS)
            {
                buflen = Marshal.ReadInt32(buflen_ptr);//.ToInt32();
                if (buflen >= Marshal.SizeOf(status))
                    status = (ulong)Marshal.ReadInt64(buf_ptr);//.ToInt64();
                else
                    nres = SmartComm2.SM_F_INSUFFICIENTBUF;
            }
            Marshal.FreeHGlobal(buf_ptr);
            Marshal.FreeHGlobal(buflen_ptr);
            return nres;
        }

        public uint IsFlippable(SMART_DEVGROUP group, ref bool enable)
        {
            uint nres = SmartComm2.SM_SUCCESS;
            UInt64 status = 0;
            if (IsOpened() == false)
                nres = SmartComm2.SM_F_DEVNOTOPENED;

            if (nres == SmartComm2.SM_SUCCESS)
                nres = GetStatus(ref status);
            if (nres == SmartComm2.SM_SUCCESS)
            {
                enable = false;

                switch (group)
                {
                    case CSC2.SMART_DEVGROUP.GROUP_SMART50:
                        enable = ((status & (SmartComm2.SMSC_S_EQUIPROTATOR | SmartComm2.SMSC_S_EQUIPLAMINATOR)) != 0);
                        break;
                    case CSC2.SMART_DEVGROUP.GROUP_SMART51:
                        enable = ((status & (SmartComm2.S51PS_S_CONNFLIPPER | SmartComm2.S51PS_S_CONNLAMINATOR)) != 0);
                        break;
                }
            }
            return nres;
        }


        public uint GetVersion(ref string ver)
        {
            int buflen = 64;
            IntPtr szbuf_ptr = Marshal.AllocHGlobal(buflen);
            IntPtr nbuflen_ptr = Marshal.AllocHGlobal(Marshal.SizeOf(buflen));
            Marshal.WriteInt32(nbuflen_ptr, buflen);
            uint nres = SmartComm2.GetVersion(m_hsmart, szbuf_ptr, nbuflen_ptr);
            if (nres == SmartComm2.SM_SUCCESS)
                ver = szbuf_ptr.ToString();
            Marshal.FreeHGlobal(szbuf_ptr);
            Marshal.FreeHGlobal(nbuflen_ptr);
            return nres;
        }

        public uint GetLamiVersion(ref string ver)
        {
            int buflen = 64;
            IntPtr szbuf_ptr = Marshal.AllocHGlobal(buflen);
            IntPtr nbuflen_ptr = Marshal.AllocHGlobal(Marshal.SizeOf(buflen));
            Marshal.WriteInt32(nbuflen_ptr, buflen);
            uint nres = SmartComm2.GetLamiVersion(m_hsmart, szbuf_ptr, nbuflen_ptr);
            if (nres == SmartComm2.SM_SUCCESS)
                ver = szbuf_ptr.ToString();
            Marshal.FreeHGlobal(szbuf_ptr);
            Marshal.FreeHGlobal(nbuflen_ptr);
            return nres;
        }


        public uint GetTemperature(ref int temperature)
        {
            IntPtr tmpr_ptr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(short)));
            IntPtr rbcol_ptr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(short)));
            uint nres = SmartComm2.GetTemperature(m_hsmart, tmpr_ptr, rbcol_ptr);
            if (nres == SmartComm2.SM_SUCCESS)
                temperature = (int)Marshal.ReadInt16(tmpr_ptr);
            Marshal.FreeHGlobal(tmpr_ptr);
            Marshal.FreeHGlobal(rbcol_ptr);
            return nres;
        }


        public uint GetRibbonType(ref int type)
        {
            IntPtr type_ptr = Marshal.AllocHGlobal(Marshal.SizeOf(type));
            uint nres = SmartComm2.GetRibbonType(m_hsmart, type_ptr);
            if (nres == SmartComm2.SM_SUCCESS)
                type = Marshal.ReadInt32(type_ptr);
            Marshal.FreeHGlobal(type_ptr);
            return nres;
        }

        public uint GetRibbonRemain(ref int remain)
        {
            IntPtr remain_ptr = Marshal.AllocHGlobal(Marshal.SizeOf(remain));
            uint nres = SmartComm2.GetRibbonRemain(m_hsmart, remain_ptr);
            if (nres == SmartComm2.SM_SUCCESS)
                remain = Marshal.ReadInt32(remain_ptr);
            Marshal.FreeHGlobal(remain_ptr);
            return nres;
        }

        public uint GetRibbonInfo(ref int type, ref int max, ref int remain, ref int grade)
        {
            IntPtr type_ptr = Marshal.AllocHGlobal(Marshal.SizeOf(type));
            IntPtr max_ptr = Marshal.AllocHGlobal(Marshal.SizeOf(max));
            IntPtr remain_ptr = Marshal.AllocHGlobal(Marshal.SizeOf(remain));
            IntPtr grade_ptr = Marshal.AllocHGlobal(Marshal.SizeOf(grade));
            uint nres = SmartComm2.GetRibbonInfo(m_hsmart, type_ptr, max_ptr, remain_ptr, grade_ptr);
            if (nres == SmartComm2.SM_SUCCESS)
            {
                type = Marshal.ReadInt32(type_ptr);
                max = Marshal.ReadInt32(max_ptr);
                remain = Marshal.ReadInt32(remain_ptr);
                grade = Marshal.ReadInt32(grade_ptr);
            }
            Marshal.FreeHGlobal(type_ptr);
            Marshal.FreeHGlobal(max_ptr);
            Marshal.FreeHGlobal(remain_ptr);
            Marshal.FreeHGlobal(grade_ptr);
            return nres;
        }

        public uint GetDisplayInfo(ref int type, ref int lang)
        {
            IntPtr type_ptr = Marshal.AllocHGlobal(Marshal.SizeOf(type));
            IntPtr lang_ptr = Marshal.AllocHGlobal(Marshal.SizeOf(lang));
            uint nres = SmartComm2.GetDisplayInfo(m_hsmart, type_ptr, lang_ptr);
            if (nres == SmartComm2.SM_SUCCESS)
            {
                type = Marshal.ReadInt32(type_ptr);
                lang = Marshal.ReadInt32(lang_ptr);
            }
            Marshal.FreeHGlobal(type_ptr);
            Marshal.FreeHGlobal(lang_ptr);
            return nres;
        }

        public uint GetSystemInfo(ref SYSINFO sysinfo)
        {
            IntPtr sysinfo_ptr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(SmartComm2.SMART_SYSINFO)));
            uint nres = SmartComm2.GetSystemInfo(m_hsmart, sysinfo_ptr);
            if (nres == SmartComm2.SM_SUCCESS)
            {
                SmartComm2.SMART_SYSINFO    si = new SmartComm2.SMART_SYSINFO();
                si = (SmartComm2.SMART_SYSINFO) Marshal.PtrToStructure(sysinfo_ptr, typeof(SmartComm2.SMART_SYSINFO));

                sysinfo.printer_ver             = si.printer_ver;
                sysinfo.printer_serial          = si.printer_serial;
                sysinfo.printer_hserial         = si.printer_hserial;
                sysinfo.printer_cleaningWarning = si.printer_cleaningWarning;
		        sysinfo.printer_cleaning        = si.printer_cleaning;
	            sysinfo.printer_totalDensity    = si.printer_totalDensity;
	            sysinfo.printer_colorDensity    = si.printer_colorDensity;
	            sysinfo.printer_blackDensity    = si.printer_blackDensity;
	            sysinfo.printer_overlayDensity  = si.printer_overlayDensity;
	            sysinfo.laminator_installed     = (si.laminator_installed != 0);
                sysinfo.laminator_ver           = si.laminator_ver;
                sysinfo.laminator_serial        = si.laminator_serial;
            }
            Marshal.FreeHGlobal(sysinfo_ptr);
            return nres;
        }

        public uint GetVRInfo(int opt, ref int vendor, ref int region)
        {
            IntPtr vendor_ptr = Marshal.AllocHGlobal(Marshal.SizeOf(vendor));
            IntPtr region_ptr = Marshal.AllocHGlobal(Marshal.SizeOf(region));
            uint nres = SmartComm2.GetVRInfo(m_hsmart, opt, vendor_ptr, region_ptr);
            if (nres == SmartComm2.SM_SUCCESS)
            {
                vendor = Marshal.ReadInt32(vendor_ptr);
                region = Marshal.ReadInt32(region_ptr);
            }
            Marshal.FreeHGlobal(vendor_ptr);
            Marshal.FreeHGlobal(region_ptr);
            return nres;
        }

        public uint GetPanelDensity(byte panel, ref int density)
        {
            IntPtr density_ptr = Marshal.AllocHGlobal(Marshal.SizeOf(density));
            uint nres = SmartComm2.GetPanelDensity(m_hsmart, panel, density_ptr);
            if (nres == SmartComm2.SM_SUCCESS)
                density = Marshal.ReadInt32(density_ptr);
            Marshal.FreeHGlobal(density_ptr);
            return nres;
        }

        public uint SetPanelDensity(byte panel, int density, string pw)
        {
            IntPtr pw_ptr = Marshal.StringToHGlobalUni(pw);
            uint nres = SmartComm2.SetPanelDensity(m_hsmart, panel, density, pw_ptr);
            Marshal.FreeHGlobal(pw_ptr);
            return nres;
        }

        public uint GetSlowPrint(ref bool flag)
        {
            IntPtr flag_ptr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(int)));
            uint nres = SmartComm2.GetSlowPrint(m_hsmart, flag_ptr);
            if (nres == SmartComm2.SM_SUCCESS)
            {
                int flagBOOL = Marshal.ReadInt32(flag_ptr);
                flag = (flagBOOL != 0);
            }
            Marshal.FreeHGlobal(flag_ptr);
            return nres;
        }

        public uint SetSlowPrint(int flag, string pw)
        {
            IntPtr pw_ptr = Marshal.StringToHGlobalUni(pw);
            uint nres = SmartComm2.SetSlowPrint(m_hsmart, flag, pw_ptr);
            Marshal.FreeHGlobal(pw_ptr);
            return nres;
        }

        public uint ReadUserMemory(string pw, int addr, ref byte[] data)
        {
            IntPtr pw_ptr = Marshal.StringToHGlobalUni(pw);
            IntPtr data_ptr = Marshal.AllocHGlobal(4);
            uint nres = SmartComm2.ReadUserMemory(m_hsmart, pw_ptr, addr, data_ptr);
            if (nres == SmartComm2.SM_SUCCESS)
            {
                data = new byte[4];
                Marshal.Copy(data_ptr, data, 0, 4);
            }
            Marshal.FreeHGlobal(pw_ptr);
            Marshal.FreeHGlobal(data_ptr);
            return nres;
        }

        public uint WriteUserMemory(string pw, int addr, byte[] data)
        {
            IntPtr pw_ptr = Marshal.StringToHGlobalUni(pw);
            IntPtr data_ptr = Marshal.AllocHGlobal(4);
            Marshal.Copy(data, 0, data_ptr, 4);
            uint nres = SmartComm2.WriteUserMemory(m_hsmart, pw_ptr, addr, data_ptr);
            Marshal.FreeHGlobal(pw_ptr);
            Marshal.FreeHGlobal(data_ptr);
            return nres;
        }




        // device control ...

        public uint SBSStart()
        {
            return SmartComm2.SBSStart(m_hsmart);
        }

        public uint SBSEnd()
        {
            return SmartComm2.SBSEnd(m_hsmart);
        }

        public uint CardIn()
        {
            return SmartComm2.CardIn(m_hsmart);
        }

        public uint CardInBack()
        {
            return SmartComm2.CardInBack(m_hsmart);
        }

        public uint CardOut()
        {
            return SmartComm2.CardOut(m_hsmart);
        }

        public uint CardOutBack()
        {
            return SmartComm2.CardOutBack(m_hsmart);
        }

        public uint CardOutBackAngle(int angle)
        {
            return SmartComm2.CardOutBackAngle(m_hsmart, angle);
        }

        public uint Move(int pos)
        {
            return SmartComm2.Move(m_hsmart, pos);
        }

        public uint MoveFromIn(int dist)
        {
            return SmartComm2.MoveFromIn(m_hsmart, dist);
        }

        public uint MoveFromOut(int dist)
        {
            return SmartComm2.MoveFromOut(m_hsmart, dist);
        }

        public uint MoveFromRotateIn(int dist)
        {
            return SmartComm2.MoveFromRotateIn(m_hsmart, dist);
        }

        public uint MoveSensor(int sensor, int accel, int dist, int speed)
        {
            return SmartComm2.MoveSensor(m_hsmart, sensor, accel, dist, speed);
        }

        public uint MovingScan(short dist, short speed, short speed2)
        {
            return SmartComm2.MovingScan(m_hsmart, dist, speed, speed2);
        }



        public uint DoPrint()
        {
            return SmartComm2.DoPrint(m_hsmart);
        }

        public uint ICHContact()
        {
            return SmartComm2.ICHContact(m_hsmart);
        }

        public uint ICHDiscontact()
        {
            return SmartComm2.ICHDiscontact(m_hsmart);
        }

        public uint Flip()
        {
            return SmartComm2.Flip(m_hsmart);
        }

        public uint DoCleaning()
        {
            return SmartComm2.DoCleaning(m_hsmart);
        }

        public uint ClearStatus()
        {
            return SmartComm2.ClearStatus(m_hsmart);
        }

        public uint Reboot()
        {
            return SmartComm2.Reboot(m_hsmart);
        }

        public uint SetLCDText(int type, int line, string text)
        {
            IntPtr text_ptr = Marshal.StringToHGlobalUni(text);
            uint nres = SmartComm2.SetLCDText(m_hsmart, type, line, text_ptr);
            Marshal.FreeHGlobal(text_ptr);
            return nres;
        }



        public uint LockPrinter2(string pw)
        {
            IntPtr pw_ptr = Marshal.StringToHGlobalUni(pw);
            uint nres = SmartComm2.LockPrinter2(m_hsmart, pw_ptr);
            Marshal.FreeHGlobal(pw_ptr);
            return nres;
        }

        public uint UnlockPrinter2(string pw)
        {
            IntPtr pw_ptr = Marshal.StringToHGlobalUni(pw);
            uint nres = SmartComm2.UnlockPrinter2(m_hsmart, pw_ptr);
            Marshal.FreeHGlobal(pw_ptr);
            return nres;
        }

        public uint UnlockPrinter3(string pw, int unlocktime)
        {
            IntPtr pw_ptr = Marshal.StringToHGlobalUni(pw);
            uint nres = SmartComm2.UnlockPrinter3(m_hsmart, pw_ptr, unlocktime);
            Marshal.FreeHGlobal(pw_ptr);
            return nres;
        }



        public uint MagConfig(SmartComm2.MAGCFG magcfg)
        {
            IntPtr magcfg_ptr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(SmartComm2.MAGCFG)));
            Marshal.StructureToPtr(magcfg, magcfg_ptr, false);
            uint nres = SmartComm2.MagConfig(m_hsmart, magcfg_ptr);
            Marshal.FreeHGlobal(magcfg_ptr);
            return nres;
        }

        public uint MagReadAction(int track)
        {
            return SmartComm2.MagReadAction(m_hsmart, track);
        }

        public uint MagReadAction2(int track, int opt)
        {
            return SmartComm2.MagReadAction2(m_hsmart, track, opt);
        }

        public uint MagGetBuffer(int track, ref string data)
        {
            /*
            int buflen = 1024;
            IntPtr buflen_ptr = Marshal.AllocHGlobal(Marshal.SizeOf(buflen));
            Marshal.WriteInt32(buflen_ptr, buflen);
            IntPtr buf_ptr = Marshal.AllocHGlobal(buflen);
            uint nres = SmartComm2.MagGetBuffer(m_hsmart, track, buf_ptr, buflen_ptr);
            if(nres == SmartComm2.SM_SUCCESS )
            {
                buflen = Marshal.ReadInt32(buflen_ptr);
                byte[] buf = new byte[buflen+1];
                Marshal.Copy(buf_ptr, buf, 0, buflen);
                data = Encoding.Unicode.GetString(buf);    // System.Text.UnicodeEncoding.Unicode.GetString
            }
            Marshal.FreeHGlobal(buflen_ptr);
            Marshal.FreeHGlobal(buf_ptr);
            return nres;
            //*/
            byte[] buf = new byte[1024];
            uint nres = MagGetBuffer(track, ref buf);
            if (nres == SmartComm2.SM_SUCCESS)
            {
                int len = buf.Length;
                byte[] buf2 = new byte[len + 1];
                buf.CopyTo(buf2, 0);
                data = Encoding.Unicode.GetString(buf2);    // System.Text.UnicodeEncoding.Unicode.GetString
            }
            else
                data = "";
            return nres;
        }

        public uint MagGetBuffer(int track, ref byte[] data)
        {
            int buflen = 1024;
            IntPtr buflen_ptr = Marshal.AllocHGlobal(Marshal.SizeOf(buflen));
            Marshal.WriteInt32(buflen_ptr, buflen);
            IntPtr buf_ptr = Marshal.AllocHGlobal(buflen);
            uint nres = SmartComm2.MagGetBuffer(m_hsmart, track, buf_ptr, buflen_ptr);
            if(nres == SmartComm2.SM_SUCCESS )
            {
                buflen = Marshal.ReadInt32(buflen_ptr);
                data = new byte[buflen];
                Marshal.Copy(buf_ptr, data, 0, buflen);
            }
            else
                data = new byte[0];
            Marshal.FreeHGlobal(buflen_ptr);
            Marshal.FreeHGlobal(buf_ptr);
            return nres;
        }

        public uint MagGetAllBuffer(bool bT1, ref string strT1, bool bT2, ref string strT2, bool bT3, ref string strT3, bool bJIS, ref string strJIS)
        {
            int t1len = 1024;
            IntPtr t1len_ptr = Marshal.AllocHGlobal(Marshal.SizeOf(t1len));
            Marshal.WriteInt32(t1len_ptr, t1len);
            IntPtr t1_ptr = Marshal.AllocHGlobal(t1len);

            int t2len = 1024;
            IntPtr t2len_ptr = Marshal.AllocHGlobal(Marshal.SizeOf(t2len));
            Marshal.WriteInt32(t2len_ptr, t2len);
            IntPtr t2_ptr = Marshal.AllocHGlobal(t2len);

            int t3len = 1024;
            IntPtr t3len_ptr = Marshal.AllocHGlobal(Marshal.SizeOf(t3len));
            Marshal.WriteInt32(t3len_ptr, t3len);
            IntPtr t3_ptr = Marshal.AllocHGlobal(t3len);

            int jislen = 1024;
            IntPtr jislen_ptr = Marshal.AllocHGlobal(Marshal.SizeOf(jislen));
            Marshal.WriteInt32(jislen_ptr, jislen);
            IntPtr jis_ptr = Marshal.AllocHGlobal(jislen);

            uint nres = SmartComm2.MagGetAllBuffer(m_hsmart,
                                                    bT1 ? 1 : 0, t1_ptr, t1len_ptr,
                                                    bT2 ? 1 : 0, t2_ptr, t2len_ptr,
                                                    bT3 ? 1 : 0, t3_ptr, t3len_ptr,
                                                    bJIS ? 1 : 0, jis_ptr, jislen_ptr);
            if (nres == SmartComm2.SM_SUCCESS)
            {
                if (bT1)
                {
                    t1len = Marshal.ReadInt32(t1len_ptr);
                    byte[] t1buf = new byte[t1len + 1];
                    Marshal.Copy(t1len_ptr, t1buf, 0, t1len);
                    strT1 = Encoding.Unicode.GetString(t1buf);    // System.Text.UnicodeEncoding.Unicode.GetString
                }
                if (bT2)
                {
                    t2len = Marshal.ReadInt32(t2len_ptr);
                    byte[] t2buf = new byte[t2len + 1];
                    Marshal.Copy(t2len_ptr, t2buf, 0, t2len);
                    strT2 = Encoding.Unicode.GetString(t2buf);    // System.Text.UnicodeEncoding.Unicode.GetString
                }
                if (bT3)
                {
                    t3len = Marshal.ReadInt32(t3len_ptr);
                    byte[] t3buf = new byte[t3len + 1];
                    Marshal.Copy(t3len_ptr, t3buf, 0, t3len);
                    strT3 = Encoding.Unicode.GetString(t3buf);    // System.Text.UnicodeEncoding.Unicode.GetString
                }
                if (bJIS)
                {
                    jislen = Marshal.ReadInt32(jislen_ptr);
                    byte[] jisbuf = new byte[jislen + 1];
                    Marshal.Copy(jislen_ptr, jisbuf, 0, jislen);
                    strJIS = Encoding.Unicode.GetString(jisbuf);    // System.Text.UnicodeEncoding.Unicode.GetString
                }
            }
            else
            {
                strT1 =
                strT2 =
                strT3 =
                strJIS = "";
            }
            Marshal.FreeHGlobal(t1len_ptr);
            Marshal.FreeHGlobal(t1_ptr);
            Marshal.FreeHGlobal(t2len_ptr);
            Marshal.FreeHGlobal(t2_ptr);
            Marshal.FreeHGlobal(t3len_ptr);
            Marshal.FreeHGlobal(t3_ptr);
            Marshal.FreeHGlobal(jislen_ptr);
            Marshal.FreeHGlobal(jis_ptr);
            return nres;
        }

        public uint MagGetAllCryptoBuffer(bool bT1, ref string strT1, bool bT2, ref string strT2, bool bT3, ref string strT3, bool bJIS, ref string strJIS, byte[] key)
        {
            int t1len = 1024;
            IntPtr t1len_ptr = Marshal.AllocHGlobal(Marshal.SizeOf(t1len));
            Marshal.WriteInt32(t1len_ptr, t1len);
            IntPtr t1_ptr = Marshal.AllocHGlobal(t1len);

            int t2len = 1024;
            IntPtr t2len_ptr = Marshal.AllocHGlobal(Marshal.SizeOf(t2len));
            Marshal.WriteInt32(t2len_ptr, t2len);
            IntPtr t2_ptr = Marshal.AllocHGlobal(t2len);

            int t3len = 1024;
            IntPtr t3len_ptr = Marshal.AllocHGlobal(Marshal.SizeOf(t3len));
            Marshal.WriteInt32(t3len_ptr, t3len);
            IntPtr t3_ptr = Marshal.AllocHGlobal(t3len);

            int jislen = 1024;
            IntPtr jislen_ptr = Marshal.AllocHGlobal(Marshal.SizeOf(jislen));
            Marshal.WriteInt32(jislen_ptr, jislen);
            IntPtr jis_ptr = Marshal.AllocHGlobal(jislen);

            IntPtr key_ptr = IntPtr.Zero;
            if(key != null)
            {
                key_ptr = Marshal.AllocHGlobal(key.Length);
                Marshal.Copy(key, 0, key_ptr, key.Length);
            }

            uint nres = SmartComm2.MagGetAllCryptoBuffer(m_hsmart,
                                                        bT1 ? 1 : 0, t1_ptr, t1len_ptr,
                                                        bT2 ? 1 : 0, t2_ptr, t2len_ptr,
                                                        bT3 ? 1 : 0, t3_ptr, t3len_ptr,
                                                        bJIS ? 1 : 0, jis_ptr, jislen_ptr,
                                                        key_ptr);
            if (nres == SmartComm2.SM_SUCCESS)
            {
                if (bT1)
                {
                    t1len = Marshal.ReadInt32(t1len_ptr);
                    byte[] t1buf = new byte[t1len + 1];
                    Marshal.Copy(t1len_ptr, t1buf, 0, t1len);
                    strT1 = Encoding.Unicode.GetString(t1buf);    // System.Text.UnicodeEncoding.Unicode.GetString
                }
                if (bT2)
                {
                    t2len = Marshal.ReadInt32(t2len_ptr);
                    byte[] t2buf = new byte[t2len + 1];
                    Marshal.Copy(t2len_ptr, t2buf, 0, t2len);
                    strT2 = Encoding.Unicode.GetString(t2buf);    // System.Text.UnicodeEncoding.Unicode.GetString
                }
                if (bT3)
                {
                    t3len = Marshal.ReadInt32(t3len_ptr);
                    byte[] t3buf = new byte[t3len + 1];
                    Marshal.Copy(t3len_ptr, t3buf, 0, t3len);
                    strT3 = Encoding.Unicode.GetString(t3buf);    // System.Text.UnicodeEncoding.Unicode.GetString
                }
                if (bJIS)
                {
                    jislen = Marshal.ReadInt32(jislen_ptr);
                    byte[] jisbuf = new byte[jislen + 1];
                    Marshal.Copy(jislen_ptr, jisbuf, 0, jislen);
                    strJIS = Encoding.Unicode.GetString(jisbuf);    // System.Text.UnicodeEncoding.Unicode.GetString
                }
            }
            else
            {
                strT1 =
                strT2 =
                strT3 =
                strJIS = "";
            }
            Marshal.FreeHGlobal(t1len_ptr);
            Marshal.FreeHGlobal(t1_ptr);
            Marshal.FreeHGlobal(t2len_ptr);
            Marshal.FreeHGlobal(t2_ptr);
            Marshal.FreeHGlobal(t3len_ptr);
            Marshal.FreeHGlobal(t3_ptr);
            Marshal.FreeHGlobal(jislen_ptr);
            Marshal.FreeHGlobal(jis_ptr);
            Marshal.FreeHGlobal(key_ptr);
            return nres;
        }


        public uint MagWriteAction(int track, bool highco)
        {
            return SmartComm2.MagWriteAction(m_hsmart, track, highco ? 1 : 0);
        }

        public uint MagWriteAction2(IntPtr hsmart_ptr, int track, int opt, int nBPIT1, int nBPIT2, int nBPIT3)
        {
            return SmartComm2.MagWriteAction2(m_hsmart, track, opt, nBPIT1, nBPIT2, nBPIT3);
        }

        public uint MagSetBuffer(int track, string data)
        {
            byte[] buf = new byte[0];
            UniStr2MultiByte(ref buf, data);
            IntPtr buf_ptr = Marshal.AllocHGlobal(buf.Length);
            Marshal.Copy(buf, 0, buf_ptr, buf.Length);
            uint nres = SmartComm2.MagSetBuffer(m_hsmart, track, buf_ptr, buf.Length);
            Marshal.FreeHGlobal(buf_ptr);
            return nres;
        }
/*
        public static extern uint MagSetAllBuffer(IntPtr hsmart_ptr, int bT1, IntPtr pbufT1, int nlenT1, int bT2, IntPtr pbufT2, int nlenT2, int bT3, IntPtr pbufT3, int nlenT3, int bTJ, IntPtr pbufTJ, int nlenTJ);    // HSMART, BOOL, BYTE*, int, BOOL, BYTE*, int, BOOL, BYTE*, int, BOOL, BYTE*, int
        public static extern uint MagBitModeGetBPI(int BPI, IntPtr pnewBPI_ptr);    // int, int*
        public static extern uint MagBitModeGetBPI2(IntPtr hsmart_ptr, int BPI, IntPtr pnewBPI_ptr);    // HSMART, int, int*
        public static extern uint MagBitModeGetMaxSize(int BPI, IntPtr pnmaxsize_ptr);    // int, int*
        public static extern uint MagBitModeGetMaxSize2(IntPtr hsmart_ptr, int BPI, IntPtr pnmaxsize_ptr);    // HSMART, int, int*
        public static extern uint MagGetCryptoBuffer(IntPtr hsmart_ptr, int track, IntPtr pbuf_ptr, IntPtr pninoutlen_ptr, IntPtr pkey_ptr);    // HSMART, int, BYTE*, int*, BYTE*
        public static extern uint MagGetAllCryptoBuffer(IntPtr hsmart_ptr, int bT1, IntPtr pbufT1_ptr, IntPtr pnlenT1_ptr, int bT2, IntPtr pbufT2_ptr, IntPtr pnlenT2_ptr, int bT3, IntPtr pbufT3_ptr, IntPtr pnlenT3_ptr, IntPtr pkey_ptr);    // HSMART, BOOL, BYTE*, int*, BOOL, BYTE*, int*, BOOL, BYTE*, int*, BYTE*
        public static extern uint MagSetCryptoBuffer(IntPtr hsmart_ptr, int track, IntPtr pbuf_ptr, int nlen, IntPtr pkey_ptr);    // HSMART, int, BYTE*, int, BYTE*
        public static extern uint MagSetAllCryptoBuffer(IntPtr hsmart_ptr, int bT1, IntPtr pbufT1_ptr, int nlenT1, int bT2, IntPtr pbufT2_ptr, int nlenT2, int bT3, IntPtr pbufT3_ptr, int nlenT3, int bTJ, IntPtr pbufTJ_ptr, int nlenTJ, IntPtr pkey_ptr);    // HSMART, BOOL, BYTE*, int, BOOL, BYTE*, int, BOOL, BYTE*, int, BOOL, BYTE*, int, BYTE*
*/

        public uint CmdRecv(byte[] cmd, int cmdlen, ref byte[] rcv)
        {
            IntPtr cmd_ptr = Marshal.AllocHGlobal(cmdlen);
            Marshal.Copy(cmd, 0, cmd_ptr, cmdlen);
            IntPtr rcv_ptr = Marshal.AllocHGlobal(rcv.Length);
            IntPtr rcvlen_ptr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(int)));
            Marshal.WriteInt32(rcvlen_ptr, rcv.Length);
            uint nres = SmartComm2.CmdRecv(m_hsmart, cmd_ptr, cmdlen, rcv_ptr, rcvlen_ptr);
            if (nres == SmartComm2.SM_SUCCESS)
            {
                int rcvlen = Marshal.ReadInt32(rcvlen_ptr);
                rcv = new byte[rcvlen];
                Marshal.Copy(rcv_ptr, rcv, 0, rcvlen);
            }
            Marshal.FreeHGlobal(cmd_ptr);
            Marshal.FreeHGlobal(rcv_ptr);
            Marshal.FreeHGlobal(rcvlen_ptr);
            return nres;
        }

        public uint CmdSend(byte[] cmd, int cmdlen)
        {
            IntPtr cmd_ptr = Marshal.AllocHGlobal(cmdlen);
            Marshal.Copy(cmd, 0, cmd_ptr, cmdlen);
            uint nres = SmartComm2.CmdSend(m_hsmart, cmd_ptr, cmdlen);
            Marshal.FreeHGlobal(cmd_ptr);
            return nres;
        }




        // document/drawing ...

        public uint OpenDocument(string csd)
        {
            IntPtr csd_ptr = Marshal.StringToHGlobalUni(csd);
            uint nres = SmartComm2.OpenDocument(m_hsmart, csd_ptr);
            Marshal.FreeHGlobal(csd_ptr);
            return nres;
        }

        public uint CloseDocument()
        {
            return SmartComm2.CloseDocument(m_hsmart);
        }

        public uint GetFieldCount(ref int count)
        {
            IntPtr count_ptr = Marshal.AllocHGlobal(Marshal.SizeOf(count));
            uint nres = SmartComm2.GetFieldCount(m_hsmart, count_ptr);
            if (nres == SmartComm2.SM_SUCCESS)
                count = Marshal.ReadInt32(count_ptr);
            Marshal.FreeHGlobal(count_ptr);
            return nres;
        }

        public uint GetFieldName(int idx, ref string name)
        {
            int bufsize = 64;
            name = "";
            for (int i = 0; i < bufsize; i++)
                name += " ";
            IntPtr name_ptr = Marshal.StringToHGlobalUni(name);
            uint nres = SmartComm2.GetFieldName(m_hsmart, idx, name_ptr, bufsize);
            if (nres == SmartComm2.SM_SUCCESS)
                name = Marshal.PtrToStringUni(name_ptr);
            name.Trim();
            Marshal.FreeHGlobal(name_ptr);
            return nres;
        }

        public uint GetFieldValue(string name, ref string value)
        {
            int buflen = 1024;
            value = "";
            for (int i = 0; i < buflen; i++)
                value += " ";
            IntPtr name_ptr = Marshal.StringToHGlobalUni(name);
            IntPtr value_ptr = Marshal.StringToHGlobalUni(value);
            uint nres = SmartComm2.GetFieldValue(m_hsmart, name_ptr, value_ptr);
            if (nres == SmartComm2.SM_SUCCESS)
                value = Marshal.PtrToStringUni(value_ptr);
            value.Trim();
            Marshal.FreeHGlobal(name_ptr);
            Marshal.FreeHGlobal(value_ptr);
            return nres;
        }

        public uint SetFieldValue(string name, string value)
        {
            IntPtr name_ptr = Marshal.StringToHGlobalUni(name);
            IntPtr value_ptr = Marshal.StringToHGlobalUni(value);
            uint nres = SmartComm2.SetFieldValue(m_hsmart, name_ptr, value_ptr);
            Marshal.FreeHGlobal(name_ptr);
            Marshal.FreeHGlobal(value_ptr);
            return nres;
        }

        public uint GetPrinterSettings(ref SmartComm2.SMART50_DEVMODE dm)
        {
            IntPtr dm_ptr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(SmartComm2.SMART50_DEVMODE)));
            uint nres = SmartComm2.GetPrinterSettings(m_hsmart, dm_ptr);
            if (nres == SmartComm2.SM_SUCCESS)
                dm = (SmartComm2.SMART50_DEVMODE) Marshal.PtrToStructure(dm_ptr, typeof(SmartComm2.SMART50_DEVMODE));
            Marshal.FreeHGlobal(dm_ptr);
            return nres;
        }

        public uint SetPrinterSettings(SmartComm2.SMART50_DEVMODE dm)
        {
            IntPtr dm_ptr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(SmartComm2.SMART50_DEVMODE)));
            Marshal.StructureToPtr(dm, dm_ptr, false);
            uint nres = SmartComm2.SetPrinterSettings(m_hsmart, dm_ptr);
            Marshal.FreeHGlobal(dm_ptr);
            return nres;
        }

        public uint GetPrinterSettings2(ref SmartComm2.SMART50_DEVMODE dm)
        {
            int dmlen = Marshal.SizeOf(typeof(SmartComm2.SMART50_DEVMODE));
            IntPtr dmlen_ptr = Marshal.AllocHGlobal(Marshal.SizeOf(dmlen));
            Marshal.WriteInt32(dmlen_ptr, dmlen);
            IntPtr dm_ptr = Marshal.AllocHGlobal(dmlen);
            uint nres = SmartComm2.GetPrinterSettings2(m_hsmart, dm_ptr, dmlen_ptr);
            if (nres == SmartComm2.SM_SUCCESS)
                dm = (SmartComm2.SMART50_DEVMODE) Marshal.PtrToStructure(dm_ptr, typeof(SmartComm2.SMART50_DEVMODE));
            Marshal.FreeHGlobal(dm_ptr);
            Marshal.FreeHGlobal(dmlen_ptr);
            return nres;
        }

        public uint GetPrinterSettings2(ref SmartComm2.SMART51_DEVMODE dm)
        {
            int dmlen = Marshal.SizeOf(typeof(SmartComm2.SMART51_DEVMODE));
            IntPtr dmlen_ptr = Marshal.AllocHGlobal(Marshal.SizeOf(dmlen));
            Marshal.WriteInt32(dmlen_ptr, dmlen);
            IntPtr dm_ptr = Marshal.AllocHGlobal(dmlen);
            uint nres = SmartComm2.GetPrinterSettings2(m_hsmart, dm_ptr, dmlen_ptr);
            if (nres == SmartComm2.SM_SUCCESS)
                dm = (SmartComm2.SMART51_DEVMODE) Marshal.PtrToStructure(dm_ptr, typeof(SmartComm2.SMART51_DEVMODE));
            Marshal.FreeHGlobal(dm_ptr);
            Marshal.FreeHGlobal(dmlen_ptr);
            return nres;
        }

        public uint SetPrinterSettings2(SmartComm2.SMART50_DEVMODE dm)
        {
            int dmlen = Marshal.SizeOf(typeof(SmartComm2.SMART50_DEVMODE));
            IntPtr dm_ptr = Marshal.AllocHGlobal(dmlen);
            Marshal.StructureToPtr(dm, dm_ptr, false);
            uint nres = SmartComm2.SetPrinterSettings2(m_hsmart, dm_ptr, dmlen);
            Marshal.FreeHGlobal(dm_ptr);
            return nres;
        }

        public uint SetPrinterSettings2(SmartComm2.SMART51_DEVMODE dm)
        {
            int dmlen = Marshal.SizeOf(typeof(SmartComm2.SMART51_DEVMODE));
            IntPtr dm_ptr = Marshal.AllocHGlobal(dmlen);
            Marshal.StructureToPtr(dm, dm_ptr, false);
            uint nres = SmartComm2.SetPrinterSettings2(m_hsmart, dm_ptr, dmlen);
            Marshal.FreeHGlobal(dm_ptr);
            return nres;
        }


        private uint GetDevModeBuffer(String drv, ref byte[] btdm, ref int dmlen)
        {
            IntPtr drv_ptr = Marshal.StringToHGlobalUni(drv);

            // acquire entire length of Extended DEVMODE
            int need = Win32.DocumentProperties(IntPtr.Zero, IntPtr.Zero, drv_ptr, IntPtr.Zero, IntPtr.Zero, 0);
            if (need <= 0)
            {
                Marshal.FreeHGlobal(drv_ptr);
                return SmartComm2.SM_F_FOUNDNODRV;
            }
            dmlen = need;

            // acqure Extended DEVMODE from driver
            IntPtr dm_ptr = Marshal.AllocHGlobal(dmlen);
            int nres = Win32.DocumentProperties(IntPtr.Zero, IntPtr.Zero, drv_ptr, dm_ptr, IntPtr.Zero, Win32.DM_OUT_BUFFER );
            if (nres <= 0)
            {
                Marshal.FreeHGlobal(drv_ptr);
                Marshal.FreeHGlobal(dm_ptr);
                return SmartComm2.SM_F_FOUNDNODRV;
            }

            // alloc buffer and copy acquired to buffer
            btdm = new byte[dmlen];
            Marshal.Copy(dm_ptr, btdm, 0, need); 

            Marshal.FreeHGlobal(drv_ptr);
            Marshal.FreeHGlobal(dm_ptr);
            return SmartComm2.SM_SUCCESS;
        }

        private uint Buffer2DevMode(byte[] btdm, ref SmartComm2.SMART50_DEVMODE dm)
        {
            Type type;
            int size = 0;

            // DEVMODE
            dm.devmode = new Win32.DEVMODEW();
            type = typeof(Win32.DEVMODEW);
            size = Marshal.SizeOf(type);
            IntPtr devmode_ptr = Marshal.AllocHGlobal(size);
            Marshal.Copy(btdm, 0, devmode_ptr, size);
            dm.devmode = (Win32.DEVMODEW) Marshal.PtrToStructure(devmode_ptr, type);
            Marshal.FreeHGlobal(devmode_ptr);

            // OEMDEV50
            dm.oemdev = new SmartComm2.OEMDEV50();
            type = typeof(SmartComm2.OEMDEV50);
            size = Marshal.SizeOf(type);
            IntPtr oemdev_ptr = Marshal.AllocHGlobal(size);
            Marshal.Copy(btdm, dm.devmode.dmSize + dm.devmode.dmDriverExtra - size, oemdev_ptr, size);
            dm.oemdev = (SmartComm2.OEMDEV50) Marshal.PtrToStructure(oemdev_ptr, type);
            Marshal.FreeHGlobal(oemdev_ptr);

            return SmartComm2.SM_SUCCESS;
        }

        private uint Buffer2DevMode(byte[] btdm, ref SmartComm2.SMART51_DEVMODE dm)
        {
            Type type;
            int size = 0;

            // DEVMODE
            dm.devmode = new Win32.DEVMODEW();
            type = typeof(Win32.DEVMODEW);
            size = Marshal.SizeOf(type);
            IntPtr devmode_ptr = Marshal.AllocHGlobal(size);
            Marshal.Copy(btdm, 0, devmode_ptr, size);
            dm.devmode = (Win32.DEVMODEW) Marshal.PtrToStructure(devmode_ptr, type);
            Marshal.FreeHGlobal(devmode_ptr);

            dm.reserved = new byte[564];

            // OEMDEV51
            dm.oemdev = new SmartComm2.OEMDEV51();
            type = typeof(SmartComm2.OEMDEV51);
            size = Marshal.SizeOf(type);
            IntPtr oemdev_ptr = Marshal.AllocHGlobal(size);
            Marshal.Copy(btdm, dm.devmode.dmSize + dm.devmode.dmDriverExtra - size, oemdev_ptr, size);
            dm.oemdev = (SmartComm2.OEMDEV51) Marshal.PtrToStructure(oemdev_ptr, type);
            Marshal.FreeHGlobal(oemdev_ptr);

            return SmartComm2.SM_SUCCESS;
        }

        private uint DevMode2Buffer(SmartComm2.SMART50_DEVMODE dm, ref byte[] btdm)
        {
            Type type;
            int size = 0;

            if (btdm == null )
                return SmartComm2.SM_F_INVALIDPARAM;

            if (btdm.Length < dm.devmode.dmSize + dm.devmode.dmDriverExtra)
                return SmartComm2.SM_F_INSUFFICIENTBUF;

            // DEVMODE
            type = typeof(Win32.DEVMODEW);
            size = Marshal.SizeOf(type);
            IntPtr devmode_ptr = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(dm.devmode, devmode_ptr, false);
            Marshal.Copy(devmode_ptr, btdm, 0, size);
            Marshal.FreeHGlobal(devmode_ptr);

            // OEMDEV50
            type = typeof(SmartComm2.OEMDEV50);
            size = Marshal.SizeOf(type);
            IntPtr oemdev_ptr = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(dm.oemdev, oemdev_ptr, false);
            Marshal.Copy(oemdev_ptr, btdm, dm.devmode.dmSize + dm.devmode.dmDriverExtra - size, size);
            Marshal.FreeHGlobal(oemdev_ptr);

            return SmartComm2.SM_SUCCESS;
        }

        private uint DevMode2Buffer(SmartComm2.SMART51_DEVMODE dm, ref byte[] btdm)
        {
            Type type;
            int size = 0;

            if (btdm == null)
                return SmartComm2.SM_F_INVALIDPARAM;

            if (btdm.Length < dm.devmode.dmSize + dm.devmode.dmDriverExtra)
                return SmartComm2.SM_F_INSUFFICIENTBUF;

            // DEVMODE
            type = typeof(Win32.DEVMODEW);
            size = Marshal.SizeOf(type);
            IntPtr devmode_ptr = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(dm.devmode, devmode_ptr, false);
            Marshal.Copy(devmode_ptr, btdm, 0, size);
            Marshal.FreeHGlobal(devmode_ptr);

            // OEMDEV50
            type = typeof(SmartComm2.OEMDEV51);
            size = Marshal.SizeOf(type);
            IntPtr oemdev_ptr = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(dm.oemdev, oemdev_ptr, false);
            Marshal.Copy(oemdev_ptr, btdm, dm.devmode.dmSize + dm.devmode.dmDriverExtra - size, size);
            Marshal.FreeHGlobal(oemdev_ptr);

            return SmartComm2.SM_SUCCESS;
        }


        public uint GetDriverSettings(String drv50, ref SmartComm2.SMART50_DEVMODE dm)
        {
            byte []btdm = new byte[1];
            int dmlen = 0;
            uint nres = GetDevModeBuffer(drv50, ref btdm, ref dmlen);
            if (nres == SmartComm2.SM_SUCCESS)
                Buffer2DevMode(btdm, ref dm);

            return nres;
        }

        public uint GetDriverSettings(String drv51, ref SmartComm2.SMART51_DEVMODE dm)
        {
            byte[] btdm = new byte[1];
            int dmlen = 0;
            uint nres = GetDevModeBuffer(drv51, ref btdm, ref dmlen);
            if (nres == SmartComm2.SM_SUCCESS)
                Buffer2DevMode(btdm, ref dm);

            return nres;
        }


        public uint SetDriverSettings(IntPtr hdc, String driver, SmartComm2.SMART50_DEVMODE dm)
        {
            // acquire extended devmode to byte array from driver
            byte[] btdm = null;
            int dmlen = 0;
            uint nres = GetDevModeBuffer(driver, ref btdm, ref dmlen);
            if (nres != SmartComm2.SM_SUCCESS)
                return nres;

            // apply SMART50_DEVMODE to byte array
            nres = DevMode2Buffer(dm, ref btdm);
            if (nres != SmartComm2.SM_SUCCESS)
                return nres;

            // alloc extended devmode and copy the byte array onto it
            int size = dm.devmode.dmSize + dm.devmode.dmDriverExtra;
            IntPtr extdm_ptr = Marshal.AllocHGlobal(size);
            Marshal.Copy(btdm, 0, extdm_ptr, size);

            // apply extended devmode to dc
            IntPtr result_ptr = Win32.ResetDC(hdc, extdm_ptr);
            if (result_ptr == IntPtr.Zero)
                nres = SmartComm2.SM_F_CREATEDC;

            Marshal.FreeHGlobal(extdm_ptr);

            return SmartComm2.SM_SUCCESS;
        }

        public uint SetDriverSettings(IntPtr hdc, String driver, SmartComm2.SMART51_DEVMODE dm)
        {
            // acquire extended devmode to byte array from driver
            byte[] btdm = null;
            int dmlen = 0;
            uint nres = GetDevModeBuffer(driver, ref btdm, ref dmlen);
            if (nres != SmartComm2.SM_SUCCESS)
                return nres;

            // apply SMART50_DEVMODE to byte array
            nres = DevMode2Buffer(dm, ref btdm);
            if (nres != SmartComm2.SM_SUCCESS)
                return nres;

            // alloc extended devmode and copy the byte array onto it
            int size = dm.devmode.dmSize + dm.devmode.dmDriverExtra;
            IntPtr extdm_ptr = Marshal.AllocHGlobal(size);
            Marshal.Copy(btdm, 0, extdm_ptr, size);

            // apply extended devmode to dc
            IntPtr result_ptr = Win32.ResetDC(hdc, extdm_ptr);
            if (result_ptr == IntPtr.Zero)
                nres = SmartComm2.SM_F_CREATEDC;

            Marshal.FreeHGlobal(extdm_ptr);

            return SmartComm2.SM_SUCCESS;
        }

        public uint DrawLine(byte page, byte panel, int x1, int y1, int x2, int y2, int thick, int style, int color)
        {
            Win32.RECT rc = new Win32.RECT();
            return DrawLine(page, panel, x1, y1, x2, y2, thick, style, color, ref rc);
        }

        public uint DrawLine(byte page, byte panel, int x1, int y1, int x2, int y2, int thick, int style, int color, ref Win32.RECT rc)
        {
            IntPtr rc_ptr = Marshal.AllocHGlobal(Marshal.SizeOf(rc));
            uint nres = SmartComm2.DrawLine(m_hsmart, page, panel, x1, y1, x2, y2, thick, style, color, rc_ptr);
            if (nres == SmartComm2.SM_SUCCESS)
                rc = (Win32.RECT) Marshal.PtrToStructure(rc_ptr, typeof(Win32.RECT));
            Marshal.FreeHGlobal(rc_ptr);
            return nres;
        }

        public uint DrawRect(byte page, byte panel, int x, int y, int cx, int cy, int color)
        {
            Win32.RECT rc = new Win32.RECT();
            return DrawRect(page, panel, x, y, cx, cy, color, ref rc);
        }

        public uint DrawRect(byte page, byte panel, int x, int y, int cx, int cy, int color, ref Win32.RECT rc)
        {
            IntPtr rc_ptr = Marshal.AllocHGlobal(Marshal.SizeOf(rc));
            uint nres = SmartComm2.DrawRect(m_hsmart, page, panel, x, y, cx, cy, color, rc_ptr);
            if (nres == SmartComm2.SM_SUCCESS)
                rc = (Win32.RECT) Marshal.PtrToStructure(rc_ptr, typeof(Win32.RECT));
            Marshal.FreeHGlobal(rc_ptr);
            return nres;
        }

        public uint DrawText(byte page, byte panel, int x, int y, string fontface, int fontsize, byte fontstyle, string text)
        {
            Win32.RECT rc = new Win32.RECT();
            return DrawText(page, panel, x, y, fontface, fontsize, fontstyle, text, ref rc);
        }

        public uint DrawText(byte page, byte panel, int x, int y, string fontface, int fontsize, byte fontstyle, string text, ref Win32.RECT rc)
        {
            IntPtr face_ptr = Marshal.StringToHGlobalUni(fontface);
            IntPtr text_ptr = Marshal.StringToHGlobalUni(text);
            IntPtr rc_ptr = Marshal.AllocHGlobal(Marshal.SizeOf(rc));
            uint nres = SmartComm2.DrawText(m_hsmart, page, panel, x, y, face_ptr, fontsize, fontstyle, text_ptr, rc_ptr);
            if (nres == SmartComm2.SM_SUCCESS)
                rc = (Win32.RECT) Marshal.PtrToStructure(rc_ptr, typeof(Win32.RECT));
            Marshal.FreeHGlobal(face_ptr);
            Marshal.FreeHGlobal(text_ptr);
            Marshal.FreeHGlobal(rc_ptr);
            return nres;
        }

        public uint DrawText2(byte page, byte panel, SmartComm2.DRAWTEXT2INFO dt, string text)
        {
            IntPtr dt_ptr = Marshal.AllocHGlobal(Marshal.SizeOf(dt));
            Marshal.StructureToPtr(dt, dt_ptr, false);
            IntPtr text_ptr = Marshal.StringToHGlobalUni(text);
            uint nres = SmartComm2.DrawText2(m_hsmart, page, panel, dt_ptr, text_ptr);
            Marshal.FreeHGlobal(dt_ptr);
            Marshal.FreeHGlobal(text_ptr);
            return nres;
        }

        public uint DrawImage(byte page, byte panel, int x, int y, int cx, int cy, string img)
        {
            Win32.RECT rc = new Win32.RECT();
            return DrawImage(page, panel, x, y, cx, cy, img, ref rc);
        }

        public uint DrawImage(byte page, byte panel, int x, int y, int cx, int cy, string img, ref Win32.RECT rc)
        {
            IntPtr img_ptr = Marshal.StringToHGlobalUni(img);
            IntPtr rc_ptr = Marshal.AllocHGlobal(Marshal.SizeOf(rc));
            uint nres = SmartComm2.DrawImage(m_hsmart, page, panel, x, y, cx, cy, img_ptr, rc_ptr);
            if (nres == SmartComm2.SM_SUCCESS)
                rc = (Win32.RECT) Marshal.PtrToStructure(rc_ptr, typeof(Win32.RECT));
            Marshal.FreeHGlobal(img_ptr);
            Marshal.FreeHGlobal(rc_ptr);
            return nres;
        }

        public uint DrawImage2(byte page, byte panel, int x, int y, int cx, int cy, int scale, byte align, string img)
        {
            Win32.RECT rc = new Win32.RECT();
            return DrawImage2(page, panel, x, y, cx, cy, scale, align, img, ref rc);
        }

        public uint DrawImage2(byte page, byte panel, int x, int y, int cx, int cy, int scale, byte align, string img, ref Win32.RECT rc)
        {
            IntPtr img_ptr = Marshal.StringToHGlobalUni(img);
            IntPtr rc_ptr = Marshal.AllocHGlobal(Marshal.SizeOf(rc));
            uint nres = SmartComm2.DrawImage2(m_hsmart, page, panel, x, y, cx, cy, scale, align, img_ptr, rc_ptr);
            if (nres == SmartComm2.SM_SUCCESS)
                rc = (Win32.RECT) Marshal.PtrToStructure(rc_ptr, typeof(Win32.RECT));
            Marshal.FreeHGlobal(img_ptr);
            Marshal.FreeHGlobal(rc_ptr);
            return nres;
        }


        public uint DrawBitmap(byte page, byte panel, int x, int y, int cx, int cy, IntPtr hbmp_ptr)
        {
            Win32.RECT rc = new Win32.RECT();
            return DrawBitmap(page, panel, x, y, cx, cy, hbmp_ptr, ref rc);
        }

        public uint DrawBitmap(byte page, byte panel, int x, int y, int cx, int cy, IntPtr hbmp_ptr, ref Win32.RECT rc)
        {
            IntPtr rc_ptr = Marshal.AllocHGlobal(Marshal.SizeOf(rc));
            uint nres = SmartComm2.DrawBitmap(m_hsmart, page, panel, x, y, cx, cy, SmartComm2.IMGSCALE_FITFRAME, SmartComm2.OBJ_ALIGN_HVCENTER, hbmp_ptr, rc_ptr);
            if (nres == SmartComm2.SM_SUCCESS)
                rc = (Win32.RECT) Marshal.PtrToStructure(rc_ptr, typeof(Win32.RECT));
            Marshal.FreeHGlobal(rc_ptr);
            return nres;
        }

        public uint DrawBitmap2(byte page, byte panel, int x, int y, int cx, int cy, int scale, byte align, IntPtr hbmp_ptr)
        {
            Win32.RECT rc = new Win32.RECT();
            return DrawBitmap2(page, panel, x, y, cx, cy, scale, align, hbmp_ptr, ref rc);
        }

        public uint DrawBitmap2(byte page, byte panel, int x, int y, int cx, int cy, int scale, byte align, IntPtr hbmp_ptr, ref Win32.RECT rc)
        {
            IntPtr rc_ptr = Marshal.AllocHGlobal(Marshal.SizeOf(rc));
            uint nres = SmartComm2.DrawImage2(m_hsmart, page, panel, x, y, cx, cy, scale, align, hbmp_ptr, rc_ptr);
            if (nres == SmartComm2.SM_SUCCESS)
                rc = (Win32.RECT) Marshal.PtrToStructure(rc_ptr, typeof(Win32.RECT));
            Marshal.FreeHGlobal(rc_ptr);
            return nres;
        }


        public uint DrawBarcode(byte page, byte panel, int x, int y, int cx, int cy, int color, string barcode, int size, string data, string postdata)
        {
            Win32.RECT rc = new Win32.RECT();
            return DrawBarcode(page, panel, x, y, cx, cy, color, barcode, size, data, postdata, ref rc);
        }

        public uint DrawBarcode(byte page, byte panel, int x, int y, int cx, int cy, int color, string barcode, int size, string data, string postdata, ref Win32.RECT rc)
        {
            IntPtr rc_ptr = Marshal.AllocHGlobal(Marshal.SizeOf(rc));
            IntPtr barcode_ptr = Marshal.StringToHGlobalUni(barcode);
            IntPtr data_ptr = Marshal.StringToHGlobalUni(data);
            IntPtr postdata_ptr = Marshal.StringToHGlobalUni(postdata);
            uint nres = SmartComm2.DrawBarcode(m_hsmart, page, panel, x, y, cx, cy, color, rc_ptr, barcode_ptr, size, data_ptr, postdata_ptr);
            if (nres == SmartComm2.SM_SUCCESS)
                rc = (Win32.RECT) Marshal.PtrToStructure(rc_ptr, typeof(Win32.RECT));
            Marshal.FreeHGlobal(rc_ptr);
            Marshal.FreeHGlobal(data_ptr);
            Marshal.FreeHGlobal(postdata_ptr);
            return nres;
        }


        public uint GetBarcodeTypeCount(ref int count)
        {
            IntPtr cnt_ptr = Marshal.AllocHGlobal(Marshal.SizeOf(count));
            uint nres = SmartComm2.GetBarcodeTypeCount(m_hsmart, cnt_ptr);
            if (nres == SmartComm2.SM_SUCCESS)
                count = Marshal.ReadInt32(cnt_ptr);
            Marshal.FreeHGlobal(cnt_ptr);
            return nres;
        }

        public uint GetBarcodeTypeName(int idx, ref string name)
        {
            int bufsize = 64;
            name = "";
            for (int i = 0; i < bufsize; i++)
                name += " ";
            IntPtr name_ptr = Marshal.StringToHGlobalUni(name);
            uint nres = SmartComm2.GetBarcodeTypeName(m_hsmart, idx, name_ptr, bufsize);
            if (nres == SmartComm2.SM_SUCCESS)
                name = Marshal.PtrToStringUni(name_ptr);
            Marshal.FreeHGlobal(name_ptr);
            return nres;
        }




        public uint GetPreviewBitmap(byte page, ref Bitmap bitmap)
        {
            IntPtr bmi_ptr = m_hprevbmp_ptr[page];
            Marshal.WriteInt32(bmi_ptr, 0);
            uint nres = SmartComm2.GetPreviewBitmap(m_hsmart, page, ref bmi_ptr);
            if (nres == SmartComm2.SM_SUCCESS && bmi_ptr != IntPtr.Zero)
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
            return nres;
        }


        public uint Print()
        {
            return SmartComm2.Print(m_hsmart);
        }

        public uint DCLPrint(int side)
        {
            return SmartComm2.DCLPrint(m_hsmart, side);
        }

        // - ppsurf_ptr == null  &&  plen_ptr != null  :  return required size of surface
        // - ppsurf_ptr != null  &&  plen_ptr != null  :  ppsurf_ptr will get the real surface
        public uint GetSurface(int page, ref IntPtr ppsurf_ptr, ref int surflen)
        {
            uint nres;
            IntPtr surflen_ptr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(int)));
            Marshal.WriteInt32(surflen_ptr, surflen);

            nres = SmartComm2.GetSurface2(m_hsmart, page, ref ppsurf_ptr, surflen_ptr);
            
            if (nres == SmartComm2.SM_SUCCESS)
                surflen = Marshal.ReadInt32(surflen_ptr);
            Marshal.FreeHGlobal(surflen_ptr);
            return nres;
        }

        public uint GetUnitInfo2(ref UNITINFO2 info, int dir)
        {
            SmartComm2.UNITINFOTEXT2 infoext = new SmartComm2.UNITINFOTEXT2();
            int infolen = Marshal.SizeOf(typeof(SmartComm2.UNITINFOTEXT2));

            IntPtr info_ptr = Marshal.AllocHGlobal(infolen);
            uint nres = SmartComm2.GetUnitInfo2(m_hsmart, info_ptr, dir);
            if (nres == SmartComm2.SM_SUCCESS)
            {
                SmartComm2.UNITINFOBASE2 infobase = new SmartComm2.UNITINFOBASE2();
                infolen = Marshal.SizeOf(typeof(SmartComm2.UNITINFOTEXT2));

                byte[] btinfo = new byte[infolen];
                Marshal.Copy(info_ptr, btinfo, 0, infolen);

                infoext = (SmartComm2.UNITINFOTEXT2)Marshal.PtrToStructure(info_ptr, typeof(SmartComm2.UNITINFOTEXT2));
                info.index = infoext.index;
                info.type = infoext.type;
                info.page = infoext.page;
                info.panel = infoext.panel;
                info.left = infoext.left;
                info.top = infoext.top;
                info.width = infoext.width;
                info.height = infoext.height;
                info.rotate = infoext.rotate;
                info.border = infoext.border;
                info.back = infoext.back;

                IntPtr ext_ptr = IntPtr.Zero;
                switch(info.type)
                {
                case SmartComm2.UNITTYPE_TEXT:
                    SmartComm2.UNITINFOTEXT2 extt = new SmartComm2.UNITINFOTEXT2();
                    ext_ptr = Marshal.AllocHGlobal(infolen);
                    Marshal.Copy(btinfo, 0, ext_ptr, infolen);
                    extt = (SmartComm2.UNITINFOTEXT2)Marshal.PtrToStructure(ext_ptr, typeof(SmartComm2.UNITINFOTEXT2));

                    info.uitMarginLeft = extt.leftMargin;
                    info.uitMarginTop = extt.topMargin;
                    info.uitMarginRight = extt.rightMargin;
                    info.uitMarginBottom = extt.bottomMargin;
                    info.uitAlign = extt.align;
                    info.uitFont = extt.font;
                    info.field = extt.field;
                    info.laser = extt.laser;
                    break;
                case SmartComm2.UNITTYPE_IMAGE:
                    SmartComm2.UNITINFOIMAGE2 exti = new SmartComm2.UNITINFOIMAGE2();
                    ext_ptr = Marshal.AllocHGlobal(infolen);
                    Marshal.Copy(btinfo, 0, ext_ptr, infolen);
                    exti = (SmartComm2.UNITINFOIMAGE2)Marshal.PtrToStructure(ext_ptr, typeof(SmartComm2.UNITINFOIMAGE2));

                    info.uiiOption = exti.option;
                    info.uiiWidthZoom = exti.widthZoom;
                    info.uiiHeightZoom = exti.heightZoom;
                    info.uiiContrast = exti.contrast;
                    info.uiiBright = exti.brightness;
                    info.uiiGrayscale = exti.grayscale;
                    info.uiiAlign = exti.align;
                    info.uiiOffset = exti.offset;
                    info.uiiScale = exti.scaleMethod;
                    info.uiiRound = exti.round;
                    info.uiiAutoPortrait = exti.autoportrait;
                    info.uiiAutoEffect = exti.autoeffect;
                    info.field = exti.field;
                    info.laser = exti.laser;
                    break;
                case SmartComm2.UNITTYPE_BARCODE:
                    SmartComm2.UNITINFOBAR2 extb = new SmartComm2.UNITINFOBAR2();
                    ext_ptr = Marshal.AllocHGlobal(infolen);
                    Marshal.Copy(btinfo, 0, ext_ptr, infolen);
                    extb = (SmartComm2.UNITINFOBAR2)Marshal.PtrToStructure(ext_ptr, typeof(SmartComm2.UNITINFOBAR2));

                    info.uibType = extb.bartype;
                    info.uibSize = extb.size;
                    info.uibOption = extb.option;
                    info.uibSize2D = extb.size2D;
                    info.uibColor = extb.barColor;
                    info.field = extb.field;
                    info.laser = extb.laser;
                    break;
                }
                Marshal.FreeHGlobal(ext_ptr);
            }//end-of if nres == success
            Marshal.FreeHGlobal(info_ptr);
            return nres;
        }

        public uint GetFieldLinkedUnitInfo2(string field, ref UNITINFO2 info, int dir)
        {
            IntPtr field_ptr = Marshal.StringToHGlobalUni(field);
            IntPtr info_ptr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(UNITINFO2)));
            //public static extern uint GetFieldLinkedUnitInfo2(IntPtr hsmart_ptr, IntPtr szfield_ptr, IntPtr punit_ptr, int dir);    // HSMART, WCHAR*, UNITINFO2*, int
            uint nres = SmartComm2.GetFieldLinkedUnitInfo2(m_hsmart, field_ptr, info_ptr, dir);
            if (nres == SmartComm2.SM_SUCCESS)
            {
                SmartComm2.UNITINFOTEXT2 infoext = new SmartComm2.UNITINFOTEXT2();
                int extinfolen = Marshal.SizeOf(typeof(SmartComm2.UNITINFOTEXT2));

                IntPtr extinfo_ptr = Marshal.AllocHGlobal(extinfolen);
                nres = SmartComm2.GetUnitInfo2(m_hsmart, extinfo_ptr, dir);
                if (nres == SmartComm2.SM_SUCCESS)
                {
                    SmartComm2.UNITINFOBASE2 infobase = new SmartComm2.UNITINFOBASE2();
                    extinfolen = Marshal.SizeOf(typeof(SmartComm2.UNITINFOTEXT2));

                    byte[] btinfo = new byte[extinfolen];
                    Marshal.Copy(info_ptr, btinfo, 0, extinfolen);

                    infoext = (SmartComm2.UNITINFOTEXT2)Marshal.PtrToStructure(info_ptr, typeof(SmartComm2.UNITINFOTEXT2));
                    info.index = infoext.index;
                    info.type = infoext.type;
                    info.page = infoext.page;
                    info.panel = infoext.panel;
                    info.left = infoext.left;
                    info.top = infoext.top;
                    info.width = infoext.width;
                    info.height = infoext.height;
                    info.rotate = infoext.rotate;
                    info.border = infoext.border;
                    info.back = infoext.back;

                    IntPtr ext_ptr = IntPtr.Zero;
                    switch (info.type)
                    {
                        case SmartComm2.UNITTYPE_TEXT:
                            SmartComm2.UNITINFOTEXT2 extt = new SmartComm2.UNITINFOTEXT2();
                            ext_ptr = Marshal.AllocHGlobal(extinfolen);
                            Marshal.Copy(btinfo, 0, ext_ptr, extinfolen);
                            extt = (SmartComm2.UNITINFOTEXT2)Marshal.PtrToStructure(ext_ptr, typeof(SmartComm2.UNITINFOTEXT2));

                            info.uitMarginLeft = extt.leftMargin;
                            info.uitMarginTop = extt.topMargin;
                            info.uitMarginRight = extt.rightMargin;
                            info.uitMarginBottom = extt.bottomMargin;
                            info.uitAlign = extt.align;
                            info.uitFont = extt.font;
                            info.field = extt.field;
                            info.laser = extt.laser;
                            break;
                        case SmartComm2.UNITTYPE_IMAGE:
                            SmartComm2.UNITINFOIMAGE2 exti = new SmartComm2.UNITINFOIMAGE2();
                            ext_ptr = Marshal.AllocHGlobal(extinfolen);
                            Marshal.Copy(btinfo, 0, ext_ptr, extinfolen);
                            exti = (SmartComm2.UNITINFOIMAGE2)Marshal.PtrToStructure(ext_ptr, typeof(SmartComm2.UNITINFOIMAGE2));

                            info.uiiOption = exti.option;
                            info.uiiWidthZoom = exti.widthZoom;
                            info.uiiHeightZoom = exti.heightZoom;
                            info.uiiContrast = exti.contrast;
                            info.uiiBright = exti.brightness;
                            info.uiiGrayscale = exti.grayscale;
                            info.uiiAlign = exti.align;
                            info.uiiOffset = exti.offset;
                            info.uiiScale = exti.scaleMethod;
                            info.uiiRound = exti.round;
                            info.uiiAutoPortrait = exti.autoportrait;
                            info.uiiAutoEffect = exti.autoeffect;
                            info.field = exti.field;
                            info.laser = exti.laser;
                            break;
                        case SmartComm2.UNITTYPE_BARCODE:
                            SmartComm2.UNITINFOBAR2 extb = new SmartComm2.UNITINFOBAR2();
                            ext_ptr = Marshal.AllocHGlobal(extinfolen);
                            Marshal.Copy(btinfo, 0, ext_ptr, extinfolen);
                            extb = (SmartComm2.UNITINFOBAR2)Marshal.PtrToStructure(ext_ptr, typeof(SmartComm2.UNITINFOBAR2));

                            info.uibType = extb.bartype;
                            info.uibSize = extb.size;
                            info.uibOption = extb.option;
                            info.uibSize2D = extb.size2D;
                            info.uibColor = extb.barColor;
                            info.field = extb.field;
                            info.laser = extb.laser;
                            break;
                    }
                    Marshal.FreeHGlobal(ext_ptr);
                }//end-of if nres == success
                Marshal.FreeHGlobal(extinfo_ptr);
            }
            Marshal.FreeHGlobal(field_ptr);
            Marshal.FreeHGlobal(info_ptr);
            return nres;
        }

        /*
        public static extern uint SetUnitInfo2(IntPtr hsmart_ptr, IntPtr punit_ptr);    // HSMART, UNITINFO2*
        public static extern uint GetUnitInfo2Direct(IntPtr hsmart_ptr, IntPtr punit_ptr);    // HSMART, UNITINFO2*

        public static extern uint GetObjectInfo2(int idx, ref IntPtr pinfo2_ptr, IntPtr ver_ptr);   // HSMART, DWORD, OBJ_INFO2**, int*
        public static extern uint ReleaseObjectInfo2(IntPtr pinfo2_ptr);   // HSMART, OBJ_INFO2*
        public static extern uint UpdateObject2(IntPtr pinfo2_ptr);        // HSMART, OBJ_INFO2*
        public static extern uint GetFirstObjectInfo2(byte page, byte panel, ref IntPtr pinfo2_ptr);      // HSMART, BYTE, BYTE, OBJ_INFO2**
        public static extern uint GetNextObjectInfo2(byte page, byte panel, ref IntPtr pinfo2_ptr);       // HSMART, BYTE, BYTE, OBJ_INFO2**

        */





        public uint GetICG(int page, ref int icg)
        {
            IntPtr icg_ptr = Marshal.AllocHGlobal(Marshal.SizeOf(icg));
            uint nres = SmartComm2.GetICG(m_hsmart, page, icg_ptr);
            if (nres == SmartComm2.SM_SUCCESS)
                icg = Marshal.ReadInt32(icg_ptr);//.ToInt32();
            return nres;
        }

        public uint SetICG(int page, int icg)
        {
            return SmartComm2.SetICG(m_hsmart, page, icg);
        }

        public uint IsBackEnabled(ref bool bDual)
        {
            IntPtr flag_ptr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(int)));
            uint nres = SmartComm2.IsBackEnabled(m_hsmart, flag_ptr);
            if (nres == SmartComm2.SM_SUCCESS)
            {
                int flag = Marshal.ReadInt32(flag_ptr);//.ToInt32();
                bDual = (flag != 0);
            }
            return nres;
        }

        
        
        // Contact & Contactless

        public uint RFPowerOn(int dev, ref byte[] buf)
        {
            int buflen = 64;
            IntPtr type_ptr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(int)));
            IntPtr buf_ptr = Marshal.AllocHGlobal(buflen);
            IntPtr buflen_ptr = Marshal.AllocHGlobal(Marshal.SizeOf(buflen));
            Marshal.WriteInt32(buflen_ptr, buflen);
            uint nres = SmartComm2.RFPowerOn(m_hsmart, dev, type_ptr, buflen_ptr, buf_ptr);
            if(nres==SmartComm2.SM_SUCCESS)
            {
                buflen = Marshal.ReadInt32(buflen_ptr);//.ToInt32();
                buf = new byte[buflen];
                Marshal.Copy(buf_ptr, buf, 0, buflen);
            }
            Marshal.FreeHGlobal(type_ptr);
            Marshal.FreeHGlobal(buf_ptr);
            Marshal.FreeHGlobal(buflen_ptr);
            return nres;
        }

        public uint RFPowerOff(int dev)
        {
            return SmartComm2.RFPowerOff(m_hsmart, dev);
        }

        public uint RFTransmit(int dev, byte[] apdu, ref byte[] recv)
        {
            int apdulen = apdu.Length;
            IntPtr apdu_ptr = Marshal.AllocHGlobal(apdulen);
            Marshal.Copy(apdu, 0, apdu_ptr, apdulen);
            int rcvlen = 512;
            IntPtr recv_ptr = Marshal.AllocHGlobal(rcvlen);
            IntPtr recvlen_ptr = Marshal.AllocHGlobal(Marshal.SizeOf(rcvlen));
            Marshal.WriteInt32(recvlen_ptr, rcvlen);
            uint nres = SmartComm2.RFTransmit(m_hsmart, dev, apdulen, apdu_ptr, recvlen_ptr, recv_ptr);
            if (nres == SmartComm2.SM_SUCCESS)
            {
                int recvlen = Marshal.ReadInt32(recvlen_ptr);//.ToInt32();
                recv = new byte[recvlen];
                Marshal.Copy(recv_ptr, recv, 0, recvlen);
            }
            Marshal.FreeHGlobal(apdu_ptr);
            Marshal.FreeHGlobal(recv_ptr);
            Marshal.FreeHGlobal(recvlen_ptr);
            return nres;
        }

        public uint GetRFReaderName(int dev, ref string reader)
        {
            int readerlen = 128;
            reader = "";
            for (int i = 0; i < readerlen; i++)
                reader += " ";
            IntPtr reader_ptr = Marshal.StringToHGlobalUni(reader);
            uint nres = SmartComm2.GetRFReaderName(m_hsmart, dev, reader_ptr);
            if (nres == SmartComm2.SM_SUCCESS)
                reader = Marshal.PtrToStringUni(reader_ptr);
            Marshal.FreeHGlobal(reader_ptr);
            return nres;
        }




        public uint ICPowerOn(int dev, ref byte[] buf)
        {
            int buflen = 64;
            IntPtr buf_ptr = Marshal.AllocHGlobal(buflen);
            IntPtr buflen_ptr = Marshal.AllocHGlobal(Marshal.SizeOf(buflen));
            Marshal.WriteInt32(buflen_ptr, buflen);
            uint nres = SmartComm2.ICPowerOn(m_hsmart, dev, buflen_ptr, buf_ptr);
            if (nres == SmartComm2.SM_SUCCESS)
            {
                buflen = Marshal.ReadInt32(buflen_ptr);//.ToInt32();
                buf = new byte[buflen];
                Marshal.Copy(buf_ptr, buf, 0, buflen);
            }
            Marshal.FreeHGlobal(buf_ptr);
            Marshal.FreeHGlobal(buflen_ptr);
            return nres;
        }

        public uint ICPowerOff(int dev)
        {
            return SmartComm2.ICPowerOff(m_hsmart, dev);
        }

        public uint ICTransmit(int dev, byte[] apdu, int apdulen, ref byte[] recv, ref int recvlen)
        {
            IntPtr apdu_ptr = Marshal.AllocHGlobal(apdulen);
            Marshal.Copy(apdu, 0, apdu_ptr, apdulen);
            int rcvlen = 512;
            IntPtr recv_ptr = Marshal.AllocHGlobal(rcvlen);
            IntPtr recvlen_ptr = Marshal.AllocHGlobal(Marshal.SizeOf(rcvlen));
            Marshal.WriteInt32(recvlen_ptr, rcvlen);
            uint nres = SmartComm2.ICTransmit(m_hsmart, dev, apdulen, apdu_ptr, recvlen_ptr, recv_ptr);
            if (nres == SmartComm2.SM_SUCCESS)
            {
                recvlen = Marshal.ReadInt32(recvlen_ptr);//.ToInt32();
                recv = new byte[recvlen];
                Marshal.Copy(recv_ptr, recv, 0, recvlen);
            }
            Marshal.FreeHGlobal(apdu_ptr);
            Marshal.FreeHGlobal(recv_ptr);
            Marshal.FreeHGlobal(recvlen_ptr);
            return nres;
        }

        public uint GetICReaderName(ref string reader)
        {
            SmartComm2.SMART_PRINTER_INFO info = new SmartComm2.SMART_PRINTER_INFO();
            IntPtr info_ptr = Marshal.AllocHGlobal(Marshal.SizeOf(info));
            IntPtr dev_ptr = Marshal.StringToHGlobalUni(m_strDevice);
            uint nres = SmartComm2.GetDeviceInfo2(info_ptr, dev_ptr, SmartComm2.OPENDEVICE_BYDESC);
            if (nres == SmartComm2.SM_SUCCESS)
            {
                info = (SmartComm2.SMART_PRINTER_INFO) Marshal.PtrToStructure(info_ptr, typeof(SmartComm2.SMART_PRINTER_INFO));
                reader = info.opt.ic1;
            }
            Marshal.FreeHGlobal(info_ptr);
            Marshal.FreeHGlobal(dev_ptr);
            return nres;
        }

        public uint GetSIMReaderName(ref string reader)
        {
            SmartComm2.SMART_PRINTER_INFO info = new SmartComm2.SMART_PRINTER_INFO();
            IntPtr info_ptr = Marshal.AllocHGlobal(Marshal.SizeOf(info));
            IntPtr dev_ptr = Marshal.StringToHGlobalUni(m_strDevice);
            uint nres = SmartComm2.GetDeviceInfo2(info_ptr, dev_ptr, SmartComm2.OPENDEVICE_BYDESC);
            if (nres == SmartComm2.SM_SUCCESS)
            {
                info = (SmartComm2.SMART_PRINTER_INFO) Marshal.PtrToStructure(info_ptr, typeof(SmartComm2.SMART_PRINTER_INFO));
                reader = info.opt.ic2;
            }
            Marshal.FreeHGlobal(info_ptr);
            Marshal.FreeHGlobal(dev_ptr);
            return nres;
        }



        // dll management

        public uint GetConfig(int cfg, ref int value)
        {
            IntPtr value_ptr = Marshal.AllocHGlobal(Marshal.SizeOf(value));
            uint nres = SmartComm2.GetConfig(m_hsmart, cfg, value_ptr);
            if (nres == SmartComm2.SM_SUCCESS)
                value = Marshal.ReadInt32(value_ptr);//.ToInt32();
            Marshal.FreeHGlobal(value_ptr);
            return nres;
        }

        public uint SetConfig(int cfg, int value)
        {
            return SmartComm2.SetConfig(m_hsmart, cfg, value);
        }



        // serial & kiosk

        public uint SerialCmdRecv(ref byte[] recv)
        {
            int rcvlen = 256;
            IntPtr rcvlen_ptr = Marshal.AllocHGlobal(Marshal.SizeOf(rcvlen));
            Marshal.WriteInt32(rcvlen_ptr, rcvlen);
            IntPtr rcv_ptr = Marshal.AllocHGlobal(rcvlen);
            uint nres = SmartComm2.SerialCmdRecv(m_hsmart, rcv_ptr, rcvlen_ptr);
            if (nres == SmartComm2.SM_SUCCESS)
            {
                rcvlen = Marshal.ReadInt32(rcvlen_ptr);//.ToInt32();
                recv = new byte[rcvlen];
                Marshal.Copy(rcv_ptr, recv, 0, rcvlen);
            }
            Marshal.FreeHGlobal(rcvlen_ptr);
            Marshal.FreeHGlobal(rcv_ptr);
            return nres;
        }

        public uint SerialCmdSend(byte[] send)
        {
            int sendlen = send.Length;
            IntPtr send_ptr = Marshal.AllocHGlobal(sendlen);
            Marshal.Copy(send, 0, send_ptr, sendlen);
            uint nres = SmartComm2.SerialCmdSend(m_hsmart, send_ptr, sendlen);
            Marshal.FreeHGlobal(send_ptr);
            return nres;
        }



        // kiosk

        public uint KioskCardIn(byte[] cmd, ref byte[] recv)
        {
            int cmdlen = cmd.Length;
            IntPtr cmd_ptr = Marshal.AllocHGlobal(cmdlen);
            Marshal.Copy(cmd, 0, cmd_ptr, cmdlen);
            int rcvlen = 256;
            IntPtr rcvlen_ptr = Marshal.AllocHGlobal(Marshal.SizeOf(rcvlen));
            Marshal.WriteInt32(rcvlen_ptr, rcvlen);
            IntPtr rcv_ptr = Marshal.AllocHGlobal(rcvlen);
            uint nres = SmartComm2.KioskCardIn(m_hsmart, cmd_ptr, cmdlen, rcv_ptr, rcvlen_ptr);
            if (nres == SmartComm2.SM_SUCCESS)
            {
                rcvlen = Marshal.ReadInt32(rcvlen_ptr);//.ToInt32();
                recv = new byte[rcvlen];
                Marshal.Copy(rcv_ptr, recv, 0, rcvlen);
            }
            Marshal.FreeHGlobal(cmd_ptr);
            Marshal.FreeHGlobal(rcvlen_ptr);
            Marshal.FreeHGlobal(rcv_ptr);
            return nres;
        }

        public uint KioskCardIn2(int hopper, int opt)
        {
            return SmartComm2.KioskCardIn2(m_hsmart, hopper, opt);
        }

        public uint KioskHopper(int hopper, int opt, ref int ret)
        {
            IntPtr ret_ptr = Marshal.AllocHGlobal(Marshal.SizeOf(ret));
            uint nres = SmartComm2.KioskHopper(m_hsmart, hopper, opt, ret_ptr);
            if (nres == SmartComm2.SM_SUCCESS)
                ret = Marshal.ReadInt32(ret_ptr);//.ToInt32();
            Marshal.FreeHGlobal(ret_ptr);
            return nres;
        }








        //____________________________________________________________________
        //

        private void PareparePreviewBitmapPtr()
        {
            m_hprevbmp_ptr = new IntPtr[SmartComm2.PAGE_COUNT];
            for (int page = 0; page < SmartComm2.PAGE_COUNT; page++)
                m_hprevbmp_ptr[page] = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(Int32)));
        }

        private void ReleasePreviewBitmapPtr()
        {
            if (m_hprevbmp_ptr.Length == 0)
                return;
            for (int page = 0; page < SmartComm2.PAGE_COUNT; page++)
                if(m_hprevbmp_ptr[page] != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(m_hprevbmp_ptr[page]);
                    m_hprevbmp_ptr[page] = IntPtr.Zero;
                }
        }

        //private
        public void UniStr2MultiByte(ref byte[] dst, string src)
        {
            Encoding asc = Encoding.ASCII;
            Encoding uni = Encoding.Unicode;

            byte[] unibyte = uni.GetBytes(src);
            dst = Encoding.Convert(uni, asc, unibyte);
        }





        //____________________________________________________________________
        //


        public struct PRINTERITEM
        {
            public string name;
            public string id;
            public string dev;
            public string desc;
            public int pid;
        }

        /// <summary>
        /// C# wrapper structure of SMART_PRINTER_LIST.
        /// @GetDeviceList
        /// </summary>
        public struct PRINTERLIST
        {
            public int n;
            public PRINTERITEM[] item;
        }




        public struct PRINTERINFO_USB
        {
            public string port;     // usb port
            public string link;     // symbolic link of usb port
            public bool is_bridge;  // Network module bridge
        }

        public struct PRINTERINFO_NET
        {
            public string ver;      // version of network protocol
            public string ip;       // ip address
            public int port;        // tcp port
            public bool is_ssl;     // ssl protocol
        }

        public struct PRINTERINFO_STD
        {
            public string name;     // printer name
            public string id;       // printer ID
            public string dev;      // device connection
            public int dev_type;    // 1=USB, 2=NET
            public int pid;         // USB product ID
            public PRINTERINFO_USB usb;
            public PRINTERINFO_NET net;
        }

        public struct PRINTERINFO_OPT
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
        public struct PRINTERINFO
        {
            public PRINTERINFO_STD std;
            public PRINTERINFO_OPT opt;
        }


        /// <summary>
        /// @GetSystemInfo
        /// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct SYSINFO
        {
            public string printer_ver;                  // printer f/w ver.
            public string printer_serial;               // printer serial number
            public string printer_hserial;              // thermal header serial number
            public int printer_cleaningWarning;    // cleaning-warning configuration value
            public int printer_cleaning;			// cleaning count
            public int printer_totalDensity;		// total density
            public int printer_colorDensity;		// YMC (color) density
            public int printer_blackDensity;		// K (black) density
            public int printer_overlayDensity;		// O (overlay) density
            public bool laminator_installed;        // flag for laminator installation
            public string laminator_ver;                // laminator f/w ver.
            public string laminator_serial;             // laminator serial number
        }


        /// <summary>
        /// @GetUnitInfo2, @GetFieldLinkedUnitInfo2, @SetUnitInfo2, @GetUnitInfo2Direct
        /// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct UNITINFO2
        {
            public int index;			// zero-based index from ElementList...
            public int type;			// object type (UNITTYPE_TEXT, UNITTYPE_IMAGE and UNITTYPE_BARCODE).
            public byte page;
            public byte panel;
            public int left;			// offsetLeft
            public int top;			// offsetTop
            public int width;
            public int height;
            public int rotate;
            public SmartComm2.Border border;
            public SmartComm2.BackGround back;

            // Text
            public short uitMarginLeft;
            public short uitMarginTop;
            public short uitMarginRight;
            public short uitMarginBottom;
            public byte uitAlign;
            public SmartComm2.FontInfo uitFont;

            // Image
            public int uiiOption;
            public int uiiWidthZoom;
            public int uiiHeightZoom;
            public short uiiContrast;
            public short uiiBright;
            public int uiiGrayscale;
            public short uiiAlign;
            public Win32.POINT uiiOffset;
            public byte uiiScale;
            public int uiiRound;
            public int uiiAutoPortrait;
            public int uiiAutoEffect;

            // Barcode
            public int uibType;
            public int uibSize;
            public int uibOption;
            public Win32.SIZE uibSize2D;
            public int uibColor;

            // common
            public string field;
            public SmartComm2.Laser laser;          // UNITINFO2.ver >= UNITINFO_VER_2
        }


        /// <summary>
        /// @GetObjectInfo2, @ReleaseObjectInfo2, @UpdateObject2, @GetFirstObjectInfo2, @GetNextObjectInfo2
        /// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct OBJINFO2
        {
            public int oiIndex;				// object index
            public byte oiPage;
            public byte oiPanel;
            public int oiLeft;
            public int oiTop;
            public int oiWidth;
            public int oiHeight;
            public int oiRotate;
            public int oiPrintable;			// print or not ...
            public int oiType;					// object type
            public SmartComm2.Border oiBorder;
            public SmartComm2.BackGround oiBack;
            public SmartComm2.Laser oiLaser;

            // RoundRect
            public int oirrRound;

            // Text
            public short oitLeftSpace;
            public short oitTopSpace;
            public short oitRightSpace;
            public short oitBottomSpace;
            public byte oitAlign;
            public int oitOption;
            public SmartComm2.FontInfo2 oitFont;
            public int oitField;
            public string data;

            // Image
            public int oiiWidth;
            public int oiiHeight;
            public int oiiZoomWidth;		// unit is permil. IMGSCALE_BASEAMP: use origin size of image
            public int oiiZoomHeight;		// unit is permil. IMGSCALE_BASEAMP: use origin size of image
            public short oiiContrast;		// -100 ~ . default value is 0.
            public short oiiBrightness;		// -256 ~ 255. default value is 0.
            public int oiiGrayscaled;
            public short oiiAlign;
            public Win32.POINT oiiOffset;			// offset position in left-upper corner of frame
            public byte oiiScale;			// image scaling method...
            public int oiiRound;
            public int oiiField;
            public string oiiFileName;

            // Barcode
            public string oibName;
            public int oibSize;
            public int oibOpt;
            public short oib2DOpt1;
            public short oib2DOpt2;
            public int oibColor;
            public int oibField;
            public string oibData;
            public string oibData2;         // zip-postal for "MaxiCode".

            // Line
            public int oilFlip;			// LINE_FLIP_HORZ | LINE_FLIP_VERT
        }
    }
}
