namespace NatCorder.Inputs
{
    using UnityEngine;
    using System;
    //using Clocks;
    //using Dispatch;
    using System.Collections.Generic;

    public sealed class TimesSynCameraInput : IDisposable
    {
        public int recordEveryNthFrame = 1;

        //public TimesSynCameraInput(IMediaRecorder mediaRecorder, IClock clock, Camera cameras)
        //{
        //    this.mediaRecorder = mediaRecorder;
        //    this.renderCamera = cameras;
        //    this.clock = clock;
        //    //DispatchUtility.onFrame += OnFrame;
        //}

        public void Dispose()
        {
            //DispatchUtility.onFrame -= OnFrame;
        }

        //private readonly IMediaRecorder mediaRecorder;
        private readonly Camera renderCamera;
        //private readonly IClock clock;
        private int frameCount;
        private Dictionary<long, long> m_TimestampDict = new Dictionary<long, long>();

        //public void CommitFrame(long timestamp)
        //{
        //    if (timestamp == 0)
        //    {
        //        timestamp = clock.Timestamp;
        //    }

        //    if (m_TimestampDict.ContainsKey(timestamp))
        //    {
        //        return;
        //    }
        //    // Check frame index
        //    if (frameCount++ % recordEveryNthFrame != 0)
        //        return;
        //    // Acquire frame
        //    var encoderFrame = mediaRecorder.AcquireFrame();
        //    // Render every camera
        //    var prevTarget = renderCamera.targetTexture;
        //    renderCamera.targetTexture = encoderFrame;
        //    renderCamera.Render();
        //    renderCamera.targetTexture = prevTarget;

        //    // Commit frame                
        //    mediaRecorder.CommitFrame(encoderFrame, timestamp);

        //    m_TimestampDict.Add(timestamp, timestamp);
        //}
    }
}
