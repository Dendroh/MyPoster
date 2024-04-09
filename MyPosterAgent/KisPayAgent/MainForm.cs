using KisPayAgent.Properties;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Printing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Resources;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Unity.Network;
using SC2;
using SC70;

namespace KisPayAgent
{
	public partial class MainForm : Form
	{
		private string serverDomain = "http://myposter.kr/";
		private string siteLogoGETURL = "http://myposter.kr/site/get_logo/";
		private readonly NetServer _netServer;
		private bool testMode = false;
		private string imagePath = "";
		private string logoURL = null;
		private Image logoImage = null;
		string printerName = Properties.Settings.Default.Printer;
		string printType;

		// 테스트를 위한 값
		private string testOrgAuthDate = "";
		private string testOrgAuthNo = "";

		// 포토프린터 SDK
		// ISP-50
		private CSC2 m_smart;
		// ISP-70
		private CSM70 mSmart70;

		public MainForm()
		{
			InitializeComponent();

			_netServer = new NetServer();
			_netServer.OnReceiveMessage += ReceiveMessage;

			m_smart = new CSC2();
			mSmart70 = new CSM70();

			Debug.WriteLine("MainForm");
		}

		private void MainForm_Load(object sender, EventArgs e)
		{
			StartServer();
			Debug.WriteLine("MainForm_Load");

			string siteID = Properties.Settings.Default.SiteID;

			if (printerName == string.Empty || siteID == string.Empty)
			{
				// 설정 창
				Settings settingDialog = new Settings();
				settingDialog.ShowDialog(this);
				settingDialog.Dispose();
			}

			// 서버에 등록되어있는 사이트 로고 조회
			if (siteID != string.Empty)
			{
				logoURL = GetSiteInfo(siteID);
			}

			if (logoURL != null)
			{
				logoImage = WebImageView(logoURL);
			} else
			{
				// 서버에 로고정보 없으면 기본 로고 이용
				logoImage = new Bitmap(KisPayAgent.Properties.Resources.myposterlogo);
			}

		}

		protected override CreateParams CreateParams
		{
			get
			{
				// Turn on WS_EX_TOOLWINDOW style bit
				CreateParams cp = base.CreateParams;
				cp.ExStyle |= 0x80;
				return cp;
			}
		}


		private string GetSiteInfo(string SiteId)
		{
			using (WebClient wc = new WebClient())
			{
				var json = new WebClient().DownloadString(siteLogoGETURL + SiteId);

				SiteInfo SiteInfoJSON = JsonConvert.DeserializeObject<SiteInfo>(json);

				Console.WriteLine(SiteInfoJSON.Success);
				Console.WriteLine(SiteInfoJSON.Logo);
				Console.WriteLine(SiteInfoJSON.File_Path);
				Console.WriteLine(SiteInfoJSON.Error);

				if (SiteInfoJSON.Success && SiteInfoJSON.Logo != null)
				{
					if (SiteInfoJSON.Logo.Length > 0)
					{
						return serverDomain + SiteInfoJSON.File_Path + SiteInfoJSON.Logo;
					}
				}
			}
			return null;
		}

		private void StartServer()
		{
			Thread tcpServerRunThread = new Thread(new ThreadStart(TcpServerRun));
			tcpServerRunThread.IsBackground = true;
			tcpServerRunThread.Start();
		}

		private void TcpServerRun()
		{
			bool startServer = _netServer.Start();
		}

		private void updateUI()
		{
			Action del = delegate ()
			{
				//MessageBox.Show("연결성공");
			};
			Invoke(del);
		}

