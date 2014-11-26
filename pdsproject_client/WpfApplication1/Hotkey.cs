using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;


namespace WpfApplication1
{
    public class Hotkey
    {
        private ModifierKeys kModifier;
        private Key key;
        private string command;

        public const string SWITCH_SERVER_CMD = "SWITCH_SERVER";
        public const string OPEN_PANEL_CMD = "OPEN_PANEL";
        public const string REMOTE_PAST_CMD = "REMOTE_PAST";

        public Hotkey(ModifierKeys km, Key k, string c)
        {
            kModifier = km;
            key = k;
            command = c;
        }

        public ModifierKeys KModifier
        {
            get { return kModifier; }
            set { kModifier = value; } 
        }

        public Key Key
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
