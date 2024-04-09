using SC2;
using SC70;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KisPayAgent
{
    public partial class Settings : Form
    {
        // 포토프린터 SDK
        // ISP-50
        private CSC2 m_smart;
        // ISP-70
        private CSM70 mSmart70;

        private string savedPrinterName;

        public Settings()
        {
            InitializeComponent();

            m_smart = new CSC2();
            mSmart70 = new CSM70();
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            string selectPrint = comboBoxPrinter.Text;
            Properties.Settings.Default.Printer = selectPrint;
            Properties.Settings.Default.SiteID = textBoxSiteID.Text;

            // 프린터 설정이 바뀌었을때만 처리
            if (!savedPrinterName.Equals(selectPrint))
            {
                if (selectPrint.Contains("IDP SMART-70"))
                {
                    SM7PRINTERLIST printList = new SM7PRINTERLIST();
                    mSmart70.GetDeviceList(ref printList);

                    for (int i = 0; i < printList.n; i++)
                    {
                        Console.WriteLine(printList.item[i].desc);
                    }

                    if (printList.item.Length == 1)
                    {
                        Properties.Settings.Default.SDKPrinter = printList.item[0].desc;
                    }
                }
                else if (selectPrint.Contains("IDP SMART-51"))
                {
                    CSC2.PRINTERLIST list = new CSC2.PRINTERLIST();
                    m_smart.GetDeviceList(ref list);
                    for (int i = 0; i < list.n; i++)
                    {
                        Console.WriteLine(list.item[i].desc);
                    }

                    if (list.item.Length == 1)
                    {
                        Properties.Settings.Default.SDKPrinter = list.item[0].desc;
                    }
                }
            }

            Properties.Settings.Default.CardCount = Int32.Parse(textBoxCardCount.Text);

            Properties.Settings.Default.Save();

            this.Close();
        }

        private void Settings_Load(object sender, EventArgs e)
        {
            savedPrinterName = Properties.Settings.Default.Printer;
            string version = Properties.Settings.Default.Version;
            string siteID = Properties.Settings.Default.SiteID;

            textBoxSiteID.Text = siteID;
            labelVersion.Text = version;

            String InstalledPrinters;
            for (int count = 0; count < PrinterSettings.InstalledPrinters.Count; count++)
            {
                InstalledPrinters = PrinterSettings.InstalledPrinters[count];
                comboBoxPrinter.Items.Add(InstalledPrinters);
            }

            comboBoxPrinter.Text = savedPrinterName;

            int cardCount = Properties.Settings.Default.CardCount;

            textBoxCardCount.Text = cardCount.ToString();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
