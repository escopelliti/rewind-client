using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using WpfApplication1;
using System.Net.Sockets;
using Protocol;

namespace CommunicationLibrary
{
    public class ChannelManager
    {
        private Server currentServer;
        public List<Server> ConnectedServer { get; set; }
        private ClientCommunicationManager ccm;
        public byte[] CurrentToken { get; set; }
        private TokenGenerator tokenGen;

        public ChannelManager()
        {            
            ccm = new ClientCommunicationManager();
            ConnectedServer = new List<Server>();
            tokenGen = new TokenGenerator();
        }

        public void OnLostComputerConnection(object sender, object param)
        {
            Server server = (Server)param;
            server = this.ConnectedServer.Find(x => x.ComputerName == server.ComputerName);
            if (server != null)
            {
                this.ConnectedServer.Remove(server);
                this.ccm.Shutdown(server.GetChannel().GetDataSocket(), SocketShutdown.Both);
                this.ccm.Shutdown(server.GetChannel().GetCmdSocket(), SocketShutdown.Both);
                this.ccm.Close(server.GetChannel().GetDataSocket());
                this.ccm.Close(server.GetChannel().GetCmdSocket());
            }
        }

        public long GetClipboardDimension()
        {
            byte[] data = new byte[16];
            int bytesRead = ccm.Receive(data, currentServer.GetChannel().GetCmdSocket());
            byte[] dimension = new byte[bytesRead];
            System.Buffer.BlockCopy(data, 0, dimension, 0, bytesRead);
            return BitConverter.ToInt64(dimension, 0);
        }

        public void SendBytes(byte[] jsonToSend)
        {
            byte[] toSend = new byte[jsonToSend.Length + TokenGenerator.TOKEN_DIM];            
            System.Buffer.BlockCopy(CurrentToken, 0, toSend, 0, TokenGenerator.TOKEN_DIM);
            System.Buffer.BlockCopy(jsonToSend, 0, toSend, TokenGenerator.TOKEN_DIM, jsonToSend.Length);
            ccm.Send(toSend, currentServer.GetChannel().GetCmdSocket());
        }

        public void ReceiveAck()
        {
            ccm.Receive(new byte[16], currentServer.GetChannel().GetCmdSocket());
        }

        public void SendInputToSever(NativeInput.INPUT inputToSend)
        {
            string json = JsonConvert.SerializeObject(inputToSend);
            byte[] toSend = Encoding.Unicode.GetBytes(json);
            ccm.Send(toSend, currentServer.GetChannel().GetDataSocket());
            ccm.Receive(new byte[5], currentServer.GetChannel().GetDataSocket());
        }

        public void AssignChannel(Server s)
        {
            Socket dataSocket = ccm.CreateSocket(ProtocolType.Tcp);
            dataSocket = ccm.Connect(s.ComputerName, s.GetChannel().DataPort, dataSocket);
            Socket cmdSocket = ccm.CreateSocket(ProtocolType.Tcp);
            cmdSocket = ccm.Connect(s.ComputerName, s.GetChannel().CmdPort, cmdSocket);            
            s.GetChannel().SetCmdSocket(cmdSocket);
            s.GetChannel().SetDataSocket(dataSocket);            
        }
 
        public Server GetCurrentServer()
        {
            return currentServer;
        }

        public void SetCurrentServer(Server cs) 
        {
            currentServer = cs;
        }

        public ushort FindFreeIdServer()
        {
            ushort id = 0;
            bool found = false;
            
            while (!found)
            {
                if (ConnectedServer.Exists(x => x.ServerID == id))
                    id++;
                else
                    found = true;
            }
            return id;
        }

        public void AddServer(Server s) 
        {
            s.ServerID = FindFreeIdServer(); 
            if (s.GetChannel() == null) {
                AssignChannel(s);
            }
            ConnectedServer.Add(s);
        }

        public void DeleteServer(Server s)
        {
            ConnectedServer.Remove(s);
        }

        public List<ComputerItem> GetComputerItemList()
        {
            List<ComputerItem> connectedComputers = new List<ComputerItem>();
            ushort idItem = 0;
            foreach (Server s in ConnectedServer)
            {
                if (s != currentServer)
                {
                    connectedComputers.Add(new ComputerItem()
                    { Name = s.ComputerName, ComputerStateImage = @"resources/images/connComputer.png", ComputerNum = idItem, ComputerID = s.ServerID });
                    idItem++;
                }
            }
            return connectedComputers;
        }

        public byte[] ReceiveData()
        {
            byte[] buffer = new byte[64 * 1024];
            int bytesRead = ccm.Receive(buffer, currentServer.GetChannel().GetCmdSocket());
            byte[] data = new byte[bytesRead];
            System.Buffer.BlockCopy(buffer, 0, data, 0, bytesRead);
            buffer = null;
            return bytesRead > 0 ? data : null;
        }

        public void EndConnectionToCurrentServer()
        {
            ReceiveAck();
            SendRequest(Protocol.ProtocolUtils.SET_RESET_FOCUS, Protocol.ProtocolUtils.FOCUS_OFF);
            ReceiveAck();
            currentServer = null;
        }



        public void SendRequest(string requestType, Object content)
        {
            StandardRequest stdReq = new StandardRequest();
            stdReq.type = requestType;
            stdReq.content = content;
            string jsonTosend = JsonConvert.SerializeObject(stdReq);
            byte[] requestToSend = Encoding.Unicode.GetBytes(jsonTosend);
            byte[] toSend = new byte[requestToSend.Length + TokenGenerator.TOKEN_DIM];
            CurrentToken = tokenGen.GetNewToken();
            System.Buffer.BlockCopy(CurrentToken, 0, toSend, 0, TokenGenerator.TOKEN_DIM);
            System.Buffer.BlockCopy(requestToSend, 0, toSend, TokenGenerator.TOKEN_DIM, requestToSend.Length);
            ccm.Send(toSend, currentServer.GetChannel().GetCmdSocket());
        }

        public void AssignNewToken() {
            CurrentToken = tokenGen.GetNewToken();
        }

        public void StartNewConnection(int id)
        {
            Server newServer = this.ConnectedServer.Find(server => server.ServerID == id);
            currentServer = newServer;
            SendRequest(Protocol.ProtocolUtils.SET_RESET_FOCUS, Protocol.ProtocolUtils.FOCUS_ON);
            ReceiveAck();
        }

        public void ResetTokenGen()
        {
            this.tokenGen.Reset();
        }
    }
}
