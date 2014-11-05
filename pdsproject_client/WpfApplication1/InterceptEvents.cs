using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Windows.Input;

//HOTkey
using System.Windows.Forms;


using CommunicationLibrary;
using NativeInput;
using System.Windows.Interop;

namespace WpfApplication1
{
    class InterceptEvents
    {

        private const int WH_KEYBOARD_LL = 13;
        private const int WH_MOUSE_LL = 14;


        private static IntPtr hookID = IntPtr.Zero;
        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);
        private static LowLevelKeyboardProc _proc = HookCallback;


        private static IntPtr _hookID_ = IntPtr.Zero;
        private delegate IntPtr LowLevelMouseProc(int nCode, IntPtr wParam, IntPtr lParam);
        private static LowLevelMouseProc _proc_ = HookCallbackMouse;

        private static InputFactory inputFactory;
        private static ChannelManager chManager;

        private static HotkeyManager hotkey;


        public InterceptEvents(ChannelManager channelManager, IntPtr windowHandle)
        {
            chManager = channelManager;
            inputFactory = new InputFactory();
            IntPtr hInstance = LoadLibrary("User32");
            _hookID_ = SetWindowsHookEx(WH_MOUSE_LL, _proc_, hInstance, 0);
            hookID = SetWindowsHookEx(WH_KEYBOARD_LL, _proc, IntPtr.Zero, 0);

            //  TO DO ... Apre il file di configurazione e leggere le hotkey impostate dall'utente
            Hotkey hot = new Hotkey(0, HotkeyManager.KeyModifier.Control, Keys.A, "switchServer");
            Hotkey hot2 = new Hotkey(0, HotkeyManager.KeyModifier.Control, Keys.B, "openPanel");
            //NON FUNZIONANO CTRL + ESCAPE, CTRL + NumLock, ALt + tab,

            List<Hotkey> hotkeyList = new List<Hotkey>();
            hotkeyList.Add(hot);
            hotkeyList.Add(hot2);
            
            hotkey = new HotkeyManager(windowHandle,hotkeyList,chManager);
                      
        }

        public void closeInterceptEvents()
        {
            UnhookWindowsHookEx(_hookID_);
            UnhookWindowsHookEx(hookID);
        }

        private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            
            if (nCode >= 0)
            {
                Message msg = new Message();
                if (GetMessage(ref msg, IntPtr.Zero, 0, 0))
                {
                    if (msg.Msg == (int)KeyboardMessages.WM_HOTKEY)
                    {
                        Console.WriteLine("Hotkey has been pressed!");

                        int id = msg.WParam.ToInt32();
                        Console.WriteLine(id);

                        hotkey.HotkeyPressed(id);         
                    }

                }
                INPUT inputToSend = inputFactory.CreateKeyboardInput(wParam, lParam);
                //chManager.sendInputToSever(inputToSend);
                
                if ((Key)Marshal.ReadInt32(lParam) == Key.LWin || (Key)Marshal.ReadInt32(lParam) == Key.RWin)
                    return (IntPtr)1;
            }           

            return CallNextHookEx(hookID, nCode, wParam, lParam);
        }
        
        private static IntPtr HookCallbackMouse(int nCode, IntPtr wParam, IntPtr lParam)
        {

           

            if (nCode >= 0)
            {
                INPUT inputToSend = inputFactory.CreateMouseInput(wParam, lParam);
                //chManager.sendInputToSever(inputToSend);
            }
            return CallNextHookEx(hookID, nCode, wParam, lParam);
        }


        [DllImport("user32.dll")]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll")]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelMouseProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll")]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("kernel32.dll")]
        static extern IntPtr LoadLibrary(string lpFileName);

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        internal static extern short GetKeyState(int virtualKeyCode);

        [DllImport("user32.dll")]
        private static extern uint MapVirtualKey(uint uCode, uint uMapType);

        [DllImport("user32.dll")]
        private static extern int ToAscii(uint uVirtKey, uint uScanCode, byte[] lpKeyState, StringBuilder lpChar, uint flags);

        [DllImport("user32.dll")]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);


        public const int WM_HOTKEY = 0x312;

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vk);
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        [DllImport("user32.dll")]
        private static extern bool GetMessage(ref Message lpMsg, IntPtr handle, uint mMsgFilterMain, uint mMsgFilerMax);
    
    }
}
