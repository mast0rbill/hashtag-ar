namespace NRKernal.Record.Tool
{
    using System;
    using System.Net;
    using System.Net.Sockets;

    public class NetWorkImageEncoder : IImageEncoder
    {
        private Socket m_ClientSocket;

        public NetWorkImageEncoder()
        {
            int port = 6000;
            string host = "192.168.68.218";
            IPAddress ip = IPAddress.Parse(host);
            IPEndPoint ipe = new IPEndPoint(ip, port);

            m_ClientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            m_ClientSocket.Connect(ipe);
        }

        ~NetWorkImageEncoder()
        {
            m_ClientSocket.Close();
        }

        public void Commit(FrameData frame)
        {
            m_ClientSocket.Send(MixBytes(frame));
        }

        public byte[] MixBytes(FrameData frame)
        {
            byte[] returnByte = null;
            using (var bufferStream = new System.IO.MemoryStream())
            {
                bufferStream.Write(BitConverter.GetBytes((int)(frame.frameType)), 0, 4);
                bufferStream.Write(BitConverter.GetBytes(frame.timeStamp), 0, 8);
                bufferStream.Write(frame.imageData, 0, frame.imageData.Length);
                returnByte = bufferStream.ToArray();
            }

            //UnityEngine.Debug.LogFormat("total length:{0} wanted length:{1}", returnByte.Length, (frame.imageData.Length + 12));
            return returnByte;
        }


        public void Stop() { }
    }
}
