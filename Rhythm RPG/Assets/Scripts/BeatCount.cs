using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class BeatCount : MonoBehaviour {

    private Text beatText;
    private void Start()
    {
        beatText = GetComponent<Text>();
        beatText.text = null;
    }
    // Update is called once per frame
    void Update () {
        beatText.text = BeatScroller.instance.songPosInBeats.ToString("#.0");
	}
}
