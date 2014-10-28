using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunicationLibrary;

//using System.Net.Sockets;


namespace WpfApplication1
{
    class Server
    {
        private UInt16 serverID;
        private Channel channel;
        private string computerName;

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

    }
}




