using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class CollisionCheck : MonoBehaviour
{
    public RagdollToggle rt;
    public LayerMask hit;
    public GameObject leftHand, rightHand;
    float knockback;
    bool multiplier;
    public Vector3 originalPos;
    public bool canChangeLeftHand;
    public bool canChangeRightHand;
    public void Start()
    {
        canChangeLeftHand = true;
        canChangeRightHand = true;
        if (gameObject.tag == "Head")
        {
            multiplier = true;
        }
        knockback = 10;

        if(gameObject.tag == "RightHand")
        {
            originalPos = transform.position;
        }
        else if (gameObject.tag == "LeftHand")
        {
            originalPos = transform.position;
        }
        else if (gameObject.tag == "Head")
        {
            originalPos = transform.position;
        }
        





    }
    public void OnCollisionEnter(Collision collision)
    {
        if(canChangeLeftHand == true)
        {
            if (collision.gameObject.tag == "LeftFist" && rt.ragdolling == false)
            {
                leftHand = collision.gameObject;
                StartCoroutine(CooldownLeft());
                rt.hit = collision.gameObject;
                rt.limb = gameObject;
                if (multiplier == true)
                {
                    rt.TakeDamage(leftHand.GetComponent<GrabObjects>().speed * 2);
                }
                else
                {
                    rt.TakeDamage(leftHand.GetComponent<GrabObjects>().speed);
                }
            }
            if (collision.gameObject.tag == "LeftFist" && rt.ragdolling == false && collision.gameObject.GetComponent<GrabObjects>().speed >= 10)
            {
                leftHand = collision.gameObject;
                rt.onFace = true;
                StartCoroutine(CooldownLeft());
                rt.hit = collision.gameObject;
                rt.limb = gameObject;
                if (multiplier == true)
                {
                    rt.TakeDamage(leftHand.GetComponent<GrabObjects>().speed * 2);
                }
                else
                {
                    rt.TakeDamage(leftHand.GetComponent<GrabObjects>().speed);
                }
                rt.enemyState = RagdollToggle.EnemyState.Ragdolling;
                GetComponent<Rigidbody>().AddForce(leftHand.transform.forward * 100f * knockback);
            }
        }

        if(canChangeRightHand == true)
        {
            if (collision.gameObject.tag == "RightFist" && rt.ragdolling == false)
            {
                rightHand = collision.gameObject;
                StartCoroutine(CooldownRight());
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
            if (collision.gameObject.tag == "RightFist" && rt.ragdolling == false && collision.gameObject.GetComponent<GrabObjects>().speed >= 10)
            {
                rightHand = collision.gameObject;
                rt.onFace = true;
                StartCoroutine(CooldownRight());
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
                rt.enemyState = RagdollToggle.EnemyState.Ragdolling;
                GetComponent<Rigidbody>().AddForce(leftHand.transform.forward * 100f * knockback);
            }
        }







        if (collision.gameObject.tag == "Ground")
        {
            rt.isGrounded = true;
            
        }
        

    }
    public IEnumerator CooldownLeft()
    {
        if (leftHand != null)
        {
            canChangeLeftHand = false;
            leftHand.GetComponent<GrabObjects>().canHarden = false;
            leftHand.GetComponent<BoxCollider>().isTrigger = true;
            leftHand.transform.GetChild(0).gameObject.SetActive(false);
            leftHand.GetComponent<GrabObjects>().StartCoroutine(leftHand.GetComponent<GrabObjects>().ReAppear());
            yield return new WaitForSeconds(3);
            leftHand.GetComponent<GrabObjects>().canHarden = true;

            leftHand = null;
            canChangeLeftHand = true;
        }
        

    }
    public IEnumerator CooldownRight()
    {
        if (rightHand != null)
        {
            
            canChangeRightHand = false;
            rightHand.GetComponent<GrabObjects>().canHarden = false;
            rightHand.GetComponent<BoxCollider>().isTrigger = true;
            rightHand.GetComponent<GrabObjects>().mesh.SetActive(false);
            rightHand.GetComponent<GrabObjects>().StartCoroutine(rightHand.GetComponent<GrabObjects>().ReAppear());
            yield return new WaitForSeconds(3);
            rightHand.GetComponent<GrabObjects>().canHarden = true;
            
            rightHand = null;
            canChangeRightHand = true;
            
        }


    }
    public void Update()
    {
        
    }
    
}
