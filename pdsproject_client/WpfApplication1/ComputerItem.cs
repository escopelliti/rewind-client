using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApplication1
{
    public class ComputerItem
    {
        public String Name { get; set; }
        public String ComputerStateImage { get; set; }
        public String focusedImage { get; set; }
        public State computerState { get; set; }
        public int computerNum { get; set; }
        public int computerID { get; set; }
    }

    public class State
    {
        public const string ACTIVE = "ACTIVE";
        public const string CONNECTED = "CONNECTED";
        public const string DISCONNECTED = "DISCONNECTED";
    }
}
