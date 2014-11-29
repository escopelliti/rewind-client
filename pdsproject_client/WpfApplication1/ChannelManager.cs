using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using WpfApplication1;
using System.Net.Sockets;
using Protocol;
using System.Collections.ObjectModel;
using System.Windows;
using System.Threading;
using System.Net;

namespace CommunicationLibrary
{
    public class ChannelManager
    {
        private Server currentServer;
        public List<Server> ConnectedServer { get; set; }
        public ClientCommunicationManager ccm { get; set; }
        public byte[] CurrentToken { get; set; }
        private TokenGenerator tokenGen;

        public ChannelManager()
        {            
            ccm = new ClientCommunicationManager();
            ConnectedServer = new List<Server>();
            tokenGen = new TokenGenerator();
            Thread ServerSocket = new Thread(() => OpenServerSocket());
            ServerSocket.Start();

        }

        private void OpenServerSocket()
        {
            ServerCommunicationManager scm = new ServerCommunicationManager();
            Socket serverSocket = scm.CreateSocket(ProtocolType.Tcp);
            serverSocket = scm.Listen(Dns.GetHostName(), 40000, serverSocket);
            if (serverSocket == null)
            {
                //Non siamo riusciti ad aprire una serversocket sul client forse sarebbe bene riavviare l'applicazione;
            }
            else
            {
                while (true)
                {
                    Socket clientSocket = scm.Accept(serverSocket);
                    Thread checkThread = new Thread(() => IsDstReacheable(clientSocket));
                    checkThread.Start();
                }
            }
        }

        private void IsDstReacheable(Socket clientSocket)
        {
            byte[] byteRead = new byte[1];
            int timeToWait = 10;
            clientSocket.ReceiveTimeout = timeToWait*10000;
            bool channelIsOpened = true;

            while (channelIsOpened)
            {
                try
                {
                    int byteReadNum = ccm.Receive(byteRead, clientSocket);
                    if (byteReadNum > 0)
                    {
                        timeToWait = Convert.ToInt32(byteRead);
                        clientSocket.ReceiveTimeout = timeToWait;
                        clientSocket.Send(new byte[1]);
                    }

                    else
                    {
                        CloseChannel();
                        ccm.Shutdown(clientSocket, SocketShutdown.Both);
                        ccm.Close(clientSocket);
                        channelIsOpened = false;
                    }
                }
                catch (Exception)
                {
                    //destination unreacheable
                    CloseChannel();
                    ccm.Shutdown(clientSocket, SocketShutdown.Both);
                    ccm.Close(clientSocket);
                    channelIsOpened = false;
                }

            }
        }

        private void CloseChannel()
        {
            ccm.Shutdown(currentServer.GetChannel().GetCmdSocket(), SocketShutdown.Both);
            Socket dataSocket = currentServer.GetChannel().GetDataSocket();
            if (dataSocket != null)
            {
                ccm.Shutdown(dataSocket, SocketShutdown.Both);
            }

        }
        public void OnLostComputerConnection(object sender, object param)
        {
            Server server = (Server)param;
            server = this.ConnectedServer.Find(x => x.ComputerName == server.ComputerName);
            if (server != null)
            {
                this.ConnectedServer.Remove(server);
                if (server.GetChannel().GetDataSocket() != null)
                {
                    this.ccm.Shutdown(server.GetChannel().GetDataSocket(), SocketShutdown.Both);
                    this.ccm.Close(server.GetChannel().GetDataSocket());
                }
                if (server.GetChannel().GetCmdSocket() != null)
                {
                    this.ccm.Shutdown(server.GetChannel().GetCmdSocket(), SocketShutdown.Both);
                    this.ccm.Close(server.GetChannel().GetCmdSocket());
                }
            }
        }

        public long GetClipboardDimension()
        {
            byte[] data = new byte[16];
            int bytesRead = ccm.Receive(data, currentServer.GetChannel().GetCmdSocket());
            byte[] dimension = new byte[bytesRead];
            System.Buffer.BlockCopy(data, 0, dimension, 0, bytesRead);
            return BitConverter.ToInt64(dimension, 0);
        }

