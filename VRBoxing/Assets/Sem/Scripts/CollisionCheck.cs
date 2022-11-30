using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionCheck : MonoBehaviour
{
    public RagdollToggle rt;
    public LayerMask hit;
    public GameObject leftHand, rightHand;
    float knockback;
    bool multiplier;
    public void Start()
    {
        if(gameObject.tag == "Head")
        {
            multiplier = true;
        }
        knockback = 10;
    }
    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "LeftFist" && rt.isOn == false && rt.health <=0)
        {
            leftHand = collision.gameObject;
            rt.hit = collision.gameObject;
            rt.limb = gameObject;
            rt.RagdollOn();
            GetComponent<Rigidbody>().AddForce(leftHand.transform.forward * 100f * knockback);
        }
        if (collision.gameObject.tag == "RightFist" && rt.isOn == false && rt.health <= 0)
        {
            rightHand = collision.gameObject;
            rt.hit = collision.gameObject;
            rt.limb = gameObject;
            rt.RagdollOn();
            GetComponent<Rigidbody>().AddForce(rightHand.transform.forward * 100f * knockback);
        }




        if (collision.gameObject.tag == "LeftFist" && rt.isOn == false)
        {
            leftHand = collision.gameObject;
            StartCoroutine(Cooldown());
            rt.hit = collision.gameObject;
            rt.limb = gameObject;
            if(multiplier == true)
            {
                rt.TakeDamage(leftHand.GetComponent<GrabObjects>().speed *2);
            }
            else
            {
                rt.TakeDamage(leftHand.GetComponent<GrabObjects>().speed);
            }
        }
        if (collision.gameObject.tag == "RightFist" && rt.isOn == false)
        {
            rightHand = collision.gameObject;
            StartCoroutine(Cooldown());
            rt.hit = collision.gameObject;
            rt.limb = gameObject;
            if (multiplier == true)
            {
                rt.TakeDamage(rightHand.GetComponent<GrabObjects>().speed * 2);
            }
            else
            {
                rt.TakeDamage(rightHand.GetComponent<GrabObjects>().speed);
            }
        }

    }
    public IEnumerator Cooldown()
    {
        if(leftHand!= null)
        {
            leftHand.GetComponent<GrabObjects>().canHarden = false;
            leftHand.GetComponent<BoxCollider>().isTrigger = true;
            leftHand.transform.GetChild(0).gameObject.SetActive(false);
            leftHand.GetComponent<GrabObjects>().StartCoroutine(leftHand.GetComponent<GrabObjects>().ReAppear());
            yield return new WaitForSeconds(3);
            leftHand.transform.GetChild(0).gameObject.SetActive(true);
            leftHand.GetComponent<GrabObjects>().hardened = false;
            leftHand.GetComponent<GrabObjects>().canHarden = true;
            leftHand = null;
        }
        if (rightHand != null)
        {
            rightHand.GetComponent<GrabObjects>().canHarden = false;
            rightHand.GetComponent<BoxCollider>().isTrigger = true;
            rightHand.transform.GetChild(0).gameObject.SetActive(false);
            rightHand.GetComponent<GrabObjects>().StartCoroutine(rightHand.GetComponent<GrabObjects>().ReAppear());
            yield return new WaitForSeconds(3);
            rightHand.transform.GetChild(0).gameObject.SetActive(true);
            rightHand.GetComponent<GrabObjects>().hardened = false;
            rightHand.GetComponent<GrabObjects>().canHarden = true;
            rightHand = null;
        }

    }
}
