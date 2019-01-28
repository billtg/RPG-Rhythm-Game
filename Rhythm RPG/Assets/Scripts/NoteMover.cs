using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteMover : MonoBehaviour {

    public GameObject spawnObject;
    public GameObject removeObject;

    public Vector2 spawnPos;
    public Vector2 removePos;
    private float beatsShownInAdvance;
    private float beatOfThisNote;
    private float songPosInBeats;

	// Use this for initialization
	void Start () {

	}

    void Update()
    {
        transform.position = Vector2.Lerp(
            spawnPos,
            removePos,
            (SongManager.instance.beatsShownInAdvance - (beatOfThisNote - SongManager.instance.songPosInBeats)) / SongManager.instance.beatsShownInAdvance
        );
        if (transform.position.y < removeLineY)
        {
            gameObject.SetActive(false);
        }
    }
}
