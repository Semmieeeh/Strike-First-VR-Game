using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SubsystemsImplementation;
using Photon.Realtime;

public class UniversalHealthBar : MonoBehaviour
{
    public float health;
    public bool dead;
    public float maxHealth;
    public float minHealth;
    void Start()
    {
        maxHealth = 100;
        minHealth = 0;
        health = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void TakeDamage(float amount)
    {
        health -= amount;
        if (health < 0)
        {
            Destroy(gameObject);
        }

    }
}
