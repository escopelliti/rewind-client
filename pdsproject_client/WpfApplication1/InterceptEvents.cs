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
using System.Diagnostics;

namespace WpfApplication1
{
    public class InterceptEvents
    {

        private const int WH_KEYBOARD_LL = 13;
        private const int WH_MOUSE_LL = 14;
        private static bool block = false;

        private static IntPtr hookID = IntPtr.Zero;
        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);
        private static LowLevelKeyboardProc _proc;


        private static IntPtr _hookID_ = IntPtr.Zero;
        private delegate IntPtr LowLevelMouseProc(int nCode, IntPtr wParam, IntPtr lParam);
        private static LowLevelMouseProc _proc_;

        private static InputFactory inputFactory;
        private static ChannelManager channelMgr;

        private static HotkeyManager hotkeyMgr;

        private static IntPtr hInstance;
        // TO MODIFY
        

        public InterceptEvents(ChannelManager ChannelManager, IntPtr windowHandle)
        {
            channelMgr = ChannelManager;
            hInstance = LoadLibrary("User32");
            _proc += HookCallback;
            _proc_ += HookCallbackMouse;
            inputFactory = new InputFactory();            
            CreateHotKeys(windowHandle);
            FullScreenRemoteServerControl fullScreenWin = new FullScreenRemoteServerControl(hotkeyMgr.RegisteredHotkey, channelMgr.getCurrentServer(), channelMgr.ConnectedServer);
            fullScreenWin.Show();
            StartCapture(); 
        }

        private void CreateHotKeys(IntPtr windowHandle)
        {
            //  TO DO ... Apre il file di configurazione e leggere le hotkey impostate dall'utente
            Hotkey hot = new Hotkey(0, HotkeyManager.KeyModifier.Control, Keys.A, HotkeyManager.SWITCH_SERVER_CMD);
            Hotkey hot2 = new Hotkey(0, HotkeyManager.KeyModifier.Control, Keys.B, HotkeyManager.OPEN_PANEL_CMD);
            //NON FUNZIONANO CTRL + ESCAPE, CTRL + NumLock, ALt + tab,

            List<Hotkey> hotkeyList = new List<Hotkey>();
            hotkeyList.Add(hot);
            hotkeyList.Add(hot2);

            hotkeyMgr = new HotkeyManager(windowHandle, hotkeyList, this);
        }

        private static void StartCapture()
        {
            
            _hookID_ = SetWindowsHookEx(WH_MOUSE_LL, _proc_, IntPtr.Zero, 0);            
            hookID = SetWindowsHookEx(WH_KEYBOARD_LL, _proc, IntPtr.Zero, 0);
        }


        [DllImport("kernel32.dll")]
        public static extern IntPtr GetModuleHandle(string lpModuleName);

        public static void RestartCapture()
        {
            block = false;
            //_proc += HookCallback;
            //_proc_ += HookCallbackMouse;
        }

        public static void StopCapture()
        {
            //_proc -= HookCallback;
            //_proc_ -= HookCallbackMouse;
            block = true;
            //UnhookWindowsHookEx(_hookID_);
            //UnhookWindowsHookEx(hookID);
        }

        private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (!block)
            {
                if (nCode >= 0)
                {
                    //int wparam = (int) wParam;
                    INPUT inputToSend = inputFactory.CreateKeyboardInput((IntPtr)wParam, (IntPtr)lParam);
                    channelMgr.sendInputToSever(inputToSend);
                    if ((Keys)Marshal.ReadInt32(lParam) == Keys.LWin || (Keys)Marshal.ReadInt32(lParam) == Keys.RWin)
                        return (IntPtr)1;

                    //Message msg = new Message();
                    //if (GetMessage(ref msg, IntPtr.Zero, 0, 0))
                    //{
                    //    if (msg.Msg == (int)KeyboardMessages.WM_HOTKEY)
                    //    {
                    //        int id = msg.WParam.ToInt32();

                    //        hotkeyMgr.HotkeyPressed(id);
                    //    }
                    //    else
                    //    {
                    //    }
                    //}

                }

                return CallNextHookEx(hookID, nCode, wParam, lParam);
            }
            return IntPtr.Zero;
        }
        
        private static IntPtr HookCallbackMouse(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (!block)
            {
                if (nCode >= 0)
                {
                    INPUT inputToSend = inputFactory.CreateMouseInput(wParam, lParam);
                    channelMgr.sendInputToSever(inputToSend);
                }
                return CallNextHookEx(hookID, nCode, wParam, lParam);
            }
            return IntPtr.Zero;
        }

        public void OnSwitch(object sender, object param)
        {
            StopCapture();
            //block = true;
            OpenWorkareaWindow();
        }

        private void OpenWorkareaWindow()
        {
            if (channelMgr.getCurrentServer() != null) {
                WorkareaWindow wk = new WorkareaWindow(channelMgr);            
                wk.computerList.ItemsSource = channelMgr.GetComputerItemList();
                wk.Show();
            } else {
                System.Windows.MessageBox.Show("Non hai ancora un computer attivo. Selezionane uno!", "Ops...", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning); 
            }
        }
       
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, int dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelMouseProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll")]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("kernel32.dll")]
        static extern IntPtr LoadLibrary(string lpFileName);
       
        [DllImport("user32.dll")]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        private static extern bool GetMessage(ref Message lpMsg, IntPtr handle, uint mMsgFilterMain, uint mMsgFilerMax);

        [DllImport("kernel32.dll", EntryPoint = "CopyMemory", SetLastError = false)]
        public static extern void CopyMemory(IntPtr dest, IntPtr src, uint count);

        public static void OnSetNewServer(object sender, object param)
        {
            RestartCapture();
            //StartCapture();
        }
    }
}
