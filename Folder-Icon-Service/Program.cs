using System.ServiceProcess;

namespace FolderIconService
{
    static class Program
    {
        static void Main()
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new IconService()
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}