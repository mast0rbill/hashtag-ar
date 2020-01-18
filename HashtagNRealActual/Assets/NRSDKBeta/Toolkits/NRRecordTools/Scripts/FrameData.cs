namespace NRKernal.Record.Tool
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public enum FrameType
    {
        RGB = 0,
        Virtual = 1,
        Unknow = 2
    }

    public class FrameData
    {
        public FrameType frameType;
        public UInt64 timeStamp;
        public byte[] imageData;

        public static FrameData CreateAEndSignalFrame()
        {
            FrameData frame = new FrameData();
            frame.frameType = FrameType.Unknow;
            frame.timeStamp = 999;
            frame.imageData = new byte[1];

            return frame;
        }

        public override string ToString()
        {
            return string.Format("timeStamp :{0} frametype:{1}", timeStamp, (int)frameType);
        }

        public int ToBytes(FrameData structObj, ref byte[] data)
        {
            byte[] byte1 = BitConverter.GetBytes((int)structObj.frameType);
            byte[] byte2 = BitConverter.GetBytes(structObj.timeStamp);
            byte[] byte3 = structObj.imageData;
            byte1.CopyTo(data, 0);
            byte2.CopyTo(data, 4);
            byte3.CopyTo(data, 12);

            return 12 + imageData.Length;
        }
    }

    [Serializable]
    public struct SlamPoseInfo
    {
        public List<SlamPose> slamPoses;
    }

    [Serializable]
    public struct SlamPose
    {
        public UInt64 timeNanos;
        public Vector3 position;
        public Quaternion rotation;

        public SlamPose(UInt64 t, Pose p)
        {
            this.timeNanos = t;
            this.position = p.position;
            this.rotation = p.rotation;
        }

        public override string ToString()
        {
            return string.Format("Time:{0} position:{1} rotation:{2}", timeNanos, position.ToString(), rotation.ToString());
        }
    }

    [Serializable]
    public struct SceneInfo
    {
        public Matrix4x4 cameraMatrix;
        public Pose cameraLocalPose;
        public List<PoseInfo> tranPoses;

        public override string ToString()
        {
            return string.Format("matrix:{0} camera pose: {1} pos len:{2}", cameraMatrix.ToString(), cameraLocalPose.ToString(), tranPoses.Count);
        }
    }

    [Serializable]
    public struct PoseInfo
    {
        public UInt64 timeNanos;
        public PoseItem[] items;

        public PoseInfo(UInt64 t, PoseItem[] items)
        {
            this.timeNanos = t;
            this.items = items;
        }
    }

    [Serializable]
    public struct PoseItem
    {
        public string name;
        public Vector3 position;
        public Quaternion rotation;

        public PoseItem(string n, Pose p)
        {
            name = n;
            position = p.position;
            rotation = p.rotation;
        }
    }
}
