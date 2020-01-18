namespace NRKernal.Record.Tool
{
    using NRKernal;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using UnityEngine;

    public class InfomationReplay
    {
        public Camera cam;
        public Transform[] TransformList;
        public static SceneInfo m_SceneInfo;
        public static SlamPoseInfo m_SlamPoseInfo;
        public PoseInfo[] PoseInfos;
        public SlamPose[] SlamPoses;

        private Transform cameraRoot;

        public void LoadInfomation(Camera c, Transform[] transforms)
        {
            cam = c;
            cameraRoot = c.transform.parent;
            TransformList = transforms;

            LoadSceneInfo();
            LoadSlamPose();
            //LoadSlamPoseByown();

            var total = PoseInfos[PoseInfos.Length - 1].timeNanos - PoseInfos[0].timeNanos;
            var step = total / (uint)(PoseInfos.Length - 1);
            Debug.Log(string.Format("Total :{0} time step：{1}", total, step));
            Debug.Log(string.Format("pos1 len:{0} pos2 len:{1}", PoseInfos.Length, SlamPoses.Length));
            Debug.LogFormat("pos1 begin:{0} end:{1} pos2 begin:{2} end:{3}", PoseInfos[0].timeNanos, PoseInfos[PoseInfos.Length - 1].timeNanos
                , SlamPoses[0].timeNanos, SlamPoses[SlamPoses.Length - 1].timeNanos);
        }

        public void LoadSceneInfo()
        {
            string path = NRTools.GetSdcardPath() + "pathInfo.txt";
            var info = File.ReadAllText(path);
            m_SceneInfo = JsonUtility.FromJson<SceneInfo>(info);

            cam.projectionMatrix = m_SceneInfo.cameraMatrix;
            cam.transform.localPosition = m_SceneInfo.cameraLocalPose.position;
            cam.transform.localRotation = m_SceneInfo.cameraLocalPose.rotation;
            Debug.Log(m_SceneInfo.ToString());

            PoseInfos = m_SceneInfo.tranPoses.ToArray();
            Array.Sort(PoseInfos, (dir1, dir2) => dir1.timeNanos.CompareTo(dir2.timeNanos));
        }

        public void LoadSlamPose()
        {
            List<SlamPose> slamposelist = new List<SlamPose>();
            string path2 = NRTools.GetSdcardPath() + "propagator_pose.dat";
            //m_SlamPoseInfo = JsonUtility.FromJson<SlamPoseInfo>(info2);

            var lines = File.ReadAllLines(path2);
            for (int i = 0; i < lines.Length; i++)
            {
                string info = lines[i];
                bool result;
                SlamPose slampose = ReadSlamPoseByString(info, out result);
                if (result)
                {
                    slamposelist.Add(slampose);
                }
            }

            SlamPoses = slamposelist.ToArray();
        }

        private SlamPose ReadSlamPoseByString(string info, out bool result)
        {
            result = false;
            SlamPose pose = new SlamPose();
            string[] temp_params = info.Split(',');
            if (temp_params.Length != 9)
            {
                //Debug.LogError("param len is not ivalid!");
                return pose;
            }

            UInt64 timenanos;
            float pos_x, pos_y, pos_z, q_x, q_y, q_z, q_w;

            if (!UInt64.TryParse(temp_params[1], out timenanos)) return pose;
            if (!float.TryParse(temp_params[2], out pos_x)) return pose;
            if (!float.TryParse(temp_params[3], out pos_y)) return pose;
            if (!float.TryParse(temp_params[4], out pos_z)) return pose;
            if (!float.TryParse(temp_params[5], out q_x)) return pose;
            if (!float.TryParse(temp_params[6], out q_y)) return pose;
            if (!float.TryParse(temp_params[7], out q_z)) return pose;
            if (!float.TryParse(temp_params[8], out q_w)) return pose;

            pose.timeNanos = timenanos;

            pose.position = new Vector3(pos_x, pos_y, pos_z);
            pose.rotation = new Quaternion(q_x, q_y, q_z, q_w);

            Matrix4x4 m = Matrix4x4.TRS(pose.position, pose.rotation, Vector3.one);
            NativeMat4f native_m = new NativeMat4f(m);
            Pose temp;
            ConversionUtility.ApiPoseToUnityPose(native_m, out temp);
            pose.position = temp.position;
            pose.rotation = temp.rotation;

            result = true;
            return pose;
        }

        public void LoadSlamPoseByown()
        {
            string path = NRTools.GetSdcardPath() + "pathInfo2.txt";
            var info = File.ReadAllText(path);
            m_SlamPoseInfo = JsonUtility.FromJson<SlamPoseInfo>(info);

            SlamPoses = m_SlamPoseInfo.slamPoses.ToArray();
            Array.Sort(SlamPoses, (dir1, dir2) => dir1.timeNanos.CompareTo(dir2.timeNanos));
        }

        public PoseInfo SynInfomationByTimeStamp(UInt64 timestamp)
        {
            SynSlamPoseByTimeStamp(timestamp);
            var index = BinarySearch(PoseInfos, 0, PoseInfos.Length - 1, timestamp);
            if (index == -1)
            {
                return PoseInfos[0];
            }
            PoseInfo p = PoseInfos[index];
            for (int i = 0; i < p.items.Length; i++)
            {
                TransformList[i].position = p.items[i].position;
                TransformList[i].rotation = p.items[i].rotation;
            }
            return p;
        }

        public void SynSlamPoseByTimeStamp(UInt64 timestamp)
        {
            var index = BinarySearch(SlamPoses, 0, SlamPoses.Length - 1, timestamp);
            if (index == -1)
            {
                Debug.LogError("can not find the pos of time:" + timestamp);
                return;
            }
            var p = SlamPoses[index];
            cameraRoot.position = p.position;
            cameraRoot.rotation = p.rotation;
        }

        public SlamPose GetNextSlamPose(UInt64 timestamp)
        {
            var index = BinarySearch(SlamPoses, 0, SlamPoses.Length - 1, timestamp);
            if (index == -1 || (index + 1) >= SlamPoses.Length)
            {
                Debug.LogError("can not find the pos of time:" + timestamp);
                return new SlamPose();
            }
            return SlamPoses[index + 1];
        }

        public UInt64 TimeStep()
        {
            return (PoseInfos[PoseInfos.Length - 1].timeNanos - PoseInfos[0].timeNanos) / (UInt64)(PoseInfos.Length - 1);
        }

        public bool IsLastFrame(UInt64 time)
        {
            if (time > PoseInfos[PoseInfos.Length - 1].timeNanos)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public UInt64 GetFirstTimeStamp()
        {
            return PoseInfos[0].timeNanos;
        }

        public static int BinarySearch(PoseInfo[] arr, int low, int high, UInt64 key)
        {
            int mid = (low + high) / 2;
            if (low > high || key < arr[0].timeNanos || key > arr[arr.Length - 1].timeNanos)
            {
                return -1;
            }
            else
            {
                int result;
                if (CheckValue(arr, mid, key, out result))
                    return result;
                else if (arr[mid].timeNanos > key)
                    return BinarySearch(arr, low, mid - 1, key);
                else
                    return BinarySearch(arr, mid + 1, high, key);
            }
        }

        public static bool CheckValue(PoseInfo[] arr, int index, UInt64 key, out int resultIndex)
        {
            bool flag = false;
            resultIndex = index;
            if (arr[index].timeNanos == key)
            {
                resultIndex = index;
                flag = true;
            }
            else if (arr[index].timeNanos < key && (index + 1) < arr.Length && arr[index + 1].timeNanos > key)
            {
                resultIndex = (key - arr[index].timeNanos) < (arr[index + 1].timeNanos - key) ? index : (index + 1);
                flag = true;
            }
            else if (arr[index].timeNanos > key && (index - 1) >= 0 && arr[index - 1].timeNanos < key)
            {
                resultIndex = (arr[index].timeNanos - key) < (key - arr[index - 1].timeNanos) ? index : (index - 1);
                flag = true;
            }
            return flag;
        }

        public static int BinarySearch(SlamPose[] arr, int low, int high, UInt64 key)
        {
            int mid = (low + high) / 2;
            if (low > high || key < arr[0].timeNanos || key > arr[arr.Length - 1].timeNanos)
            {
                return -1;
            }
            else
            {
                int result;
                if (CheckValue(arr, mid, key, out result))
                    return result;
                else if (arr[mid].timeNanos > key)
                    return BinarySearch(arr, low, mid - 1, key);
                else
                    return BinarySearch(arr, mid + 1, high, key);
            }
        }

        public static bool CheckValue(SlamPose[] arr, int index, UInt64 key, out int resultIndex)
        {
            bool flag = false;
            resultIndex = index;
            if (arr[index].timeNanos == key)
            {
                resultIndex = index;
                flag = true;
            }
            else if (arr[index].timeNanos < key && (index + 1) < arr.Length && arr[index + 1].timeNanos > key)
            {
                resultIndex = (key - arr[index].timeNanos) < (arr[index + 1].timeNanos - key) ? index : (index + 1);
                flag = true;
            }
            else if (arr[index].timeNanos > key && (index - 1) >= 0 && arr[index - 1].timeNanos < key)
            {
                resultIndex = (arr[index].timeNanos - key) < (key - arr[index - 1].timeNanos) ? index : (index - 1);
                flag = true;
            }
            return flag;
        }
    }
}
