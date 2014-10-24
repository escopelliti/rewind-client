using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Text;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Input;
using CommunicationLibrary;
using System.Net.Sockets;
using Newtonsoft.Json;


class InterceptKeys
{
    private const int WH_KEYBOARD_LL = 13;
    private const int WM_KEYDOWN = 0x0100;
    private const int WM_KEYUP = 0x0101;
    private const int WH_MOUSE_LL = 14;

    private static IntPtr hookID = IntPtr.Zero;
    private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

    private static IntPtr _hookID_ = IntPtr.Zero;
    private delegate IntPtr LowLevelMouseProc(int nCode, IntPtr wParam, IntPtr lParam);
    
    
    private static ClientCommunicationManager ccm;
    private static Socket socket;

    static void Main()
    {
        ccm = new ClientCommunicationManager();
        socket = ccm.CreateSocket(ProtocolType.Tcp);
        socket = ccm.Connect("INSIDEMYHEAD", 12001, socket);


        _hookID_ = SetWindowsHookEx(WH_MOUSE_LL, HookCallbackMouse, IntPtr.Zero, 0);
        hookID = SetWindowsHookEx(WH_KEYBOARD_LL, HookCallback, IntPtr.Zero, 0);
        
        Application.Run();

        UnhookWindowsHookEx(_hookID_);
        UnhookWindowsHookEx(hookID);
    }

    
    private enum MouseMessages
    {
        WM_LBUTTONDOWN = 0x0201,
        WM_LBUTTONUP = 0x0202,
        WM_MOUSEMOVE = 0x0200,
        WM_MOUSEWHEEL = 0x020A,
        WM_RBUTTONDOWN = 0x0204,
        WM_RBUTTONUP = 0x0205
    }


    private static IntPtr HookCallbackMouse(int nCode, IntPtr wParam, IntPtr lParam)
    {


        if (nCode >= 0 && (MouseMessages)wParam == MouseMessages.WM_MOUSEMOVE)
        {
            Console.WriteLine("Mouse");

            INPUT input_move = new INPUT();
            input_move.type = 0;
            input_move.mi = (MOUSEINPUT)Marshal.PtrToStructure(lParam, typeof(MOUSEINPUT));
            input_move.mi.dwFlags = (int)(MouseFlag.Move | MouseFlag.Absolute);
            string json = JsonConvert.SerializeObject(input_move);
            byte[] toSend = Encoding.Unicode.GetBytes(json);
            ccm.Send(toSend, socket);
            //ccm.Receive(new byte[5], socket);

            
        }

        return CallNextHookEx(_hookID_, nCode, wParam, lParam); 
    }


    private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
    
