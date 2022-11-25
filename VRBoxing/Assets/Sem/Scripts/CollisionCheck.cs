using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionCheck : MonoBehaviour
{
    public RagdollToggle rt;
    public LayerMask hit;
    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Fist" && rt.isOn == false)
        {
            rt.hit = collision.gameObject;
            rt.RagdollOn();
        }
    }
}
