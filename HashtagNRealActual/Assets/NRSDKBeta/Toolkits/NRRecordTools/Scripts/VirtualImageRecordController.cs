namespace NRKernal.Record.Tool
{
    using UnityEngine;
    using System.Collections;

    public class VirtualImageRecordController : MonoBehaviour
    {
        public Transform RGBCameraRig;
        public Camera CaptureCamera;
        private InfomationReplay m_InfomationReplay;
        public ulong timeOffset = 0;
        private bool m_IsInitialized = false;

        FrameDataRecorder FrameDataRecorder { get; set; }

        public void Initialize(FrameDataRecorder frameRecord)
        {
            if (m_IsInitialized)
            {
                return;
            }
            FrameDataRecorder = frameRecord;

            CaptureCamera.targetTexture = new RenderTexture(480, 270, 24, RenderTextureFormat.ARGB32);
            InitInfomationRecorder();
            m_IsInitialized = true;
        }

        public void InitInfomationRecorder()
        {
            Transform[] trans = new Transform[1];
            trans[0] = GameObject.Find("SceneObject").transform;

            m_InfomationReplay = new InfomationReplay();
            m_InfomationReplay.LoadInfomation(CaptureCamera, trans);
        }

        public void StartRecordVirtualImage()
        {
            currentTime = 0;
            StartCoroutine(RecordVirtualImage());
        }

        private ulong currentTime;
        private IEnumerator RecordVirtualImage()
        {
            Texture2D tempTex = null;
            bool stoped = false;
            while (!stoped)
            {
                yield return new WaitForEndOfFrame();
                for (int i = 0; i < 100; i++)
                {
                    if (currentTime == 0)
                    {
                        var begin = m_InfomationReplay.GetFirstTimeStamp();
                        currentTime = begin - timeOffset * 1000000;
                    }

                    if (m_InfomationReplay.IsLastFrame(currentTime))
                    {
                        stoped = true;
                        break;
                    }
                    var slampose = m_InfomationReplay.GetNextSlamPose(currentTime);
                    if (slampose.timeNanos == 0)
                    {
                        stoped = true;
                        break;
                    }
                    currentTime = slampose.timeNanos;

                    RGBCameraRig.position = slampose.position;
                    RGBCameraRig.rotation = slampose.rotation;

                    var frame = FrameDataRecorder.CreateAFrame();
                    frame.timeStamp = slampose.timeNanos;
                    frame.frameType = FrameType.Virtual;

                    RenderTexture.active = CaptureCamera.targetTexture;
                    CaptureCamera.Render();

                    if (tempTex == null)
                    {
                        tempTex = new Texture2D(CaptureCamera.targetTexture.width, CaptureCamera.targetTexture.height);
                    }
                    tempTex.ReadPixels(new Rect(0, 0, tempTex.width, tempTex.height), 0, 0);
                    tempTex.Apply();
                    frame.imageData = tempTex.EncodeToPNG();
                    FrameDataRecorder.Write(frame);

                    Debug.Log("Write A Frame：" + frame.timeStamp);
                }

            }
        }
    }
}
