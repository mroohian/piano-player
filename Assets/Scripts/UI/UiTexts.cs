using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiTexts : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        var text = GetComponent<UnityEngine.UI.Text>();

        text.text = "Score: 0";
    }
}
