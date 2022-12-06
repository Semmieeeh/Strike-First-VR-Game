using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RagdollToggle : MonoBehaviour
{
    public GameObject rig, hips, limb, offsetObject, hit;
    public Collider[] ragdollParts;
    public Rigidbody[] ragdollLimbs;
    public GameObject[] children;   
    public bool isOn, inPos, isGrounded, gravity, canReturnToIdle;    
    public float knockback, health, maxHealth, minHealth, ragdollTime;
    public Vector3[] transformList;
    public Quaternion[] rotationList;
    public Transform originalPos;
    public int posCheck;


    void Start()
    {        
        gravity = true;
        minHealth = 0;
        maxHealth = 100;
        health = maxHealth;
        canReturnToIdle = false;
        GetRagdoll();
        hips = transform.GetChild(1).gameObject;        

    }        
    public void GetRagdoll()
    {
        ragdollParts = rig.GetComponentsInChildren<Collider>();
        ragdollLimbs = rig.GetComponentsInChildren<Rigidbody>();
        foreach(Rigidbody rb in ragdollLimbs)
        {
            rb.isKinematic = true;
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            
        }        
        isOn = false;
    }
    public void RagdollOn()
    {        
        for (int i = 0; i < ragdollLimbs.Length; i++)
        {
            transformList[i] = ragdollLimbs[i].gameObject.transform.position;
            
            rotationList[i] = ragdollLimbs[i].gameObject.transform.rotation;
        }
        foreach (Rigidbody rb in ragdollLimbs)
        {
            rb.isKinematic = false;
            
        }                
        isOn = true;
        canReturnToIdle = false;
        inPos = false;
        Invoke(nameof(RagdollOff), ragdollTime);

    }
    public void RagdollOff()
    {
        if(isGrounded == true)
        {
            foreach (Rigidbody rb in ragdollLimbs)
            {
                rb.isKinematic = true;
            }
            canReturnToIdle = true;
        }
        else
        {
            Destroy(gameObject);
        }
               
    }
    public void RagdollOnDeath()
    {
        
        foreach (Rigidbody rb in ragdollLimbs)
        {
            rb.isKinematic = false;

        }
        isOn = true;
        canReturnToIdle = false;
        inPos = false;
        

    }
    int j;
    void Die()
    {
        Destroy(gameObject);
    }
    public void BackToIdle()
    {        
        for (int i = 0; i < ragdollLimbs.Length;)
        {
            inPos = false;
            j++;
            ragdollLimbs[i].gameObject.transform.position = Vector3.Slerp(ragdollLimbs[i].gameObject.transform.position, transformList[i],Time.deltaTime *100);
            ragdollLimbs[i].gameObject.transform.rotation = Quaternion.RotateTowards(ragdollLimbs[i].gameObject.transform.rotation, rotationList[i],Time.deltaTime * 100);
            if (ragdollLimbs[i].gameObject.transform.position == transformList[i] && ragdollLimbs[i].gameObject.transform.rotation == rotationList[i])
            {                
                i++;                
            }
            if (j > 12000)
            {
                j = 0;
                break;
            }            
        }
        isOn = false;
        canReturnToIdle = false;        
    }


    public void TakeDamage(float damage)
    {
        health -= damage;
    }

    private void Update()
    {
        if (isOn == true && inPos == true)
        {
            RagdollOn();            
        }

        if (gravity == true)
        {
            foreach (Rigidbody rb in ragdollLimbs)
            {
                rb.useGravity = true;
            }
        }
        else if (gravity == false)
        {
            foreach (Rigidbody rb in ragdollLimbs)
            {
                rb.useGravity = false;
            }
        }

        if (canReturnToIdle == true && isOn == true && health>0 && isGrounded)
        {
            BackToIdle();
        }


        if (inPos == false && isOn == false)
        {
            hips.transform.position = new Vector3(hips.transform.position.x, 3.05f, hips.transform.position.z);
            if(hips.transform.position.y == 3.05f)
            {
                inPos = true;
                float height;
                height = 2.5f;
                hips.transform.position = new Vector3(hips.transform.position.x, height, hips.transform.position.z);
            }
        }        
        if(health <= minHealth)
        {
            Invoke(nameof(Die), 5f);
            health = 1;
        }
    }
}
