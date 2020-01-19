using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BobUpAndDown : MonoBehaviour {

	public float startY;
	public bool init = false;

	public void Initialize() {
		init = true;
		startY = transform.position.y;
	}
	
	// Update is called once per frame
	void Update () {
		if(!init)
			return;
		
		float sin = Mathf.Sin(Time.time * 3f) * 0.02f;
		transform.position = new Vector3(transform.position.x, startY + sin, transform.position.z);
	}
}
