using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionCheck : MonoBehaviour
{
    public RagdollToggle rt;
    public void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Cube" && rt.isOn == false)
        {
            rt.RagdollOn();
        }
    }
}
