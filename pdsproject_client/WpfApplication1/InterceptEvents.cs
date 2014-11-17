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

using System.Threading;

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

        public InterceptEvents(ChannelManager ChannelManager)
        {
            channelMgr = ChannelManager;
            _proc += HookCallback;
            _proc_ += HookCallbackMouse;
            inputFactory = new InputFactory();            
            StartCapture(); 
        }

        private static void StartCapture()
        {
            hookID = SetWindowsHookEx(WH_KEYBOARD_LL, _proc, IntPtr.Zero, 0);
            _hookID_ = SetWindowsHookEx(WH_MOUSE_LL, _proc_, IntPtr.Zero, 0);   
        }


        [DllImport("kernel32.dll")]
        public static extern IntPtr GetModuleHandle(string lpModuleName);

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
                    List<INPUT> inputToSendArray = inputFactory.CreateKeyboardInput((IntPtr)wParam, (IntPtr)lParam);
                    foreach (INPUT inputToSend in inputToSendArray)
                    {
                        channelMgr.SendInputToSever(inputToSend);
                    }
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
                    channelMgr.SendInputToSever(inputToSend);
                }
                return CallNextHookEx(hookID, nCode, wParam, lParam);
            }
            return IntPtr.Zero;
        }        

        public static void ResetKModifier()
        {
            INPUT inputToSend = inputFactory.CreateKeyUpInput(Keys.Control);
            channelMgr.SendInputToSever(inputToSend);
            inputToSend = inputFactory.CreateKeyUpInput(Keys.Shift);
            channelMgr.SendInputToSever(inputToSend);
            inputToSend = inputFactory.CreateKeyUpInput(Keys.Alt);
            channelMgr.SendInputToSever(inputToSend);
        } 

        public static void OnSetNewServer(object sender, object param)
        {
            RestartCapture();
        }
       
        [DllImport("user32.dll", CharSet=CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet=CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelMouseProc lpfn, IntPtr hMod, int dwThreadId);

        [DllImport("user32.dll")]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll")]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

    }
}
