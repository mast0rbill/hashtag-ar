namespace NRKernal.ThirdView
{
    using System;

    public struct ThirdViewConfig
    {
        public string serverIP;
        public int port;
        public int width;
        public int height;
        public int rate;
        public bool useDebugUI;

        public ThirdViewConfig(string serverip, int p = 6000, int w = 1920, int h = 1080, int rate = 30, bool usedebug = false)
        {
            this.serverIP = serverip;
            this.port = p;
            this.width = w;
            this.height = h;
            this.rate = rate;
            this.useDebugUI = usedebug;
        }
    }
}
