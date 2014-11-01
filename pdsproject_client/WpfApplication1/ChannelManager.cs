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
        private Server currentServer;
        private Dictionary<UInt16,Server> serverDictionary;
        private UInt16 lastServerId;
        private static ClientCommunicationManager ccm;
        
        public ChannelManager() 
        {
            lastServerId = 0;            
            InitilizeDictionary();
            ccm = new ClientCommunicationManager();
        }

        public void AssignChannel(Server s)
        {
            Socket dataSocket = ccm.CreateSocket(ProtocolType.Tcp);
            dataSocket = ccm.Connect(s.ComputerName, 12001, dataSocket);
            Socket cmdSocket = ccm.CreateSocket(ProtocolType.Tcp);
            cmdSocket = ccm.Connect(s.ComputerName, 12000, cmdSocket);
            Channel ch = new Channel();
            ch.SetCmdSocket(cmdSocket);
            ch.SetDataSocket(dataSocket);
            s.SetChannel(ch);
        }

        private void InitilizeDictionary()
        {
            serverDictionary = new Dictionary<ushort, Server>();
        }

        public void sendInputToSever(NativeInput.INPUT inputToSend)
        {
            string json = JsonConvert.SerializeObject(inputToSend);            
            byte[] toSend = Encoding.Unicode.GetBytes(json);
            ccm.Send(toSend, currentServer.GetChannel().GetDataSocket());
            ccm.Receive(new byte[5], currentServer.GetChannel().GetDataSocket());
        }
        
        public Server getCurrentServer()
        {
            return currentServer;
        }

        public void setCurrentServer(Server cs) 
        {
            currentServer = cs;
        }

        public int addServer(Server s) 
        {
            lastServerId++;
            if (s.GetChannel() == null) {
                AssignChannel(s);
            }
            serverDictionary.Add(lastServerId,s);
            return lastServerId;
        }

        public void deleteServer(Server s)
        {
            serverDictionary.Remove(s.ServerID);
        }

        public void switchServer(int n)
        {

        }

        internal void switchServer(System.Windows.Input.Key key)
        {

        }
    }
}
