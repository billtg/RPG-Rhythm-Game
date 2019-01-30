using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BeatScroller : MonoBehaviour {

    //sStatic song information
    public float beatTempo;
    public float secPerBeat;
    public AudioSource audioSource { get { return GetComponent<AudioSource>(); } }
    private SongInfo songInfo;

    //Dynamic song information
    public float songPosition;
    public float songPosInBeats;
    public float dspSongTime;
    public bool musicStarted = false;

    //static note information
    public int numberOfNotes = 200;
    public float timeSig;
    public float startDelayBeats;

    //dynamic note information

    //Track (individual buttons) variables
    [Header("Spawn Points")]
    public float[] trackSpawnPosY;
    private int len;
    private int[] trackNextIndices;
    private List<SongInfo.Track> tracks;

    //Static UI/gameplay information
    public float beatsShownInAdvance;
    public GameObject notePrefab;
    public float startLineX;
    public float finishLineX;
    public float removeLineX;
    public GameObject countDownCanvas;
    public Text countDownText;

    //Instance
    public static BeatScroller instance;
    
    // Use this for initialization
    void Start ()
    {
        instance = this;

        
        //Calculate the number of seconds per beat
        secPerBeat = 60f / beatTempo;

        //Set len to the number of tracks (should be 4, maybe 8 in the future)
        //start a nextIndex for each track and set it to 0
        len = trackSpawnPosY.Length;
        Debug.Log(len.ToString());

        trackNextIndices = new int[len];
        for (int i = 0; i < len; i++)
        {
            trackNextIndices[i] = 0;
        }

        //Initialize tracks as a list of length len
        tracks = new List<SongInfo.Track>();
        for (int i=0; i < len; i++)
        {
            tracks.Add(new SongInfo.Track());
        }

        //Populate the tracks from the song info
        //*Eventually I'll do this, but in the mean time, make a dummy set
        //tracks = songInfo.tracks;
        PopulateTracks();

        //Run the countdown preparing to start
        StartCoroutine(CountDown());
	}

	// Update is called once per frame
	void Update () {

        //Only do things if the music has started
        if (!musicStarted) return;

        //calculate the position of the song in seconds from dsp space
        songPosition = (float)(AudioSettings.dspTime - dspSongTime);

        //calculate the position in beats
        songPosInBeats = songPosition / secPerBeat;

        //Check each track to see if the next note's spawn time is on this beat
        for (int i=0; i<len; i++)
        {
            int nextIndex = trackNextIndices[i];
            SongInfo.Track currTrack = tracks[i];

            //Check that there are more notes to spawn, then determine if it's already time to spawn the next note in the notes array
            if (nextIndex < currTrack.notes.Count && currTrack.notes[nextIndex].note < (songPosInBeats + beatsShownInAdvance))
            {
                SongInfo.Note currNote = currTrack.notes[nextIndex];

                //Instantiate the current note
                //** At some point it would be better to try and make a pool of notes rather than waste process instantiating all the time
                NoteController noteController = ((GameObject)Instantiate(notePrefab)).GetComponent<NoteController>();

                //Set the note's startup conditions and let it fly
                noteController.Initialize(trackSpawnPosY[i], startLineX, finishLineX, removeLineX, 0, currNote.note);

                //Debug.Log("Spawned note with beat #: " + notes[nextIndex].ToString() + "on beat " + songPosInBeats.ToString());

                //Increase the note index
                trackNextIndices[i]++;
            }
        }
        
    }

    void StartMusic()
    {
        Debug.Log("Starting Music");
        //Record the time when the audio starts
        dspSongTime = (float)AudioSettings.dspTime;

        //start the song
        audioSource.Play();

        musicStarted = true;
    }

    IEnumerator CountDown ()
    {
        yield return new WaitForSeconds(1f);

        for (int i=0; i<3; i++)
        {
            countDownText.text = (3-i).ToString();
            yield return new WaitForSeconds(1f);
        }

        countDownCanvas.SetActive(false);
        StartMusic();

    }

    void PopulateTracks()
    {
        Debug.Log("Populating Tracks");
        Debug.Log("total of " + tracks.Count.ToString() + " tracks");
        //Create notes for each track
        for (int i=0; i<len; i++)
        {
            Debug.Log("Populating track " + i.ToString());
            tracks[i].notes = new List<SongInfo.Note>();
            //For this track, make the test notes, based on the time signature
            for (int j = 0; j < numberOfNotes; j++)
            {
                tracks[i].notes.Add(new SongInfo.Note());
                if (j == 0)
                {
                    //Make the first note occur timeSig beats after the track start, plus startDelay beats, plus i (offsets tracks)
                    tracks[i].notes[j].note = timeSig + startDelayBeats + i;
                    //Debug.Log(notes[j].ToString());
                }
                else
                {
                    //Increment the next note by one timeSig beats from the last note
                    tracks[i].notes[j].note = tracks[i].notes[j - 1].note + timeSig;
                }
                //Debug.Log("Track " + i.ToString() + " note " + j.ToString() + " beat" + tracks[i].notes[j].note.ToString());
            }
            Debug.Log("Track " + i.ToString() + " populated with " + tracks[i].notes.Count.ToString() + " notes");
        }
        Debug.Log("Tracks populated");
    }
}
