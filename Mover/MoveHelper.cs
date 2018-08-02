using System;
using System.ComponentModel;
using System.IO;

namespace Mover
{
    class MoveHelper
    {

        private MovingDialog dialog;
        private Reflector refl;
        private int x, y;
        private BackgroundWorker w = new BackgroundWorker();

        public MoveHelper(Reflector refl, int x, int y)
        {
            this.refl = refl;
            this.x = x;
            this.y = y;
        }

        public void Move(String SourceFilePath, String DestFilePath, int cache_size)
        {
            refl.threadManager.StartThread(() =>
            {
                byte[] buffer = new byte[cache_size];
                OnStart(SourceFilePath);
                using (FileStream source = new FileStream(SourceFilePath, FileMode.Open, FileAccess.Read))
                {
                    long fileLength = source.Length;
                    using (FileStream dest = new FileStream(DestFilePath, FileMode.CreateNew, FileAccess.Write))
                    {
                        long totalBytes = 0;
                        int currentBlockSize = 0;
                        int lastPer = 0;

                        while ((currentBlockSize = source.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            totalBytes += currentBlockSize;

                            dest.Write(buffer, 0, currentBlockSize);

                            int per;
                            if ((per = (int)((totalBytes*100)/fileLength)) > lastPer)
                            {
                                OnProgressChanged(per, SourceFilePath);
                                lastPer = per;
                            }
                        }
                    }
                }
                File.Delete(SourceFilePath);
                OnDone(SourceFilePath);
                w.RunWorkerAsync();
            }, "AutoMover-FileMover");
        }

        private void OnProgressChanged(int per, string path)
        {
            //dialog.setProgressBarPercentage(per);
            //dialog.Update();
            refl.UpdatePer(path, per);
        }

        private void OnDone(string s)
        {
            Util.Print("Done moving " + s + ".", refl, ConsoleColor.Green);
            //dialog.Close();
            refl.movingFiles.Remove(s);
            refl.RemoveMove(s);
        }

        private void OnStart(string s)
        {
            dialog = new MovingDialog(x, y);
            dialog.setFilename(s);
            refl.AddMove(s);
            //dialog.Show();
            //dialog.Update();
            Util.Print("Started moving " + s + ".", refl, ConsoleColor.DarkGreen);
        }
    }
}
