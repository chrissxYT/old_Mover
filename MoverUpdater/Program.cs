using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;
using System.IO.Compression;

namespace MoverUpdater
{
    class Program
    {
        private static readonly string ver = "0.2.0";

        private static WebClient client = new WebClient();
        private static string url = "http://chrissx.lima-city.de/AutoMover/update.dl";
        private static readonly string moverFile = Environment.CurrentDirectory + "\\Mover.exe";
        private static readonly string betaFile = Environment.CurrentDirectory + "\\MoverBeta.config";
        private static readonly string debugFile = Environment.CurrentDirectory + "\\MoverDebug.config";
        private static readonly string tempZip = Path.GetTempFileName();
        private static bool beta = false;
        private static bool debug = false;

        static void Main(string[] useless_af)
        {
            Console.WriteLine("::AutoMover-Updater by chrissx (version " + ver + ")::");
            Console.WriteLine("Waiting for AutoMover to close...");
            Thread.Sleep(200);
            Console.WriteLine("Loading config.");
            LoadConfig();
            Console.WriteLine("Clearing dir.");
            Clear();
            Console.WriteLine("Downloading update-zip.");
            Download();
            while (client.IsBusy)
            {
                Thread.Sleep(20);
            }
            Console.WriteLine("Done.");
            Console.WriteLine("Decompressing zip...");
            ZipFile.ExtractToDirectory(tempZip, Environment.CurrentDirectory);
            Console.WriteLine("Done.");
            Console.WriteLine("Switching back to AutoMover...");
            Process.Start(moverFile);
            Environment.Exit(0);
        }

        private static void Clear()
        {
            foreach (string f in Directory.EnumerateFiles(Environment.CurrentDirectory))
            {
                if (f.Split('/')[f.Split('/').Length - 1] != "MoverUpdater.exe")
                {
                    try
                    {
                        File.Delete(f);
                    }
                    catch (Exception e)
                    {
                        if (debug)
                        {
                            Console.WriteLine(e.Message + "\n" + e.StackTrace);
                        }
                    }
                }
            }
        }

        private static void Download()
        {
            client.DownloadProgressChanged += new DownloadProgressChangedEventHandler((object sender, DownloadProgressChangedEventArgs args) => { Console.WriteLine(args.ProgressPercentage + "% downloaded."); });
            client.DownloadFileAsync(new Uri(url), tempZip);
        }

        private static void LoadConfig()
        {
            try
            {
                if (File.Exists(debugFile))
                {
                    debug = true;
                }
                else
                {
                    debug = false;
                }
            }
            catch
            {
                Console.WriteLine("Error reading debug-config, set it to true.");
                debug = true;
            }

            try
            {
                if (File.Exists(betaFile))
                {
                    beta = true;
                }
                else
                {
                    beta = false;
                }
            }
            catch
            {
                beta = false;
            }

            if (beta)
            {
                url = "http://chrissx.lima-city.de/AutoMover/beta/update.dl";
            }
            try
            {
                File.Delete(betaFile);
                File.Delete(debugFile);
            }catch{}
        }
    }
}