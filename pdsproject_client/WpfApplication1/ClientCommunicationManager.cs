using System;
using System.Net.Sockets;
using System.Net;

namespace ConnectionModule.CommunicationLibrary
{
    public class ClientCommunicationManager : ClientServerCommunicationManager
    {
        public Socket Connect(IPAddress ipAddress, ushort port, Socket socket)
        {            
            IPEndPoint remoteEP = new IPEndPoint(ipAddress, port);
            try
            {
                socket.Connect(remoteEP);
            }
            catch (SocketException) 
            {
                return null;
            }
            catch (ArgumentNullException) 
            {
                return null;
            }
            catch (Exception)
            {
                return null;
            }
            return socket;                      
        }        
    }
}
