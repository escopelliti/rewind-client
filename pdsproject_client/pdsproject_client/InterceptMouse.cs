//using System;
//using System.Diagnostics;
//using System.Windows.Forms;
//using System.Runtime.InteropServices;
//using System.Collections.Concurrent;
//using System.Threading.Tasks;
//using System.Threading;
//using System.Windows.Input;
////using Newtonsoft.Json;
////using pdsproject_client;

//class InterceptMouse
//{
//    private static LowLevelMouseProc _proc = HookCallback;
//    private static IntPtr _hookID = IntPtr.Zero;
//    private static ConcurrentDictionary<MouseMessages, Action> mouseDictionary;

//    private static LowLevelKeyboardProc _proc_ = HookCallback_;
//    private static IntPtr _hookID_ = IntPtr.Zero;
//    private static int counter = 0;

//    [STAThreadAttribute]
//    public static void Main()
//    {
//        mouseDictionary = new ConcurrentDictionary<MouseMessages, Action>();
//        mouseDictionary[MouseMessages.WM_LBUTTONDOWN] = () => ThreadPool.QueueUserWorkItem(new WaitCallback(ManageMouseClick), null); ;
//        _hookID = SetHook(_proc);
//        _hookID_ = SetHook(_proc_);
//        Application.Run();
//        UnhookWindowsHookEx(_hookID);
//        UnhookWindowsHookEx(_hookID_);

//    }

//    private static void ManageMouseClick(Object obj)
//    {
//        Console.WriteLine("CLICK");

//    }

//    private static IntPtr SetHook(LowLevelMouseProc proc)
//    {
//        using (Process curProcess = Process.GetCurrentProcess())
//        using (ProcessModule curModule = curProcess.MainModule)
//        {
//            return SetWindowsHookEx(WH_MOUSE_LL, proc,
//                GetModuleHandle(curModule.ModuleName), 0);
//        }
//    }

//    private static IntPtr SetHook(LowLevelKeyboardProc proc)
//    {
//        using (Process curProcess = Process.GetCurrentProcess())
//        using (ProcessModule curModule = curProcess.MainModule)
//        {
//            return SetWindowsHookEx(WH_KEYBOARD_LL, proc, GetModuleHandle(curModule.ModuleName), 0);
//        }
//    }

//    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
//    private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

//    private const int WH_KEYBOARD_LL = 13;
//    private const int WM_KEYDOWN = 0x0100;
//    private delegate IntPtr LowLevelMouseProc(int nCode, IntPtr wParam, IntPtr lParam);
//    private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);


//    //private static uint sendinput()
//    //{

//    //    UInt16 scanCode = 'a';

//    //    var down = new INPUT
//    //    {
//    //        type = (UInt32)InputType.Keyboard,
//    //        ki = new KEYBDINPUT
//    //                {
//    //                    wVk = 0,
//    //                    wScan = scanCode,
//    //                    dwFlags = (UInt32)KeyboardFlag.Unicode,
//    //                    time = 0,
//    //                    dwExtraInfo = IntPtr.Zero
//    //                }            
//    //    };

//    //    var up = new INPUT
//    //    {
//    //        type = (UInt32)InputType.Keyboard,
//    //        ki = new KEYBDINPUT
//    //                {
//    //                    wVk = 0,
//    //                    wScan = scanCode,
//    //                    dwFlags =
//    //                        (UInt32)(KeyboardFlag.KeyUp | KeyboardFlag.Unicode),
//    //                    time = 0,
//    //                    dwExtraInfo = IntPtr.Zero
//    //                }            
//    //    };

//    //    // Handle extended keys:
//    //    // If the scan code is preceded by a prefix byte that has the value 0xE0 (224),
//    //    // we need to include the KEYEVENTF_EXTENDEDKEY flag in the Flags property. 
//    //    if ((scanCode & 0xFF00) == 0xE000)
//    //    {
//    //        down.ki.dwFlags |= (UInt32)KeyboardFlag.ExtendedKey;
//    //        up.ki.dwFlags |= (UInt32)KeyboardFlag.ExtendedKey;
//    //    }

//    //    INPUT[] inputList = { down, up };
//    //    return SendInput(2, inputList, Marshal.SizeOf(down));
//    //}


//    private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
//    {

//        //
//        //if ((wParam & MK_CONTROL) != 1)
//        if (nCode >= 0 && MouseMessages.WM_LBUTTONDOWN == (MouseMessages)wParam /* wParam == (IntPtr)WM_KEYDOWN && Keys.LControlKey == (Keys)Marshal.ReadInt32(lParam)*/)
//        {
//            //mouseDictionary[(MouseMessages)wParam]();
//            //Console.WriteLine("\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\");
//            MSLLHOOKSTRUCT hookStruct = (MSLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(MSLLHOOKSTRUCT));
           
//            ////=============================================================================

