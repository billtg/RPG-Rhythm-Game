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
   // public bool paused;

    public void Initialize(float posY, float startX, float endX, float removeLineX, float posZ, float targetBeat)
    {
        this.startX = startX;
        this.endX = endX;
        this.beat = targetBeat;
        this.removeLineX = removeLineX;
        this.posY = posY;

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
        transform.position = Vector2.Lerp(
        new Vector2(startX, posY),
        new Vector2(removeLineX, posY),
        (BeatScroller.instance.beatsShownInAdvance - (beat - BeatScroller.instance.songPosInBeats)) / BeatScroller.instance.beatsShownInAdvance
    );
        if (transform.position.x <= removeLineX)
        {
            Destroy(gameObject);
        }
    }
}
