using System.Windows.Input;

namespace SilvaShell.App.Core;

public static class HotkeyManager
{
    public static bool IsCtrlNumber(KeyEventArgs e, out int index)
    {
        index = -1;
        if ((Keyboard.Modifiers & ModifierKeys.Control) == 0)
            return false;

        if (e.Key >= Key.D1 && e.Key <= Key.D9)
        {
            index = (int)e.Key - (int)Key.D1;
            return true;
        }

        if (e.Key >= Key.NumPad1 && e.Key <= Key.NumPad9)
        {
            index = (int)e.Key - (int)Key.NumPad1;
            return true;
        }

        return false;
    }
}