		private void ReceiveMessage(string message)
		{
			testMode = false;

			if (message != null)
			{
				if (message.Length > 0)
				{
					textBoxLog.AppendText(message + "\r\n");
					AgentSendData agentSendDataJSON = JsonConvert.DeserializeObject<AgentSendData>(message);
					printType = agentSendDataJSON.PrintType;

					if (agentSendDataJSON.Command.Equals("payment"))
					{
						PayProcess(agentSendDataJSON.Amount);
					} else if (agentSendDataJSON.Command.Equals("payment_cancel"))
					{
						PayCancelProcess(agentSendDataJSON.Amount, agentSendDataJSON.PaymentDate, agentSendDataJSON.PaymentAuthNum);
					} else if (agentSendDataJSON.Command.Equals("print"))
					{
						short PrintCount = Convert.ToInt16(agentSendDataJSON.PrintCount);
						if (agentSendDataJSON.PrintType.Equals("card"))
						{
							if (printerName.Contains("IDP SMART-51"))
								PosterImagePrint(agentSendDataJSON.FilePath, PrintCount, "IDP SMART-51 Card Printer");
							else if (printerName.Contains("IDP SMART-70"))
								PosterImagePrint(agentSendDataJSON.FilePath, PrintCount, "IDP SMART-70 Card Printer");
							else
								PosterImagePrint(agentSendDataJSON.FilePath, PrintCount);
						} else if (agentSendDataJSON.PrintType.Equals("film"))
						{
							PosterImagePrint(agentSendDataJSON.FilePath, PrintCount, "DP-DS620");
						}
					} else if (agentSendDataJSON.Command.Equals("print_status"))
					{
						short PrintCount = Convert.ToInt16(agentSendDataJSON.PrintCount);

						if (printerName.Contains("IDP SMART-51"))
							GetStatusIDPSmart51Print(PrintCount);
						else if (printerName.Contains("IDP SMART-70"))
							GetStatusIDPSmart70Print(PrintCount);
						else
						{
							// Unity 결과 전송
							PrintStatusResponse response = new PrintStatusResponse();
							response.Result = 0;

							// JSON 변환
							var serialised = JsonConvert.SerializeObject(response);
							Debug.WriteLine(serialised.ToString());

							// 테스트 결제 모드가 아니면 결과 전송
							if (!testMode)
							{
								_netServer.SendMessage(serialised.ToString());
							}
						}
					} else
					{

					}
				}
			}
			Debug.WriteLine(message);
		}

		private void AgentDataSetting(string tranAmt, string orgDate, string orgAuthNo)
		{

			if (tranAmt.Length > 0)
			{
				// 결제 금액 : txtTranAmt
				axKisPosAgent1.inTranAmt = tranAmt;

				// 부가세
				// 부가세 계산 : 총결제금액 / 11 소수점 반올림
				float tranAmtInt = float.Parse(tranAmt, CultureInfo.InvariantCulture.NumberFormat);
				float vat = tranAmtInt / 11;
				int vatAmt = (int)Math.Round(vat);
				axKisPosAgent1.inVatAmt = vatAmt.ToString();

				// 봉사료
				axKisPosAgent1.inSvcAmt = "0";
				// 할부개월 - 일시불 : 00
				axKisPosAgent1.inInstallment = "00";

				// 승인 취소 일때 해당값 셋팅
				if (orgDate.Length > 0 && orgAuthNo.Length > 0)
				{
					axKisPosAgent1.inOrgAuthDate = orgDate;
					axKisPosAgent1.inOrgAuthNo = orgAuthNo;
				}
			}

		}

		private short AgentApproval(String agentIP, short agentPort, String tranCode)
		{
			axKisPosAgent1.inAgentIP = agentIP;
			axKisPosAgent1.inAgentPort = agentPort;

			axKisPosAgent1.inTranCode = tranCode;

			return axKisPosAgent1.KIS_Approval();
		}

		private short AgentApprovalEvent(String agentIP, short agentPort, String tranCode)
		{
			axKisPosAgent1.inAgentIP = agentIP;
			axKisPosAgent1.inAgentPort = agentPort;

			axKisPosAgent1.inTranCode = tranCode;

			return axKisPosAgent1.KIS_Approval_Event();
		}


		private void TestPayButton_Click(object sender, EventArgs e)
		{
			testMode = true;
			PayProcess("4000");
		}

		private void PayProcess(string price)
		{
			String tranCode = "";

			axKisPosAgent1.Init();

			// 결제
			tranCode = "D1";

			AgentDataSetting(price, "", "");

			int chk;

			chk = AgentApproval("127.0.0.1", 1515, tranCode);

			AgentResponse(0);
		}

		private void PayCancelProcess(string price, string orgDate, string orgAuthNo)
		{
			String tranCode = "";

			axKisPosAgent1.Init();

			// 결제 취소
			tranCode = "D2";

			AgentDataSetting(price, orgDate, orgAuthNo);

			int chk;

			chk = AgentApproval("127.0.0.1", 1515, tranCode);

			AgentResponse(0);
		}