    {
        Keys key = 0;

        if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN)
        {

            //Console.WriteLine("CACCACACACACACACAAC");
            //RECOVERY LCONTROL PRESS
            //INPUT input_move4 = new INPUT();
            //input_move4.type = 1;
            //input_move4.ki = new KEYBDINPUT();
            //input_move4.ki.wVk = VirtualKeyCode.LSHIFT;
            //input_move4.ki.wScan = 0;
            //input_move4.ki.dwFlags = KeyboardFlag.KeyUp;
            //input_move4.ki.dwExtraInfo = IntPtr.Zero;
            //string json = JsonConvert.SerializeObject(input_move4);
            //byte[] toSend = Encoding.Unicode.GetBytes(json);
            //ccm.Send(toSend, socket);


            key = (Keys)Marshal.ReadInt32(lParam);
            //keyState
            byte[] ks = new byte[256];
            for (int x = 0; x < 256; x++)
                ks[x] = (byte)GetKeyState(x);
            string keyc;
            string s = "";

            INPUT input_move = new INPUT();
            input_move.type = 1;
            input_move.ki = new KEYBDINPUT();


            if ((key >= Keys.D0 && key <= Keys.Z) ||
                (key >= Keys.NumPad0 && key <= Keys.NumPad9 && Control.IsKeyLocked(Keys.NumLock)) ||
                (key >= Keys.Oem1 && key <= Keys.Oem102))
            {
                StringBuilder sb = new StringBuilder(1);
                //ToAscii() - converte in carattere lo scancode e il virtualkey che gli passo 
                //considerando lo stato anche di altri tasti perchè lo legge da ks
                //e mettendo il risultato nel buffer sb
                // MapVirtualKey() dal virtualkey ottien lo scancode
                ToAscii((uint)key, MapVirtualKey((uint)key, 0), ks, sb, 0);
                /*keyc*/
                s = sb.ToString(0, 1);
                /*s = keyc;*/

                char[] f = s.ToCharArray();


                 //   CLIENT
                
                 //     |
                 //     V
                
                 // Socket (f)
                
                 //     |
                 //     V
                
                 //  SERVER


                input_move.ki.wVk = 0;
                input_move.ki.wScan = f[0];
                input_move.ki.dwFlags = KeyboardFlag.Unicode;
                input_move.ki.dwExtraInfo = IntPtr.Zero;

                //STUFF
                Console.WriteLine("input_move.ki.wScan = " + input_move.ki.wScan);
                Console.WriteLine("MapVirtualKey((uint)key, 0) = " + MapVirtualKey((uint)key, 0));
                Console.WriteLine("key = " + key);
                Console.WriteLine(s);

                string json = JsonConvert.SerializeObject(input_move);
                byte[] toSend = Encoding.Unicode.GetBytes(json);
                ccm.Send(toSend, socket);
                ccm.Receive(new byte[5], socket);

            }
            else if (true/*( key != Keys.LControlKey && key != Keys.RControlKey) || (key != Keys.LShiftKey && key != Keys.RShiftKey)*/)
            {
                input_move.ki.wVk = (VirtualKeyCode)key;
                input_move.ki.wScan = 0;
                input_move.ki.dwFlags = 0;
                input_move.ki.dwExtraInfo = IntPtr.Zero;
                string json = JsonConvert.SerializeObject(input_move);
                byte[] toSend = Encoding.Unicode.GetBytes(json);
                ccm.Send(toSend, socket);
                ccm.Receive(new byte[5], socket);
                Console.WriteLine("inviato down");

            }

        }
        
        if (key == Keys.LWin || key == Keys.RWin)
            return (IntPtr)1;

        
        if (nCode >= 0 && wParam == (IntPtr)WM_KEYUP)
        {
            key = (Keys)Marshal.ReadInt32(lParam);
            //keyState
            byte[] ks = new byte[256];
            for (int x = 0; x < 256; x++)
                ks[x] = (byte)GetKeyState(x);


            if (/*(key == Keys.LControlKey || key == Keys.RControlKey) || (key == Keys.LShiftKey || key == Keys.RShiftKey)*/true)
            {

                INPUT input_move = new INPUT();
                input_move.type = 1;
                input_move.ki = new KEYBDINPUT();
                input_move.ki.wVk = (VirtualKeyCode)key;
                input_move.ki.dwFlags = KeyboardFlag.KeyUp;
                string json = JsonConvert.SerializeObject(input_move);
                byte[] toSend = Encoding.Unicode.GetBytes(json);
                ccm.Send(toSend, socket);
                ccm.Receive(new byte[5], socket);
                Console.WriteLine("inviato up");
            }
        }
        return CallNextHookEx(hookID, nCode, wParam, lParam);
    }
    
    [DllImport("user32.dll")]
    private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);
    [DllImport("user32.dll")]
    private static extern bool UnhookWindowsHookEx(IntPtr hhk);
    [DllImport("user32.dll")]
    private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);
    [DllImport("user32.dll")]
    private static extern uint MapVirtualKey(uint uCode, uint uMapType);
    [DllImport("user32.dll")]
    private static extern int ToAscii(uint uVirtKey, uint uScanCode, byte[] lpKeyState, StringBuilder lpChar, uint flags);
    [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
    internal static extern short GetKeyState(int virtualKeyCode);

    [DllImport("user32.dll", EntryPoint = "SendInput", SetLastError = true)]
    public static extern UInt32 SendInput(uint numberOfInputs, INPUT[] inputs, int sizeOfInputStructure);
    }