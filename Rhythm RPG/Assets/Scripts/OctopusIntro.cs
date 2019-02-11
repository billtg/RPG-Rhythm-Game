using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OctopusIntro : MonoBehaviour {

    private Animator animator;
    private AnimatorClipInfo[] animatorClipInfo;
    private float startTime;
    private bool stopIntro;
    public float startX;
    public float startY;
    public float finishY;

	// Use this for initialization
	void Start () {
        animator = GetComponent<Animator>();
        animatorClipInfo = animator.GetCurrentAnimatorClipInfo(0);
        Debug.Log("Clip length: " + animatorClipInfo[0].clip.length);
        startTime = Time.time;
	}
	
	// Update is called once per frame
	void Update () {
        if (!stopIntro)
        {
            this.transform.position = Vector2.Lerp(new Vector2(startX, startY), new Vector2(startX, finishY), (Time.time - startTime) / animatorClipInfo[0].clip.length);
        }
        if (this.transform.position.y >= finishY)
        {
            stopIntro = true;
        }
	}
}
