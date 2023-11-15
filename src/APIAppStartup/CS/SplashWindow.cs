// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt
using System.Threading;
using System.Windows.Forms;

namespace APIAppStartup
{
    public delegate void DelegateCloseSplash();

    public partial class SplashWindow : Form
    {
        private static SplashWindow _instance;
        private static Thread _instanceCaller;

        private readonly DelegateCloseSplash m_delegateClose;

        public SplashWindow()
        {
            InitializeComponent();
            m_delegateClose = InternalCloseSplash;
        }

        //start a new thread to display splash window
        public static void StartSplash()
        {
            _instance = new SplashWindow();
            _instance.TopMost = true;
            _instanceCaller = new Thread(MySplashThreadFunc);
            _instanceCaller.Start();
        }

        //kill the thread
        public static void StopSplash()
        {
            _instance?.Invoke(_instance.m_delegateClose);
        }

        //show Revit version info
        public static void ShowVersion(string version)
        {
            _instance.Version.Text = version;
        }

        private void InternalCloseSplash()
        {
            Close();
            Dispose();
        }

        // this is called by the new thread to show the splash screen
        private static void MySplashThreadFunc()
        {
            if (_instance != null)
            {
                _instance.TopMost = true;
                _instance.ShowDialog();
            }
        }
    }
}
