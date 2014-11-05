using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using NativeInput;
using CommunicationLibrary;
using System.Windows;


namespace WpfApplication1
{
    class HotkeyManager
    {
        IntPtr windowHandle;
        List<Hotkey> registeredHotkey;
        private static Dictionary<string, Delegate> commandActionDictionary;

        ChannelManager cm;
        
        
        public delegate void SwitchServerEventHandler(Object sender, Object param);
        public event SwitchServerEventHandler SwitchServeHandler;

        //public delegate void OpenPanelEventHandler(Object sender, Object param);
        //public event OpenPanelEventHandler OpnePanelHandler;


        public HotkeyManager(IntPtr Handle, List<Hotkey> hotkeys,ChannelManager channelManager )
        {
            windowHandle = Handle;
            cm = channelManager;
            registeredHotkey = new List<Hotkey>();
            commandActionDictionary = new Dictionary<string,Delegate>();

            foreach (Hotkey h in hotkeys)
            {
                AddHotkey(h); 
            }

        }

        protected virtual void OnSwitch(EventArgs eventArgs)
        {
            SwitchServerEventHandler handler = SwitchServeHandler;
            if (handler != null)
            {
                // Invoco il delegato 
                handler(this, eventArgs);
            }
        }


        public void DeleteHotekey(Hotkey h)
        {
            if (registeredHotkey.Contains(h))
            {
                registeredHotkey.Remove(h);
                if (!UnregisterHotKey(windowHandle, h.IdHotkey))
                {
                    throw new InvalidOperationException("unable to unregister hotkey");

                }
            }

            else
            {
                MessageBox.Show("404 - Hotkey not found",
                   "Errore!", MessageBoxButton.OK, MessageBoxImage.Error); 
            } 
        }

        public void AddHotkey(Hotkey h)
        {
            if (registeredHotkey.Exists(x => x.Command == h.Command))
            {
                List<Hotkey> toCancelHotkeys = registeredHotkey.FindAll(x => x.Command == h.Command);
                foreach (Hotkey hotkeyElement in toCancelHotkeys)
                {
                    if (UnregisterHotKey(windowHandle, hotkeyElement.IdHotkey))
                    {
                        registeredHotkey.Remove(hotkeyElement);
                    }
                }
            }

            if (registeredHotkey.Exists(x => (x.KModifier == h.KModifier && x.Key == h.Key)))
            {
                MessageBox.Show("Non puoi inserire due combinazioni di tasti uguali",
                    "Attenzione!", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            
            if (registeredHotkey.Exists(x => x.IdHotkey == h.IdHotkey))
                h.IdHotkey = FindFreeIdHotkey();

            try
            {
                RegisterHotkey(h);
            }
            catch (InvalidOperationException e)
            {
                MessageBox.Show("Errore durante la registrazione della hotkey",
                    "Attenzione!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
        private void RegisterHotkey(Hotkey h)
        {
            if (RegisterHotKey(windowHandle, h.IdHotkey, (int)h.KModifier, h.Key.GetHashCode()))
            {
                switch (h.Command)
                {
                    case "switchServer":
                        // Istanzio il delegato dell'evento
                        SwitchServeHandler = new SwitchServerEventHandler(cm.OnSwitch);
                        // Inserisco il comando nel Dictionary 
                        commandActionDictionary[h.Command] = new Action<Object>(obj => OnSwitch(new EventArgs()));
                        break;

                    case "openPanel":
                        break;

                    default:
                        break;
                }

                registeredHotkey.Add(h);
            }
            else
            {
                throw new InvalidOperationException("unable to register hotkey");
            }
        }

        public int FindFreeIdHotkey()
        {
            int id = 0;
            bool found = false;
            while (!found)
            {
                if (registeredHotkey.Exists(x => x.IdHotkey == id))
                    id++;
                else
                    found = true;
            }
            return id;
        }

        //public void ChangeHotkey(int id, KeyModifier km, Keys k)
        //{
        //    if (!RegisterHotKey(windowHandle, 2, (int)km, k.GetHashCode()))
        //    {
        //        Console.WriteLine("fallito 3");
        //    }
        //}

        
        public void HotkeyPressed(int id)
        {
            string command = " ";
            foreach (Hotkey hotkeyElement in registeredHotkey)
            {
                if (hotkeyElement.IdHotkey == id)
                {
                    command = hotkeyElement.Command;
                    commandActionDictionary[command].DynamicInvoke(command);
                }
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