//            //INCOLLA TUTTO UNICODE NON FUNZIONANTE 
//            INPUT input_move = new INPUT();
//            input_move.type = 1;
//            input_move.ki = new KEYBDINPUT();
//            input_move.ki.wVk = 0;
//            input_move.ki.wScan = (UInt16)VirtualKeyCode.LCONTROL;
//            input_move.ki.dwFlags = KeyboardFlag.Unicode;
//            input_move.ki.dwExtraInfo = IntPtr.Zero;
//            INPUT[] input = { input_move };
//            Console.WriteLine(SendInput(1, input, Marshal.SizeOf(input_move)));

//            INPUT input_move2 = new INPUT();
//            input_move2.type = 1;
//            input_move2.ki = new KEYBDINPUT();
//            input_move2.ki.wVk = 0;
//            input_move2.ki.wScan = 'v';
//            input_move2.ki.dwFlags = KeyboardFlag.Unicode;
//            input_move2.ki.dwExtraInfo = IntPtr.Zero;
//            input[0] = input_move2;
//            Console.WriteLine(SendInput(1, input, Marshal.SizeOf(input_move2)));

//            INPUT input_move3 = new INPUT();
//            input_move3.type = 1;
//            input_move3.ki = new KEYBDINPUT();
//            input_move3.ki.wVk = 0;
//            input_move3.ki.wScan = 'v';
//            input_move3.ki.dwFlags = KeyboardFlag.KeyUp | KeyboardFlag.Unicode;
//            input_move3.ki.dwExtraInfo = IntPtr.Zero;
//            input[0] = input_move3;
//            Console.WriteLine(SendInput(1, input, Marshal.SizeOf(input_move3)));

//            INPUT input_move4 = new INPUT();
//            input_move4.type = 1;
//            input_move4.ki = new KEYBDINPUT();
//            input_move4.ki.wVk = 0;
//            input_move4.ki.wScan = (UInt16)VirtualKeyCode.LCONTROL;
//            input_move4.ki.dwFlags = KeyboardFlag.KeyUp | KeyboardFlag.Unicode;
//            input_move4.ki.dwExtraInfo = IntPtr.Zero;
//            input[0] = input_move4;
//            Console.WriteLine(SendInput(1, input, Marshal.SizeOf(input_move4)));

//            ////=============================================================================
          
//            ////INCOLLA FUNZIONANTE 
//            //INPUT input_move = new INPUT();
//            //input_move.type = 1;
//            //input_move.ki = new KEYBDINPUT();
//            //input_move.ki.wVk = VirtualKeyCode.LCONTROL;
//            //input_move.ki.wScan = 0;
//            //input_move.ki.dwFlags = 0;
//            //input_move.ki.dwExtraInfo = IntPtr.Zero;
//            //INPUT[] input = { input_move };
//            //Console.WriteLine(SendInput(1, input, Marshal.SizeOf(input_move)));

//            //INPUT input_move2 = new INPUT();
//            //input_move2.type = 1;
//            //input_move2.ki = new KEYBDINPUT();
//            //input_move2.ki.wVk = VirtualKeyCode.VK_V;
//            //input_move2.ki.wScan = 0;
//            //input_move2.ki.dwFlags = 0;
//            //input_move2.ki.dwExtraInfo = IntPtr.Zero;
//            //input[0] = input_move2;
//            //Console.WriteLine(SendInput(1, input, Marshal.SizeOf(input_move2)));

//            //INPUT input_move3 = new INPUT();
//            //input_move3.type = 1;
//            //input_move3.ki = new KEYBDINPUT();
//            //input_move3.ki.wVk = VirtualKeyCode.VK_V;
//            //input_move3.ki.wScan = 0;
//            //input_move3.ki.dwFlags = KeyboardFlag.KeyUp;
//            //input_move3.ki.dwExtraInfo = IntPtr.Zero;
//            //input[0] = input_move3;
//            //Console.WriteLine(SendInput(1, input, Marshal.SizeOf(input_move3)));

//            //INPUT input_move4 = new INPUT();
//            //input_move4.type = 1;
//            //input_move4.ki = new KEYBDINPUT();
//            //input_move4.ki.wVk = VirtualKeyCode.LCONTROL;
//            //input_move4.ki.wScan = 0;
//            //input_move4.ki.dwFlags = KeyboardFlag.KeyUp;
//            //input_move4.ki.dwExtraInfo = IntPtr.Zero;
//            //input[0] = input_move4;
//            //Console.WriteLine(SendInput(1, input, Marshal.SizeOf(input_move4)));

//            //=============================================================================

//            ////UNICODE FUNZIONANTE
//            //INPUT input_move = new INPUT();
//            //input_move.type = 1;
//            //input_move.ki = new KEYBDINPUT();
//            //input_move.ki.wVk = 0;
//            //input_move.ki.wScan = '/';
//            //input_move.ki.dwFlags = KeyboardFlag.Unicode;
//            //input_move.ki.dwExtraInfo = IntPtr.Zero;
//            //INPUT[] input = { input_move };
//            //Console.WriteLine(SendInput(1, input, Marshal.SizeOf(input_move)));

