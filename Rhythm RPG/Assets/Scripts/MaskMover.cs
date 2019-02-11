using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaskMover : MonoBehaviour {

    public float forwardDistance;
    public float backwardDistance;
    public float maximumPush;

    public static MaskMover instance;

	// Use this for initialization
	void Start () {
        instance = this;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void MoveForward()
    {
        if (BeatScroller.instance.musicStarted)
        {
            this.transform.Translate(new Vector3(forwardDistance, 0));
            if (this.transform.position.x >= BeatScroller.instance.finishLineX + 1)
            {
                BeatScroller.instance.StopGame(false);
            }
        }
    }

    public void MoveBackward()
    {
        if (BeatScroller.instance.musicStarted)
        {
            if (this.transform.position.x >= maximumPush)
            {
                this.transform.Translate(new Vector3(-backwardDistance, 0));
            }
        }
    }
}
