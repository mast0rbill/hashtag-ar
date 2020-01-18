namespace NRKernal.Record.Tool
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Sockets;
    using System.Threading;

    public class PackageSender
    {
        private Socket m_ClientSocket;
        private const int _BuffSizePerFrame = 1024;
        private byte[] _TempData = new byte[1024 * 1024];
        Queue<PackData> waitForSendQueue = new Queue<PackData>();

        ObjectPool ObjectPool { get; set; }
        Thread SendThread;

        public PackageSender(RecordConfig config)
        {
            IPAddress ip = IPAddress.Parse(config.server.ip);
            IPEndPoint ipe = new IPEndPoint(ip, config.server.port);

            m_ClientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            m_ClientSocket.Connect(ipe);
            m_ClientSocket.SendBufferSize = 1024 * 1024;

            SendThread = new Thread(SendMsg);//开启发送消息线程
            SendThread.IsBackground = true;
            SendThread.Start();

            ObjectPool = new ObjectPool();
        }

        ~PackageSender()
        {
            m_ClientSocket.Close();
            SendThread.Abort();
        }

        private void SendMsg()
        {
            byte[] buff = new byte[_BuffSizePerFrame];
            while (true)
            {
                if (waitForSendQueue.Count != 0)
                {
                    var packadata = waitForSendQueue.Dequeue();
                    packadata.ToBytes(ref buff);
                    m_ClientSocket.Send(buff);
                    ObjectPool.Retrieve<PackData>(packadata);
                }
                Thread.Sleep(5);
            }
        }

        public void Commit(FrameData frame)
        {
            int len = frame.ToBytes(frame, ref _TempData);
            int datasize = _BuffSizePerFrame - 12;
            int offset = len % datasize;
            int count = len / datasize + (offset == 0 ? 0 : 1);

            for (int i = 0; i < count; i++)
            {
                PackData packdata = ObjectPool.Get<PackData>();
                if (i < count - 1)
                {
                    packdata.cursize = datasize;
                    packdata.last_pak = 0;
                }
                else
                {
                    packdata.cursize = offset == 0 ? datasize : offset;
                    packdata.last_pak = 1;

                }
                if (packdata.data == null )
                {
                    packdata.data = new byte[datasize];
                }
                packdata.datasize = len;
                Array.Copy(_TempData, i * datasize, packdata.data, 0, packdata.cursize);

                waitForSendQueue.Enqueue(packdata);
            }
        }
    }
}
