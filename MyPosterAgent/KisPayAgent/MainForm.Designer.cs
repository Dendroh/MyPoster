namespace KisPayAgent
{
    partial class MainForm
    {
        /// <summary>
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 디자이너에서 생성한 코드

        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
			this.TestPayButton = new System.Windows.Forms.Button();
			this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
			this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.결제테스트ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItemSetting = new System.Windows.Forms.ToolStripMenuItem();
			this.종료ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.buttonPrint = new System.Windows.Forms.Button();
			this.buttonTestPaySend = new System.Windows.Forms.Button();
			this.TestPrintStatusButton = new System.Windows.Forms.Button();
			this.TestPayCancelButton = new System.Windows.Forms.Button();
			this.textBoxLog = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.contextMenuStrip1.SuspendLayout();
			this.SuspendLayout();
			// 
			// TestPayButton
			// 
			this.TestPayButton.Location = new System.Drawing.Point(14, 70);
			this.TestPayButton.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.TestPayButton.Name = "TestPayButton";
			this.TestPayButton.Size = new System.Drawing.Size(125, 38);
			this.TestPayButton.TabIndex = 2;
			this.TestPayButton.Text = "테스트 결제";
			this.TestPayButton.UseVisualStyleBackColor = true;
			this.TestPayButton.Click += new System.EventHandler(this.TestPayButton_Click);
			// 
			// notifyIcon1
			// 
			this.notifyIcon1.ContextMenuStrip = this.contextMenuStrip1;
			this.notifyIcon1.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon1.Icon")));
			this.notifyIcon1.Text = "KISConnectAgent";
			this.notifyIcon1.Visible = true;
			// 
			// contextMenuStrip1
			// 
			this.contextMenuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
			this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.결제테스트ToolStripMenuItem,
            this.toolStripMenuItemSetting,
            this.종료ToolStripMenuItem});
			this.contextMenuStrip1.Name = "contextMenuStrip1";
			this.contextMenuStrip1.Size = new System.Drawing.Size(159, 76);
			// 
			// 결제테스트ToolStripMenuItem
			// 
			this.결제테스트ToolStripMenuItem.Name = "결제테스트ToolStripMenuItem";
			this.결제테스트ToolStripMenuItem.Size = new System.Drawing.Size(158, 24);
			this.결제테스트ToolStripMenuItem.Text = "결제 테스트";
			this.결제테스트ToolStripMenuItem.Click += new System.EventHandler(this.PayTestMenuItem_Click);
			// 
			// toolStripMenuItemSetting
			// 
			this.toolStripMenuItemSetting.Name = "toolStripMenuItemSetting";
			this.toolStripMenuItemSetting.Size = new System.Drawing.Size(158, 24);
			this.toolStripMenuItemSetting.Text = "설정";
			this.toolStripMenuItemSetting.Click += new System.EventHandler(this.toolStripMenuItemSetting_Click);
			// 
			// 종료ToolStripMenuItem
			// 
			this.종료ToolStripMenuItem.Name = "종료ToolStripMenuItem";
			this.종료ToolStripMenuItem.Size = new System.Drawing.Size(158, 24);
			this.종료ToolStripMenuItem.Text = "종료";
			this.종료ToolStripMenuItem.Click += new System.EventHandler(this.ExitMenuItem_Click);
			// 
			// buttonPrint
			// 
			this.buttonPrint.Location = new System.Drawing.Point(283, 70);
			this.buttonPrint.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.buttonPrint.Name = "buttonPrint";
			this.buttonPrint.Size = new System.Drawing.Size(99, 38);
			this.buttonPrint.TabIndex = 3;
			this.buttonPrint.Text = "테스트프린터";
			this.buttonPrint.UseVisualStyleBackColor = true;
			this.buttonPrint.Click += new System.EventHandler(this.buttonPrint_Click);
			// 
			// buttonTestPaySend
			// 
			this.buttonTestPaySend.Location = new System.Drawing.Point(145, 70);
			this.buttonTestPaySend.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.buttonTestPaySend.Name = "buttonTestPaySend";
			this.buttonTestPaySend.Size = new System.Drawing.Size(131, 38);
			this.buttonTestPaySend.TabIndex = 4;
			this.buttonTestPaySend.Text = "테스트 결제 전송";
			this.buttonTestPaySend.UseVisualStyleBackColor = true;
			this.buttonTestPaySend.Click += new System.EventHandler(this.buttonTestPaySend_Click);
			// 
			// TestPrintStatusButton
			// 
			this.TestPrintStatusButton.Location = new System.Drawing.Point(390, 70);
			this.TestPrintStatusButton.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.TestPrintStatusButton.Name = "TestPrintStatusButton";
			this.TestPrintStatusButton.Size = new System.Drawing.Size(125, 38);
			this.TestPrintStatusButton.TabIndex = 5;
			this.TestPrintStatusButton.Text = "프린터상태조회";
			this.TestPrintStatusButton.UseVisualStyleBackColor = true;
			this.TestPrintStatusButton.Click += new System.EventHandler(this.TestPrintStatusButton_Click);
			// 
			// TestPayCancelButton
			// 
			this.TestPayCancelButton.Location = new System.Drawing.Point(14, 115);
			this.TestPayCancelButton.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.TestPayCancelButton.Name = "TestPayCancelButton";
			this.TestPayCancelButton.Size = new System.Drawing.Size(125, 38);
			this.TestPayCancelButton.TabIndex = 6;
			this.TestPayCancelButton.Text = "테스트 결제 취소";
			this.TestPayCancelButton.UseVisualStyleBackColor = true;
			this.TestPayCancelButton.Click += new System.EventHandler(this.TestPayCancelButton_Click);
			// 
			// textBoxLog
			// 
			this.textBoxLog.BackColor = System.Drawing.SystemColors.Window;
			this.textBoxLog.Location = new System.Drawing.Point(14, 230);
			this.textBoxLog.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.textBoxLog.Multiline = true;
			this.textBoxLog.Name = "textBoxLog";
			this.textBoxLog.ReadOnly = true;
			this.textBoxLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.textBoxLog.Size = new System.Drawing.Size(627, 193);
			this.textBoxLog.TabIndex = 7;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Font = new System.Drawing.Font("굴림", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
			this.label1.Location = new System.Drawing.Point(15, 16);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(209, 24);
			this.label1.TabIndex = 8;
			this.label1.Text = "마이포스터 Agent";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Font = new System.Drawing.Font("굴림", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
			this.label2.Location = new System.Drawing.Point(17, 191);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(44, 20);
			this.label2.TabIndex = 9;
			this.label2.Text = "Log";
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(649, 430);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.textBoxLog);
			this.Controls.Add(this.TestPayCancelButton);
			this.Controls.Add(this.TestPrintStatusButton);
			this.Controls.Add(this.buttonTestPaySend);
			this.Controls.Add(this.buttonPrint);
			this.Controls.Add(this.TestPayButton);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.Name = "MainForm";
			this.ShowInTaskbar = false;
			this.Text = "MainForm";
			this.WindowState = System.Windows.Forms.FormWindowState.Minimized;
			this.Load += new System.EventHandler(this.MainForm_Load);
			this.contextMenuStrip1.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private AxKisPosAgentLib.AxKisPosAgent axKisPosAgent1;
        private System.Windows.Forms.Button TestPayButton;
        private System.Windows.Forms.NotifyIcon notifyIcon1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem 결제테스트ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 종료ToolStripMenuItem;
        private System.Windows.Forms.Button buttonPrint;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemSetting;
        private System.Windows.Forms.Button buttonTestPaySend;
        private System.Windows.Forms.Button TestPrintStatusButton;
        private System.Windows.Forms.Button TestPayCancelButton;
        private System.Windows.Forms.TextBox textBoxLog;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
    }
}

