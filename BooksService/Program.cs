using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using BooksService.Data;

namespace BooksService
{
    static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        static void Main()
        {
            string[] args = Environment.GetCommandLineArgs();
            if ((args.Length == 2) && (args[1] == "c"))
            {
                Console.WriteLine("Start console");
                Console.WriteLine("Press ENTER to exit...");
                WCFService s = new WCFService();
                s.Start();
                Console.Read();
                s.Stop();
            }
            else
            {
                ServiceBase[] ServicesToRun;
                ServicesToRun = new ServiceBase[]
                {
                new BooksService()
                };
                ServiceBase.Run(ServicesToRun);
            }

            DBAccess.Close();
        }
    }
}
