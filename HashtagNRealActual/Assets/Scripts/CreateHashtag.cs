using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateHashtag : MonoBehaviour {
	public GameObject welcomeScreen;
	public GameObject createHashtagScreen;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	//switches screen to add hashtag
	public void AddHashtag() {
		Debug.Log("Clicked button! Attempting to switch screen");
		welcomeScreen.SetActive(false);
		createHashtagScreen.SetActive(true);
	}
}
