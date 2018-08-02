using System;
using System.IO;

namespace Mover
{
    class DirFileChecker
    {
        public static void StartCheck(Reflector refl)
        {
            if (!Directory.Exists(refl.config.log_path))
            {
                Directory.CreateDirectory(refl.config.log_path);
            }
            if (!Directory.Exists(refl.config.dir_to_put))
            {
                Directory.CreateDirectory(refl.config.dir_to_put);
            }
            if (File.Exists(P.changelog_file))
            {
                String[] changelog = File.ReadAllLines(P.changelog_file);
                File.Delete(P.changelog_file);
                foreach (String s in changelog)
                {
                    Util.Print(s, refl, ConsoleColor.Yellow);
                }
            }
            if (File.Exists(UpdateManager.updaterFile))
            {
                File.Delete(UpdateManager.updaterFile);
                if (refl.config.debug)
                {
                    Util.Print("Deleted Updater-File.", refl, ConsoleColor.Yellow);
                }
            }
        }
    }
}
