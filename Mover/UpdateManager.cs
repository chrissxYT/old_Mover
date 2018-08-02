using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;

namespace Mover
{
    class UpdateManager
    {

        public static readonly string downloadUrl = "http://chrissx.lima-city.de/AutoMover/MoverUpdater.dl";
        public static readonly string versionUrl = "http://chrissx.lima-city.de/AutoMover/version.dl";
        public static readonly string betaVersionUrl = "http://chrissx.lima-city.de/AutoMover/beta/version.dl";
        public static readonly string updaterFile = Environment.CurrentDirectory + "\\MoverUpdater.exe";
        private Reflector refl;

        public UpdateManager(Reflector refl)
        {
            this.refl = refl;
        }

        public void StartUpdateListener()
        {
            refl.threadManager.StartThread(() => {
                try
                {
                    while (refl.run && !refl.stop)
                    {
                        Thread.Sleep(1000 * 60);
                        CheckUpdates(false);
                    }
                }
                catch(Exception e)
                {
                    if (refl.config.debug)
                    {
                        Util.Print(Util.ExceptionString(e), refl, ConsoleColor.Red);
                    }
                }
            }, "AutoMover-UpdateChecker");
        }

        public void CheckUpdates(bool print)
        {
            try
            {
                WebClient client = new WebClient();
                if (print)
                    Util.Print("Searching for updates.", refl, ConsoleColor.Yellow);
                string tempVersionFile = Path.GetTempFileName();
                client.DownloadFile(versionUrl, tempVersionFile);
                string verRaw = File.ReadAllText(tempVersionFile);
                ulong serverVersion = ulong.Parse(verRaw);
                if (refl.config.beta)
                {
                    string tempBetaVersionFile = Path.GetTempFileName();
                    client.DownloadFile(betaVersionUrl, tempBetaVersionFile);
                    verRaw = File.ReadAllText(tempBetaVersionFile);
                    serverVersion = ulong.Parse(verRaw);
                }
                if (serverVersion > Program.VersionNum)
                {
                    Util.Print("An update is available, it will be installed now.", refl, ConsoleColor.Green);
                    UpdateDialog dialog = new UpdateDialog();
                    dialog.SetVersion(serverVersion);
                    dialog.SetCount(3);
                    Thread.Sleep(1000);
                    dialog.SetCount(2);
                    Thread.Sleep(1000);
                    dialog.SetCount(1);
                    Thread.Sleep(1000);
                    dialog.SetCount(0);
                    Update(dialog, true);
                }
                else if (Program.VersionNum > serverVersion)
                {
                    Util.Print("The server-version is older than the current, if you aren't chrissx, this isn't supposed to happen, please use \"update\" to be sure you're not running into an error.", refl, ConsoleColor.Yellow);
                }
                else
                {
                    Util.Print("Your version is up to date.", refl, ConsoleColor.DarkGreen);
                }
            }
            catch(Exception e)
            {
                Util.Print("Error while trying to check for updates, please check your internet connection!", refl, ConsoleColor.Red);
                if (refl.config.debug)
                {
                    Util.Print(Util.ExceptionString(e), refl);
                }
            }
        }

        public void Update(UpdateDialog dialog, bool dialogIsNotNull)
        {
            try
            {
                WebClient c = new WebClient();
                Util.Print("Updating updater.", refl, ConsoleColor.DarkGreen);
                c.DownloadFile(downloadUrl, updaterFile);
                Util.Print("Updated updater.", refl, ConsoleColor.Green);
                refl.run = false;
                refl.stop = true;
                refl.blockthreads = true;
                Util.Print("Joining threads in order to update.", refl, ConsoleColor.Gray);
                refl.threadManager.Destruct();
                if (refl.config.beta)
                {
                    File.WriteAllText(P.beta_file, "true");
                }
                if (refl.config.debug)
                {
                    File.WriteAllText(P.debug_file, "true");
                }
                Util.Print("Updating.", refl, ConsoleColor.DarkGreen);
                refl.logger.SaveLog(refl.config.log_path, false);
                if(dialogIsNotNull)
                    dialog.Close();
                Process.Start(updaterFile);
                Environment.Exit(0);
            }
            catch(Exception e)
            {
                Util.Print("Error while trying to update, please check your internet connection!", refl, ConsoleColor.Red);
                if (refl.config.debug)
                {
                    Util.Print(Util.ExceptionString(e), refl);
                }
            }
        }
    }
}
