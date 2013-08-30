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
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Intel.STC.Api;
using Intel.STC.Net;
using Intel.STC.App;
using Intel.STC.Cloud;
using System.Net;
using System.Threading.Tasks;
using System.Diagnostics;
using Newtonsoft.Json.Linq;
using System.Net.Sockets;
using System.Threading;

namespace SimpleChat
{
    public partial class SimpleChat : Form
    {
        #region GlobalVars

        public static string API_KEY_ID = "DF26B96A-F1A7-4721-959C-760191C446A8";
        public static string API_KEY_SECRET = "Nq2yl0bPFxQFse6Ts+uqVQuwHWHz98IUrGkflJ3wZRQ=";
        public static string APP_TYPE_ID = "7A1B397B-B576-44C4-943F-1BEF4F490C06";
        public static string ServiceName = "SimpleChat";

        /// <summary>
        /// Discovery API interface into STC used to get a list of
        /// remote STCSessions that you can communicate with.
        /// </summary>
        STCNeighborhood Hood;

        /// <summary>
        /// Initiator API interface into the STC used to initiate the connection
        /// process.
        /// </summary>
        STCInitiator Initiator;

        /// <summary>
        /// The responder API interface into the STC used to respond to an
        /// incoming invitation request. 
        /// </summary>
        STCResponder Responder;

        /// <summary>
        /// The NetStream API interface into the STC used for communication.
        /// </summary>
        NetStream Stream;

        /// <summary>
        /// The STCSession object for remote session the application is connected to.
        /// </summary>
        STCSession RemoteSession;

        /// <summary>
        /// UserList is a local user list where the application will
        /// be responsbile for maintaining the most up to date user
        /// list
        /// </summary>
        private static List<STCSession> UserList;

        private delegate void VoidThunkDelegate();
        private bool OpenForConn = false, connected = false, discover = true;
        private String discovery = "";
        private Thread tDiscover, tHeartbeat, tDeploy;
        private Socket socket;

        #endregion

        #region SimpleChat_LifeCycle
        public SimpleChat()
        {
            InitializeComponent();
        }



        private void SimpleChat_Load(object sender, EventArgs e)
        {
            ///Initializing the user List
            UserList = new List<STCSession>();

            ///Create the STCApplicationId from the app constants
            STCApplicationId appId = new STCApplicationId(new Guid(API_KEY_ID), API_KEY_SECRET, APP_TYPE_ID);
            appId.ApiId = new Guid(API_KEY_ID);
            appId.ServiceName = "SimpleChat";

            ///Initializing the Responder object
            Responder = new STCResponder(appId, true);
            Responder.InviteReceived += new InviteReceivedHandler(SC_InviteReceived);
            Responder.CommunicationStarted += new CommunicationStartedHandler(SC_CommunicationStarted);

            Responder.Start();

            ///Initializing the Initiator object
            Initiator = new STCInitiator(appId);
            Initiator.InviteeResponded += new InviteeRespondedHandler(SC_InviteResponded);
            Initiator.CommunicationStarted += new CommunicationStartedHandler(SC_CommunicationStarted);

            btnInviteReq.Enabled = true;
        }
        #endregion

        #region STC_Discovery_API

        private void btnDiscoveryStart_Click(object sender, EventArgs e)
        {
            UserList.Clear();
            ListUpdated();

            Hood = new STCNeighborhood();
            Hood.DiscoveryEvent += new STCNeighborhoodDelegate(sdDiscoveryHandler);
            Hood.Start();
            Hood.FindUsers();
        }

        private void sdDiscoveryHandler(STCNeighborhood hood, STCSession session, STCNeighborhoodUpdateType type)
        {
            switch (type)
            {
                case STCNeighborhoodUpdateType.ARRIVAL:
                    UserList.Add(session);
                    break;
                //case STCNeighborhoodUpdateType.DEPARTURE:
                //    UserList.Remove(session);
                //    break;
                default:
                    break;
            }
            ListUpdated();
        }

        /// <summary>
        /// This method will update the List Box with the most current
        /// UserList.
        /// </summary>
        private void ListUpdated()
        {
            if (InvokeRequired)
            {
                this.BeginInvoke(new VoidThunkDelegate(delegate() { ListUpdated(); }));
                return;
            }
            lbNeighborhood.Enabled = true;
            lbNeighborhood.Items.Clear();

            UserList = UserList.Distinct().ToList();    // make sure all are distinct users

            for (int i = 0; i < UserList.Count; i++)
            {
                lbNeighborhood.Items.Add(UserList[i].Guid.ToString());
            }
        }

        private void lbNeighborhood_SelectedIndexChanged(object sender, EventArgs e)
        {
            int i = lbNeighborhood.SelectedIndex;
            if (i >= 0)
            {
                lblUserName.Text = UserList[i].User.Name;
                lblStatus.Text = UserList[i].StatusText;
                if (UserList[i].IsSelf == true)
                {
                    this.lblMyDevice.Text = "My Device";
                }
                else
                {
                    this.lblMyDevice.Text = "NOT My Device";
                }

                if (UserList[i].Avatar != null && UserList[i].Avatar.GetLength(0) != 0)
                {
                    Image avatar = Image.FromStream(new MemoryStream(UserList[i].Avatar));
                    pbAvatar.BackgroundImage = avatar;
                }
            }

            //Not working as intended.
            /*
            if (UserList[i].ContainsGadget(APP_GUID, 0) == true)
                btnInviteReq.Enabled = true;
            else
                btnInviteReq.Enabled = false;
             */
        }
        #endregion

