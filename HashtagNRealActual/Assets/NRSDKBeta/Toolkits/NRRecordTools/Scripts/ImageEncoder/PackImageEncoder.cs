using UnityEngine;

namespace NRKernal.Record.Tool
{
    public class PackImageEncoder : IImageEncoder
    {
        public PackageSender PackageSender { get; set; }

        public FrameData EndSignal;

        public PackImageEncoder(RecordConfig config)
        {
            PackageSender = new PackageSender(config);

            EndSignal = FrameData.CreateAEndSignalFrame();
        }

        public void Commit(FrameData frame)
        {
            PackageSender.Commit(frame);
        }

        public void Stop()
        {
            Debug.Log("Stop...");
            PackageSender.Commit(EndSignal);
        }
    }
}
