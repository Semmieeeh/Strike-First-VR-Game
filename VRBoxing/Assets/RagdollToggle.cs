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
    public bool ragdolling, inPos, isGrounded, gravity, canReturnToIdle;    
    public float knockback, health, maxHealth, minHealth, ragdollTime, maxRagdollTime, minRagdollTime;
    private Vector3[] transformList;
    private Quaternion[] rotationList;
    private Transform originalPos;
    public int posCheck;
    public bool toggle;
    public Animator anim;
    public bool walking;
    public bool onFace;
    public bool reset;
    
    
    public enum EnemyState
    {
        Ragdolling,
        NotRagdolling,
    }
    public EnemyState enemyState;


    void Start()
    {
        ragdollTime = 5;
        gravity = true;
        minHealth = 0;
        
        health = maxHealth;
        canReturnToIdle = false;
        GetRagdoll();
        hips = transform.GetChild(1).gameObject;
        anim = GetComponent<Animator>();

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
        ragdolling = false;
    }            
    void Die()
    {
        Destroy(gameObject);
    }
    //int j;
    //public void BackToIdle()
    //{        
    //    for (int i = 0; i < ragdollLimbs.Length;)
    //    {
    //        inPos = false;
    //        j++;
    //        ragdollLimbs[i].gameObject.transform.position = Vector3.Slerp(ragdollLimbs[i].gameObject.transform.position, transformList[i],Time.deltaTime *100);
    //        ragdollLimbs[i].gameObject.transform.rotation = Quaternion.RotateTowards(ragdollLimbs[i].gameObject.transform.rotation, rotationList[i],Time.deltaTime * 100);
    //        if (ragdollLimbs[i].gameObject.transform.position == transformList[i] && ragdollLimbs[i].gameObject.transform.rotation == rotationList[i])
    //        {                
    //            i++;                
    //        }
    //        if (j > 12000)
    //        {
    //            j = 0;
    //            break;
    //        }            
    //    }
    //    isOn = false;
    //    canReturnToIdle = false;
    //    enemyState = EnemyState.NotRagdolling;
    //}


    public void TakeDamage(float damage)
    {
        health -= damage;
    }
    
    public void Ragdolling()
    {
        reset = false;
        anim.enabled = false;
        onFace = true;
        ragdollTime -= Time.deltaTime;
        ragdolling = true;
        
        
        foreach(Rigidbody rb in ragdollLimbs)
        {
            rb.isKinematic = false;
        }
    }
    public void NotRagdolling()
    {
        if(reset == false)
        {
      
            ragdollTime = 5;
            anim.enabled = true;

            isGrounded = false;
            foreach (Rigidbody rb in ragdollLimbs)
            {
                rb.isKinematic = true;
            }
            reset = true;
            Invoke(nameof(ResetBool), 0.05f);
        }
        
    }
    public void ResetBool()
    {
        onFace = false;
        ragdolling = false;
    }
    private void Update()
    {
        //anim.SetBool("Walking", walking);
        anim.SetBool("Ragdolling", ragdolling);
        anim.SetBool("Face", onFace);
        switch (enemyState)
        {
            case EnemyState.NotRagdolling:
                NotRagdolling();
                toggle = false;
                
                break;
            case EnemyState.Ragdolling:
                Ragdolling();
                toggle = true;

                break;
                
        }

        if(health <= minHealth)
        {
            Invoke(nameof(Die), 5f);
            health = 1;
        }




        if(toggle == true)
        {
            foreach(Rigidbody rb in ragdollLimbs)
            {
                rb.isKinematic = false;
            }
        }
        else
        {
            foreach (Rigidbody rb in ragdollLimbs)
            {
                rb.isKinematic = true;
            }
        }
        if(ragdollTime <= minRagdollTime && isGrounded == true)
        {
            ragdollTime = minRagdollTime;
            enemyState = EnemyState.NotRagdolling;
            
        }
        if (hips.transform.localPosition.y != 0f && ragdolling == false)
        {
            hips.transform.localPosition = new Vector3(hips.transform.localPosition.x, 0f, hips.transform.localPosition.z);
        }
    }
}
