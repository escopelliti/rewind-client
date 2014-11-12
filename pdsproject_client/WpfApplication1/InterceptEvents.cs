using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Forms;
using CommunicationLibrary;
using NativeInput;

namespace WpfApplication1
{
    [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
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


        private static IntPtr hInstance;
        // TO MODIFY


        public InterceptEvents(/*ChannelManager ChannelManager,*/)
        {
            //channelMgr = ChannelManager;
            hInstance = LoadLibrary("User32");
            _proc += HookCallback;
            _proc_ += HookCallbackMouse;
            inputFactory = new InputFactory();
            StartCapture(); 
        }

        private static void StartCapture()
        {

            //_hookID_ = SetWindowsHookEx(WH_MOUSE_LL, _proc_, IntPtr.Zero, AppDomain.GetCurrentThreadId()/*System.Threading.Thread.CurrentThread.ManagedThreadId*/);
            //Console.WriteLine(Marshal.GetLastWin32Error());
            
            hookID = SetWindowsHookEx(WH_KEYBOARD_LL, _proc, IntPtr.Zero, 0);
        }

        public static void RestartCapture()
        {
            block = false;
       
        }

        public static void StopCapture()
        {
            block = true;
        }

        private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (!block)
            {
                if (nCode >= 0)
                {
                    INPUT inputToSend = inputFactory.CreateKeyboardInput((IntPtr)wParam, (IntPtr)lParam);
                    //channelMgr.sendInputToSever(inputToSend);
                    if ((Keys)Marshal.ReadInt32(lParam) == Keys.LWin || (Keys)Marshal.ReadInt32(lParam) == Keys.RWin)
                        return (IntPtr)1;

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
            ResetKModifier();
            OpenWorkareaWindow();
        }

        private void ResetKModifier()
        {

            INPUT inputToSend = inputFactory.CreateKeyUpInput(Keys.Control);
            //channelMgr.sendInputToSever(inputToSend);
            inputToSend = inputFactory.CreateKeyUpInput(Keys.Shift);
            //channelMgr.sendInputToSever(inputToSend);
            inputToSend = inputFactory.CreateKeyUpInput(Keys.Alt);
            //channelMgr.sendInputToSever(inputToSend);

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


        [DllImport("user32.dll", CharSet=CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet=CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelMouseProc lpfn, IntPtr hMod, int dwThreadId);

        [DllImport("user32.dll")]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("kernel32.dll")]
        static extern IntPtr LoadLibrary(string lpFileName);
       
        [DllImport("user32.dll")]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", EntryPoint = "CopyMemory", SetLastError = false)]
        public static extern void CopyMemory(IntPtr dest, IntPtr src, uint count);

        public static void OnSetNewServer(object sender, object param)
        {
            RestartCapture();
            //StartCapture();
        }



    }
}
