using System;
using ConnectionModule;

namespace GenericDataStructure
{
    public class ServerEventArgs : EventArgs
    {
        public Server Server { get; set; }

        public ServerEventArgs(Server s)
        {
            this.Server = s;
        }
    }
}
