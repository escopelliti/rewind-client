using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunicationLibrary;

//using System.Net.Sockets;


namespace WpfApplication1
{
    public class Server
    {
        private ushort serverID;
        private Channel channel;
        private string computerName;
        private ushort dataPort;
        private ushort cmdPort;

        public Server()
        {
            serverID = 0;
        }

        public UInt16 ServerID
        {
            get { return serverID; }
            set { serverID = value; }
        }

        public string ComputerName
        {
            get { return computerName; }
            set { computerName = value; }
        }

        public Channel GetChannel()
        {
            return channel;
        }

        public void SetChannel(Channel ch)
        {
            channel = ch;
        }

        public UInt16 DataPort
        {
            get { return dataPort; }
            set { dataPort = value; }
        }

        public UInt16 CmdPort
        {
            get { return cmdPort; }
            set { cmdPort = value; }
        }
    }
}




