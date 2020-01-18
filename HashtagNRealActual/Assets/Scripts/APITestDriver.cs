using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class APITestDriver : MonoBehaviour {
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.T)) {
			HashtagAPI.instance.GetObjects("dummyLoc", "testTag", OnObjectRetrieval);
		}
		if(Input.GetKeyDown(KeyCode.J)) {
			HashtagAPI.instance.PostObject("dummyLoc", "testTag", "image", "dummySource", OnPostError);
		}
	}

	public void OnObjectRetrieval(HashtagObject[] objs) {
		foreach(var obj in objs) {
			Debug.Log(obj.location);
		}
	}

	public void OnPostError(string e) {
		Debug.LogError(e);
	}
}
