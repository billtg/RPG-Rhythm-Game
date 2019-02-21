using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EditorConductor : MonoBehaviour {

    public static EditorConductor instance;

    //Song Information
    [Header("Song Information")]
    public float beatTempo;
    public float secPerBeat;
    public float offsetToFirstBeat;
    public float beatDelay;
    public AudioSource audioSource { get { return GetComponent<AudioSource>(); } }
    public SongInfo songInfo;
    //Dynamic song information
    public float songPosition;
    public float songPosInBeats;
    public float dspSongTime;
    public float startingBeat;
    public bool musicStarted = false;

    [Header("Note and Note tracking")]
    public int[] trackNextIndices;
    public float beatsShownInAdvance;
    public GameObject notePrefab;
    public float startLineX;
    public float finishLineX;
    public float removeLineX;
    public float[] trackSpawnPosY;

    [Header("Grid Spawning")]
    public GameObject barPrefab;
    public GameObject halfBarPrefab;
    public float barsSpawned;

    //UI
    [Header("UI")]
    public Text countDownText;
    public Text accuracyText;
    public Text currentBeatText;

    //Inputs
    [Header("Inputs")]
    public KeyCode fillButton;
    public KeyCode stopSong;




    // Use this for initialization
    void Start() {
        instance = this;

        accuracyText.text = null;
        countDownText.text = null;

        //Load and calculate song information
        songInfo = SongEditor.instance.songInfo;
        beatTempo = songInfo.bpm;
        secPerBeat = 60f / beatTempo;
        offsetToFirstBeat = songInfo.songOffset;
        beatDelay = songInfo.startDelayBeats;
        audioSource.clip = songInfo.song;

        //Initialize the tracking indexes for the note lists
        trackNextIndices = new int[songInfo.tracks.Length];

        //Reset the barsSpawned
        barsSpawned = 0;

    }

    // Update is called once per frame
    void Update()
    {
        //Only do things if the music has started
        if (!musicStarted) return;

        if (Input.GetKeyDown(stopSong))
        {
            StopSong();
        }

        //calculate the position of the song in seconds from dsp space
        songPosition = (float)(AudioSettings.dspTime - dspSongTime + startingBeat * secPerBeat);

        //calculate the position in beats
        songPosInBeats = songPosition / secPerBeat - beatDelay;

        //Check if solo or fill needs activating
        //if (Input.GetKeyDown(fillButton) && fillMeter == fillMeterMax)
        //{
        //    Debug.Log("Fill activated!");
        //    activateFill = true;
        //    InfoText.text = "Fill Activated!";
        //}

        //Check each track to see if the next note's spawn time is on this beat
        SpawnNotes();
        SpawnGrid();
        //SpawnFills();
        
        //update the beat
        currentBeatText.text = (songPosInBeats).ToString("#.0");
    }

    IEnumerator CountDown(float beatToStartAt)
    {
        yield return new WaitForSeconds(1f);

        for (int i = 0; i < 2; i++)
        {
            countDownText.text = (2 - i).ToString();
            yield return new WaitForSeconds(1f);
        }

        countDownText.text = null;
        StartMusic();

    }

    void StartMusic()
    {
        Debug.Log("Starting Music at beat: " + startingBeat.ToString());
        //Record the time when the audio starts
        dspSongTime = (float)AudioSettings.dspTime + offsetToFirstBeat;

        //start the song on the beat specified
        audioSource.time = startingBeat*secPerBeat;
        audioSource.Play();

        musicStarted = true;
    }

    void StopSong()
    {
        Debug.Log("Stopping Song");
        musicStarted = false;
        audioSource.Stop();
        SongEditor.instance.isPlaying = false;
        SongEditor.instance.UpdateSongPosition();
        //Reset the barsSpawned
        barsSpawned = 0;
        currentBeatText.text = SongEditor.instance.currentBeat.ToString();
    }

    private void SpawnNotes()
    {
        for (int i = 0; i < songInfo.tracks.Length; i++)
        {
            int nextIndex = trackNextIndices[i];
            SongInfo.Track currTrack = songInfo.tracks[i];

            //Check that there are more notes to spawn, then determine if it's already time to spawn the next note in the notes array
            if (nextIndex < currTrack.notes.Count && currTrack.notes[nextIndex].note < (songPosInBeats + beatsShownInAdvance))
            {
                SongInfo.Note currNote = currTrack.notes[nextIndex];

                //Instantiate the current note
                //** At some point it would be better to try and make a pool of notes rather than waste process instantiating all the time
                NoteControllerEditor noteControllerEditor = ((GameObject)Instantiate(notePrefab)).GetComponent<NoteControllerEditor>();

                //Set the note's startup conditions and let it fly
                noteControllerEditor.Initialize(trackSpawnPosY[i], startLineX, finishLineX, removeLineX, 0, currNote.note, InputController.instance.keyBindings[i], i);

                //Debug.Log("Spawned note with beat #: " + notes[nextIndex].ToString() + "on beat " + songPosInBeats.ToString());

                //Increase the note index
                trackNextIndices[i]++;
            }
        }
    }

    private void SpawnFills()
    {

    }

    private void SpawnGrid()
    {
        if (songPosInBeats + beatsShownInAdvance > barsSpawned)
        {
            //Spawn a bar
            //Debug.Log("spawning bar on beat: " + songPosInBeats.ToString() + " for beat: " + (songPosInBeats + beatsShownInAdvance).ToString());
            //Spawn a bar
            BarController barController = ((GameObject)Instantiate(barPrefab)).GetComponent<BarController>();
            barController.Initialize(0, startLineX, finishLineX, removeLineX, barsSpawned, false);
            //Spawn a half bar
            //BarController halfBar = ((GameObject)Instantiate(halfBarPrefab)).GetComponent<BarController>();
            //barController.Initialize(0, startLineX, finishLineX, removeLineX, (barsSpawned+0.5f), true);
            barsSpawned += 1;
        }
    }

    private void DetermineTrackNoteIndices()
    {
        for (int i = 0; i < trackNextIndices.Length; i++)
        {
            //Figure out the indice of the next note that should be spawned on this track
            if (songInfo.tracks[i].notes.Count == 0)
            {
                Debug.Log("No notes in this track. Indice is 0 but doesn't matter");
                trackNextIndices[i] = 0;
            }
            else
            {
                Debug.Log("Checking notes in track " + i.ToString());
                bool foundNote = false;
                //Check the beat of each note to see if it occurs after the current beat.
                for (int j = 0; j< songInfo.tracks[i].notes.Count; j++)
                {
                    if (songInfo.tracks[i].notes[j].note > SongEditor.instance.currentBeat && !foundNote)
                    {
                        Debug.Log("Found the next playable note in track " + i.ToString());
                        trackNextIndices[i] = j;
                        foundNote = true;
                    }
                }
            }
        }
    }

    public void PlayFromCurrent()
    {
        Debug.Log("conductor playing from current beat");
        DetermineTrackNoteIndices();
        SongEditor.instance.ClearGrid();
        SongEditor.instance.ClearNotes();
        SongEditor.instance.ClearFills();
        startingBeat = SongEditor.instance.currentBeat;
        StartCoroutine(CountDown(SongEditor.instance.currentBeat));
    }

    public void PlayFromStart()
    {
        Debug.Log("Playing from start");
        //Set all track indices to 0;
        for (int i = 0; i < songInfo.tracks.Length; i++)
        {
            trackNextIndices[i] = 0;
        }
        SongEditor.instance.ClearGrid();
        SongEditor.instance.ClearNotes();
        SongEditor.instance.ClearFills();
        startingBeat = 0f;
        StartCoroutine(CountDown(0f));
    }

    public void NoteHit(int noteScore, string noteAccuracy)
    {
        //Debug.Log("Note hit BeatScroller");
        accuracyText.text = noteAccuracy;
        ////comboCount++;
        //if (comboCount > 5)
        //{
        //    comboText.text = "Combo: \n" + comboCount.ToString();
        //}
        //UpgradeMultiplier();
        //MaskMover.instance.MoveBackward();
        //if (!activateFill)
        //{
        //    AddFill();
        //    UpdateFillText();
        //}
    }

    public void MissHit()
    {
        Debug.Log("Missed!");
        accuracyText.text = "Missed!";
        //comboCount = 0;
        //comboText.text = null;
        //MaskMover.instance.MoveForward();
    }
}
