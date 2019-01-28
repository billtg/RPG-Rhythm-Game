using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SongManager : MonoBehaviour {

    public float bpm;
    public float beatsShownInAdvance;
    public GameObject noteObject;
    public float songPosInBeats;

    private float[] notes = { 10f, 20f, 25f, 30f, 35f, 45f };
    private int nextIndex = 0;

    private float songPosition;
    private float secPerBeat;
    private float dspSongTime;

    public static SongManager instance;

	// Use this for initialization
	void Start () {
        instance = this;
        //Calculate how many seconds in one beat
        secPerBeat = 60f / bpm;

        //Record the time when the song starts
        dspSongTime = (float)AudioSettings.dspTime;

        //start the song
        GetComponent<AudioSource>().Play();
	}
	
	// Update is called once per frame
	void Update () {
        //Calculate the position of the song in seconds
        songPosition = (float)(AudioSettings.dspTime - dspSongTime);

        //Calculate the position of the song in beats
        songPosInBeats = songPosition / secPerBeat;

        if (nextIndex < notes.Length && notes[nextIndex] < songPosInBeats + beatsShownInAdvance)
        {
            Instantiate(noteObject);

            //initialize the fields of the music note

            nextIndex++;

        }
	}
}
