using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[System.Serializable]
public struct IDDto {
	public string oid;
}

[System.Serializable]
public struct HashtagObject {
	public string location;
	public string hashtag;
	public string objType;
	public string source;
}

public class HashtagAPI : MonoBehaviour {

	public static HashtagAPI instance;

	void Awake() {
		if(instance == null) {
			instance = this;
		}
	}

	public void GetObjects(string location, string hashtag, Action<HashtagObject[]> then) {
		StartCoroutine(GetRoutine(location, hashtag, then));
	}

	[System.Serializable]
	struct HashtagArrayWrapper {
		public HashtagObject[] objs;
	}

	private IEnumerator GetRoutine(string location, string hashtag, Action<HashtagObject[]> then) {
		UnityWebRequest req = UnityWebRequest.Get("https://hashtag-vr-api.azurewebsites.net/api/ObjectController");
		req.SetRequestHeader("location", location);
		req.SetRequestHeader("hashtag", hashtag);

		yield return req.SendWebRequest();

		if(req.isNetworkError) {
			Debug.LogError(req.error);
		} else {
			Debug.LogError(req.downloadHandler.text);
			string formattedResp = $"{{\"objs\":{req.downloadHandler.text}}}";
			HashtagObject[] hashtagObjs = ((HashtagArrayWrapper)JsonUtility.FromJson<HashtagArrayWrapper>(formattedResp)).objs;
			then(hashtagObjs);
		}
	}

	public void PostObject(string location, string hashtag, string objType, string source, Action<string> onError) {
		StartCoroutine(PostRoutine(location, hashtag, objType, source, onError));
	}

	private IEnumerator PostRoutine(string location, string hashtag, string objType, string source, Action<string> onError) {
		UnityWebRequest req = UnityWebRequest.Post("https://hashtag-vr-api.azurewebsites.net/api/ObjectController", "");
		req.SetRequestHeader("location", location);
		req.SetRequestHeader("hashtag", hashtag);
		req.SetRequestHeader("objType", objType);
		req.SetRequestHeader("source", source);

		yield return req.SendWebRequest();

		if(req.isNetworkError) {
			onError(req.error);
		} else {
			Debug.Log("Post!");
		}
	}
}