        public void SendBytes(byte[] jsonToSend)
        {
            byte[] toSend = new byte[jsonToSend.Length + TokenGenerator.TOKEN_DIM];            
            System.Buffer.BlockCopy(CurrentToken, 0, toSend, 0, TokenGenerator.TOKEN_DIM);
            System.Buffer.BlockCopy(jsonToSend, 0, toSend, TokenGenerator.TOKEN_DIM, jsonToSend.Length);
            ccm.Send(toSend, currentServer.GetChannel().GetCmdSocket());
        }

        public void ReceiveAck()
        {
            ccm.Receive(new byte[16], currentServer.GetChannel().GetCmdSocket());
        }

        public void SendInputToSever(NativeInput.INPUT inputToSend)
        {
            string json = JsonConvert.SerializeObject(inputToSend);
            byte[] toSend = Encoding.Unicode.GetBytes(json);
            ccm.Send(toSend, currentServer.GetChannel().GetDataSocket());
            if (ccm.Receive(new byte[5], currentServer.GetChannel().GetDataSocket()) <= 0)
            {
                DeleteServer(currentServer, SocketShutdown.Both);
                currentServer = null;
                foreach (Window win in System.Windows.Application.Current.Windows)
                {
                    if (win is FullScreenRemoteServerControl)
                    {
                        ((FullScreenRemoteServerControl)win).Close();
                    }
                }
                InterceptEvents.StopCapture();
            }
        }

        public void AssignCmdChannel(Server s)
        {
            int retry = 3;
            Socket cmdSocket = ccm.CreateSocket(ProtocolType.Tcp);
            while (retry > 0)
            {
                cmdSocket = ccm.Connect(s.GetChannel().ipAddress, s.GetChannel().CmdPort, cmdSocket);
                if (cmdSocket == null)
                {
                    retry--;
                }
                else
                {
                    s.GetChannel().SetCmdSocket(cmdSocket);
                    break;
                }
            }

            if (cmdSocket == null) 
            {
                throw new NullReferenceException();
            }
        }

        public void AssignDataChannel(Server s)
        {
            s = ConnectedServer.Find(x => x.ServerID == s.ServerID);
            if (s != null)
            {
                Socket dataSocket = ccm.CreateSocket(ProtocolType.Tcp);
                dataSocket = ccm.Connect(s.GetChannel().ipAddress, s.GetChannel().DataPort, dataSocket);
                s.GetChannel().SetDataSocket(dataSocket);
            }            
        }
 
        public Server GetCurrentServer()
        {
            return currentServer;
        }

        public void SetCurrentServer(Server cs) 
        {
            currentServer = cs;
        }

        public ushort FindFreeIdServer()
        {
            ushort id = 0;
            bool found = false;
            
            while (!found)
            {
                if (ConnectedServer.Exists(x => x.ServerID == id))
                    id++;
                else
                    found = true;
            }
            return id;
        }

