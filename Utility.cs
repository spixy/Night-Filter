using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.Win32;

namespace Night_Filter
{
    public static class Utility
    {
        public enum VirtualKeyStates
        {
            VK_RETURN = 0x0D,
            VK_SHIFT = 0x10,
            VK_CONTROL = 0x11,
            VK_MENU = 0x12,
            VK_PAUSE = 0x13,
            VK_SPACE = 0x20,
            //
            D0 = 0x30,
            D1 = 0x31,
            D2 = 0x32,
            D3 = 0x33,
            D4 = 0x34,
            D5 = 0x35,
            D6 = 0x36,
            D7 = 0x37,
            D8 = 0x38,
            D9 = 0x39,
            //
            VK_NUMPAD0 = 0x60,
            VK_NUMPAD1 = 0x61,
            VK_NUMPAD2 = 0x62,
            VK_NUMPAD3 = 0x63,
            VK_NUMPAD4 = 0x64,
            VK_NUMPAD5 = 0x65,
            VK_NUMPAD6 = 0x66,
            VK_NUMPAD7 = 0x67,
            VK_NUMPAD8 = 0x68,
            VK_NUMPAD9 = 0x69,
            VK_PLUS = 0x6B,
            VK_MINUS = 0x6D,
            //
            VK_F1 = 0x70,
            VK_F2 = 0x71,
            VK_F3 = 0x72,
            VK_F4 = 0x73,
            VK_F5 = 0x74,
            VK_F6 = 0x75,
            VK_F7 = 0x76,
            VK_F8 = 0x77,
            VK_F9 = 0x78,
            VK_F10 = 0x79,
            VK_F11 = 0x7A,
            VK_F12 = 0x7B,
        }

        /// <summary>
        /// Retrieves the status of the specified virtual key
        /// </summary>
        [DllImport("user32.dll")]
        private static extern short GetKeyState(VirtualKeyStates nVirtKey);

        /// <summary>
        /// Retrieves information about the specified window
        /// </summary>
        [DllImport("user32.dll")]
        public static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        /// <summary>
        /// Changes an attribute of the specified window
        /// </summary>
        [DllImport("user32.dll")]
        public static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        /// <summary>
        /// Uncheck all items except "exclude"
        /// </summary>
        /// <param name="exclude">item to check</param>
        /// <param name="where">items collection</param>
        public static void Uncheck(ToolStripMenuItem exclude, ToolStripItemCollection where)
        {
            foreach (ToolStripMenuItem item in where)
            {
                item.Checked = (item == exclude);
            }
        }

        /// <summary>
        /// Check if program is set at system startup
        /// </summary>
        /// <param name="program">program name</param>
        /// <returns>true if registry key exists, else if not</returns>
        public static bool IsAtStartup(string program)
        {
            try
            {
                RegistryKey rk = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run\\");
                return (rk != null && rk.GetValue(program) != null);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Set program at system startup
        /// </summary>
        /// <param name="program">program name</param>
        /// <param name="value">true if set at startup, false if remove from startup</param>
        /// <returns>true if success, false if error</returns>
        public static bool SetAsStartup(string program, bool value)
        {
            try
            {
                RegistryKey rk = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

                if (rk == null)
                    return false;

                if (value)
                {
                    rk.SetValue(program, Application.ExecutablePath);
                }
                else
                {
                    rk.DeleteValue(program, false);
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Check whether key was pressed
        /// </summary>
        public static bool IsKeyPressed(VirtualKeyStates key)
        {
            switch (GetKeyState(key))
            {
                case 0: // Not pressed and not toggled on.
                case 1: // Not pressed, but toggled on
                    return false;

                default: // Pressed (and may be toggled on)
                    return true;
            }
        }

    }
}
