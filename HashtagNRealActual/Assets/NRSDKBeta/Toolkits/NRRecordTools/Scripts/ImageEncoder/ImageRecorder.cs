namespace NRKernal.Record.Tool
{
    using System.Collections.Generic;
    using System.Threading;

    public class ImageRecorder
    {
        Queue<FrameData> m_WaitingQueue = new Queue<FrameData>();

        ObjectPool FrameDataPool { get; set; }

        Thread m_EncodingThread;

        IImageEncoder m_Encoder;

        public ImageRecorder(ObjectPool pool, IImageEncoder encoder)
        {
            FrameDataPool = pool;
            m_Encoder = encoder;
            StartEncode();
        }

        public void Add(FrameData frame)
        {
            m_WaitingQueue.Enqueue(frame);
        }

        public void EncodImage(FrameData frame)
        {
            //UnityEngine.Debug.Log("Encode a image frame:" + frame.ToString());
            m_Encoder.Commit(frame);
            FrameDataPool.Retrieve<FrameData>(frame);
        }

        private void StartEncode()
        {
            if (m_EncodingThread == null)
            {
                m_EncodingThread = new Thread(Oprate);
                m_EncodingThread.IsBackground = true;
            }
            m_EncodingThread.Start();
        }

        public void Stop()
        {
            m_Encoder.Stop();
        }

        public void Release()
        {
            m_EncodingThread.Abort();
        }

        private void Oprate()
        {
            while (true)
            {
                Thread.Sleep(10);
                while (m_WaitingQueue.Count != 0)
                {
                    Thread.Sleep(16);
                    EncodImage(m_WaitingQueue.Dequeue());
                }
            }
        }
    }
}
