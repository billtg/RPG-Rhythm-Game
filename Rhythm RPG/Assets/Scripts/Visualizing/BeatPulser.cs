using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeatPulser : MonoBehaviour {

    public float flashPower;

    private Material material;
    // Use this for initialization
    void Start()
    {
        material = GetComponent<MeshRenderer>().materials[0];
    }

    // Update is called once per frame
    void Update()
    {
        ChangeSaturation(HowLongToBeat());
    }

    float HowLongToBeat()
    {
        //Check the itneger value of the next beat
        float nextBeat = Mathf.Ceil(BeatScroller.instance.songPosInBeats);
        //Calculate the distance to the next beat (between 1 and 0)
        float distanceToBeat = nextBeat - BeatScroller.instance.songPosInBeats;
        //Return the value
        return distanceToBeat;
    }

    void ChangeSaturation(float beatDistance)
    {
        beatDistance -= 0.5f;
        beatDistance = Mathf.Abs(beatDistance * 2f);
        //beatDistance now between 0 and 1, with 0 representing 0.5 from beat, and 1 representing fully at beat
        //Raise to 10th power to make it sharp.
        float satValue = Mathf.Pow(beatDistance, flashPower);

        material.SetColor("_EmissionColor", new Color(satValue, 0, 0));
    }
}
