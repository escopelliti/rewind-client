using System;
using System.Diagnostics;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Input;
using Newtonsoft.Json;
using pdsproject_client;
using System.Text;

class InterceptMouse
{
    private static LowLevelMouseProc _proc = HookCallback;
    private static IntPtr _hookID = IntPtr.Zero;
    private static ConcurrentDictionary<MouseMessages, Action> mouseDictionary;
    private static LowLevelKeyboardProc _proc_ = HookCallback_;
    private static IntPtr _hookID_ = IntPtr.Zero;
    private static InputLanguage lang;

    [STAThreadAttribute]
    public static void Main()
    {
        lang = GetInputLanguageByName("it");
        SetKeyboardLayout(lang);
        mouseDictionary = new ConcurrentDictionary<MouseMessages, Action>();
        mouseDictionary[MouseMessages.WM_LBUTTONDOWN] = () => ThreadPool.QueueUserWorkItem(new WaitCallback(ManageMouseClick), null); ;
        _hookID = SetHook(_proc);
        _hookID_ = SetHook(_proc_);
        Application.Run();
        UnhookWindowsHookEx(_hookID);
        UnhookWindowsHookEx(_hookID_);

    }

    private static void ManageMouseClick(Object obj)
    {
        Console.WriteLine("CLICK");

    }

    private static IntPtr SetHook(LowLevelMouseProc proc)
    {
        using (Process curProcess = Process.GetCurrentProcess())
        using (ProcessModule curModule = curProcess.MainModule)
        {
            return SetWindowsHookEx(WH_MOUSE_LL, proc,
                GetModuleHandle(curModule.ModuleName), 0);
        }
    }

    private static IntPtr SetHook(LowLevelKeyboardProc proc)
    {
        using (Process curProcess = Process.GetCurrentProcess())
        using (ProcessModule curModule = curProcess.MainModule)
        {
            return SetWindowsHookEx(WH_KEYBOARD_LL, proc, GetModuleHandle(curModule.ModuleName), 0);
        }
    }

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