        public void AddServer(Server s) 
        {
            s.ServerID = FindFreeIdServer();
            s.Authenticated = false;
            if (this.ConnectedServer.Find(x => x.ComputerName == s.ComputerName) != null)
            {
                return;
            }
            if (s.GetChannel() == null) {
                try
                {
                    AssignCmdChannel(s);
                }
                catch (Exception) 
                {
                    System.Windows.MessageBox.Show("Il computer sembra non rispondere!", "Ops...", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }
            ConnectedServer.Add(s);
        }

        public void DeleteServer(Server s, SocketShutdown mode)
        {
            if (ConnectedServer.Remove(s))
            {
                Socket cmdSocket = s.GetChannel().GetCmdSocket();
                Socket dataSocket = s.GetChannel().GetDataSocket();
                if(cmdSocket != null) 
                {
                    ccm.Shutdown(cmdSocket, mode);
                    ccm.Close(cmdSocket);
                    cmdSocket = null;
                    s.GetChannel().SetCmdSocket(cmdSocket);
                }
                if (dataSocket != null)
                {
                    ccm.Shutdown(dataSocket, mode);
                    ccm.Close(dataSocket);
                    dataSocket = null;
                    s.GetChannel().SetDataSocket(dataSocket);
                }
            }
        }

        public ObservableCollection<ComputerItem> GetComputerItemList()
        {
            ObservableCollection<ComputerItem> connectedComputers = new ObservableCollection<ComputerItem>();
            ushort idItem = 0;
            foreach (Server s in ConnectedServer)
            {
                if (s != currentServer)
                {
                    connectedComputers.Add(new ComputerItem()
                    { Name = s.ComputerName, ComputerStateImage = @"resources/images/connComputer.png", ComputerNum = idItem, ComputerID = s.ServerID });
                    idItem++;
                }
            }
            return connectedComputers;
        }

        public byte[] ReceiveData()
        {
            byte[] buffer = new byte[64 * 1024];
            int bytesRead = ccm.Receive(buffer, currentServer.GetChannel().GetCmdSocket());
            byte[] data = new byte[bytesRead];
            System.Buffer.BlockCopy(buffer, 0, data, 0, bytesRead);
            buffer = null;
            return bytesRead > 0 ? data : null;
        }

        public void EndConnectionToCurrentServer()
        {            
            InterceptEvents.ResetKModifier();
            SendRequest(Protocol.ProtocolUtils.SET_RESET_FOCUS, Protocol.ProtocolUtils.FOCUS_OFF);
            ReceiveAck();
            currentServer = null;
        }

        public void SendRequest(string requestType, Object content, Socket socket)
        {
            StandardRequest stdReq = new StandardRequest();
            stdReq.type = requestType;
            stdReq.content = content;
            string jsonTosend = JsonConvert.SerializeObject(stdReq);
            byte[] requestToSend = Encoding.Unicode.GetBytes(jsonTosend);
            byte[] toSend = new byte[requestToSend.Length + TokenGenerator.TOKEN_DIM];
            CurrentToken = tokenGen.GetNewToken();
            System.Buffer.BlockCopy(CurrentToken, 0, toSend, 0, TokenGenerator.TOKEN_DIM);
            System.Buffer.BlockCopy(requestToSend, 0, toSend, TokenGenerator.TOKEN_DIM, requestToSend.Length);
            ccm.Send(toSend, socket);
        }

        public void SendRequest(string requestType, Object content)
        {
            StandardRequest stdReq = new StandardRequest();
            stdReq.type = requestType;
            stdReq.content = content;
            string jsonTosend = JsonConvert.SerializeObject(stdReq);
            byte[] requestToSend = Encoding.Unicode.GetBytes(jsonTosend);
            byte[] toSend = new byte[requestToSend.Length + TokenGenerator.TOKEN_DIM];
            CurrentToken = tokenGen.GetNewToken();
            System.Buffer.BlockCopy(CurrentToken, 0, toSend, 0, TokenGenerator.TOKEN_DIM);
            System.Buffer.BlockCopy(requestToSend, 0, toSend, TokenGenerator.TOKEN_DIM, requestToSend.Length);
            ccm.Send(toSend, currentServer.GetChannel().GetCmdSocket());
        }

        public void AssignNewToken() {
            CurrentToken = tokenGen.GetNewToken();
        }

        public void StartNewConnection(int id)
        {
            Server newServer = this.ConnectedServer.Find(server => server.ServerID == id);
            currentServer = newServer;
            SendRequest(Protocol.ProtocolUtils.SET_RESET_FOCUS, Protocol.ProtocolUtils.FOCUS_ON);
            ReceiveAck();
            try
            {
                AssignDataChannel(newServer);
            }
            catch (Exception)
            {
                return;
            }            
        }

        public void ResetTokenGen()
        {
            this.tokenGen.Reset();
        }
    }
}