        #region STC_Invitation_And_Connection_API_Events

        private void SC_InviteReceived(STCSession session, uint inviteHandle)
        {
            DialogResult result = MessageBox.Show("Accept invitation from " + session.User.Name + "?", "Invitation request", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
                Responder.RespondToInvite(inviteHandle, true);
            else
                Responder.RespondToInvite(inviteHandle, false);
        }


        /// <summary>
        /// Event called when remote session responds to the invite.
        /// </summary>
        /// Handle cases where the event is accepted or rejected by the Remote
        /// session. The invite can also be timed out or ignored. 
        /// <param name="session"> remote session</param>
        /// <param name="response">invitation responce by the remote session</param>
        private void SC_InviteResponded(STCSession session, InviteResponse response)
        {
        }

        /// <summary>
        /// if the invite is accepted, this event will be fired
        /// </summary>
        /// if the stream is null, the connection is dropped
        /// <param name="session">remote session</param>
        /// <param name="stream">Stream handle used for communication</param>
        private void SC_CommunicationStarted(STCSession session, NetStream stream)
        {
            if (stream == null)
            {
                stopChat();
                //MessageBox.Show("Connection Dropped");
            }
            else
            {
                this.Stream = stream;
                this.RemoteSession = session;
                startChat();
            }
        }


        private void deploy_Click(object sender, EventArgs e)
        {
            addMessage(">>Reading deployment JSON");
            System.IO.StreamReader file = new System.IO.StreamReader("json.txt");
            String json = file.ReadLine();
            file.Close();
            addMessage(">>Reading finished");

            foreach (STCSession s in UserList)
            {
                tryConnect(s);

                byte[] msgToSend = Encoding.UTF8.GetBytes("d" + json);

                addMessage(">>Writing deployment message");
                IAsyncResult res = Stream.BeginWrite(msgToSend, 0, msgToSend.Length, new AsyncCallback(writeComplete), null);
                Stream.EndWrite(res);
                addMessage(">>Deployment message written");
            }
        }
        #endregion

        #region STC_Communication_API


        private static int MAX_MSG_SIZE = 1024;
        private byte[] receiveBuffer = new byte[MAX_MSG_SIZE + 1];
        private void startChat()
        {
            if (InvokeRequired)
            {
                this.BeginInvoke(new VoidThunkDelegate(delegate() { startChat(); }));
                return;
            }

            addMessage(">>Connected to: " + this.RemoteSession.User.Name);

            lbChatBox.Enabled = tbChatMsg.Enabled = btnSend.Enabled = btnClose.Enabled = true;
            lbChatBox.TopIndex = lbChatBox.Items.Count - 1;
            lbNeighborhood.Enabled = false;

            IAsyncResult res = this.Stream.BeginRead(receiveBuffer, 0, receiveBuffer.Length, new AsyncCallback(msgReceived), null);
            if (res == null)
                stopChat();
            connected = true;
        }

        private void msgReceived(IAsyncResult result)
        {
            int bytesRead = this.Stream.EndRead(result);
            if (bytesRead <= 0)
                stopChat();
            else
            {
                String comm = Encoding.UTF8.GetString(receiveBuffer);
                if (comm.StartsWith("c"))
                {
                    comm = comm.Substring(1).Trim('\0');
                    discovery += comm + "\n";
                }
                addMessageToChatbox(receiveBuffer, false);
                startChat();
                OpenForConn = true;
            }
        }

        private void addMessageToChatbox(byte[] message, bool self)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new VoidThunkDelegate(delegate() { addMessageToChatbox(message, self); }));
                return;
            }

            String msg, comm = Encoding.UTF8.GetString(message);
            if (self)
                msg = "You: " + comm;
            else
                msg = "Remote Session: " + comm;

