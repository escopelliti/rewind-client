using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bonjour;
using WpfApplication1;

namespace Discovery
{
    public class ServiceDiscovery
    {
        private Bonjour.DNSSDEventManager m_eventManager = null;

        //Service object
        private Bonjour.DNSSDService m_service = null;
        
        //Browser object which browses the network for a service type
        private Bonjour.DNSSDService m_browser = null;

        //Resolver object which contains the results of a resolved domain name or a query record
        private Bonjour.DNSSDService m_resolver = null;

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
            BrowseServers();
            this.serverList = new List<Server>();
            this.mainWindow = mainWindow;
            this.newComputerHandler += mainWindow.OnNewComputerConnected;
            this.lostComputerHandler += mainWindow.OnLostComputerConnection;
            this.lostComputerHandler += mainWindow.channelMgr.OnLostComputerConnection;
        }

        private void SetupEventManager()
        {
            m_eventManager = new DNSSDEventManager();
            m_eventManager.ServiceFound += new _IDNSSDEvents_ServiceFoundEventHandler(this.ServiceFound);
            m_eventManager.ServiceLost += new _IDNSSDEvents_ServiceLostEventHandler(this.ServiceLost);
            m_eventManager.ServiceResolved += new _IDNSSDEvents_ServiceResolvedEventHandler(this.ServiceResolved);
            m_eventManager.QueryRecordAnswered += new _IDNSSDEvents_QueryRecordAnsweredEventHandler(this.QueryAnswered);
            m_eventManager.OperationFailed += new _IDNSSDEvents_OperationFailedEventHandler(this.OperationFailed);
        }



        private void BrowseServers()
        {
            try
            {
                m_service = new Bonjour.DNSSDService();
                m_browser = m_service.Browse(0, 0, "_dataListening._tcp", null, m_eventManager);
                m_browser = m_service.Browse(0, 0, "_cmdListening._tcp", null, m_eventManager);
            }
            catch
            {
                System.Windows.MessageBox.Show("Browse Failed", "Error");
            }
        }

        public void QueryAnswered(DNSSDService service, DNSSDFlags flags, uint ifIndex,
            String fullName, DNSSDRRType rrtype, DNSSDRRClass rrclass, Object rdata, uint ttl)
        {
            //
            // Stop the resolve to reduce the burden on the network
            //
            //m_resolver.Stop();
            //m_resolver = null;
            String hostname = fullName.Substring(0, fullName.IndexOf("."));
            Server server = this.serverList.Find(x => x.ComputerName == hostname);
            uint bits = BitConverter.ToUInt32((Byte[])rdata, 0);                        
            System.Net.IPAddress address = new System.Net.IPAddress(bits);
            server.GetChannel().ipAddress = address;
            OnNewComputer(server);
        }

        public void
       OperationFailed
                   (
                   DNSSDService service,
                   DNSSDError error
                   )
        {
            System.Windows.MessageBox.Show("Operation returned an error code " + error, "Error");
        }

        //
        // ServiceLost
        //
        // Called by DNSServices core as a result of a Browse call
        //
        public void
        ServiceLost
                    (
                    DNSSDService sref,
                    DNSSDFlags flags,
                    uint ifIndex,
                    String serviceName,
                    String regType,
                    String domain
                    )
        {
            Server server = this.serverList.Find(x => serviceName.Contains(x.ComputerName));
            if (server != null)
            {
                this.serverList.Remove(server);
                try 
                {
                    server.GetChannel().GetCmdSocket().Shutdown(System.Net.Sockets.SocketShutdown.Both);
                    server.GetChannel().GetCmdSocket().Close();                                   
                    server.GetChannel().GetDataSocket().Shutdown(System.Net.Sockets.SocketShutdown.Both);
                    server.GetChannel().GetDataSocket().Close();
                }
                catch (NullReferenceException nre)
                {
                    //nothing to close;
                }
                OnDisconnettedComputerService(server);
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

        //
        // ServiceResolved
        //
        // Called by DNSServices core as a result of DNSService.Resolve()
        // call
        //
        public void ServiceResolved
                    (
                    DNSSDService sref,
                    DNSSDFlags flags,
                    uint ifIndex,
                    String fullName,
                    String hostName,
                    ushort port,
                    TXTRecord txtRecord
                    )
        {
            //m_resolver.Stop();
           // m_resolver = null;
            String hostname = hostName.Substring(0, hostName.IndexOf("."));

            Server server = this.serverList.Find(x => x.ComputerName == hostname);
           
            CommunicationLibrary.Channel channel;
            if (server == null)
            {
                server = new Server();
                server.ComputerName = hostname;
                channel = new CommunicationLibrary.Channel();
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
            this.serverList.Add(server);
            //
            // Now query for the IP address associated with "hostName"
            //
            if (channel.CmdPort != 0 && channel.DataPort != 0)
            {
                try
                {
                    m_resolver = m_service.QueryRecord(0, ifIndex, hostName, DNSSDRRType.kDNSSDType_A, DNSSDRRClass.kDNSSDClass_IN, m_eventManager);
                }
                catch
                {
                    System.Windows.MessageBox.Show("QueryRecord Failed", "Error");
                }
            }
        }


        public void ServiceFound (
                    DNSSDService sref,
                    DNSSDFlags flags,
                    uint ifIndex,
                    String serviceName,
                    String regType,
                    String domain)
        {
                //resolve the name in order to obtain the IP Address and port
            m_resolver = m_service.Resolve(0, ifIndex, serviceName, regType, domain, m_eventManager);
        }
        
    }
}
