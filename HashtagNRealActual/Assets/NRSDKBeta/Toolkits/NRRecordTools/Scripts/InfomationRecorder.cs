namespace NRKernal.Record.Tool
{
    using System.Collections.Generic;
    using UnityEngine;
    using System.IO;
    using System;
    using System.Threading;
    using NRKernal;

    public class InfomationRecorder : MonoBehaviour
    {
        public Camera Camera;
        public Transform[] TransformList;
        public static SceneInfo m_SceneInfo = new SceneInfo();
        private static UInt64 beginTime = 0;
        public static SlamPoseInfo m_SlamPoseInfo = new SlamPoseInfo();

        UInt64 currentTime;
        bool isopen = false;
        private int SynCount = 30;

        public static InfomationRecorder Create(Camera c, Transform[] recordTrans)
        {
            var recorder = FindObjectOfType<InfomationRecorder>();
            if (recorder == null)
            {
                recorder = new GameObject("InfomationRecorder").AddComponent<InfomationRecorder>();
                recorder.Camera = c;
                recorder.TransformList = recordTrans;

                m_SceneInfo.cameraMatrix = c.projectionMatrix;
                m_SceneInfo.cameraLocalPose = new Pose(c.transform.localPosition, c.transform.localRotation);
                m_SceneInfo.tranPoses = new List<PoseInfo>();

                m_SlamPoseInfo.slamPoses = new List<SlamPose>();
            }

            return recorder;
        }

        public void RecordPoseInfomation(UInt64 time)
        {
            if (time == 0)
            {
                return;
            }
            if (beginTime == 0)
            {
                Debug.Log("Start record thread!");
                beginTime = time;
                currentTime = beginTime - 100000000;
                StartSlamposeRecordThread();
            }
            var poseinfo = new PoseInfo(time, new PoseItem[TransformList.Length]);
            for (int i = 0; i < TransformList.Length; i++)
            {
                var pose = new Pose(TransformList[i].position, TransformList[i].rotation);
                poseinfo.items[i].name = TransformList[i].name;
                poseinfo.items[i].position = pose.position;
                poseinfo.items[i].rotation = pose.rotation;

            }
            m_SceneInfo.tranPoses.Add(poseinfo);
        }

        Thread SlamposeRecordThread = null;
        private void StartSlamposeRecordThread()
        {
            if (SlamposeRecordThread == null)
            {
                isopen = true;
                SlamposeRecordThread = new Thread(RecordSlamPose);
                SlamposeRecordThread.IsBackground = true;
                SlamposeRecordThread.Start();
            }
        }

        public void RecordSlamPose()
        {
            int count = 0;
            while (isopen)
            {
                // syn rgb cameratime every 10 times
                //if (count % SynCount == 0)
                //{
                //    currentTime = NRRgbCamera.HMDTimeNanos;
                //}
                Pose pose = Pose.identity;
                NRFrame.GetHeadPoseByTime(ref pose, currentTime);
                m_SlamPoseInfo.slamPoses.Add(new SlamPose(currentTime, pose));
                Thread.Sleep(10);
                currentTime += 10000000;
                count++;
            }
        }

        public void SaveToFile()
        {
            Debug.Log("SaveToFile");
            try
            {
                isopen = false;
                if (SlamposeRecordThread != null)
                {
                    SlamposeRecordThread.Abort();
                }

                string info = JsonUtility.ToJson(m_SceneInfo);//JsonMapper.ToJson(m_SceneInfo);
                string path = @"/storage/emulated/0/pathInfo.txt";
                Debug.Log("Save file to path:" + path);
                File.WriteAllText(path, info);

                string path2 = @"/storage/emulated/0/pathInfo2.txt";
                File.WriteAllText(path2, JsonUtility.ToJson(m_SlamPoseInfo));
            }
            catch (Exception e)
            {
                Debug.Log(e.Message.ToString());
                throw;
            }
        }
    }
}