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

namespace SimpleDiscovery
{
    partial class SimpleDiscovery
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
            this.gbDiscovery = new System.Windows.Forms.GroupBox();
            this.gbUserInfo = new System.Windows.Forms.GroupBox();
            this.pbAvatar = new System.Windows.Forms.PictureBox();
            this.lblStatus = new System.Windows.Forms.Label();
            this.lblUserName = new System.Windows.Forms.Label();
            this.lbNeighborhood = new System.Windows.Forms.ListBox();
            this.btnNeighborhoodStart = new System.Windows.Forms.Button();
            this.lblMyDevice = new System.Windows.Forms.Label();
            this.gbDiscovery.SuspendLayout();
            this.gbUserInfo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbAvatar)).BeginInit();
            this.SuspendLayout();
            // 
            // gbDiscovery
            // 
            this.gbDiscovery.Controls.Add(this.gbUserInfo);
            this.gbDiscovery.Controls.Add(this.lbNeighborhood);
            this.gbDiscovery.Controls.Add(this.btnNeighborhoodStart);
            this.gbDiscovery.Location = new System.Drawing.Point(12, 13);
            this.gbDiscovery.Name = "gbDiscovery";
            this.gbDiscovery.Size = new System.Drawing.Size(444, 232);
            this.gbDiscovery.TabIndex = 0;
            this.gbDiscovery.TabStop = false;
            this.gbDiscovery.Text = "Discovery APIs";
            // 
            // gbUserInfo
            // 
            this.gbUserInfo.Controls.Add(this.lblMyDevice);
            this.gbUserInfo.Controls.Add(this.pbAvatar);
            this.gbUserInfo.Controls.Add(this.lblStatus);
            this.gbUserInfo.Controls.Add(this.lblUserName);
            this.gbUserInfo.Location = new System.Drawing.Point(260, 19);
            this.gbUserInfo.Name = "gbUserInfo";
            this.gbUserInfo.Size = new System.Drawing.Size(168, 207);
            this.gbUserInfo.TabIndex = 2;
            this.gbUserInfo.TabStop = false;
            this.gbUserInfo.Text = "User Info";
            // 
            // pbAvatar
            // 
            this.pbAvatar.Location = new System.Drawing.Point(23, 52);
            this.pbAvatar.Name = "pbAvatar";
            this.pbAvatar.Size = new System.Drawing.Size(123, 121);
            this.pbAvatar.TabIndex = 2;
            this.pbAvatar.TabStop = false;
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStatus.Location = new System.Drawing.Point(35, 36);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(37, 13);
            this.lblStatus.TabIndex = 1;
            this.lblStatus.Text = "Status";
            // 
            // lblUserName
            // 
            this.lblUserName.AutoSize = true;
            this.lblUserName.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblUserName.Location = new System.Drawing.Point(19, 16);
            this.lblUserName.Name = "lblUserName";
            this.lblUserName.Size = new System.Drawing.Size(93, 20);
            this.lblUserName.TabIndex = 0;
            this.lblUserName.Text = "UserName";
            // 
            // lbNeighborhood
            // 
            this.lbNeighborhood.FormattingEnabled = true;
            this.lbNeighborhood.Location = new System.Drawing.Point(6, 54);
            this.lbNeighborhood.Name = "lbNeighborhood";
            this.lbNeighborhood.Size = new System.Drawing.Size(233, 173);
            this.lbNeighborhood.TabIndex = 1;
            this.lbNeighborhood.SelectedIndexChanged += new System.EventHandler(this.lbNeighborhood_SelectedIndexChanged);
            // 
            // btnNeighborhoodStart
            // 
            this.btnNeighborhoodStart.Location = new System.Drawing.Point(6, 25);
            this.btnNeighborhoodStart.Name = "btnNeighborhoodStart";
            this.btnNeighborhoodStart.Size = new System.Drawing.Size(233, 23);
            this.btnNeighborhoodStart.TabIndex = 0;
            this.btnNeighborhoodStart.Text = "Neighborhood Start";
            this.btnNeighborhoodStart.UseVisualStyleBackColor = true;
            this.btnNeighborhoodStart.Click += new System.EventHandler(this.btnNeighborhoodStart_Click);
            // 
            // lblMyDevice
            // 
            this.lblMyDevice.AutoSize = true;
            this.lblMyDevice.Location = new System.Drawing.Point(37, 176);
            this.lblMyDevice.Name = "lblMyDevice";
            this.lblMyDevice.Size = new System.Drawing.Size(75, 13);
            this.lblMyDevice.TabIndex = 3;
            this.lblMyDevice.Text = "Is My Device?";
            // 
            // SimpleDiscovery
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(463, 257);
            this.Controls.Add(this.gbDiscovery);
            this.Name = "SimpleDiscovery";
            this.Text = "SimpleDiscovery";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.gbDiscovery.ResumeLayout(false);
            this.gbUserInfo.ResumeLayout(false);
            this.gbUserInfo.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbAvatar)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gbDiscovery;
        private System.Windows.Forms.Button btnNeighborhoodStart;
        private System.Windows.Forms.ListBox lbNeighborhood;
        private System.Windows.Forms.GroupBox gbUserInfo;
        private System.Windows.Forms.PictureBox pbAvatar;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Label lblUserName;
        private System.Windows.Forms.Label lblMyDevice;

    }
}

