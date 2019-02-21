using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour {

    public static BattleManager instance;

    //Vessels
    [Header("Vessels")]
    public VesselManager[] vesselManager;
    public KeyCode[] vesselKeys;
    public float vesselCapacity;
    public float vesselIncrement;
    public float overChargeCapacity;
    public float overChargeIncrement;

    //Enemy
    [Header("Enemy")]
    public GameObject enemyObject;
    public GameObject enemyHealthBar;
    public float maxEnemyHealth = 1000;
    public float enemyHealth;
    public float attackIncrement;

    //Audio
    private AudioSource audioSource;

    private void Start()
    {
        instance = this;
        enemyHealth = maxEnemyHealth;
        audioSource = GetComponent<AudioSource>();
    }

    public void AddMana()
    {
        //Check if there's an empty vessel
        //If so, add some mana to it
        //If not, see how close it is to overcharging
        Debug.Log("Attempting to add Mana");
        bool foundEmptyVessel = false;
        int firstEmptyVesselIndex=0;
        for (int i = 0; i < vesselManager.Length; i++)
        {
            if (!vesselManager[i].isFull && !foundEmptyVessel)
            {
                Debug.Log("Found first empty vessel");
                foundEmptyVessel = true;
                firstEmptyVesselIndex = i;
            }
        }

        //Either all full (and possibly charged, or you have the index of the first empty vessel
        if (foundEmptyVessel)
        {
            Debug.Log("Adding mana to first empty vessel, vessel: "+ firstEmptyVesselIndex.ToString());
            vesselManager[firstEmptyVesselIndex].AddMana();
        }
        else
        {
            //Add overcharge to each full vessel
            for (int i = 0; i < vesselManager.Length; i++)
            {
                Debug.Log("Adding overcharge to vessel " + i.ToString());
                vesselManager[i].AddOverCharge();
            }
        }
    }

    public void ClearOverCharge()
    {
        Debug.Log("Clearing overcharged vessels");
        //Check each vessel for overcharge. If it finds one, it clears it.
        for (int i=0; i<vesselManager.Length; i++)
        {
            if (vesselManager[i].isOverCharged)
            {
                vesselManager[i].EmptyMana();
            }
        }
    }

    private void Update()
    {
        for (int i=0; i< vesselKeys.Length; i++)
        {
            if (Input.GetKeyDown(vesselKeys[i]))
            {
                //Check that it's full or overcharged
                if (vesselManager[i].isFull)
                {
                    Debug.Log("Casting spell on vessel: " + i.ToString());
                    //Keep spell in this class.
                    //Make something here that checks which character you just pushed, and respond accordingly.
                    //For now, just drain the mana, and attack the boss
                    vesselManager[i].EmptyMana();
                    enemyHealth -= attackIncrement;
                    enemyHealthBar.transform.localScale = new Vector3(1, enemyHealth/maxEnemyHealth, 1);
                    audioSource.Play();
                }
            }
        }
    }
}
