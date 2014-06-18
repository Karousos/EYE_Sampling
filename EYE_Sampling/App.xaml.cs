using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Threading;

namespace EYE_Sampling
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        #region RunASingleInstance
        Semaphore sema;
        bool shouldRelease = false;

        protected override void OnStartup(StartupEventArgs e)
        {

            bool result = false; //Semaphore.TryOpenExisting("EYE_Sampling", out sema);

            if (result) // we have another instance running
            {
                App.Current.Shutdown();
            }
            else
            {
                try
                {
                    sema = new Semaphore(1, 1, "EYE_Sampling");
                }
                catch
                {
                    App.Current.Shutdown(); //
                }
            }

            if (!sema.WaitOne(0))
            {
                App.Current.Shutdown();
            }
            else
            {
                shouldRelease = true;
            }


            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            if (sema != null && shouldRelease)
            {
                sema.Release();
            }
        }
        #endregion

    }
}
