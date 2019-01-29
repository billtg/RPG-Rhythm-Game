using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeatScroller : MonoBehaviour {

    //sStatic song information
    public float beatTempo;
    public float secPerBeat;
    public AudioSource audioSource { get { return GetComponent<AudioSource>(); } }

    //Dynamic song information
    public float songPosition;
    public float songPosInBeats;
    public float dspSongTime;

    //static note information
    public float timeSig;
    private float[] notes = new float[200];

    //dynamic note information
    int nextIndex = 0;

    //Static UI/gameplay information
    public float beatsShownInAdvance;
    public GameObject notePrefab;
    public float startLineX;
    public float finishLineX;
    public float removeLineX;

    //Instance
    public static BeatScroller instance;
    
    // Use this for initialization
    void Start ()
    {
        instance = this;

        //Make the test notes, based on the time signature
        for (int i=0; i < notes.Length; i++)
        {
            if (i == 0)
            {
                notes[i] = timeSig;
            }
            else
            {
                notes[i] = notes[i - 1] + timeSig;
            }
        }
        //Calculate the number of seconds per beat
        secPerBeat = 60f / beatTempo;


        //Record the time when the audio starts
        dspSongTime = (float)AudioSettings.dspTime;

        //start the song
        audioSource.Play();
	}
	
	// Update is called once per frame
	void Update () {
        //calculate the position of the song in seconds from dsp space
        songPosition = (float)(AudioSettings.dspTime - dspSongTime);

        //calculate the position in beats
        songPosInBeats = songPosition / secPerBeat;


        //Check that there are more notes to spawn, then determine if it's already time to spawn the next note in the notes array
        if (nextIndex < notes.Length && notes[nextIndex] < (songPosInBeats + beatsShownInAdvance))
        {
            //Create a note at the startLine
            //*I used 5 right now for simplicity, but need to update to 
            //Instantiate(notePrefab, new Vector3(startLineX, 5.5f, 0f), Quaternion.identity);

            //Try this. Gotta tweak
            NoteController noteController = ((GameObject)Instantiate(notePrefab)).GetComponent<NoteController>();
            noteController.Initialize(5.5f, startLineX, finishLineX, removeLineX, 0, notes[nextIndex]);


            //Increase the note index
            nextIndex++;
        }
	}
}
