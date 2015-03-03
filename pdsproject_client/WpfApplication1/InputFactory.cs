using System.Windows.Input;
using System.Runtime.InteropServices;
using System;
using System.Text;
using System.Collections.Generic;
using System.Windows.Forms;


namespace KeyboardMouseController.NativeInput
{
    public class InputFactory
    {

        private Dictionary<MouseMessages, MouseFlag> mouseFlagDictionary;

        public InputFactory() 
        {
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
            GetKeyboardState(keyState);
            return keyState;
        }

        //Verifichiamo che il codice del carattere è tra quelli che possiamo inviare usando la codifica Unicode
        private Boolean IsUnicodeChar(Keys key)
        {
            if((key >= Keys.D0 && key <= Keys.Z) ||
                (key >= Keys.NumPad0 && key <= Keys.NumPad9 && Keyboard.GetKeyStates(Key.CapsLock) == KeyStates.Toggled) ||
                (key >= Keys.Oem1 && key <= Keys.Oem102))
            {
                    return true;
            }            
            else return false;
        }

        public List<INPUT> CreateKeyboardInput(IntPtr wParam, IntPtr lParam)
        {
            Keys key = (Keys)Marshal.ReadInt32(lParam);
            string s = string.Empty;
            List<INPUT> inputToSend = new List<INPUT>();
         
            if (wParam == (IntPtr)KeyboardMessages.WM_KEYDOWN)
            {
                
                if ((Keyboard.GetKeyStates(Key.LeftCtrl) & KeyStates.Down) > 0 ||
                    (Keyboard.GetKeyStates(Key.RightCtrl) & KeyStates.Down) > 0)
                {

                    inputToSend.Add(CreateKeyDownInput(Keys.LControlKey));

                    if ((Keyboard.GetKeyStates(Key.LeftShift) & KeyStates.Down) > 0 ||
                        (Keyboard.GetKeyStates(Key.RightShift) & KeyStates.Down) > 0)
                    {
                        inputToSend.Add(CreateKeyDownInput(Keys.LShiftKey));
                    }

                    if ((Keyboard.GetKeyStates(Key.LeftAlt) & KeyStates.Down) > 0 ||
                        (Keyboard.GetKeyStates(Key.RightAlt) & KeyStates.Down) > 0)
                    {
                        inputToSend.Add(CreateKeyDownInput(Keys.LMenu));
                    }

                    if (key != Keys.LControlKey && key != Keys.RControlKey)
                    {
                        inputToSend.Add(CreateKeyDownInput(key));
                    }

                    // send key up 
                    if (key != Keys.LControlKey && key != Keys.RControlKey)
                    {
                        inputToSend.Add(CreateKeyUpInput(key));
                    }

                    if ((Keyboard.GetKeyStates(Key.LeftAlt) & KeyStates.Down) > 0 ||
                        (Keyboard.GetKeyStates(Key.RightAlt) & KeyStates.Down) > 0) 
                    {
                        inputToSend.Add(CreateKeyUpInput(Keys.LMenu));
                    }

                    if ((Keyboard.GetKeyStates(Key.LeftShift) & KeyStates.Down) > 0 ||
                        (Keyboard.GetKeyStates(Key.RightShift) & KeyStates.Down) > 0)
                    {
                        inputToSend.Add(CreateKeyUpInput(Keys.LShiftKey));
                    }

                    inputToSend.Add(CreateKeyUpInput(Keys.LShiftKey));
                }

                // Send key down using unicode
                else
                {
                   
                    if (IsUnicodeChar(key))
                    {
                        StringBuilder sb = new StringBuilder(1);
                        byte[] ks = GetKeyState();
                        ToAscii((uint)key, MapVirtualKey((uint)key, 0), ks, sb, 0);
                        s = sb.ToString(0, 1);
                        char[] f = s.ToCharArray();

                        INPUT keyboard_input = new INPUT();
                        keyboard_input.type = TYPE.INPUT_KEYBOARD;
                        keyboard_input.ki = new KEYBDINPUT(); 
                        keyboard_input.ki.wVk = 0;
                        keyboard_input.ki.wScan = f[0];
                        keyboard_input.ki.dwFlags = KeyboardFlag.Unicode;
                        keyboard_input.ki.dwExtraInfo = IntPtr.Zero;
                        inputToSend.Add(keyboard_input);
                    }

                    else
                    {
                        // send Keydown using Virtualcode 
                        inputToSend.Add(CreateKeyDownInput(key));
                    }
                }
            }

            //send keyup
            if (wParam == (IntPtr)KeyboardMessages.WM_KEYUP)
            {
                inputToSend.Add(CreateKeyUpInput(key));
            }

            return inputToSend;
        }

        public INPUT CreateKeyDownInput(Keys key)
        {
            INPUT keyboard_input = new INPUT();
            keyboard_input.type = TYPE.INPUT_KEYBOARD;
            keyboard_input.ki = new KEYBDINPUT();
            keyboard_input.ki.wVk = (VirtualKeyCode)key;
            keyboard_input.ki.wScan = 0;
            keyboard_input.ki.dwFlags = 0;
            keyboard_input.ki.dwExtraInfo = IntPtr.Zero;
            return keyboard_input;
        }

        public INPUT CreateKeyUpInput(Keys key)
        {
            INPUT keyboard_input = new INPUT();
            keyboard_input.type = TYPE.INPUT_KEYBOARD;
            keyboard_input.ki = new KEYBDINPUT();
            keyboard_input.ki.wVk = (VirtualKeyCode)key;
            keyboard_input.ki.wScan = 0;
            keyboard_input.ki.dwFlags = KeyboardFlag.KeyUp;
            keyboard_input.ki.dwExtraInfo = IntPtr.Zero;
            return keyboard_input;
        }

        public INPUT CreateMouseInput(IntPtr wParam, IntPtr lParam)
        {
            INPUT mouse_input = new INPUT();
            mouse_input.type = TYPE.INPUT_MOUSE;
            mouse_input.mi = (MOUSEINPUT)Marshal.PtrToStructure(lParam, typeof(MOUSEINPUT));
            mouse_input.mi.time = 0;
            mouse_input.mi.dwFlags = mouseFlagDictionary[(MouseMessages)wParam];
            if ((MouseMessages)wParam == MouseMessages.WM_MOUSE_HORIZONTAL_WHEEL || 
                (MouseMessages)wParam == MouseMessages.WM_MOUSE_VERTICAL_WHEEL)
            {
                mouse_input.mi.mouseData = mouse_input.mi.mouseData >> 16;
            }
            return mouse_input;
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        internal static extern short GetKeyState(int virtualKeyCode);

        [DllImport("user32.dll")]
        private static extern int ToAscii(uint uVirtKey, uint uScanCode, byte[] lpKeyState, StringBuilder lpChar, uint flags);

        [DllImport("user32.dll")]
        private static extern uint MapVirtualKey(uint uCode, uint uMapType);

        [DllImport("user32")]
        private static extern int GetKeyboardState(byte[] pbKeyState);
    }

}