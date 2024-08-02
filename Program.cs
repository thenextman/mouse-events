using static mouse_events.Form1;

namespace mouse_events;

static class Program
{
    internal static IntPtr _hookID = IntPtr.Zero;

    private static LowLevelMouseProc _proc = HookCallback;

    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
        // To customize application configuration such as set high DPI settings or default font,
        // see https://aka.ms/applicationconfiguration.
        _hookID = SetHook(_proc);
        ApplicationConfiguration.Initialize();
        Application.Run(new Form1());
        UnhookWindowsHookEx(_hookID);
    }    
}