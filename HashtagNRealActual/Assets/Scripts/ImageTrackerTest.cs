using System.Collections;
using System.Collections.Generic;
using NRKernal.NRExamples;
using NRKernal.Persistence;
using NRKernal;
using UnityEngine;

public class ImageTrackerTest : MonoBehaviour
{
	public static Transform worldAnchor = null;
	public static NRTrackableImage Image;

	public int usedImageIndex = 0;
	public TrackingImageVisualizer TrackingImageVisualizerPrefab;
	TrackingImageVisualizer visualizer;
    void Update()
    {
		if (NRFrame.SessionStatus != SessionState.Tracking)
        {
            return;
        }

        List<NRTrackableImage> trackableImages = new List<NRTrackableImage>();
        NRFrame.GetTrackables<NRTrackableImage>(trackableImages, NRTrackableQueryFilter.All);
        Pose pose;
        foreach (var image in trackableImages)
        {
            if (image.GetDataBaseIndex() == usedImageIndex)
            {
                pose = image.GetCenterPose();
                if (visualizer == null)
                {
                    visualizer = (TrackingImageVisualizer)Instantiate(TrackingImageVisualizerPrefab, pose.position, pose.rotation);
                    visualizer.Image = image;
					Image = image;
					visualizer.transform.parent = transform;
					worldAnchor = transform;
                }
                break;
            }
        }
    }
}
