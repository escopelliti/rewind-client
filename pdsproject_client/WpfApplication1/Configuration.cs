using System;
using System.Collections.Generic;
using KeyboardMouseController;

namespace ClientConfiguration
{
    public class Configuration {

        public Configuration()
        {
            hotkeyList = new List<Hotkey>();
        }
        
        public List<Hotkey> hotkeyList { get; set; }
    }
}
