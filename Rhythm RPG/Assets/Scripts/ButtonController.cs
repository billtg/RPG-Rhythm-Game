using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonController : MonoBehaviour {

    private SpriteRenderer spriteRenderer;
    public Sprite defaultImage;
    public Sprite pressedImage;
    public int buttonInt;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
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
