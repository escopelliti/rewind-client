using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApplication1
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
