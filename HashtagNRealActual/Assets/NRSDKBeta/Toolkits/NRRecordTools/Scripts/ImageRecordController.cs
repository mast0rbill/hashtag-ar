namespace NRKernal.Record.Tool
{
    using NRKernal;
    using System.IO;
    using UnityEngine;

    public class ImageRecordController : MonoBehaviour
    {
        public Camera CaptureCamera;
        private Transform CameraRig;

        NRRGBCamTexture CameraCapture { get; set; }
        FrameDataRecorder FrameDataRecorder { get; set; }

        public int ScreenWidth = 1280;
        public int ScreenHeight = 720;
        Texture2D m_EncodeTex = null;

        void Start()
        {
            CameraRig = CaptureCamera.transform.parent;

            CaptureCamera.targetTexture = new RenderTexture(ScreenWidth, ScreenHeight, 24, RenderTextureFormat.ARGB32);

            CameraCapture = new NRRGBCamTexture();
            CameraCapture.OnUpdate += OnFrameUpdate;

            this.InitConfig();
        }

        private void InitConfig()
        {
            string path = Path.Combine(NRTools.GetSdcardPath(), "record.config");
            path = path.Replace("file:/", "");
            if (!File.Exists(path))
            {
                Debug.Log("[PackageSender] write a default config :" + path);
                var newconfig = LitJson.JsonMapper.ToJson(new RecordConfig());
                File.WriteAllText(path, newconfig);
            }
            RecordConfig config = LitJson.JsonMapper.ToObject<RecordConfig>(File.ReadAllText(path));
            Debug.Log(config.ToString());

            if (config.isOnline)
            {
                FrameDataRecorder = new FrameDataRecorder(new PackImageEncoder(config));
            }
            else
            {
                FrameDataRecorder = new FrameDataRecorder(new LocalImageEncoder());
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.S))
            {
                CameraCapture.Play();
                FrameDataRecorder.Start();
            }

            if (Input.GetKeyDown(KeyCode.T))
            {
                Debug.Log("Stop rgb camera..");
                CameraCapture.Stop();
                FrameDataRecorder.Stop();
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                EncodeVirtualImage(NRTools.GetTimeStamp());
            }
        }

        private void OnFrameUpdate(RGBTextureFrame rgbframe)
        {
            FrameData frame = FrameDataRecorder.CreateAFrame();
            frame.timeStamp = rgbframe.timeStamp;
            frame.imageData = rgbframe.texture.EncodeToPNG();
            frame.frameType = FrameType.RGB;
            FrameDataRecorder.Write(frame);

            EncodeVirtualImage(rgbframe.timeStamp);
        }

        private void EncodeVirtualImage(ulong timestamp)
        {
#if !UNITY_EDITOR
            var head_pose = Pose.identity;
            NRFrame.GetHeadPoseByTime(ref head_pose, timestamp);
#else
            var head_pose = new Pose(Random.insideUnitSphere, Quaternion.Euler(Random.insideUnitSphere));
#endif

            CameraRig.transform.position = head_pose.position;
            CameraRig.transform.rotation = head_pose.rotation;

            var frame = FrameDataRecorder.CreateAFrame();
            frame.timeStamp = timestamp;
            frame.frameType = FrameType.Virtual;

            RenderTexture.active = CaptureCamera.targetTexture;
            CaptureCamera.Render();

            if (m_EncodeTex == null || m_EncodeTex.width != CaptureCamera.targetTexture.width || m_EncodeTex.height != CaptureCamera.targetTexture.height)
            {
                m_EncodeTex = new Texture2D(CaptureCamera.targetTexture.width, CaptureCamera.targetTexture.height);
            }
            m_EncodeTex.ReadPixels(new Rect(0, 0, m_EncodeTex.width, m_EncodeTex.height), 0, 0);
            m_EncodeTex.Apply();
            frame.imageData = m_EncodeTex.EncodeToJPG();

            FrameDataRecorder.Write(frame);
        }
    }
}