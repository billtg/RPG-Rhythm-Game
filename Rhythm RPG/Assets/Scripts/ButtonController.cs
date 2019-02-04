using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonController : MonoBehaviour {

    private SpriteRenderer spriteRenderer;
    public Sprite defaultImage;
    public Sprite pressedImage;
    public int buttonInt;
    public KeyCode keyCode;
    public bool canBeHit = false;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        //Check if it's being hit
        if (Input.GetKeyDown(keyCode) && !canBeHit)
        {
            Debug.Log("Pressed button without arrow");
            BeatScroller.instance.MissHit();
        }
    }

    private void OnTriggerEnter2D()
    {
            canBeHit = true;
    }

    private void OnTriggerExit2D()
    {
            canBeHit = false;
    }
    public void Pressed()
    {
        spriteRenderer.sprite = pressedImage;
        //Debug.Log("Button " + buttonInt + " pressed");
    }

    public void UnPressed()
    {
        spriteRenderer.sprite = defaultImage;
        //Debug.Log("Button " + buttonInt + " released");
    }


}
