using System.Windows;
using Microsoft.Shell.App;
using System;
using System.Collections.Generic;

namespace PdFuse
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// Code provided by Arik Poznanski http://blogs.microsoft.co.il/arik/2010/05/28/wpf-single-instance-application/
    /// for single instance application
    /// </summary>
    public partial class App : Application, ISingleInstanceApp
    {
        private const string _uniqueString = "UniqueStringForPdFuse";

        [STAThread]
        public static void Main()
        {            
            if (SingleInstance<App>.InitializeAsFirstInstance(_uniqueString))
            {
                var application = new App();
                application.InitializeComponent();
                application.Run();

                SingleInstance<App>.Cleanup();
            }
        }

        public bool SignalExternalCommandLineArgs(IList<string> args)
        {
            return true;
        }
    }
}
