using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Forms;

using ConnectionModule;
using KeyboardMouseController.NativeInput;

namespace KeyboardMouseController.HookMgr
{
    public class InterceptEvents
    {

        private const int WH_KEYBOARD_LL = 13;
        private const int WH_MOUSE_LL = 14;
        private static bool block = false;

        private static IntPtr keyboardHookHandle = IntPtr.Zero;
        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);
        private static LowLevelKeyboardProc keyboardProc;

        private static IntPtr mouseHookHandle = IntPtr.Zero;
        private delegate IntPtr LowLevelMouseProc(int nCode, IntPtr wParam, IntPtr lParam);
        private static LowLevelMouseProc mouseProc;

        private static InputFactory inputFactory;
        private static ChannelManager channelMgr;
 
        public InterceptEvents(ChannelManager ChannelManager)
        {
            channelMgr = ChannelManager;
            keyboardProc += HookCallback;
            mouseProc += HookCallbackMouse;
            inputFactory = new InputFactory();            
            StartCapture(); 
        }

        private static void StartCapture()
        {
            keyboardHookHandle = SetWindowsHookEx(WH_KEYBOARD_LL, keyboardProc, IntPtr.Zero, 0);
            mouseHookHandle = SetWindowsHookEx(WH_MOUSE_LL, mouseProc, IntPtr.Zero, 0);   
        }
        
        public static void SwitchBlock()
        {
            block = !block;
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
                    List<INPUT> inputToSendArray = inputFactory.CreateKeyboardInput((IntPtr)wParam, (IntPtr)lParam);
                    
                    foreach (INPUT inputToSend in inputToSendArray)
                    {
                        channelMgr.SendInputToSever(inputToSend);
                    }
                    if ((Keys)Marshal.ReadInt32(lParam) == Keys.LWin || (Keys)Marshal.ReadInt32(lParam) == Keys.RWin)
                        return (IntPtr)1;
                }

                return CallNextHookEx(keyboardHookHandle, nCode, wParam, lParam);
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
                return CallNextHookEx(keyboardHookHandle, nCode, wParam, lParam);
            }
            return IntPtr.Zero;
        }        

        public static void ResetKModifier()
        {

            INPUT inputToSend = inputFactory.CreateKeyUpInput(Keys.LControlKey);
            channelMgr.SendInputToSever(inputToSend);
            inputToSend = inputFactory.CreateKeyUpInput(Keys.LShiftKey);
            channelMgr.SendInputToSever(inputToSend);
            inputToSend = inputFactory.CreateKeyUpInput(Keys.LMenu);
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