    private const int WH_KEYBOARD_LL = 13;
    private const int WM_KEYDOWN = 0x0100;
    private delegate IntPtr LowLevelMouseProc(int nCode, IntPtr wParam, IntPtr lParam);
    private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);


    //private static uint sendinput()
    //{

    //    UInt16 scanCode = 'a';

    //    var down = new INPUT
    //    {
    //        type = (UInt32)InputType.Keyboard,
    //        ki = new KEYBDINPUT
    //                {
    //                    wVk = 0,
    //                    wScan = scanCode,
    //                    dwFlags = (UInt32)KeyboardFlag.Unicode,
    //                    time = 0,
    //                    dwExtraInfo = IntPtr.Zero
    //                }            
    //    };

    //    var up = new INPUT
    //    {
    //        type = (UInt32)InputType.Keyboard,
    //        ki = new KEYBDINPUT
    //                {
    //                    wVk = 0,
    //                    wScan = scanCode,
    //                    dwFlags =
    //                        (UInt32)(KeyboardFlag.KeyUp | KeyboardFlag.Unicode),
    //                    time = 0,
    //                    dwExtraInfo = IntPtr.Zero
    //                }            
    //    };

    //    // Handle extended keys:
    //    // If the scan code is preceded by a prefix byte that has the value 0xE0 (224),
    //    // we need to include the KEYEVENTF_EXTENDEDKEY flag in the Flags property. 
    //    if ((scanCode & 0xFF00) == 0xE000)
    //    {
    //        down.ki.dwFlags |= (UInt32)KeyboardFlag.ExtendedKey;
    //        up.ki.dwFlags |= (UInt32)KeyboardFlag.ExtendedKey;
    //    }

    //    INPUT[] inputList = { down, up };
    //    return SendInput(2, inputList, Marshal.SizeOf(down));
    //}

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    internal static extern int MapVirtualKey(int uCode, int uMapType);

    private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
    {

        //
        //if ((wParam & MK_CONTROL) != 1)
        if (nCode >= 0 && MouseMessages.WM_LBUTTONDOWN == (MouseMessages)wParam /* wParam == (IntPtr)WM_KEYDOWN && Keys.LControlKey == (Keys)Marshal.ReadInt32(lParam)*/)
        {
            //mouseDictionary[(MouseMessages)wParam]();
            //Console.WriteLine("\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\");
            MSLLHOOKSTRUCT hookStruct = (MSLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(MSLLHOOKSTRUCT));







            //int scanCode = character;
            //INPUT down = new INPUT();
            //down.type = 1;
            //down.ki = new KEYBDINPUT();
            //down.ki.wVk = 0;
            //down.ki.wScan = scanCode;
            //down.ki.dwFlags = KeyboardFlag.Unicode;
            //down.ki.time = 0;
            //down.ki.dwExtraInfo = IntPtr.Zero;

            //int scancode = MapVirtualKey(0xA0, 0);

            //char a = 'a';
            //int scan = a;
            INPUT input = new INPUT();
            input.type = 1;
            input.ki = new KEYBDINPUT();
            input.ki.wScan = 0;
            input.ki.time = 0;
            input.ki.dwExtraInfo = (IntPtr)0;
            input.ki.wVk = VirtualKeyCode.LCONTROL;
            input.ki.dwFlags = 0;

            //scancode = MapVirtualKey(0x32, 0);

            //INPUT input2 = new INPUT();
            //input.type = 1;
            //input.ki = new KEYBDINPUT();
            //input.ki.wScan = 0;
            //input.ki.time = 0;
            //input.ki.dwExtraInfo = (IntPtr)0;
            //input.ki.wVk = VirtualKeyCode.VK_2;
            //input.ki.dwFlags = 0;


            //a = '^';
            //INPUT up = new INPUT();
            //up.type = 1; //INPUT_KEYBOARD
            //up.ki = new KEYBDINPUT();
            //up.ki.wVk = VirtualKeyCode.LSHIFT;
            //up.ki.wScan = 0;
            //up.ki.time = 0;
            //up.ki.dwFlags = KeyboardFlag.KeyUp;  //KEYEVENTF_UNICODE
            //up.ki.dwExtraInfo = (IntPtr)0;

            //INPUT[] inputList = { input, input2};
            //INPUT inputStruct = new INPUT();
            //inputStruct.type = 1;
            //inputStruct.ki.wVk = 0;
            //inputStruct.ki.wScan = 'a';
            //inputStruct.ki.time = 0;
            //var flags = 0x0004;
            //inputStruct.ki.dwFlags = flags;
            //inputStruct.ki.dwExtraInfo = GetMessageExtraInfo();

            INPUT[] ip = { input };
            //SendInput(1, ip, Marshal.SizeOf(input));


            //KEYBDINPUT kb= new KEYBDINPUT();

            //INPUT single = new INPUT();


            //kb.wScan = 0x00c5;
            //kb.dwFlags = KeyboardFlag.Unicode;
            //single.type = 1;
            //single.ki = kb;
            //INPUT[] input = { single };
            //uint num = SendInput(1,input,Marshal.SizeOf(single));

            //keybd_event(VK_NUMLOCK,
            //          0x45,
            //          KEYEVENTF_EXTENDEDKEY | 0,
            //          0);
            //uint num = SendInput(2, inputList, Marshal.SizeOf(input));
            //Console.WriteLine(Marshal.GetLastWin32Error());

            //Console.WriteLine(Keyboard.GetKeyStates(Key.LeftShift));

            ////inputList[0] = up;

            //SendInput(1, new INPUT[]{up}, Marshal.SizeOf(up));
            //Console.WriteLine(Keyboard.GetKeyStates(Key.LeftShift));
            //input.ki.dwFlags = KeyboardFlag.ScanCode | KeyboardFlag.KeyUp;
            //num = SendInput(1, inputList, Marshal.SizeOf(input));
            ////Console.WriteLine(sendinput());
            //Console.WriteLine(Marshal.GetLastWin32Error());

            //SendKeys.SendWait("воздушной");

            //Console.WriteLine(Keyboard.GetKeyStates(Key.A));

            //Console.WriteLine(hookStruct.pt.x + ", " + hookStruct.pt.y);
        }
        return CallNextHookEx(_hookID, nCode, wParam, lParam);

    }

    [StructLayout(LayoutKind.Sequential)]
    private struct KBDLLHOOKSTRUCT
    {
        public Keys key;
        public int scanCode;
        public int flags;
        public int time;
        public IntPtr extra;
    }
    private const int VK_SHIFT = 0x10;
    private const int VK_CONTROL = 0x11;

    private static InputLanguage GetInputLanguageByName(string inputName)
    {
        foreach (InputLanguage lang in InputLanguage.InstalledInputLanguages)
        {
            //Console.WriteLine(lang.LayoutName);
            if (lang.Culture.EnglishName.ToLower().StartsWith(inputName))
                return lang;
        }
        return null;
    }

    private static void SetKeyboardLayout(InputLanguage layout)
    {
        InputLanguage.CurrentInputLanguage = layout;
    } 


    private static IntPtr HookCallback_(int nCode, IntPtr wParam, IntPtr lParam)
    {
        SetKeyboardLayout(lang);
        //GetKeyState(VK_CONTROL) < 0 && GetKeyState(VK_SHIFT) < 0 --> funziona ma serve il dllImport ed entra ogni volta che schiaccio anche solo CTRL + SHIFT
        //(Keyboard.GetKeyStates(Key.LeftCtrl) & KeyStates.Down) > 0 --> funziona ma 
        if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN /*&& GetKeyState(VK_CONTROL) < 0 && GetKeyState(VK_SHIFT) < 0 (Keyboard.GetKeyStates(Key.LeftShift) & KeyStates.Down) > 0*/)
        {

            //questa struttura funziona solo con tasti che hanno un codice tra 1 e 254
            KBDLLHOOKSTRUCT objKeyInfo = (KBDLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(KBDLLHOOKSTRUCT));
            if (objKeyInfo.key == Keys.RWin || objKeyInfo.key == Keys.LWin)
            {

                Console.WriteLine((int)Keys.LControlKey);
                return (IntPtr)1;
            }



            

            //INPUT input = new INPUT();
            //input.type = 1;
            //input.ki = new KEYBDINPUT();
            //input.ki.wScan = '\u0028';
            //input.ki.time = 0;
            //input.ki.dwExtraInfo = (IntPtr)0;
            //input.ki.wVk = 0;
            //input.ki.dwFlags = (KeyboardFlag.Unicode | KeyboardFlag.ExtendedKey);
            //INPUT[] inputList = { input };

            //string json = JsonConvert.SerializeObject(inputList);
            //Console.WriteLine(json);

            int scan = MapVirtualKey( (int) objKeyInfo.key, 0);

            long vkCode = Marshal.ReadInt64(lParam);
            Console.WriteLine((Keys)vkCode + " " + scan);
            //if (counter == 1)
            //{
            //    SendInput(1, inputList, Marshal.SizeOf(input));
            //}
            //counter++;

            //mouseDictionary[(MouseMessages)wParam]();
            Console.WriteLine("\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\");
            //MSLLHOOKSTRUCT hookStruct = (MSLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(MSLLHOOKSTRUCT));
            //Console.WriteLine(hookStruct.flags);
            //Console.WriteLine(hookStruct.pt.x + ", " + hookStruct.pt.y);
        }
        return CallNextHookEx(_hookID, nCode, wParam, lParam);
    }

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    static extern short VkKeyScanEx(char ch, IntPtr dwhkl);

    private const int WH_MOUSE_LL = 14;
    private static IntPtr MK_CONTROL = new IntPtr(0x0008);
    private enum MouseMessages
    {
        WM_LBUTTONDOWN = 0x0201,
        WM_LBUTTONUP = 0x0202,
        WM_MOUSEMOVE = 0x0200,
        WM_MOUSEWHEEL = 0x020A,
        WM_RBUTTONDOWN = 0x0204,
        WM_RBUTTONUP = 0x0205
    }

    [DllImport("user32.dll", EntryPoint = "SendInput", SetLastError = true)]
    public static extern UInt32 SendInput(uint numberOfInputs, INPUT[] inputs, int sizeOfInputStructure);

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

    [DllImport("user32")]
    private static extern short GetKeyState(int vKey);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelMouseProc lpfn, IntPtr hMod, uint dwThreadId);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool UnhookWindowsHookEx(IntPtr hhk);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode,
        IntPtr wParam, IntPtr lParam);

    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr GetModuleHandle(string lpModuleName);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr GetMessageExtraInfo();
}