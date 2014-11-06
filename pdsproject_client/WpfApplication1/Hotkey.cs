using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WpfApplication1
{
    public class Hotkey
    {
        private int idHotkey;
        HotkeyManager.KeyModifier kModifier;
        Keys key;
        string command;

        public Hotkey() { }

        public Hotkey(int id, HotkeyManager.KeyModifier km, Keys k, string c)
        {
            idHotkey = id;
            kModifier = km;
            key = k;
            command = c;
        }
        
        public int IdHotkey
        {
            get { return idHotkey; }
            set { idHotkey = value; }
        }

        public HotkeyManager.KeyModifier KModifier
        {
            get { return kModifier; }
            set { kModifier = value; } 
        }

        public Keys Key
        {
            get { return key; }
            set { key = value; }
        }
        
        public string Command
        {
            get { return command; }
            set { command = value; }
        }

    }
}
