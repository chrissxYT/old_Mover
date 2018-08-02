using System.Data;

namespace Mover
{
    class ConfigParser
    {
        string[] inp;
        Reflector refl;

        public ConfigParser(string[] input, Reflector refl)
        {
            inp = input;
            this.refl = refl;
        }

        public Config Parse()
        {
            Config output = new Config();
            bool[] used = new bool[6] {false, false, false, false, false, false};
            foreach(string s in inp)
            {
                char[] c = s.ToCharArray();
                string[] ss = s.Split('/');
                if (c.Length == 0 || c[0] == '#' || c[0] == ' ' || c[0] == '/' && c[1] == '/')
                {
                    //Comments and blank lines
                }
                else if (s.StartsWith("dirToPut"))
                {
                    output.dir_to_put = ss[1];
                    used[0] = true;
                }
                else if (s.StartsWith("logPath"))
                {
                    output.log_path = ss[1];
                    used[1] = true;
                }
                else if (s.StartsWith("cacheSize"))
                {
                    output.cache_size = Expression(ss[1]);
                    used[2] = true;
                }
                else if (s.StartsWith("debug"))
                {
                    output.debug = Bool(ss[1]);
                    used[3] = true;
                }
                else if (s.StartsWith("beta"))
                {
                    output.beta = Bool(ss[1]);
                    used[4] = true;
                }
                else if (s.StartsWith("maxRam"))
                {
                    output.maxRam = Expression(ss[1]);
                    used[5] = true;
                }
                else
                {
                    Util.Print("Can't parse the line \"" + s + "\", this should be your fault.", refl, System.ConsoleColor.Red);
                }
            }
            bool notused = false;
            foreach(bool b in used)
            {
                if (!b)
                    notused = true;
            }
            if (notused)
                Util.Print("A config variable is not set, so the default value will be used, if you want to regenerate the config type \"config-regen\"!", refl, System.ConsoleColor.Red);
            return output;
        }

        private int Expression(string s)
        {
            DataTable table = new DataTable();
            return (int)table.Compute(s, "");
        }

        private bool Bool(string inp)
        {
            char c = inp.ToCharArray()[0];
            switch (c)
            {
                case '1':
                    return true;
                case 't':
                    return true;
            }
            if (inp.StartsWith("true"))
            {
                return true;
            }
            else if (inp.StartsWith("tru"))
            {
                return true;
            }
            else if (inp.StartsWith("tr"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
