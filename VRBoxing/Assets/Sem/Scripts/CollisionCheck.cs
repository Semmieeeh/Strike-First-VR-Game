using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionCheck : MonoBehaviour
{
    public RagdollToggle rt;
    public LayerMask hit;
    public GameObject hand, rightHand;
    float knockback;
    bool multiplier;
    public void Start()
    {
        if (gameObject.tag == "Head")
        {
            multiplier = true;
        }
        knockback = 10;
    }
    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Fist" && rt.isOn == false && rt.health <= 0)
        {
            hand = collision.gameObject;
            rt.hit = collision.gameObject;
            rt.limb = gameObject;
            rt.RagdollOn();
            GetComponent<Rigidbody>().AddForce(hand.transform.forward * 20f * hand.GetComponent<GrabObjects>().speed * knockback);
        }

        if (collision.gameObject.tag == "Fist" && rt.isOn == false)
        {
            hand = collision.gameObject;
            StartCoroutine(Cooldown());
            rt.hit = collision.gameObject;
            rt.limb = gameObject;
            if (multiplier == true)
            {
                rt.TakeDamage(hand.GetComponent<GrabObjects>().speed * 2);
            }
            else
            {
                rt.TakeDamage(hand.GetComponent<GrabObjects>().speed);
            }
        }
        if (collision.gameObject.tag == "Fist" && rt.isOn == false && collision.gameObject.GetComponent<GrabObjects>().speed >= 15)
        {
            hand = collision.gameObject;
            StartCoroutine(Cooldown());
            rt.hit = collision.gameObject;
            rt.limb = gameObject;
            if (multiplier == true)
            {
                rt.TakeDamage(hand.GetComponent<GrabObjects>().speed * 2);
            }
            else
            {
                rt.TakeDamage(hand.GetComponent<GrabObjects>().speed);
            }
            rt.RagdollOn();
            GetComponent<Rigidbody>().AddForce(hand.transform.forward * 100f * knockback);
        }



        if (collision.gameObject.tag == "Ground")
        {
            rt.isGrounded = true;
            rt.offsetObject = collision.gameObject;
            
        }
        else
        {
            rt.isGrounded = false;
            rt.offsetObject = null;
            
        }

    }
    public IEnumerator Cooldown()
    {
        if (hand != null)
        {
            hand.GetComponent<GrabObjects>().canHarden = false;
            hand.GetComponent<BoxCollider>().isTrigger = true;
            hand.transform.GetChild(0).gameObject.SetActive(false);
            hand.GetComponent<GrabObjects>().StartCoroutine(hand.GetComponent<GrabObjects>().ReAppear());
            yield return new WaitForSeconds(3);            
            hand.GetComponent<GrabObjects>().hardened = false;
            
            hand.GetComponent<GrabObjects>().canHarden = true;
            hand = null;
        }
        

    }
}
