using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NRKernal;

public class ObjectPlacementController : MonoBehaviour {
	public GameObject textPrefab;
	public AudioClip placeClip;

	void Update () {
		if (!NRInput.GetButtonDown(ControllerButton.TRIGGER))
        {
            return;
        }
		
		Vector3 pos = NRSessionManager.Instance.NRHMDPoseTracker.centerCamera.transform.position;
		Quaternion rot = Quaternion.LookRotation(-NRSessionManager.Instance.NRHMDPoseTracker.centerCamera.transform.forward, Vector3.up);
		GetComponent<AudioSource>().PlayOneShot(placeClip);
		GameObject go = Instantiate(textPrefab, pos, rot);
		go.GetComponent<TextBoxController>().SetText("Test text 123456", NRSessionManager.Instance.NRHMDPoseTracker.centerCamera.transform.position + NRSessionManager.Instance.NRHMDPoseTracker.centerCamera.transform.forward * 2f);
	}
}
