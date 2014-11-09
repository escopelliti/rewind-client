using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bonjour;

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

        public ServiceDiscovery()
        {
            m_eventManager = new DNSSDEventManager();
            m_eventManager.ServiceFound += new _IDNSSDEvents_ServiceFoundEventHandler(this.ServiceFound);
            m_eventManager.ServiceLost += new _IDNSSDEvents_ServiceLostEventHandler(this.ServiceLost);
            m_eventManager.ServiceResolved += new _IDNSSDEvents_ServiceResolvedEventHandler(this.ServiceResolved);
            m_eventManager.QueryRecordAnswered += new _IDNSSDEvents_QueryRecordAnsweredEventHandler(this.QueryAnswered);
            m_eventManager.OperationFailed += new _IDNSSDEvents_OperationFailedEventHandler(this.OperationFailed);

            try
            {
                //creates a new service object and browses for the service type "_messageStreamer._tcp"
                m_service = new Bonjour.DNSSDService();
                m_browser = m_service.Browse(0, 0, "_dataListening._tcp", null, m_eventManager);
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
            m_resolver.Stop();
            m_resolver = null;

            //Obtains the IP address of the service that it was querying for 
            uint bits = BitConverter.ToUInt32((Byte[])rdata, 0);
            System.Net.IPAddress address = new System.Net.IPAddress(bits);
            Console.WriteLine(address.ToString());
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
            Console.WriteLine("SERVICE LOST");
        }

        //
        // ServiceResolved
        //
        // Called by DNSServices core as a result of DNSService.Resolve()
        // call
        //
        public void
        ServiceResolved
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
            m_resolver.Stop();
            m_resolver = null;

            //received the port number of the service 
            Console.WriteLine("port " + port);

            //
            // Now query for the IP address associated with "hostName"
            //
            try
            {
                m_resolver = m_service.QueryRecord(0, ifIndex, hostName, DNSSDRRType.kDNSSDType_A, DNSSDRRClass.kDNSSDClass_IN, m_eventManager);
            }
            catch
            {
                System.Windows.MessageBox.Show("QueryRecord Failed", "Error");
                
            }
        }


        public void
        ServiceFound
                    (
                    DNSSDService sref,
                    DNSSDFlags flags,
                    uint ifIndex,
                    String serviceName,
                    String regType,
                    String domain
                    )
        {
            //check to see that this is a new service instance that we have not already found.
            //if (serviceName != m_name)
            //{
            //    InterfaceIndex = ifIndex;
            //    Name = serviceName;
            //    Type = regType;
            //    Domain = domain;
            //    Address = null;

                //resolve the name in order to obtain the IP Address and port
            m_resolver = m_service.Resolve(0, ifIndex, serviceName, regType, domain, m_eventManager);
            //}
        }
        
    }
}
