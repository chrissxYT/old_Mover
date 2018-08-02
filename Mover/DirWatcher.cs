using System.ComponentModel;
using System.IO;
using System.Threading;

namespace Mover
{
    class DirWatcher
    {
        public void StartWatching(string dir, int sleep_time, Reflector refl)
        {
            refl.threadManager.StartThread(() => {
                while (Directory.Exists(dir) || Directory.GetFiles(dir).Length > 0 || Directory.GetDirectories(dir).Length > 0)
                    Thread.Sleep(sleep_time);
                Util.Print("Done moving dir " + dir + ".", refl, System.ConsoleColor.Green);
                Directory.Delete(dir);
            }, "AutoMover-DirWatcher");
        }
    }
}