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
        private List<Server> serverConnected;
        private ClientCommunicationManager ccm;
        public byte[] CurrentToken { get; set; }
        private TokenGenerator tokenGen;

        public ChannelManager()
        {            
            ccm = new ClientCommunicationManager();
            serverConnected = new List<Server>();
            tokenGen = new TokenGenerator();
        }

        public long GetClipboardDimension()
        {
            byte[] dimension = new byte[5];
            ccm.Receive(dimension, currentServer.GetChannel().GetDataSocket());
            return Convert.ToInt64(dimension);
        }

        public void SendBytes(byte[] toSend)
        {
            ccm.Send(toSend, currentServer.GetChannel().GetCmdSocket());
        }

        public void ReceiveAck()
        {
            ccm.Receive(new byte[4], currentServer.GetChannel().GetCmdSocket());
        }

        public void sendInputToSever(NativeInput.INPUT inputToSend)
        {
            string json = JsonConvert.SerializeObject(inputToSend);
            byte[] toSend = Encoding.Unicode.GetBytes(json);
            ccm.Send(toSend, currentServer.GetChannel().GetDataSocket());
            ccm.Receive(new byte[5], currentServer.GetChannel().GetDataSocket());
        }
        
        public void AssignChannel(Server s)
        {
            Socket dataSocket = ccm.CreateSocket(ProtocolType.Tcp);
            dataSocket = ccm.Connect(s.ComputerName, s.DataPort, dataSocket);
            Socket cmdSocket = ccm.CreateSocket(ProtocolType.Tcp);
            cmdSocket = ccm.Connect(s.ComputerName, s.CmdPort, cmdSocket);
            Channel ch = new Channel();
            ch.SetCmdSocket(cmdSocket);
            ch.SetDataSocket(dataSocket);
            s.SetChannel(ch);
        }
 
        public Server getCurrentServer()
        {
            return currentServer;
        }

        public void setCurrentServer(Server cs) 
        {
            currentServer = cs;
        }

        public ushort FindFreeIdServer()
        {
            ushort id = 0;
            bool found = false;
            
            while (!found)
            {
                if (serverConnected.Exists(x => x.ServerID == id))
                    id++;
                else
                    found = true;
            }
            return id;
        }

        public void addServer(Server s) 
        {
            s.ServerID = FindFreeIdServer(); 
            if (s.GetChannel() == null) {
                AssignChannel(s);
            }
            serverConnected.Add(s);
        }

        public void deleteServer(Server s)
        {
            serverConnected.Remove(s);
        }

        public List<string> GetComputerNames()
        {
            List<string> computerNames = new List<string>();
            foreach (Server s in serverConnected)
            {
                if (s != currentServer)
                {
                    computerNames.Add(s.ComputerName);
                }
            }
                return computerNames;
        }

        public byte[] ReceiveData()
        {
            byte[] buffer = new byte[1024];
            int bytesRead = ccm.Receive(buffer, currentServer.GetChannel().GetCmdSocket());
            return bytesRead > 0 ? buffer : null;
        }

        public void EndConnectionToCurrentServer()
        {
            SendRequest(Protocol.ProtocolUtils.SET_RESET_FOCUS, Protocol.ProtocolUtils.FOCUS_OFF);
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

        public void StartNewConnection(int id)
        {
            Server newServer = this.serverConnected.Find(server => server.ServerID == id);
            currentServer = newServer;
            SendRequest(Protocol.ProtocolUtils.SET_RESET_FOCUS, Protocol.ProtocolUtils.FOCUS_ON);            
        }
    }
}
