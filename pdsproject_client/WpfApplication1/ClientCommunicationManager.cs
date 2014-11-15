using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.IO;

namespace CommunicationLibrary
{
    public class ClientCommunicationManager : ClientServerCommunicationManager
    {
        public Socket Connect(IPAddress ipAddress, ushort port, Socket socket)
        {
            IPEndPoint remoteEP = new IPEndPoint(ipAddress, port);
            socket.Connect(remoteEP);
            return socket;
        }        
    }
}
