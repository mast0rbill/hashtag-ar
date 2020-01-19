﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextBoxController : MonoBehaviour {

	public TextMeshPro textMeshPro;
	private Vector3 targetPos;
	private bool reached = false;

	public void SetText(string t, Vector3 targetPos) {
		textMeshPro.text = t;
		this.targetPos = targetPos;
	}

	void Update() {
		gameObject.transform.position = Vector3.Lerp(gameObject.transform.position, targetPos, Time.deltaTime * 12f);
		if((gameObject.transform.position - targetPos).sqrMagnitude <= 0.1f) {
			if(gameObject.GetComponent<BobUpAndDown>() != null) 
				gameObject.GetComponent<BobUpAndDown>().Initialize();
			enabled = false;
		}
	}
}
