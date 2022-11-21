using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.InputSystem;



public class VRMovement : MonoBehaviour
{
    
         
    void Start()
    {
       
        
    }

    // Update is called once per frame
    public void Movement(InputAction.CallbackContext context)
    {
        Vector2 moveDir = context.ReadValue<Vector2>();
        //print(moveDir);
        print("Moving");
    }

    public void Look(InputAction.CallbackContext context)
    {
        Vector3 lookDir = context.ReadValue<Vector2>();
        //print(lookDir);
        print("Looking");
    }

    public void HardenFist(InputAction.CallbackContext context)
    {
        float axis = context.ReadValue<float>();
        //print(axis);
        print("Hardened");
    }
    public void ReleaseHarden(InputAction.CallbackContext context)
    {
        float axis = context.ReadValue<float>();
        //print(axis);
        print("UnHardened");
    }
    public void Grab(InputAction.CallbackContext context)
    {
        float axis = context.ReadValue<float>();
        //print(axis);
        print("Grabbed");
    }
    public void ReleaseGrab(InputAction.CallbackContext context)
    {
        float axis = context.ReadValue<float>();
       // print(axis);
        print("Released Grab");
    }

    public void Jump(InputAction.CallbackContext context)
    {
        float axis = context.ReadValue<float>();
      //  print(axis);
        print("Jumped");
    }
}