		private void AgentResponse(int mode)
		{
			bool paymentStatus = false;
			string orgAuthDate = "";
			string orgAuthNo = "";
			string outTranAmt = "";

			String tranCode = axKisPosAgent1.inTranCode;
			int outRtn = axKisPosAgent1.outRtn;

			Debug.WriteLine("리턴코드 : [" + outRtn + "]");

			if (outRtn == 0)
			{
				String resCode = axKisPosAgent1.outReplyCode;

				Debug.WriteLine("응답코드 : [" + resCode + "]");
				Debug.WriteLine("메세지 : [" + axKisPosAgent1.outReplyMsg1 + "]");
				Debug.WriteLine("메세지 : [" + axKisPosAgent1.outReplyMsg2 + "]");

				textBoxLog.AppendText("응답코드 : [" + resCode + "]" + "\r\n");
				textBoxLog.AppendText("메세지 : [" + axKisPosAgent1.outReplyMsg1 + "]" + "\r\n");
				textBoxLog.AppendText("메세지: [" + axKisPosAgent1.outReplyMsg2 + "]" + "\r\n");

				//정상거래응답
				if (resCode.Equals("0000") && tranCode.Equals("ST") == false && tranCode.Equals("RI") == false)
				{
					Debug.WriteLine("단말기번호 : [" + axKisPosAgent1.outCatId + "]");
					Debug.WriteLine("WCC : [" + axKisPosAgent1.outWCC + "]");
					Debug.WriteLine("카드빈 : [" + axKisPosAgent1.outCardNo + "]");
					Debug.WriteLine("할부개월 : [" + axKisPosAgent1.outInstallment + "]");
					Debug.WriteLine("거래금액 : [" + axKisPosAgent1.outTranAmt + "]");
					Debug.WriteLine("부가세액 : [" + axKisPosAgent1.outVatAmt + "]");
					Debug.WriteLine("봉사료 : [" + axKisPosAgent1.outSvcAmt + "]");
					Debug.WriteLine("잔액 : [" + axKisPosAgent1.outJanAmt + "]");
					Debug.WriteLine("승인번호 : [" + axKisPosAgent1.outAuthNo + "]");
					Debug.WriteLine("거래일자 : [" + axKisPosAgent1.outReplyDate + "]");
					Debug.WriteLine("매입사코드 : [" + axKisPosAgent1.outAccepterCode + "]");
					Debug.WriteLine("매입사명 : [" + axKisPosAgent1.outAccepterName + "]");
					Debug.WriteLine("발급사코드 : [" + axKisPosAgent1.outIssuerCode + "]");
					Debug.WriteLine("발급사명 : [" + axKisPosAgent1.outIssuerName + "]");
					Debug.WriteLine("거래일련번호 : [" + axKisPosAgent1.outTranNo + "]");
					Debug.WriteLine("가맹점번호 : [" + axKisPosAgent1.outMerchantRegNo + "]");

					// 거래금액
					outTranAmt = axKisPosAgent1.outTranAmt;

					if (axKisPosAgent1.outReplyDate != "" && axKisPosAgent1.outAuthNo != "")
					{
						// 승인날짜
						orgAuthDate = axKisPosAgent1.outReplyDate;
						// 승인번호
						orgAuthNo = axKisPosAgent1.outAuthNo;
					}


					paymentStatus = true;

				}
				/* 
				 * 응답코드 : 77FB
				 * 리더기 부팅 후 상호인증 및 무결성점검 미진행시 발생
				 * 상호인증 및 무결성점검 실행 후 재결제.
				*/
				else if (resCode.Equals("77FB"))
				{
					paymentStatus = false;
					tranCode = "RI";
					if (mode == 0)
					{
						AgentApproval("127.0.0.1", 1515, tranCode);
						Debug.WriteLine("응답코드 : [" + axKisPosAgent1.outReplyCode + "]");
						Debug.WriteLine("메세지 : [" + axKisPosAgent1.outReplyMsg1 + "]");
						Debug.WriteLine("메세지 : [" + axKisPosAgent1.outReplyMsg2 + "]");
					} else
					{
						AgentApprovalEvent("127.0.0.1", 1515, tranCode);
						Debug.WriteLine("===================================================");
						return;
					}
				} else
				{
					paymentStatus = false;
					Debug.WriteLine("===================================================");
					//MessageBox.Show(axKisPosAgent1.outReplyMsg1);
				}


			}

			// 취소시 원승인날짜 포맷이 YYMMDD라서 앞에 2자리 잘라냄
			string tempOrgAuthDate = "";
			if (orgAuthDate.Length == 8)
			{
				tempOrgAuthDate = orgAuthDate.Trim().Substring(2);
			}

			// Unity 결과 전송
			PaymentResponse response = new PaymentResponse();
			response.Result = paymentStatus.ToString();
			response.Price = outTranAmt.Trim();
			response.ApprovalDate = tempOrgAuthDate;
			response.ApprovalNum = orgAuthNo.Trim();

			// 테스트를 위한 승인 데이터 저장
			testOrgAuthDate = tempOrgAuthDate;
			testOrgAuthNo = orgAuthNo.Trim();

			// JSON 변환
			var serialised = JsonConvert.SerializeObject(response);
			Debug.WriteLine(serialised.ToString());

			// 테스트 결제 모드가 아니면 결과 전송
			if (!testMode)
			{
				_netServer.SendMessage(serialised.ToString());

			} else
			{
				//MessageBox.Show(serialised.ToString());
			}

			textBoxLog.AppendText(serialised.ToString() + "\r\n");

			Debug.WriteLine("===================================================");
			axKisPosAgent1.Init();
		}

