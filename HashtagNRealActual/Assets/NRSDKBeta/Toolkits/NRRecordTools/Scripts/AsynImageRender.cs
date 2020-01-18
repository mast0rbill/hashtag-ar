using System.Collections.Generic;
using System.Collections;
using System.IO;
using Unity.Collections;
using UnityEngine;
using System;
using UnityEngine.UI;
using NRKernal;

#if UNITY_2018_2_OR_NEWER
using UnityEngine.Rendering;
#else
using UnityEngine.Experimental.Rendering;
#endif

namespace NRKernal.Record.Tool
{
    public class FrameImage
    {
        public UInt64 timeStamp;
        public Texture2D rgbTex;
    }

    public class AsynImageRender : MonoBehaviour
    {
        public Text TimeStamp;
        public RawImage Image;
        public RawImage Preview;

        Queue<AsyncGPUReadbackRequest> _requests = new Queue<AsyncGPUReadbackRequest>();

        public ObjectPool ObjectPool { get; set; }

        public Queue<FrameImage> preOprateQueue = new Queue<FrameImage>();

        private bool isOpen = false;
        private Texture2D RenderImage;

        private void Start()
        {
            Application.targetFrameRate = 60;
            ObjectPool = new ObjectPool();
            Camera.main.targetTexture = new RenderTexture(1280, 720, 24);
            StartCoroutine(ReadThread());
            StartCoroutine(WriteThread());

            RenderImage = new Texture2D(Camera.main.targetTexture.width, Camera.main.targetTexture.height, TextureFormat.RGBA32, false);
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                var frame = ObjectPool.Get<FrameImage>();
                frame.timeStamp = NRKernal.NRTools.GetTimeStamp();
                if (frame.rgbTex == null)
                {
                    frame.rgbTex = Texture2D.whiteTexture;
                }
                Debug.Log("write :" + frame.timeStamp);
                preOprateQueue.Enqueue(frame);
            }

            if (Input.GetKeyDown(KeyCode.O))
            {
                isOpen = !isOpen;
            }

            while (_requests.Count > 0)
            {
                Debug.Log("get a request!");
                var req = _requests.Peek();

                if (req.hasError)
                {
                    Debug.Log("GPU readback error detected.");
                    _requests.Dequeue();
                }
                else if (req.done)
                {
                    var buffer = req.GetData<Color32>();

                    SaveBitmap(buffer, Camera.main.targetTexture.width, Camera.main.targetTexture.height, NRKernal.NRTools.GetTimeStamp());
                    _requests.Dequeue();
                }
                else
                {
                    break;
                }
            }
        }

        //void OnRenderImage(RenderTexture source, RenderTexture destination)
        //{
        //    if (_requests.Count < 8)
        //        _requests.Enqueue(AsyncGPUReadback.Request(source));
        //    else
        //        Debug.Log("Too many requests.");

        //    Graphics.Blit(source, destination);
        //}

        void SaveBitmap(NativeArray<Color32> buffer, int width, int height, UInt64 timestamp)
        {
            var tex = new Texture2D(width, height, TextureFormat.RGBA32, false);
            tex.SetPixels32(buffer.ToArray());
            tex.Apply();
            string path = Path.Combine(NRKernal.NRTools.GetSdcardPath(), string.Format("RecordImages/Test/{0}.png", timestamp));
            File.WriteAllBytes(path, ImageConversion.EncodeToPNG(tex));
            Destroy(tex);
        }

        IEnumerator WriteThread()
        {
            while (true)
            {
                if (isOpen)
                {
                    var frame = ObjectPool.Get<FrameImage>();
                    frame.timeStamp = NRKernal.NRTools.GetTimeStamp();
                    if (frame.rgbTex == null)
                    {
                        frame.rgbTex = Texture2D.whiteTexture;
                    }
                    //Debug.Log("write :" + frame.timeStamp);
                    preOprateQueue.Enqueue(frame);
                }

                yield return new WaitForEndOfFrame();
            }
        }

        IEnumerator ReadThread()
        {
            while (true)
            {
                if (preOprateQueue.Count > 0)
                {
                    var frame = preOprateQueue.Peek();
                    TimeStamp.text = frame.timeStamp.ToString();
                    Image.texture = frame.rgbTex;
                    Camera.main.Render();
                    //Debug.Log("render :" + NRTools.GetTimeStamp().ToString());
                    //_requests.Enqueue(AsyncGPUReadback.Request(Camera.main.targetTexture));
                    AsyncGPUReadback.Request(Camera.main.targetTexture, 0, GPUReadBack);
                    ObjectPool.Retrieve<FrameImage>(frame);
                    preOprateQueue.Dequeue();
                }


                yield return new WaitForEndOfFrame();
            }
        }

        private void GPUReadBack(AsyncGPUReadbackRequest req)
        {
            if (req.done)
            {
                Debug.Log("result :" + NRTools.GetTimeStamp().ToString());
                var buffer = req.GetData<Color32>();

                RenderImage.SetPixels32(buffer.ToArray());
                RenderImage.Apply();

                Preview.texture = RenderImage;
                Debug.Log("read :" + NRTools.GetTimeStamp().ToString());

            }
        }
    }
}
