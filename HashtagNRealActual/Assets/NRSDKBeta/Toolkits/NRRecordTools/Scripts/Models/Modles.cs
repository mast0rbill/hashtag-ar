namespace NRKernal.Record.Tool
{
    using System;

    public class PackData
    {
        public int datasize;       //數據大小
        public int cursize;        //當前大小
        public int last_pak;       //是否是最後一片
        public byte[] data;        //帧数据 

        public override string ToString()
        {
            return string.Format("total size:{0},cursize:{1} islast pack :{2} data size:{3}", datasize, cursize, last_pak, data.Length);
        }

        // PackData to bytes
        public void ToBytes(ref byte[] data)
        {
            byte[] byte1 = BitConverter.GetBytes(this.datasize);
            byte[] byte2 = BitConverter.GetBytes(this.cursize);
            byte[] byte3 = BitConverter.GetBytes(this.last_pak);
            byte[] byte4 = this.data;
            byte1.CopyTo(data, 0);
            byte2.CopyTo(data, 4);
            byte3.CopyTo(data, 8);
            byte4.CopyTo(data, 12);
        }
    }

    [Serializable]
    public class RecordConfig
    {
        public bool isOnline = true;

        [Serializable]
        public class ServerConfig
        {
            public string ip;
            public int port;

            public ServerConfig()
            {
                this.ip = "192.168.68.218";
                this.port = 6000;
            }

            public ServerConfig(string ip, int port)
            {
                this.ip = ip;
                this.port = port;
            }
        }

        public ServerConfig server;

        public RecordConfig()
        {
            isOnline = true;
            server = new ServerConfig();
        }

        public override string ToString()
        {
            return string.Format("isonline :{0} ip:{1} port:{2}", isOnline, server.ip, server.port);
        }
    }
}
