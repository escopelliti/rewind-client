using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;

namespace CommunicationLibrary
{
    public class Channel
    {

        private Socket dataSocket;
        private Socket cmdSocket;
        public ushort CmdPort { get; set; }
        public ushort DataPort { get; set; }
        public System.Net.IPAddress ipAddress { get; set; }

        public Channel()
        {
            this.ipAddress = null;
        }

        public Socket GetCmdSocket()
        {
            return cmdSocket;
        }

        public void SetCmdSocket(Socket socket)
        {
            cmdSocket = socket;
        }

        public void SetDataSocket(Socket socket)
        {
            dataSocket = socket;
        }

        public Socket GetDataSocket()
        {
            return dataSocket;
        }

    }
}
