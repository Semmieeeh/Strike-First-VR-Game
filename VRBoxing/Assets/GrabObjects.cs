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
    public bool canHarden;
    public Vector3[] handPos = new Vector3[4];
    public float timer;
    int handPosCount = 0;
    public float speed;

    void Start()
    {
        hand = gameObject;
        canHarden = true;
        anim = GetComponent<Animator>();
    }

    public void HardenFist(InputAction.CallbackContext context)
    {
        if (context.started && canHarden == true)
        {
            hardened = true;
            hand.GetComponent<BoxCollider>().isTrigger = false;
            Debug.Log("Hardened");
        }
    }
    public void ReleaseHarden(InputAction.CallbackContext context)
    {
        if (context.canceled && hardened == true)
        {
            hardened = false;
            hand.GetComponent<BoxCollider>().isTrigger = true;
            Debug.Log("Un-hardened");
        }
    }
    

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Fist")
        {
            //canGrab = true;
            grabObject = other.transform.gameObject;
        }
        else if(grabbed != true)
        {
            //canGrab = false;
            grabObject = null;
        }
    }
    private void Update()
    {
        anim.SetBool("Hardened", hardened);
        timer += Time.deltaTime;
        if(timer > 0.1f)
        {
            if(handPosCount < 4)
            {
                handPosCount++;
            }
            timer = 0;
            handPos[3] = handPos[2];
            handPos[2] = handPos[1];
            handPos[1] = handPos[0];
            handPos[0] = transform.position;
        }
        
        if(handPosCount == 4)
        {
            speed = Vector3.Distance(handPos[3], handPos[2]) + Vector3.Distance(handPos[2], handPos[1]) + Vector3.Distance(handPos[1], handPos[0]);
            speed = speed / 3;
            speed = speed * 100;

        }




    }
    public IEnumerator ReAppear()
    {
        yield return new WaitForSeconds(3);
        transform.GetChild(0).gameObject.SetActive(true);
        hardened = false;
        canHarden = true;
    }
}
