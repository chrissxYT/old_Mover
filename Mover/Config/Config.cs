namespace Mover
{
    class Config
    {
        public string dir_to_put = "C:\\MOVED\\";
        public string log_path = "C:\\MOVER-LOGS\\";
        public int cache_size = 8 * 1024;
        public bool debug = false;
        public bool beta = false;
        public long maxRam = 1024 * 1024 * 256;
    }
}