using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OctopusIntro : MonoBehaviour {

    private Animator animator;
    private AnimatorClipInfo[] animatorClipInfo;
    private float startTime;
    private bool stopIntro;

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
            this.transform.position = Vector2.Lerp(new Vector2(3, -2), new Vector2(3, 5), (Time.time - startTime) / animatorClipInfo[0].clip.length);
        }
        if (this.transform.position.y >= 5)
        {
            stopIntro = true;
        }
	}
}
