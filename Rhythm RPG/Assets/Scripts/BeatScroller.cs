using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeatScroller : MonoBehaviour {

    public float beatTempo;

    public bool hasStarted;

	// Use this for initialization
	void Start () {
        beatTempo = beatTempo / 60f;
	}
	
	// Update is called once per frame
	void Update () {
		if (!hasStarted)
        {
            //Press any button to start
            //if (Input.anyKeyDown)
            //{
            //    Debug.Log("Starting!");
            //    hasStarted = true;
            //}
        } else
        {
            MoveNote();
        }
	}

    void MoveNote()
    {
        transform.position -= new Vector3(0f,beatTempo*Time.deltaTime,0f);
    }
}
