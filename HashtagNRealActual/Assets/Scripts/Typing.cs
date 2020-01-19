using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Typing : MonoBehaviour, IPointerClickHandler {
	
	public Text inputText;

	public void OnPointerClick (PointerEventData eventData) {
		GetComponent<InputField>().Select();
		Debug.LogError("HERE");
		TouchScreenKeyboard.Open("", TouchScreenKeyboardType.ASCIICapable);
    }

	/*// Use this for initialization
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
	}*/
}
