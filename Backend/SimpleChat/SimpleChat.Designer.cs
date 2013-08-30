/*
Copyright (c) 2011-2013, Intel Corporation

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions are met:

    * Redistributions of source code must retain the above copyright notice,
      this list of conditions and the following disclaimer.

    * Redistributions in binary form must reproduce the above copyright notice,
      this list of conditions and the following disclaimer in the documentation
      and/or other materials provided with the distribution.

    * Neither the name of Intel Corporation nor the names of its contributors
      may be used to endorse or promote products derived from this software
      without specific prior written permission.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED.
IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT,
INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING,
BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE
OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED
OF THE POSSIBILITY OF SUCH DAMAGE.
*/

namespace SimpleChat
{
    partial class SimpleChat
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.groupBoxDiscovery = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.freqTxt = new System.Windows.Forms.Label();
            this.heartbeatFreq = new System.Windows.Forms.TextBox();
            this.pbAvatar = new System.Windows.Forms.PictureBox();
            this.lblStatus = new System.Windows.Forms.Label();
            this.lblUserName = new System.Windows.Forms.Label();
            this.lbNeighborhood = new System.Windows.Forms.ListBox();
            this.btnDiscoveryStart = new System.Windows.Forms.Button();
            this.groupBoxInvitation = new System.Windows.Forms.GroupBox();
            this.start = new System.Windows.Forms.Button();
            this.heartbeat = new System.Windows.Forms.Button();
            this.deploy = new System.Windows.Forms.Button();
            this.lblMyDevice = new System.Windows.Forms.Label();
            this.btnInviteReq = new System.Windows.Forms.Button();
            this.groupBoxCommunication = new System.Windows.Forms.GroupBox();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnSend = new System.Windows.Forms.Button();
            this.tbChatMsg = new System.Windows.Forms.TextBox();
            this.lbChatBox = new System.Windows.Forms.ListBox();
            this.groupBoxDiscovery.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbAvatar)).BeginInit();
            this.groupBoxInvitation.SuspendLayout();
            this.groupBoxCommunication.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBoxDiscovery
            // 
            this.groupBoxDiscovery.Controls.Add(this.groupBox2);
            this.groupBoxDiscovery.Controls.Add(this.lbNeighborhood);
            this.groupBoxDiscovery.Controls.Add(this.btnDiscoveryStart);
            this.groupBoxDiscovery.Location = new System.Drawing.Point(12, 12);
            this.groupBoxDiscovery.Name = "groupBoxDiscovery";
            this.groupBoxDiscovery.Size = new System.Drawing.Size(454, 230);
            this.groupBoxDiscovery.TabIndex = 0;
            this.groupBoxDiscovery.TabStop = false;
            this.groupBoxDiscovery.Text = "Discovery";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.freqTxt);
            this.groupBox2.Controls.Add(this.heartbeatFreq);
            this.groupBox2.Controls.Add(this.pbAvatar);
            this.groupBox2.Controls.Add(this.lblStatus);
            this.groupBox2.Controls.Add(this.lblUserName);
            this.groupBox2.Location = new System.Drawing.Point(246, 20);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(200, 202);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "User Info";
            // 
            // freqTxt
            // 
            this.freqTxt.AutoSize = true;
            this.freqTxt.Location = new System.Drawing.Point(13, 178);
            this.freqTxt.Name = "freqTxt";
            this.freqTxt.Size = new System.Drawing.Size(107, 13);
            this.freqTxt.TabIndex = 5;
            this.freqTxt.Text = "Heartbeat Frequency";
            // 
            // heartbeatFreq
            // 
            this.heartbeatFreq.Location = new System.Drawing.Point(126, 176);
            this.heartbeatFreq.Name = "heartbeatFreq";
            this.heartbeatFreq.Size = new System.Drawing.Size(57, 20);
            this.heartbeatFreq.TabIndex = 4;
            this.heartbeatFreq.Text = "3000";
            // 
            // pbAvatar
            // 
            this.pbAvatar.Location = new System.Drawing.Point(10, 52);
            this.pbAvatar.Name = "pbAvatar";
            this.pbAvatar.Size = new System.Drawing.Size(173, 89);
            this.pbAvatar.TabIndex = 3;
            this.pbAvatar.TabStop = false;
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStatus.Location = new System.Drawing.Point(27, 36);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(37, 13);
            this.lblStatus.TabIndex = 2;
            this.lblStatus.Text = "Status";
            // 
            // lblUserName
            // 
            this.lblUserName.AutoSize = true;
            this.lblUserName.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblUserName.Location = new System.Drawing.Point(6, 16);
            this.lblUserName.Name = "lblUserName";
            this.lblUserName.Size = new System.Drawing.Size(93, 20);
            this.lblUserName.TabIndex = 1;
            this.lblUserName.Text = "UserName";
            // 
            // lbNeighborhood
            // 
            this.lbNeighborhood.Enabled = false;
            this.lbNeighborhood.FormattingEnabled = true;
            this.lbNeighborhood.Location = new System.Drawing.Point(6, 49);
            this.lbNeighborhood.Name = "lbNeighborhood";
            this.lbNeighborhood.Size = new System.Drawing.Size(233, 173);
            this.lbNeighborhood.TabIndex = 2;
            this.lbNeighborhood.SelectedIndexChanged += new System.EventHandler(this.lbNeighborhood_SelectedIndexChanged);
            // 
            // btnDiscoveryStart
            // 
            this.btnDiscoveryStart.Location = new System.Drawing.Point(6, 20);
            this.btnDiscoveryStart.Name = "btnDiscoveryStart";
            this.btnDiscoveryStart.Size = new System.Drawing.Size(233, 23);
            this.btnDiscoveryStart.TabIndex = 0;
            this.btnDiscoveryStart.Text = "Neighborhood Start";
            this.btnDiscoveryStart.UseVisualStyleBackColor = true;
            this.btnDiscoveryStart.Click += new System.EventHandler(this.btnDiscoveryStart_Click);
            // 
            // groupBoxInvitation
            // 
            this.groupBoxInvitation.Controls.Add(this.start);
            this.groupBoxInvitation.Controls.Add(this.heartbeat);
            this.groupBoxInvitation.Controls.Add(this.deploy);
            this.groupBoxInvitation.Controls.Add(this.lblMyDevice);
            this.groupBoxInvitation.Controls.Add(this.btnInviteReq);
            this.groupBoxInvitation.Location = new System.Drawing.Point(472, 12);
            this.groupBoxInvitation.Name = "groupBoxInvitation";
            this.groupBoxInvitation.Size = new System.Drawing.Size(187, 230);
            this.groupBoxInvitation.TabIndex = 1;
            this.groupBoxInvitation.TabStop = false;
            this.groupBoxInvitation.Text = "Invitation and Connection";
            // 
            // start
            // 
            this.start.Location = new System.Drawing.Point(6, 198);
            this.start.Name = "start";
            this.start.Size = new System.Drawing.Size(175, 23);
            this.start.TabIndex = 4;
            this.start.Text = "Start";
            this.start.UseVisualStyleBackColor = true;
            this.start.Click += new System.EventHandler(this.start_Click);
            // 
            // heartbeat
            // 
            this.heartbeat.Location = new System.Drawing.Point(7, 106);
            this.heartbeat.Name = "heartbeat";
            this.heartbeat.Size = new System.Drawing.Size(174, 23);
            this.heartbeat.TabIndex = 3;
            this.heartbeat.Text = "Heartbeat";
            this.heartbeat.UseVisualStyleBackColor = true;
            this.heartbeat.Click += new System.EventHandler(this.heartbeat_Click);
            // 
            // deploy
            // 
            this.deploy.Location = new System.Drawing.Point(7, 76);
            this.deploy.Name = "deploy";
            this.deploy.Size = new System.Drawing.Size(174, 23);
            this.deploy.TabIndex = 2;
            this.deploy.Text = "Deploy";
            this.deploy.UseVisualStyleBackColor = true;
            this.deploy.Click += new System.EventHandler(this.deploy_Click);
            // 
            // lblMyDevice
            // 
            this.lblMyDevice.AutoSize = true;
            this.lblMyDevice.Location = new System.Drawing.Point(51, 157);
            this.lblMyDevice.Name = "lblMyDevice";
            this.lblMyDevice.Size = new System.Drawing.Size(74, 13);
            this.lblMyDevice.TabIndex = 1;
            this.lblMyDevice.Text = "is My Device?";
            // 
            // btnInviteReq
            // 
            this.btnInviteReq.Enabled = false;
            this.btnInviteReq.Location = new System.Drawing.Point(6, 46);
            this.btnInviteReq.Name = "btnInviteReq";
            this.btnInviteReq.Size = new System.Drawing.Size(175, 23);
            this.btnInviteReq.TabIndex = 0;
            this.btnInviteReq.Text = "Detect Functions";
            this.btnInviteReq.UseVisualStyleBackColor = true;
            this.btnInviteReq.Click += new System.EventHandler(this.btnInviteReq_Click);
            // 
            // groupBoxCommunication
            // 
            this.groupBoxCommunication.Controls.Add(this.btnClose);
            this.groupBoxCommunication.Controls.Add(this.btnSend);
            this.groupBoxCommunication.Controls.Add(this.tbChatMsg);
            this.groupBoxCommunication.Controls.Add(this.lbChatBox);
            this.groupBoxCommunication.Location = new System.Drawing.Point(16, 248);
            this.groupBoxCommunication.Name = "groupBoxCommunication";
            this.groupBoxCommunication.Size = new System.Drawing.Size(637, 186);
            this.groupBoxCommunication.TabIndex = 2;
            this.groupBoxCommunication.TabStop = false;
            this.groupBoxCommunication.Text = "Communication";
            // 
            // btnClose
            // 
            this.btnClose.Enabled = false;
            this.btnClose.Location = new System.Drawing.Point(587, 152);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(44, 23);
            this.btnClose.TabIndex = 3;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnSend
            // 
            this.btnSend.Enabled = false;
            this.btnSend.Location = new System.Drawing.Point(537, 152);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size(44, 23);
            this.btnSend.TabIndex = 2;
            this.btnSend.Text = "Send";
            this.btnSend.UseVisualStyleBackColor = true;
            this.btnSend.Click += new System.EventHandler(this.btnSend_Click);
            // 
            // tbChatMsg
            // 
            this.tbChatMsg.Enabled = false;
            this.tbChatMsg.Location = new System.Drawing.Point(13, 155);
            this.tbChatMsg.Name = "tbChatMsg";
            this.tbChatMsg.Size = new System.Drawing.Size(518, 20);
            this.tbChatMsg.TabIndex = 1;
            this.tbChatMsg.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tbChatMsg_KeyPress);
            // 
            // lbChatBox
            // 
            this.lbChatBox.Enabled = false;
            this.lbChatBox.FormattingEnabled = true;
            this.lbChatBox.HorizontalScrollbar = true;
            this.lbChatBox.Location = new System.Drawing.Point(13, 18);
            this.lbChatBox.Name = "lbChatBox";
            this.lbChatBox.Size = new System.Drawing.Size(618, 121);
            this.lbChatBox.TabIndex = 0;
            // 
            // SimpleChat
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(665, 442);
            this.Controls.Add(this.groupBoxCommunication);
            this.Controls.Add(this.groupBoxInvitation);
            this.Controls.Add(this.groupBoxDiscovery);
            this.Name = "SimpleChat";
            this.Text = "SimpleChat";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SimpleChat_FormClosing);
            this.Load += new System.EventHandler(this.SimpleChat_Load);
            this.groupBoxDiscovery.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbAvatar)).EndInit();
            this.groupBoxInvitation.ResumeLayout(false);
            this.groupBoxInvitation.PerformLayout();
            this.groupBoxCommunication.ResumeLayout(false);
            this.groupBoxCommunication.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBoxDiscovery;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.PictureBox pbAvatar;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Label lblUserName;
        private System.Windows.Forms.ListBox lbNeighborhood;
        private System.Windows.Forms.Button btnDiscoveryStart;
        private System.Windows.Forms.GroupBox groupBoxInvitation;
        private System.Windows.Forms.Button btnInviteReq;
        private System.Windows.Forms.GroupBox groupBoxCommunication;
        private System.Windows.Forms.Button btnSend;
        private System.Windows.Forms.TextBox tbChatMsg;
        private System.Windows.Forms.ListBox lbChatBox;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Label lblMyDevice;
        private System.Windows.Forms.Button deploy;
        private System.Windows.Forms.Button heartbeat;
        private System.Windows.Forms.Button start;
        private System.Windows.Forms.Label freqTxt;
        private System.Windows.Forms.TextBox heartbeatFreq;
    }
}