            lbChatBox.Items.Add(msg);
        }


        private void stopChat()
        {
            if (InvokeRequired)
            {
                this.BeginInvoke(new VoidThunkDelegate(delegate() { stopChat(); }));
                return;
            }

            // MessageBox.Show("Remote Session was closed");
            lbChatBox.Items.Add("Remote Session was disconnected.");
            lbChatBox.Enabled = tbChatMsg.Enabled = btnSend.Enabled = btnClose.Enabled = true;
            lbNeighborhood.Enabled = true;

            tHeartbeat.Abort();
            tDiscover.Abort();
            discover = true;
            if (socket.Connected)
                socket.Disconnect(true);
        }

        private void btnInviteReq_Click(object sender, EventArgs e)
        {
            tDiscover = new Thread(discoverThread);
            tDiscover.Start();
        }

        private void discoverThread()
        {
            for (int i = 0; i < UserList.Count; i++)
            {
                OpenForConn = false;
                tryConnect(UserList[i]);
                while (!OpenForConn)
                {
                    Thread.Sleep(100);
                }
            }

            byte[] buffer = new ASCIIEncoding().GetBytes(discovery);
            socket.Send(buffer);
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            byte[] msgToSend = Encoding.UTF8.GetBytes(tbChatMsg.Text);

            IAsyncResult res = this.Stream.BeginWrite(msgToSend, 0, msgToSend.Length, new AsyncCallback(writeComplete), null);

            addMessageToChatbox(msgToSend, true);
            lbChatBox.TopIndex = lbChatBox.Items.Count - 1;
            tbChatMsg.Text = "";
        }

        private void tbChatMsg_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)(Keys.Return))
            {
                btnSend_Click(this, null);
            }
        }

        private void writeComplete(IAsyncResult result)
        {
            try
            {
                this.Stream.EndWrite(result);
            }
            catch (IOException ex)
            {
                Debug.Write(ex.StackTrace);
                stopChat();
            }
        }
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Stream.Close();
        }
        #endregion

        private void heartbeat_Click(object sender, EventArgs e)
        {
            addMessage(">>Reading heartbeat JSON");
            System.IO.StreamReader file = new System.IO.StreamReader("heartbeat.txt");
            String content = file.ReadLine();
            file.Close();
            addMessage(">>Reading finished");

            JArray jsonArray = JArray.Parse(content);
            for (int i = 0; i < jsonArray.Count; i++)
            {
                String deviceId = (String)jsonArray[i]["deviceId"];

                foreach (STCSession d in UserList)
                {
                    if (d.User.Name.Equals(deviceId))
                    {
                        tryConnect(d);

                        byte[] msgToSend = Encoding.UTF8.GetBytes("u" + jsonArray[i]);

                        addMessage(">>Writing heartbeat message");
                        IAsyncResult res = Stream.BeginWrite(msgToSend, 0, msgToSend.Length, new AsyncCallback(writeComplete), null);
                        Stream.EndWrite(res);
                        addMessage(">>Heartbeat sent");
                        break;
                    }
                }
            }
        }

        private void start_Click(object sender, EventArgs e)
        {
            try
            {
                String ip = "127.0.0.1";
                int port = 9001;
                int maxBuffer = 100;

                Console.WriteLine("\nWaiting for connection..");

                IPAddress ipAddress = IPAddress.Parse(ip);
                TcpListener tcpListener = new TcpListener(ipAddress, port);
                tcpListener.Start();

                Thread pinger = new Thread(
                    () =>
                    {
                        while (discover)
                        {
                            btnDiscoveryStart_Click(null, null);
                            Thread.Sleep(5000);
                        }
                    });
                pinger.Start();

                Thread t = new Thread(
                    () =>
                    {
                        while (true)
                        {
                            addMessage("Backend Server");
                            addMessage(">>Listening on socket");
                            using (socket = tcpListener.AcceptSocket())
                            {
                                byte[] receiveBuffer = new byte[maxBuffer];
                                int usedBuffer = socket.Receive(receiveBuffer);
                                String msg = "";

                                for (int i = 0; i < usedBuffer; i++)
                                    msg += Convert.ToChar(receiveBuffer[i]);

                                addMessage(">>Received message: " + msg);

                                if (msg.Equals("discover"))
                                {
                                    addMessage("Discovery");
                                    btnInviteReq_Click(null, null);
                                    tDiscover.Join();
                                }

                                if (msg.Equals("deploy"))
                                {
                                    addMessage("Deployment");
                                    discover = false;
                                    tDeploy = new Thread(deployer);
                                    tDeploy.Start();
                                    tDeploy.Join();
                                    addMessage("Heartbeat");
                                    tHeartbeat = new Thread(heart);
                                    tHeartbeat.Start();
                                }
                            }
                        }
                    });
                t.Start();
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("Error: {0}", ex.StackTrace));
            }
        }

        private void deployer()
        {
            deploy_Click(null, null);
        }

        private void heart()
        {
            while (true)
            {
                Thread.Sleep(Convert.ToInt32(heartbeatFreq.Text));
                heartbeat_Click(null, null);
            }
        }

        private void SimpleChat_FormClosing(object sender, FormClosingEventArgs e)
        {
            Environment.Exit(0);
        }

        delegate void SetTextCallback(string text);

        private void addMessage(string text)
        {
            if (this.lbChatBox.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(addMessage);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                this.lbChatBox.Items.Add(text);
            }
        }
        private void tryConnect(STCSession session)
        {
            for (int i = 0; i < 5; i++)
            {
                Stopwatch sw = Stopwatch.StartNew();

                addMessage(">>Connecting to " + session.User.Name);
                connected = false;
                Initiator.Start(session);

                while (!connected && sw.ElapsedMilliseconds < 5000)
                {
                    Thread.Sleep(100);
                }

                if (connected)
                {
                    break;
                }

                addMessage(">>Connection to " + session.User.Name + " timed out. Retrying.");
                Initiator.Stop();
            }

            if (!connected)
            {
                addMessage(">>Connection to " + session.User.Name + " failed miserably.");
            }
        }
    }
}