using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FillBar : MonoBehaviour {

    public float maxScale;
    public float minScale;

	// Use this for initialization
	void Start () {
        this.transform.localScale = new Vector3(minScale, 0, 0);
	}
	
    public void UpdateScale(float ratio)
    {
        Debug.Log(ratio.ToString());
        if (ratio > 0)
        {
            Debug.Log("Ratio > 0");
            this.transform.localScale = Vector3.Lerp(
                new Vector3(1, minScale, 1),
                new Vector3(1, maxScale, 1),
                ratio);
        }
        else
        {
            Debug.Log("Ratio of 0");
            this.transform.localScale = new Vector3(1, minScale, 1);
        }

        if (ratio == 1)
        {
            this.GetComponent<SpriteRenderer>().color = Color.green;
        }
        else
        {
            this.GetComponent<SpriteRenderer>().color = Color.red;
        }
    }
}
