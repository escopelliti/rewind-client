﻿//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using System.Collections;

using System.Windows.Input;
using System.Runtime.InteropServices;
using System;
using System.Text;
using System.Collections.Generic;




namespace NativeInput
{
    class InputFactory
    {

        private Dictionary<MouseMessages, MouseFlag> mouseFlagDictionary;

        public InputFactory() {
            InitilizeDictionary();
        }
        
        private void InitilizeDictionary()
        {
            mouseFlagDictionary = new Dictionary<MouseMessages,MouseFlag>();
            mouseFlagDictionary[MouseMessages.WM_LBUTTONDOWN] = MouseFlag.LeftDown;
            mouseFlagDictionary[MouseMessages.WM_LBUTTONUP] = MouseFlag.LeftUp;
            mouseFlagDictionary[MouseMessages.WM_MOUSEMOVE] = MouseFlag.Move | MouseFlag.Absolute;
            mouseFlagDictionary[MouseMessages.WM_MOUSE_VERTICAL_WHEEL] = MouseFlag.VerticalWheel;
            mouseFlagDictionary[MouseMessages.WM_MOUSE_HORIZONTAL_WHEEL] = MouseFlag.HorizontalWheel;
            mouseFlagDictionary[MouseMessages.WM_RBUTTONDOWN] = MouseFlag.RightDown;
            mouseFlagDictionary[MouseMessages.WM_RBUTTONUP] = MouseFlag.RightUp;
            mouseFlagDictionary[MouseMessages.WM_MBUTTONDOWN] = MouseFlag.MiddleDown;
            mouseFlagDictionary[MouseMessages.WM_MBUTTONUP] = MouseFlag.MiddleUp;        
        }

        private byte[] GetKeyState()
        {
            byte[] keyState = new byte[256];
            for (int x = 0; x < 256; x++)
                keyState[x] = (byte)GetKeyState(x);
            return keyState;
        }

        //Verifichiamo che il codice del carattere è tra quelli che possiamo inviare usando la codifica Unicode
        private Boolean IsUnicodeChar(Key key)
        {
            if((key >= Key.D0 && key <= Key.Z) ||
                (key >= Key.NumPad0 && key <= Key.NumPad9 && Keyboard.GetKeyStates(Key.CapsLock) == KeyStates.Toggled) ||
                (key >= Key.Oem1 && key <= Key.Oem102))
            {
                    return true;
            }
            
            else return false;
        }

        public INPUT CreateKeyboardInput(IntPtr wParam, IntPtr lParam)
        {
            Key key = (Key)Marshal.ReadInt32(lParam);
            string s = "";

            INPUT keyboard_input = new INPUT();
            keyboard_input.type = TYPE.INPUT_KEYBOARD;
            keyboard_input.ki = new KEYBDINPUT();

            if (wParam == (IntPtr)KeyboardMessages.WM_KEYDOWN)
            {
                // Send key down using unicode
                if (IsUnicodeChar(key))
                {
                    StringBuilder sb = new StringBuilder(1);
                    byte[] ks = GetKeyState();
                    ToAscii((uint)key, MapVirtualKey((uint)key, 0), ks, sb, 0);
                    s = sb.ToString(0, 1);
                    char[] f = s.ToCharArray();

                    keyboard_input.ki.wVk = 0;
                    keyboard_input.ki.wScan = f[0];
                    keyboard_input.ki.dwFlags = KeyboardFlag.Unicode;
                    keyboard_input.ki.dwExtraInfo = IntPtr.Zero;
                }

                // send Keydown using Virtualcode 
                else
                {
                    keyboard_input.ki.wVk = (VirtualKeyCode)key;
                    keyboard_input.ki.wScan = 0;
                    keyboard_input.ki.dwFlags = 0;
                    keyboard_input.ki.dwExtraInfo = IntPtr.Zero;
                }
            }

            else
            {
                keyboard_input.ki.wVk = (VirtualKeyCode)key;
                keyboard_input.ki.dwFlags = KeyboardFlag.KeyUp;
            }

            return keyboard_input;
        }

        public INPUT CreateMouseInput(IntPtr wParam, IntPtr lParam)
        {
            INPUT mouse_input = new INPUT();
            mouse_input.type = TYPE.INPUT_MOUSE;
            mouse_input.mi = (MOUSEINPUT)Marshal.PtrToStructure(lParam, typeof(MOUSEINPUT));
            //Coordinate prese direttamente con PtrToStruct()
            mouse_input.mi.dwFlags = mouseFlagDictionary[(MouseMessages)wParam];
            if ((MouseMessages)wParam == MouseMessages.WM_MOUSE_HORIZONTAL_WHEEL || 
                (MouseMessages)wParam == MouseMessages.WM_MOUSE_VERTICAL_WHEEL)
            {
                //Quantità del movimento della rotellina fisso
                mouse_input.mi.mouseData = 10;
            }
            return mouse_input;

        }
        

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        internal static extern short GetKeyState(int virtualKeyCode);

        [DllImport("user32.dll")]
        private static extern int ToAscii(uint uVirtKey, uint uScanCode, byte[] lpKeyState, StringBuilder lpChar, uint flags);

        [DllImport("user32.dll")]
        private static extern uint MapVirtualKey(uint uCode, uint uMapType);
    }
    
}