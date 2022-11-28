using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionCheck : MonoBehaviour
{
    public RagdollToggle rt;
    public LayerMask hit;
    public GameObject hand;
    float knockback;
    public void Start()
    {
        knockback = 10;
    }
    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Fist" && rt.isOn == false)
        {
            hand = collision.gameObject;
            rt.hit = collision.gameObject;
            rt.RagdollOn();
            GetComponent<Rigidbody>().AddForce(hand.transform.forward * 100f * knockback);
        }
    }
}