//            //=============================================================================

//            //Console.WriteLine(hookStruct.pt.x + ", " + hookStruct.pt.y);
//        }
//        return CallNextHookEx(_hookID, nCode, wParam, lParam);

//    }

//    [StructLayout(LayoutKind.Sequential)]
//    private struct KBDLLHOOKSTRUCT
//    {
//        public Keys key;
//        public int scanCode;
//        public int flags;
//        public int time;
//        public IntPtr extra;
//    }
//    private const int VK_SHIFT = 0x10;
//    private const int VK_CONTROL = 0x11;

//    private static IntPtr HookCallback_(int nCode, IntPtr wParam, IntPtr lParam)
//    {

//        //GetKeyState(VK_CONTROL) < 0 && GetKeyState(VK_SHIFT) < 0 --> funziona ma serve il dllImport ed entra ogni volta che schiaccio anche solo CTRL + SHIFT
//        //(Keyboard.GetKeyStates(Key.LeftCtrl) & KeyStates.Down) > 0 --> funziona ma 
//        if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN /*&& GetKeyState(VK_CONTROL) < 0 && GetKeyState(VK_SHIFT) < 0 (Keyboard.GetKeyStates(Key.LeftShift) & KeyStates.Down) > 0*/)
//        {

//            //questa struttura funziona solo con tasti che hanno un codice tra 1 e 254
//            KBDLLHOOKSTRUCT objKeyInfo = (KBDLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(KBDLLHOOKSTRUCT));
//            if (objKeyInfo.key == Keys.RWin || objKeyInfo.key == Keys.LWin)
//            {

//                Console.WriteLine((int)Keys.LControlKey);
//                return (IntPtr)1;
//            }

//            //INPUT input = new INPUT();
//            //input.type = 1;
//            //input.ki = new KEYBDINPUT();
//            //input.ki.wScan = '\u0028';
//            //input.ki.time = 0;
//            //input.ki.dwExtraInfo = (IntPtr)0;
//            //input.ki.wVk = 0;
//            //input.ki.dwFlags = (KeyboardFlag.Unicode | KeyboardFlag.ExtendedKey);
//            //INPUT[] inputList = { input };

//            //string json = JsonConvert.SerializeObject(inputList);
//            //Console.WriteLine(json);

//            long vkCode = Marshal.ReadInt64(lParam);
//            Console.WriteLine((Keys)vkCode);
//            //if (counter == 1)
//            //{
//            //    SendInput(1, inputList, Marshal.SizeOf(input));
//            //}
//            //counter++;

//            //mouseDictionary[(MouseMessages)wParam]();
//            Console.WriteLine("\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\");
//            //MSLLHOOKSTRUCT hookStruct = (MSLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(MSLLHOOKSTRUCT));
//            //Console.WriteLine(hookStruct.flags);
//            //Console.WriteLine(hookStruct.pt.x + ", " + hookStruct.pt.y);
//        }
//        return CallNextHookEx(_hookID, nCode, wParam, lParam);
//    }

//    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
//    static extern short VkKeyScanEx(char ch, IntPtr dwhkl);

//    private const int WH_MOUSE_LL = 14;
//    private static IntPtr MK_CONTROL = new IntPtr(0x0008);
//    private enum MouseMessages
//    {
//        WM_LBUTTONDOWN = 0x0201,
//        WM_LBUTTONUP = 0x0202,
//        WM_MOUSEMOVE = 0x0200,
//        WM_MOUSEWHEEL = 0x020A,
//        WM_RBUTTONDOWN = 0x0204,
//        WM_RBUTTONUP = 0x0205
//    }

//    [DllImport("user32.dll", EntryPoint = "SendInput", SetLastError = true)]
//    public static extern UInt32 SendInput(uint numberOfInputs, INPUT[] inputs, int sizeOfInputStructure);

//    [StructLayout(LayoutKind.Sequential)]
//    private struct POINT
//    {
//        public int x;
//        public int y;
//    }

//    [StructLayout(LayoutKind.Sequential)]
//    private struct MSLLHOOKSTRUCT
//    {
//        public POINT pt;
//        public uint mouseData;
//        public uint flags;
//        public uint time;
//        public IntPtr dwExtraInfo;
//    }

//    [DllImport("user32")]
//    private static extern short GetKeyState(int vKey);

//    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
//    private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelMouseProc lpfn, IntPtr hMod, uint dwThreadId);

//    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
//    [return: MarshalAs(UnmanagedType.Bool)]
//    private static extern bool UnhookWindowsHookEx(IntPtr hhk);

//    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
//    private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode,
//        IntPtr wParam, IntPtr lParam);

//    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
//    private static extern IntPtr GetModuleHandle(string lpModuleName);

//    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
//    private static extern IntPtr GetMessageExtraInfo();
//}