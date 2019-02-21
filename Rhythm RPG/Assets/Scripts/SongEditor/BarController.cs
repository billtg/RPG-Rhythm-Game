using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarController : MonoBehaviour {
    public float startX;
    public float endX;
    public float removeLineX;
    public float beat;
    public int times;
    public float posY;
    public bool isHalf;
    public TextMesh textMesh;

    private void Awake()
    {
    }

    public void Initialize(float posY, float startX, float endX, float removeLineX, float targetBeat, bool isHalf)
    {
        this.startX = startX;
        this.endX = endX;
        this.beat = targetBeat;
        this.removeLineX = removeLineX;
        this.posY = posY;
        this.isHalf = isHalf;

        //paused = false;

        //set position
        transform.position = new Vector3(startX, posY, 0);

        if (!isHalf)
        {
            textMesh = GetComponentInChildren<TextMesh>();
            //set beat
            textMesh.text = targetBeat.ToString();
        }
        else
        {
            startX -= 1;
        }
    }

    // Update is called once per frame
    void Update ()
    {
        //Move the bar towards the destroy line by interpolating its position between the spawn point and the destroy line
        //based on the song location in beats
        transform.position = Vector2.Lerp(
        new Vector2(startX, posY),
        new Vector2(removeLineX, posY),
        (EditorConductor.instance.beatsShownInAdvance - (beat - EditorConductor.instance.songPosInBeats)) / (EditorConductor.instance.beatsShownInAdvance) * (endX - startX) / (removeLineX - startX));

        //Destroy the bar if it hits the remove line
        if (transform.position.x >= removeLineX)
        {
            Destroy(gameObject);
        }
    }
}
