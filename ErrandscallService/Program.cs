using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace ErrandscallService
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {

#if DEBUG
            ErrandscallService bm = new ErrandscallService();
            bm.RunDebug();
            System.Threading.Thread.Sleep(System.Threading.Timeout.Infinite);
#else
            ServiceBase[] servicestorun;
            servicestorun = new ServiceBase[]
            {
                new ErrandscallService()
            };
            ServiceBase.Run(servicestorun);
#endif
        }
    }
}
