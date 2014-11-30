using System;
using System.Net.Sockets;

namespace ConnectionModule
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
