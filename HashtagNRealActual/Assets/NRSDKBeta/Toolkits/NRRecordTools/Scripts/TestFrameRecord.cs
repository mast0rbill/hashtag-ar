using NRKernal.Record;
using UnityEngine;
using System.Collections;
using NRKernal;
using NatCorder.Examples;

public class TestFrameRecord : MonoBehaviour
{
    //FrameDataRecorder FrameDataRecorder { get; set; }
    NRRGBCamTexture CameraCapture { get; set; }
    TimeSynReplayCam replayCamera { get; set; }

    public Pose[] poseList;

    private int index = 0;

    public Pose NextPose
    {
        get
        {
            if (index == poseList.Length)
            {
                index = 0;
            }
            return poseList[index++];
        }
    }

    public Texture2D tex;

    public UnityEngine.UI.RawImage image;

    private void Start()
    {
        var resolution = new Resolution();
        resolution.width = 1280;
        resolution.height = 720;
        Camera.main.targetTexture = new RenderTexture(resolution.width, resolution.height, 24, RenderTextureFormat.ARGB32);
        //FrameDataRecorder = new FrameDataRecorder(new LocalImageEncoder());

        //CameraCapture = gameObject.AddComponent<NRCameraCapture>();
        //CameraCapture.ImageFormat = CameraImageFormat.RGB_888;
        //CameraCapture.OnFrameUpdate += OnFrameUpdate;


        replayCamera = gameObject.GetComponent<TimeSynReplayCam>();
        replayCamera.TargetCamera = Camera.main;


        RenderTexture rt = new RenderTexture(tex.width, tex.height, 24);
        BlitTex2RenderTex(tex, rt);
        image.texture = rt;
    }

    public void BlitTex2RenderTex(Texture2D t, RenderTexture rt)
    {
        Graphics.Blit(t, rt);
    }

    //private void OnFrameUpdate(byte[] data)
    //{
    //    FrameData frame = FrameDataRecorder.CreateAFrame();
    //    frame.timeStamp = CameraCapture.HMDTimeNanos;
    //    frame.imageData = data;
    //    frame.frameType = FrameType.RGB;
    //    FrameDataRecorder.Write(frame);
    //}

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            //CameraCapture.Play();
            StartCoroutine(SaveCameraTexture());
        }

        //if (Input.GetKeyDown(KeyCode.W))
        //{
        //    for (int i = 0; i < 6; i++)
        //    {
        //        WriteAFrame();
        //    }
        //    //StartCoroutine(SynWriteFrame());
        //}

        if (Input.GetKeyDown(KeyCode.V))
        {
            replayCamera.StartRecording();
            CommitAVideoFrame();
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            replayCamera.StopRecording();
        }
    }

    private Texture2D tempTex;
    private IEnumerator SaveCameraTexture()
    {
        while (true && index != poseList.Length)
        {
            yield return new WaitForSecondsRealtime(1f);
            //WriteAFrame();
        }
    }

    //private IEnumerator SynWriteFrame()
    //{
    //    var frame = FrameDataRecorder.CreateAFrame();

    //    frame.timeStamp = (System.UInt64)(index);
    //    frame.frameType = FrameType.Virtual;

    //    var pose = NextPose;
    //    Camera.main.transform.position = pose.position;
    //    Camera.main.transform.rotation = pose.rotation;
    //    Debug.Log(pose.ToString());
    //    yield return new WaitForEndOfFrame();
    //    RenderTexture.active = Camera.main.targetTexture;

    //    if (tempTex == null)
    //    {
    //        tempTex = new Texture2D(Camera.main.targetTexture.width, Camera.main.targetTexture.height);
    //    }
    //    tempTex.ReadPixels(new Rect(0, 0, tempTex.width, tempTex.height), 0, 0);
    //    tempTex.Apply();

    //    frame.imageData = tempTex.EncodeToPNG();

    //    FrameDataRecorder.Write(frame);

    //    Debug.Log("Write A Frame：" + frame.timeStamp);
    //}

    //private void WriteAFrame()
    //{
    //    var frame = FrameDataRecorder.CreateAFrame();

    //    frame.timeStamp = (System.UInt64)(index);
    //    frame.frameType = FrameType.RGB;

    //    var pose = NextPose;
    //    Camera.main.transform.position = pose.position;
    //    Camera.main.transform.rotation = pose.rotation;
    //    Debug.Log(pose.ToString());
    //    RenderTexture.active = Camera.main.targetTexture;
    //    Camera.main.Render();

    //    if (tempTex == null)
    //    {
    //        tempTex = new Texture2D(Camera.main.targetTexture.width, Camera.main.targetTexture.height);
    //    }
    //    tempTex.ReadPixels(new Rect(0, 0, tempTex.width, tempTex.height), 0, 0);
    //    tempTex.Apply();

    //    frame.imageData = tempTex.EncodeToPNG();

    //    FrameDataRecorder.Write(frame);

    //    Debug.Log("Write A Frame：" + frame.timeStamp);
    //}

    private void CommitAVideoFrame()
    {
        var pose = NextPose;
        Camera.main.transform.position = pose.position;
        Camera.main.transform.rotation = pose.rotation;
        replayCamera.Commit();
    }

    //private void OnDestroy()
    //{
    //    FrameDataRecorder.Close();
    //}
}
