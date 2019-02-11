using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BeatScroller : MonoBehaviour {

    //sStatic song information
    [Header("Song Info")]
    public float beatTempo;
    public float secPerBeat;
    public float offsetToFirstBeat;
    public AudioSource audioSource { get { return GetComponent<AudioSource>(); } }
    private SongInfo songInfo;

    //Dynamic song information
    public float songPosition;
    public float songPosInBeats;
    public float dspSongTime;
    public bool musicStarted = false;
    public Text InfoText;

    //static note information
    [Header("Note Info")]
    public int numberOfNotes = 200;
    public float timeSig;
    public float startDelayBeats;

    //Fill information
    [Header("Fill Information")]
    public bool activateFill;
    public KeyCode fillButton;
    private List<SongInfo.Track> fillTracks;
    public float fillOffset;
    private int[] trackNextFillIndices;
    public GameObject fillPrefab;
    public int fillMeter;
    public int fillMeterMax;
    public int fillIncrementUp;
    public int fillIncrementDown;
    public FillBar fillBar;
    //public bool activateSolo;
    //public KeyCode soloButton;

    //Track (individual buttons) variables
    [Header("Spawn Points")]
    public float[] trackSpawnPosY;
    private int len;
    private int[] trackNextIndices;
    private List<SongInfo.Track> tracks;

    //Static UI/gameplay information
    [Header("Gameplay/UI")]
    public float beatsShownInAdvance;
    public GameObject notePrefab;
    public float startLineX;
    public float finishLineX;
    public float removeLineX;
    public GameObject countDownCanvas;
    public Text countDownText;
    public GameObject gameOverCanvas;

    //Dynamic UI/Gameplay info
    public int score;
    public int multiplier = 1;
    public int comboCount = 0;
    public Text scoreText;
    public Text multiplierText;
    public Text accuracyText;
    public Text comboText;
    public Text winText;
    public Text fillText;

    //Instance
    public static BeatScroller instance;
    
    // Use this for initialization
    void Start ()
    {
        instance = this;

        //Reset Score and UI objects.
        multiplier = 1;
        scoreText.text = "Score: 0";
        multiplierText.text = "Multipler: x1";
        fillText.text = "Fill: " + fillMeter;
        fillBar.UpdateScale(fillMeter/fillMeterMax);
        accuracyText.text = null;
        comboText.text = null;
        score = 0;

        //Calculate the number of seconds per beat
        secPerBeat = 60f / beatTempo;

        //Set len to the number of tracks (should be 4, maybe 8 in the future)
        //start a nextIndex for each track and set it to 0
        len = trackSpawnPosY.Length;

        trackNextIndices = new int[len];
        trackNextFillIndices = new int[len];
        for (int i = 0; i < len; i++)
        {
            trackNextIndices[i] = 0;
            trackNextFillIndices[i] = 0;
        }

        //Initialize tracks and fillTracks as a list of length len
        tracks = new List<SongInfo.Track>();
        fillTracks = new List<SongInfo.Track>();
        for (int i=0; i < len; i++)
        {
            tracks.Add(new SongInfo.Track());
            fillTracks.Add(new SongInfo.Track());
        }


        //Populate the tracks from the song info
        //*Eventually I'll do this, but in the mean time, make a dummy set
        //tracks = songInfo.tracks;
        PopulateTracks();
        PopulateFills();

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
        
        //Check if solo or fill needs activating
        if (Input.GetKeyDown(fillButton) && fillMeter == fillMeterMax)
        {
            Debug.Log("Fill activated!");
            activateFill = true;
            InfoText.text = "Fill Activated!";
        }

        //Check each track to see if the next note's spawn time is on this beat
        SpawnNotes();
        SpawnFills();

    }

    void StartMusic()
    {
        Debug.Log("Starting Music");
        //Record the time when the audio starts
        dspSongTime = (float)AudioSettings.dspTime + offsetToFirstBeat;

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

    void SpawnNotes()
    {
        for (int i = 0; i < len; i++)
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
                noteController.Initialize(trackSpawnPosY[i], startLineX, finishLineX, removeLineX, 0, currNote.note, InputController.instance.keyBindings[i], i);

                //Debug.Log("Spawned note with beat #: " + notes[nextIndex].ToString() + "on beat " + songPosInBeats.ToString());

                //Increase the note index
                trackNextIndices[i]++;
            }
        }
    }

    void SpawnFills()
    {
        //do for each fill track
        for (int i = 0; i < len; i++)
        {
            int nextIndex = trackNextFillIndices[i];
            SongInfo.Track currTrack = fillTracks[i];

            //Check that there are more notes to spawn, then determine if it's already time to spawn the next note in the notes array
            if (nextIndex < currTrack.notes.Count && currTrack.notes[nextIndex].note < (songPosInBeats + beatsShownInAdvance))
            {
                if (activateFill)
                {
                    //Spawn a fill note
                    SongInfo.Note currNote = currTrack.notes[nextIndex];

                    //Instantiate the current note
                    //** At some point it would be better to try and make a pool of notes rather than waste process instantiating all the time
                    NoteController noteController = ((GameObject)Instantiate(fillPrefab)).GetComponent<NoteController>();

                    //Set the note's startup conditions and let it fly
                    noteController.Initialize(trackSpawnPosY[i], startLineX, finishLineX, removeLineX, 0, currNote.note, InputController.instance.keyBindings[i], i);

                    //Debug.Log("Spawned note with beat #: " + notes[nextIndex].ToString() + "on beat " + songPosInBeats.ToString());
                    //Reduce the fill meter
                    ReduceFill();

                }
                //Increase the note index
                trackNextFillIndices[i]++;
            }
        }
    }

    public void NoteHit(int noteScore, string noteAccuracy)
    {
        //Debug.Log("Note hit BeatScroller");
        score += noteScore * multiplier;
        scoreText.text = "Score: " + score.ToString();
        accuracyText.text = noteAccuracy;
        comboCount++;
        if (comboCount > 5)
        {
            comboText.text = "Combo: \n" + comboCount.ToString(); 
        }
        UpgradeMultiplier();
        MaskMover.instance.MoveBackward();
        if (!activateFill)
        {
            AddFill();
            UpdateFillText();
        }
    }
    public void MissHit()
    {
        Debug.Log("Missed!");
        accuracyText.text = "Missed!";
        multiplier = 1;
        multiplierText.text = "Multiplier: x1";
        comboCount = 0;
        comboText.text = null;
        MaskMover.instance.MoveForward();
    }

    void UpgradeMultiplier()
    {
        if (comboCount == 5)
        {
            multiplier = 2;
        }
        if (comboCount == 15)
        {
            multiplier = 3;
        }
        if (comboCount == 40)
        {
            multiplier = 4;
        }
        multiplierText.text = "Multiplier: x" + multiplier.ToString();
    }

    public void StopGame(bool winCondition)
    {
        gameOverCanvas.SetActive(true);
        if (winCondition)
        {
            Debug.Log("Game won");
            //audioSource.Stop();
            winText.gameObject.SetActive(true);
            accuracyText.gameObject.SetActive(false);
            winText.text = "You \nWin!";
            musicStarted = false;
        }
        else
        {
            Debug.Log("Game lost");
            //audioSource.Stop();
            winText.gameObject.SetActive(true);
            accuracyText.gameObject.SetActive(false);
            winText.text = "You \nLose!";
            musicStarted = false;
        }
    }

    void ReduceFill()
    {
        if (fillMeter > fillIncrementDown)
        {
            //Reduce the fill meter if it's not going to be zero by reducing it.
            fillMeter -= fillIncrementDown;
            UpdateFillText();
        }
        else
        {
            fillMeter = 0;
            activateFill = false;
            InfoText.text = null;
            UpdateFillText();
        }
    }

    void AddFill()
    {
        //Add to the fill meter if it's not maxed
        if (fillMeter < fillMeterMax)
        {
            //Add fill increment times the multiplier
            if (fillMeterMax - fillMeter < fillIncrementUp*multiplier)
            {
                Debug.Log("Max Fill!");
                fillMeter = fillMeterMax;
                UpdateFillText();
                InfoText.text = "Fill Ready!\n Press " + fillButton.ToString();
            }
            else
            {
                Debug.Log("Upping Fill Meter");
                fillMeter += (fillIncrementUp*multiplier);
                UpdateFillText();
            }
        }
    }

    void UpdateFillText()
    {
        fillText.text = "Fill: " + fillMeter;
        fillBar.UpdateScale((float)fillMeter/fillMeterMax);
    }

    void PopulateTracks()
    {
        //Debug.Log("Populating Tracks");
        //Debug.Log("total of " + tracks.Count.ToString() + " tracks");
        //Create notes for each track
        for (int i=0; i<len; i++)
        {
            //Debug.Log("Populating track " + i.ToString());
            tracks[i].notes = new List<SongInfo.Note>();
            //For this track, make the test notes, based on the time signature
            for (int j = 0; j < numberOfNotes; j++)
            {
                tracks[i].notes.Add(new SongInfo.Note());
                if (j == 0)
                {
                    //Make the first note occur timeSig beats after the track start, plus startDelay beats, plus i (offsets tracks)
                    tracks[i].notes[j].note = timeSig + startDelayBeats + timeSig*i/tracks.Count;
                    //Debug.Log(notes[j].ToString());
                }
                else
                {
                    //Increment the next note by one timeSig beats from the last note
                    tracks[i].notes[j].note = tracks[i].notes[j - 1].note + timeSig;
                }
                //Debug.Log("Track " + i.ToString() + " note " + j.ToString() + " beat" + tracks[i].notes[j].note.ToString());
            }
            //Debug.Log("Track " + i.ToString() + " populated with " + tracks[i].notes.Count.ToString() + " notes");
        }
        //Debug.Log("Tracks populated");
    }

    void PopulateFills()
    {
        //Debug.Log("Populating Fill Tracks");
        //Debug.Log("total of " + fillTracks.Count.ToString() + " tracks");
        //Create notes for each track
        //i refers to the track, j refers to the note in that track
        for (int i = 0; i < len; i++)
        {
            //Debug.Log("Populating track " + i.ToString());
            fillTracks[i].notes = new List<SongInfo.Note>();
            //For this track, make the test notes, based on the time signature
            for (int j = 0; j < numberOfNotes; j++)
            {
                fillTracks[i].notes.Add(new SongInfo.Note());
                if (j == 0)
                {
                    //Make the first note occur timeSig beats after the track start, plus startDelay beats, plus i (offsets tracks)
                    //***NOTE, for fill I added two beats to each to offset them from the regular tracks.
                    fillTracks[i].notes[j].note = timeSig + startDelayBeats + timeSig * i / fillTracks.Count + fillOffset;
                    //Debug.Log(notes[j].ToString());
                }
                else
                {
                    //Increment the next note by one timeSig beats from the last note
                    fillTracks[i].notes[j].note = fillTracks[i].notes[j - 1].note + timeSig;
                }
                Debug.Log("Track " + i.ToString() + " note " + j.ToString() + " beat" + tracks[i].notes[j].note.ToString());
            }
            //Debug.Log("Track " + i.ToString() + " populated with " + tracks[i].notes.Count.ToString() + " notes");
        }
        //Debug.Log("Tracks populated");
    }

    
}
