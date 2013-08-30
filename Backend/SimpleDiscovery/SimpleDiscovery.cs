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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Intel.STC.Api;
using System.Diagnostics;
using System.IO;


namespace SimpleDiscovery
{

    /// <summary>
    /// This is a simple discover application
    /// </summary>
    public partial class SimpleDiscovery : Form
    {
        STCNeighborhood Hood;
        List<STCSession> UserList;

        public SimpleDiscovery()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Debug.Write("Welcome to Simple Discovery\n");
            UserList = new List<STCSession>();  
        }

        private void btnNeighborhoodStart_Click(object sender, EventArgs e)
        {
            UserList.Clear();
            Hood = new STCNeighborhood();
            Hood.DiscoveryEvent += new STCNeighborhoodDelegate(sdDiscoveryHandler);

            Hood.Start();
            Hood.FindUsers();
        }

        private void sdDiscoveryHandler(STCNeighborhood hood, STCSession session, STCNeighborhoodUpdateType type)
        {
            Debug.Write(session.Guid.ToString());
            switch (type)
            {
                case STCNeighborhoodUpdateType.ARRIVAL:
                    Debug.Write(":Session Arrived\n");
                    UserList.Add(session);
                    break;
                //case STCNeighborhoodUpdateType.DEPARTURE:
                //  Debug.Write(":Session Departed\n");
                //    UserList.Remove(session);
                //    break;
                case STCNeighborhoodUpdateType.UPDATE:
                    Debug.Write(":Session Update\n");
                    ///probably not the best way to do this.
                    //UserList.Remove(session);
                    //UserList.Add(session);
                    break;
                default:
                    Debug.Write("should not happen");
                    break;
            }
            ListUpdated();
        }

        private delegate void VoidThunkDelegate();
        private void ListUpdated()
        {
            if (InvokeRequired)
            {
                this.BeginInvoke(new VoidThunkDelegate(delegate() { ListUpdated(); }));
                return;
            }
            lbNeighborhood.Items.Clear();

            for (int i = 0; i < UserList.Count; i++)
            {
                lbNeighborhood.Items.Add(UserList[i].Guid.ToString());
            }    
        }

        private void lbNeighborhood_SelectedIndexChanged(object sender, EventArgs e)
        {
            Debug.Write(lbNeighborhood.SelectedIndex);
            int i = lbNeighborhood.SelectedIndex;
            lblUserName.Text    = UserList[i].User.Name;
            lblStatus.Text      = UserList[i].StatusText;

            if (UserList[i].IsSelf == true)
            {
                this.lblMyDevice.Text = "My Device";
            }
            else
            {
                this.lblMyDevice.Text = "NOT My Device";
            }

            if (UserList[i].Avatar != null)
            {
                Image avatar = Image.FromStream(new MemoryStream(UserList[i].Avatar));
                pbAvatar.BackgroundImage = avatar;
            }    
        }
    }
}
