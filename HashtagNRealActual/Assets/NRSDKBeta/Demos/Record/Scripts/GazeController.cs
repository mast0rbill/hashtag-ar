using UnityEngine;

namespace NRKernal.NRExamples
{
    /// <summary>
    /// Controls the HelloAR example.
    /// </summary>
    public class GazeController : MonoBehaviour
    {
        /// <summary>
        /// A model to place when a raycast from a user touch hits a plane.
        /// </summary>
        public GameObject AndyPlanePrefab;

        void Update()
        {
            if (!Input.GetKeyDown(KeyCode.D))
            {
                return;
            }
            // Get controller laser origin.
            Transform laserAnchor = NRSessionManager.Instance.NRHMDPoseTracker.centerCamera.transform;

            RaycastHit hitResult;
            if (Physics.Raycast(new Ray(laserAnchor.transform.position, laserAnchor.transform.forward), out hitResult, 10))
            {
                if (hitResult.collider.gameObject != null)
                {
                    // Instantiate Andy model at the hit point / compensate for the hit point rotation.
                    Instantiate(AndyPlanePrefab, hitResult.point, Quaternion.identity,transform);
                }
            }
        }
    }
}