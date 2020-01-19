using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HashtagSwapper : MonoBehaviour {

	public HTObjectController2 controller;
	public GameObject[] hashtagScreens;
	
	public AudioClip swapSound;

	public void SetHashtag(int i) {
		gameObject.GetComponent<AudioSource>().PlayOneShot(swapSound);

		if(!HTObjectController2.canSwapHashtag)
			return;

		foreach(GameObject go in hashtagScreens) {
			go.SetActive(false);
		}

		hashtagScreens[i].SetActive(true);

		if(i == 0) {
			controller.SetHashtag("hacktothefuture");
		} else if(i == 1) {
			controller.SetHashtag("mitmedialab");
		} else if(i == 2) {
			controller.SetHashtag("socialgood");
		}
	}
}
