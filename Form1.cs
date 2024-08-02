using System.Diagnostics;
using System.Runtime.InteropServices;

namespace mouse_events;

public partial class Form1 : Form
{
    private static TextBox? txtOutput;

    internal static IntPtr SetHook(LowLevelMouseProc proc)
    {
        using Process curProcess = Process.GetCurrentProcess();

        if (curProcess?.MainModule is null)
        {
            throw new Exception("couldn't get process main module!");
        }

        using ProcessModule curModule = curProcess.MainModule;
        return SetWindowsHookEx(WH_MOUSE_LL, proc,
                GetModuleHandle(curModule.ModuleName), 0);
    }

    internal delegate IntPtr LowLevelMouseProc(int nCode, IntPtr wParam, IntPtr lParam);

    internal static IntPtr HookCallback(
        int nCode, IntPtr wParam, IntPtr lParam)
    {
        if (nCode >= 0 && lParam != IntPtr.Zero && MouseMessages.WM_MOUSEWHEEL == (MouseMessages)wParam)
        {
#pragma warning disable CS8605 // Unboxing a possibly null value.
            MSLLHOOKSTRUCT hookStruct = (MSLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(MSLLHOOKSTRUCT));
#pragma warning restore CS8605 // Unboxing a possibly null value.
            byte[] bytes = BitConverter.GetBytes(hookStruct.mouseData);
            short s = BitConverter.ToInt16(bytes, 2);

            txtOutput?.AppendText($"{DateTime.Now.ToString("HH:mm:ss.fff"),-15}{s,-4}{Environment.NewLine}");
        }
        return CallNextHookEx(Program._hookID, nCode, wParam, lParam);
    }

    private const int WH_MOUSE_LL = 14;

    private enum MouseMessages
    {
        WM_LBUTTONDOWN = 0x0201,
        WM_LBUTTONUP = 0x0202,
        WM_MOUSEMOVE = 0x0200,
        WM_MOUSEWHEEL = 0x020A,
        WM_RBUTTONDOWN = 0x0204,
        WM_RBUTTONUP = 0x0205
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

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr SetWindowsHookEx(int idHook,
        LowLevelMouseProc lpfn, IntPtr hMod, uint dwThreadId);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool UnhookWindowsHookEx(IntPtr hhk);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode,
        IntPtr wParam, IntPtr lParam);

    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr GetModuleHandle(string lpModuleName);

    public Form1()
    {
        InitializeComponent();

        txtOutput = this.textBox1;
    }
}
