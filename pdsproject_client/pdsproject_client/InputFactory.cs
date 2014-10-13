using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Collections;


namespace pdsproject_client
{
    class InputFactory
    {
        private List<INPUT> inputList;

        public InputFactory()
        {
            inputList = new List<INPUT>();
        }

        public List<INPUT> clearList()
        {
            inputList.Clear();
            return inputList;
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
            input.ki.dwFlags = IsExtendedKey(keyCode) ? KeyboardFlag.KeyUp | KeyboardFlag.ExtendedKey : KeyboardFlag.KeyUp;
            input.ki.dwExtraInfo = IntPtr.Zero;
            inputList.Add(input);
            return inputList;
        }

        public List<INPUT> KeyPress(VirtualKeyCode keyCode)
        {
            KeyDown(keyCode);
            KeyUp(keyCode);
            return inputList;
        }

        public List<INPUT> AddCharacter(char character)
        {
            int scanCode = character;
            INPUT down = new INPUT();
            down.type = 1;
            down.ki = new KEYBDINPUT();
            down.ki.wVk = 0;
            down.ki.wScan = scanCode;
            down.ki.dwFlags = KeyboardFlag.Unicode;
            down.ki.time = 0;
            down.ki.dwExtraInfo = IntPtr.Zero;

            INPUT up = new INPUT();
            up.type = 1;
            up.ki = new KEYBDINPUT();
            up.ki.wVk = 0;
            up.ki.wScan = scanCode;
            up.ki.dwFlags = KeyboardFlag.KeyUp | KeyboardFlag.Unicode;
            up.ki.time = 0;
            up.ki.dwExtraInfo = IntPtr.Zero;

            // Handle extended keys:
            // If the scan code is preceded by a prefix byte that has the value 0xE0 (224),
            // we need to include the KEYEVENTF_EXTENDEDKEY flag in the Flags property. 
            if ((scanCode & 0xFF00) == 0xE000)
            {
                down.ki.dwFlags |= KeyboardFlag.ExtendedKey;
                up.ki.dwFlags |= KeyboardFlag.ExtendedKey;
            }

            inputList.Add(down);
            inputList.Add(up);
            return inputList;
        }

        // Adds all of the characters in the specified <see cref="IEnumerable{T}"/> of <see cref="char"/>.
        public List<INPUT> AddCharacters(IEnumerable<char> characters)
        {
            foreach (var character in characters)
            {
                AddCharacter (character);
            }
            return inputList;
        }

        // Adds the characters in the specified <see cref="string"/>.
        public List<INPUT> AddCharacters(string characters)
        {
            return AddCharacters(characters.ToCharArray());
        }
        // MOUSE INPUT

        // Moves the mouse relative to its current position.
        public List<INPUT> RelativeMouseMovement(int x, int y)
        {
            INPUT movement = new INPUT();
            movement.type = 0;
            movement.mi.dwFlags = MouseFlag.Move;
            movement.mi.dx = x;
            movement.mi.dy = y;
            inputList.Add(movement);
            return inputList;
        }

        // Move the mouse to an absolute position.
        public List<INPUT> AbsoluteMouseMovement(int absoluteX, int absoluteY)
        {
            INPUT movement = new INPUT();
            movement.type = 0;
            movement.mi.dwFlags = MouseFlag.Move | MouseFlag.Absolute;
            movement.mi.dx = absoluteX;
            movement.mi.dy = absoluteY;
            inputList.Add(movement);
            return inputList;
        }

        // Adds a mouse button down for the specified button.
        public List<INPUT> MouseButtonDown(MouseButton button)
        {
            INPUT buttonDown = new INPUT();
            buttonDown.type = 0;
            buttonDown.mi.dwFlags = ToMouseButtonFlag(button,true);
            inputList.Add(buttonDown);
            return inputList;
        }

        // Adds a mouse button down for the specified button.
        public List<INPUT> MouseXButtonDown(int xButtonId)
        {
            INPUT buttonDown = new INPUT();
            buttonDown.type = 0;
            buttonDown.mi.dwFlags = MouseFlag.XDown;
            buttonDown.mi.mouseData = xButtonId;
            inputList.Add(buttonDown);
            return inputList;
        }

