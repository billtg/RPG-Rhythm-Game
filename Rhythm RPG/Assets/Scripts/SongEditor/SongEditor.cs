using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SongEditor : MonoBehaviour {

    //Button information
    [Header("Buttons")]
    public InputController inputController;
    public float[] trackHeights = new float[4];

    //Song information
    [Header("Song Information")]
    public SongInfo songInfo;
    public Text songInfoText;
    public float bpm;

    //note information
    [Header("Note Information")]
    public GameObject arrowCursor;
    public GameObject arrowPrefab;
    public GameObject fillCursor;
    public GameObject fillPrefab;
    public GameObject activeCursor;

    //UI/Editor controls
    [Header("Editor Controls")]
    public float snapAccuracyY;
    public float snapAccuracyX;
    public bool onTrack;
    public Vector3 mouseVector;
    public Vector3 snapVector;
    public float currentBeat;
    public Text currentBeatText;
    public float beatSpacing;
    public GameObject beatBars;
    public GameObject halfBars;
    float screenSizeGameSpace;
    int numBars;
    public KeyCode fillButton;

    //Note

    // Use this for initialization
    void Start () {
        songInfoText.text = "Song: " + songInfo.songTitle + "\nBPM: " + songInfo.bpm.ToString();
        bpm = songInfo.bpm;
        currentBeat = 0;
        currentBeatText.text = currentBeat.ToString();

        //Set up the bar space
        screenSizeGameSpace = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, 0, 0)).x * 2;
        numBars = Mathf.RoundToInt(screenSizeGameSpace / beatSpacing);
        Debug.Log("Screen is " + screenSizeGameSpace.ToString() + " units wide, with room for " + numBars.ToString() + "beat bars");


        //Get track heights
        for (int i=0; i< 4; i++)
        {
            ButtonController button = inputController.buttons[i];
            //Debug.Log(button.gameObject.transform.position.y.ToString());
            trackHeights[i] = button.gameObject.transform.position.y;
        }

        //Draw everything
        UpdateSongPosition();

        //Set up the cursor
        activeCursor = arrowCursor;
        fillCursor.transform.position = new Vector3(-10, 0, 0);
    }
	
	// Update is called once per frame
	void Update () {
        SnapNotes();

        //Change the beat on mouse scrolling
        if (Input.mouseScrollDelta.y != 0)
        {
            //Debug.Log("Scrolling: " + Input.mouseScrollDelta.y.ToString());
            if (Input.mouseScrollDelta.y < 0)
            {
                if (currentBeat > 0)
                {
                    currentBeat--;
                    UpdateSongPosition();
                }
            }
            else
            {
                currentBeat++;
                UpdateSongPosition();
            }
        }

        //On click, add the arrow to the song
        if (Input.GetMouseButtonDown(0) && !Input.GetKey(fillButton))
        {
            AddArrow();
        }

        //On right-click, remove the arrow from the song
        if (Input.GetMouseButtonDown(1) && !Input.GetKey(fillButton))
        {
            RemoveArrow();
        }

        //Holding fill button, add fill arrows on click
        if (Input.GetKey(fillButton) && Input.GetMouseButtonDown(0))
        {
            AddFill();
        }
        //holding fill button, remove fill arrows on click
        if (Input.GetKey(fillButton) && Input.GetMouseButtonDown(1))
        {
            RemoveFill();
        }
        //Change the cursor on fillButton press
        if (Input.GetKeyDown(fillButton))
        {
            activeCursor = fillCursor;
            arrowCursor.transform.position = new Vector3(-10, 0, 0);
        }
        //reset cursor when fill button is released
        if (Input.GetKeyUp(fillButton))
        {
            activeCursor = arrowCursor;
            fillCursor.transform.position = new Vector3(-10, 0, 0);
        }
	}

    void AddArrow()
    {
        if (onTrack)
        {
            //Determine what beat and track you clicked on
            int clickedTrack = TrackIndexFromHeight(snapVector.y);
            float clickedBeat = BeatNumberFromX(snapVector.x);
            Debug.Log("Clicked on track: " + clickedTrack + ", beat " + clickedBeat.ToString());
            SongInfo.Note noteToAdd = new SongInfo.Note();
            noteToAdd.note = clickedBeat;
            //Add arrow to the songInfo at that point
            //Check the specific track of that song to see where it fits
            if (songInfo.tracks[clickedTrack].notes.Count == 0)
            {
                Debug.Log("Adding to empty track");
                songInfo.tracks[clickedTrack].notes.Add(noteToAdd);
                UpdateSongPosition();
            }
            else
            {
                Debug.Log("Adding to populated track");
                bool noteAdded = false;
                for (int i = 0; i < songInfo.tracks[clickedTrack].notes.Count; i++)
                {
                    Debug.Log("Checking note: " + i.ToString());
                    if (songInfo.tracks[clickedTrack].notes[i].note == clickedBeat)
                    {
                        Debug.Log("Already a note there!");
                        noteAdded = true;
                    }
                    else if (songInfo.tracks[clickedTrack].notes[i].note > clickedBeat && !noteAdded)
                    {
                        //found note with a beat later than the clicked beat at index i.
                        //Insert the noteToAdd before it
                        Debug.Log("Adding note at index: " + i.ToString());
                        songInfo.tracks[clickedTrack].notes.Insert(i, noteToAdd);
                        noteAdded = true;
                    }
                }
                //If you reached the end of the track without adding a note, this beat must occur after the last note
                if (!noteAdded)
                {
                    Debug.Log("Adding at the end of the track");
                    songInfo.tracks[clickedTrack].notes.Add(noteToAdd);
                }
                UpdateSongPosition();
            }
        }
        else
        {
            Debug.Log("Invalid Click");
        }
    }

    void RemoveArrow()
    {
        if (onTrack)
        {
            //Determine what beat and track you clicked on
            int clickedTrack = TrackIndexFromHeight(snapVector.y);
            float clickedBeat = BeatNumberFromX(snapVector.x);
            Debug.Log("Clicked on track: " + clickedTrack + ", beat " + clickedBeat.ToString());
            SongInfo.Note noteToRemove = new SongInfo.Note();
            noteToRemove.note = clickedBeat;
            //Add arrow to the songInfo at that point
            //Check the specific track of that song to see where it fits
            if (songInfo.tracks[clickedTrack].notes.Count == 0)
            {
                Debug.Log("No arrows on that track");
                UpdateSongPosition();
            }
            else
            {
                Debug.Log("Checking populated track");
                bool noteRemoved = false;
                for (int i = 0; i < songInfo.tracks[clickedTrack].notes.Count; i++)
                {
                    Debug.Log("Checking note: " + i.ToString());
                    if (songInfo.tracks[clickedTrack].notes[i].note == clickedBeat)
                    {
                        Debug.Log("Found a note!");
                        songInfo.tracks[clickedTrack].notes.RemoveAt(i);
                        noteRemoved = true;
                    }
                }
                //If you reached the end of the track without adding a note, this beat must occur after the last note
                if (!noteRemoved)
                {
                    Debug.Log("Didn't right-click on note");
                }
                UpdateSongPosition();
            }
        }
        else
        {
            Debug.Log("Invalid Right Click");
        }
    }

    void AddFill()
    {
        if (onTrack)
        {
            //Determine what beat and track you clicked on
            int clickedTrack = TrackIndexFromHeight(snapVector.y);
            float clickedBeat = BeatNumberFromX(snapVector.x);
            Debug.Log("Clicked on fill track: " + clickedTrack + ", beat " + clickedBeat.ToString());
            SongInfo.Note fillToAdd = new SongInfo.Note();
            fillToAdd.note = clickedBeat;
            //Add arrow to the songInfo fill track at that point
            //Check the specific track of that song to see where it fits
            if (songInfo.fillTracks[clickedTrack].notes.Count == 0)
            {
                Debug.Log("Adding to empty fill track");
                songInfo.fillTracks[clickedTrack].notes.Add(fillToAdd);
                UpdateSongPosition();
            }
            else
            {
                Debug.Log("Adding to populated fill track");
                bool noteAdded = false;
                for (int i = 0; i < songInfo.fillTracks[clickedTrack].notes.Count; i++)
                {
                    Debug.Log("Checking fill note: " + i.ToString());
                    if (songInfo.fillTracks[clickedTrack].notes[i].note == clickedBeat)
                    {
                        Debug.Log("Already a fill note there!");
                        noteAdded = true;
                    }
                    else if (songInfo.fillTracks[clickedTrack].notes[i].note > clickedBeat && !noteAdded)
                    {
                        //found note with a beat later than the clicked beat at index i.
                        //Insert the noteToAdd before it
                        Debug.Log("Adding fill note at index: " + i.ToString());
                        songInfo.fillTracks[clickedTrack].notes.Insert(i, fillToAdd);
                        noteAdded = true;
                    }
                }
                //If you reached the end of the track without adding a note, this beat must occur after the last note
                if (!noteAdded)
                {
                    Debug.Log("Adding at the end of the fill track");
                    songInfo.fillTracks[clickedTrack].notes.Add(fillToAdd);
                }
                UpdateSongPosition();
            }
        }
        else
        {
            Debug.Log("Invalid Click");
        }
    }

    void RemoveFill()
    {
        if (onTrack)
        {
            //Determine what beat and track you clicked on
            int clickedTrack = TrackIndexFromHeight(snapVector.y);
            float clickedBeat = BeatNumberFromX(snapVector.x);
            Debug.Log("Clicked on fill track: " + clickedTrack + ", beat " + clickedBeat.ToString());
            SongInfo.Note fillToRemove = new SongInfo.Note();
            fillToRemove.note = clickedBeat;

            //Add arrow to the songInfo at that point
            //Check the specific fill track of that song to see where it fits
            if (songInfo.fillTracks[clickedTrack].notes.Count == 0)
            {
                Debug.Log("No arrows on that fill track");
                UpdateSongPosition();
            }
            else
            {
                Debug.Log("Checking populated fill track");
                bool noteRemoved = false;
                for (int i = 0; i < songInfo.fillTracks[clickedTrack].notes.Count; i++)
                {
                    Debug.Log("Checking fill note: " + i.ToString());
                    if (songInfo.fillTracks[clickedTrack].notes[i].note == clickedBeat)
                    {
                        Debug.Log("Found a fill note!");
                        songInfo.fillTracks[clickedTrack].notes.RemoveAt(i);
                        noteRemoved = true;
                    }
                }
                //If you reached the end of the fill track without adding a note, this beat must occur after the last note
                if (!noteRemoved)
                {
                    Debug.Log("Didn't right-click on fill note");
                }
                UpdateSongPosition();
            }
        }
        else
        {
            Debug.Log("Invalid Right Click");
        }
    }
    void SnapNotes()
    {
        //Reset onTrack for the update
        onTrack = false;

        //Determine mouse position in game space
        mouseVector = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseVector.z = 0f;
        snapVector = mouseVector;

        //Determine if close to track line
        for (int i = 0; i < trackHeights.Length; i++)
        {
            if (Mathf.Abs(mouseVector.y - trackHeights[i]) < snapAccuracyY)
            {
                snapVector.y = trackHeights[i];
                onTrack = true;
                SetRotation(i, activeCursor);
            }
        }

        //Determine if close to a beat line, but only if you're on a track.
        if (onTrack)
        {
            for (int i = 0; i < numBars*2; i++)
            {
                if (Mathf.Abs(mouseVector.x - (inputController.transform.position.x - beatSpacing * (i + 1)/2)) < snapAccuracyX)
                {
                    snapVector.x = inputController.transform.position.x - beatSpacing * (i + 1)/2;
                    onTrack = true;
                }
            }
        }

        //Make sure you're not behind the buttons
        if (onTrack)
        {
            if (mouseVector.x > inputController.transform.position.x - snapAccuracyX)
            {
                onTrack = false;
            }
        }

        //Snap y to track heights, otherwise don't render it
        if (onTrack)
        {
            activeCursor.transform.position = snapVector;
        }
        else
        {
            activeCursor.transform.position = new Vector3(-10, 0, 0);
        }
    }

    void DrawNotes()
    {
        //First, destroy any existing notes. Helpful for scrolling
        GameObject[] existingArrows = GameObject.FindGameObjectsWithTag("Arrow");
        foreach (GameObject existingArrow in existingArrows)
        {
            Destroy(existingArrow);
        }

        //do on every track
        for (int i=0; i < songInfo.tracks.Length; i++)
        {
            SongInfo.Track currTrack = songInfo.tracks[i];
            //Do on every note on the track for now
            for (int j=0; j < currTrack.notes.Count; j++)
            {
                if (currTrack.notes[j].note >= currentBeat)
                {
                    GameObject dummyArrow = Instantiate(arrowPrefab);
                    Vector2 arrowPosition = new Vector2(
                        inputController.transform.position.x - (currTrack.notes[j].note - currentBeat) * beatSpacing, trackHeights[i]);
                    dummyArrow.transform.position = arrowPosition;
                    SetRotation(i, dummyArrow);
                    //Debug.Log("Arrow placed in track " + i.ToString() + ", beat " + currTrack.notes[j].note.ToString());
                }
            }
        }
    }

    void DrawFills()
    {
        //First, destroy any existing fill notes. Helpful for scrolling
        GameObject[] existingFills = GameObject.FindGameObjectsWithTag("FillArrow");
        foreach (GameObject existingFill in existingFills)
        {
            Destroy(existingFill);
        }

        //do on every track
        for (int i = 0; i < songInfo.fillTracks.Length; i++)
        {
            SongInfo.Track currTrack = songInfo.fillTracks[i];
            //Do on every fill note on the track for now
            for (int j = 0; j < currTrack.notes.Count; j++)
            {
                //Render all notes > currentBeat
                if (currTrack.notes[j].note >= currentBeat)
                {
                    GameObject dummyFill = Instantiate(fillPrefab);
                    Vector2 fillPosition = new Vector2(
                        inputController.transform.position.x - (currTrack.notes[j].note - currentBeat) * beatSpacing, trackHeights[i]);
                    dummyFill.transform.position = fillPosition;
                    SetRotation(i, dummyFill);
                    //Debug.Log("Arrow placed in track " + i.ToString() + ", beat " + currTrack.notes[j].note.ToString());
                }
            }
        }
    }

    void DrawGrid()
    {
        //first, clear out any existing beat bars. Helps as we scroll
        GameObject[] existingGrid = GameObject.FindGameObjectsWithTag("BeatBar");
        foreach (GameObject existingBar in existingGrid)
        {
            Destroy(existingBar);
        }

        //Make new bars at the beat spacing, with beat numbers
        for (int i=0; i<numBars; i++)
        {
            GameObject currBar = Instantiate(beatBars);
            TextMesh barText = currBar.GetComponentInChildren<TextMesh>();
            currBar.transform.position = new Vector3(inputController.transform.position.x - beatSpacing*(i+1), 0);
            barText.text = (currentBeat + (i + 1)).ToString();
        }

        //Make half bars
        for (int i = 0; i < numBars; i++)
        {
            GameObject currBar = Instantiate(halfBars);
            currBar.transform.position = new Vector3(inputController.transform.position.x - (beatSpacing * (i + 1)) - 0.5f, 0);
        }
    }

    private void SetRotation(int trackNum, GameObject arrowToRotate)
    {
        if (trackNum == 0 || trackNum == 1)
        {
            arrowToRotate.transform.rotation = Quaternion.Euler(0, 0, (90 + trackNum * 90));
        }
        else if (trackNum == 2)
        {
            arrowToRotate.transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        else
        {
            arrowToRotate.transform.rotation = Quaternion.Euler(0, 0, 270);
        }
    }

    private void UpdateSongPosition()
    {
        currentBeatText.text = currentBeat.ToString();
        DrawGrid();
        DrawNotes();
        DrawFills();
    }

    private int TrackIndexFromHeight(float height)
    {
        for (int i = 0; i < trackHeights.Length; i++)
        {
            if (height == trackHeights[i])
                return i;
        }
        return 4;
    }

    private float BeatNumberFromX (float xValue)
    {
        for (int i = 0; i< numBars*2; i++)
        {
            if (xValue == inputController.transform.position.x - beatSpacing * (i + 1)/2)
            {
                return (currentBeat + beatSpacing * (i + 1) / 2f);
            }
        }
        Debug.Log("Returning 0 beat. This should never happen!");
        return 0;
    }
}
