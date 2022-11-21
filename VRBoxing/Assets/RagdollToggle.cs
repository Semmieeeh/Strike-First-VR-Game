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
    public bool isOn;
   
    void Start()
    {
        GetRagdoll();
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


    public void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Cube")
        {
            RagdollOn();
        }
    }
}
