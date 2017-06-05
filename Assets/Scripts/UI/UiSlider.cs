using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiSlider : MonoBehaviour {

	// Use this for initialization
	void Start () {
        var slider = gameObject.GetComponent<UnityEngine.UI.Slider>();
        slider.value = 0;
        slider.maxValue = 100;
    }
	
	// Update is called once per frame
	void Update () {
        var slider = gameObject.GetComponent<UnityEngine.UI.Slider>();

        slider.value = Time.time;
	}
}
