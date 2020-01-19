using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextBoxController : MonoBehaviour {

	public TextMeshPro textMeshPro;
	private Vector3 targetPos;
	private bool reached = false;
	public GameObject[] hashtags;
	public Renderer backRend;
	public Material[] mats;

	public void SetText(string t, Vector3 targetPos, int curHashtag) {
		textMeshPro.text = t;
		this.targetPos = targetPos;

		foreach(GameObject g in hashtags) {
			g.SetActive(false);
		}

		if(curHashtag != 3)
			hashtags[curHashtag].SetActive(true);
		backRend.material = mats[curHashtag];
	}

	void Update() {
		gameObject.transform.position = Vector3.Lerp(gameObject.transform.position, targetPos, Time.deltaTime * 6f);
		if((gameObject.transform.position - targetPos).sqrMagnitude <= 0.1f) {
			if(gameObject.GetComponent<BobUpAndDown>() != null) 
				gameObject.GetComponent<BobUpAndDown>().Initialize();
			enabled = false;
		}
	}
}
