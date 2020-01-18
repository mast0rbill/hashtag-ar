using System;

namespace NRKernal.Record.Tool
{
    public class FrameDataRecorder
    {
        ObjectPool FrameDataPool { get; set; }
        ImageRecorder ImageRecorder { get; set; }

        public FrameDataRecorder(IImageEncoder encoder)
        {
            FrameDataPool = new ObjectPool();
            ImageRecorder = new ImageRecorder(FrameDataPool, encoder);
        }

        public void Write(FrameData data)
        {
            ImageRecorder.Add(data);
        }

        public FrameData CreateAFrame()
        {
            return FrameDataPool.Get<FrameData>();
        }

        public void Close()
        {
            ImageRecorder.Stop();
        }

        public void Start()
        {

        }

        public void Stop()
        {
            ImageRecorder.Stop();
        }
    }
}
