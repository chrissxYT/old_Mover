using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Threading;

namespace Mover
{
    class Logger
    {

        public Logger(Reflector refl)
        {
            this.refl = refl;
        }

        private Reflector refl;

        private List<String> log = new List<String>();

        public void SaveLog(String dir, bool async)
        {
            refl.threadManager.StartThread(() =>
            {
                String targetFile = dir + "\\" + Util.BuildDate();
                string ext = ".log";
                while (File.Exists(targetFile + ext))
                    targetFile += "_";
                refl.blocklog = true;
                Util.WriteTextFile(targetFile + ext, log.ToArray());
                Util.Print("Cleared console and saved log to " + targetFile + ext, refl, ConsoleColor.Yellow);
                refl.blocklog = false;
                log.Clear();
            }, "AutoMover-LogSaver");
            if (!async)
                refl.threadManager.JoinThread("AutoMover-LogSaver");
        }

        public void AddLog(string s)
        {
            log.Add(s);
        }
    }
}