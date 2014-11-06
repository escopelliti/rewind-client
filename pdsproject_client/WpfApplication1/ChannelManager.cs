using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using WpfApplication1;
using System.Net.Sockets;


namespace CommunicationLibrary
{
    class ChannelManager
    {
        protected Server currentServer;
        protected List<Server> serverConnected;
        protected static ClientCommunicationManager ccm;


        public ChannelManager()
        {
            ccm = new ClientCommunicationManager();
            serverConnected = new List<Server>();
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
   
    }
}
