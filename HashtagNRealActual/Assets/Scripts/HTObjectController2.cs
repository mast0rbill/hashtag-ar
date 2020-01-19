using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HTObjectController2 : MonoBehaviour {

	bool init = false;
	bool lastTracking = false;
	public Transform worldParent;

	public GameObject[] worldMap;

	void Start() {
		worldParent.gameObject.SetActive(false);
	}

	void Update() {
		if(ImageTrackerTest.worldAnchor == null)
			return;

		if(!init) {
			worldParent.gameObject.SetActive(true);
			init = true;
			worldParent.position = new Vector3(ImageTrackerTest.worldAnchor.position.x, 0f, ImageTrackerTest.worldAnchor.position.z);
			worldParent.rotation = Quaternion.Euler(worldParent.eulerAngles.x, ImageTrackerTest.worldAnchor.eulerAngles.y, worldParent.eulerAngles.z);
			SetHashtag("hacktothefuture");	
		}
	}

	public void SetHashtag(string ht) {
		foreach(GameObject o in worldMap) {
			o.SetActive(false);
		}

		if(ht.ToLower() == "hacktothefuture") {
			worldMap[0].SetActive(true);
		} else if(ht.ToLower() == "mitmedialab") {
			worldMap[1].SetActive(true);
		} else if(ht.ToLower() == "socialgood") {
			worldMap[2].SetActive(true);
		}
	}
	
}
