using UnityEngine;
using System.Collections;
using NRKernal.Record;

namespace NRKernal.ThirdView
{
    public class NRThirdViewCapture : MonoBehaviour
    {
        public NRRecordBehaviour.NRRecordEvent OnReady;
#if !UNITY_EDITOR
        private VideoEncoder VideoEncoder { get; set; }
#endif
        public Camera CaptureCamera;
        public LayerMask CaptureCameraMask;
        private NativeEncodeConfig encodeConfig;

        NativeMat3f FCC_78 = new NativeMat3f(
           new Vector3(1402.06530528f, 0, 0),
           new Vector3(0, 1401.16300406f, 0),
           new Vector3(939.51336953f, 545.53574753f, 1)
       );

        [HideInInspector]
        public bool isPlaying = false;
        private bool m_IsInit = false;

        void Awake()
        {
            CaptureCamera.enabled = false;
            CaptureCamera.cullingMask = CaptureCameraMask;
            CaptureCamera.backgroundColor = new Color(0, 0, 0, 0);
        }

        public void Init(NativeEncodeConfig config)
        {
            if (m_IsInit)
            {
                return;
            }

#if !UNITY_EDITOR
            VideoEncoder = new VideoEncoder();
#endif
            encodeConfig = config;
            this.SetConfigration(encodeConfig);

            CaptureCamera.enabled = true;
            CaptureCamera.targetTexture = new RenderTexture(encodeConfig.width, encodeConfig.height, 24, RenderTextureFormat.ARGB32);
            CaptureCamera.depthTextureMode = DepthTextureMode.Depth;

            //Use default fov.
            UpdateCameraParam(ProjectMatrixUtility.CalculateFOVByFCC(FCC_78));

            m_IsInit = true;

            if (OnReady != null)
            {
                OnReady();
            }
        }

        public void UpdateCameraParam(Fov4f fov)
        {
            //Update fov.
            NRDebugger.Log(fov.ToString());
            CaptureCamera.projectionMatrix = ProjectMatrixUtility.PerspectiveOffCenter(fov.left, fov.right, fov.bottom, fov.top,
                CaptureCamera.nearClipPlane, CaptureCamera.farClipPlane);
            NRDebugger.Log(CaptureCamera.projectionMatrix.ToString());
        }

        public void StartRecord()
        {
            if (!m_IsInit || isPlaying)
            {
                return;
            }
#if !UNITY_EDITOR
            VideoEncoder.Start();
#endif

            StopCoroutine("UpdateFrame");
            StartCoroutine("UpdateFrame");

            isPlaying = true;
        }

        public void StopRecord()
        {
            if (!m_IsInit || !isPlaying)
            {
                return;
            }
#if !UNITY_EDITOR
            VideoEncoder.Stop();
#endif
            StopCoroutine("UpdateFrame");

            isPlaying = false;
        }

        private IEnumerator UpdateFrame()
        {
            int fps = 30;
            if (encodeConfig.fps > 0 || encodeConfig.fps < 30)
            {
                fps = encodeConfig.fps;
            }
            while (true)
            {
                yield return new WaitForSeconds(1f / fps);

#if !UNITY_EDITOR
                VideoEncoder.Commit(CaptureCamera.targetTexture, NRTools.GetTimeStamp());
#endif
            }
        }

        private void SetConfigration(NativeEncodeConfig config)
        {
            NRDebugger.Log("SetConfigration :" + config.ToString());
#if !UNITY_EDITOR
            VideoEncoder.SetConfig(config);
#endif
        }
    }
}
