﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApplication1
{
    public class Configuration {

        public Configuration()
        {
            hotkeyList = new List<Hotkey>();
        }
        
        public List<Hotkey> hotkeyList { get; set; }

    }
}
