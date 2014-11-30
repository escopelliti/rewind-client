using System;
using System.Collections.Generic;

using Bonjour;
using MainApp;
using ConnectionModule;

namespace Discovery
{
    public class ServiceDiscovery
    {
        private Bonjour.DNSSDEventManager eventManager = null;

        //Service object
        private Bonjour.DNSSDService dataService = null;
        private Bonjour.DNSSDService cmdService = null;
        
        //Browser object which browses the network for a service type
        private Bonjour.DNSSDService browser1 = null;
        private Bonjour.DNSSDService browser2 = null;

        //Resolver object which contains the results of a resolved domain name or a query record
        private Bonjour.DNSSDService resolver = null;

        private List<Server> serverList;
        private MainWindow mainWindow;
        public delegate void NewComputerEventHandler(Object sender, Object param);
        public event NewComputerEventHandler newComputerHandler;

        public delegate void LostComputerEventHandler(Object sender, Object param);
        public event LostComputerEventHandler lostComputerHandler;

        private void OnNewComputer(Server server)
        {
            NewComputerEventHandler handler = newComputerHandler;
            if (handler != null)
            {
                handler(this, server);
            }
        }

        public ServiceDiscovery(MainWindow mainWindow)
        {
            SetupEventManager();
            this.serverList = new List<Server>();
            BrowseServers();            
            this.mainWindow = mainWindow;
            this.newComputerHandler += mainWindow.OnNewComputerConnected;
            this.lostComputerHandler += mainWindow.OnLostComputerConnection;
            this.lostComputerHandler += mainWindow.channelMgr.OnLostComputerConnection;
        }

        private void SetupEventManager()
        {
            eventManager = new DNSSDEventManager();
            eventManager.ServiceFound += new _IDNSSDEvents_ServiceFoundEventHandler(this.ServiceFound);
            eventManager.ServiceLost += new _IDNSSDEvents_ServiceLostEventHandler(this.ServiceLost);
            eventManager.ServiceResolved += new _IDNSSDEvents_ServiceResolvedEventHandler(this.ServiceResolved);
            eventManager.QueryRecordAnswered += new _IDNSSDEvents_QueryRecordAnsweredEventHandler(this.QueryRecordAnswered);
            eventManager.OperationFailed += new _IDNSSDEvents_OperationFailedEventHandler(this.OperationFailed);
        }

        private void BrowseServers()
        {
            try
            {
                dataService = new Bonjour.DNSSDService();
                cmdService = new Bonjour.DNSSDService();
                browser1 = dataService.Browse(0, 0, "_dataListening._tcp", null, eventManager);
                browser2 = cmdService.Browse(0, 0, "_cmdListening._tcp", null, eventManager);
            }
            catch
            {
                System.Windows.MessageBox.Show("Si è riscontrato un grave problema. Prova a riavviare l'applicazione.", "Ops..", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                Environment.Exit(-1);
            }
        }

        public void QueryRecordAnswered(DNSSDService service, DNSSDFlags flags, uint ifIndex, String fullName, DNSSDRRType rrtype, DNSSDRRClass rrclass, Object rdata, uint ttl)
        {
            try
            {
                String hostname = fullName.Substring(0, fullName.IndexOf("."));
                Server server = this.serverList.Find(x => x.ComputerName == hostname);
                uint bits = BitConverter.ToUInt32((Byte[])rdata, 0);
                System.Net.IPAddress address = new System.Net.IPAddress(bits);
                server.GetChannel().ipAddress = address;
                OnNewComputer(server);
            }
            catch (Exception)
            {
                return;
            }            
        }

        public void OperationFailed(DNSSDService service, DNSSDError error)
        {
            System.Windows.MessageBox.Show("Si è riscontrato un grave problema. Prova a riavviare l'applicazione.", "Ops..", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            Environment.Exit(-1);
        }

        public void ServiceLost (DNSSDService sref, DNSSDFlags flags, uint ifIndex, String serviceName, String regType, String domain)
        {
            try 
            {
                Server server = this.serverList.Find(x => serviceName.Contains(x.ComputerName));
                if (server != null)
                {
                    this.serverList.Remove(server);                
                    server.GetChannel().GetCmdSocket().Shutdown(System.Net.Sockets.SocketShutdown.Both);
                    server.GetChannel().GetCmdSocket().Close();                                   
                    server.GetChannel().GetDataSocket().Shutdown(System.Net.Sockets.SocketShutdown.Both);
                    server.GetChannel().GetDataSocket().Close();
                }
                OnDisconnettedComputerService(server);
            }
            catch (NullReferenceException)
            {
                //nothing to close;
            }
            catch (Exception)
            {
                //nothing to do
            }          
        }

        private void OnDisconnettedComputerService(Server server)
        {
            LostComputerEventHandler handler = lostComputerHandler;
            if (handler != null)
            {
                handler(this, server);
            }
        }

        public void ServiceResolved(DNSSDService sref, DNSSDFlags flags, uint ifIndex, String fullName, String hostName, ushort port, TXTRecord txtRecord)
        {            
            String hostname = hostName.Substring(0, hostName.IndexOf("."));
            Server server = this.serverList.Find(x => x.ComputerName == hostname);           
            Channel channel;
            if (server == null)
            {
                server = new Server();
                server.ComputerName = hostname;
                channel = new Channel();
            }  else {
                this.serverList.Remove(server);
                channel = server.GetChannel();
            }
            if (fullName.Contains("_dataListening._tcp"))
            {
                channel.DataPort = port;
            }
            else
            {
                channel.CmdPort = port;
            }
            server.SetChannel(channel);
            server.Authenticated = false;
            this.serverList.Add(server);
            if (channel.CmdPort != 0 && channel.DataPort != 0)
            {
                try
                {
                    resolver = dataService.QueryRecord(0, ifIndex, hostName, DNSSDRRType.kDNSSDType_A, DNSSDRRClass.kDNSSDClass_IN, eventManager);
                }
                catch
                {
                    System.Windows.MessageBox.Show("QueryRecord Failed", "Error");
                }
            }
        }

        public void Stop()
        {
            try 
            {
                browser1.Stop();
                browser1 = null;
                dataService.Stop();
                dataService = null;
                resolver.Stop();
                resolver = null;
            }
            catch (Exception)
            {
                //already stopped - nothing to do;
                return;
            }
            
        }

        public void ServiceFound (DNSSDService sref, DNSSDFlags flags, uint ifIndex, String serviceName, String regType, String domain)
        {                
            resolver = dataService.Resolve(0, ifIndex, serviceName, regType, domain, eventManager);
        }        
    }
}
