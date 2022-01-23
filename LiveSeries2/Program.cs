using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace LiveSeries2
{

    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            if (!SingleInstance.Start())
            {
                SingleInstance.ShowFirstInstance();
                return;
            }
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FormLiveSeries());

            SingleInstance.Stop();
        }
    }

    public static class WinApi
    {
        [DllImport("user32")]
        public static extern int RegisterWindowMessage(string message);

        public static int RegisterWindowMessage(string format, params object[] args)
        {
            string message = string.Format(format, args);
            return RegisterWindowMessage(message);
        }

        public const int HWND_BROADCAST = 0xffff;
        public const int SW_SHOWNORMAL = 1;

        [DllImport("user32")]
        public static extern bool PostMessage(IntPtr hwnd, int msg, IntPtr wparam, IntPtr lparam);

        [DllImport("user32.dll")]
        public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        public static void ShowToFront(IntPtr window)
        {
            ShowWindow(window, SW_SHOWNORMAL);
            SetForegroundWindow(window);
        }
    }

    static public class SingleInstance
    {
        public const string ProgramGuid = "98337c7a-3b11-40b1-9768-c9c53a768191";
        public static readonly int WM_SHOWFIRSTINSTANCE =
            WinApi.RegisterWindowMessage("WM_SHOWFIRSTINSTANCE|{0}", ProgramGuid);
        static Mutex mutex;
        static public bool Start()
        {
            string mutexName = string.Format(@"Local\{0}", ProgramGuid);

            // if you want your app to be limited to a single instance
            // across ALL SESSIONS (multiple users & terminal services), then use the following line instead:
            // string mutexName = String.Format(@"Global\{0}", ProgramInfo.AssemblyGuid);

            mutex = new Mutex(true, mutexName, out bool onlyInstance);
            return onlyInstance;
        }
        static public void ShowFirstInstance()
        {
            WinApi.PostMessage((IntPtr)WinApi.HWND_BROADCAST, WM_SHOWFIRSTINSTANCE, IntPtr.Zero, IntPtr.Zero);
        }
        static public void Stop()
        {
            mutex.ReleaseMutex();
        }
    }
}
