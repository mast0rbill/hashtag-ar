using NRKernal;
using System.Collections.Generic;
using UnityEngine;

public class PlaneDetetForRecord : MonoBehaviour
{
    /// <summary>
    /// A prefab for tracking and visualizing detected planes.
    /// </summary>
    public GameObject DetectedPlanePrefab;

    /// <summary>
    /// A list to hold new planes NRSDK began tracking in the current frame. This object is used across
    /// the application to avoid per-frame allocations.
    /// </summary>
    private List<NRTrackablePlane> m_NewPlanes = new List<NRTrackablePlane>();

    public void Update()
    {
        NRFrame.GetTrackables<NRTrackablePlane>(m_NewPlanes, NRTrackableQueryFilter.New);
        for (int i = 0; i < m_NewPlanes.Count; i++)
        {
            var pose = m_NewPlanes[i].GetCenterPose();
            DetectedPlanePrefab.transform.position = pose.position;
            DetectedPlanePrefab.transform.rotation = pose.rotation;

            DisablePlaneDetect();
        }
    }

    public void DisablePlaneDetect()
    {
        var config = NRSessionManager.Instance.NRSessionBehaviour.SessionConfig;
        config.PlaneFindingMode = TrackablePlaneFindingMode.DISABLE;
        NRSessionManager.Instance.SetConfiguration(config);
    }
}
