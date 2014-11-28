using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Threading;

namespace CommunicationLibrary
{
    public class ClientServerCommunicationManager
    {

        public Socket CreateSocket(ProtocolType protocolType)
        {
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, protocolType);
            socket.SendBufferSize = 64 * 1024;
            socket.ReceiveBufferSize = 64 * 1024;
            return socket;
        }

        public void Send(byte[] toSend, Socket socket)
        {
            try
            {
                socket.Send(toSend);
            }
            catch (SocketException)
            {
                return;
            }
            catch (ObjectDisposedException)
            {
                return;
            }
            catch (Exception)
            {
                return;
            }
        }

        public int Receive(byte[] bytes, Socket socket)
        {
            int bytesRead = 0;
            try
            {
                bytesRead = socket.Receive(bytes);
            }
            catch (SocketException ex)
            {
                return 0;
            }
            catch (Exception ex)
            {
                return 0;
            }
            return bytesRead;
        }

        public void Shutdown(Socket socket, SocketShutdown shutdownMode)
        {
            try
            {
                socket.Shutdown(shutdownMode);
            }
            catch (SocketException ex)
            {
                return;
            }
            catch (ObjectDisposedException ex)
            {
                return;
            }
        }

        public void Close(Socket socket)
        {
            try
            {
                socket.Close();
            }
            catch (SocketException ex)
            {
                return;
            }
        }
    }
}
