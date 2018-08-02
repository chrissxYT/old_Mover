using System;
using System.Diagnostics;

namespace Mover
{
    class StartStopManager
    {
        public static void Restart(Reflector refl)
        {
            refl.stop = true;
            refl.run = false;
            refl.blockthreads = true;
            Util.Print("Joining threads in order to restart.", refl, ConsoleColor.Gray);
            refl.threadManager.Destruct();
            Util.Print("Restarting.", refl, ConsoleColor.DarkRed);
            refl.logger.SaveLog(refl.config.log_path, false);
            Process.Start(Environment.CurrentDirectory + "\\Mover.exe");
            Environment.Exit(0);
        }

        public static void Exit(Reflector refl)
        {
            refl.stop = true;
            refl.run = false;
            refl.blockthreads = true;
            Util.Print("Joining threads.", refl, ConsoleColor.Gray);
            refl.threadManager.Destruct();
            Util.Print("Shutting down.", refl, ConsoleColor.DarkRed);
            refl.logger.SaveLog(refl.config.log_path, false);
            Environment.Exit(0);
        }
    }
}
