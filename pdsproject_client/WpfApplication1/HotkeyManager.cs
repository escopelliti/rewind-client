using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using NativeInput;
using System.Windows.Forms;



namespace WpfApplication1
{
    class HotkeyManager
    {
        IntPtr windowHandle;

        public HotkeyManager(IntPtr Handle)
        {
            windowHandle = Handle;
            
            //int id = 0;     // The id of the hotkey. 
            if (!RegisterHotKey(windowHandle, 0, (int)KeyModifier.Control, Keys.T.GetHashCode())) 
            { 
                Console.WriteLine("fallito 1"); 
            }
            
            if (!RegisterHotKey(windowHandle, 1, (int)KeyModifier.Control, Keys.Y.GetHashCode())) 
            { 
                Console.WriteLine("fallito 2"); 
            }
        }

        public void ChangeSwitchHotkey(KeyModifier km, Keys k, int id){
            if (!RegisterHotKey(windowHandle, 2, (int)km, k.GetHashCode()))
            {
                Console.WriteLine("fallito 3");
            }
        }


       public enum KeyModifier
        {
            None = 0,
            Alt = 1,
            Control = 2,
            Shift = 4,
            WinKey = 8
        }


        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vk);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

    }

}
