using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Threading;

namespace Mover
{
    class ListChecker
    {
        public static void CheckMovingDrives(Reflector refl)
        {
            refl.threadManager.StartThread(() =>
            {
                foreach (string dr in refl.movingDrives)
                {
                    bool not_moving_anymore = true;
                    foreach (string s in refl.movingFiles)
                    {
                        if (dr.Equals(s.Split('\\')[0] + "\\"))
                        {
                            not_moving_anymore = false;
                        }
                    }
                    if (not_moving_anymore)
                    {
                        refl.threadManager.StartThread(() => 
                        {
                            Thread.Sleep(300);
                            Util.Print("Done moving drive " + dr + ".", refl, ConsoleColor.Green);
                            refl.movingDrives.Remove(dr);
                            refl.movedDrives.Add(dr);
                        }, "AutoMover-Delayer");
                    }
                }
            }, "AutoMover-MovingDriveChecker");
        }

        public static void CheckMovedDrives(IEnumerable<DriveInfo> info, Reflector refl)
        {
            refl.threadManager.StartThread(() =>
            {
                foreach (string d in refl.movedDrives)
                {
                    bool plugged_in = true;
                    foreach (DriveInfo dr in info)
                    {
                        if (d == dr.RootDirectory.ToString())
                        {
                            plugged_in = false;
                        }
                    }
                    if (plugged_in)
                    {
                        BackgroundWorker ww = new BackgroundWorker();
                        Thread tt = new Thread(() =>
                        {
                            Thread.Sleep(300);
                            Util.Print("Drive " + d + " removed.", refl, ConsoleColor.Yellow);
                            refl.movedDrives.Remove(d);
                        });
                        tt.Name = "AutoMover-Delayer";
                        ww.RunWorkerCompleted += new RunWorkerCompletedEventHandler((object sender, RunWorkerCompletedEventArgs args) =>
                        {
                            refl.backThreads.Remove(tt);
                        });
                        tt.Start();
                        if(!refl.blockthreads)
                            refl.backThreads.Add(tt);
                    }
                }
            }, "AutoMover-MovedDriveChecker");
        }
    }
}