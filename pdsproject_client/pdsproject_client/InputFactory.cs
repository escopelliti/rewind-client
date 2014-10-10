using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Collections;


namespace pdsproject_client
{
    static class InputFactory
    {
        private List<INPUT> inputList;

        public InputFactory()
        {
            inputList = new List<INPUT>();
        }

        public static bool IsExtendedKey(VirtualKeyCode keyCode)
        {
            if (keyCode == VirtualKeyCode.MENU ||
                keyCode == VirtualKeyCode.LMENU ||
                keyCode == VirtualKeyCode.RMENU ||
                keyCode == VirtualKeyCode.CONTROL ||
                keyCode == VirtualKeyCode.RCONTROL ||
                keyCode == VirtualKeyCode.INSERT ||
                keyCode == VirtualKeyCode.DELETE ||
                keyCode == VirtualKeyCode.HOME ||
                keyCode == VirtualKeyCode.END ||
                keyCode == VirtualKeyCode.PRIOR ||
                keyCode == VirtualKeyCode.NEXT ||
                keyCode == VirtualKeyCode.RIGHT ||
                keyCode == VirtualKeyCode.UP ||
                keyCode == VirtualKeyCode.LEFT ||
                keyCode == VirtualKeyCode.DOWN ||
                keyCode == VirtualKeyCode.NUMLOCK ||
                keyCode == VirtualKeyCode.CANCEL ||
                keyCode == VirtualKeyCode.SNAPSHOT ||
                keyCode == VirtualKeyCode.DIVIDE)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        // Adds a key down to the list and returns it.
        public List<INPUT> KeyDown(VirtualKeyCode keyCode)
        {
            INPUT input = new INPUT();
            input.type = 1;
            input.ki = new KEYBDINPUT();
            input.ki.wVk = keyCode;
            input.ki.wScan = 0;
            input.ki.dwFlags = IsExtendedKey(keyCode) ? KeyboardFlag.ExtendedKey : 0;
            input.ki.dwExtraInfo = IntPtr.Zero;
            inputList.Add(input);
            return inputList;
            }

        public List<INPUT> KeyUp(VirtualKeyCode keyCode)
        {
            INPUT input = new INPUT();
            input.type = 1;
            input.ki = new KEYBDINPUT();
            input.ki.wVk = keyCode;
            input.ki.wScan = 0;
            input.ki.dwFlags = IsExtendedKey(keyCode) ?  KeyboardFlag.KeyUp | KeyboardFlag.ExtendedKey : KeyboardFlag.KeyUp;
            input.ki.dwExtraInfo = IntPtr.Zero;
            inputList.Add(input);
            return inputList;
        }

        public List<INPUT> KeyPress(VirtualKeyCode keyCode)
        {
            inputList = KeyDown(keyCode);
            List<INPUT> inputlist2= KeyUp(keyCode);
            inputList.Union(inputlist2);
            return inputList;
        }

        
    }
}

