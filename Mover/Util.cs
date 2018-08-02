using Mover.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Mover
{
    class Util
    {
        public static string BuildDate()
        {
            string Out = DateTime.Now.Year.ToString();
            if (DateTime.Now.Month < 10)
            {
                Out += "0";
            }
            Out += DateTime.Now.Month.ToString();
            if (DateTime.Now.Day < 10)
            {
                Out += "0";
            }
            Out += DateTime.Now.Day.ToString() + "_";
            if (DateTime.Now.Hour < 10)
            {
                Out += "0";
            }
            Out += DateTime.Now.Hour.ToString();
            if (DateTime.Now.Minute < 10)
            {
                Out += "0";
            }
            Out += DateTime.Now.Minute.ToString();
            if (DateTime.Now.Second < 10)
            {
                Out += "0";
            }
            Out += DateTime.Now.Second.ToString();
            return Out;
        }

        public static string CurrentTime()
        {
            string Out = "";
            if (DateTime.Now.Hour < 10)
            {
                Out += "0";
            }
            Out += DateTime.Now.Hour + ":";
            if (DateTime.Now.Minute < 10)
            {
                Out += "0";
            }
            Out += DateTime.Now.Minute + ":";
            if (DateTime.Now.Second < 10)
            {
                Out += "0";
            }
            Out += DateTime.Now.Second;
            return Out;
        }

        public static IEnumerable<DriveInfo> GetDrives()
        {
            return DriveInfo.GetDrives().Where(drive => drive.IsReady);
        }
		
		public static void ProcessCUI(string input, Reflector refl)
		{
			if(input == "stop")
            {
                StartStopManager.Exit(refl);
            }
            else if(input == "exit")
            {
                StartStopManager.Exit(refl);
            }
            else if(input == "info")
            {
                Print("by chrissx, created in December of 2017", refl);
            }
            else if(input == "moving")
            {
                Print("Currently moving files:", refl, ConsoleColor.Gray);
                foreach(string s in refl.movingFiles)
                {
                    Print(s, refl);
                }
            }
            else if(input == "help")
            {
                Print("Currently available commands: stop, exit, info, moving, help, files, update, config-regen", refl);
            }
            else if(input == "files")
            {
                Print("Currently moving files:", refl, ConsoleColor.Gray);
				foreach (string s in refl.movingFiles)
				{
					Print(s, refl);
				}
            }
            else if(input == "update")
            {
                refl.updateManager.Update(null, false);
            }
            else if(input == "config-regen")
            {
                Print("Regenerating config.", refl, ConsoleColor.Yellow);
                refl.stop = true;
                refl.run = false;
                refl.blockthreads = true;
                Print("Joining threads in order to stop for config.", refl, ConsoleColor.Gray);
                refl.threadManager.Destruct();
                Print("Stopping for config regen.", refl, ConsoleColor.DarkRed);
                File.Delete(P.config_file);
                File.WriteAllText(P.config_file, Resources.DEF_CONFIG);
                refl.logger.SaveLog(refl.config.log_path, false);
                Environment.Exit(0);
            }
            else
            {
                Print("Cannot recognize that command. Enter \"help\" for help.", refl, ConsoleColor.DarkRed);
            }
		}

        public static string ExceptionString(Exception e)
        {
            return e.ToString() + " " + e.Message + "\n" + e.StackTrace;
        }
		
		public static void Print(String s, Reflector refl, ConsoleColor color)
        {
            refl.threadManager.StartThread(() =>
            {
                while (refl.blockconsole)
                {
                    Thread.Sleep(20);
                }
                refl.blockconsole = true;
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("[" + CurrentTime() + "] ");
                Console.ForegroundColor = color;
                Console.WriteLine(s);
                Console.ForegroundColor = ConsoleColor.White;
                refl.blockconsole = false;
                while (refl.blocklog)
                {
                    Thread.Sleep(20);
                }
                refl.logger.AddLog("[" + CurrentTime() + "][OUT]["+color+"] " + s);
                refl.i++;
            }, "AutoMover-ConsolePrinter");
        }

        public static void Print(string s, Reflector refl)
        {
            Print(s, refl, ConsoleColor.White);
        }

        public static void MoveDir(DirectoryInfo info, string tDir, bool slash, Reflector refl)
        {
            refl.threadManager.StartThread(() =>
            {
                //FILES
                foreach (FileInfo f in info.GetFiles())
                {
                    string sF = f.FullName;
                    string tF;
                    if (slash)
                    {
                        tF = tDir + "\\" + f.Name;
                    }
                    else
                    {
                        tF = tDir + f.Name;
                    }
                    while (refl.blockmove)
                    {
                        Thread.Sleep(50);
                    }
                    refl.blockmove = true;
                    int i = 2;
                    while (File.Exists(tF))
                    {
                        string[] split = tF.Split('.');
                        if (split.Length == 1)
                        {
                            tF = tF + i;
                        }
                        else
                        {
                            string splitted = split[0];
                            for (int a = 1; a < split.Length - 1; a++)
                            {
                                splitted += "." + split[a];
                            }
                            string ext = split[split.Length - 1];
                            tF = splitted + i + "." + ext;
                        }
                        i++;
                    }
                    bool move = true;
                    refl.blockmoveadd = true;
                    move = !refl.movingFiles.Contains(sF);
                    refl.blockmoveadd = false;
                    if (move)
                    {
                        MoveHelper helper = new MoveHelper(refl, refl.currentX, refl.currentY);
                        refl.IncreasePos();
                        helper.Move(sF, tF, refl.config.cache_size);
                        while (refl.blockmoveadd)
                        {
                            Thread.Sleep(50);
                        }
                        refl.movingFiles.Add(sF);
                    }
                    refl.blockmove = false;
                }

                //DIRS
                foreach (DirectoryInfo f in info.GetDirectories())
                {
                    if (!f.Name.Equals("System Volume Information"))
                    {
                        Print("Now moving: " + f.FullName, refl, ConsoleColor.DarkGreen);
                        if (!Directory.Exists(tDir + f.Name))
                        {
                            Directory.CreateDirectory(tDir + f.Name);
                        }
                        string tD = tDir + f.Name;
                        if (slash)
                        {
                            tD = tDir + "\\" + f.Name;
                        }
                        MoveDir(f, tD, true, refl);
                    }
                }

                //WATCHER
                DirWatcher watcher = new DirWatcher();
                watcher.StartWatching(info.FullName, 1000*5, refl);
            }, "AutoMover-DirMover");
        }

        public static void WriteTextFile(string file, string[] text)
        {
            FileStream f = new FileStream(file, FileMode.CreateNew, FileAccess.Write);
            byte[] currentBytes = Encoding.UTF8.GetBytes(text[0]);
            byte[] newline = Encoding.UTF8.GetBytes(Environment.NewLine);
            f.Write(currentBytes, 0, currentBytes.Length);
            for(int i = 1; i < text.Length; i++)
            {
                currentBytes = Encoding.UTF8.GetBytes(text[i]);
                f.Write(newline, 0, newline.Length);
                f.Write(currentBytes, 0, currentBytes.Length);
            }
        }

        public static void ShowNotification(string title, string text, ToolTipIcon icon)
        {
            NotifyIcon ni = new NotifyIcon();
            ni.Visible = false;
            ni.ShowBalloonTip(1000, title, text, icon);
        }
    }
}