		private void PayTestMenuItem_Click(object sender, EventArgs e)
		{
			testMode = true;
			PayProcess("1000");
		}

		private void ExitMenuItem_Click(object sender, EventArgs e)
		{
			Application.Exit();
		}

		private void buttonPrint_Click(object sender, EventArgs e)
		{
			PosterImagePrint(Application.StartupPath + @"\\1.jpg", 1);
		}

		void PosterImagePrint(string path, short count)
		{
			int cardCount = Properties.Settings.Default.CardCount;

			imagePath = path;

			if (count <= 0)
				count = 1;

			// Properties에 카드 수량 변경
			cardCount -= count;

			// 예외 사항 체크
			if (cardCount < 0)
				cardCount = 0;

			Properties.Settings.Default.CardCount = cardCount;
			Properties.Settings.Default.Save();


			PrintDialog printDialog = new PrintDialog();

			PrintDocument printDocument = new PrintDocument();

			printDialog.Document = printDocument; //add the document to the dialog box...        
			printDocument.DefaultPageSettings.Landscape = false;
			printDocument.OriginAtMargins = false;

			Margins margins = new Margins(0, 0, 0, 0);
			printDocument.PrinterSettings.DefaultPageSettings.Margins = margins;
			printDocument.PrinterSettings.Copies = count;

			printDocument.PrintPage += new System.Drawing.Printing.PrintPageEventHandler(imageToPrint_PrintPage);

			//printDocument.PrinterSettings.PrinterName = "DP-DS820";
			//printDocument.PrinterSettings.PrinterName = "HP LaserJet CM1415fnw";
			//printDocument.PrinterSettings.PrinterName = "IDP SMART-70 Card Printer";

			printDocument.PrinterSettings.PrinterName = printerName;
			printDocument.Print();
		}

		void PosterImagePrint(string path, short count, string printName)
		{
			int cardCount = Properties.Settings.Default.CardCount;

			imagePath = path;

			if (count <= 0)
				count = 1;

			// Properties에 카드 수량 변경
			cardCount -= count;

			// 예외 사항 체크
			if (cardCount < 0)
				cardCount = 0;

			Properties.Settings.Default.CardCount = cardCount;
			Properties.Settings.Default.Save();


			PrintDialog printDialog = new PrintDialog();

			PrintDocument printDocument = new PrintDocument();

			printDialog.Document = printDocument; //add the document to the dialog box...        
			printDocument.DefaultPageSettings.Landscape = false;
			printDocument.OriginAtMargins = false;

			Margins margins = new Margins(0, 0, 0, 0);
			printDocument.PrinterSettings.DefaultPageSettings.Margins = margins;
			printDocument.PrinterSettings.Copies = count;

			printDocument.PrintPage += new System.Drawing.Printing.PrintPageEventHandler(imageToPrint_PrintPage) ;

			//printDocument.PrinterSettings.PrinterName = "DP-DS820";
			//printDocument.PrinterSettings.PrinterName = "HP LaserJet CM1415fnw";
			//printDocument.PrinterSettings.PrinterName = "IDP SMART-70 Card Printer";

			printDocument.PrinterSettings.PrinterName = printName;
			printDocument.Print();
		}

