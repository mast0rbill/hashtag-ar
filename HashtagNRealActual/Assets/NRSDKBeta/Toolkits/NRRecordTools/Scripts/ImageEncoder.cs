namespace NRKernal.Record.Tool
{
    using UnityEngine;

    public class ImageEncoder : MonoBehaviour
    {
        public Camera EncodeCamera;
        FrameDataRecorder FrameDataRecorder { get; set; }

        private bool m_IsInitialized = false;
        Texture2D m_EncodeTex = null;

        public FrameType FrameType;

        public void Start()
        {
            this.Initialize(new FrameDataRecorder(new PackImageEncoder(new RecordConfig())));
        }

        public void Initialize(FrameDataRecorder frameRecord)
        {
            if (m_IsInitialized)
            {
                return;
            }
            FrameDataRecorder = frameRecord;
            EncodeCamera.targetTexture = new RenderTexture(1280, 720, 24, RenderTextureFormat.ARGB32);
            m_IsInitialized = true;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Commit(NRKernal.NRTools.GetTimeStamp());
            }
        }

        public void Commit(ulong timestamp)
        {
            var frame = FrameDataRecorder.CreateAFrame();
            frame.timeStamp = timestamp;
            frame.frameType = FrameType;

            RenderTexture.active = EncodeCamera.targetTexture;
            EncodeCamera.Render();

            if (m_EncodeTex == null || m_EncodeTex.width != EncodeCamera.targetTexture.width || m_EncodeTex.height != EncodeCamera.targetTexture.height)
            {
                m_EncodeTex = new Texture2D(EncodeCamera.targetTexture.width, EncodeCamera.targetTexture.height);
            }
            m_EncodeTex.ReadPixels(new Rect(0, 0, m_EncodeTex.width, m_EncodeTex.height), 0, 0);
            m_EncodeTex.Apply();
            frame.imageData = m_EncodeTex.EncodeToJPG();
            FrameDataRecorder.Write(frame);

            Debug.Log("Write A Frame：" + frame.timeStamp);
        }
    }
}