using NRKernal;
using UnityEngine;

public class InputControllerDemo : MonoBehaviour
{
    public GameObject AndyPlanePrefab;

    void Update()
    {
        if (!NRInput.GetButtonDown(ControllerButton.TRIGGER))
        {
            return;
        }

        // Get controller laser origin.
        Transform laserAnchor = NRInput.AnchorsHelper.GetAnchor(ControllerAnchorEnum.RightLaserAnchor);

        RaycastHit hitResult;
        if (Physics.Raycast(new Ray(laserAnchor.transform.position, laserAnchor.transform.forward), out hitResult, 10))
        {
            if (hitResult.collider.gameObject != null)
            {
                // Instantiate Andy model at the hit point / compensate for the hit point rotation.
                var go = Instantiate(AndyPlanePrefab, hitResult.point, Quaternion.identity);
                go.transform.localScale = Vector3.one * 0.5f;
                DisableTrackable();
            }
        }
    }

    private void DisableTrackable()
    {
        var config = NRSessionManager.Instance.NRSessionBehaviour.SessionConfig;
        config.PlaneFindingMode = TrackablePlaneFindingMode.DISABLE;
        config.ImageTrackingMode = TrackableImageFindingMode.DISABLE;
        NRSessionManager.Instance.SetConfiguration(config);
    }
}