		Bitmap WebImageView(string URL)
		{
			try
			{
				WebClient Downloader = new WebClient();
				Stream ImageStream = Downloader.OpenRead(URL);
				Bitmap DownloadImage = Bitmap.FromStream(ImageStream) as Bitmap;
				return DownloadImage;
			} catch (Exception)
			{
				return null;
			}

		}


		void imageToPrint_PrintPage(object sender, PrintPageEventArgs e)
		{
			Image img;

			if (printType.Equals("film"))
			{
				// Poster Image Print
				using (var bmpTemp = new Bitmap(imagePath))
				{
					img = new Bitmap(bmpTemp);

					e.Graphics.DrawImage(img, 20, 0, 375, 500);
				}

				if (logoImage != null)
				{
					Image imgLogo = logoImage;

					e.Graphics.DrawImage(imgLogo, 20, 530, 175, 65);

					e.Graphics.DrawImage(imgLogo, 222, 530, 175, 65);
				}
			} else
			{
				// Poster Image Print
				using (var bmpTemp = new Bitmap(imagePath))
				{
					img = new Bitmap(bmpTemp);

					e.Graphics.DrawImage(img, 2, 0, 210, 272);
				}

				if (logoImage != null)
				{
					Image imgLogo = logoImage;

					e.Graphics.DrawImage(imgLogo, 12, 272, 189, 70);
				}
			}
		}

		private void toolStripMenuItemSetting_Click(object sender, EventArgs e)
		{
			// 설정 창
			Settings settingDialog = new Settings();
			settingDialog.ShowDialog(this);
			settingDialog.Dispose();
		}

		private void buttonTestPaySend_Click(object sender, EventArgs e)
		{
			// Unity 결과 전송
			PaymentResponse response = new PaymentResponse();
			response.Result = "True";
			response.Price = "1000";
			response.ApprovalDate = "190619";
			response.ApprovalNum = "37142761";

			// JSON 변환
			var serialised = JsonConvert.SerializeObject(response);
			Debug.WriteLine(serialised.ToString());

			_netServer.SendMessage(serialised.ToString());
		}

		private void GetStatusIDPSmart51Print(int count)
		{
			int printStatus = 0;
			uint nRes;
			ulong statusInfo = 0;
			int ribbonRemainCount = 0;

			string printerSDKName = Properties.Settings.Default.SDKPrinter;
			int cardCount = Properties.Settings.Default.CardCount;

			// IDP SMART-51이 포함되어있으면 처리
			if (printerName.Contains("IDP SMART-51") && printerSDKName != null && printerSDKName.Length > 0)
			{
				textBoxLog.AppendText(printerSDKName + "\r\n");

				nRes = m_smart.OpenDevice(printerSDKName);
				if (nRes == SmartComm2.SM_SUCCESS)
				{
					m_smart.GetStatus(ref statusInfo);
					m_smart.GetRibbonRemain(ref ribbonRemainCount);

					// 프린터 상태 체크(Status 값이 0이 아닌경우 전부 에러 발생(카드 없음 등등)
					if (statusInfo == 0)
					{
						printStatus = 1;

						// 프린터 상태는 정상이어도 리본남아있는 양이 출력하려는 갯수보다 적으면 false
						Console.WriteLine("ribbonRemainCount : " + ribbonRemainCount);
						textBoxLog.AppendText("ribbonRemainCount: " + ribbonRemainCount + "\r\n");
						if (ribbonRemainCount < count)
							printStatus = 0;

						// 남아있는 카드 갯수 체크
						if (cardCount >= count)
							printStatus = 1;
						else
							printStatus = 0;
					} else if (statusInfo == 0x200000 || statusInfo == 0x400200 || statusInfo == 0x400201 || statusInfo == 0x400202 || statusInfo == 0x400208 ||
						  statusInfo == 0x400210 || statusInfo == 0x400240 || statusInfo == 0x400241 || statusInfo == 0x400242 || statusInfo == 0x400250 ||
						  statusInfo == 0x400280 || statusInfo == 0x440200 || statusInfo == 0x440208 || statusInfo == 0x440210 || statusInfo == 0x440240 ||
						  statusInfo == 0x440241 || statusInfo == 0x480202 || statusInfo == 0x480242)

					{
						// 프린트 진행중
						printStatus = 2;
					} else
					{
						printStatus = 0;
					}


					Console.WriteLine("statusInfo: " + statusInfo.ToString("X"));
					textBoxLog.AppendText("statusInfo: " + statusInfo.ToString("X") + "\r\n");
				} else
				{
					printStatus = 0;
				}

				// close device...
				m_smart.CloseDevice();

			}

			textBoxLog.AppendText("Result: " + printStatus + "\r\n");

			// Unity 결과 전송
			PrintStatusResponse response = new PrintStatusResponse();
			response.Result = printStatus;

			// JSON 변환
			var serialised = JsonConvert.SerializeObject(response);
			Debug.WriteLine(serialised.ToString());

			// 테스트 결제 모드가 아니면 결과 전송
			if (!testMode)
			{
				_netServer.SendMessage(serialised.ToString());
				textBoxLog.AppendText(serialised.ToString() + "\r\n");
			} else
			{
				//MessageBox.Show(serialised.ToString());
			}
		}

