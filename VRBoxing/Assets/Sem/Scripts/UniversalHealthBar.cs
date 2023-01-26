using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SubsystemsImplementation;
using Photon.Realtime;
using Photon.Pun;

public class UniversalHealthBar : MonoBehaviour
{
    public float regenDuration;
    public float health
    {
        get
        {
            return (float)Server.MyPlayer.CustomProperties[Server.kHealth];
        }
        set 
        {
            var props = Server.MyPlayer.CustomProperties;
            props[Server.kHealth] = value;
            Server.MyPlayer.SetCustomProperties(props);
        }
    }
    public bool dead;
    public float maxHealth;
    public float minHealth;
    public float healResetTime;
    VRMovement vm;
    void Start()
    {
        maxHealth = 1000;
        minHealth = 0;
        health = maxHealth;
        vm = GetComponent<VRMovement>();
    }
    public void Heal()
    {
        if(healResetTime <= 0)
        {
            regenDuration = 3;
            healResetTime = 10;
        }
        else if(healResetTime > 0)
        {
            //you cant heal ui
        }
        
    }
    // Update is called once per frame
    void Update()
    {
        if(regenDuration > 0)
        {
            Server.ApplyHealth(15 * Time.deltaTime);
        }
        regenDuration -= 1*Time.deltaTime;
        healResetTime -= 1 * Time.deltaTime;
    }
}
