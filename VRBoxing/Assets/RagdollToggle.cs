using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RagdollToggle : MonoBehaviour
{
    public GameObject rig;    
    public Collider[] ragdollParts;
    public Rigidbody[] ragdollLimbs;    
    public GameObject[] children;
    public GameObject hit;
    public bool isOn;
    public GameObject fpsCam;
    public GameObject limb;
    public float knockback;
    public float health;
    public float maxHealth;
    public float minHealth;
   
    void Start()
    {
        minHealth = 0;
        maxHealth = 100;
        health = maxHealth;
        isOn = false;
        GetRagdoll();
        foreach(Rigidbody rb in ragdollLimbs)
        {
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        }
        RagdollOff();
    }
  
    public void GetRagdoll()
    {
        ragdollParts = rig.GetComponentsInChildren<Collider>();
        ragdollLimbs = rig.GetComponentsInChildren<Rigidbody>();
    }
    public void RagdollOn()
    {
        
        foreach (Rigidbody rb in ragdollLimbs)
        {
            rb.isKinematic = false;
            //rb.AddForce(hit.transform.forward * 10f, ForceMode.Impulse);
        }
        isOn = true;
       
    }
    public void RagdollOff()
    {
        
        foreach(Rigidbody rb in ragdollLimbs)
        {
            rb.isKinematic = true;
        }
        
    }
    public void TakeDamage(float damage)
    {
        health -= damage;   
    }

    public void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Cube")
        {
            RagdollOn();
        }
    }
}
