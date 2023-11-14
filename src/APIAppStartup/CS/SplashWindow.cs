// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt
using System.Threading;
using System.Windows.Forms;

namespace APIAppStartup
{
    public delegate void DelegateCloseSplash();

    public partial class SplashWindow : Form
    {
        private static SplashWindow m_instance;
        private static Thread InstanceCaller;

        private readonly DelegateCloseSplash m_delegateClose;

        public SplashWindow()
        {
            InitializeComponent();
            m_delegateClose = InternalCloseSplash;
        }

        //start a new thread to display splash window
        public static void StartSplash()
        {
            m_instance = new SplashWindow();
            m_instance.TopMost = true;
            InstanceCaller = new Thread(MySplashThreadFunc);
            InstanceCaller.Start();
        }

        //kill the thread
        public static void StopSplash()
        {
            if (m_instance != null) m_instance.Invoke(m_instance.m_delegateClose);
        }

        //show Revit version info
        public static void ShowVersion(string version)
        {
            m_instance.Version.Text = version;
        }

        private void InternalCloseSplash()
        {
            Close();
            Dispose();
        }

        // this is called by the new thread to show the splash screen
        private static void MySplashThreadFunc()
        {
            if (m_instance != null)
            {
                m_instance.TopMost = true;
                m_instance.ShowDialog();
            }
        }
    }
}
