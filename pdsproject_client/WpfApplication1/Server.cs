using System;

namespace ConnectionModule
{
    public class Server
    {
        private ushort serverID;
        private Channel channel;
        private string computerName;
        public bool Authenticated { get; set; }

        public Server()
        {
            serverID = 0;
        }

        public ushort ServerID
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




