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


using System.Drawing;


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

    private static LowLevelMouseProc _proc_ = HookCallbackMouse;
    private static LowLevelKeyboardProc _proc = HookCallback;
    
    private static ClientCommunicationManager ccm;
    private static Socket socket;


    static void Main()
    {
        ccm = new ClientCommunicationManager();
        socket = ccm.CreateSocket(ProtocolType.Tcp);
        socket = ccm.Connect("INSIDEMYHEAD", 12001, socket);

        IntPtr hInstance = LoadLibrary("User32");
        _hookID_ = SetWindowsHookEx(WH_MOUSE_LL, _proc_, hInstance, 0);
        //_hookID_ = SetWindowsHookEx(WH_MOUSE_LL, HookCallbackMouse, hInstance.Zero, 0);
        //hookID = SetWindowsHookEx(WH_KEYBOARD_LL, HookCallback, IntPtr.Zero, 0);
        hookID = SetWindowsHookEx(WH_KEYBOARD_LL, _proc, IntPtr.Zero, 0);

        Application.Run();

        UnhookWindowsHookEx(_hookID_);
        UnhookWindowsHookEx(hookID);
    }

    private static IntPtr HookCallbackMouse(int nCode, IntPtr wParam, IntPtr lParam)
    {


        if (nCode >= 0)
        {

            INPUT input_move = new INPUT();
            input_move.type = 0;
            input_move.mi = new MOUSEINPUT();
            
            MSLLHOOKSTRUCT hookStruct = (MSLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(MSLLHOOKSTRUCT));
            //Point point = new Point(lParam.ToInt32());
            
            input_move.mi.dx = (UInt16)hookStruct.pt.x;
            input_move.mi.dy = (UInt16)hookStruct.pt.y;


            switch ((MouseMessages)wParam) { 
                case MouseMessages.WM_LBUTTONDOWN :
                    Console.WriteLine("WM_LBUTTONDOWN");
                    input_move.mi.dwFlags = MouseFlag.LeftDown;
                    break;

                case MouseMessages.WM_LBUTTONUP:
                    Console.WriteLine("WM_LBUTTONUP");
                    input_move.mi.dwFlags = MouseFlag.LeftUp;
                    break;

                case MouseMessages.WM_MOUSEMOVE:
                    Console.WriteLine(hookStruct.pt.x + " " + hookStruct.pt.y);
                    Console.WriteLine("WM_MOUSEMOVE");
                    Console.WriteLine(hookStruct.flags);
                    input_move.mi.dwFlags = MouseFlag.Move | MouseFlag.Absolute;
                    Console.WriteLine(input_move.mi.dwFlags);
                    break;

                case MouseMessages.WM_MOUSE_VERTICAL_WHEEL:
                    Console.WriteLine("WM_MOUSEWHEEL");
                    input_move.mi.dwFlags = MouseFlag.VerticalWheel;
                    input_move.mi.mouseData = hookStruct.mouseData;
                    break;

                case MouseMessages.WM_MOUSE_HORIZONTAL_WHEEL:
                    Console.WriteLine("WM_MOUSEWHEEL");
                    input_move.mi.dwFlags = MouseFlag.HorizontalWheel;
                    input_move.mi.mouseData = hookStruct.mouseData;
                    break;

                case MouseMessages.WM_RBUTTONDOWN:
                    Console.WriteLine("WM_RBUTTONDOWN");
                    input_move.mi.dwFlags = MouseFlag.RightDown;
                    break;

                case MouseMessages.WM_RBUTTONUP:
                    Console.WriteLine("WM_RBUTTONUP");
                    input_move.mi.dwFlags = MouseFlag.RightUp;
                    break;

                default:
                    // POSSIBILE ERRORE SE ENTRA NELLA SCELTA DI DEFAULT
                    Console.Write("Mouse Event not managed");
                    break;
            }


        

            Console.WriteLine(input_move.mi.dwFlags);
            string json = JsonConvert.SerializeObject(input_move);
            byte[] toSend = Encoding.Unicode.GetBytes(json);
            ccm.Send(toSend, socket);
            ccm.Receive(new byte[5], socket);
                        
        }

        return CallNextHookEx(_hookID_, nCode, wParam, lParam); 
    }


    private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
    
    {
        Keys key = 0;

        if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN)
        {

            key = (Keys)Marshal.ReadInt32(lParam);
            //keyState
            byte[] ks = new byte[256];
            for (int x = 0; x < 256; x++)
                ks[x] = (byte)GetKeyState(x);
            string s = "";

            string json;
            byte[] toSend;

            INPUT input_move = new INPUT();
            input_move.type = 1;
            input_move.ki = new KEYBDINPUT();

            // Send key down using unicode
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
                s = sb.ToString(0, 1);

                char[] f = s.ToCharArray();

                input_move.ki.wVk = 0;
                input_move.ki.wScan = f[0];
                input_move.ki.dwFlags = KeyboardFlag.Unicode;
                input_move.ki.dwExtraInfo = IntPtr.Zero;

                Console.WriteLine("input_move.ki.wScan = " + input_move.ki.wScan);
                Console.WriteLine("MapVirtualKey((uint)key, 0) = " + MapVirtualKey((uint)key, 0));
                Console.WriteLine("key = " + key);
                Console.WriteLine(s);

                json = JsonConvert.SerializeObject(input_move);
                toSend = Encoding.Unicode.GetBytes(json);
                ccm.Send(toSend, socket);
                ccm.Receive(new byte[5], socket);

            }

            // send Keydown using Virtualcode 
            else
            {
                input_move.ki.wVk = (VirtualKeyCode)key;
                input_move.ki.wScan = 0;
                input_move.ki.dwFlags = 0;
                input_move.ki.dwExtraInfo = IntPtr.Zero;

                json = JsonConvert.SerializeObject(input_move);
                toSend = Encoding.Unicode.GetBytes(json);
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
        
        return CallNextHookEx(hookID, nCode, wParam, lParam);
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct POINT
    {
        public int x;
        public int y;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct MSLLHOOKSTRUCT
    {
        public POINT pt;
        public uint mouseData;
        public uint flags;
        public uint time;
        public IntPtr dwExtraInfo;
    }

    [DllImport("user32.dll")]
    private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

    [DllImport("user32.dll")]
    private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelMouseProc lpfn, IntPtr hMod, uint dwThreadId);

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

    [DllImport("kernel32.dll")]
    static extern IntPtr LoadLibrary(string lpFileName);

    }