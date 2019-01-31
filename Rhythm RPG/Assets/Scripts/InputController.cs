﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : MonoBehaviour {

    //Track info
    //cache the number of tracks
    private int trackLength;

    //Button input configuration
    public List<KeyCode> keyBindings;
    public List<ButtonController> buttons;

    public static InputController instance;

    // Use this for initialization
    void Start () {
        instance = this;

        //Assign keybindings from InputManager
        //* For now just make it easy
        keyBindings = new List<KeyCode>();
        keyBindings.Add(KeyCode.UpArrow);
        keyBindings.Add(KeyCode.LeftArrow);
        keyBindings.Add(KeyCode.RightArrow);
        keyBindings.Add(KeyCode.DownArrow);
        keyBindings.Add(KeyCode.Escape);

        trackLength = keyBindings.Count;
        Debug.Log("IC Tracks: " + trackLength.ToString());
    }
	
	// Update is called once per frame
	void Update () {
        for (int i = 0; i < trackLength; i++)
        {
            if (Input.GetKeyDown(keyBindings[i]))
            {
                buttons[i].Pressed();
            }
            if (Input.GetKeyUp(keyBindings[i]))
            {
                buttons[i].UnPressed();
            }
        }
    }

    void Inputted(int i)
    {
        //inform Conductor and other interested classes
        //if (inputtedEvent != null) inputtedEvent(i);
       
    }
}
