using Oculus.Interaction;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GrabObjects : MonoBehaviour
{
    public bool canGrab;
    public bool grabbed;
    public GameObject hand;
    public GameObject grabObject;
    Animator anim;
    public bool hardened;
    void Start()
    {
        hand = gameObject;  
        anim = GetComponent<Animator>();
    }

    public void HardenFist(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            hardened = true;
            hand.GetComponent<BoxCollider>().isTrigger = false;
            Debug.Log("Hardened");
        }
    }
    public void ReleaseHarden(InputAction.CallbackContext context)
    {
        if (context.canceled)
        {
            hardened = false;
            hand.GetComponent<BoxCollider>().isTrigger = true;
            Debug.Log("Un-hardened");
        }
    }
    public void Grab(InputAction.CallbackContext context)
    {

        if (context.started)
        {
            Debug.Log("Tried to grab");
            if (canGrab && grabObject != null)
            {
                grabbed = true;
                grabObject.transform.position = hand.transform.position;
                Debug.Log("Grabbed");
            }
        }
    }

    public void ReleaseGrab(InputAction.CallbackContext context)
    {
        if (context.canceled)
        {
            grabbed = false;
            Debug.Log("Released");
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Grab")
        {
            canGrab = true;
            grabObject = other.transform.gameObject;
        }
        else if(grabbed != true)
        {
            canGrab = false;
            grabObject = null;
        }
    }
    private void Update()
    {
        anim.SetBool("Hardened", hardened);
    }
}
