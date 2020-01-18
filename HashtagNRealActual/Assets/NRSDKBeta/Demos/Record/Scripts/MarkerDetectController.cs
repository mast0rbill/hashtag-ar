using NRKernal;
using System.Collections.Generic;
using UnityEngine;

public class MarkerDetectController : MonoBehaviour
{
    public GameObject TestPrefab;

    private List<NRTrackableImage> m_TempTrackingImages = new List<NRTrackableImage>();

    private NRTrackableImage currentTrackable = null;

    private bool needUpdata = true;

    private GameObject m_MoniPlane = null;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))
        {
            needUpdata = !needUpdata;
        }

        if (!needUpdata)
        {
            return;
        }

        if (currentTrackable != null)
        {
            var pose = currentTrackable.GetCenterPose();
            if (m_MoniPlane == null)
            {
                m_MoniPlane = Instantiate(TestPrefab, pose.position, pose.rotation);
            }

            m_MoniPlane.transform.rotation = pose.rotation;
            m_MoniPlane.transform.position = pose.position;
        }

        NRFrame.GetTrackables<NRTrackableImage>(m_TempTrackingImages, NRTrackableQueryFilter.New);

        foreach (var image in m_TempTrackingImages)
        {
            currentTrackable = image;
            return;
        }
    }
}
