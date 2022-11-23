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
    void Start()
    {
        hand = gameObject;        
    }

    
    public void Grab(InputAction.CallbackContext context)
    {

        if (context.started)
        {
            Debug.Log("Tried to grab");
            if (canGrab && grabObject != null)
            {
                grabObject.transform.position = hand.transform.position;
                grabbed = true;
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
}
