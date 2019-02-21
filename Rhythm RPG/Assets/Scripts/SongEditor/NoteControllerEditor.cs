using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteControllerEditor : MonoBehaviour {

    public float startX;
    public float endX;
    public float removeLineX;
    public float beat;
    public int times;
    public float posY;
    public bool canBeHit;
    public KeyCode keyCode;
    // public bool paused;

    public float perfectThreshold;
    public float goodThreshold;

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
        (EditorConductor.instance.beatsShownInAdvance - (beat - EditorConductor.instance.songPosInBeats)) / (EditorConductor.instance.beatsShownInAdvance) * (endX-startX) / (removeLineX-startX));

        //Destroy the note if it hits the remove line
        if (transform.position.x >= removeLineX)
        {
            MissHit();
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

            //EditorConductor.instance.MissHit();
        }
    }

    private void NoteHit()
    {
        Debug.Log("Arrow hit!");
        //Check accuracy. Current only checks one side, but ultimatley it should check the variable for perfect hit line
        float accuracy = Mathf.Abs(this.beat - EditorConductor.instance.songPosInBeats);
        Debug.Log(accuracy.ToString());
        if (accuracy < perfectThreshold)
        {
            PerfectHit();
        } else if (accuracy < goodThreshold)
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
        EditorConductor.instance.NoteHit(100, "Perfect!");
        Destroy(gameObject);

        //gameObject.SetActive(false);
    }

    public void GoodHit()
    {
        //Score it
        EditorConductor.instance.NoteHit(50, "Good!");
        Destroy(gameObject);
        //gameObject.SetActive(false);
    }

    public void BadHit()
    {
        //Score it
        EditorConductor.instance.NoteHit(10, "Bad!");
        Destroy(gameObject);
        //gameObject.SetActive(false);
    }

    public void MissHit()
    {
        //Boo!
        EditorConductor.instance.MissHit();
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
