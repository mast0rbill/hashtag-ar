using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AddContent : MonoBehaviour {
	public GameObject additionalContent;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void onClick() {
		additionalContent.SetActive(true);
	}
}
