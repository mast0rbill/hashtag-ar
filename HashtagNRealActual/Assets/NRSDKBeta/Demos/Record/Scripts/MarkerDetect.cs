using NRKernal;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarkerDetect : MonoBehaviour
{
    public GameObject TestPrefab;

    private List<NRTrackableImage> m_TempTrackingImages = new List<NRTrackableImage>();

    private NRTrackableImage currentTrackable = null;

    private bool needUpdata = true;

    private int frameCount = 0;
    private int maxFrame = 5;

    private Vector3 ofset = Vector3.zero;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            TestPrefab.transform.position += Vector3.up * 0.02f;
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            TestPrefab.transform.position += Vector3.down * 0.02f;
        }

        if (Input.GetKeyDown(KeyCode.U))
        {
            needUpdata = !needUpdata;
        }

        if (!needUpdata || frameCount == maxFrame)
        {
            if (frameCount == maxFrame)
            {
                StopImageTracking();
            }
            return;
        }

        if (currentTrackable != null)
        {
            frameCount++;
            //TestPrefab.transform.localScale = new Vector3(currentTrackable.ExtentX, currentTrackable.ExtentZ, currentTrackable.ExtentZ);

            TestPrefab.transform.rotation = currentTrackable.GetCenterPose().rotation;

            var position = currentTrackable.GetCenterPose().position - ofset;
            TestPrefab.transform.position = position + TestPrefab.transform.up * 0.5f * currentTrackable.ExtentX;
        }

        NRFrame.GetTrackables<NRTrackableImage>(m_TempTrackingImages, NRTrackableQueryFilter.New);

        foreach (var image in m_TempTrackingImages)
        {
            currentTrackable = image;
            return;
        }
    }

    private void StopImageTracking()
    {
        var config = NRSessionManager.Instance.NRSessionBehaviour.SessionConfig;
        config.ImageTrackingMode = TrackableImageFindingMode.DISABLE;
        NRSessionManager.Instance.SetConfiguration(config);
    }
}