		private void GetStatusIDPSmart70Print(int count)
		{
			int printStatus = 0;
			uint nRes;
			int ribbonRemainCount70 = 0;
			uint printReady = 0;
			byte mStatus = 0x00;

			string printerSDKName = Properties.Settings.Default.SDKPrinter;
			int cardCount = Properties.Settings.Default.CardCount;

			// IDP SMART-51이 포함되어있으면 처리
			if (printerName.Contains("IDP SMART-70") && printerSDKName.Length > 0)
			{
				// open device...
				nRes = mSmart70.OpenDevice(printerSDKName);

				if (nRes == SmartComm70.SM_SUCCESS)
				{
					// 0x40 - Main Printer
					// 0x10 - Input Hopper
					mSmart70.IsModuleReady(0x40, ref printReady);
					mSmart70.GetState(0x10, ref mStatus);
					mSmart70.GetRibbonRemain(0x40, ref ribbonRemainCount70);

					// 프린터 상태 체크(Status 값이 0이 아닌경우 전부 에러 발생(카드 없음 등등)
					textBoxLog.AppendText("printReady : " + printReady + ", mStatus:" + mStatus + "\r\n");
					Console.WriteLine("printReady : " + printReady + ", mStatus:" + mStatus + "\r\n");
					if (printReady == 0 && mStatus == 0x04)
					{
						printStatus = 1;

						// 프린터 상태는 정상이어도 리본남아있는 양이 출력하려는 갯수보다 적으면 false
						Console.WriteLine("ribbonRemainCount : " + ribbonRemainCount70);
						textBoxLog.AppendText("ribbonRemainCount : " + ribbonRemainCount70 + "\r\n");

						// 리본 인쇄 개수 체크
						if (ribbonRemainCount70 < count)
							printStatus = 0;

						// 남아있는 카드 갯수 체크
						if (cardCount >= count)
							printStatus = 1;
						else
							printStatus = 0;
					} else if ((printReady == 2147484505 && mStatus == 0x04) || (printReady == 2147484505 && mStatus == 0x06))
					{
						// 프린트 중일때 값 설정
						printStatus = 2;
					} else
						printStatus = 0;

				} else
				{
					printStatus = 0;
				}

				// close device...
				mSmart70.CloseDevice();

			}

			// Unity 결과 전송
			PrintStatusResponse response = new PrintStatusResponse();
			response.Result = printStatus;


			// JSON 변환
			var serialised = JsonConvert.SerializeObject(response);
			Debug.WriteLine(serialised.ToString());

			// 테스트 결제 모드가 아니면 결과 전송
			if (!testMode)
			{
				_netServer.SendMessage(serialised.ToString());
				textBoxLog.AppendText(serialised.ToString() + "\r\n");
			} else
			{
				//MessageBox.Show(serialised.ToString());
			}
		}

		private void TestPrintStatusButton_Click(object sender, EventArgs e)
		{
			testMode = true;

			if (printerName.Contains("IDP SMART-51"))
				GetStatusIDPSmart51Print(2);
			else if (printerName.Contains("IDP SMART-70"))
				GetStatusIDPSmart70Print(2);
		}

		private void TestPayCancelButton_Click(object sender, EventArgs e)
		{
			testMode = true;

			if (testOrgAuthDate.Length > 0 && testOrgAuthNo.Length > 0)
				PayCancelProcess("1000", testOrgAuthDate, testOrgAuthNo);
		}
	}
}
