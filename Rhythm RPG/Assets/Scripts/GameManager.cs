using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    public AudioSource theMusic;
    public bool startPlaying;
    public BeatScroller bs;

    public int currentScore;
    public int scorePerNote = 100;
    public int currentMultiplier;
    public int multiplierTracker;
    public int[] multiplierThresholds;

    public Text scoreText;
    public Text multiText;

    public static GameManager instance;

	// Use this for initialization
	void Start () {
        instance = this;

        currentMultiplier = 1;
        scoreText.text = "Score: 0";
        multiText.text = "Multipler: x1";
    }
	
	// Update is called once per frame
	void Update () {
		if (!startPlaying)
        {
            if (Input.anyKeyDown)
            {
                startPlaying = true;
                bs.hasStarted = true;

                theMusic.Play();
            }
        }
	}

    public void NoteHit()
    {
        Debug.Log("Hit on time");

        currentScore += scorePerNote * currentMultiplier;

        if (currentMultiplier - 1 < multiplierThresholds.Length)
        {
            multiplierTracker++;

            if (multiplierThresholds[currentMultiplier - 1] < multiplierTracker)
            {
                multiplierTracker = 0;
                currentMultiplier++;
            }
        }
        


        scoreText.text = "Score: " + currentScore.ToString();
        multiText.text = "Multipler: x" + currentMultiplier.ToString();
    }

    public void NoteMissed()
    {
        Debug.Log("Missed Note");
        currentMultiplier = 1;
        multiplierTracker = 0;
        multiText.text = "Multipler: x" + currentMultiplier.ToString();
    }
}
