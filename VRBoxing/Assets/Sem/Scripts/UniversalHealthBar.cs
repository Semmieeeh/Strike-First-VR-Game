using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SubsystemsImplementation;
using Photon.Realtime;
using Photon.Pun;

public class UniversalHealthBar : MonoBehaviour
{
    public float p1Health,p2Health, regenDuration;
    public float health;
    public bool dead;
    public float maxHealth;
    public float minHealth;
    VRMovement vm;
    void Start()
    {
        
        maxHealth = 100;
        minHealth = 0;
        health = maxHealth;
        vm = GetComponent<VRMovement>();
    }
    public void Heal()
    {
        regenDuration = 3;
        vm.healResetTime = 15;
    }
    // Update is called once per frame
    void Update()
    {
        if (p1Health < 0)
        {
            //Destroy(gameObject);
        }
        if(regenDuration > 0)
        {
            p1Health += 10*Time.deltaTime;
        }
        regenDuration -= 1*Time.deltaTime;
    }
    public void TakeDamage(float amount)
    {
        health -= amount;
    }
}