        // Adds a mouse button up for the specified button.
        public List<INPUT> MouseButtonUp(MouseButton button)
        {
            INPUT buttonUp = new INPUT();
            buttonUp.type = 0;
            buttonUp.mi.dwFlags = ToMouseButtonFlag(button,false);
            inputList.Add(buttonUp);
            return inputList;
        }

        // Adds a mouse button up for the specified button.
        public List<INPUT> MouseXButtonUp(int xButtonId)
        {
            INPUT buttonUp = new INPUT();
            buttonUp.type = 0;
            buttonUp.mi.dwFlags = MouseFlag.XUp;
            buttonUp.mi.mouseData = xButtonId;
            inputList.Add(buttonUp);
            return inputList;
        }

        // Adds a single click of the specified button.
        public List<INPUT> MouseButtonClick(MouseButton button)
        {
            MouseButtonDown(button);
            MouseButtonUp(button);
            return inputList;
        }

        // Adds a single click of the specified button.
        public List<INPUT> MouseXButtonClick(int xButtonId)
        {
            MouseXButtonDown(xButtonId);
            MouseXButtonUp(xButtonId);
            return inputList;
        }

        // Adds a double click of the specified button.
        public List<INPUT> MouseButtonDoubleClick(MouseButton button)
        {
            MouseButtonClick(button);
            MouseButtonClick(button);
            return inputList;
        }

        // Adds a double click of the specified button.
        public List<INPUT> AddMouseXButtonDoubleClick(int xButtonId)
        {
            MouseXButtonClick(xButtonId);
            MouseXButtonClick(xButtonId);
            return inputList;
        }

        // Scroll the vertical mouse wheel by the specified amount.
        public List<INPUT> MouseVerticalWheelScroll(int scrollAmount)
        {
            INPUT scroll = new INPUT();
            scroll.type = 0;
            scroll.mi.dwFlags = MouseFlag.VerticalWheel;
            scroll.mi.mouseData = scrollAmount;
            inputList.Add(scroll);
            return inputList;

        }

        // Scroll the horizontal mouse wheel by the specified amount.
        public List<INPUT> MouseHorizontalWheelScroll(int scrollAmount)
        {
            INPUT scroll = new INPUT();
            scroll.type = 0;
            scroll.mi.dwFlags = MouseFlag.HorizontalWheel;
            scroll.mi.mouseData = scrollAmount;
            inputList.Add(scroll);
            return inputList;
        }

        private static MouseFlag ToMouseButtonFlag(MouseButton button, bool DownButton)
        {
            //UP Button
            if (DownButton == true)
            {
                switch (button)
                {
                    case MouseButton.LeftButton:
                        return MouseFlag.LeftDown;

                    case MouseButton.MiddleButton:
                        return MouseFlag.MiddleDown;

                    case MouseButton.RightButton:
                        return MouseFlag.RightDown;

                    default:
                        return MouseFlag.LeftDown;
                }
            }
            else
            {
                switch (button)
                {
                    case MouseButton.LeftButton:
                        return MouseFlag.LeftUp;

                    case MouseButton.MiddleButton:
                        return MouseFlag.MiddleUp;

                    case MouseButton.RightButton:
                        return MouseFlag.RightUp;

                    default:
                        return MouseFlag.LeftUp;
                }
            }
        }
  



        // Move the mouse to the absolute position on the virtual desktop.
        public List<INPUT> AbsoluteMouseMovementOnVirtualDesktop(int absoluteX, int absoluteY)
        {
            INPUT movement = new INPUT ();
            movement.type = 0 ;
            movement.mi.dwFlags = MouseFlag.Move | MouseFlag.Absolute | MouseFlag.VirtualDesk;
            movement.mi.dx = absoluteX;
            movement.mi.dy = absoluteY;
            inputList.Add(movement);
            return inputList;
            
        }
    }
}
       
      

        

