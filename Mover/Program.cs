using Mover.Properties;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace Mover
{
    class Program
    {
        //VERSION
        public static readonly ulong VersionNum = 099;
        public static readonly string VersionStr = "0.9.9";
        //VERSION

        //BETA INFO
        public static readonly ulong BetaVersionNum = 17;
        public static readonly string BetaVersionStr = "17";
        //BETA INFO

        private static Reflector refl;

        static void Main(string[] args)
        {
            refl = new Reflector();
            Util.Print("::AutoMover by chrissx (version " + VersionStr + ")::", refl, ConsoleColor.Cyan);
            if (!File.Exists(P.config_file))
            {
                File.WriteAllText(P.config_file, Resources.DEF_CONFIG);
                Util.Print("CREATED CONFIG FILE, PLEASE EDIT IT, THE LOCATION IS "+ P.config_file, refl, ConsoleColor.Red);
                Util.Print("PRESS ENTER TO CLOSE THIS", refl, ConsoleColor.Red);
                Console.Read();
                Environment.Exit(0);
            }
            refl.config = new ConfigParser(File.ReadAllLines(P.config_file), refl).Parse();
            if (refl.config.beta)
            {
                Util.Print("[BETA "+BetaVersionStr+"]", refl, ConsoleColor.Cyan);
            }
            DirFileChecker.StartCheck(refl);
            Thread.Sleep(50);
            InitDrives(Util.GetDrives());
            refl.threadManager.StartThread(() =>
            {
                try
                {
                    while (refl.run && !refl.stop)
                    {
                        Thread.Sleep(1000);

                        CheckDrives(Util.GetDrives());
                        ListChecker.CheckMovingDrives(refl);
                        ListChecker.CheckMovedDrives(Util.GetDrives(), refl);
                        if (refl.i >= 100)
                        {
                            Console.Clear();
                            refl.logger.SaveLog(refl.config.log_path, true);
                            refl.i = 0;
                        }
                        if (Process.GetCurrentProcess().WorkingSet64 > refl.config.maxRam)
                        {
                            StartStopManager.Restart(refl);
                        }
                    }
                }
                catch(Exception e)
                {
                    if (refl.config.debug)
                    {
                        Util.Print(Util.ExceptionString(e), refl);
                    }
                }
            }, "AutoMover-BackgroundChecker");
            Thread.Sleep(50);
            refl.updateManager.CheckUpdates(true);
            while (refl.run && !refl.stop)
            {
                string inpu = Console.ReadLine();
                refl.logger.AddLog("["+Util.CurrentTime()+"][IN] "+inpu);
                string[] splitted = inpu.Split(' ');
                string input = splitted[0];
                Util.ProcessCUI(input, refl);
            }
        }

        private static void InitDrives(IEnumerable<DriveInfo> driveInfo)
        {
            foreach (DriveInfo i in driveInfo)
            {
                refl.movedDrives.Add(i.RootDirectory.ToString());
            }
        }

        private static void CheckDrives(IEnumerable<DriveInfo> driveInfo)
        {
            refl.threadManager.StartThread(() =>
            {
                foreach (var dr in driveInfo)
                {
                    bool b;
                    b = !refl.movingDrives.Contains(dr.Name) && !refl.movedDrives.Contains(dr.Name);
                    if (b)
                    {
                        Util.Print("Now moving: " + dr.Name, refl, ConsoleColor.DarkGreen);
                        refl.movingDrives.Add(dr.Name);
                        Util.MoveDir(dr.RootDirectory, refl.config.dir_to_put, false, refl);
                    }
                }
            }, "AutoMover-DriveChecker");
        }
    }
}