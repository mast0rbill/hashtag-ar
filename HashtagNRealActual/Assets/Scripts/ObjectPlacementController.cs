using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NRKernal;
using UnityEngine.UI;

public class ObjectPlacementController : MonoBehaviour {
	public GameObject textPrefab;
	public AudioClip placeClip;
	public InputField inputField;

	void Update () {
		if (!NRInput.GetButtonDown(ControllerButton.TRIGGER))
        {
            return;
        }
		
		Vector3 pos = NRSessionManager.Instance.NRHMDPoseTracker.centerCamera.transform.position;
		Quaternion rot = Quaternion.LookRotation(-NRSessionManager.Instance.NRHMDPoseTracker.centerCamera.transform.forward, Vector3.up);
		GetComponent<AudioSource>().PlayOneShot(placeClip);
		GameObject go = Instantiate(textPrefab, pos, rot);
		string toSend = inputField.text == "" ? "Hello!" : inputField.text;
		go.GetComponent<TextBoxController>().SetText(toSend, NRSessionManager.Instance.NRHMDPoseTracker.centerCamera.transform.position + NRSessionManager.Instance.NRHMDPoseTracker.centerCamera.transform.forward * 2f, HTObjectController2.currentHashtag);
		go.transform.parent = HTObjectController2.currentHashtagParent.transform;
		inputField.text = "";
	}
}
