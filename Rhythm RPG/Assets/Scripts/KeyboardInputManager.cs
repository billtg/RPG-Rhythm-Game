using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardInputManager : MonoBehaviour {

    //singleton
    public static KeyboardInputManager instance;

    //Binding
    public enum KeyBindings { Track1 = 0, Track2, Track3, Track4, Pause };
    private const string Default1 = "UpArrow";
    private const string Default2 = "LeftArrow";
    private const string Default3 = "RightArrow";
    private const string Default4 = "DownArrow";
    private const string DefaultPause = "Escape";
    private List<KeyCode> keys;

    // Use this for initialization
    void Awake () {
        //singleton
        if (instance == null)
        {
            DontDestroyOnLoad(gameObject);
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        //configure the key according to player prefs
        keys = new List<KeyCode>();
        //keys.Add(KeyBindings.Track1.ToString());

        //This is the original set-up. It may be important over time
        //keys[(int)KeyBindings.Track1] = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString(KeyBindings.Track1.ToString(), Default1));
        //keys[(int)KeyBindings.Track2] = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString(KeyBindings.Track2.ToString(), Default2));
        //keys[(int)KeyBindings.Track3] = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString(KeyBindings.Track3.ToString(), Default3));
        //keys[(int)KeyBindings.Track4] = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString(KeyBindings.Track4.ToString(), Default4));
        //keys[(int)KeyBindings.Pause] = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString(KeyBindings.Pause.ToString(), DefaultPause));

    }

    // Update is called once per frame
    void Update () {
		
	}
}
