using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Typing : MonoBehaviour {
	
	private TouchScreenKeyboard keyboard;
	public Text inputText;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		inputText.text = keyboard.text;
		if (inputText.text == "HackToTheFuture") {
			// HARDCODING LOL
			inputText.color = Color.red;
		}
		if (inputText.text == "MIT Media Lab") {
			// LOOOOOOOL
			inputText.color = Color.black;
		}
	}

	void OnClick() {
		keyboard = TouchScreenKeyboard.Open("", TouchScreenKeyboardType.ASCIICapable);
	}
}
