using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VesselManager : MonoBehaviour {

    private Animator animator;

    public float vesselLevel;
    public bool isOverCharged;
    public bool isFull;
    public float overChargeLevel;

    // Use this for initialization
    void Start () {
        animator = GetComponent<Animator>();
	}
	
	public void AddMana()
    {
        //Check if addding it will make it full
        if (vesselLevel + BattleManager.instance.vesselIncrement >= BattleManager.instance.vesselCapacity)
        {
            Debug.Log("Vessel filled");
            vesselLevel = BattleManager.instance.vesselCapacity;
            isFull = true;
            animator.SetBool("vesselFull", true);
            animator.SetBool("vesselFilling", false);
        }
        else
        {
            //Not at capcity, so just dump it in
            Debug.Log("Adding " + BattleManager.instance.vesselIncrement.ToString() + " mana to vessel.");
            //If it's going from empty, change the animation
            if (vesselLevel == 0)
            {
                //Add the mana
                vesselLevel += BattleManager.instance.vesselIncrement;
                animator.SetBool("vesselEmpty", false);
                animator.SetBool("vesselFilling", true);
            }
            else
            {
                vesselLevel += BattleManager.instance.vesselIncrement;
            }
        }
    }

    public void AddOverCharge()
    {
        //Told to add overcharge. Only do it if this vessel isn't overcharged
        if (!isOverCharged)
        {
            //Check if you're hitting overcharge
            if (overChargeLevel + BattleManager.instance.overChargeIncrement >= BattleManager.instance.overChargeCapacity)
            {
                //Overcharge!
                overChargeLevel = BattleManager.instance.overChargeCapacity;
                isOverCharged = true;
                animator.SetBool("vesselFull", false);
                animator.SetBool("vesselOverCharged", true);
            }
            else
            {
                overChargeLevel += BattleManager.instance.overChargeIncrement;
            }
        }
    }

    public void EmptyMana()
    {
        vesselLevel = 0;
        overChargeLevel = 0;
        isOverCharged = false;
        isFull = false;
        animator.SetBool("vesselEmpty", true);
        animator.SetBool("vesselFilling", false);
        animator.SetBool("vesselFull", false);
        animator.SetBool("vesselOverCharged", false);
    }
}
