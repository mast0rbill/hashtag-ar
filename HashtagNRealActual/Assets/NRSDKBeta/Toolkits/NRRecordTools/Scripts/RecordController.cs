namespace NRKernal.Record
{
    using NatCorder.Examples;
    using NRKernal;
    using NRKernal.NRExamples;
    using System;
    using UnityEngine;
    using UnityEngine.UI;

    public class RecordController : MonoBehaviour
    {
        public Transform RGBCameraRig;
        public Camera CaptureCamera;
        public Camera RecordCamera;
        public RawImage RGBCameraImage;
        public RawImage VirtualCameraImage;
        private NRRGBCamTexture CameraCapture { get; set; }
        private bool m_IsInitialized = false;
        public TimeSynReplayCam ReplayCam;
        public Texture2D DefaultTex;
        public Text timeTips;

        public bool UseRecord = false;
        public bool DebugMode = false;

        public Texture PreviewTexture
        {
            get
            {
                return RecordCamera.targetTexture;
            }
        }

        //private InfomationRecorder m_InfomationRecorder;
        //private InfomationReplay m_InfomationReplay;
        private UInt64 frameCount = 0;
        private UInt64 timeOffset = 1;

        public enum RecordType
        {
            Read,
            Write,
            None
        }
        private RecordType recordType = RecordType.None;

        public enum BlendMode
        {
            SingleEye,
            DoubleEye
        }

        public BlendMode blendMode = BlendMode.SingleEye;
        private int OriginCaptureCameraMask;
        private int OriginRecordCameraMask;

        private bool m_StartRecord = false;
        public int RecordScreenWidth = 1280;
        public int RecordScreenHeight = 720;

        void Start()
        {
            if (DebugMode)
            {
#if !UNITY_EDITOR && UNITY_ANDROID
            recordType = RecordType.Write;
#elif UNITY_EDITOR
                recordType = RecordType.Read;
#endif
            }
            else
            {
                recordType = RecordType.None;
            }

#if !UNITY_EDITOR && UNITY_ANDROID
            CameraCapture = new NRRGBCamTexture();
            RGBCameraImage.texture = CameraCapture.GetTexture();
            CameraCapture.OnUpdate += OnFrameUpdate;
#else
            //if (RGBCameraImage.gameObject.GetComponent<CameraPreview>() == null)
            //{
            //    var preview = RGBCameraImage.gameObject.AddComponent<CameraPreview>();
            //    preview.cameraName = "5M Camera";
            //}
#endif

            ReplayCam.TargetCamera = RecordCamera;

            //LayerBlend = gameObject.GetComponentInChildren<LayerBlend>();
            //LayerBlend.SetBlendMode(LayerBlend.BlendMode.Combine);
            GameObject.Find("NRLogo").GetComponent<AutoRotate>().enabled = false;

            OriginCaptureCameraMask = CaptureCamera.cullingMask;
            OriginRecordCameraMask = RecordCamera.cullingMask;

            InitRecordCameraByBlendMode();
        }

        private void Initialize()
        {
            if (m_IsInitialized)
            {
                return;
            }

#if !UNITY_EDITOR
        bool result;
        var matrix_data = NRFrame.GetEyeProjectMatrix(out result,CaptureCamera.nearClipPlane, CaptureCamera.farClipPlane);
        if (result)
        {
            CaptureCamera.projectionMatrix = matrix_data.RGBEyeMatrix;
            CaptureCamera.targetTexture = new RenderTexture(RecordScreenWidth, RecordScreenHeight, 24, RenderTextureFormat.ARGB32);
            Debug.Log("[Matrix] RGB Camera Project Matrix :" + CaptureCamera.projectionMatrix.ToString());

            var eyeposFromHead = NRFrame.EyePosFromHead;
            CaptureCamera.transform.localPosition = eyeposFromHead.RGBEyePos.position;
            CaptureCamera.transform.localRotation = eyeposFromHead.RGBEyePos.rotation;
            if(DebugMode) InitInfomationRecorder();
            m_IsInitialized = true;
        }
#else
            CaptureCamera.targetTexture = new RenderTexture(RecordScreenWidth, RecordScreenHeight, 24, RenderTextureFormat.ARGB32);
            if (DebugMode) InitInfomationRecorder();
            m_IsInitialized = true;
#endif
        }

        private void InitRecordCameraByBlendMode()
        {
            RectTransform rgbscreen = RecordCamera.transform.Find("Canvas/RGBScreen").GetComponent<RectTransform>();
            RectTransform virtualscreen = RecordCamera.transform.Find("Canvas/VirtualScreen").GetComponent<RectTransform>();
            int width, height;
            switch (blendMode)
            {
                case BlendMode.SingleEye:
                    width = RecordScreenWidth;
                    height = RecordScreenHeight;
                    RecordCamera.aspect = (float)width / height;
                    ReplayCam.videoWidth = width;
                    ReplayCam.videoHeight = height;
                    RecordCamera.targetTexture = new RenderTexture(width, height, 24, RenderTextureFormat.ARGB32);

                    rgbscreen.anchoredPosition3D = new Vector3(width * 0.5f, 0, 0);
                    rgbscreen.sizeDelta = new Vector2(width, height);

                    virtualscreen.anchoredPosition3D = new Vector3(-width * 0.5f, 0, 0);
                    virtualscreen.sizeDelta = new Vector2(width, height);
                    break;
                case BlendMode.DoubleEye:
                    width = RecordScreenWidth * 2;
                    height = RecordScreenHeight;
                    RecordCamera.aspect = (float)width / height;
                    ReplayCam.videoWidth = width;
                    ReplayCam.videoHeight = height;
                    RecordCamera.targetTexture = new RenderTexture(width, height, 24, RenderTextureFormat.ARGB32);

                    rgbscreen.anchoredPosition3D = new Vector3(width * 0.25f, 0, 0);
                    rgbscreen.sizeDelta = new Vector2(width * 0.5f, height);

                    virtualscreen.anchoredPosition3D = new Vector3(-width * 0.25f, 0, 0);
                    virtualscreen.sizeDelta = new Vector2(width * 0.5f, height);
                    break;
                default:
                    break;
            }
        }

        private void InitInfomationRecorder()
        {
            Transform[] trans = new Transform[1];
            trans[0] = GameObject.Find("SceneObject").transform;

            //switch (recordType)
            //{
            //    case RecordType.Read:
            //        m_InfomationReplay = new InfomationReplay();
            //        m_InfomationReplay.LoadInfomation(CaptureCamera, trans);
            //        break;
            //    case RecordType.Write:
            //        m_InfomationRecorder = InfomationRecorder.Create(CaptureCamera, trans);
            //        break;
            //    default:
            //        break;
            //}
        }

        private void OnFirstFramReady()
        {

        }

        private void OnFrameUpdate(RGBTextureFrame rgbframe)
        {
            UpdateInfo();

            //CaptureCamera.cullingMask = OriginCaptureCameraMask;
            //RecordCamera.cullingMask = OriginRecordCameraMask;
            //CaptureCamera.Render();

            //VirtualCameraImage.texture = CaptureCamera.targetTexture;
            //if (CameraCapture != null && CameraCapture.Texture == null)
            //{
            //    RGBCameraImage.texture = DefaultTex;
            //}

            ReplayCam.Commit((long)(rgbframe.timeStamp - timeOffset * 1000000));

            //CaptureCamera.cullingMask = 0;
            //RecordCamera.cullingMask = 0;
        }

        private void OnError(string msg)
        {
            Debug.Log(msg);
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.R) || NRInput.GetButtonDown(ControllerButton.APP))
            {
                StartRecord();
            }

            if (Input.GetKeyDown(KeyCode.T) || NRInput.GetButtonDown(ControllerButton.HOME))
            {
                StopRecord();
            }

            if (Input.GetKeyDown(KeyCode.W))
            {
                timeOffset++;
            }
            if (Input.GetKeyDown(KeyCode.S))
            {
                timeOffset--;

            }
            timeTips.text = timeOffset.ToString();
            //UpdateInfo();
        }

        private void StartRecord()
        {
            currentTime = 0;
            GameObject.Find("NRLogo").GetComponent<AutoRotate>().enabled = true;
            m_StartRecord = true;
            if (CameraCapture != null)
                CameraCapture.Play();
            if (UseRecord)
            {
                ReplayCam.StartRecording();
            }
        }

        private void StopRecord()
        {
            GameObject.Find("NRLogo").GetComponent<AutoRotate>().enabled = false;
            m_StartRecord = false;

            if (CameraCapture != null)
                CameraCapture.Stop();
            if (UseRecord) ReplayCam.StopRecording();
            //if (m_InfomationRecorder != null)
            //{
            //    m_InfomationRecorder.SaveToFile();
            //}
        }

        UInt64 currentTime = 0;
        private void UpdateInfo()
        {
            Initialize();

            if (!DebugMode)
            {
#if !UNITY_EDITOR && UNITY_ANDROID
                UpdateHeadPoseByTimestamp(CameraCapture.CurrentFrame.timeStamp - timeOffset * 1000000);
#else
                UpdateHeadPoseByTimestamp(0, timeOffset * 1000000);
#endif
            }

            VirtualCameraImage.texture = CaptureCamera.targetTexture;
            if (CameraCapture != null && CameraCapture.GetTexture() == null)
            {
                RGBCameraImage.texture = DefaultTex;
            }
        }

        private void FixedUpdate()
        {
            if (DebugMode)
            {
                if (m_StartRecord)
                {
                    switch (recordType)
                    {
                        case RecordType.Read:
                            //var begin = m_InfomationReplay.GetFirstTimeStamp();
                            //if (currentTime == 0)
                            //{
                            //    currentTime = begin - timeOffset * 1000000;
                            //}

                            //if (m_InfomationReplay.IsLastFrame(currentTime + timeOffset * 1000000))
                            //{
                            //    this.StopRecord();
                            //    return;
                            //}
                            //m_InfomationReplay.SynInfomationByTimeStamp(currentTime);
                            currentTime += (ulong)(Time.fixedDeltaTime * 1000000000);
                            break;
                        case RecordType.Write:
#if UNITY_EDITOR
                            frameCount++;
                            //m_InfomationRecorder.RecordPoseInfomation(frameCount * timeOffset * 1000000);
#else
                    //m_InfomationRecorder.RecordPoseInfomation(CameraCapture.CurrentFrame.timeStamp);
#endif
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        private void UpdateHeadPoseByTimestamp(UInt64 timestamp, UInt64 predict = 0)
        {
            //Debug.LogFormat("RGB camera time:{0} predict time:{1}", NRRgbCamera.HMDTimeNanos, timestamp);
            Pose head_pose = Pose.identity;
            NRFrame.GetHeadPoseByTime(ref head_pose, timestamp, predict);
            RGBCameraRig.transform.position = head_pose.position;
            RGBCameraRig.transform.rotation = head_pose.rotation;
        }

        void OnDestroy()
        {
            if (CameraCapture != null)
                CameraCapture.Stop();
        }
    }
}