/* 
*   NatCorder
*   Copyright (c) 2019 Yusuf Olokoba
*/

namespace NatCorder.Examples
{
#if UNITY_EDITOR
    using UnityEditor;
#endif
    using UnityEngine;
    using UnityEngine.UI;
    using System.Collections;
    //using Clocks;
    using Inputs;

    public class TimeSynReplayCam : MonoBehaviour
    {
        [Header("Recording")]
        public int videoWidth = 1280;
        public int videoHeight = 720;

        //private MP4Recorder videoRecorder;
        //private IClock recordingClock;
        private TimesSynCameraInput cameraInput;

        public Camera TargetCamera;

        private bool isStarted = false;

        public void StartRecording()
        {
            if (isStarted)
            {
                return;
            }
            // Start recording
            //recordingClock = new RealtimeClock();
            //videoRecorder = new MP4Recorder(
            //    videoWidth,
            //    videoHeight,
            //    60,
            //    0,
            //    0,
            //    OnReplay
            //);

            if (TargetCamera == null)
            {
                TargetCamera = Camera.main;
            }
            // Create recording inputs
            //cameraInput = new TimesSynCameraInput(videoRecorder, recordingClock, TargetCamera);

            isStarted = true;
        }

        public void Commit(long timestamp = 0)
        {
            //cameraInput.CommitFrame(timestamp);
        }

        public void StopRecording()
        {
            cameraInput.Dispose();
            // Stop recording
            //videoRecorder.Dispose();

            isStarted = false;
        }

        private void OnReplay(string path)
        {
            Debug.Log("Saved recording to: " + path);
            // Playback the video
#if UNITY_EDITOR || UNITY_SDANDALONE_WIN
            EditorUtility.OpenWithDefaultApp(path);
#elif UNITY_IOS
            Handheld.PlayFullScreenMovie("file://" + path);
#elif UNITY_ANDROID
            //Handheld.PlayFullScreenMovie(path);
#endif
        }
    }
}