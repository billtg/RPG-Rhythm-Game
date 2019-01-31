using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteController : MonoBehaviour {

    public float startX;
    public float endX;
    public float removeLineX;
    public float beat;
    public int times;
    public float posY;
    public bool canBeHit;
    public KeyCode keyCode;
    // public bool paused;

    public void Initialize(float posY, float startX, float endX, float removeLineX, float posZ, float targetBeat, KeyCode keyCode, int track)
    {
        this.startX = startX;
        this.endX = endX;
        this.beat = targetBeat;
        this.removeLineX = removeLineX;
        this.posY = posY;
        this.keyCode = keyCode;
        SetRotation(track);

        //paused = false;

        //set position
        transform.position = new Vector3(startX, posY, posZ);


        //randomize background
        //GetComponent<SpriteRenderer>().sprite = backgroundSprites[UnityEngine.Random.Range(0, backgroundSprites.Length)];

        //set times
        //if (times > 0)
        //{
        //    timesText.text = times.ToString();
        //    timesTextBackground.SetActive(true);
        //}
        //else
        //{
        //    timesTextBackground.SetActive(false);

        //    //randomize rotation
        //    //transform.rotation = Quaternion.Euler(0f, 0f, UnityEngine.Random.Range(0f, 359f));
        //}

    }
	
	// Update is called once per frame
	void Update () {

        //Check if it's being hit
        if (Input.GetKeyDown(keyCode) && canBeHit)
        {
            NoteHit();
        }

        //Move the note towards the destroy line by interpolating its position between the spawn point and the destroy line
        //based on the song location in beats
        transform.position = Vector2.Lerp(
        new Vector2(startX, posY),
        new Vector2(removeLineX, posY),
        (BeatScroller.instance.beatsShownInAdvance - (beat - BeatScroller.instance.songPosInBeats)) / BeatScroller.instance.beatsShownInAdvance);

        //Ddestroy the note if it hits the remove line
        if (transform.position.x <= removeLineX)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Activator")
        {
            canBeHit = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Activator")
        {
            canBeHit = false;

            //BeatScroller.instance.MissHit();
        }
    }

    private void NoteHit()
    {
        Debug.Log("Arrow hit!");
        //Check accuracy. Current only checks one side, but ultimatley it should check the variable for perfect hit line
        float accuracy = Mathf.Abs(this.transform.position.x + 10);
        if (accuracy < 0.25)
        {
            PerfectHit();
        } else if (accuracy < 0.5)
        {
            GoodHit();
        } else
        {
            BadHit();
        }
    }


    public void PerfectHit()
    {
        //Score it
        BeatScroller.instance.NoteHit(100, "Perfect!");
        Destroy(gameObject);

        //gameObject.SetActive(false);
    }

    public void GoodHit()
    {
        //Score it
        BeatScroller.instance.NoteHit(50, "Good!");
        Destroy(gameObject);
        //gameObject.SetActive(false);
    }

    public void BadHit()
    {
        //Score it
        BeatScroller.instance.NoteHit(10, "Bad!");
        Destroy(gameObject);
        //gameObject.SetActive(false);
    }

    private void SetRotation(int trackNum)
    {
        if (trackNum == 0 || trackNum == 1)
        {
            this.transform.rotation = Quaternion.Euler(0, 0, (90 + trackNum * 90));
        } else if (trackNum == 2)
        {
            this.transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        else
        {
            this.transform.rotation = Quaternion.Euler(0, 0, 270);
        }
    }
}
